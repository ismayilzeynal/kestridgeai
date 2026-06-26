"use client";

import { useEffect, useState } from "react";
import { ArrowRight } from "lucide-react";

// Persistent mobile call-to-action. Appears once the hero is scrolled past and
// hides while the contact form itself is on screen (so it never covers it).
export function MobileCTA() {
  const [pastHero, setPastHero] = useState(false);
  const [contactVisible, setContactVisible] = useState(false);

  useEffect(() => {
    const onScroll = () =>
      setPastHero(window.scrollY > window.innerHeight * 0.7);
    onScroll();
    window.addEventListener("scroll", onScroll, { passive: true });
    return () => window.removeEventListener("scroll", onScroll);
  }, []);

  useEffect(() => {
    const contact = document.getElementById("contact");
    if (!contact) return;
    const io = new IntersectionObserver(
      ([entry]) => setContactVisible(entry.isIntersecting),
      { threshold: 0.04 }
    );
    io.observe(contact);
    return () => io.disconnect();
  }, []);

  const show = pastHero && !contactVisible;

  return (
    <div
      className={`fixed inset-x-0 bottom-0 z-40 px-4 pb-[calc(env(safe-area-inset-bottom)+0.75rem)] pt-3 transition-all duration-300 ease-smooth sm:hidden ${
        show ? "translate-y-0 opacity-100" : "pointer-events-none translate-y-6 opacity-0"
      }`}
    >
      <div className="absolute inset-0 -z-10 bg-gradient-to-t from-bg via-bg/90 to-transparent" />
      <a href="#contact" className="btn-primary group w-full" tabIndex={show ? 0 : -1}>
        Start a project
        <ArrowRight className="h-[18px] w-[18px] transition-transform duration-300 group-hover:translate-x-1" />
      </a>
    </div>
  );
}
