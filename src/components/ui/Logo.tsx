import Link from "next/link";
import { site } from "@/lib/site";

// The horizontal lockup from the brand kit, one file rather than a mark plus
// live text: the wordmark is set uppercase with its own tracking, which page
// text is not (Logo/00-docs/03-SITE-NOTES.md section 4).
//
// Sized by height, as the kit requires (section 5.1). The viewBox carries
// 2.7u of clear space around 18u of ink, and the ink must not drop below
// 20px, so the box floor is 20 * 23.4 / 18 = 26px. max() keeps that floor
// when a reader shrinks their browser font, while rem still scales it up.
export function Logo({ className = "" }: { className?: string }) {
  return (
    <Link
      href="/"
      aria-label={`${site.brand} home page`}
      className={`group inline-flex items-center ${className}`}
    >
      {/* eslint-disable-next-line @next/next/no-img-element */}
      <img
        src="/brand/lockup-light.svg"
        alt=""
        aria-hidden
        width={1160}
        height={234}
        className="h-[max(1.625rem,26px)] w-auto transition-transform duration-500 ease-smooth group-hover:scale-[1.02] sm:h-[max(1.75rem,26px)]"
      />
    </Link>
  );
}
