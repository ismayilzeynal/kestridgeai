import { Brain, BarChart3, Workflow, ShieldCheck, type LucideIcon } from "lucide-react";

// Four, not seventeen. Only the service set is driven by the content API, so
// the card icons in Company, Security and WhyChooseUs never enter a registry
// and the client bundle carries exactly what it carries today. A Record of
// every Lucide icon inside a client module would pin all of them into the
// bundle whether they are used or not.
export const SERVICE_ICONS: Record<string, LucideIcon> = {
  Brain,
  BarChart3,
  Workflow,
  ShieldCheck,
};

// The API validates icon names against this same list, so a name the site does
// not know cannot be saved. The fallback exists so that if one ever did, the
// result is a slightly wrong icon rather than a crash during render.
export const resolveServiceIcon = (name: string): LucideIcon => SERVICE_ICONS[name] ?? Workflow;
