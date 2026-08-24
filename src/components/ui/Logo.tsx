import Link from "next/link";
import { site } from "@/lib/site";

// The horizontal lockup from the brand kit, one file rather than a mark plus
// live text: the wordmark is set uppercase with its own tracking, which page
// text is not (Logo/03-SITE-NOTES.md section 4).
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
        width={1603}
        height={234}
        className="h-[1.4rem] w-auto transition-transform duration-500 ease-smooth group-hover:scale-[1.02] sm:h-6"
      />
    </Link>
  );
}
