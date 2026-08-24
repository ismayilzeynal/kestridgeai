import Link from "next/link";
import { site } from "@/lib/site";

export function Logo({ className = "" }: { className?: string }) {
  return (
    <Link
      href="/"
      aria-label={`${site.brand} home page`}
      className={`group inline-flex items-center gap-2.5 ${className}`}
    >
      {/* eslint-disable-next-line @next/next/no-img-element */}
      <img
        src="/brand/mark.svg"
        alt=""
        aria-hidden
        width={34}
        height={34}
        className="h-8 w-8 transition-transform duration-500 ease-smooth group-hover:scale-[1.05]"
      />
      <span className="text-[1.1765rem] font-bold tracking-[-0.03em] text-ink">
        {site.brand}
      </span>
    </Link>
  );
}
