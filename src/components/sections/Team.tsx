import { Linkedin } from "lucide-react";
import { team } from "@/data/team";
import { site } from "@/lib/site";
import { SectionHeading } from "@/components/ui/SectionHeading";
import { Reveal } from "@/components/ui/Reveal";
import { LinkedInButton } from "@/components/ui/LinkedInButton";

export function Team() {
  return (
    <section id="team" className="relative py-24 sm:py-32">
      <div className="container-x">
        <div className="flex flex-col items-start justify-between gap-8 sm:flex-row sm:items-end">
          <SectionHeading
            eyebrow="The people"
            title={
              <>
                The faces behind the{" "}
                <span className="text-accent-grad">work</span>
              </>
            }
            description="A senior team of engineers across AI, security, automation and data. Real people who stay accountable from kickoff to long after launch."
          />
          <Reveal delay={0.1}>
            <LinkedInButton label="Meet us on LinkedIn" />
          </Reveal>
        </div>

        <div className="mt-14 grid grid-cols-2 gap-4 sm:gap-5 md:grid-cols-3 lg:grid-cols-4">
          {team.map((m, i) => (
            <Reveal key={m.name} delay={(i % 4) * 0.06}>
              <article className="card card-hover group h-full overflow-hidden rounded-3xl">
                <div className="relative aspect-[4/5] overflow-hidden">
                  {/* eslint-disable-next-line @next/next/no-img-element */}
                  <img
                    src={m.photo}
                    alt={m.name}
                    loading="lazy"
                    className="h-full w-full object-cover grayscale transition-all duration-700 ease-smooth group-hover:scale-[1.04] group-hover:grayscale-0"
                  />
                  <div className="absolute inset-0 bg-gradient-to-t from-surface via-surface/10 to-transparent" />
                  <a
                    href={m.linkedin}
                    target="_blank"
                    rel="noopener noreferrer"
                    aria-label={`${m.name} — ${site.brand} on LinkedIn`}
                    className="absolute right-3 top-3 grid h-9 w-9 place-items-center rounded-full border border-line-strong bg-surface/80 text-ink opacity-80 backdrop-blur transition-all duration-300 ease-smooth hover:bg-accent hover:text-white sm:opacity-0 sm:group-hover:opacity-100"
                  >
                    <Linkedin className="h-4 w-4" strokeWidth={1.8} />
                  </a>
                </div>
                <div className="p-4 sm:p-5">
                  <h3 className="font-sans text-[16.5px] font-semibold tracking-tight text-ink">
                    {m.name}
                  </h3>
                  <p className="mt-1 text-[14px] font-medium text-accent">{m.role}</p>
                  <p className="mt-2 font-mono text-[12px] leading-relaxed text-faint">
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
