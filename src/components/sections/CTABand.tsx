"use client";

import { motion } from "framer-motion";
import { ArrowRight } from "lucide-react";
import { LinkedInButton } from "@/components/ui/LinkedInButton";

export function CTABand() {
  return (
    <section className="relative py-16 sm:py-24">
      <div className="container-x">
        <div className="relative overflow-hidden rounded-[2rem] border border-line-strong bg-gradient-to-b from-surface to-bg-soft px-7 py-14 text-center sm:px-12 sm:py-20">
          <div
            aria-hidden
            className="pointer-events-none absolute inset-x-0 -top-32 mx-auto h-64 w-[80%] rounded-full opacity-25 blur-[110px]"
            style={{ background: "radial-gradient(closest-side, var(--accent), transparent)" }}
          />
          <div className="bg-grid bg-grid-fade absolute inset-0 opacity-60" aria-hidden />

          <div className="relative mx-auto max-w-2xl">
            <motion.span
              initial={{ opacity: 0, y: 10 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true }}
              transition={{ duration: 0.6 }}
              className="eyebrow justify-center"
            >
              Ready when you are
            </motion.span>
            <motion.h2
              initial={{ opacity: 0, y: 16 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true }}
              transition={{ duration: 0.7, delay: 0.06, ease: [0.22, 1, 0.36, 1] }}
              className="mt-5 text-balance text-4xl text-signal sm:text-[3.2rem] sm:leading-[1.05]"
            >
              Have something that needs{" "}
              <span className="text-accent-grad italic">solving?</span>
            </motion.h2>
            <motion.p
              initial={{ opacity: 0 }}
              whileInView={{ opacity: 1 }}
              viewport={{ once: true }}
              transition={{ duration: 0.7, delay: 0.16 }}
              className="mx-auto mt-5 max-w-lg text-pretty text-lg leading-relaxed text-muted"
            >
              One short conversation is all it takes to find out how we can help.
              No pressure, no jargon — just engineers who listen.
            </motion.p>
            <motion.div
              initial={{ opacity: 0, y: 14 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true }}
              transition={{ duration: 0.7, delay: 0.24 }}
              className="mt-9 flex flex-wrap items-center justify-center gap-3"
            >
              <a href="#contact" className="btn-primary group">
                Start a project
                <ArrowRight className="h-[18px] w-[18px] transition-transform duration-300 group-hover:translate-x-1" />
              </a>
              <LinkedInButton />
            </motion.div>
          </div>
        </div>
      </div>
    </section>
  );
}
