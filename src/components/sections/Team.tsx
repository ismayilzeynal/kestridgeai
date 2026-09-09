import { type Member } from "@/data/team";
import { Reveal } from "@/components/ui/Reveal";
import { TeamPhoto } from "@/components/ui/TeamPhoto";

export function Team({ members }: { members: Member[] }) {
  return (
    <section
      id="founders"
      className="relative scroll-mt-8 pb-20 pt-16 sm:scroll-mt-4 sm:pb-28 sm:pt-20"
    >
      <div className="container-x">
        <Reveal>
          <h2 className="text-balance text-4xl text-signal sm:text-5xl">
            The <span className="text-accent-grad">founders</span>
          </h2>
        </Reveal>

        <div className="mt-12 grid grid-cols-1 gap-4 sm:grid-cols-2 lg:mt-14 lg:grid-cols-4">
          {members.map((m, i) => (
            <Reveal key={m.name} delay={(i % 4) * 0.06}>
              <article className="card card-hover group flex h-full flex-col items-start gap-5 rounded-2xl p-6 sm:flex-row sm:items-center lg:flex-col lg:items-start">
                <TeamPhoto photo={m.photo} name={m.name} initials={m.initials} />
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
