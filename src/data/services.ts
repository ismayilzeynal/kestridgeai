import { Brain, Workflow, ShieldCheck, BarChart3, type LucideIcon } from "lucide-react";

export type DeliveryStep = {
  phase: string;
  summary: string; // one line for the desktop stepper
  what: string; // full copy, shown on smaller screens
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
  summary: "We gather requirements with your team.",
  what: "We go through what the system needs to do, and confirm the requirements with you.",
};

const DESIGN: DeliveryStep = {
  phase: "Solution Design",
  summary: "We write the plan and agree on it with you.",
  what: "You get the solution, scope, and schedule in writing, revised until you approve it.",
};

const BUILD: DeliveryStep = {
  phase: "Build and Integration",
  summary: "We build the system and connect it.",
  what: "We build it, connect it to your existing systems, and test it with your team.",
};

const DELIVER: DeliveryStep = {
  phase: "Go Live and Support",
  summary: "We put the system into use and support it.",
  what: "The system goes into daily use. You choose whether we keep running it or hand it over with documentation.",
};

export const services: Service[] = [
  {
    id: "ai",
    index: "01",
    name: "AI Solutions",
    tagline: "AI solutions that will improve business",
    cardLabel: "Forecasting and assistants for your staff",
    description:
      "We analyze your business with you to identify the AI solution best suited to your needs - including ROI projections, an implementation roadmap, and security considerations.",
    icon: Brain,
    highlights: [
      "Predictive analytics and forecasting",
      "Machine learning",
      "Agentic AI and generative AI",
      "Deep learning",
      "Other services on request",
    ],
    steps: [CONSULT, DESIGN, BUILD, DELIVER],
  },
  {
    id: "analytics",
    index: "02",
    name: "Data Analytics",
    tagline: "Business reports from your data",
    cardLabel: "Dashboards and business reporting",
    description:
      "We pull your data together, check it for errors, and build the reports you need.",
    icon: BarChart3,
    highlights: [
      "Data cleanup and checks",
      "Management dashboards",
      "Scheduled reports",
      "Other services on request",
    ],
    steps: [CONSULT, DESIGN, BUILD, DELIVER],
  },
  {
    id: "automation",
    index: "03",
    name: "Automation",
    tagline: "Automation for repetitive work",
    cardLabel: "Routine steps run without manual work",
    description:
      "We document the current manual process and define the conditions under which each step should run automatically.",
    icon: Workflow,
    highlights: [
      "Invoice matching and processing",
      "Approval routing",
      "Report generation",
      "Reminders and follow-ups",
      "Data entry from forms and emails",
      "Other services on request",
    ],
    steps: [CONSULT, DESIGN, BUILD, DELIVER],
  },
  {
    id: "security",
    index: "04",
    name: "IT Security",
    tagline: "Security review and correction",
    cardLabel: "Assessment, access rules, and monitoring",
    description:
      "We review your systems, report what we find, and fix what you approve. We provide staff training and certification preparation.",
    icon: ShieldCheck,
    highlights: [
      "Vulnerability assessment",
      "Access control review",
      "Configuration audits",
      "Remediation",
      "Staff training and certification preparation",
      "Other services on request",
    ],
    steps: [
      {
        phase: "Consultation",
        summary: "We review what you need to protect.",
        what: "We go through your current systems and record what needs protection and who can reach it today.",
      },
      {
        phase: "Assessment",
        summary: "We test the systems and list the weaknesses.",
        what: "You receive a written report of the weaknesses we find, ordered by how serious each one is.",
      },
      {
        phase: "Implementation",
        summary: "We fix the findings and set up controls.",
        what: "We correct the findings you choose, set up access rules, encryption, and backups, and confirm each item is closed.",
      },
      {
        phase: "Monitoring and Support",
        summary: "We monitor and respond to issues.",
        what: "We monitor the systems we run for you and act on the issues that come up.",
      },
    ],
  },
];
