"use client";

import { useState } from "react";

/**
 * The photo when it loads, the initials when it does not.
 *
 * A client leaf on purpose, so Team.tsx stays a server component. Before this
 * existed, any non-empty photo string rendered an img with no fallback, so a
 * name that no longer matches a file on disk was a visibly broken avatar on the
 * live site. Once the value comes out of a database rather than out of git,
 * that stops being a typo somebody catches in review.
 */
export function TeamPhoto({
  photo,
  name,
  initials,
}: {
  photo: string;
  name: string;
  initials: string;
}) {
  const [failed, setFailed] = useState(false);

  const base =
    "grid h-[4.2353rem] w-[4.2353rem] shrink-0 place-items-center overflow-hidden rounded-full border border-line bg-bg-soft";

  if (!photo || failed) {
    return (
      <span className={base} aria-hidden>
        <span className="font-sans text-[1.1765rem] font-semibold tracking-tight text-muted">
          {initials}
        </span>
      </span>
    );
  }

  return (
    <span className={base}>
      {/* eslint-disable-next-line @next/next/no-img-element */}
      <img
        src={photo}
        alt={name}
        width={72}
        height={72}
        loading="lazy"
        onError={() => setFailed(true)}
        className="h-full w-full object-cover grayscale transition-all duration-700 ease-smooth group-hover:grayscale-0"
      />
    </span>
  );
}
