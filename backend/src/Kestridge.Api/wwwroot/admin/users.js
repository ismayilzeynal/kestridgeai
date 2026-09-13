// The Users tab. No innerHTML here either: the CSP sets
// require-trusted-types-for 'script' for the whole panel, and a display name
// is text that another operator typed.
//
// Dependencies are injected by admin.js rather than imported from it, so the
// two modules do not form a cycle.
let api;
let el;
let clear;
let show;
let when;

export function initUsers(deps) {
  ({ api, el, clear, show, when } = deps);
}

// Both message lines are made by draw(). Every action redraws the whole view
// from the server, so a result has to be written after the redraw, into the
// line the redraw just made, or it is gone before it is read.
let topLine = null;
let formLine = null;

// The last result, kept until the operator has had it on screen. A create can
// take seconds while the mail to the team times out, the nav stays usable
// meanwhile, and coming back to Users redraws the view from scratch. A result
// that lived only in the page would be gone by then, and with it the warning
// that the mail to the team was not sent.
let kept = null;

// Every message the operator can be shown for a server refusal. The server
// answers with a field and a one-word reason; the sentences live here, as they
// do in content.js, so the API carries no display copy.
const REASONS = {
  "username:format": "A username is 2 to 64 characters: lowercase letters, digits, dots, hyphens and"
    + " underscores, starting with a letter or a digit.",
  "username:taken": "That username is already in use, or is already waiting for a first sign-in.",
  "displayName:length": "Enter a display name of at most 64 characters.",
  "displayName:dash": "Long dashes are not allowed in a display name. Use a plain hyphen.",
  "displayName:emoji": "Emoji are not allowed in a display name.",
  "displayName:newline": "A display name cannot contain a line break.",
  "displayName:angle": "The characters < and > are not allowed in a display name.",
  "displayName:control": "There is an invisible control character in the display name.",
  "password:length": "The initial password has to be between 12 and 128 characters long.",
  code: "That code did not match your authenticator, or it was already used. Wait for the next code"
    + " and try again.",
  self: "You cannot do that to your own account.",
  disabled: "That account is disabled, so its authenticator cannot be reset.",
};

const STATUS = {
  active: ["active", "tag good"],
  disabled: ["disabled", "tag danger"],
  reset: ["authenticator reset", "tag warn"],
  no_authenticator: ["no authenticator", "tag warn"],
};

const FIELDS = [
  ["username", "Username", { autocomplete: "off", autocapitalize: "none", autocorrect: "off", spellcheck: "false" },
    "Lowercase letters, digits, dots, hyphens and underscores. This is what they type to sign in,"
    + " and it cannot be changed later."],
  ["displayName", "Display name", { autocomplete: "off" },
    "Their name as the panel shows it. It cannot be changed later either."],
  ["password", "Initial password", { type: "password", autocomplete: "new-password" },
    "At least 12 characters. It only works until they choose their own at the first sign-in."],
  ["repeat", "Repeat password", { type: "password", autocomplete: "new-password" }],
  ["code", "Your authenticator code", { inputmode: "numeric", autocomplete: "one-time-code", maxlength: "6" },
    "The current code from your own authenticator app, so that a session left open on a desk"
    + " cannot create an account on its own."],
];

export async function openUsers() {
  // Only a result that was already shown is dropped. One that arrived while
  // another tab was open is still waiting for this visit.
  if (kept && kept.seen) {
    kept = null;
  }

  show("users");
  await load();
}

// Returns the list it drew, or null when there was none to draw.
async function load() {
  try {
    const d = await api("/users");
    draw(d);
    return d;
  } catch (err) {
    // api() has already shown the sign in form for a lost session.
    if (err.message === "auth") {
      return null;
    }

    const root = document.getElementById("users");
    clear(root);
    root.appendChild(el("h1", "Users"));
    topLine = el("div");
    formLine = null;
    root.appendChild(topLine);
    root.appendChild(el("p", "The list of users could not be loaded. Try again in a minute.", { class: "danger" }));
    put();
    return null;
  }
}

