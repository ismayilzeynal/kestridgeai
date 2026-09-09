// The Website tab. No innerHTML here either: the CSP sets
// require-trusted-types-for 'script' for the whole panel.
//
// Dependencies are injected by admin.js rather than imported from it, so the
// two modules do not form a cycle.
let api;
let el;
let clear;
let show;
let when;

export function initSite(deps) {
  ({ api, el, clear, show, when } = deps);
}

let data = null;
let assets = null;
let editor = null;

// Every message the operator can be shown for a server refusal. The server
// answers with a field and a one-word reason; inventing the sentence here keeps
// the API free of display copy and keeps the copy in one place.
const REASONS = {
  dash: "Long dashes are not allowed anywhere on the site. Use a plain hyphen.",
  emoji: "Emoji are not used anywhere on this site.",
  newline: "Line breaks are not allowed. This text is shown as one paragraph.",
  angle: "The characters < and > are not allowed.",
  control: "There is an invisible control character in this text.",
  length: "This is empty, or longer than the layout can hold.",
  count: "That would break the page layout.",
  asset: "That image or icon is not one the website ships.",
  immutable: "That cannot be created or changed from here.",
};

const SETS = {
  faq: {
    title: "Questions and answers",
    unit: "items",
    anchor: "questions",
    add: "Add a question",
    canDelete: true,
    label: (r) => r.question || "New question",
    blank: () => ({ id: null, question: "", answer: "" }),
    endpoint: "/content/faq",
    fields: [
      { key: "question", label: "Question", max: 200, rec: 64,
        hint: "Past 64 characters the card heading wraps to three lines." },
      { key: "answer", label: "Answer", max: 600, rec: 220, area: true,
        hint: "Past 220 characters the cards in a row stop matching heights." },
    ],
  },
  services: {
    title: "Services",
    unit: "services",
    anchor: "services",
    note: "Services are fixed at four because the page layout is built around four. To add one, ask a developer.",
    label: (r) => r.name,
    endpoint: "/content/services",
    fields: [
      { key: "slug", label: "Identifier", readonly: true,
        hint: "Fixed. This string is also the contact form value and a link target." },
      { key: "name", label: "Name", max: 60, rec: 24,
        hint: "Past 24 characters the tab rail wraps and the four tabs stop matching heights." },
      { key: "tagline", label: "Tagline", max: 120, rec: 60 },
      { key: "cardLabel", label: "Card label", max: 60, rec: 46,
        hint: "Second line of the hero card. Past 46 characters that card gets taller than the other three." },
      { key: "description", label: "Description", max: 400, rec: 240, area: true },
      { key: "iconName", label: "Icon", choices: () => assets.icons.service },
      { key: "highlights", list: true, label: "Highlights", max: 80, rec: 40, min: 3, cap: 6 },
      { key: "steps", steps: true, label: "Project steps" },
    ],
  },
  team: {
    title: "Founders",
    unit: "people",
    anchor: "founders",
    note: "The founders grid is four across. A fifth person would sit alone on his own row, so people cannot be added or removed from here.",
    label: (r) => r.name,
    endpoint: "/content/team",
    fields: [
      { key: "name", label: "Name", max: 80, rec: 28, hint: "Past 28 characters the name wraps under the photo." },
      { key: "initials", label: "Initials", max: 3, rec: 3,
        hint: "One to three capital letters. Shown when there is no photo." },
      { key: "role", label: "Role", max: 60, rec: 20 },
      { key: "focus", label: "Focus", max: 120, rec: 60, hint: "Past 60 characters this runs to two lines." },
      { key: "photo", label: "Photo", choices: () => ["", ...assets.photos],
        hint: "To use a different photo, a developer has to add the image file to the website first." },
    ],
  },
  companies: {
    title: "Client logos",
    unit: "logos",
    anchor: "top",
    note: "Logos are hidden rather than deleted, so the image stays available to put back.",
    addAsset: true,
    label: (r) => r.name + (r.hidden ? " (hidden)" : ""),
    endpoint: "/content/companies",
    fields: [
      { key: "logoFile", label: "Image file", readonly: true },
      { key: "name", label: "Name", max: 80, rec: 34,
        hint: "The marquee never wraps. A long name lengthens the track and visibly slows the loop." },
      { key: "hidden", label: "Hidden from the website", check: true },
    ],
  },
};

export async function openSite() {
  if (!leave()) {
    return;
  }

  show("site");
  data = await api("/content");
  assets = assets || await api("/assets");
  editor = null;
  overview();
}

