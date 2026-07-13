import { companies } from "@/data/companies";

export function MarqueeLogos() {
  const row = [...companies, ...companies];
  return (
    <div className="mask-fade-x relative w-full overflow-hidden" aria-hidden>

      <div className="flex w-max animate-marquee items-center gap-14 pr-14 hover:[animation-play-state:paused]">
        {row.map((c, i) => (
          <div
            key={`${c.slug}-${i}`}
            className="group flex h-10 shrink-0 items-center"
            title={c.name}
          >
            {/* eslint-disable-next-line @next/next/no-img-element */}
            <img
              src={`https://cdn.simpleicons.org/${c.slug}/475569`}
              alt={c.name}
              loading="lazy"
              className="h-7 w-auto opacity-50 transition duration-500 ease-smooth group-hover:opacity-100"
              style={{ maxWidth: 132 }}
            />
          </div>
        ))}
      </div>
    </div>
  );
}
