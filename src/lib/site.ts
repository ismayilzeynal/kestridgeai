// Single source of truth for brand + contact details.
export const site = {
  brand: "Kestridge AI",
  legalName: "Kestridge AI Inc.",
  // Canonical production origin. Everything derived from it - canonical
  // tags, sitemap, OG images, JSON-LD - so it has to be the apex the site
  // actually serves on: www.kestridge.com 308-redirects here.
  url: "https://kestridge.com",
  tagline: "AI, automation, IT security, and data analytics for businesses.",
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

// Same order as the services array, so the form dropdown matches the page.
export const serviceOptions: ServiceOption[] = [
  { value: "ai", label: "AI Solutions" },
  { value: "analytics", label: "Data Analytics" },
  { value: "automation", label: "Automation" },
  { value: "security", label: "IT Security" },
  { value: "general", label: "General inquiry" },
];
