// One deterministic scroll path for the whole site. Native hash jumps,
// scrollIntoView and CSS smooth-scrolling behave differently per browser and
// can be cancelled by re-renders - this rAF animator always behaves the same.
let raf = 0;

function cancelAnim() {
  if (raf) {
    cancelAnimationFrame(raf);
    raf = 0;
  }
}

function animateScrollTo(target: number) {
  cancelAnim();
  const reduced = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
  if (reduced) {
    window.scrollTo(0, target);
    return;
  }
  const start = window.scrollY;
  const dist = target - start;
  if (Math.abs(dist) < 2) return;
  const dur = Math.min(850, Math.max(400, Math.abs(dist) * 0.22));
  const t0 = performance.now();
  const ease = (t: number) =>
    t < 0.5 ? 4 * t * t * t : 1 - Math.pow(-2 * t + 2, 3) / 2;
  // hand control back to the user the moment they scroll themselves
  const cancel = () => cancelAnim();
  window.addEventListener("wheel", cancel, { once: true, passive: true });
  window.addEventListener("touchstart", cancel, { once: true, passive: true });
  const step = (now: number) => {
    const p = Math.min(1, (now - t0) / dur);
    window.scrollTo(0, start + dist * ease(p));
    raf = p < 1 ? requestAnimationFrame(step) : 0;
  };
  raf = requestAnimationFrame(step);
}

/** Scroll to an element, honoring its CSS scroll-margin-top. */
export function scrollToId(id: string) {
  if (typeof window === "undefined") return;
  const el = document.getElementById(id);
  if (!el) return;
  // Move keyboard focus with the scroll, otherwise "Skip to main content"
  // and every nav anchor leave the tab order where it was.
  if (!el.hasAttribute("tabindex")) el.setAttribute("tabindex", "-1");
  el.focus({ preventScroll: true });
  const smt = parseFloat(getComputedStyle(el).scrollMarginTop || "0") || 0;
  const top = Math.max(0, el.getBoundingClientRect().top + window.scrollY - smt);
  animateScrollTo(top);
}

/** Select a Services tab (by id) and scroll the Services section into place. */
export function openService(id: string) {
  window.dispatchEvent(new CustomEvent("select-service-tab", { detail: id }));
  // Defer past the re-render the event triggers (accordion/tab switch) - // a scroll started synchronously gets cancelled by that layout change.
  window.setTimeout(() => scrollToId("services"), 120);
}
