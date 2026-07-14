import { Plus, ArrowRight } from "lucide-react";
import { SectionHeading } from "@/components/ui/SectionHeading";
import { Reveal } from "@/components/ui/Reveal";
import { site } from "@/lib/site";

const faqs = [
  {
    q: "What does AIVanta do?",
    a: "We design, build and secure the systems businesses rely on — applied AI solutions, business automation, cybersecurity and data analytics. Every engagement is built around your specific requirements.",
  },
  {
    q: "Where is AIVanta based?",
    a: "AIVanta is an American technology company based in Illinois, USA. We work with a global network of engineering specialists, so you get local accountability with global capability.",
  },
  {
    q: "How do we start working together?",
    a: `The fastest way is the contact form on this site — tell us what you're trying to solve. We reply within ${site.responseTime}, then we scope the work with you, agree a plan and timeline, and start building.`,
  },
  {
    q: "How do you handle data security and confidentiality?",
    a: "Confidentiality is the foundation of how we work. Every engagement is covered by strict confidentiality, NDAs on request, least-privilege access, and end-to-end encryption. We never reuse or train shared models on your data.",
  },
  {
    q: "Do you work with startups and small companies, or only large enterprises?",
    a: "Both. We work with startups and mid-size companies through to large enterprises — every engagement is scoped to fit your needs and budget.",
  },
  {
    q: "What does a project cost?",
    a: "It depends on scope, which we define together before any commitment. The first conversation is always free, with no obligation.",
  },
];

const faqSchema = {
  "@context": "https://schema.org",
  "@type": "FAQPage",
  mainEntity: faqs.map((f) => ({
    "@type": "Question",
    name: f.q,
    acceptedAnswer: { "@type": "Answer", text: f.a },
  })),
};

export function FAQ() {
  return (
    <section id="faq" className="relative scroll-mt-5 pb-24 pt-16 sm:pb-32 sm:pt-20">
      <div className="container-x">
        <SectionHeading
          eyebrow="Questions"
          title={
            <>
              Answers before you <span className="text-accent-grad">ask</span>
            </>
          }
        />

        <div className="mt-12 grid items-start gap-8 lg:grid-cols-[minmax(0,1fr)_340px]">
          <div className="flex flex-col gap-3">
            {faqs.map((f, i) => (
              <Reveal key={f.q} delay={(i % 6) * 0.04}>
                <details className="group card rounded-2xl px-5 py-1 sm:px-6 [&_summary::-webkit-details-marker]:hidden">
                  <summary className="flex cursor-pointer list-none items-center justify-between gap-4 py-5 text-[17.5px] font-semibold text-ink">
                    {f.q}
                    <span className="grid h-7 w-7 shrink-0 place-items-center rounded-full border border-line text-accent transition-transform duration-300 group-open:rotate-45">
                      <Plus className="h-4 w-4" strokeWidth={2} />
                    </span>
                  </summary>
                  <p className="pb-5 pr-10 text-pretty leading-relaxed text-muted">
                    {f.a}
                  </p>
                </details>
              </Reveal>
            ))}
          </div>

          {/* Companion rail — balances the two-column rhythm */}
          <Reveal delay={0.1}>
            <aside className="card hidden rounded-2xl p-6 lg:sticky lg:top-24 lg:block">
              <h3 className="text-[18px] font-bold tracking-tight text-ink">
                Still have questions?
              </h3>
              <p className="mt-2 text-[14.5px] leading-relaxed text-muted">
                Tell us what you&apos;re working on — an engineer will get back
                to you within {site.responseTime}.
              </p>
              <a href="#contact" className="btn-primary group mt-5 w-full !py-3 text-[14.5px]">
                Ask us directly
                <ArrowRight className="h-4 w-4 transition-transform duration-300 group-hover:translate-x-0.5" />
              </a>
              <a
                href={`mailto:${site.email}`}
                className="mt-3 block text-center text-[13.5px] text-muted underline-offset-4 hover:text-ink hover:underline"
              >
                {site.email}
              </a>
            </aside>
          </Reveal>
        </div>
      </div>

      <script
        type="application/ld+json"
        dangerouslySetInnerHTML={{ __html: JSON.stringify(faqSchema) }}
      />
    </section>
  );
}
