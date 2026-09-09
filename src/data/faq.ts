export type Faq = {
  q: string;
  a: string;
};

// Lifted out of FAQ.tsx so this file can be both the rendered source and the
// fallback when the content API is unreachable. The field names q and a match
// what GET /api/content returns, so no adapter sits between the two.
export const faqs: Faq[] = [
  {
    q: "What does Kestridge AI do?",
    a: "We build AI, automation, IT security, and data analytics systems, connect them to what you already run, and support them after launch.",
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
    a: "Only authorized people can reach your data, and we use it only for your work. We sign a nondisclosure agreement for the project.",
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
    a: "Access to the systems involved, and someone on your team who knows the process.",
  },
];
