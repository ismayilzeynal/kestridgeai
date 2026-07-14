"use client";

import { useEffect } from "react";
import { scrollToId } from "@/lib/scroll";

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
      history.pushState(null, "", `#${id}`);
      scrollToId(id);
    };
    document.addEventListener("click", onClick);
    return () => document.removeEventListener("click", onClick);
  }, []);
  return null;
}
