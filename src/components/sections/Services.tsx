"use client";

import { useEffect, useRef, useState, type KeyboardEvent } from "react";
import { AnimatePresence, motion } from "framer-motion";
import { ArrowRight, Check, ChevronDown, Lock } from "lucide-react";
import { type Service } from "@/data/services";
import { resolveServiceIcon } from "@/lib/icons";
import { Reveal } from "@/components/ui/Reveal";
import { scrollToId } from "@/lib/scroll";

const ease = [0.22, 1, 0.36, 1] as const;

function startProject(id: string) {
  window.dispatchEvent(new CustomEvent("prefill-service", { detail: id }));
  scrollToId("contact");
}

export function Services({ services }: { services: Service[] }) {
  const [active, setActive] = useState(0);
  const [openId, setOpenId] = useState<string | null>(null); // mobile accordion
  const svc = services[active];
  const tabRefs = useRef<(HTMLButtonElement | null)[]>([]);

  // Hero "Where we help" cards drive both layouts.
  useEffect(() => {
    const handler = (e: Event) => {
      const id = (e as CustomEvent<string>).detail;
      const idx = services.findIndex((s) => s.id === id);
      if (idx >= 0) {
        setActive(idx);
        setOpenId(id);
      }
    };
    window.addEventListener("select-service-tab", handler);
    return () => window.removeEventListener("select-service-tab", handler);
  }, [services]);

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

  // Second line of defence. validate() in src/lib/content.ts already refuses
  // an empty services array, and if both ever failed this line would take the
  // whole route down during render at svc.id below.
  if (!svc) {
    return null;
  }

  return (
    <section id="services" className="relative scroll-mt-12 py-12 sm:scroll-mt-8 sm:py-16 lg:scroll-mt-12 lg:py-12">
      <div className="container-x">
        <Reveal>
          <h2 className="text-balance text-4xl text-signal sm:text-5xl">
            <span className="text-accent-grad">Core</span> services
          </h2>
        </Reveal>

        {/* ============ MOBILE / TABLET: vertical accordion ============ */}
        <div className="mt-7 flex flex-col gap-3 lg:hidden">
          {services.map((s, i) => {
            const open = openId === s.id;
            const Icon = resolveServiceIcon(s.icon);
            return (
              <div
                key={s.id}
                id={`svc-acc-${s.id}`}
                className={`card overflow-hidden rounded-2xl transition-colors duration-300 ${
                  open ? "border-line-strong" : ""
                }`}
              >
                <button
                  onClick={() => {
                    const next = open ? null : s.id;
                    setOpenId(next);
                    if (next) {
                      // keep the opened card in view once it expands
                      window.setTimeout(() => {
                        document
                          .getElementById(`svc-acc-${s.id}`)
                          ?.scrollIntoView({ block: "nearest", behavior: "smooth" });
                      }, 380);
                    }
                  }}
                  id={`svc-acc-btn-${s.id}`}
                  aria-expanded={open}
                  aria-controls={`svc-acc-panel-${s.id}`}
                  className="flex w-full items-center gap-3.5 px-4 py-4 text-left"
                >
                  <span
                    className={`grid h-11 w-11 shrink-0 place-items-center rounded-xl border transition-colors duration-300 ${
                      open
                        ? "border-line-strong bg-bg-soft text-accent"
                        : "border-line bg-bg-soft text-muted"
                    }`}
                  >
                    <Icon className="h-5 w-5" strokeWidth={1.6} />
                  </span>
                  <span className="flex-1">
                    <span className="block font-mono text-[0.6471rem] text-faint">
                      {String(i + 1).padStart(2, "0")}
                    </span>
                    <span className="block text-[0.9706rem] font-semibold tracking-tight text-ink">
                      {s.name}
                    </span>
                  </span>
                  <ChevronDown
                    className={`h-5 w-5 shrink-0 text-faint transition-transform duration-300 ${
                      open ? "rotate-180 text-accent" : ""
                    }`}
                  />
                </button>

                {/* Persistent wrapper so aria-controls always resolves, even
                    while the panel is collapsed. */}
                <div
                  id={`svc-acc-panel-${s.id}`}
                  role="region"
                  aria-labelledby={`svc-acc-btn-${s.id}`}
                >
                <AnimatePresence initial={false}>
                  {open && (
                    <motion.div
                      key="panel"
                      initial={{ height: 0, opacity: 0 }}
                      animate={{ height: "auto", opacity: 1 }}
                      exit={{ height: 0, opacity: 0 }}
                      transition={{ duration: 0.35, ease }}
                    >
                      <div className="border-t border-line px-4 pb-5 pt-4">
                        <AccordionBody svc={s} />
                      </div>
                    </motion.div>
                  )}
                </AnimatePresence>
                </div>
              </div>
            );
          })}
        </div>

        {/* ============ DESKTOP: tab rail + panel ============ */}
        <div className="mt-5 hidden gap-5 lg:grid lg:grid-cols-[17.6471rem_minmax(0,1fr)]">
          {/* Tab rail */}
          <Reveal>
            <div>
              <div
                role="tablist"
                aria-label="Services"
                aria-orientation="vertical"
                onKeyDown={onTabKeyDown}
                className="flex flex-col gap-2"
              >
                {services.map((s, i) => {
                  const isActive = i === active;
                  const Icon = resolveServiceIcon(s.icon);
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
                      className={`group relative flex items-center gap-3.5 rounded-2xl border px-4 py-3.5 text-left transition-all duration-300 ease-smooth ${
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
                        <Icon className="h-5 w-5" strokeWidth={1.6} />
                      </span>
                      <span className="flex-1">
                        <span className="block font-mono text-[0.6471rem] text-faint">
                          {String(i + 1).padStart(2, "0")}
                        </span>
                        <span
                          className={`block text-[0.9118rem] font-semibold tracking-tight ${
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
            </div>
          </Reveal>

          {/* Panel */}
          <div
            id="svc-panel"
            role="tabpanel"
            aria-labelledby={`svc-tab-${svc.id}`}
            tabIndex={0}
            className="card rounded-[1.5rem] p-6"
          >
            <AnimatePresence mode="wait" initial={false}>
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

                <p className="mt-2.5 max-w-3xl text-pretty text-[0.9118rem] leading-relaxed text-muted">
                  {svc.description}
                </p>

                <ul className="mt-4 flex flex-wrap gap-x-7 gap-y-2.5">
                  {svc.highlights.map((h) => (
                    <li
                      key={h}
                      className="inline-flex items-center gap-2 text-[0.8529rem] font-medium text-ink"
                    >
                      <Check className="h-4 w-4 text-accent" strokeWidth={2.4} />
                      {h}
                    </li>
                  ))}
                </ul>

                {/* Delivery stepper - connected timeline */}
                <ol className="relative mt-5 grid grid-cols-4 gap-6">
                  <span
                    aria-hidden
                    className="absolute left-0 right-6 top-[calc(0.875rem-0.5px)] h-px bg-[color:var(--border-strong)]"
                  />
                  {svc.steps.map((step, i) => (
                    <li key={step.phase} className="relative">
                      <span className="relative z-10 grid h-7 w-7 place-items-center rounded-full border-2 border-accent bg-surface font-mono text-[0.7353rem] font-medium text-accent">
                        {i + 1}
                      </span>
                      <h4 className="mt-2.5 text-[0.8824rem] font-semibold tracking-tight text-ink">
                        {step.phase}
                      </h4>
                      <p className="mt-1 text-[0.7941rem] leading-snug text-muted">
                        {step.summary}
                      </p>
                    </li>
                  ))}
                </ol>

                <p className="mt-3 text-[0.7647rem] text-faint">
                  Each step is scheduled in the written plan before work begins.
                </p>

                {/* Footer: confidentiality + contact */}
                <div className="mt-4 flex flex-col items-start justify-between gap-4 border-t border-line pt-3 sm:flex-row sm:items-center">
                  <p className="flex items-center gap-2.5 text-[0.7941rem] text-faint">
                    <Lock className="h-4 w-4 shrink-0 text-accent" strokeWidth={1.7} />
                    Your information stays confidential.
                  </p>
                  <button
                    onClick={() => startProject(svc.id)}
                    className="btn-primary group shrink-0 !py-3 !px-6 text-[0.8824rem]"
                  >
                    Contact us about {svc.name}
                    <ArrowRight className="h-[1.0588rem] w-[1.0588rem] transition-transform duration-300 group-hover:translate-x-1" />
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

/** Expanded accordion content - tuned for small screens. */
function AccordionBody({ svc }: { svc: Service }) {
  return (
    <div>
      <h3 className="text-balance text-[1.1176rem] font-bold tracking-tight text-ink">
        {svc.tagline}
      </h3>
      <p className="mt-2 text-pretty text-[0.8824rem] leading-relaxed text-muted">
        {svc.description}
      </p>

      <ul className="mt-4 flex flex-col gap-2">
        {svc.highlights.map((h) => (
          <li
            key={h}
            className="inline-flex items-center gap-2.5 text-[0.8529rem] font-medium text-ink"
          >
            <Check className="h-4 w-4 shrink-0 text-accent" strokeWidth={2.4} />
            {h}
          </li>
        ))}
      </ul>

      {/* Vertical delivery timeline */}
      <div className="mt-6">
        <div className="mb-3 flex items-center gap-3">
          <span className="font-mono text-[0.6176rem] uppercase tracking-label text-faint">
            Project steps
          </span>
          <span className="hairline flex-1" />
        </div>
        <ol className="flex flex-col">
          {svc.steps.map((step, i) => (
            <li key={step.phase} className="relative flex gap-4 pb-6 last:pb-1">
              {i < svc.steps.length - 1 && (
                <span
                  aria-hidden
                  className="absolute bottom-0 left-[0.8824rem] top-8 w-px bg-[color:var(--border-strong)]"
                />
              )}
              <span className="relative z-10 grid h-8 w-8 shrink-0 place-items-center rounded-full border-2 border-accent bg-surface font-mono text-[0.7353rem] font-medium text-accent">
                {i + 1}
              </span>
              <div className="pt-1">
                <h4 className="text-[0.9118rem] font-semibold tracking-tight text-ink">
                  {step.phase}
                </h4>
                <p className="mt-1.5 text-[0.8235rem] leading-relaxed text-muted">
                  {step.what}
                </p>
              </div>
            </li>
          ))}
        </ol>
      </div>

      <p className="mt-1 text-[0.7647rem] text-faint">
        Each step is scheduled in the written plan before work begins.
      </p>

      <button
        onClick={() => startProject(svc.id)}
        className="btn-primary group mt-5 w-full !py-3.5 text-[0.8824rem]"
      >
        Contact us about {svc.name}
        <ArrowRight className="h-[1.0588rem] w-[1.0588rem] transition-transform duration-300 group-hover:translate-x-1" />
      </button>
      <p className="mt-3 flex items-center justify-center gap-2 text-center text-[0.7353rem] text-faint">
        <Lock className="h-3.5 w-3.5 shrink-0 text-accent" strokeWidth={1.7} />
        Your information stays confidential.
      </p>
    </div>
  );
}
