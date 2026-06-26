export type Company = {
  name: string;
  slug: string;
};

// Logos of organizations our team members have previously worked with /
// alongside. These are PLACEHOLDERS rendered from the Simple Icons CDN until
// the final, approved set is confirmed. (Simple Icons does not host every
// brand — e.g. OpenAI/Amazon/Microsoft — so recognizable equivalents stand in.)
export const companies: Company[] = [
  { name: "Anthropic", slug: "anthropic" },
  { name: "NVIDIA", slug: "nvidia" },
  { name: "Google", slug: "google" },
  { name: "Meta", slug: "meta" },
  { name: "Hugging Face", slug: "huggingface" },
  { name: "Palantir", slug: "palantir" },
  { name: "Databricks", slug: "databricks" },
  { name: "Snowflake", slug: "snowflake" },
  { name: "Netflix", slug: "netflix" },
  { name: "Uber", slug: "uber" },
  { name: "Stripe", slug: "stripe" },
  { name: "McDonald's", slug: "mcdonalds" },
  { name: "Tesla", slug: "tesla" },
  { name: "Visa", slug: "visa" },
  { name: "Mastercard", slug: "mastercard" },
  { name: "Cisco", slug: "cisco" },
  { name: "Spotify", slug: "spotify" },
  { name: "Samsung", slug: "samsung" },
];
