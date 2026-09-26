import type { MetadataRoute } from "next";
// Imported for its content-hashed URL. The bare /icon.svg route is served
// immutable for a year, so a new icon at the same path never replaces a
// cached old one in an installed app.
import icon from "./icon.svg";

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
      { src: icon.src, sizes: "any", type: "image/svg+xml" },
      { src: "/brand/icon-192.png", sizes: "192x192", type: "image/png" },
      { src: "/brand/icon-512.png", sizes: "512x512", type: "image/png" },
      {
        src: "/brand/icon-maskable-512.png",
        sizes: "512x512",
        type: "image/png",
        purpose: "maskable",
      },
    ],
  };
}
