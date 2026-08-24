import { Reveal } from "@/components/ui/Reveal";
import { site } from "@/lib/site";

const faqs = [
  {
    q: "What does Kestridge AI do?",
    a: "We build the four kinds of systems listed above, connect them to what you already run, and support them after launch.",
  },
  {
    q: "What kinds of work do you take on?",
    a: "Common examples are entering incoming orders, matching invoices, routing approvals, and reporting from records you already keep.",
  },
  {
    q: "Where is Kestridge AI based?",
    a: "Kestridge AI is based in Illinois. We also work with engineering specialists outside the United States.",
  },
  {
    q: "How does a project start?",
    a: "A project starts with a consultation to gather your requirements. We then send a written plan and proposed solution for your approval.",
  },
  {
    q: "How do you handle our data?",
    a: "We encrypt your data, limit who can reach it, and use it only for your work. A nondisclosure agreement is available on request.",
  },
  {
    q: "What size companies do you work with?",
    a: "We accept projects from companies of any size. Scope, schedule, and cost are set per project in the written plan.",
  },
  {
    q: "How is cost determined?",
    a: "Cost depends on the scope of the work, which is set in the written plan. Nothing is committed until you approve it.",
  },
  {
    q: "What do you need from us during a project?",
    a: "One person who knows the process, and access to the systems involved.",
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
    <section
      id="questions"
      className="relative scroll-mt-5 pb-20 pt-16 sm:pb-28 sm:pt-20"
    >
      <div className="container-x">
        <Reveal>
          <h2 className="text-balance text-4xl text-signal sm:text-5xl">
            Common <span className="text-accent-grad">questions</span>
          </h2>
        </Reveal>

        {/* Answers stay open by design: short pairs, no expanding panels. */}
        <dl className="mt-10 grid gap-x-10 gap-y-7 sm:grid-cols-2 lg:mt-12 xl:grid-cols-3 xl:gap-x-8">
          {faqs.map((f, i) => (
            <Reveal
              key={f.q}
              delay={(i % 2) * 0.05}
              className="border-t border-line pt-5"
            >
              <dt className="text-[16px] font-semibold tracking-tight text-ink">
                {f.q}
              </dt>
              <dd className="mt-2 text-pretty text-[14.5px] leading-relaxed text-muted">
                {f.a}
              </dd>
            </Reveal>
          ))}
        </dl>

        <Reveal delay={0.1}>
          <p className="mt-10 text-[15px] text-muted">
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

      <script
        type="application/ld+json"
        dangerouslySetInnerHTML={{ __html: JSON.stringify(faqSchema) }}
      />
    </section>
  );
}
