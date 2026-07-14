import Link from "next/link";
import { ArrowRight } from "lucide-react";

export function LegalHeader({
  title,
  updated,
  intro,
}: {
  title: string;
  updated: string;
  intro: string;
}) {
  return (
    <section className="relative pt-32 pb-4 sm:pt-36">
      <div className="container-x">
        <span className="eyebrow">Legal</span>
        <h1 className="mt-5 max-w-3xl text-balance text-[clamp(2rem,6vw,3rem)] leading-[1.05] text-signal">
          {title}
        </h1>
        <p className="mt-4 font-mono text-[12px] uppercase tracking-label text-faint">
          Last updated: {updated}
        </p>
        <p className="mt-6 max-w-3xl text-pretty text-lg leading-relaxed text-muted">
          {intro}
        </p>
      </div>
    </section>
  );
}

export function LegalBody({ children }: { children: React.ReactNode }) {
  return (
    <section className="relative pb-24 pt-6 sm:pb-32">
      <div className="container-x">
        <div className="legal-prose max-w-3xl">{children}</div>
      </div>
    </section>
  );
}

export function LegalFootnote({
  label,
  href,
  linkText,
}: {
  label: string;
  href: string;
  linkText: string;
}) {
  return (
    <div className="mt-12 flex flex-wrap items-center gap-2 rounded-2xl border border-line bg-surface px-5 py-4 text-sm text-muted">
      <span>{label}</span>
      <Link
        href={href}
        className="inline-flex items-center gap-1.5 font-medium text-accent hover:underline"
      >
        {linkText}
        <ArrowRight className="h-4 w-4" strokeWidth={1.7} />
      </Link>
    </div>
  );
}
