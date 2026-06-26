"use client";

import { useEffect, useRef, useState, type FormEvent } from "react";
import { AnimatePresence, motion } from "framer-motion";
import {
  ArrowRight,
  ArrowUpRight,
  Mail,
  MapPin,
  ShieldCheck,
  CheckCircle2,
  Loader2,
  ChevronDown,
} from "lucide-react";
import { site, serviceOptions } from "@/lib/site";
import { SectionHeading } from "@/components/ui/SectionHeading";
import { Reveal } from "@/components/ui/Reveal";
import { LinkedInButton } from "@/components/ui/LinkedInButton";

type Status = "idle" | "submitting" | "success";
type Errors = Partial<Record<"name" | "email" | "service" | "message", string>>;

const ease = [0.22, 1, 0.36, 1] as const;

export function Contact() {
  const [status, setStatus] = useState<Status>("idle");
  const [errors, setErrors] = useState<Errors>({});
  const [service, setService] = useState("");
  const [name, setName] = useState("");
  const formRef = useRef<HTMLFormElement>(null);
  const timerRef = useRef<ReturnType<typeof setTimeout>>();

  // Preselect the service when a "Start a … project" button is clicked.
  useEffect(() => {
    const handler = (e: Event) => {
      const detail = (e as CustomEvent<string>).detail;
      if (detail) setService(detail);
    };
    window.addEventListener("prefill-service", handler);
    return () => window.removeEventListener("prefill-service", handler);
  }, []);

  // Clear any pending simulated-submit timer on unmount.
  useEffect(() => () => clearTimeout(timerRef.current), []);

  const validate = (data: FormData): Errors => {
    const next: Errors = {};
    const n = (data.get("name") as string)?.trim();
    const email = (data.get("email") as string)?.trim();
    const svc = data.get("service") as string;
    const msg = (data.get("message") as string)?.trim();
    if (!n) next.name = "Please tell us your name.";
    if (!email) next.email = "We need an email to reach you.";
    else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email))
      next.email = "That email doesn't look right.";
    if (!svc) next.service = "Pick the area you're interested in.";
    if (!msg || msg.length < 10)
      next.message = "A sentence or two about your project helps a lot.";
    return next;
  };

  const onSubmit = (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const data = new FormData(e.currentTarget);
    const found = validate(data);
    setErrors(found);
    if (Object.keys(found).length > 0) return;

    setName((data.get("name") as string)?.trim() || "");
    setStatus("submitting");
    // Frontend-only: simulate a request. Wire to a backend later.
    timerRef.current = setTimeout(() => setStatus("success"), 1100);
  };

  const reset = () => {
    clearTimeout(timerRef.current);
    setStatus("idle");
    setErrors({});
    setService("");
    formRef.current?.reset();
  };

  return (
    <section id="contact" className="relative scroll-mt-24 py-24 sm:py-32">
      <div className="container-x">
        <div className="grid gap-12 lg:grid-cols-[0.85fr_1.15fr]">
          {/* Left — invitation + contact rails */}
          <div className="lg:sticky lg:top-28 lg:max-h-[calc(100vh-8rem)] lg:self-start lg:overflow-y-auto">

            <SectionHeading
              eyebrow="Let's talk"
              title={
                <>
                  Tell us what you&apos;re{" "}
                  <span className="text-accent-grad italic">trying to solve</span>
                </>
              }
              description={`A short description of your project and how to reach you is all we need. We'll get back to you within ${site.responseTime}.`}
            />

            <div className="mt-9 flex flex-col gap-3">
              <a
                href={`mailto:${site.email}`}
                className="card card-hover group flex items-center gap-4 rounded-2xl p-4"
              >
                <span className="grid h-11 w-11 place-items-center rounded-xl border border-line bg-surface text-accent">
                  <Mail className="h-5 w-5" strokeWidth={1.6} />
                </span>
                <span className="flex-1">
                  <span className="block font-mono text-[11px] uppercase tracking-label text-faint">
                    Email
                  </span>
                  <span className="text-[15px] text-ink">{site.email}</span>
                </span>
                <ArrowRight className="h-4 w-4 text-faint transition-all duration-300 group-hover:translate-x-0.5 group-hover:text-accent" />
              </a>

              <a
                href={site.linkedin}
                target="_blank"
                rel="noopener noreferrer"
                className="card card-hover group flex items-center gap-4 rounded-2xl p-4"
              >
                <span className="grid h-11 w-11 place-items-center rounded-xl border border-line bg-surface text-accent">
                  <svg viewBox="0 0 24 24" className="h-5 w-5 fill-current" aria-hidden>
                    <path d="M20.45 20.45h-3.56v-5.57c0-1.33-.02-3.04-1.85-3.04-1.85 0-2.13 1.45-2.13 2.94v5.67H9.35V9h3.41v1.56h.05c.48-.9 1.64-1.85 3.37-1.85 3.6 0 4.27 2.37 4.27 5.45v6.29zM5.34 7.43a2.06 2.06 0 1 1 0-4.13 2.06 2.06 0 0 1 0 4.13zM7.12 20.45H3.56V9h3.56v11.45zM22.22 0H1.77C.79 0 0 .77 0 1.73v20.54C0 23.22.79 24 1.77 24h20.45c.98 0 1.78-.78 1.78-1.73V1.73C24 .77 23.2 0 22.22 0z" />
                  </svg>
                </span>
                <span className="flex-1">
                  <span className="block font-mono text-[11px] uppercase tracking-label text-faint">
                    LinkedIn
                  </span>
                  <span className="text-[15px] text-ink">Follow our work</span>
                </span>
                <ArrowRight className="h-4 w-4 text-faint transition-all duration-300 group-hover:translate-x-0.5 group-hover:text-accent" />
              </a>

              <div className="flex items-center gap-4 rounded-2xl border border-line p-4">
                <span className="grid h-11 w-11 place-items-center rounded-xl border border-line bg-surface text-accent">
                  <MapPin className="h-5 w-5" strokeWidth={1.6} />
                </span>
                <span className="flex-1">
                  <span className="block font-mono text-[11px] uppercase tracking-label text-faint">
                    Based in
                  </span>
                  <span className="text-[15px] text-ink">{site.location}</span>
                </span>
              </div>
            </div>

            <p className="mt-5 flex items-start gap-2.5 text-[13px] text-faint">
              <ShieldCheck className="mt-0.5 h-4 w-4 shrink-0 text-accent" strokeWidth={1.7} />
              Everything you share here is kept strictly confidential.
            </p>
          </div>

          {/* Right — the form */}
          <Reveal delay={0.06}>
            <div className="card relative overflow-hidden rounded-[1.6rem] p-6 sm:p-9">
              <AnimatePresence mode="wait">
                {status === "success" ? (
                  <motion.div
                    key="success"
                    initial={{ opacity: 0, y: 16 }}
                    animate={{ opacity: 1, y: 0 }}
                    exit={{ opacity: 0 }}
                    transition={{ duration: 0.5, ease }}
                    className="flex min-h-[420px] flex-col items-center justify-center text-center"
                  >
                    <span className="relative mb-6 grid h-16 w-16 place-items-center rounded-full border border-line-strong bg-surface text-accent">
                      <span className="absolute inset-0 rounded-full glow-accent" />
                      <CheckCircle2 className="h-8 w-8" strokeWidth={1.6} />
                    </span>
                    <h3 className="font-display text-3xl text-ink">
                      Thank you{name ? `, ${name.split(" ")[0]}` : ""}.
                    </h3>
                    <p className="mt-3 max-w-sm text-pretty leading-relaxed text-muted">
                      Your message is in. We&apos;ll review it and get back to you
                      within {site.responseTime}.
                    </p>
                    <div className="mt-8 flex flex-col items-center gap-4">
                      <p className="text-sm text-faint">
                        While you wait — stay close to what we&apos;re building.
                      </p>
                      <div className="flex flex-col items-center gap-3 sm:flex-row">
                        <LinkedInButton variant="primary" />
                        <a href={`mailto:${site.email}`} className="btn-ghost group">
                          Email us directly
                          <ArrowUpRight className="h-[18px] w-[18px] transition-transform duration-300 ease-smooth group-hover:translate-x-0.5 group-hover:-translate-y-0.5" />
                        </a>
                      </div>
                      <button
                        onClick={reset}
                        className="text-sm text-muted underline-offset-4 transition-colors hover:text-ink hover:underline"
                      >
                        Send another message
                      </button>
                    </div>
                  </motion.div>
                ) : (
                  <motion.form
                    key="form"
                    ref={formRef}
                    onSubmit={onSubmit}
                    noValidate
                    initial={{ opacity: 0 }}
                    animate={{ opacity: 1 }}
                    exit={{ opacity: 0 }}
                    className="flex flex-col gap-5"
                  >
                    <div className="grid gap-5 sm:grid-cols-2">
                      <Field
                        label="Full name"
                        name="name"
                        placeholder="Jane Doe"
                        autoComplete="name"
                        required
                        error={errors.name}
                      />
                      <Field
                        label="Email"
                        name="email"
                        type="email"
                        placeholder="jane@company.com"
                        autoComplete="email"
                        required
                        error={errors.email}
                      />
                    </div>

                    <div className="grid gap-5 sm:grid-cols-2">
                      <Field
                        label="Company"
                        name="company"
                        placeholder="Company name"
                        optional
                        autoComplete="organization"
                      />
                      <Field
                        label="Phone"
                        name="phone"
                        type="tel"
                        placeholder="(555) 000-0000"
                        optional
                        autoComplete="tel"
                      />
                    </div>

                    {/* Service select */}
                    <div className="flex flex-col gap-2">
                      <Label htmlFor="service">What can we help with?</Label>
                      <div className="relative">
                        <select
                          id="service"
                          name="service"
                          value={service}
                          onChange={(e) => setService(e.target.value)}
                          aria-invalid={!!errors.service}
                          aria-describedby={errors.service ? "service-error" : undefined}
                          className={`peer w-full appearance-none rounded-xl border bg-bg-soft px-4 py-3.5 text-[15px] text-ink outline-none transition-colors duration-200 focus-visible:border-accent focus-visible:ring-2 focus-visible:ring-accent ${
                            service ? "text-ink" : "text-faint"
                          } ${errors.service ? "border-red-400/60" : "border-line"}`}
                        >
                          <option value="" disabled hidden>
                            Select an area…
                          </option>
                          {serviceOptions.map((o) => (
                            <option key={o.value} value={o.value} className="text-ink">
                              {o.label}
                            </option>
                          ))}
                        </select>
                        <ChevronDown className="pointer-events-none absolute right-4 top-1/2 h-4 w-4 -translate-y-1/2 text-faint peer-focus:text-accent" />
                      </div>
                      {errors.service && (
                        <ErrorText id="service-error">{errors.service}</ErrorText>
                      )}
                    </div>

                    {/* Message */}
                    <div className="flex flex-col gap-2">
                      <Label htmlFor="message">Brief project description</Label>
                      <textarea
                        id="message"
                        name="message"
                        rows={4}
                        placeholder="A sentence or two about what you're trying to build or solve…"
                        aria-invalid={!!errors.message}
                        aria-describedby={errors.message ? "message-error" : undefined}
                        className={`w-full resize-none rounded-xl border bg-bg-soft px-4 py-3.5 text-[15px] text-ink outline-none transition-colors duration-200 placeholder:text-faint focus-visible:border-accent focus-visible:ring-2 focus-visible:ring-accent ${
                          errors.message ? "border-red-400/60" : "border-line"
                        }`}
                      />
                      {errors.message && (
                        <ErrorText id="message-error">{errors.message}</ErrorText>
                      )}
                    </div>

                    <p className="-mb-1 flex flex-wrap items-center justify-center gap-x-2.5 gap-y-1 text-center font-mono text-[11px] uppercase tracking-label text-faint">
                      <span>Free first conversation</span>
                      <span className="text-line-strong">·</span>
                      <span>No commitment</span>
                      <span className="text-line-strong">·</span>
                      <span>Confidential</span>
                    </p>

                    <button
                      type="submit"
                      disabled={status === "submitting"}
                      className="btn-primary group mt-1 w-full disabled:cursor-not-allowed disabled:opacity-80"
                    >
                      {status === "submitting" ? (
                        <>
                          <Loader2 className="h-[18px] w-[18px] animate-spin" />
                          Sending…
                        </>
                      ) : (
                        <>
                          Send message
                          <ArrowRight className="h-[18px] w-[18px] transition-transform duration-300 group-hover:translate-x-1" />
                        </>
                      )}
                    </button>

                    <p className="text-center text-xs text-faint">
                      No spam, ever. We only use your details to reply to this
                      request.
                    </p>
                  </motion.form>
                )}
              </AnimatePresence>
            </div>
          </Reveal>
        </div>
      </div>
    </section>
  );
}

