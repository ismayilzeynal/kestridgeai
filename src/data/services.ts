import { Brain, Workflow, ShieldCheck, BarChart3, type LucideIcon } from "lucide-react";

export type DeliveryStep = {
  phase: string;
  summary: string; // one line for the desktop stepper
  what: string; // full copy, shown on smaller screens
  /** Only the ongoing steps carry a timing value; the rest are set in the plan. */
  timeline?: string;
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
  timeline: "Ongoing",
};

export const services: Service[] = [
  {
    id: "ai",
    index: "01",
    name: "AI Solutions",
    tagline: "AI for work that involves documents and judgment",
    cardLabel: "Forecasting and assistants for your staff",
    description:
      "We build software that handles work your staff now does by hand.",
    icon: Brain,
    highlights: [
      "Forecasting from your past records",
      "A search tool for your own documents",
      "Sorting and routing of incoming requests",
    ],
    steps: [CONSULT, DESIGN, BUILD, DELIVER],
  },
  {
    id: "automation",
    index: "02",
    name: "Automation",
    tagline: "Automation of repeated, rule based steps",
    cardLabel: "Routine steps run without manual work",
    description:
      "Automation follows rules you set and runs the same way every time. We build it around the steps your team repeats by hand.",
    icon: Workflow,
    highlights: [
      "Automated approval routing",
      "Records passed between systems without retyping",
      "Automated data entry",
    ],
    steps: [CONSULT, DESIGN, BUILD, DELIVER],
  },
  {
    id: "security",
    index: "03",
    name: "IT Security",
    tagline: "Review and correction of security weaknesses",
    cardLabel: "Assessment, access rules, and monitoring",
    description:
      "We review your systems and report what we find. You decide what to fix, and we fix it.",
    icon: ShieldCheck,
    highlights: [
      "Testing of systems and networks",
      "Access rules and encryption",
      "Around the clock monitoring",
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
        what: "We monitor the systems we run for you and respond to issues that need attention.",
        timeline: "Ongoing",
      },
    ],
  },
  {
    id: "analytics",
    index: "04",
    name: "Data Analytics",
    tagline: "Business reporting from your own records",
    cardLabel: "Dashboards and business reporting",
    description:
      "We pull your data together, check it for errors, and build the reports your people need.",
    icon: BarChart3,
    highlights: [
      "Data cleanup and checks",
      "Management dashboards",
      "Scheduled reports",
    ],
    steps: [CONSULT, DESIGN, BUILD, DELIVER],
  },
];
