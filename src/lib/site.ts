// Single source of truth for brand + contact details.
// Brand name and logo are NOT final — "test_logo" is a placeholder until approved.
export const site = {
  brand: "test_logo",
  location: "Chicago, IL",
  email: "hello@testlogo.com", // placeholder until the company name is finalized
  linkedin: "https://www.linkedin.com/company/testlogo",
  responseTime: "2–3 business days",
  nav: [
    { label: "Services", href: "#services" },
    { label: "Team", href: "#team" },
    { label: "Security", href: "#security" },
    { label: "Contact", href: "#contact" },
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
