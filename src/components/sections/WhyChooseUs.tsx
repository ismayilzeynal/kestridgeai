import {
  Handshake,
  FileCheck2,
  MessageSquareDot,
  CheckCircle2,
  Clock,
  ArrowRight,
} from "lucide-react";
import { SectionHeading } from "@/components/ui/SectionHeading";
import { Reveal } from "@/components/ui/Reveal";

const points = [
  {
    icon: Handshake,
    title: "We start by understanding you",
    body: "We sit down with you and learn exactly what you need before anyone writes a line of code.",
  },
  {
    icon: FileCheck2,
    title: "We agree on what & when",
    body: "Clear scope, clear deliverables, clear timeline. You know what you're getting and by when.",
  },
  {
    icon: MessageSquareDot,
    title: "We keep you informed",
    body: "You stay in the loop through the entire engagement — no surprises, no black boxes.",
  },
  {
    icon: CheckCircle2,
    title: "It works in your environment",
    body: "When we're done, we make sure everything actually works where it matters: in production.",
  },
  {
    icon: Clock,
    title: "We're here 24/7",
    body: "If something comes up, we're available around the clock. We don't walk away.",
  },
];

export function WhyChooseUs() {
  return (
    <section className="relative py-24 sm:py-32">
      <div className="container-x">
        <div className="grid gap-14 lg:grid-cols-[0.8fr_1.2fr]">
          <div className="lg:sticky lg:top-28 lg:self-start">
            <SectionHeading
              eyebrow="How we work"
              title={
                <>
                  Why companies <span className="text-accent-grad italic">choose</span> us
                </>
              }
              description="We're not a vendor that disappears after kickoff. We work like part of your team — from the first conversation to long after launch."
            />
            <Reveal delay={0.18}>
              <a href="#contact" className="btn-primary group mt-8 inline-flex">
                Work with us
                <ArrowRight className="h-[18px] w-[18px] transition-transform duration-300 group-hover:translate-x-1" />
              </a>
            </Reveal>
          </div>

          <div className="flex flex-col">
            {points.map((p, i) => (
              <Reveal key={p.title} delay={i * 0.05}>
                <div className="group grid grid-cols-[auto_1fr] gap-5 border-t border-line py-7 transition-colors duration-300 first:border-t-0 hover:border-line-strong">
                  <div className="grid h-12 w-12 place-items-center rounded-2xl border border-line bg-surface text-accent transition-all duration-500 ease-smooth group-hover:border-line-strong group-hover:shadow-glow-accent">
                    <p.icon className="h-[22px] w-[22px]" strokeWidth={1.6} />
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