function overview() {
  const root = document.getElementById("site");
  clear(root);

  root.appendChild(el("h1", "Website content"));
  root.appendChild(el("p", data.lastPublishedAt
    ? "Last changed " + when(data.lastPublishedAt) + ". Changes appear on kestridge.com within a few minutes of saving."
    : "Nothing has been changed yet. The website is showing the copy that ships with it."));

  const table = el("table");
  for (const key of ["faq", "services", "team", "companies"]) {
    const set = SETS[key];
    const rows = data[key];
    const tr = el("tr");
    tr.appendChild(el("td", set.title));
    tr.appendChild(el("td", rows.length + " " + set.unit));
    tr.appendChild(el("td", lastBy(rows)));

    const td = el("td");
    const open = el("button", "Edit");
    open.addEventListener("click", () => openEditor(key));
    td.appendChild(open);
    tr.appendChild(td);
    table.appendChild(tr);
  }

  root.appendChild(table);
}

function lastBy(rows) {
  const changed = rows.filter((r) => r.updatedBy && r.updatedBy !== "seed");
  if (changed.length === 0) {
    return "not changed yet";
  }

  const latest = changed.reduce((a, b) => (a.updatedAt > b.updatedAt ? a : b));
  return "changed by " + latest.updatedBy + ", " + when(latest.updatedAt);
}

function openEditor(key, index) {
  const set = SETS[key];
  editor = { key, set, rows: data[key].map(copy), index: index ?? 0, dirty: false };
  draw();
}

const copy = (row) => JSON.parse(JSON.stringify(row));

function draw() {
  const root = document.getElementById("site");
  clear(root);

  const { set, rows } = editor;

  const back = el("button", "Back to the list");
  back.addEventListener("click", () => { if (leave()) { openSite(); } });
  root.appendChild(back);

  root.appendChild(el("h1", set.title));
  if (set.note) {
    root.appendChild(el("p", set.note));
  }

  // Owned by the root, not by the form. draw() rebuilds the form after every
  // save, so a message written into the form is gone before it is read.
  editor.status = el("div");
  root.appendChild(editor.status);

  const layout = el("div", null, { class: "split" });

  // Left: the list. Up and down buttons as well as drag, because drag alone is
  // unusable on some touchpads and unreachable from the keyboard.
  const list = el("ul", null, { class: "picker" });
  rows.forEach((row, i) => {
    const li = el("li", null, { draggable: "true" });
    if (i === editor.index) {
      li.setAttribute("class", "on");
    }

    const pick = el("button", set.label(row));
    pick.addEventListener("click", () => { if (leave()) { editor.index = i; draw(); } });
    li.appendChild(pick);

    const up = el("button", "Up", { title: "Move up", "aria-label": "Move up" });
    up.disabled = i === 0;
    up.addEventListener("click", () => move(i, i - 1));

    const down = el("button", "Down", { title: "Move down", "aria-label": "Move down" });
    down.disabled = i === rows.length - 1;
    down.addEventListener("click", () => move(i, i + 1));

    li.appendChild(up);
    li.appendChild(down);

    li.addEventListener("dragstart", (e) => { e.dataTransfer.setData("text/plain", String(i)); });
    li.addEventListener("dragover", (e) => e.preventDefault());
    li.addEventListener("drop", (e) => {
      e.preventDefault();
      const from = Number(e.dataTransfer.getData("text/plain"));
      if (Number.isInteger(from) && from !== i) {
        move(from, i);
      }
    });

    list.appendChild(li);
  });

  const left = el("div");
  left.appendChild(list);

  if (set.add) {
    const add = el("button", set.add);
    add.addEventListener("click", () => {
      if (!leave()) {
        return;
      }

      editor.rows.push(set.blank());
      editor.index = editor.rows.length - 1;
      draw();
    });
    left.appendChild(add);
  }

  if (set.addAsset) {
    left.appendChild(addAsset(set));
  }

  layout.appendChild(left);
  layout.appendChild(form());
  root.appendChild(layout);
}

