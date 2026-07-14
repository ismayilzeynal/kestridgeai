// Anchor landing is tuned per-section via CSS `scroll-margin-top` so the
// section HEADING (not its padded top edge) stops just under the fixed navbar.
// This helper mirrors the native behavior for JS-driven scrolls.
export function scrollToId(id: string) {
  if (typeof window === "undefined") return;
  const el = document.getElementById(id);
  if (!el) return;
  const smt = parseFloat(getComputedStyle(el).scrollMarginTop || "0") || 0;
  const top = el.getBoundingClientRect().top + window.scrollY - smt;
  window.scrollTo({ top, behavior: "smooth" });
}

/** Select a Services tab (by id) and scroll the Services section into place. */
export function openService(id: string) {
  window.dispatchEvent(new CustomEvent("select-service-tab", { detail: id }));
  scrollToId("services");
}
