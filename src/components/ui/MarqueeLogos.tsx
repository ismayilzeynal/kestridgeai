"use client";

import { useState } from "react";
import type { Company } from "@/data/companies";

// A client component only so a tile can hide itself when its image 404s. The
// file name comes out of the content database now, and a name with no file
// behind it would otherwise be a broken image icon scrolling across the page.
function Logo({ company }: { company: Company }) {
  const [failed, setFailed] = useState(false);

  if (failed) {
    return null;
  }

  return (
    <li className="group flex shrink-0 items-center gap-3">
      {/* eslint-disable-next-line @next/next/no-img-element */}
      <img
        src={`/logos/${company.file}.jpg`}
        alt=""
        width={36}
        height={36}
        loading="lazy"
        onError={() => setFailed(true)}
        className="h-9 w-9 shrink-0 rounded-lg object-contain opacity-85 saturate-[0.85] transition duration-500 ease-smooth group-hover:opacity-100 group-hover:saturate-100"
      />
      <span className="whitespace-nowrap text-[0.8235rem] font-medium text-muted transition-colors duration-500 group-hover:text-ink">
        {company.name}
      </span>
    </li>
  );
}

function LogoRow({ companies }: { companies: Company[] }) {
  return (
    <ul className="flex shrink-0 items-center gap-10 pr-10 sm:gap-14 sm:pr-14">
      {companies.map((c) => (
        <Logo key={c.file} company={c} />
      ))}
    </ul>
  );
}

export function MarqueeLogos({ companies }: { companies: Company[] }) {
  return (
    <>
      {/* The animated track is duplicated four times, so it is hidden from
          assistive tech and the list below carries the names once. */}
      <div className="mask-fade-x relative w-full overflow-hidden" aria-hidden>
        <div className="flex w-max animate-marquee items-center hover:[animation-play-state:paused]">
          <LogoRow companies={companies} />
          <LogoRow companies={companies} />
          <LogoRow companies={companies} />
          <LogoRow companies={companies} />
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
