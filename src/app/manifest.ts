import type { MetadataRoute } from "next";

export default function manifest(): MetadataRoute.Manifest {
  return {
    name: "Kestridge AI",
    short_name: "Kestridge",
    description:
      "United States technology company building AI, automation, IT security, and data analytics systems.",
    start_url: "/",
    display: "standalone",
    background_color: "#f5f6f9",
    theme_color: "#f5f6f9",
    icons: [{ src: "/icon.svg", sizes: "any", type: "image/svg+xml" }],
  };
}
