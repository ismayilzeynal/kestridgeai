import { companies } from "@/data/companies";

function LogoRow() {
  return (
    <ul className="flex shrink-0 items-center gap-14 pr-14">
      {companies.map((c) => (
        <li
          key={c.slug}
          className="group flex h-10 shrink-0 items-center"
          title={c.name}
        >
          {/* eslint-disable-next-line @next/next/no-img-element */}
          <img
            src={`https://cdn.simpleicons.org/${c.slug}/475569`}
            alt={c.name}
            loading="lazy"
            className="h-8 w-auto opacity-55 transition duration-500 ease-smooth group-hover:opacity-100"
            style={{ maxWidth: 140 }}
          />
        </li>
      ))}
    </ul>
  );
}

export function MarqueeLogos() {
  return (
    <div className="mask-fade-x relative w-full overflow-hidden" aria-hidden>
      {/* two identical rows; the track translates by exactly one row width */}
      <div className="flex w-max animate-marquee items-center hover:[animation-play-state:paused]">
        <LogoRow />
        <LogoRow />
      </div>
    </div>
  );
}
