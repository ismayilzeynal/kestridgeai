"use client";

import { useRef, useState, type KeyboardEvent } from "react";
import { AnimatePresence, motion } from "framer-motion";
import { ArrowRight, Check, Lock } from "lucide-react";
import { services } from "@/data/services";
import { SectionHeading } from "@/components/ui/SectionHeading";
import { Reveal } from "@/components/ui/Reveal";

const ease = [0.22, 1, 0.36, 1] as const;
const NAV_OFFSET = 96;

export function Services() {
  const [active, setActive] = useState(0);
  const svc = services[active];
  const tabRefs = useRef<(HTMLButtonElement | null)[]>([]);

  const selectAndScroll = (id: string) => {
    window.dispatchEvent(new CustomEvent("prefill-service", { detail: id }));
    const el = document.getElementById("contact");
    if (el) {
      const top = el.getBoundingClientRect().top + window.scrollY - NAV_OFFSET;
      window.scrollTo({ top, behavior: "smooth" });
    }
  };

  const onTabKeyDown = (e: KeyboardEvent<HTMLDivElement>) => {
    const count = services.length;
    let next: number | null = null;
    if (e.key === "ArrowRight" || e.key === "ArrowDown") next = (active + 1) % count;
    else if (e.key === "ArrowLeft" || e.key === "ArrowUp") next = (active - 1 + count) % count;
    else if (e.key === "Home") next = 0;
    else if (e.key === "End") next = count - 1;
    if (next !== null) {
      e.preventDefault();
      setActive(next);
      tabRefs.current[next]?.focus();
    }
  };

  return (
    <section id="services" className="relative scroll-mt-24 py-24 sm:py-32">
      <div className="container-x">
        <SectionHeading
          eyebrow="What we do"
          title={
            <>
              Services built around{" "}
              <span className="text-accent-grad italic">your</span> requirements
            </>
          }
          description="Four disciplines, one way of working: understand the problem, agree the plan, build it into your environment, and stay until it runs."
        />

        <div className="mt-14 grid gap-6 lg:grid-cols-[340px_1fr]">
          {/* Tab rail */}
          <Reveal>
            <div className="relative">
              <div
                role="tablist"
                aria-label="Services"
                aria-orientation="vertical"
                onKeyDown={onTabKeyDown}
                className="flex gap-3 overflow-x-auto pb-2 [scrollbar-width:none] [-ms-overflow-style:none] [&::-webkit-scrollbar]:hidden lg:flex-col lg:gap-2.5 lg:overflow-visible lg:pb-0 snap-x snap-mandatory"
              >
                {services.map((s, i) => {
                  const isActive = i === active;
                  return (
                    <button
                      key={s.id}
                      ref={(el) => {
                        tabRefs.current[i] = el;
                      }}
                      id={`svc-tab-${s.id}`}
                      role="tab"
                      aria-selected={isActive}
                      aria-controls="svc-panel"
                      tabIndex={isActive ? 0 : -1}
                      onClick={() => setActive(i)}
                      className={`group relative flex min-w-[220px] shrink-0 snap-start items-center gap-4 rounded-2xl border px-5 py-4 text-left transition-all duration-300 ease-smooth lg:min-w-0 ${
                        isActive
                          ? "border-line-strong bg-surface"
                          : "border-line bg-transparent hover:bg-surface/60"
                      }`}
                    >
                      {isActive && (
                        <motion.span
                          layoutId="svc-active"
                          className="absolute inset-y-3 left-0 w-[3px] rounded-full bg-accent"
                          style={{ boxShadow: "0 0 14px 1px var(--accent-glow)" }}
                        />
                      )}
                      <span
                        className={`grid h-11 w-11 place-items-center rounded-xl border transition-colors duration-300 ${
                          isActive
                            ? "border-line-strong bg-bg text-accent"
                            : "border-line bg-surface text-muted group-hover:text-ink"
                        }`}
                      >
                        <s.icon className="h-5 w-5" strokeWidth={1.6} />
                      </span>
                      <span className="flex-1">
                        <span className="block font-mono text-[11px] text-faint">
                          {s.index}
                        </span>
                        <span
                          className={`block text-[15px] font-semibold tracking-tight ${
                            isActive ? "text-ink" : "text-muted group-hover:text-ink"
                          }`}
                        >
                          {s.name}
                        </span>
                      </span>
                      <ArrowRight
                        className={`h-4 w-4 transition-all duration-300 ${
                          isActive
                            ? "text-accent opacity-100"
                            : "text-faint opacity-0 group-hover:opacity-100"
                        }`}
                      />
                    </button>
                  );
                })}
              </div>
              {/* overflow fade affordance on mobile */}
              <div
                aria-hidden
                className="pointer-events-none absolute inset-y-0 right-0 w-12 bg-gradient-to-l from-bg to-transparent lg:hidden"
              />
            </div>
          </Reveal>

          {/* Panel */}
          <div
            id="svc-panel"
            role="tabpanel"
            aria-labelledby={`svc-tab-${svc.id}`}
            tabIndex={0}
            className="card rounded-[1.6rem] p-6 sm:p-9"
          >
            <AnimatePresence mode="wait">
              <motion.div
                key={svc.id}
                initial={{ opacity: 0, y: 14 }}
                animate={{ opacity: 1, y: 0 }}
                exit={{ opacity: 0, y: -10 }}
                transition={{ duration: 0.4, ease }}
              >
                <div className="flex flex-col gap-2">
                  <span className="font-mono text-xs uppercase tracking-label text-faint">
                    {svc.index} — {svc.name}
                  </span>
                  <h3 className="text-balance font-display text-2xl text-ink sm:text-[2rem] sm:leading-tight">
                    {svc.tagline}
                  </h3>
                </div>

                <p className="mt-5 max-w-2xl text-pretty leading-relaxed text-muted">
                  {svc.description}
                </p>

                <div className="mt-6 flex flex-wrap gap-2.5">
                  {svc.highlights.map((h) => (
                    <span
                      key={h}
                      className="inline-flex items-center gap-2 rounded-full border border-line bg-bg-soft px-3.5 py-1.5 text-[13px] text-muted"
                    >
                      <Check className="h-3.5 w-3.5 text-accent" strokeWidth={2.4} />
                      {h}
                    </span>
                  ))}
                </div>

                {/* Delivery steps */}
                <div className="mt-9">
                  <div className="mb-4 flex items-center gap-3">
                    <span className="font-mono text-[11px] uppercase tracking-label text-faint">
                      How we deliver
                    </span>
                    <span className="hairline flex-1" />
                  </div>
                  <ol className="grid gap-3 md:grid-cols-2">
                    {svc.steps.map((step, i) => (
                      <li
                        key={step.phase}
                        className="relative rounded-2xl border border-line bg-bg-soft/60 p-4"
                      >
                        <div className="flex items-center justify-between gap-3">
                          <span className="flex items-center gap-2.5">
                            <span className="grid h-6 w-6 place-items-center rounded-md bg-surface font-mono text-[11px] text-accent">
                              {i + 1}
                            </span>
                            <span className="text-sm font-semibold text-ink">
                              {step.phase}
                            </span>
                          </span>
                          <span className="shrink-0 font-mono text-[10.5px] uppercase tracking-label text-faint">
                            {step.timeline}
                          </span>
                        </div>
                        <p className="mt-2.5 text-[13.5px] leading-relaxed text-muted">
                          {step.what}
                        </p>
                      </li>
                    ))}
                  </ol>
                </div>

                {/* Footer: confidentiality + CTA */}
                <div className="mt-8 flex flex-col items-start justify-between gap-5 border-t border-line pt-6 sm:flex-row sm:items-center">
                  <p className="flex items-start gap-2.5 text-[13px] text-faint sm:max-w-sm">
                    <Lock className="mt-0.5 h-4 w-4 shrink-0 text-accent" strokeWidth={1.7} />
                    Everything you share is 100% confidential. Every engineer on
                    your project follows strict security and confidentiality rules.
                  </p>
                  <button
                    onClick={() => selectAndScroll(svc.id)}
                    className="btn-primary group shrink-0"
                  >
                    Start your {svc.name} project
                    <ArrowRight className="h-[18px] w-[18px] transition-transform duration-300 group-hover:translate-x-1" />
                  </button>
                </div>
              </motion.div>
            </AnimatePresence>
          </div>
        </div>
      </div>
    </section>
  );
}