// Offered only when the site actually ships an image nobody is using yet. When
// every logo is in use the control is absent rather than disabled, because
// "add" with nothing to add is not a state worth explaining.
function addAsset(set) {
  const wrap = el("div", null, { class: "field" });
  const used = new Set(editor.rows.map((r) => r.logoFile));
  const spare = assets.logos.filter((file) => !used.has(file));

  if (spare.length === 0) {
    wrap.appendChild(el("p", "Every logo the website ships is already on the page.", { class: "hint" }));
    return wrap;
  }

  wrap.appendChild(el("label", "Add a logo"));

  const pick = el("select");
  for (const file of spare) {
    pick.appendChild(el("option", file, { value: file }));
  }

  const go = el("button", "Add");
  go.addEventListener("click", async () => {
    if (!leave()) {
      return;
    }

    // The file name is the starting point for the display name, not the final
    // one. The editor opens on the new row so it can be corrected immediately,
    // which matters for names like "ey" and "eigroup".
    const file = pick.value;
    const result = await api(set.endpoint + "/add", { logoFile: file, name: titleFor(file) });

    // Reload rather than splice a row in: the server assigned the id and the
    // sort order, and guessing either is how the next reorder becomes a 400.
    await openSite();
    openEditor("companies");
    editor.index = Math.max(0, editor.rows.findIndex((r) => r.id === result.row.id));
    draw();
    banner(result, "Added to the website. Correct the name if it needs one.");
  });

  wrap.appendChild(pick);
  wrap.appendChild(go);
  wrap.appendChild(el("p", "Only images already committed to the website appear here. To use a different one, a developer has to add the file first.", { class: "hint" }));
  return wrap;
}

const titleFor = (file) => file.split("-").map((w) => w.charAt(0).toUpperCase() + w.slice(1)).join(" ");

async function move(from, to) {
  if (!leave()) {
    return;
  }

  const { rows } = editor;
  // A row that has never been saved has no id, and reorder demands the
  // complete set. Save it first.
  if (to < 0 || to >= rows.length || rows.some((r) => r.id === null)) {
    return;
  }

  const [moved] = rows.splice(from, 1);
  rows.splice(to, 0, moved);
  editor.index = to;
  draw();

  // The complete id list, every time. A partial list is a 400, which is what
  // makes this impossible to half apply: there is no ordering the panel can
  // send that leaves two rows sharing a position.
  data[editor.key] = rows.map(copy);

  const result = await api(editor.set.endpoint + "/reorder", { ids: rows.map((r) => r.id) });
  banner(result, "Order saved.");
}

function form() {
  const { set, rows, index } = editor;
  const row = rows[index];
  const wrap = el("div", null, { class: "form" });

  if (!row) {
    return wrap;
  }

  const draft = restore(row);
  const values = {};

  for (const field of set.fields) {
    if (field.steps) {
      values[field.key] = (draft[field.key] || []).map(copy);
      wrap.appendChild(stepsBlock(field, values[field.key], mark));
      continue;
    }

    if (field.list) {
      values[field.key] = [...(draft[field.key] || [])];
      wrap.appendChild(listBlock(field, values, mark));
      continue;
    }

    wrap.appendChild(single(field, draft, values, mark));
  }

  const actions = el("p");
  const save = el("button", "Save");
  save.disabled = true;
  const cancel = el("button", "Cancel");

  function mark() {
    editor.dirty = true;
    save.disabled = false;
    sessionStorage.setItem(key(row), JSON.stringify(values));
  }

  save.addEventListener("click", async () => {
    const blank = firstBlank(set, values);
    if (blank) {
      banner({ ok: false }, blank);
      return;
    }

    save.disabled = true;
    const body = { id: row.id, ...values };

    try {
      const result = await api(set.endpoint + "/save", body);
      Object.assign(row, result.row);
      data[editor.key] = editor.rows.map(copy);
      done(row);

      // Redraw first, then write into the status line the redraw just made.
      draw();
      banner(result, "Saved and published.");
    } catch (err) {
      save.disabled = false;
      const detail = err.data || {};
      banner({ ok: false }, detail.field
        ? detail.field + ": " + (REASONS[detail.reason] || "Refused.")
        : "The change was not saved.");
    }
  });

  cancel.addEventListener("click", () => {
    done(row);
    draw();
  });

  actions.appendChild(save);
  actions.appendChild(document.createTextNode(" "));
  actions.appendChild(cancel);

  if (set.canDelete && row.id !== null) {
    const del = el("button", "Delete", { class: "danger" });
    del.addEventListener("click", async () => {
      if (!confirm("Delete this question from the website?")) {
        return;
      }

      try {
        const result = await api(set.endpoint + "/delete", { id: row.id });
        done(row);
        editor.rows.splice(editor.index, 1);
        editor.index = Math.max(0, editor.index - 1);
        data[editor.key] = editor.rows.map(copy);
        draw();
        banner(result, "Deleted from the website.");
      } catch {
        banner({ ok: false }, "The last question cannot be deleted: it would remove the question and answer section from the page.");
      }
    });
    actions.appendChild(document.createTextNode(" "));
    actions.appendChild(del);
  }

  wrap.appendChild(actions);
  return wrap;
}

