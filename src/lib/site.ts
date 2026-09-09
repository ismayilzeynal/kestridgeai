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

// serviceOptions used to live here as a second hand-maintained copy of the
// services array, kept in step by a code comment. page.tsx now derives it from
// the services themselves, so the two cannot drift.
export type ServiceOption = { value: string; label: string };
