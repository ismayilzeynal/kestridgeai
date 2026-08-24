"use client";

import { useEffect } from "react";
import { navigateToId, scrollToId } from "@/lib/scroll";

// Global click delegation: every same-page anchor ("#id" or "/#id" while on
// "/") scrolls through the one shared animator, so in-page navigation feels
// identical no matter which link triggered it. Handlers that already called
// preventDefault (e.g. the navbar's menu-aware logic) are left alone.
export function SmoothAnchors() {
  useEffect(() => {
    const onClick = (e: MouseEvent) => {
      if (
        e.defaultPrevented ||
        e.button !== 0 ||
        e.metaKey ||
        e.ctrlKey ||
        e.shiftKey ||
        e.altKey
      )
        return;
      const a = (e.target as Element | null)?.closest?.("a");
      if (!a) return;
      const href = a.getAttribute("href") ?? "";
      const hashIndex = href.indexOf("#");
      if (hashIndex === -1) return;
      const base = href.slice(0, hashIndex);
      if (base && base !== location.pathname) return;
      const id = decodeURIComponent(href.slice(hashIndex + 1));
      if (!id || !document.getElementById(id)) return;
      e.preventDefault();
      navigateToId(id);
    };
    document.addEventListener("click", onClick);
    return () => document.removeEventListener("click", onClick);
  }, []);

  // A hash already in the URL (a shared link, or the /about redirect) was
  // jumped to natively before hydration, using the raw border box. Re-place it
  // through the same math. Twice: once after hydration and once after the last
  // image settles, since a late layout shift moves the target underneath us.
  // Both placements stand down the moment the reader scrolls on their own.
  useEffect(() => {
    const id = decodeURIComponent(location.hash.slice(1));
    if (!id || !document.getElementById(id)) return;
    let owned = true;
    const release = () => {
      owned = false;
    };
    const place = () => owned && scrollToId(id, true);
    const timers = [window.setTimeout(place, 60), window.setTimeout(place, 450)];
    const opts = { passive: true } as const;
    window.addEventListener("wheel", release, opts);
    window.addEventListener("touchstart", release, opts);
    window.addEventListener("keydown", release, opts);
    window.addEventListener("load", place);
    return () => {
      timers.forEach(window.clearTimeout);
      window.removeEventListener("wheel", release);
      window.removeEventListener("touchstart", release);
      window.removeEventListener("keydown", release);
      window.removeEventListener("load", place);
    };
  }, []);
  return null;
}
