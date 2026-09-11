"use client";

import { useEffect, useRef, useState, type FormEvent } from "react";
import Link from "next/link";
import { AnimatePresence, motion } from "framer-motion";
import {
  ArrowRight,
  ArrowUpRight,
  Mail,
  MapPin,
  CheckCircle2,
  AlertCircle,
  Loader2,
  ChevronDown,
} from "lucide-react";
import { site, type ServiceOption } from "@/lib/site";
import { Reveal } from "@/components/ui/Reveal";
import { LinkedInButton } from "@/components/ui/LinkedInButton";
import { LinkedInIcon } from "@/components/ui/LinkedInIcon";

type Status = "idle" | "submitting" | "success" | "handoff" | "error";
type Errors = Partial<Record<"name" | "email" | "service" | "message", string>>;

const ease = [0.22, 1, 0.36, 1] as const;

export function Contact({ serviceOptions }: { serviceOptions: ServiceOption[] }) {
  const [status, setStatus] = useState<Status>("idle");
  const [errors, setErrors] = useState<Errors>({});
  const [service, setService] = useState("");
  // Live region must exist before the message lands, or nothing is announced.
  const [errorSummary, setErrorSummary] = useState("");
  const formRef = useRef<HTMLFormElement>(null);

  // Preselect the service when a "Start a … project" button is clicked.
  useEffect(() => {
    const handler = (e: Event) => {
      const detail = (e as CustomEvent<string>).detail;
      if (detail) setService(detail);
    };
    window.addEventListener("prefill-service", handler);
    return () => window.removeEventListener("prefill-service", handler);
  }, []);

  const validate = (data: FormData): Errors => {
    const next: Errors = {};
    const n = (data.get("name") as string)?.trim();
    const email = (data.get("email") as string)?.trim();
    const svc = data.get("service") as string;
    const msg = (data.get("message") as string)?.trim();
    if (!n) next.name = "Please enter your name.";
    if (!email) next.email = "Please enter your email address.";
    else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email))
      next.email = "Please enter a valid email address.";
    if (!svc) next.service = "Please select a service area.";
    if (!msg || msg.length < 10)
      next.message = "Please add at least one sentence about the work.";
    return next;
  };

  const onSubmit = (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const data = new FormData(e.currentTarget);
    const found = validate(data);
    setErrors(found);
    if (Object.keys(found).length > 0) {
      const n = Object.keys(found).length;
      setErrorSummary(
        n === 1 ? "1 field needs attention." : `${n} fields need attention.`
      );
      // Bring the first invalid field into view and focus it.
      const first = (["name", "email", "service", "message"] as const).find(
        (k) => found[k]
      );
      if (first) {
        const el = document.getElementById(first);
        el?.scrollIntoView({ block: "center", behavior: "smooth" });
        el?.focus({ preventScroll: true });
      }
      return;
    }

    setErrorSummary("");
    const sender = (data.get("name") as string)?.trim() || "";
    setStatus("submitting");

    const endpoint = process.env.NEXT_PUBLIC_FORM_ENDPOINT;
    const label =
      serviceOptions.find((o) => o.value === data.get("service"))?.label ?? "";

    // No form endpoint configured yet. Hand the message to the visitor's own
    // mail client rather than tell them we received something we never got.
    if (!endpoint) {
      const body = [
        `Name: ${sender}`,
        `Email: ${data.get("email")}`,
        `Service: ${label}`,
        "",
        (data.get("message") as string) ?? "",
      ].join("\n");
      window.location.href = `mailto:${site.email}?subject=${encodeURIComponent(
        `Project inquiry: ${label || "General"}`
      )}&body=${encodeURIComponent(body)}`;
      setStatus("handoff");
      return;
    }

    fetch(endpoint, {
      method: "POST",
      headers: { Accept: "application/json" },
      body: data,
    })
      .then((r) => setStatus(r.ok ? "success" : "error"))
      .catch(() => setStatus("error"));
  };

  const reset = () => {
    setStatus("idle");
    setErrors({});
    setErrorSummary("");
    setService("");
    formRef.current?.reset();
  };

  return (
    <section id="contact" className="relative scroll-mt-8 pb-24 pt-16 sm:scroll-mt-4 sm:pb-32 sm:pt-20 lg:scroll-mt-12 lg:pt-12">
      <div className="container-x">
        <div className="grid gap-12 lg:grid-cols-[0.85fr_1.15fr] lg:gap-10">
          {/* Left - invitation + contact rails */}
          <div className="lg:self-start">

            <Reveal>
              <h2 className="text-balance text-4xl text-signal sm:text-5xl">
                Send us a <span className="text-accent-grad">message</span>
              </h2>
            </Reveal>
            <Reveal delay={0.12}>
              <p className="mt-5 text-pretty text-lg leading-relaxed text-muted">
                Tell us about your project. We'll get back to you within a few
                business days.
              </p>
            </Reveal>

            <div className="mt-9 flex flex-col gap-3 lg:mt-6">
              <a
                href={`mailto:${site.email}`}
                className="card card-hover group flex items-center gap-4 rounded-2xl p-4"
              >
                <span className="grid h-11 w-11 place-items-center rounded-xl border border-line bg-surface text-accent">
                  <Mail className="h-5 w-5" strokeWidth={1.6} />
                </span>
                <span className="flex-1">
                  <span className="block font-mono text-[0.6471rem] uppercase tracking-label text-faint">
                    Email
                  </span>
                  <span className="text-[0.9412rem] text-ink">{site.email}</span>
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
                  <LinkedInIcon className="h-5 w-5" />
                </span>
                <span className="flex-1">
                  <span className="block font-mono text-[0.6471rem] uppercase tracking-label text-faint">
                    LinkedIn
                  </span>
                  <span className="text-[0.9412rem] text-ink">Follow us on LinkedIn</span>
                </span>
                <ArrowRight className="h-4 w-4 text-faint transition-all duration-300 group-hover:translate-x-0.5 group-hover:text-accent" />
              </a>

              <div className="flex items-center gap-4 rounded-2xl border border-line p-4">
                <span className="grid h-11 w-11 place-items-center rounded-xl border border-line bg-surface text-accent">
                  <MapPin className="h-5 w-5" strokeWidth={1.6} />
                </span>
                <span className="flex-1">
                  <span className="block font-mono text-[0.6471rem] uppercase tracking-label text-faint">
                    Based in
                  </span>
                  <span className="text-[0.9412rem] text-ink">{site.location}</span>
                </span>
              </div>
            </div>

          </div>

          {/* Right - the form */}
          <Reveal delay={0.06}>
            <div className="card relative overflow-hidden rounded-[1.6rem] p-6 sm:p-9 lg:p-5">
              <AnimatePresence mode="wait" initial={false}>
                {status === "success" ||
                status === "handoff" ||
                status === "error" ? (
                  <motion.div
                    key="result"
                    initial={{ opacity: 0, y: 16 }}
                    animate={{ opacity: 1, y: 0 }}
                    exit={{ opacity: 0 }}
                    transition={{ duration: 0.5, ease }}
                    className="flex min-h-[24.7059rem] flex-col items-center justify-center text-center"
                  >
                    <span
                      className={`relative mb-6 grid h-16 w-16 place-items-center rounded-full border border-line-strong bg-surface ${
                        status === "error" ? "text-red-500" : "text-accent"
                      }`}
                    >
                      {status !== "error" && (
                        <span className="absolute inset-0 rounded-full glow-accent" />
                      )}
                      {status === "success" && (
                        <CheckCircle2 className="h-8 w-8" strokeWidth={1.6} />
                      )}
                      {status === "handoff" && (
                        <Mail className="h-8 w-8" strokeWidth={1.6} />
                      )}
                      {status === "error" && (
                        <AlertCircle className="h-8 w-8" strokeWidth={1.6} />
                      )}
                    </span>
                    <h3 className="font-display text-3xl text-ink">
                      {status === "success" && "Thank you for reaching out."}
                      {status === "handoff" && "Your message is ready to send."}
                      {status === "error" && "That did not go through."}
                    </h3>
                    <p className="mt-3 max-w-sm text-pretty leading-relaxed text-muted">
                      {status === "success" && "We'll respond shortly."}
                      {status === "handoff" &&
                        `Your email app should have opened with it. If it did not, write to ${site.email}.`}
                      {status === "error" &&
                        `We could not send the message. Please write to ${site.email} instead.`}
                    </p>
                    <div className="mt-8 flex flex-col items-center gap-4">
                      <div className="flex flex-col items-center gap-3 sm:flex-row">
                        {status !== "success" && (
                          <a href={`mailto:${site.email}`} className="btn-primary group">
                            Email us
                            <ArrowUpRight className="h-[1.0588rem] w-[1.0588rem] transition-transform duration-300 ease-smooth group-hover:translate-x-0.5 group-hover:-translate-y-0.5" />
                          </a>
                        )}
                        <LinkedInButton variant={status === "success" ? "primary" : "ghost"} />
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
                    className="flex flex-col gap-5 lg:gap-3.5"
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
                        label="Email address"
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
                        label="Phone number"
                        name="phone"
                        type="tel"
                        placeholder="(555) 000-0000"
                        optional
                        autoComplete="tel"
                      />
                    </div>

                    {/* Service select */}
                    <div className="flex flex-col gap-2">
                      <Label htmlFor="contact-service">Service area</Label>
                      <div className="relative">
                        <select
                          id="contact-service"
                          name="service"
                          value={service}
                          onChange={(e) => setService(e.target.value)}
                          aria-invalid={!!errors.service}
                          aria-describedby={errors.service ? "contact-service-error" : undefined}
                          className={`peer w-full appearance-none rounded-xl border bg-bg-soft px-4 py-3.5 text-[0.9412rem] text-ink outline-none transition-colors duration-200 focus-visible:border-accent focus-visible:ring-2 focus-visible:ring-accent ${
                            service ? "text-ink" : "text-faint"
                          } ${errors.service ? "border-red-500/70" : "border-line"}`}
                        >
                          <option value="" disabled hidden>
                            Select a service area
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
                        <ErrorText id="contact-service-error">{errors.service}</ErrorText>
                      )}
                    </div>

                    {/* Message */}
                    <div className="flex flex-col gap-2">
                      <Label htmlFor="contact-message">Description of the work</Label>
                      <textarea
                        id="contact-message"
                        name="message"
                        rows={3}
                        placeholder="A short description of the work and the systems involved."
                        aria-invalid={!!errors.message}
                        aria-describedby={errors.message ? "contact-message-error" : undefined}
                        className={`w-full resize-none rounded-xl border bg-bg-soft px-4 py-3.5 text-[0.9412rem] text-ink outline-none transition-colors duration-200 placeholder:text-faint focus-visible:border-accent focus-visible:ring-2 focus-visible:ring-accent ${
                          errors.message ? "border-red-500/70" : "border-line"
                        }`}
                      />
                      {errors.message && (
                        <ErrorText id="contact-message-error">{errors.message}</ErrorText>
                      )}
                    </div>

                    <p aria-live="polite" className="sr-only">
                      {errorSummary}
                    </p>

                    <input
                      type="text"
                      name="_gotcha"
                      tabIndex={-1}
                      autoComplete="off"
                      aria-hidden="true"
                      className="hidden"
                    />

                    <button
                      type="submit"
                      disabled={status === "submitting"}
                      className="btn-primary group mt-1 w-full disabled:cursor-not-allowed disabled:opacity-80"
                    >
                      {status === "submitting" ? (
                        <>
                          <Loader2 className="h-[1.0588rem] w-[1.0588rem] animate-spin" />
                          Sending
                        </>
                      ) : (
                        <>
                          Send message
                          <ArrowRight className="h-[1.0588rem] w-[1.0588rem] transition-transform duration-300 group-hover:translate-x-1" />
                        </>
                      )}
                    </button>

                    <p className="-mt-1 text-center text-[0.7059rem] leading-snug text-faint lg:-mt-2">
                      By sending this message, you agree to our{" "}
                      <Link
                        href="/privacy"
                        className="underline underline-offset-2 transition-colors hover:text-ink"
                      >
                        Privacy Policy
                      </Link>
                      .
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
    <span id={id} className="text-[0.7647rem] font-medium text-red-600">
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
        <Label htmlFor={`contact-${name}`}>{label}</Label>
        {optional && (
          <span className="font-mono text-[0.6176rem] uppercase tracking-wider text-faint">
            optional
          </span>
        )}
      </span>
      <input
        id={`contact-${name}`}
        name={name}
        type={type}
        placeholder={placeholder}
        required={required}
        autoComplete={autoComplete}
        aria-invalid={!!error}
        aria-describedby={error ? `contact-${name}-error` : undefined}
        className={`w-full rounded-xl border bg-bg-soft px-4 py-3.5 text-[0.9412rem] text-ink outline-none transition-colors duration-200 placeholder:text-faint focus-visible:border-accent focus-visible:ring-2 focus-visible:ring-accent ${
          error ? "border-red-500/70" : "border-line"
        }`}
      />
      {error && <ErrorText id={`contact-${name}-error`}>{error}</ErrorText>}
    </div>
  );
}
