import Link from "next/link";
import { site } from "@/lib/site";

export function Logo({ className = "" }: { className?: string }) {
  return (
    <Link
      href="/"
      aria-label={`${site.brand} — home`}
      className={`group inline-flex items-center gap-2.5 ${className}`}
    >
      <span className="relative grid h-9 w-9 place-items-center rounded-xl border border-line-strong bg-surface">
        {/* abstract engineered mark (temporary until final logo art) */}
        <svg width="18" height="18" viewBox="0 0 18 18" fill="none" aria-hidden>
          <path
            d="M9 1.2 16.2 9 9 16.8 1.8 9 9 1.2Z"
            stroke="var(--accent)"
            strokeWidth="1.2"
            opacity="0.5"
          />
          <path
            d="M9 5 13 9l-4 4-4-4 4-4Z"
            fill="var(--accent)"
            className="transition-transform duration-500 ease-smooth group-hover:scale-110"
          />
        </svg>
      </span>
      <span className="text-[17px] font-semibold tracking-tight text-ink">
        <span className="text-accent">AI</span>Vanta
      </span>
    </Link>
  );
}
