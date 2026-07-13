"use client";

import { motion } from "framer-motion";
import { ArrowRight, ArrowUpRight } from "lucide-react";
import { services } from "@/data/services";
import { openService, scrollToId } from "@/lib/scroll";

const ease = [0.22, 1, 0.36, 1] as const;

export function Hero() {
  return (
    <section id="top" className="relative overflow-hidden pb-16 pt-32 sm:pt-40">
      <div className="container-x">
        <div className="grid items-center gap-14 lg:grid-cols-[1.05fr_0.95fr]">
          {/* Left — message */}
          <div>
            <motion.div
              initial={{ opacity: 0, y: 14 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ duration: 0.6, ease }}
              className="eyebrow"
            >
              United States · Applied AI &amp; Engineering
            </motion.div>

            <h1 className="mt-7 text-balance text-[clamp(2.3rem,8.4vw,4.4rem)] font-bold leading-[1.05]">
              <Line delay={0.06}>Enterprise AI, automation</Line>
              <Line delay={0.14}>and security,</Line>
              <Line delay={0.22}>
                <span className="text-accent-grad">engineered around you.</span>
              </Line>
            </h1>

            <motion.p
              initial={{ opacity: 0, y: 16 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ duration: 0.7, delay: 0.34, ease }}
              className="mt-8 max-w-xl text-pretty text-[19px] leading-relaxed text-muted"
            >
              An American engineering team that works closely with you,
              understands exactly what you need, and delivers it — then stays
              until everything runs the way it should.
            </motion.p>

            <motion.div
              initial={{ opacity: 0, y: 16 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ duration: 0.7, delay: 0.46, ease }}
              className="mt-10 flex flex-wrap items-center gap-3"
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
            </motion.div>
          </div>

          {/* Right — capability console */}
          <motion.div
            initial={{ opacity: 0, y: 30 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.8, delay: 0.3, ease }}
            className="relative"
          >
            <div className="card glow-accent overflow-hidden rounded-[1.6rem] p-1.5">
              <div className="rounded-[1.25rem] border border-line bg-bg-soft p-2">
                <div className="flex items-center justify-between px-4 py-3.5">
                  <span className="font-mono text-[12px] uppercase tracking-[0.18em] text-muted">
                    Where we help
                  </span>
                  <span className="flex items-center gap-2 font-mono text-[12px] text-faint">
                    <span className="relative flex h-1.5 w-1.5" aria-hidden>
                      <span className="absolute inline-flex h-full w-full animate-pulse-ring rounded-full bg-accent" />
                      <span className="relative inline-flex h-1.5 w-1.5 rounded-full bg-accent" />
                    </span>
                    online
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
                      <span className="grid h-10 w-10 place-items-center rounded-lg border border-line bg-surface text-accent transition-colors duration-300 group-hover:border-line-strong">
                        <s.icon className="h-5 w-5" strokeWidth={1.6} />
                      </span>
                      <span className="flex-1">
                        <span className="block text-[16px] font-semibold text-ink">
                          {s.name}
                        </span>
                        <span className="block text-[13px] text-faint">
                          {s.tagline.replace(/^We (build|help you secure) /i, "")}
                        </span>
                      </span>
                      <ArrowUpRight className="h-[18px] w-[18px] text-faint transition-all duration-300 group-hover:text-accent group-hover:translate-x-0.5 group-hover:-translate-y-0.5" />
                    </button>
                  ))}
                </div>
              </div>
            </div>
          </motion.div>
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
      <motion.span
        className="block text-signal"
        initial={{ y: "110%" }}
        animate={{ y: 0 }}
        transition={{ duration: 0.8, delay, ease }}
      >
        {children}
      </motion.span>
    </span>
  );
}
