import { Landmark, Globe2, ShieldCheck, CheckCircle2 } from "lucide-react";
import { Reveal } from "@/components/ui/Reveal";

const cards = [
  {
    icon: Landmark,
    title: "Who you contract with",
    body: "Your agreement is with Kestridge AI, a United States company.",
  },
  {
    icon: Globe2,
    title: "Engineering network",
    body: "For specific experience we bring in specialists from outside the United States.",
  },
  {
    icon: ShieldCheck,
    title: "Security and confidentiality",
    body: "Every project includes a security review before work begins.",
  },
  {
    icon: CheckCircle2,
    title: "Project completion",
    body: "A project is complete when the system is running in your business and your staff use it.",
  },
];

export function Company() {
  return (
    <section
      id="company"
      className="relative scroll-mt-4 py-20 sm:-scroll-mt-4 sm:py-28"
    >
      <div className="container-x">
        <div className="grid gap-12 lg:grid-cols-[0.85fr_1.15fr] lg:gap-14">
          <div className="lg:sticky lg:top-24 lg:self-start">
            <Reveal>
              <h2 className="text-balance text-4xl text-signal sm:text-5xl">
                A United States{" "}
                <span className="text-accent-grad">technology company</span>
              </h2>
            </Reveal>
          </div>

          <div>
            <div className="flex flex-col gap-5 text-pretty text-[1.0294rem] leading-relaxed text-muted">
              <Reveal>
                <p>
                  Kestridge AI is a technology company based in Illinois. We work
                  in four areas: AI solutions, automation, IT security, and data
                  analytics.
                </p>
              </Reveal>
            </div>

            <div className="mt-10 grid gap-4 sm:grid-cols-2">
              {cards.map((c, i) => (
                <Reveal key={c.title} delay={(i % 2) * 0.06}>
                  <div className="card card-hover h-full rounded-2xl p-5">
                    <div className="grid h-10 w-10 place-items-center rounded-xl border border-line bg-bg-soft text-accent">
                      <c.icon className="h-5 w-5" strokeWidth={1.6} />
                    </div>
                    <h3 className="mt-3.5 font-sans text-[0.9412rem] font-semibold tracking-tight text-ink">
                      {c.title}
                    </h3>
                    <p className="mt-1.5 text-[0.8529rem] leading-relaxed text-muted">
                      {c.body}
                    </p>
                  </div>
                </Reveal>
              ))}
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}
