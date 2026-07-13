import { Plus } from "lucide-react";
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
    a: "Both. Whether you're an individual with a single AI idea or an organization modernizing its stack, we scope the engagement to fit your needs and budget.",
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
    <section id="faq" className="relative py-24 sm:py-32">
      <div className="container-x">
        <SectionHeading
          eyebrow="Questions"
          title={
            <>
              Answers before you <span className="text-accent-grad italic">ask</span>
            </>
          }
          align="center"
        />

        <div className="mx-auto mt-12 flex max-w-3xl flex-col gap-3">
          {faqs.map((f, i) => (
            <Reveal key={f.q} delay={(i % 6) * 0.04}>
              <details className="group card rounded-2xl px-5 py-1 sm:px-6 [&_summary::-webkit-details-marker]:hidden">
                <summary className="flex cursor-pointer list-none items-center justify-between gap-4 py-4 text-[16px] font-semibold text-ink">
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
      </div>

      <script
        type="application/ld+json"
        dangerouslySetInnerHTML={{ __html: JSON.stringify(faqSchema) }}
      />
    </section>
  );
}
