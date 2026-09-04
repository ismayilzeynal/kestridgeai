"use client";

import { ArrowUpRight } from "lucide-react";
import { services } from "@/data/services";
import { site } from "@/lib/site";
import { openService, scrollToId } from "@/lib/scroll";

export function Hero() {
  return (
    <section id="top" className="relative overflow-hidden pb-16 pt-28 sm:pt-32">
      <div className="container-x">
        <div className="grid items-start gap-12 lg:grid-cols-[1.12fr_0.88fr]">
          {/* Left, the message */}
          <div>
            <div className="anim-rise eyebrow" style={{ animationDelay: "0.05s" }}>
              {site.location}
            </div>

            {/* One wrapping heading rather than three clipped lines: the
                sentence is long enough that a fixed line split breaks at some
                viewport width. */}
            <h1
              className="anim-rise mt-7 text-balance text-[clamp(2rem,4.1vw,2.95rem)] font-bold leading-[1.1] text-signal"
              style={{ animationDelay: "0.08s" }}
            >
              We build smart systems that{" "}
              <span className="text-accent-grad">run business better.</span>
            </h1>

            <p
              className="anim-rise mt-6 max-w-xl text-pretty text-[1.1176rem] leading-relaxed text-muted"
              style={{ animationDelay: "0.3s" }}
            >
              We work with your business to understand what matters, evaluate the
              ROI, and carry it through to a secure implementation.
            </p>

            {/* One button only: the navigation bar already carries the
                contact action, so the hero does not repeat it. */}
            <div
              className="anim-rise mt-9 flex flex-wrap items-center gap-3"
              style={{ animationDelay: "0.42s" }}
            >
              <button
                onClick={() => scrollToId("services")}
                className="btn-ghost group"
              >
                See our services
                <ArrowUpRight className="h-[1.0588rem] w-[1.0588rem] transition-transform duration-300 ease-smooth group-hover:translate-x-0.5 group-hover:-translate-y-0.5" />
              </button>
            </div>
          </div>

          {/* Right, the four areas of work */}
          <div className="anim-rise relative" style={{ animationDelay: "0.28s" }}>
            <div className="card glow-accent overflow-hidden rounded-[1.6rem] p-1.5">
              <div className="rounded-[1.25rem] border border-line bg-bg-soft p-2">
                <div className="px-4 py-3.5">
                  <span className="font-mono text-[0.7059rem] uppercase tracking-[0.18em] text-muted">
                    Areas of work
                  </span>
                </div>
                <div className="flex flex-col">
                  {services.map((s) => (
                    <button
                      key={s.id}
                      onClick={() => openService(s.id)}
                      className="group flex items-center gap-4 rounded-xl border border-transparent px-4 py-4 text-left transition-all duration-300 ease-smooth hover:border-line hover:bg-surface-2"
                    >
                      <span className="font-mono text-[0.7647rem] text-faint">{s.index}</span>
                      <span className="grid h-11 w-11 place-items-center rounded-xl border border-line bg-surface text-accent transition-colors duration-300 group-hover:border-line-strong">
                        <s.icon className="h-5 w-5" strokeWidth={1.6} />
                      </span>
                      <span className="flex-1">
                        <span className="block text-[0.9412rem] font-semibold text-ink">
                          {s.name}
                        </span>
                        <span className="block text-[0.7647rem] text-faint">
                          {s.cardLabel}
                        </span>
                      </span>
                      <ArrowUpRight className="h-[1.0588rem] w-[1.0588rem] text-faint transition-all duration-300 group-hover:text-accent group-hover:translate-x-0.5 group-hover:-translate-y-0.5" />
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
