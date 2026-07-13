// Fixed navbar is 68px; add a little breathing room above anchored targets.
export const NAV_OFFSET = 84;

export function scrollToId(id: string, offset = NAV_OFFSET) {
  if (typeof window === "undefined") return;
  const el = document.getElementById(id);
  if (!el) return;
  const top = el.getBoundingClientRect().top + window.scrollY - offset;
  window.scrollTo({ top, behavior: "smooth" });
}

/** Select a Services tab (by id) and scroll the Services section into place. */
export function openService(id: string) {
  window.dispatchEvent(new CustomEvent("select-service-tab", { detail: id }));
  scrollToId("services");
}