function draw(d) {
  const root = document.getElementById("users");
  clear(root);

  root.appendChild(el("h1", "Users"));
  root.appendChild(el("p", "Everyone listed here has the same access to the whole panel, including this page."));

  topLine = el("div");
  root.appendChild(topLine);

  root.appendChild(el("h2", "Accounts"));
  root.appendChild(accounts(d));

  root.appendChild(el("h2", "Waiting for first sign-in"));
  if (d.pending.length === 0) {
    root.appendChild(el("p", "Nobody is waiting for a first sign-in."));
  } else {
    root.appendChild(pending(d.pending));
  }

  root.appendChild(el("h2", "Create a user"));
  root.appendChild(el("p", "You choose a temporary password and give it to the person yourself. At their first"
    + " sign-in they choose their own and set up an authenticator."));
  root.appendChild(createForm());

  // Under the form rather than at the top of the page, where the operator
  // who just pressed the button is not looking.
  formLine = el("div");
  root.appendChild(formLine);
  put();
}

function table(headings) {
  const t = el("table");
  const head = el("tr");
  for (const h of headings) {
    head.appendChild(el("th", h));
  }

  const thead = el("thead");
  thead.appendChild(head);
  t.appendChild(thead);

  const body = el("tbody");
  t.appendChild(body);
  return { t, body };
}

function accounts(d) {
  const { t, body } = table(["Username", "Name", "Status", "Last sign-in", ""]);

  for (const r of d.accounts) {
    const self = r.id === d.self;
    const tr = el("tr");
    tr.appendChild(el("td", self ? r.username + " (you)" : r.username));
    tr.appendChild(el("td", r.displayName));
    tr.appendChild(statusCell(r));
    tr.appendChild(el("td", r.lastLoginAt ? when(r.lastLoginAt) : "never"));

    // Nothing on your own row: the server refuses both for yourself, and a
    // disabled account can only be re-enabled by a database operator, so
    // neither button would do anything there but fail.
    const td = el("td");
    if (!self && r.status !== "disabled") {
      const reset = el("button", "Reset authenticator");
      reset.addEventListener("click", () => resetAuthenticator(r, reset));

      const off = el("button", "Disable", { class: "danger" });
      off.addEventListener("click", () => disable(r, off));

      // .row, because the cell is narrow and the two buttons wrap, and a
      // wrapped pair needs the gap a text node space cannot give.
      const actions = el("div", null, { class: "row" });
      actions.appendChild(reset);
      actions.appendChild(off);
      td.appendChild(actions);
    }

    tr.appendChild(td);
    body.appendChild(tr);
  }

  return t;
}

function statusCell(r) {
  const td = el("td");
  const [label, cls] = STATUS[r.status] || [r.status, "tag"];
  td.appendChild(el("span", label, { class: cls }));

  // The password lockout ends by itself, so it sits next to the status rather
  // than replacing it. The server sends lockedUntil only while the lock lasts
  // by its own clock; comparing again with this computer's clock could only
  // hide a real lock when that clock runs ahead.
  if (r.lockedUntil) {
    td.appendChild(locked(r.lockedUntil));
  }

  if (r.status === "reset" && r.resetExpiresAt) {
    td.appendChild(document.createTextNode("until " + when(r.resetExpiresAt)));
  }

  return td;
}

function locked(until) {
  return el("span", "locked", { class: "tag danger", title: "Until " + when(until) });
}

function pending(rows) {
  const { t, body } = table(["Username", "Name", "Created by", "Expires", ""]);

  for (const p of rows) {
    const tr = el("tr");
    for (const v of [p.username, p.displayName, p.createdBy]) {
      tr.appendChild(el("td", v));
    }

    // Wrong temporary passwords lock an invitation the way they lock an
    // account, and while it lasts even the right password gets the code step.
    // Nothing else in the row would tell the operator why.
    const expires = el("td", p.expired ? "Expired" : when(p.expiresAt));
    if (p.lockedUntil) {
      expires.appendChild(document.createTextNode(" "));
      expires.appendChild(locked(p.lockedUntil));
    }

    tr.appendChild(expires);

    const td = el("td");
    const del = el("button", "Delete", { class: "danger" });
    del.addEventListener("click", () => deletePending(p, del));
    td.appendChild(del);
    tr.appendChild(td);
    body.appendChild(tr);
  }

  return t;
}

