import type { Metadata, Viewport } from "next";
import { Fraunces, Hanken_Grotesk, JetBrains_Mono } from "next/font/google";
import { Providers } from "@/components/Providers";
import "./globals.css";

const display = Fraunces({
  subsets: ["latin"],
  axes: ["opsz"],
  variable: "--font-display",
  display: "swap",
});

const sans = Hanken_Grotesk({
  subsets: ["latin"],
  weight: ["400", "500", "600", "700"],
  variable: "--font-sans",
  display: "swap",
});

const mono = JetBrains_Mono({
  subsets: ["latin"],
  weight: ["400", "500"],
  variable: "--font-mono",
  display: "swap",
});

const SITE_URL = "https://testlogo.example";

export const metadata: Metadata = {
  metadataBase: new URL(SITE_URL),
  title: {
    default: "test_logo — AI, Automation, Security & Analytics Engineering",
    template: "%s · test_logo",
  },
  description:
    "A team of experienced IT engineers in Chicago building AI solutions, automation, cybersecurity and analytics around your requirements. We don't walk away until everything works.",
  keywords: [
    "AI solutions",
    "automation",
    "cybersecurity",
    "data analytics",
    "IT engineering",
    "Chicago",
  ],
  openGraph: {
    title: "test_logo — AI, Automation, Security & Analytics Engineering",
    description:
      "Experienced IT engineers who understand what you need and deliver it. AI, automation, security and analytics — built around your requirements.",
    type: "website",
    locale: "en_US",
    url: SITE_URL,
  },
  robots: { index: true, follow: true },
};

export const viewport: Viewport = {
  themeColor: "#08090b",
  width: "device-width",
  initialScale: 1,
};

export default function RootLayout({
  children,
}: Readonly<{ children: React.ReactNode }>) {
  return (
    <html lang="en" className={`${display.variable} ${sans.variable} ${mono.variable}`}>
      <body className="grain antialiased">
        {/* No-JS / hydration-failure fallback: reveal motion content that would
            otherwise stay at its hidden initial state. */}
        <noscript>
          <style>{`[style*="opacity:0"]{opacity:1!important;transform:none!important}`}</style>
        </noscript>
        <Providers>{children}</Providers>
      </body>
    </html>
  );
}
