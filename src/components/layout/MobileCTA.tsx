"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { ArrowRight } from "lucide-react";
import { scrollToId } from "@/lib/scroll";

// Persistent mobile call-to-action. Appears once the hero is scrolled past and
// hides while the contact form itself is on screen (so it never covers it).
export function MobileCTA() {
  const [pastHero, setPastHero] = useState(false);
  const [hiddenZone, setHiddenZone] = useState(false);
  const pathname = usePathname();

  useEffect(() => {
    const onScroll = () =>
      setPastHero(window.scrollY > window.innerHeight * 0.7);
    onScroll();
    window.addEventListener("scroll", onScroll, { passive: true });
    return () => window.removeEventListener("scroll", onScroll);
  }, []);

  // Hide while the contact form or the footer is on screen - the pill must
  // never cover the form it points to, nor the footer's last lines.
  useEffect(() => {
    const zones = [
      document.getElementById("contact"),
      document.getElementById("site-footer"),
    ].filter(Boolean) as Element[];
    if (zones.length === 0) return;
    const visible = new Set<Element>();
    const io = new IntersectionObserver(
      (entries) => {
        for (const e of entries) {
          if (e.isIntersecting) visible.add(e.target);
          else visible.delete(e.target);
        }
        setHiddenZone(visible.size > 0);
      },
      { threshold: 0.04 }
    );
    zones.forEach((z) => io.observe(z));
    return () => io.disconnect();
  }, []);

  const show = pastHero && !hiddenZone;

  return (
    <div
      id="mobile-cta"
      className={`fixed inset-x-0 bottom-0 z-40 px-4 pb-[calc(env(safe-area-inset-bottom)+0.75rem)] pt-3 transition-all duration-300 ease-smooth lg:hidden ${
        show ? "translate-y-0 opacity-100" : "pointer-events-none translate-y-6 opacity-0"
      }`}
    >
      <div className="absolute inset-0 -z-10 bg-gradient-to-t from-bg via-bg/90 to-transparent" />
      <Link
        href="/#contact"
        onClick={(e) => {
          if (pathname === "/") {
            e.preventDefault();
            scrollToId("contact");
          }
        }}
        className="btn-primary group w-full"
        tabIndex={show ? 0 : -1}
      >
        Contact us
        <ArrowRight className="h-[18px] w-[18px] transition-transform duration-300 group-hover:translate-x-1" />
      </Link>
    </div>
  );
}
