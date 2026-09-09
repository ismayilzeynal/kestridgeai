import { Reveal } from "@/components/ui/Reveal";
import { site } from "@/lib/site";
import type { Faq } from "@/data/faq";

export function FAQ({ faqs }: { faqs: Faq[] }) {
  // Built here rather than at module scope, from the same array that renders.
  // Structured data that can disagree with the visible text is a Google
  // structured-data violation, and once the answers come from a database that
  // is not a hypothetical.
  const faqSchema = {
    "@context": "https://schema.org",
    "@type": "FAQPage",
    mainEntity: faqs.map((f) => ({
      "@type": "Question",
      name: f.q,
      acceptedAnswer: { "@type": "Answer", text: f.a },
    })),
  };

  return (
    <section
      id="questions"
      className="relative scroll-mt-8 pb-20 pt-16 sm:scroll-mt-4 sm:pb-28 sm:pt-20"
    >
      <div className="container-x">
        <Reveal>
          <h2 className="text-balance text-4xl text-signal sm:text-5xl">
            Common <span className="text-accent-grad">questions</span>
          </h2>
        </Reveal>

        {/* Answers stay open by design: short pairs, no expanding panels. */}
        <dl className="mt-8 grid gap-x-10 gap-y-6 sm:grid-cols-2 xl:grid-cols-3 xl:gap-x-8">
          {faqs.map((f, i) => (
            <Reveal
              key={f.q}
              delay={(i % 2) * 0.05}
              className="border-t border-line pt-5"
            >
              <dt className="text-[0.9412rem] font-semibold tracking-tight text-ink">
                {f.q}
              </dt>
              <dd className="mt-2 text-pretty text-[0.8529rem] leading-relaxed text-muted">
                {f.a}
              </dd>
            </Reveal>
          ))}
        </dl>

        <Reveal delay={0.1}>
          <p className="mt-6 text-[0.8824rem] text-muted">
            For anything not covered here, write to us at{" "}
            <a
              href={`mailto:${site.email}`}
              className="font-medium text-accent underline-offset-4 hover:underline"
            >
              {site.email}
            </a>
            .
          </p>
        </Reveal>
      </div>

      {/* JSON.stringify does not escape a closing script tag, and this goes
          into dangerouslySetInnerHTML, so an answer containing one would
          break out of the block. The API rejects angle brackets on the
          write path as well: both halves are required, neither alone. */}
      <script
        type="application/ld+json"
        dangerouslySetInnerHTML={{ __html: JSON.stringify(faqSchema).replace(/</g, "\\u003c") }}
      />
    </section>
  );
}
