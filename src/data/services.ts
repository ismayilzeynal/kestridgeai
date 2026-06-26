import { Brain, Workflow, ShieldCheck, BarChart3, type LucideIcon } from "lucide-react";

export type DeliveryStep = {
  phase: string;
  what: string;
  timeline: string;
};

export type Service = {
  id: string;
  index: string;
  name: string;
  tagline: string;
  description: string;
  icon: LucideIcon;
  highlights: string[];
  steps: DeliveryStep[];
};

const CONSULT: DeliveryStep = {
  phase: "Consultation",
  what: "We meet with you, gather all the requirements, and make sure we fully understand what needs to be built. This usually takes several rounds of conversation.",
  timeline: "1–2 weeks",
};

const DESIGN: DeliveryStep = {
  phase: "Solution Design",
  what: "We come back with a proposed solution and work through it with you until it is agreed and ready to build.",
  timeline: "1–2 weeks",
};

const BUILD: DeliveryStep = {
  phase: "Build & Integrate",
  what: "We put together the right team and start building and integrating the solution into your environment.",
  timeline: "Project scope",
};

const DELIVER: DeliveryStep = {
  phase: "Delivery & Support",
  what: "We push to production, monitor everything, fix any issues, and make sure it is running correctly — then continue with ongoing support or hand it off to your team.",
  timeline: "Ongoing / handoff",
};

export const services: Service[] = [
  {
    id: "ai",
    index: "01",
    name: "AI Solutions",
    tagline: "We build AI solutions around your requirements.",
    description:
      "We design and deploy AI that fits the way your business actually operates. Whether you need smarter decision-making, automated insights from your data, or intelligent systems that learn and adapt — we build it, integrate it, and make sure it delivers measurable results.",
    icon: Brain,
    highlights: [
      "Decision-support & predictive models",
      "LLM assistants & retrieval systems",
      "Measurable, production-grade outcomes",
    ],
    steps: [CONSULT, DESIGN, BUILD, DELIVER],
  },
  {
    id: "automation",
    index: "02",
    name: "Automation",
    tagline: "We build automation around your processes.",
    description:
      "We look at how your team currently works, identify what can be automated, and build it. We connect your systems, eliminate the manual steps, and make sure everything runs reliably — without anyone having to babysit it.",
    icon: Workflow,
    highlights: [
      "Workflow & systems orchestration",
      "No-touch, reliable pipelines",
      "Hours saved, errors removed",
    ],
    steps: [CONSULT, DESIGN, BUILD, DELIVER],
  },
  {
    id: "security",
    index: "03",
    name: "IT Security",
    tagline: "We help you secure your environment before problems happen.",
    description:
      "We assess your environment, identify vulnerabilities, and fix them. We put the right controls in place, keep you compliant with US regulations, and monitor your systems around the clock. We also prepare you for security certifications and point your team toward the right training.",
    icon: ShieldCheck,
    highlights: [
      "Vulnerability assessment & hardening",
      "US compliance & certification readiness",
      "24/7 monitoring & rapid response",
    ],
    steps: [
      {
        phase: "Consultation",
        what: "We meet with you and learn about your current environment, your concerns, and what you are trying to protect.",
        timeline: "1–2 weeks",
      },
      {
        phase: "Assessment",
        what: "We go through your systems, identify any vulnerabilities, and give you a clear picture of where you stand — with concrete recommendations.",
        timeline: "1–2 weeks",
      },
      {
        phase: "Implementation",
        what: "We fix the issues, put the right controls in place, and make sure everything is configured correctly.",
        timeline: "Project scope",
      },
      {
        phase: "Monitoring & Support",
        what: "We monitor your environment on an ongoing basis and stay available 24/7 if something needs immediate attention.",
        timeline: "Ongoing · 24/7",
      },
    ],
  },
  {
    id: "analytics",
    index: "04",
    name: "Analytics",
    tagline: "We build analytics around your business data.",
    description:
      "We take your data, clean it up, and build the pipelines that keep it flowing consistently. We set up dashboards and reports so the right people have the information they need. Every day, you get a clear and accurate picture of what is happening in your business.",
    icon: BarChart3,
    highlights: [
      "Reliable data pipelines",
      "Dashboards & self-serve reporting",
      "A clear picture, every single day",
    ],
    steps: [CONSULT, DESIGN, BUILD, DELIVER],
  },
];
