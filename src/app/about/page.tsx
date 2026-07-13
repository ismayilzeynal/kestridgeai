import type { Metadata } from "next";
import Link from "next/link";
import {
  MapPin,
  Globe2,
  ShieldCheck,
  Target,
  Landmark,
  ArrowRight,
} from "lucide-react";
import { Reveal } from "@/components/ui/Reveal";
import { SectionHeading } from "@/components/ui/SectionHeading";
import { LinkedInButton } from "@/components/ui/LinkedInButton";
import { site } from "@/lib/site";

export const metadata: Metadata = {
  title: "About",
  description:
    "AIVanta is an American technology company based in Illinois, delivering applied AI, automation, security and analytics — with access to global engineering talent.",
  alternates: { canonical: "/about" },
};

const principles = [
  {
    icon: Landmark,
    title: "American-based accountability",
    body: "A United States company you can hold accountable — working to US standards, regulations and expectations.",
  },
  {
    icon: Globe2,
    title: "Access to global talent",
    body: "We draw on a worldwide network of specialists, so the right expertise is on every problem, exactly when it's needed.",
  },
  {
    icon: ShieldCheck,
    title: "Security & confidentiality first",
    body: "Strict controls on every engagement. Your data and systems are treated with the same care as our own.",
  },
  {
    icon: Target,
    title: "Built for outcomes",
    body: "We measure success by what runs reliably in production — not by hours billed or slideware delivered.",
  },
];

export default function AboutPage() {
  return (
    <>
      {/* Header */}
      <section className="relative overflow-hidden pt-32 pb-16 sm:pt-40">
        <div className="container-x">
          <Reveal>
            <span className="eyebrow">About AIVanta</span>
          </Reveal>
          <Reveal delay={0.06}>
            <h1 className="mt-6 max-w-4xl text-balance text-[clamp(2rem,7vw,3.6rem)] leading-[1.05] text-signal">
              An American company, built on{" "}
              <span className="text-accent-grad italic">global engineering.</span>
            </h1>
          </Reveal>
          <Reveal delay={0.12}>
            <p className="mt-7 max-w-2xl text-pretty text-lg leading-relaxed text-muted">
              AIVanta is a United States technology company delivering applied AI,
              automation, cybersecurity and analytics. We&apos;re based in
              Illinois — and we draw on a network of engineering talent worldwide,
              so you get local accountability with global capability.
            </p>
          </Reveal>

          <Reveal delay={0.18}>
            <div className="mt-8 flex flex-wrap gap-3">
              <span className="inline-flex items-center gap-2.5 rounded-full border border-line bg-surface px-4 py-2.5 text-sm text-ink">
                <MapPin className="h-4 w-4 text-accent" strokeWidth={1.7} />
                Based in Illinois, USA
              </span>
              <span className="inline-flex items-center gap-2.5 rounded-full border border-line bg-surface px-4 py-2.5 text-sm text-ink">
                <Globe2 className="h-4 w-4 text-accent" strokeWidth={1.7} />
                Global engineering network
              </span>
            </div>
          </Reveal>
        </div>
      </section>

      {/* Narrative */}
      <section className="relative py-16 sm:py-20">
        <div className="container-x">
          <div className="grid gap-12 lg:grid-cols-[0.8fr_1.2fr]">
            <div className="lg:sticky lg:top-28 lg:self-start">
              <SectionHeading
                eyebrow="Who we are"
                title={
                  <>
                    Engineering-first, <span className="text-accent-grad italic">delivery-obsessed.</span>
                  </>
                }
              />
            </div>
            <div className="flex flex-col gap-6 text-pretty text-lg leading-relaxed text-muted">
              <Reveal>
                <p>
                  We design, build and secure the systems businesses rely on —
                  applied AI, automation, cybersecurity and analytics — with the
                  rigor and accountability you expect from an American partner.
                </p>
              </Reveal>
              <Reveal delay={0.06}>
                <p>
                  Our engineers work closely with each client, and we draw on a
                  global network of specialists so every project gets exactly the
                  expertise it needs, when it needs it. Access to global resources
                  means we can move quickly without compromising on quality.
                </p>
              </Reveal>
              <Reveal delay={0.12}>
                <p>
                  And we don&apos;t disappear after kickoff. We stay involved until
                  the work is running the way it should — and we&apos;re available
                  around the clock if something comes up.
                </p>
              </Reveal>
            </div>
          </div>
        </div>
      </section>

      {/* Principles */}
      <section className="relative py-16 sm:py-20">
        <div className="container-x">
          <SectionHeading
            eyebrow="What we stand for"
            title={
              <>
                Principles that <span className="text-accent-grad italic">hold up</span> under pressure
              </>
            }
            align="center"
          />
          <div className="mt-12 grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
            {principles.map((p, i) => (
              <Reveal key={p.title} delay={(i % 4) * 0.06}>
                <div className="card card-hover h-full rounded-2xl p-6">
                  <div className="grid h-11 w-11 place-items-center rounded-xl border border-line bg-bg-soft text-accent">
                    <p.icon className="h-5 w-5" strokeWidth={1.6} />
                  </div>
                  <h3 className="mt-4 font-sans text-[15px] font-semibold tracking-tight text-ink">
                    {p.title}
                  </h3>
                  <p className="mt-2 text-[13.5px] leading-relaxed text-muted">
                    {p.body}
                  </p>
                </div>
              </Reveal>
            ))}
          </div>
        </div>
      </section>

      {/* Location + CTA */}
      <section className="relative py-16 sm:py-24">
        <div className="container-x">
          <div className="relative overflow-hidden rounded-[2rem] border border-line bg-surface p-8 sm:p-12 lg:p-16">
            <div className="bg-grid bg-grid-fade absolute inset-0 opacity-60" aria-hidden />
            <div className="relative grid gap-10 lg:grid-cols-[1fr_auto] lg:items-center">
              <div>
                <span className="eyebrow">Where we&apos;re based</span>
                <h2 className="mt-5 text-balance text-3xl text-signal sm:text-4xl">
                  Illinois, United States
                </h2>
                <p className="mt-5 max-w-xl text-pretty text-lg leading-relaxed text-muted">
                  We operate as a US-based company. A precise office address will
                  be published here as we finalize it. Until then, the fastest way
                  to reach us is through the form, by email, or on LinkedIn.
                </p>
                <div className="mt-8 flex flex-wrap gap-3">
                  <Link href="/#contact" className="btn-primary group">
                    Start a project
                    <ArrowRight className="h-[18px] w-[18px] transition-transform duration-300 group-hover:translate-x-1" />
                  </Link>
                  <LinkedInButton />
                </div>
              </div>
              <div className="hidden lg:block">
                <span
                  aria-hidden
                  className="grid h-40 w-40 place-items-center rounded-3xl border border-line bg-bg-soft text-accent"
                >
                  <MapPin className="h-16 w-16" strokeWidth={1} />
                </span>
              </div>
            </div>
          </div>
        </div>
      </section>
    </>
  );
}
