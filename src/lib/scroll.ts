// One deterministic scroll path for the whole site. Native hash jumps,
// scrollIntoView and CSS smooth-scrolling behave differently per browser and
// can be cancelled by re-renders - this rAF animator always behaves the same.
let raf = 0;
let abort: AbortController | null = null;

function cancelAnim() {
  if (raf) {
    cancelAnimationFrame(raf);
    raf = 0;
  }
  abort?.abort();
  abort = null;
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
  // Hand control back to the user the moment they scroll themselves, but ride
  // out the trackpad momentum still decaying from the scroll that brought them
  // to the link: a single stray tick used to strand them mid-page.
  abort = new AbortController();
  const { signal } = abort;
  let drift = 0;
  const release = () => cancelAnim();
  const onWheel = (e: WheelEvent) => {
    if (performance.now() - t0 < 120) return;
    drift += Math.abs(e.deltaY);
    if (drift > 24) release();
  };
  window.addEventListener("wheel", onWheel, { passive: true, signal });
  window.addEventListener("touchstart", release, { passive: true, signal });
  const step = (now: number) => {
    const p = Math.min(1, (now - t0) / dur);
    window.scrollTo(0, start + dist * ease(p));
    if (p < 1) {
      raf = requestAnimationFrame(step);
    } else {
      raf = 0;
      abort?.abort();
      abort = null;
    }
  };
  raf = requestAnimationFrame(step);
}

/**
 * Scroll to an element, honoring its CSS scroll-margin-top exactly as the
 * browser's own fragment jump does. The two must agree: a deep link is placed
 * natively before this code runs, and any disagreement shows up as a jump.
 */
export function scrollToId(id: string, instant = false) {
  if (typeof window === "undefined") return;
  const el = document.getElementById(id);
  if (!el) return;
  // Move keyboard focus with the scroll, otherwise "Skip to main content"
  // and every nav anchor leave the tab order where it was.
  if (!el.hasAttribute("tabindex")) el.setAttribute("tabindex", "-1");
  el.focus({ preventScroll: true });
  const smt = parseFloat(getComputedStyle(el).scrollMarginTop || "0") || 0;
  const top = Math.max(0, el.getBoundingClientRect().top + window.scrollY - smt);
  if (instant) {
    cancelAnim();
    window.scrollTo(0, top);
    return;
  }
  animateScrollTo(top);
}

/**
 * Same as scrollToId, plus the URL hash, so the position stays shareable and
 * the back button works. Link handlers use this; plain buttons use scrollToId,
 * which keeps them out of the history stack.
 */
export function navigateToId(id: string, instant = false) {
  if (typeof window === "undefined") return;
  if (!document.getElementById(id)) return;
  if (location.hash !== `#${id}`) history.pushState(null, "", `#${id}`);
  scrollToId(id, instant);
}

/** Select a Services tab (by id) and scroll the Services section into place. */
export function openService(id: string) {
  window.dispatchEvent(new CustomEvent("select-service-tab", { detail: id }));
  // Defer past the re-render the event triggers (accordion/tab switch) - // a scroll started synchronously gets cancelled by that layout change.
  window.setTimeout(() => scrollToId("services"), 120);
}
