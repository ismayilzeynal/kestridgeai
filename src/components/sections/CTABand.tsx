import { ArrowRight } from "lucide-react";
import { Reveal } from "@/components/ui/Reveal";
import { LinkedInButton } from "@/components/ui/LinkedInButton";

export function CTABand() {
  return (
    <section className="relative py-16 sm:py-24">
      <div className="container-x">
        <div className="relative overflow-hidden rounded-[2rem] border border-line-strong bg-gradient-to-b from-surface to-bg-soft px-7 py-14 text-center sm:px-12 sm:py-20">
          <div
            aria-hidden
            className="pointer-events-none absolute inset-x-0 -top-32 mx-auto h-64 w-[80%]"
            style={{
              background:
                "radial-gradient(closest-side, rgba(11,122,103,0.14), transparent 70%)",
            }}
          />
          <div className="bg-grid bg-grid-fade absolute inset-0 opacity-60" aria-hidden />

          <div className="relative mx-auto max-w-2xl">
            <Reveal>
              <span className="eyebrow justify-center">Ready when you are</span>
            </Reveal>
            <Reveal delay={0.06}>
              <h2 className="mt-5 text-balance text-4xl text-signal sm:text-[3rem] sm:leading-[1.05]">
                Have something that needs{" "}
                <span className="text-accent-grad">solving?</span>
              </h2>
            </Reveal>
            <Reveal delay={0.12}>
              <p className="mx-auto mt-5 max-w-lg text-pretty text-lg leading-relaxed text-muted">
                One short conversation is all it takes to find out how we can
                help. No pressure, no jargon — just engineers who listen.
              </p>
            </Reveal>
            <Reveal delay={0.18}>
              <div className="mt-9 flex flex-wrap items-center justify-center gap-3">
                <a href="#contact" className="btn-primary group">
                  Start a project
                  <ArrowRight className="h-[18px] w-[18px] transition-transform duration-300 group-hover:translate-x-1" />
                </a>
                <LinkedInButton />
              </div>
            </Reveal>
          </div>
        </div>
      </div>
    </section>
  );
}
