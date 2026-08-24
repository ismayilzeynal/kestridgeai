"use client";

import { useEffect, useRef, useState, type MouseEvent } from "react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { Menu, X, ArrowRight } from "lucide-react";
import { Logo } from "@/components/ui/Logo";
import { site } from "@/lib/site";
import { navigateToId } from "@/lib/scroll";

export function Navbar() {
  const [scrolled, setScrolled] = useState(false);
  const [open, setOpen] = useState(false);
  const toggleRef = useRef<HTMLButtonElement>(null);
  const firstLinkRef = useRef<HTMLAnchorElement>(null);
  const pathname = usePathname();

  useEffect(() => {
    const onScroll = () => setScrolled(window.scrollY > 16);
    onScroll();
    window.addEventListener("scroll", onScroll, { passive: true });
    return () => window.removeEventListener("scroll", onScroll);
  }, []);

  useEffect(() => {
    document.body.style.overflow = open ? "hidden" : "";
    // Everything focusable outside the sheet has to be inert while it claims
    // aria-modal, otherwise Tab walks straight out of the dialog.
    const outside = [
      "main",
      "site-footer",
      "skip-link",
      "nav-brand",
      "mobile-cta",
    ]
      .map((id) => document.getElementById(id))
      .filter(Boolean) as HTMLElement[];
    if (open) {
      outside.forEach((el) => el.setAttribute("inert", ""));
      firstLinkRef.current?.focus();
    } else {
      outside.forEach((el) => el.removeAttribute("inert"));
    }
    const onKey = (e: KeyboardEvent) => {
      if (e.key === "Escape" && open) {
        setOpen(false);
        toggleRef.current?.focus();
      }
    };
    window.addEventListener("keydown", onKey);
    return () => {
      document.body.style.overflow = "";
      outside.forEach((el) => el.removeAttribute("inert"));
      window.removeEventListener("keydown", onKey);
    };
  }, [open]);

  // Same-page hash links: close the menu first, restore body scroll, THEN
  // scroll - a hash jump while the menu holds `overflow:hidden` is silently
  // blocked on iOS. Cross-page links fall through to normal navigation.
  const onNavClick = (e: MouseEvent<HTMLAnchorElement>, href: string) => {
    // Modified clicks belong to the browser: let them open a new tab.
    if (e.button !== 0 || e.metaKey || e.ctrlKey || e.shiftKey || e.altKey) return;
    const hashIndex = href.indexOf("#");
    if (hashIndex === -1) {
      setOpen(false);
      return;
    }
    const base = href.slice(0, hashIndex) || "/";
    const id = href.slice(hashIndex + 1);
    if (pathname === base) {
      e.preventDefault();
      setOpen(false);
      // wait one tick so overflow:hidden is lifted before scrolling
      window.setTimeout(() => navigateToId(id), 60);
    } else {
      setOpen(false);
    }
  };

  return (
    <header className="fixed inset-x-0 top-0 z-50">
      <a
        id="skip-link"
        href="#main"
        className="sr-only rounded-full bg-ink px-5 py-3 text-sm font-semibold text-white focus:not-sr-only focus:absolute focus:left-4 focus:top-3 focus:z-50"
      >
        Skip to main content
      </a>
      <div
        className={`transition-all duration-500 ease-smooth ${
          scrolled
            ? "border-b border-line bg-bg/95 backdrop-blur-sm"
            : "border-b border-transparent bg-transparent"
        }`}
      >
        <nav
          aria-label="Main"
          className="container-x flex h-16 items-center justify-between"
        >
          <span id="nav-brand">
            <Logo />
          </span>

          <div className="hidden items-center gap-9 lg:flex">
            {site.nav.map((n) => (
              <Link
                key={n.href}
                href={n.href}
                onClick={(e) => onNavClick(e, n.href)}
                className="text-sm text-muted transition-colors duration-300 hover:text-ink"
              >
                {n.label}
              </Link>
            ))}
          </div>

          <div className="hidden lg:block">
            <Link
              href="/#contact"
              onClick={(e) => onNavClick(e, "/#contact")}
              className="btn-primary !py-3 !px-5 text-sm group"
            >
              Contact us
              <ArrowRight className="h-4 w-4 transition-transform duration-300 group-hover:translate-x-0.5" />
            </Link>
          </div>

          <button
            ref={toggleRef}
            onClick={() => setOpen((v) => !v)}
            className="grid h-11 w-11 place-items-center rounded-lg border border-line-strong text-ink lg:hidden"
            aria-label={open ? "Close menu" : "Open menu"}
            aria-expanded={open}
            aria-controls="mobile-menu"
          >
            {open ? <X className="h-5 w-5" /> : <Menu className="h-5 w-5" />}
          </button>
        </nav>
      </div>

      {/* Mobile sheet */}
      <div
        id="mobile-menu"
        role="dialog"
        aria-modal="true"
        aria-label="Site menu"
        aria-hidden={!open}
        className={`fixed inset-0 top-16 z-40 origin-top bg-bg transition-all duration-300 lg:hidden ${
          open ? "pointer-events-auto opacity-100" : "pointer-events-none opacity-0"
        }`}
      >
        <div className="container-x flex flex-col gap-1 pt-6">
          {site.nav.map((n, i) => (
            <Link
              key={n.href}
              ref={i === 0 ? firstLinkRef : undefined}
              href={n.href}
              onClick={(e) => onNavClick(e, n.href)}
              tabIndex={open ? 0 : -1}
              className="border-b border-line py-4 text-2xl font-bold tracking-tight text-ink"
            >
              {n.label}
            </Link>
          ))}
          <Link
            href="/#contact"
            onClick={(e) => onNavClick(e, "/#contact")}
            tabIndex={open ? 0 : -1}
            className="btn-primary mt-6 w-full"
          >
            Contact us
            <ArrowRight className="h-4 w-4" />
          </Link>
        </div>
      </div>
    </header>
  );
}
