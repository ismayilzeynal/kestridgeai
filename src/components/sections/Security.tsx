import {
  ShieldCheck,
  KeyRound,
  LockKeyhole,
  FileSignature,
  EyeOff,
  Radar,
  FileText,
  Globe2,
} from "lucide-react";
import { Reveal } from "@/components/ui/Reveal";

const commitments = [
  {
    icon: FileSignature,
    title: "Confidentiality on every project",
    body: "It applies from the first conversation. A nondisclosure agreement is available on request.",
  },
  {
    icon: KeyRound,
    title: "Limited access",
    body: "Each engineer reaches only the systems their part of the work needs.",
  },
  {
    icon: LockKeyhole,
    title: "Encryption",
    body: "Your data is scrambled in transit and in storage, readable only by people we authorize.",
  },
  {
    icon: EyeOff,
    title: "Your data stays yours",
    body: "We use it only for your project, never to teach AI systems that serve other companies.",
  },
  {
    icon: FileText,
    title: "A written record",
    body: "You receive a document listing who has access, what is encrypted, and what is monitored.",
  },
  {
    icon: Globe2,
    title: "Specialists abroad",
    body: "They work under the same terms as our own engineers, and we name everyone assigned to your project.",
  },
  {
    icon: Radar,
    title: "Monitoring around the clock",
    body: "We monitor the systems we run for you and respond to issues at any hour.",
  },
];

export function Security() {
  return (
    <section
      id="security"
      className="relative scroll-mt-5 pb-20 pt-16 sm:pb-28 sm:pt-20 lg:scroll-mt-[26px]"
    >
      <div className="container-x">
        <div className="relative rounded-[2rem] border border-line bg-bg-soft/60 p-6 sm:p-10 lg:py-10">
          {/* Decoration is clipped by its own wrapper so the panel itself can
              stay overflow-visible, otherwise it becomes the scrollport for the
              sticky column below and the column never pins. */}
          <div
            aria-hidden
            className="pointer-events-none absolute inset-0 overflow-hidden rounded-[2rem]"
          >
            <div
              className="absolute -right-24 -top-24 h-80 w-80 rounded-full opacity-20 blur-[100px]"
              style={{ background: "radial-gradient(closest-side, var(--accent), transparent)" }}
            />
            <div className="bg-grid absolute inset-0 opacity-50" />
          </div>

          <div className="relative grid gap-12 lg:grid-cols-[0.9fr_1.1fr] lg:gap-10">
            <div className="lg:sticky lg:top-24 lg:self-start">
              <h2 className="text-balance text-4xl text-signal sm:text-5xl">
                How we handle{" "}
                <span className="text-accent-grad">your systems and data</span>
              </h2>
              <p className="mt-5 max-w-md text-pretty text-lg leading-relaxed text-muted">
                We work inside the systems your business runs on. Security
                review is part of every project, not an add-on.
              </p>
              <p className="mt-5 flex items-center gap-2.5 text-[15px] text-muted">
                <ShieldCheck className="h-5 w-5 shrink-0 text-accent" strokeWidth={1.7} />
                These terms apply to every project.
              </p>
            </div>

            <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-1 lg:gap-3 xl:grid-cols-2">
              {commitments.map((c, i) => (
                <Reveal key={c.title} delay={(i % 2) * 0.06}>
                  <div className="card card-hover h-full rounded-2xl p-5 lg:p-4">
                    <div className="grid h-10 w-10 place-items-center rounded-xl border border-line bg-surface text-accent">
                      <c.icon className="h-5 w-5" strokeWidth={1.6} />
                    </div>
                    <h3 className="mt-3 font-sans text-[16px] font-semibold tracking-tight text-ink">
                      {c.title}
                    </h3>
                    <p className="mt-1.5 text-[14.5px] leading-relaxed text-muted">
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