function Label({
  children,
  htmlFor,
}: {
  children: React.ReactNode;
  htmlFor?: string;
}) {
  return (
    <label htmlFor={htmlFor} className="text-sm font-medium text-ink">
      {children}
    </label>
  );
}

function ErrorText({
  children,
  id,
}: {
  children: React.ReactNode;
  id?: string;
}) {
  return (
    <span id={id} className="text-[12.5px] text-red-400">
      {children}
    </span>
  );
}

function Field({
  label,
  name,
  type = "text",
  placeholder,
  required,
  optional,
  error,
  autoComplete,
}: {
  label: string;
  name: string;
  type?: string;
  placeholder?: string;
  required?: boolean;
  optional?: boolean;
  error?: string;
  autoComplete?: string;
}) {
  return (
    <div className="flex flex-col gap-2">
      <span className="flex items-center justify-between">
        <Label htmlFor={name}>{label}</Label>
        {optional && (
          <span className="font-mono text-[10.5px] uppercase tracking-wider text-faint">
            optional
          </span>
        )}
      </span>
      <input
        id={name}
        name={name}
        type={type}
        placeholder={placeholder}
        required={required}
        autoComplete={autoComplete}
        aria-invalid={!!error}
        aria-describedby={error ? `${name}-error` : undefined}
        className={`w-full rounded-xl border bg-bg-soft px-4 py-3.5 text-[15px] text-ink outline-none transition-colors duration-200 placeholder:text-faint focus-visible:border-accent focus-visible:ring-2 focus-visible:ring-accent ${
          error ? "border-red-400/60" : "border-line"
        }`}
      />
      {error && <ErrorText id={`${name}-error`}>{error}</ErrorText>}
    </div>
  );
}
