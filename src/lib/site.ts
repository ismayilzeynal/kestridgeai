// Single source of truth for brand + contact details.
// Logo artwork is not final yet: the wordmark stands in until the kit lands.
export const site = {
  brand: "Kestridge AI",
  legalName: "Kestridge AI Inc.",
  // Public production URL: update when the custom domain is live.
  url: "https://testlogo-site.vercel.app",
  tagline: "AI, automation, IT security, and data analytics for US businesses.",
  location: "Illinois, United States",
  region: "IL",
  country: "United States",
  email: "info@kestridge.com",
  linkedin: "https://www.linkedin.com/company/kestridge-ai",
  nav: [
    { label: "Services", href: "/#services" },
    { label: "Process", href: "/#process" },
    { label: "Company", href: "/#company" },
    { label: "Founders", href: "/#founders" },
    { label: "Security", href: "/#security" },
    { label: "Questions", href: "/#questions" },
  ],
};

export type ServiceOption = { value: string; label: string };

export const serviceOptions: ServiceOption[] = [
  { value: "ai", label: "AI Solutions" },
  { value: "automation", label: "Automation" },
  { value: "security", label: "IT Security" },
  { value: "analytics", label: "Data Analytics" },
  { value: "general", label: "General inquiry" },
];
