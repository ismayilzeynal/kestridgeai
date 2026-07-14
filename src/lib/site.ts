// Single source of truth for brand + contact details.
// Logo artwork is not final yet — the wordmark "AIVanta" stands in for it.
export const site = {
  brand: "AIVanta",
  // Public production URL — update when the custom domain is live.
  url: "https://testlogo-site.vercel.app",
  tagline: "Applied AI, automation, security & analytics for American business.",
  location: "Illinois, USA",
  region: "IL",
  country: "United States",
  email: "info@aivanta.com", // placeholder until the domain mailbox is live
  linkedin: "https://www.linkedin.com/company/aivanta",
  responseTime: "2–3 business days",
  nav: [
    { label: "Services", href: "/#services" },
    { label: "About", href: "/about" },
    { label: "Team", href: "/#team" },
    { label: "Security", href: "/#security" },
  ],
};

export type ServiceOption = { value: string; label: string };

export const serviceOptions: ServiceOption[] = [
  { value: "ai", label: "AI Solutions" },
  { value: "automation", label: "Automation" },
  { value: "security", label: "IT Security" },
  { value: "analytics", label: "Analytics" },
  { value: "general", label: "General / Not sure yet" },
];
