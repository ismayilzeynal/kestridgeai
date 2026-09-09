// No innerHTML anywhere in this file. The CSP sets require-trusted-types-for
// 'script', and submission text arrives from a public form: rendering it as
// markup would be stored XSS on the origin that holds the token.
import { initSite, openSite } from "./content.js";

const $ = (id) => document.getElementById(id);
let token = sessionStorage.getItem("k") || "";
let cursor = null;
let lastPreview = null;

// sessionStorage, not localStorage: the token dies with the tab.
const save = (t) => { token = t; if (t) sessionStorage.setItem("k", t); else sessionStorage.removeItem("k"); };

async function api(path, body, raw) {
  const init = { method: body === undefined ? "GET" : "POST", headers: { Accept: "application/json" } };
  if (token) init.headers.Authorization = "Bearer " + token;
  if (body !== undefined) { init.headers["Content-Type"] = "application/json"; init.body = JSON.stringify(body); }
  const r = await fetch("/api/admin" + path, init);
  if (r.status === 401) { save(""); show("login"); throw new Error("auth"); }
  if (raw) return r;
  const data = await r.json().catch(() => ({ ok: false, error: "parse" }));
  if (!r.ok) throw Object.assign(new Error(data.error || "error"), { data });
  return data;
}

function show(view) {
  for (const id of ["login", "inbox", "detail", "privacy", "jobs", "site"]) $(id).hidden = id !== view;
  $("bar").hidden = view === "login";
}

const el = (tag, text, attrs) => {
  const n = document.createElement(tag);
  if (text !== undefined && text !== null) n.textContent = String(text);
  for (const k in attrs || {}) n.setAttribute(k, attrs[k]);
  return n;
};
const clear = (n) => { while (n.firstChild) n.removeChild(n.firstChild); };
const when = (iso) => iso ? new Date(iso).toISOString().replace("T", " ").slice(0, 16) + " UTC" : "";

$("loginForm").addEventListener("submit", async (e) => {
  e.preventDefault();
  $("loginError").textContent = "";
  const f = new FormData(e.target);
  try {
    const d = await api("/login", { username: f.get("username"), password: f.get("password"), code: f.get("code") });
    save(d.token);
    $("who").textContent = d.displayName;
    e.target.reset();
    openInbox();
  } catch {
    // One message for every failure. The server answers 401 identically for a
    // wrong password, an unknown user and a locked account, so the panel cannot
    // be used to discover which usernames exist.
    $("loginError").textContent = "Sign in failed. Check the password and the current code.";
  }
});

$("logout").addEventListener("click", async () => {
  try { await api("/logout", {}); } catch { /* the token is going away regardless */ }
  save(""); show("login");
});

for (const b of document.querySelectorAll("nav button")) {
  b.addEventListener("click", () => ({ inbox: openInbox, site: openSite, privacy: openPrivacy, jobs: openJobs })[b.dataset.view]());
}

async function openInbox(append) {
  show("inbox");
  const d = await api("/submissions?state=" + encodeURIComponent($("state").value) + (append && cursor ? "&before=" + cursor : ""));
  if (!append) clear($("rows"));
  $("counts").textContent = d.counts.new + " new, " + d.counts.all + " total" + (d.counts.held ? ", " + d.counts.held + " held" : "");
  for (const r of d.rows) $("rows").appendChild(rowFor(r));
  cursor = d.nextBefore;
  $("more").hidden = !cursor;
}

function rowFor(r) {
  const tr = el("tr");
  tr.appendChild(el("td", when(r.createdAt)));
  tr.appendChild(el("td", r.name));
  tr.appendChild(el("td", r.company));
  tr.appendChild(el("td", r.service));
  tr.appendChild(el("td", r.preview));
  const td = el("td");
  if (r.legalHold) td.appendChild(el("span", "hold", { class: "tag" }));
  if (r.handled) td.appendChild(el("span", "handled", { class: "tag" }));
  if (r.notifyState === "failed") td.appendChild(el("span", "mail failed", { class: "tag danger" }));
  const open = el("button", "Open");
  open.addEventListener("click", () => openDetail(r.id));
  td.appendChild(open);
  tr.appendChild(td);
  return tr;
}

$("state").addEventListener("change", () => { cursor = null; openInbox(); });
$("more").addEventListener("click", () => openInbox(true));

$("searchForm").addEventListener("submit", async (e) => {
  e.preventDefault();
  // A POST, not a query string. A GET would write the address in full to the
  // nginx access log, a file outside the retention purge and outside every
  // window DSR-PROCESS.md tells a data subject about.
  const d = await api("/submissions/search", { q: $("q").value, state: $("state").value });
  show("inbox"); clear($("rows"));
  for (const r of d.rows) $("rows").appendChild(rowFor(r));
  cursor = null; $("more").hidden = true;
  $("counts").textContent = d.rows.length + " matching";
});

$("exportBtn").addEventListener("click", async () => {
  const withMessage = confirm("Include the full message text in the export?\n\nOK includes it. Cancel exports contact details only.\n\nThe file is a copy of personal data that lives outside the database.");
  const r = await api("/submissions/export", { state: $("state").value, includeMessage: withMessage }, true);
  const url = URL.createObjectURL(await r.blob());
  const a = el("a", null, { href: url, download: "kestridge-submissions.csv" });
  document.body.appendChild(a); a.click(); a.remove(); URL.revokeObjectURL(url);
});