async function resetAuthenticator(r, button) {
  const typed = prompt("Reset the authenticator for " + r.displayName + " (" + r.username + ")?\n\n"
    + "Their sessions end now. At their next sign-in they use their current password and set up a new"
    + " authenticator.\n\nType the current code from your own authenticator to confirm:");
  if (typed === null) {
    return;
  }

  const code = typed.trim();
  if (!/^[0-9]{6}$/.test(code)) {
    say("top", [["danger", "Enter the six digit code from your own authenticator app."]]);
    return;
  }

  button.disabled = true;
  try {
    const result = await api("/users/" + r.id + "/reset-authenticator", { code });
    const d = await load();
    const row = d && d.accounts.find((a) => a.id === r.id);
    const lines = [["good", "The authenticator for " + r.displayName + " was reset and their sessions have ended."
      + " They set up a new one at their next sign-in"
      + (row && row.resetExpiresAt ? ", which has to happen before " + when(row.resetExpiresAt) : "") + "."]];
    say("top", notice(lines, result));
  } catch (err) {
    button.disabled = false;
    await refused("top", err, "The authenticator was not reset.");
  }
}

async function disable(r, button) {
  if (!confirm("Disable " + r.displayName + " (" + r.username + ")?\n\n"
    + "Their sessions end now and they cannot sign in again. Only a database operator can re-enable"
    + " an account; the panel cannot.")) {
    return;
  }

  button.disabled = true;
  try {
    await api("/users/" + r.id + "/disable", {});
    await load();
    say("top", [["good", r.displayName + " is disabled and has been signed out."]]);
  } catch (err) {
    button.disabled = false;
    await refused("top", err, "The account was not disabled.");
  }
}

async function deletePending(p, button) {
  if (!confirm("Delete the new account for " + p.displayName + " (" + p.username + ")?\n\n"
    + "The password they were given stops working, and the username is free to use again.")) {
    return;
  }

  button.disabled = true;
  try {
    await api("/users/pending/" + p.id + "/delete", {});
    await load();
    say("top", [["good", "The new account for " + p.displayName + " was deleted."]]);
  } catch (err) {
    button.disabled = false;
    await refused("top", err, "The new account was not deleted.");
  }
}

function createForm() {
  const form = el("form", null, { class: "form" });
  const inputs = {};

  for (const [key, label, attrs, hint] of FIELDS) {
    const block = el("div", null, { class: "field" });
    block.appendChild(el("label", label, { for: "u-" + key }));
    inputs[key] = el("input", null, { id: "u-" + key, ...attrs });
    block.appendChild(inputs[key]);
    if (hint) {
      block.appendChild(el("p", hint, { class: "hint" }));
    }

    form.appendChild(block);
  }

  const submit = el("button", "Create user", { type: "submit" });
  form.appendChild(submit);

  form.addEventListener("submit", async (e) => {
    e.preventDefault();

    const v = {
      username: inputs.username.value.trim().toLowerCase(),
      displayName: inputs.displayName.value.trim(),
      password: inputs.password.value,
      repeat: inputs.repeat.value,
      code: inputs.code.value.trim(),
    };

    const problem = firstProblem(v);
    if (problem) {
      say("form", [["danger", problem[1]]]);
      inputs[problem[0]].focus();
      return;
    }

    submit.disabled = true;
    try {
      const result = await api("/users/create",
        { username: v.username, displayName: v.displayName, password: v.password, code: v.code });

      // Redraw first, which also empties the form, so neither copy of the
      // password stays in the page.
      await load();
      const p = result.pending;
      say("form", notice([["good", "The account for " + p.username + " is waiting for first sign-in until "
        + when(p.expiresAt) + ". Give " + p.username + " the password directly, not by email. At first"
        + " sign-in they choose their own password and set up an authenticator."]], result));
    } catch (err) {
      submit.disabled = false;
      const detail = err.data || {};

      // The server checks the three fields before the code, so a refusal for
      // one of them left the code unused. Anything later, "taken" included, may
      // already have spent it, and the same code would only be refused again.
      const early = (detail.field === "username" && detail.reason === "format")
        || detail.field === "displayName" || detail.field === "password";
      if (!early) {
        inputs.code.value = "";
      }

      if (inputs[detail.field]) {
        inputs[detail.field].focus();
      }

      await refused("form", err, "The user was not created.");
    }
  });

  return form;
}

