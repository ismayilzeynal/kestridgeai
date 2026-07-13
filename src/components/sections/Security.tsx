import {
  ShieldCheck,
  KeyRound,
  LockKeyhole,
  FileSignature,
  EyeOff,
  Radar,
  ArrowRight,
} from "lucide-react";
import { Reveal, Stagger, StaggerItem } from "@/components/ui/Reveal";

const commitments = [
  {
    icon: FileSignature,
    title: "Confidential by default",
    body: "Every engagement is covered by strict confidentiality. NDAs on request, always.",
  },
  {
    icon: KeyRound,
    title: "Least-privilege access",
    body: "Only the engineers on your project can touch your systems — and only what they need.",
  },
  {
    icon: LockKeyhole,
    title: "Encrypted end to end",
    body: "Your data is encrypted in transit and at rest, handled on hardened infrastructure.",
  },
  {
    icon: EyeOff,
    title: "Your AI data stays yours",
    body: "We never reuse or train shared models on your data. What's yours stays yours.",
  },
  {
    icon: ShieldCheck,
    title: "Built for US compliance",
    body: "We work to US regulations and help you stay certification-ready.",
  },
  {
    icon: Radar,
    title: "Monitored around the clock",
    body: "Continuous monitoring with a team available 24/7 for anything urgent.",
  },
];

export function Security() {
  return (
    <section id="security" className="relative py-24 sm:py-32">
      <div className="container-x">
        <div className="relative overflow-hidden rounded-[2rem] border border-line bg-bg-soft/60 p-6 sm:p-12 lg:p-16">
          {/* accent atmosphere */}
          <div
            aria-hidden
            className="pointer-events-none absolute -right-24 -top-24 h-80 w-80 rounded-full opacity-20 blur-[100px]"
            style={{ background: "radial-gradient(closest-side, var(--accent), transparent)" }}
          />
          <div className="bg-grid absolute inset-0 opacity-50" aria-hidden />

          <div className="relative grid gap-12 lg:grid-cols-[0.9fr_1.1fr]">
            <div className="lg:sticky lg:top-28 lg:self-start">
              <span className="eyebrow">Privacy &amp; security</span>
              <h2 className="mt-5 text-balance text-4xl text-signal sm:text-5xl">
                Your privacy isn&apos;t a feature.{" "}
                <span className="text-accent-grad italic">It&apos;s the foundation.</span>
              </h2>
              <p className="mt-6 max-w-md text-pretty text-lg leading-relaxed text-muted">
                We work with sensitive systems and sensitive data every day — AI
                models, production environments, business intelligence. We treat
                all of it like it&apos;s our own, because trust is the only way
                this work happens.
              </p>
              <div className="mt-8 flex flex-wrap gap-3">
                <span className="inline-flex items-center gap-3 rounded-full border border-line-strong bg-surface px-4 py-2.5">
                  <ShieldCheck className="h-5 w-5 text-accent" strokeWidth={1.7} />
                  <span className="text-sm text-ink">100% confidential</span>
                </span>
                <span className="inline-flex items-center gap-3 rounded-full border border-line-strong bg-surface px-4 py-2.5">
                  <Radar className="h-5 w-5 text-accent" strokeWidth={1.7} />
                  <span className="text-sm text-ink">24/7 monitoring &amp; response</span>
                </span>
              </div>
              <Reveal delay={0.12}>
                <a href="#contact" className="btn-primary group mt-7 inline-flex">
                  Start a confidential conversation
                  <ArrowRight className="h-[18px] w-[18px] transition-transform duration-300 group-hover:translate-x-1" />
                </a>
              </Reveal>
            </div>

            <Stagger className="grid gap-4 sm:grid-cols-2">
              {commitments.map((c) => (
                <StaggerItem key={c.title}>
                  <div className="card card-hover h-full rounded-2xl p-5">
                    <div className="grid h-11 w-11 place-items-center rounded-xl border border-line bg-surface text-accent">
                      <c.icon className="h-5 w-5" strokeWidth={1.6} />
                    </div>
                    <h3 className="mt-4 font-sans text-[15px] font-semibold tracking-tight text-ink">
                      {c.title}
                    </h3>
                    <p className="mt-2 text-[13.5px] leading-relaxed text-muted">
                      {c.body}
                    </p>
                  </div>
                </StaggerItem>
              ))}
            </Stagger>
          </div>
        </div>
      </div>
    </section>
  );
}