async function openDetail(id) {
  const { submission: s } = await api("/submissions/" + id);
  show("detail");
  const root = $("detail"); clear(root);

  root.appendChild(el("h1", s.name));
  const dl = el("dl");
  const pairs = [["Received", when(s.createdAt)], ["Email", s.email], ["Company", s.company || "-"],
    ["Phone", s.phone || "-"], ["Service", s.service],
    ["Notification", s.notifyState + (s.notifyError ? ": " + s.notifyError : "")],
    ["Handled", s.handled ? when(s.handledAt) + " by " + s.handledBy : "no"],
    ["Deleted after", s.purgeAfter]];
  for (const [k, v] of pairs) { dl.appendChild(el("dt", k)); dl.appendChild(el("dd", v)); }
  root.appendChild(dl);
  root.appendChild(el("div", s.message, { class: "msg" }));

  const reply = el("a", "Reply by email", { href: "mailto:" + s.email });
  const handled = el("button", s.handled ? "Mark as not handled" : "Mark as handled");
  handled.addEventListener("click", async () => { await api("/submissions/" + id + "/handled", { handled: !s.handled }); openDetail(id); });
  const hold = el("button", s.legalHold ? "Release legal hold" : "Place legal hold");
  hold.addEventListener("click", async () => {
    if (!s.legalHold && !confirm("Place a legal hold?\n\nThis row will survive the 24 month purge and cannot be removed by a data subject request until the hold is released.\n\nRecord the reason outside the database.")) return;
    await api("/submissions/" + id + "/legal-hold", { legalHold: !s.legalHold }); openDetail(id);
  });
  const back = el("button", "Back to inbox");
  back.addEventListener("click", () => openInbox());

  const actions = el("p");
  for (const n of [reply, handled, hold, back]) { actions.appendChild(n); actions.appendChild(document.createTextNode(" ")); }
  root.appendChild(actions);
}

async function openPrivacy() {
  show("privacy");
  const d = await api("/dsr/log");
  clear($("dsrRows"));
  for (const r of d.rows) {
    const tr = el("tr");
    for (const v of [r.receivedOn, r.requestType, r.rowsAffected, r.affectedIds, r.handledBy, r.closedOn || ""]) tr.appendChild(el("td", v));
    $("dsrRows").appendChild(tr);
  }
}

$("dsrForm").addEventListener("submit", async (e) => {
  e.preventDefault();
  const email = $("dsrEmail").value.trim().toLowerCase();
  const p = await api("/dsr/preview", { email });
  lastPreview = p;
  const out = $("dsrResult"); clear(out);

  out.appendChild(el("p", p.rowsFound + " submission(s) from " + p.subject + ". " + p.blockedByLegalHold + " on legal hold. " + p.priorRequests + " earlier request(s)."));
  if (p.rowsFound === 0) {
    out.appendChild(el("p", "Nothing is held for this address. That is a complete and correct answer to give them."));
    return;
  }

  const today = new Date().toISOString().slice(0, 10);

  const access = el("button", "Produce portable copy");
  access.addEventListener("click", async () => {
    const d = await api("/dsr/access", { email, receivedOn: today });
    out.appendChild(el("p", "Copy this into your reply. The request has been logged."));
    out.appendChild(el("pre", d.record, { class: "msg" }));
  });

  const del = el("button", "Delete everything for this address", { class: "danger" });
  del.addEventListener("click", async () => {
    const typed = prompt("This permanently deletes " + (p.rowsFound - p.blockedByLegalHold) + " submission(s). Rows on legal hold are kept.\n\nType the address to confirm:");
    if (typed === null) return;
    try {
      const d = await api("/dsr/delete", {
        email, confirmEmail: typed.trim().toLowerCase(), receivedOn: today,
        requestType: "delete", ids: lastPreview.ids,
      });
      out.appendChild(el("p", "Deleted " + d.deleted + ". Kept " + d.blockedByLegalHold + " on legal hold. Logged as #" + d.dsrLogId + "."));
      openPrivacy();
    } catch (err) {
      // 409 means a submission arrived between the lookup and the click, so the
      // operator would be deleting something they never saw.
      out.appendChild(el("p", err.message === "stale"
        ? "Something arrived for this address since you looked it up. Look it up again."
        : "Refused: " + err.message, { class: "danger" }));
    }
  });

  const actions = el("p");
  actions.appendChild(access);
  actions.appendChild(document.createTextNode(" "));
  actions.appendChild(del);
  out.appendChild(actions);
});

async function openJobs() {
  show("jobs");
  const d = await api("/jobs");
  const root = $("jobs"); clear(root);
  root.appendChild(el("h1", "Health"));
  root.appendChild(el("p", "Notifications: " + d.notify.sent + " sent, " + d.notify.pending + " pending, " + d.notify.failed + " failed."));
  if (d.notify.failed) {
    root.appendChild(el("p", "Failed notifications need a human. See RUNBOOK.md, notifications stuck.", { class: "danger" }));
  }
  root.appendChild(el("h2", "Retention purge"));
  const t = el("table");
  const head = el("tr");
  for (const h of ["Started", "Outcome", "Cutoff", "Rows", "ms"]) head.appendChild(el("th", h));
  t.appendChild(head);
  for (const r of d.retention) {
    const tr = el("tr");
    for (const v of [when(r.startedAt), r.outcome, r.cutoffDate, r.rowsAffected, r.durationMs]) tr.appendChild(el("td", v));
    t.appendChild(tr);
  }
  root.appendChild(t);
}

// Injected rather than imported the other way round, so admin.js and
// content.js do not form an import cycle.
initSite({ api, el, clear, show, when });

// Restore a session across a reload without asking for the code again.
if (token) {
  api("/session").then((d) => { $("who").textContent = d.displayName; openInbox(); }).catch(() => show("login"));
} else {
  show("login");
}