// The same rules the server applies, in the same order, so the operator gets
// the sentence at the field without a round trip. The repeat is the one check
// that exists only here.
function firstProblem(v) {
  if (!/^[a-z0-9][a-z0-9._-]{1,63}$/.test(v.username)) {
    return ["username", REASONS["username:format"]];
  }

  const name = v.displayName;
  const nameRules = [
    [/[\r\n]/, "newline"],
    [/[\u2012-\u2015]/, "dash"],
    [/[<>]/, "angle"],
    [/[\u{1F300}-\u{1FAFF}\u{2600}-\u{27BF}\u{FE0F}]/u, "emoji"],
    [/[\u0000-\u001F\u007F-\u009F]/, "control"],
  ];
  for (const [pattern, reason] of nameRules) {
    if (pattern.test(name)) {
      return ["displayName", REASONS["displayName:" + reason]];
    }
  }

  if (name.length === 0 || name.length > 64) {
    return ["displayName", REASONS["displayName:length"]];
  }

  if (v.password.length < 12 || v.password.length > 128) {
    return ["password", REASONS["password:length"]];
  }

  if (v.password !== v.repeat) {
    return ["repeat", "The two passwords are not the same."];
  }

  if (!/^[0-9]{6}$/.test(v.code)) {
    return ["code", "Enter the six digit code from your own authenticator app."];
  }

  return null;
}

// The mail to the team address is sent after the change is committed and on a
// best effort basis, so the change stands either way. The operator is told
// when it did not go, because then nobody else has heard about it.
function notice(lines, result) {
  if (!result.notified) {
    lines.push(["warn", "The notice mail to the team address was not sent. Tell the team yourself."]);
  }

  return lines;
}

function say(where, lines) {
  kept = { where, lines, seen: false };
  put();
}

// Writes the kept result into its line, but only while the view is on screen.
// A hidden view is redrawn by openUsers() before it is shown, and that redraw
// calls this again. Both lines are emptied first, because a redraw during this
// visit puts the older result back just before an action says something newer.
function put() {
  if (!kept || document.getElementById("users").hidden) {
    return;
  }

  for (const line of [topLine, formLine]) {
    if (line) {
      clear(line);
    }
  }

  const out = (kept.where === "form" && formLine) || topLine;
  if (!out) {
    return;
  }

  for (const [cls, text] of kept.lines) {
    out.appendChild(el("p", text, { class: cls }));
  }

  kept.seen = true;
}

// Every refusal is shown. A 404 means the row went away since the list was
// drawn, so the list is drawn again before saying so.
async function refused(where, err, fallback) {
  if (err.message === "auth") {
    return;
  }

  if (err.status === 404) {
    await load();
    say(where, [["danger", "That entry no longer exists. The list has been reloaded."]]);
    return;
  }

  const detail = err.data || {};

  // The step-up counts wrong codes against the operator's own account, and
  // once that is locked every code is refused as a wrong one. "Wait for the
  // next code" would then keep them retrying for the whole lock, which blocks
  // their own sign-in too. The list says whether it is locked. It is drawn
  // again only when it is, so a plain typo does not empty the create form.
  if (err.status === 400 && detail.field === "code") {
    let d = null;
    try {
      d = await api("/users");
    } catch (reload) {
      // api() has already shown the sign in form for a lost session.
      if (reload.message === "auth") {
        return;
      }
    }

    const me = d && d.accounts.find((a) => a.id === d.self);
    if (me && me.lockedUntil) {
      draw(d);
      say(where, [["danger", "Your own account is locked until " + when(me.lockedUntil) + " after too many"
        + " wrong codes. Signing in again is blocked until then too."]]);
      return;
    }
  }

  const sentence = REASONS[detail.field + ":" + detail.reason] || REASONS[detail.field];
  say(where, [["danger", sentence
    || (err.status === 429 ? "Too many requests from this network. Wait a few minutes and try again." : fallback)]]);
}
