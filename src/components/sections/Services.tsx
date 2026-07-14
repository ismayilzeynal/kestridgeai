"use client";

import { useEffect, useRef, useState, type KeyboardEvent } from "react";
import { AnimatePresence, motion } from "framer-motion";
import { ArrowRight, Check, Lock } from "lucide-react";
import { services } from "@/data/services";
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
    <section id="services" className="relative scroll-mt-[48px] py-12 sm:scroll-mt-8 sm:py-16">
      <div className="container-x">
        {/* Split header: title left, supporting copy right — compact on laptops */}
        <div className="grid items-end gap-x-12 gap-y-4 lg:grid-cols-[1fr_minmax(0,400px)]">
          <div>
            <Reveal>
              <span className="eyebrow">What we do</span>
            </Reveal>
            <Reveal delay={0.06}>
              <h2 className="mt-4 text-balance text-3xl text-signal sm:text-[2.4rem]">
                Services built around{" "}
                <span className="text-accent-grad">your</span> requirements
              </h2>
            </Reveal>
          </div>
          <Reveal delay={0.12}>
            <p className="text-pretty text-[16px] leading-relaxed text-muted lg:pb-1">
              Four disciplines, one way of working — understand it, plan it,
              build it into your environment, and stay until it runs.
            </p>
          </Reveal>
        </div>

        <div className="mt-6 grid gap-5 lg:grid-cols-[300px_1fr]">
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
                      className={`group relative flex min-w-[220px] shrink-0 snap-start items-center gap-3.5 rounded-2xl border px-4 py-4 text-left transition-all duration-300 ease-smooth lg:min-w-0 ${
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
                        className={`grid h-11 w-11 place-items-center rounded-xl border transition-colors duration-300 ${
                          isActive
                            ? "border-line-strong bg-bg-soft text-accent"
                            : "border-line bg-bg-soft text-muted group-hover:text-ink"
                        }`}
                      >
                        <s.icon className="h-5 w-5" strokeWidth={1.6} />
                      </span>
                      <span className="flex-1">
                        <span className="block font-mono text-[11px] text-faint">
                          {s.index}
                        </span>
                        <span
                          className={`block text-[15.5px] font-semibold tracking-tight ${
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
              {/* quiet utility link fills the rail's residual space on desktop */}
              <button
                onClick={() => scrollToId("contact")}
                className="mt-4 hidden w-full items-center justify-between rounded-2xl border border-dashed border-line-strong px-4 py-4 text-left text-[14px] text-muted transition-colors duration-300 hover:border-[color:var(--accent)] hover:text-ink lg:flex"
              >
                Not sure where to start? Talk to an engineer
                <ArrowRight className="h-4 w-4 shrink-0 text-accent" />
              </button>
            </div>
          </Reveal>

          {/* Panel */}
          <div
            id="svc-panel"
            role="tabpanel"
            aria-labelledby={`svc-tab-${svc.id}`}
            tabIndex={0}
            className="card rounded-[1.5rem] p-6 sm:p-7"
          >
            <AnimatePresence mode="wait">
              <motion.div
                key={svc.id}
                initial={{ opacity: 0, y: 12 }}
                animate={{ opacity: 1, y: 0 }}
                exit={{ opacity: 0, y: -8 }}
                transition={{ duration: 0.35, ease }}
              >
                <h3 className="text-balance text-[1.45rem] font-bold tracking-tight text-ink sm:text-[1.65rem]">
                  {svc.tagline}
                </h3>

                <p className="mt-3 max-w-3xl text-pretty text-[15.5px] leading-relaxed text-muted">
                  {svc.description}
                </p>

                {/* Highlights — quiet inline list, no pill chrome */}
                <ul className="mt-5 flex flex-wrap gap-x-7 gap-y-2.5">
                  {svc.highlights.map((h) => (
                    <li
                      key={h}
                      className="inline-flex items-center gap-2 text-[14.5px] font-medium text-ink"
                    >
                      <Check className="h-4 w-4 text-accent" strokeWidth={2.4} />
                      {h}
                    </li>
                  ))}
                </ul>

                {/* Delivery stepper — connected timeline on desktop */}
                <ol className="relative mt-7 hidden grid-cols-4 gap-6 lg:grid">
                  {/* connector line, behind the nodes */}
                  <span
                    aria-hidden
                    className="absolute left-0 right-6 top-[15px] h-px bg-[color:var(--border-strong)]"
                  />
                  {svc.steps.map((step, i) => (
                    <li key={step.phase} className="relative">
                      <span className="relative z-10 grid h-8 w-8 place-items-center rounded-full border-2 border-accent bg-surface font-mono text-[12.5px] font-medium text-accent">
                        {i + 1}
                      </span>
                      <h4 className="mt-3.5 text-[15px] font-semibold tracking-tight text-ink">
                        {step.phase}
                      </h4>
                      <p className="mt-1 text-[13.5px] leading-snug text-muted">
                        {step.summary}
                      </p>
                      <span className="mt-2 block font-mono text-[11px] uppercase tracking-wide text-faint">
                        {step.timeline}
                      </span>
                    </li>
                  ))}
                </ol>

                {/* Delivery steps — vertical timeline on smaller screens */}
                <ol className="mt-8 flex flex-col lg:hidden">
                  {svc.steps.map((step, i) => (
                    <li key={step.phase} className="relative flex gap-4 pb-6 last:pb-0">
                      {i < svc.steps.length - 1 && (
                        <span
                          aria-hidden
                          className="absolute left-[15px] top-8 bottom-0 w-px bg-[color:var(--border-strong)]"
                        />
                      )}
                      <span className="relative z-10 grid h-8 w-8 shrink-0 place-items-center rounded-full border-2 border-accent bg-surface font-mono text-[12.5px] font-medium text-accent">
                        {i + 1}
                      </span>
                      <div className="pt-1">
                        <div className="flex flex-wrap items-baseline gap-x-3 gap-y-1">
                          <h4 className="text-[15.5px] font-semibold tracking-tight text-ink">
                            {step.phase}
                          </h4>
                          <span className="font-mono text-[11px] uppercase tracking-wide text-faint">
                            {step.timeline}
                          </span>
                        </div>
                        <p className="mt-1.5 text-[14px] leading-relaxed text-muted">
                          {step.what}
                        </p>
                      </div>
                    </li>
                  ))}
                </ol>

                {/* Footer: confidentiality + CTA */}
                <div className="mt-6 flex flex-col items-start justify-between gap-4 border-t border-line pt-4 sm:flex-row sm:items-center">
                  <p className="flex items-center gap-2.5 text-[13.5px] text-faint">
                    <Lock className="h-4 w-4 shrink-0 text-accent" strokeWidth={1.7} />
                    NDA-first — confidential on every engagement.
                  </p>
                  <button
                    onClick={() => selectAndScroll(svc.id)}
                    className="btn-primary group shrink-0 !py-3.5 !px-6 text-[15px]"
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
