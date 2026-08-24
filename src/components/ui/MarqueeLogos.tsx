import { companies } from "@/data/companies";

function LogoRow() {
  return (
    <ul className="flex shrink-0 items-center gap-10 pr-10 sm:gap-14 sm:pr-14">
      {companies.map((c) => (
        <li key={c.file} className="group flex shrink-0 items-center gap-3">
          {/* eslint-disable-next-line @next/next/no-img-element */}
          <img
            src={`/logos/${c.file}.jpg`}
            alt=""
            width={36}
            height={36}
            loading="lazy"
            className="h-9 w-9 shrink-0 rounded-lg object-contain opacity-85 saturate-[0.85] transition duration-500 ease-smooth group-hover:opacity-100 group-hover:saturate-100"
          />
          <span className="whitespace-nowrap text-[0.8235rem] font-medium text-muted transition-colors duration-500 group-hover:text-ink">
            {c.name}
          </span>
        </li>
      ))}
    </ul>
  );
}

export function MarqueeLogos() {
  return (
    <>
      {/* The animated track is duplicated four times, so it is hidden from
          assistive tech and the list below carries the names once. */}
      <div className="mask-fade-x relative w-full overflow-hidden" aria-hidden>
        <div className="flex w-max animate-marquee items-center hover:[animation-play-state:paused]">
          <LogoRow />
          <LogoRow />
          <LogoRow />
          <LogoRow />
        </div>
      </div>
      <ul className="sr-only">
        {companies.map((c) => (
          <li key={c.file}>{c.name}</li>
        ))}
      </ul>
    </>
  );
}
