import { team, type Member } from "@/data/team";
import { Reveal } from "@/components/ui/Reveal";

export function Team() {
  return (
    <section
      id="founders"
      className="relative scroll-mt-8 pb-20 pt-16 sm:scroll-mt-4 sm:pb-28 sm:pt-20"
    >
      <div className="container-x">
        <Reveal>
          <h2 className="text-balance text-4xl text-signal sm:text-5xl">
            The four <span className="text-accent-grad">founders</span>
          </h2>
        </Reveal>
        <Reveal delay={0.12}>
          <p className="mt-5 max-w-2xl text-pretty text-lg leading-relaxed text-muted">
            The company has four founders. The same four people run it today.
          </p>
        </Reveal>

        <div className="mt-12 grid grid-cols-1 gap-4 sm:grid-cols-2 lg:mt-14 lg:grid-cols-4">
          {team.map((m, i) => (
            <Reveal key={m.name} delay={(i % 4) * 0.06}>
              <article className="card card-hover group flex h-full flex-col items-start gap-5 rounded-2xl p-6 sm:flex-row sm:items-center lg:flex-col lg:items-start">
                <Portrait member={m} />
                <div>
                  <h3 className="font-sans text-[1rem] font-semibold tracking-tight text-ink">
                    {m.name}
                  </h3>
                  <p className="mt-0.5 text-[0.8235rem] font-medium text-accent">
                    {m.role}
                  </p>
                  <p className="mt-2 text-[0.8235rem] leading-relaxed text-muted">
                    {m.focus}
                  </p>
                </div>
              </article>
            </Reveal>
          ))}
        </div>
      </div>
    </section>
  );
}

/** Photo when we have one, otherwise a quiet initials avatar. */
function Portrait({ member }: { member: Member }) {
  const base =
    "grid h-[4.2353rem] w-[4.2353rem] shrink-0 place-items-center overflow-hidden rounded-full border border-line";

  if (!member.photo) {
    return (
      <span className={`${base} bg-bg-soft`} aria-hidden>
        <span className="font-sans text-[1.1765rem] font-semibold tracking-tight text-muted">
          {member.initials}
        </span>
      </span>
    );
  }

  return (
    <span className={`${base} bg-bg-soft`}>
      {/* eslint-disable-next-line @next/next/no-img-element */}
      <img
        src={member.photo}
        alt={member.name}
        width={72}
        height={72}
        loading="lazy"
        className="h-full w-full object-cover grayscale transition-all duration-700 ease-smooth group-hover:grayscale-0"
      />
    </span>
  );
}
