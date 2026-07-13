import type { MetadataRoute } from "next";

export default function manifest(): MetadataRoute.Manifest {
  return {
    name: "AIVanta",
    short_name: "AIVanta",
    description:
      "American technology company delivering applied AI, automation, security and analytics.",
    start_url: "/",
    display: "standalone",
    background_color: "#f5f6f9",
    theme_color: "#f5f6f9",
    icons: [{ src: "/icon.svg", sizes: "any", type: "image/svg+xml" }],
  };
}