function single(field, draft, values, mark) {
  const block = el("div", null, { class: "field" });
  const id = "f-" + field.key;
  block.appendChild(el("label", field.label, { for: id }));

  if (field.readonly) {
    values[field.key] = draft[field.key];
    block.appendChild(el("p", draft[field.key], { class: "fixed" }));
    if (field.hint) {
      block.appendChild(el("p", field.hint, { class: "hint" }));
    }

    return block;
  }

  if (field.check) {
    const box = el("input", null, { type: "checkbox", id });
    box.checked = Boolean(draft[field.key]);
    values[field.key] = box.checked;
    box.addEventListener("change", () => { values[field.key] = box.checked; mark(); });
    block.appendChild(box);
    return block;
  }

  if (field.choices) {
    const select = el("select", null, { id });
    for (const choice of field.choices()) {
      select.appendChild(el("option", choice === "" ? "No photo, show initials" : choice, { value: choice }));
    }

    select.value = draft[field.key] || "";
    values[field.key] = select.value;
    select.addEventListener("change", () => { values[field.key] = select.value; mark(); });
    block.appendChild(select);
    if (field.hint) {
      block.appendChild(el("p", field.hint, { class: "hint" }));
    }

    return block;
  }

  const input = el(field.area ? "textarea" : "input", null, { id });
  input.value = draft[field.key] || "";
  values[field.key] = input.value;

  const counter = el("span", "", { class: "count" });
  const said = el("p", "", { class: "hint" });

  const update = () => {
    values[field.key] = input.value;
    counter.textContent = input.value.length + " / " + field.max;
    counter.setAttribute("class",
      "count" + (input.value.length > field.max ? " danger" : input.value.length > field.rec ? " warn" : ""));
  };

  input.addEventListener("input", () => { update(); mark(); });

  // Normalizing on blur and on paste rather than on every keystroke: rewriting
  // under a moving cursor is how an editor loses a word without noticing.
  const tidy = () => {
    const result = normalize(input.value);
    if (result.value !== input.value) {
      input.value = result.value;
      said.textContent = result.notes.join(" ");
    }

    update();
    mark();
  };

  input.addEventListener("blur", tidy);
  input.addEventListener("paste", () => window.setTimeout(tidy, 0));

  block.appendChild(input);
  block.appendChild(counter);
  if (field.hint) {
    block.appendChild(el("p", field.hint, { class: "hint" }));
  }

  block.appendChild(said);
  update();
  return block;
}

function listBlock(field, values, mark) {
  const block = el("div", null, { class: "field" });
  block.appendChild(el("label", field.label));
  block.appendChild(el("p", "Between " + field.min + " and " + field.cap + " lines.", { class: "hint" }));

  const redraw = () => {
    clear(items);
    values[field.key].forEach((text, i) => {
      const line = el("div", null, { class: "row" });
      const input = el("input");
      input.value = text;
      input.addEventListener("input", () => { values[field.key][i] = input.value; mark(); });
      input.addEventListener("blur", () => {
        const result = normalize(input.value);
        input.value = result.value;
        values[field.key][i] = result.value;
        mark();
      });

      const drop = el("button", "Remove");
      drop.disabled = values[field.key].length <= field.min;
      drop.addEventListener("click", () => { values[field.key].splice(i, 1); mark(); redraw(); });

      line.appendChild(input);
      line.appendChild(drop);
      items.appendChild(line);
    });

    add.disabled = values[field.key].length >= field.cap;
  };

  const items = el("div");
  const add = el("button", "Add a line");
  add.addEventListener("click", () => { values[field.key].push(""); mark(); redraw(); });

  block.appendChild(items);
  block.appendChild(add);
  redraw();
  return block;
}

// Exactly four, always. The desktop stepper is a four column grid with one
// connector line drawn across it, so the count is not a preference.
function stepsBlock(field, steps, mark) {
  const block = el("div", null, { class: "field" });
  block.appendChild(el("label", field.label));
  block.appendChild(el("p", "Four steps, in order. The website numbers them.", { class: "hint" }));

  steps.forEach((step, i) => {
    const card = el("div", null, { class: "step" });
    card.appendChild(el("h3", "Step " + (i + 1)));

    for (const [key, label, max] of [["phase", "Heading", 40], ["summary", "One line", 80], ["what", "Full text", 240]]) {
      const line = el("div", null, { class: "field" });
      line.appendChild(el("label", label));
      const input = el(key === "what" ? "textarea" : "input");
      input.value = step[key] || "";
      input.addEventListener("input", () => { step[key] = input.value; mark(); });
      input.addEventListener("blur", () => {
        const result = normalize(input.value);
        input.value = result.value;
        step[key] = result.value;
        mark();
      });

      line.appendChild(input);
      line.appendChild(el("span", "limit " + max, { class: "count" }));
      card.appendChild(line);
    }

    block.appendChild(card);
  });

  return block;
}

