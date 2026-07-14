import { Brain, Workflow, ShieldCheck, BarChart3, type LucideIcon } from "lucide-react";

export type DeliveryStep = {
  phase: string;
  summary: string; // one crafted line for the desktop stepper
  what: string; // full copy, shown on smaller screens
  timeline: string;
};

export type Service = {
  id: string;
  index: string;
  name: string;
  tagline: string;
  cardLabel: string; // short parallel noun phrase for the hero console card
  description: string;
  icon: LucideIcon;
  highlights: string[];
  steps: DeliveryStep[];
};

const CONSULT: DeliveryStep = {
  phase: "Consultation",
  summary: "We learn your requirements in depth.",
  what: "We meet with you, gather all the requirements, and make sure we fully understand what needs to be built.",
  timeline: "1–2 weeks",
};

const DESIGN: DeliveryStep = {
  phase: "Solution Design",
  summary: "We agree the solution together.",
  what: "We come back with a proposed solution and work through it with you until it is agreed and ready to build.",
  timeline: "1–2 weeks",
};

const BUILD: DeliveryStep = {
  phase: "Build & Integrate",
  summary: "We build it in your environment.",
  what: "We put together the right team and start building and integrating the solution into your environment.",
  timeline: "Project scope",
};

const DELIVER: DeliveryStep = {
  phase: "Delivery & Support",
  summary: "Production, monitoring, support.",
  what: "We push to production, monitor everything, fix any issues — then continue with ongoing support or hand off to your team.",
  timeline: "Ongoing",
};

export const services: Service[] = [
  {
    id: "ai",
    index: "01",
    name: "AI Solutions",
    tagline: "Production AI that fits how you operate.",
    cardLabel: "Decision support & LLM systems",
    description:
      "We design and deploy AI that fits how your business actually operates — smarter decision-making, automated insights from your data, and intelligent systems that deliver measurable results.",
    icon: Brain,
    highlights: [
      "Decision-support & predictive models",
      "LLM assistants & retrieval systems",
      "Production-grade outcomes",
    ],
    steps: [CONSULT, DESIGN, BUILD, DELIVER],
  },
  {
    id: "automation",
    index: "02",
    name: "Automation",
    tagline: "The manual work, removed from your week.",
    cardLabel: "Connected systems, zero manual steps",
    description:
      "We study how your team works, find what can be automated, and build it — connecting your systems and eliminating manual steps so everything runs reliably on its own.",
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
    tagline: "Secured before problems happen.",
    cardLabel: "Assess, harden, monitor — 24/7",
    description:
      "We assess your environment, fix vulnerabilities, and put the right controls in place — keeping you compliant with US regulations and monitored around the clock.",
    icon: ShieldCheck,
    highlights: [
      "Assessment & hardening",
      "US compliance & certification readiness",
      "24/7 monitoring & response",
    ],
    steps: [
      {
        phase: "Consultation",
        summary: "We learn your environment and concerns.",
        what: "We meet with you and learn about your current environment, your concerns, and what you are trying to protect.",
        timeline: "1–2 weeks",
      },
      {
        phase: "Assessment",
        summary: "We map vulnerabilities and risks.",
        what: "We go through your systems, identify any vulnerabilities, and give you a clear picture of where you stand.",
        timeline: "1–2 weeks",
      },
      {
        phase: "Implementation",
        summary: "We fix and harden your systems.",
        what: "We fix the issues, put the right controls in place, and make sure everything is configured correctly.",
        timeline: "Project scope",
      },
      {
        phase: "Monitoring & Support",
        summary: "Around-the-clock protection.",
        what: "We monitor your environment on an ongoing basis and stay available 24/7 if something needs immediate attention.",
        timeline: "Ongoing · 24/7",
      },
    ],
  },
  {
    id: "analytics",
    index: "04",
    name: "Analytics",
    tagline: "A clear picture of the business, every day.",
    cardLabel: "Pipelines, dashboards & reporting",
    description:
      "We clean up your data, build reliable pipelines, and set up dashboards and reports — so the right people always have a clear, accurate picture of the business.",
    icon: BarChart3,
    highlights: [
      "Reliable data pipelines",
      "Dashboards & self-serve reporting",
      "A clear picture, every day",
    ],
    steps: [CONSULT, DESIGN, BUILD, DELIVER],
  },
];
