import { Handshake, FileCheck2, Boxes, LifeBuoy } from "lucide-react";
import { Reveal } from "@/components/ui/Reveal";

const points = [
  {
    icon: Handshake,
    title: "We start with your requirements",
    body: "We meet first and write down what the business needs, before any work starts.",
  },
  {
    icon: FileCheck2,
    title: "We agree on the plan in writing",
    body: "You get the solution and the schedule in writing. Work starts once you approve it.",
  },
  {
    icon: Boxes,
    title: "We build inside your existing systems",
    body: "We connect it to the systems your staff already use, and test it there before anyone relies on it.",
  },
  {
    icon: LifeBuoy,
    title: "We support it after launch",
    body: "We monitor the systems we run for you and respond to issues that come up.",
  },
];

export function WhyChooseUs() {
  return (
    <section
      id="process"
      aria-label="How we run a project"
      className="relative scroll-mt-4 py-20 sm:-scroll-mt-4 sm:py-28"
    >
      <div className="container-x">
        <div className="grid gap-14 lg:grid-cols-[0.8fr_1.2fr]">
          <div className="lg:sticky lg:top-24 lg:self-start">
            <Reveal>
              <h2 className="text-balance text-4xl text-signal sm:text-5xl">
                How we <span className="text-accent-grad">run</span> a project
              </h2>
            </Reveal>
            <Reveal delay={0.12}>
              <p className="mt-5 text-pretty text-lg leading-relaxed text-muted">
                We follow the same method on every project, from the first
                consultation to support after launch.
              </p>
            </Reveal>
          </div>

          <div className="flex flex-col">
            {points.map((p, i) => (
              <Reveal key={p.title} delay={i * 0.05}>
                <div className="group grid grid-cols-[auto_1fr] gap-5 border-t border-line py-7 transition-colors duration-300 first:border-t-0 hover:border-line-strong">
                  <div className="grid h-12 w-12 place-items-center rounded-2xl border border-line bg-surface text-accent transition-all duration-500 ease-smooth group-hover:border-line-strong group-hover:shadow-glow-accent">
                    <p.icon className="h-[1.2941rem] w-[1.2941rem]" strokeWidth={1.6} />
                  </div>
                  <div className="pt-1">
                    <h3 className="font-display text-[1.35rem] font-medium text-ink">
                      {p.title}
                    </h3>
                    <p className="mt-2 text-pretty leading-relaxed text-muted">
                      {p.body}
                    </p>
                  </div>
                </div>
              </Reveal>
            ))}
          </div>
        </div>
      </div>
    </section>
  );
}
