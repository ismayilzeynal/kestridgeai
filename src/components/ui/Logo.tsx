import { site } from "@/lib/site";

export function Logo({ className = "" }: { className?: string }) {
  return (
    <a
      href="#top"
      aria-label={`${site.brand} — home`}
      className={`group inline-flex items-center gap-2.5 ${className}`}
    >
      <span className="relative grid h-9 w-9 place-items-center rounded-xl border border-line-strong bg-surface">
        {/* abstract engineered mark */}
        <svg width="18" height="18" viewBox="0 0 18 18" fill="none" aria-hidden>
          <path
            d="M9 1.2 16.2 9 9 16.8 1.8 9 9 1.2Z"
            stroke="var(--accent)"
            strokeWidth="1.2"
            opacity="0.55"
          />
          <path
            d="M9 5 13 9l-4 4-4-4 4-4Z"
            fill="var(--accent)"
            className="transition-transform duration-500 ease-smooth group-hover:scale-110"
          />
        </svg>
        <span className="absolute inset-0 rounded-xl opacity-0 transition-opacity duration-500 group-hover:opacity-100 glow-accent" />
      </span>
      <span className="font-mono text-[15px] font-medium tracking-tight text-ink">
        test<span className="text-accent">_</span>logo
      </span>
    </a>
  );
}
