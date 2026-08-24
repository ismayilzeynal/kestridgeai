import type { MetadataRoute } from "next";

export default function manifest(): MetadataRoute.Manifest {
  return {
    name: "Kestridge AI",
    short_name: "Kestridge",
    description:
      "United States technology company building AI, automation, IT security, and data analytics systems.",
    start_url: "/",
    display: "standalone",
    background_color: "#fafaf7",
    theme_color: "#fafaf7",
    icons: [
      { src: "/icon.svg", sizes: "any", type: "image/svg+xml" },
      { src: "/brand/icon-192.png", sizes: "192x192", type: "image/png" },
      { src: "/brand/icon-512.png", sizes: "512x512", type: "image/png" },
      {
        src: "/brand/icon-512.png",
        sizes: "512x512",
        type: "image/png",
        purpose: "maskable",
      },
    ],
  };
}
