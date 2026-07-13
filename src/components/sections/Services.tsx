"use client";

import { useEffect, useRef, useState, type KeyboardEvent } from "react";
import { AnimatePresence, motion } from "framer-motion";
import { ArrowRight, Check, Lock } from "lucide-react";
import { services } from "@/data/services";
import { SectionHeading } from "@/components/ui/SectionHeading";
import { Reveal } from "@/components/ui/Reveal";
import { scrollToId } from "@/lib/scroll";

const ease = [0.22, 1, 0.36, 1] as const;

export function Services() {
  const [active, setActive] = useState(0);
  const svc = services[active];
  const tabRefs = useRef<(HTMLButtonElement | null)[]>([]);

  // Let the hero "Where we help" cards drive the active tab.
  useEffect(() => {
    const handler = (e: Event) => {
      const id = (e as CustomEvent<string>).detail;
      const idx = services.findIndex((s) => s.id === id);
      if (idx >= 0) setActive(idx);
    };
    window.addEventListener("select-service-tab", handler);
    return () => window.removeEventListener("select-service-tab", handler);
  }, []);

  const selectAndScroll = (id: string) => {
    window.dispatchEvent(new CustomEvent("prefill-service", { detail: id }));
    scrollToId("contact");
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
    <section id="services" className="relative py-12 sm:py-16">
      <div className="container-x">
        <SectionHeading
          compact
          eyebrow="What we do"
          title={
            <>
              Services built around{" "}
              <span className="text-accent-grad italic">your</span> requirements
            </>
          }
          description="Four disciplines, one way of working — understand it, plan it, build it into your environment, and stay until it runs."
        />

        <div className="mt-7 grid gap-5 lg:grid-cols-[300px_1fr]">
          {/* Tab rail */}
          <Reveal>
            <div className="relative">
              <div
                role="tablist"
                aria-label="Services"
                aria-orientation="vertical"
                onKeyDown={onTabKeyDown}
                className="flex gap-3 overflow-x-auto pb-2 [scrollbar-width:none] [-ms-overflow-style:none] [&::-webkit-scrollbar]:hidden lg:flex-col lg:gap-2 lg:overflow-visible lg:pb-0 snap-x snap-mandatory"
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
                      className={`group relative flex min-w-[210px] shrink-0 snap-start items-center gap-3.5 rounded-2xl border px-4 py-3.5 text-left transition-all duration-300 ease-smooth lg:min-w-0 ${
                        isActive
                          ? "border-line-strong bg-surface shadow-sm"
                          : "border-line bg-surface/50 hover:bg-surface"
                      }`}
                    >
                      {isActive && (
                        <motion.span
                          layoutId="svc-active"
                          className="absolute inset-y-2.5 left-0 w-[3px] rounded-full bg-accent"
                        />
                      )}
                      <span
                        className={`grid h-10 w-10 place-items-center rounded-xl border transition-colors duration-300 ${
                          isActive
                            ? "border-line-strong bg-bg-soft text-accent"
                            : "border-line bg-bg-soft text-muted group-hover:text-ink"
                        }`}
                      >
                        <s.icon className="h-[18px] w-[18px]" strokeWidth={1.6} />
                      </span>
                      <span className="flex-1">
                        <span className="block font-mono text-[10.5px] text-faint">
                          {s.index}
                        </span>
                        <span
                          className={`block text-sm font-semibold tracking-tight ${
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
            className="card rounded-[1.5rem] p-5 sm:p-6"
          >
            <AnimatePresence mode="wait">
              <motion.div
                key={svc.id}
                initial={{ opacity: 0, y: 12 }}
                animate={{ opacity: 1, y: 0 }}
                exit={{ opacity: 0, y: -8 }}
                transition={{ duration: 0.35, ease }}
              >
                <div className="flex flex-wrap items-baseline justify-between gap-x-4 gap-y-1">
                  <h3 className="text-balance font-display text-xl text-ink sm:text-[1.5rem] sm:leading-tight">
                    {svc.tagline}
                  </h3>
                  <span className="font-mono text-[11px] uppercase tracking-label text-accent">
                    {svc.index} / {svc.name}
                  </span>
                </div>

                <p className="mt-2.5 line-clamp-3 max-w-2xl text-pretty text-[14.5px] leading-relaxed text-muted lg:line-clamp-2">
                  {svc.description}
                </p>

                <div className="mt-3.5 flex flex-wrap gap-2">
                  {svc.highlights.map((h) => (
                    <span
                      key={h}
                      className="inline-flex items-center gap-1.5 rounded-full border border-line bg-bg-soft px-3 py-1 text-[12.5px] text-muted"
                    >
                      <Check className="h-3.5 w-3.5 text-accent" strokeWidth={2.4} />
                      {h}
                    </span>
                  ))}
                </div>

                {/* Delivery steps */}
                <div className="mt-5">
                  <div className="mb-2.5 flex items-center gap-3">
                    <span className="font-mono text-[10.5px] uppercase tracking-label text-faint">
                      How we deliver
                    </span>
                    <span className="hairline flex-1" />
                  </div>
                  <ol className="grid gap-2 sm:grid-cols-2">
                    {svc.steps.map((step, i) => (
                      <li
                        key={step.phase}
                        className="rounded-xl border border-line bg-bg-soft/70 p-3"
                      >
                        <div className="flex items-center justify-between gap-3">
                          <span className="flex items-center gap-2">
                            <span className="grid h-5 w-5 place-items-center rounded-md bg-surface font-mono text-[10.5px] text-accent shadow-sm">
                              {i + 1}
                            </span>
                            <span className="text-[13.5px] font-semibold text-ink">
                              {step.phase}
                            </span>
                          </span>
                          <span className="shrink-0 font-mono text-[10px] uppercase tracking-wide text-faint">
                            {step.timeline}
                          </span>
                        </div>
                        <p className="mt-1.5 line-clamp-2 text-[12.5px] leading-snug text-muted">
                          {step.what}
                        </p>
                      </li>
                    ))}
                  </ol>
                </div>

                {/* Footer: confidentiality + CTA */}
                <div className="mt-4 flex flex-col items-start justify-between gap-4 border-t border-line pt-4 sm:flex-row sm:items-center">
                  <p className="flex items-start gap-2 text-[12.5px] text-faint sm:max-w-sm">
                    <Lock className="mt-0.5 h-4 w-4 shrink-0 text-accent" strokeWidth={1.7} />
                    Everything you share is 100% confidential — every engineer
                    follows strict security and confidentiality rules.
                  </p>
                  <button
                    onClick={() => selectAndScroll(svc.id)}
                    className="btn-primary group shrink-0 !py-3 !px-5 text-sm"
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