// Returns a whole sentence about the first empty field, or null. Read-only
// fields are skipped: the operator cannot fix those, and a slug is never blank.
// Only a list line can be removed, so only a list line is offered that.
function firstBlank(set, values) {
  for (const field of set.fields) {
    if (field.readonly || field.check || field.choices) {
      continue;
    }

    if (field.list) {
      const empty = (values[field.key] || []).findIndex((t) => !t || !t.trim());
      if (empty >= 0) {
        return field.label + " line " + (empty + 1)
          + " is empty. Fill it in, or remove the line.";
      }

      continue;
    }

    if (field.steps) {
      for (let i = 0; i < (values[field.key] || []).length; i++) {
        const step = values[field.key][i];
        for (const key of ["phase", "summary", "what"]) {
          if (!step[key] || !step[key].trim()) {
            return "Step " + (i + 1) + " has an empty field. All four steps are"
              + " shown on the website, so none of them can be left blank.";
          }
        }
      }

      continue;
    }

    if (!values[field.key] || !String(values[field.key]).trim()) {
      return field.label + " is empty.";
    }
  }

  return null;
}

// Visible, and it always says what it changed. The server refuses all of this
// outright, so silently sending it would produce a rejection the operator
// cannot act on, and silently rewriting it would show them different words
// after the next reload.
function normalize(value) {
  const notes = [];
  let v = value;

  const DASH = /[\u2012\u2013\u2014\u2015]/g;
  const ZERO = /[\u200B-\u200D\u2060\uFEFF]/g;
  const CONTROL = /[\u0000-\u0008\u000B\u000C\u000E-\u001F\u007F-\u009F]/g;

  const dashes = (v.match(DASH) || []).length;
  if (dashes) {
    v = v.replace(DASH, "-");
    notes.push(dashes + " long dash(es) were replaced with plain hyphens.");
  }

  if (ZERO.test(v)) {
    v = v.replace(ZERO, "");
    notes.push("Invisible characters were removed.");
  }

  if (v.indexOf("\u00A0") >= 0) {
    v = v.split("\u00A0").join(" ");
    notes.push("Non-breaking spaces were replaced with ordinary spaces.");
  }

  if (/[\r\n]/.test(v)) {
    notes.push("Line breaks were removed. This text is shown as one paragraph on the website.");
  }

  if (CONTROL.test(v)) {
    v = v.replace(CONTROL, "");
    notes.push("Control characters were removed.");
  }

  // Last, so that a removed line break leaves one space rather than two.
  v = v.replace(/\s+/g, " ").trim();

  return { value: v, notes };
}

// Three states, and the copy never claims more than the API knows. No build
// runs when content is saved, and no CDN is purged, so the panel does not say
// either of those things happened.
function banner(result, message) {
  const out = editor && editor.status;
  if (!out) {
    return;
  }

  clear(out);

  if (!result.ok) {
    out.appendChild(el("p", message, { class: "danger" }));
    return;
  }

  const line = el("p", null, { class: result.published ? "good" : "warn" });
  line.appendChild(document.createTextNode(result.published
    ? message + " "
    : "Saved. The website did not confirm the update. It will pick up the change within 5 minutes. "));

  const link = el("a", "View it on the website",
    { href: "https://kestridge.com/#" + (editor ? editor.set.anchor : "top"), target: "_blank", rel: "noreferrer" });
  line.appendChild(link);
  out.appendChild(line);

  window.setTimeout(() => clear(out), 6000);
}

// Content drafts only. Submission text is never mirrored anywhere.
const key = (row) => "draft:" + editor.key + ":" + (row.id ?? "new");

function restore(row) {
  const saved = sessionStorage.getItem(key(row));
  if (!saved) {
    return row;
  }

  try {
    return { ...row, ...JSON.parse(saved) };
  } catch {
    return row;
  }
}

function done(row) {
  sessionStorage.removeItem(key(row));
  editor.dirty = false;
}

function leave() {
  return !editor || !editor.dirty
    || confirm("This entry has unsaved changes. Leave without saving?");
}

window.addEventListener("beforeunload", (e) => {
  if (editor && editor.dirty) {
    e.preventDefault();
    e.returnValue = "";
  }
});
