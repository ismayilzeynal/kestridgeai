"use client";

import { ArrowRight, ArrowUpRight } from "lucide-react";
import { services } from "@/data/services";
import { openService, scrollToId } from "@/lib/scroll";

export function Hero() {
  return (
    <section id="top" className="relative overflow-hidden pb-16 pt-28 sm:pt-32">
      <div className="container-x">
        <div className="grid items-start gap-12 lg:grid-cols-[1.12fr_0.88fr]">
          {/* Left — message */}
          <div>
            <div className="anim-rise eyebrow" style={{ animationDelay: "0.05s" }}>
              United States · Applied AI &amp; Engineering
            </div>

            <h1 className="mt-7 text-[clamp(2.1rem,4.4vw,3.2rem)] font-bold leading-[1.08]">
              <Line delay={0.06}>Enterprise AI,</Line>
              <Line delay={0.14}>automation &amp; security,</Line>
              <Line delay={0.22}>
                <span className="text-accent-grad">engineered around you.</span>
              </Line>
            </h1>

            <p
              className="anim-rise mt-6 max-w-xl text-pretty text-[19px] leading-relaxed text-muted"
              style={{ animationDelay: "0.3s" }}
            >
              An American company whose engineers work closely with you,
              understand exactly what you need, and deliver it — then stay
              until everything runs the way it should.
            </p>

            <div
              className="anim-rise mt-9 flex flex-wrap items-center gap-3"
              style={{ animationDelay: "0.42s" }}
            >
              <a href="#contact" className="btn-primary group">
                Start a project
                <ArrowRight className="h-[18px] w-[18px] transition-transform duration-300 ease-smooth group-hover:translate-x-1" />
              </a>
              <button
                onClick={() => scrollToId("services")}
                className="btn-ghost group"
              >
                Explore services
                <ArrowUpRight className="h-[18px] w-[18px] transition-transform duration-300 ease-smooth group-hover:translate-x-0.5 group-hover:-translate-y-0.5" />
              </button>
            </div>
          </div>

          {/* Right — capability console */}
          <div className="anim-rise relative" style={{ animationDelay: "0.28s" }}>
            <div className="card glow-accent overflow-hidden rounded-[1.6rem] p-1.5">
              <div className="rounded-[1.25rem] border border-line bg-bg-soft p-2">
                <div className="px-4 py-3.5">
                  <span className="font-mono text-[12px] uppercase tracking-[0.18em] text-muted">
                    Where we help
                  </span>
                </div>
                <div className="flex flex-col">
                  {services.map((s) => (
                    <button
                      key={s.id}
                      onClick={() => openService(s.id)}
                      className="group flex items-center gap-4 rounded-xl border border-transparent px-4 py-4 text-left transition-all duration-300 ease-smooth hover:border-line hover:bg-surface-2"
                    >
                      <span className="font-mono text-[13px] text-faint">{s.index}</span>
                      <span className="grid h-11 w-11 place-items-center rounded-xl border border-line bg-surface text-accent transition-colors duration-300 group-hover:border-line-strong">
                        <s.icon className="h-5 w-5" strokeWidth={1.6} />
                      </span>
                      <span className="flex-1">
                        <span className="block text-[16px] font-semibold text-ink">
                          {s.name}
                        </span>
                        <span className="block text-[13px] text-faint">
                          {s.cardLabel}
                        </span>
                      </span>
                      <ArrowUpRight className="h-[18px] w-[18px] text-faint transition-all duration-300 group-hover:text-accent group-hover:translate-x-0.5 group-hover:-translate-y-0.5" />
                    </button>
                  ))}
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}

function Line({ children, delay }: { children: React.ReactNode; delay: number }) {
  return (
    // pb + matching -mb give the clip region room for tall descenders (g, y)
    // without loosening the line spacing.
    <span className="block overflow-hidden pb-[0.16em] -mb-[0.16em]">
      <span className="anim-line block text-signal" style={{ animationDelay: `${delay}s` }}>
        {children}
      </span>
    </span>
  );
}
