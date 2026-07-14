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
    body: "Strict confidentiality on every engagement — NDA on request.",
  },
  {
    icon: KeyRound,
    title: "Least-privilege access",
    body: "Only your project's engineers touch your systems.",
  },
  {
    icon: LockKeyhole,
    title: "Encrypted end to end",
    body: "Encrypted in transit and at rest, on hardened infrastructure.",
  },
  {
    icon: EyeOff,
    title: "Your AI data stays yours",
    body: "We never train shared models on your data.",
  },
  {
    icon: ShieldCheck,
    title: "Built for US compliance",
    body: "We work to US regulations and help you stay certification-ready.",
  },
  {
    icon: Radar,
    title: "Monitored around the clock",
    body: "Continuous monitoring, 24/7 response.",
  },
];

export function Security() {
  return (
    <section id="security" className="relative scroll-mt-5 pb-24 pt-16 sm:pb-32 sm:pt-20 lg:scroll-mt-[26px] lg:pt-12">
      <div className="container-x">
        <div className="relative overflow-hidden rounded-[2rem] border border-line bg-bg-soft/60 p-6 sm:p-10 lg:py-8">
          {/* accent atmosphere */}
          <div
            aria-hidden
            className="pointer-events-none absolute -right-24 -top-24 h-80 w-80 rounded-full opacity-20 blur-[100px]"
            style={{ background: "radial-gradient(closest-side, var(--accent), transparent)" }}
          />
          <div className="bg-grid absolute inset-0 opacity-50" aria-hidden />

          <div className="relative grid gap-12 lg:gap-10 lg:grid-cols-[0.9fr_1.1fr]">
            <div className="lg:sticky lg:top-24 lg:self-start">
              <span className="eyebrow">Privacy &amp; security</span>
              <h2 className="mt-4 text-balance text-4xl text-signal sm:text-5xl">
                Your privacy isn&apos;t a feature.{" "}
                <span className="text-accent-grad">It&apos;s the foundation.</span>
              </h2>
              <p className="mt-4 max-w-md text-pretty text-lg leading-relaxed text-muted">
                We work with sensitive systems and sensitive data every day — AI
                models, production environments, business intelligence. We treat
                all of it like it&apos;s our own, because trust is the only way
                this work happens.
              </p>
              <div className="mt-5 flex flex-wrap gap-3">
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
                <a href="#contact" className="btn-primary group mt-5 inline-flex">
                  Start a confidential conversation
                  <ArrowRight className="h-[18px] w-[18px] transition-transform duration-300 group-hover:translate-x-1" />
                </a>
              </Reveal>
            </div>

            <Stagger className="grid gap-4 sm:grid-cols-2 lg:gap-3">
              {commitments.map((c) => (
                <StaggerItem key={c.title}>
                  <div className="card card-hover h-full rounded-2xl p-5 lg:p-4">
                    <div className="grid h-10 w-10 place-items-center rounded-xl border border-line bg-surface text-accent">
                      <c.icon className="h-5 w-5" strokeWidth={1.6} />
                    </div>
                    <h3 className="mt-3 font-sans text-[16.5px] font-semibold tracking-tight text-ink">
                      {c.title}
                    </h3>
                    <p className="mt-1.5 text-[14.5px] leading-relaxed text-muted">
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
