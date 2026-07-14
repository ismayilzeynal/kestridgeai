import Link from "next/link";
import { Mail, MapPin, ArrowUpRight, ArrowRight } from "lucide-react";
import { Logo } from "@/components/ui/Logo";
import { LinkedInButton } from "@/components/ui/LinkedInButton";
import { site } from "@/lib/site";
import { services } from "@/data/services";

export function Footer() {
  return (
    <footer id="site-footer" className="relative border-t border-line bg-surface">
      <div className="container-x py-16">
        <div className="grid gap-12 sm:grid-cols-2 lg:grid-cols-[1.4fr_1fr_1fr]">
          <div>
            <Logo />
            <p className="mt-5 max-w-xs text-pretty text-sm leading-relaxed text-muted">
              An American technology company building AI, automation, security
              and analytics around what your business actually needs.
            </p>
            <div className="mt-6">
              <LinkedInButton />
            </div>
          </div>

          <div>
            <h4 className="font-mono text-[11px] uppercase tracking-label text-faint">
              Services
            </h4>
            <ul className="mt-5 flex flex-col gap-3">
              {services.map((s) => (
                <li key={s.id}>
                  <Link
                    href="/#services"
                    className="link-underline text-sm text-muted hover:text-ink"
                  >
                    {s.name}
                  </Link>
                </li>
              ))}
            </ul>

            <h4 className="mt-8 font-mono text-[11px] uppercase tracking-label text-faint">
              Company
            </h4>
            <ul className="mt-4 flex flex-col gap-3">
              <li>
                <Link
                  href="/about"
                  className="link-underline text-sm text-muted hover:text-ink"
                >
                  About
                </Link>
              </li>
            </ul>
          </div>

          <div>
            <h4 className="font-mono text-[11px] uppercase tracking-label text-faint">
              Get in touch
            </h4>
            <ul className="mt-5 flex flex-col gap-4">
              <li>
                <Link
                  href="/#contact"
                  className="group inline-flex items-center gap-2.5 text-sm text-ink hover:text-accent"
                >
                  <ArrowRight
                    className="h-4 w-4 text-accent transition-transform duration-300 group-hover:translate-x-0.5"
                    strokeWidth={1.7}
                  />
                  Send us a message
                </Link>
              </li>
              <li>
                <a
                  href={`mailto:${site.email}`}
                  className="group inline-flex items-center gap-2.5 text-sm text-muted hover:text-ink"
                >
                  <Mail className="h-4 w-4 text-faint" strokeWidth={1.7} />
                  {site.email}
                </a>
              </li>
              <li>
                <a
                  href={site.linkedin}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="group inline-flex items-center gap-2.5 text-sm text-muted hover:text-ink"
                >
                  <ArrowUpRight className="h-4 w-4 text-faint" strokeWidth={1.7} />
                  LinkedIn
                </a>
              </li>
              <li className="inline-flex items-center gap-2.5 text-sm text-muted">
                <MapPin className="h-4 w-4 text-faint" strokeWidth={1.7} />
                {site.location}
              </li>
            </ul>
          </div>
        </div>

        <div className="mt-14 hairline" />

        <div className="mt-6 flex flex-col items-start justify-between gap-4 sm:flex-row sm:items-center">
          <p className="text-xs text-faint">
            © {new Date().getFullYear()} {site.brand}. American-based · Global
            engineering resources.
          </p>
          <p className="text-xs text-faint">
            All information you share with us is kept strictly confidential.
          </p>
        </div>
      </div>
    </footer>
  );
}
