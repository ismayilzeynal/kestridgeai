import type { Metadata, Viewport } from "next";
import { Archivo } from "next/font/google";
import { GeistMono } from "geist/font/mono";
import { Analytics } from "@vercel/analytics/next";
import { Providers } from "@/components/Providers";
import { CookieConsent } from "@/components/CookieConsent";
import { SmoothAnchors } from "@/components/SmoothAnchors";
import { Background } from "@/components/layout/Background";
import { ScrollProgress } from "@/components/layout/ScrollProgress";
import { Navbar } from "@/components/layout/Navbar";
import { MobileCTA } from "@/components/layout/MobileCTA";
import { Footer } from "@/components/layout/Footer";
import { site } from "@/lib/site";
import "./globals.css";

const sans = Archivo({
  subsets: ["latin"],
  variable: "--font-sans",
  display: "swap",
});

const TITLE = "Kestridge AI | AI, Automation and IT Security in Illinois";
const DESCRIPTION =
  "Kestridge AI is a United States technology company based in Illinois. We build AI, automation, IT security, and data analytics systems for US businesses.";

export const metadata: Metadata = {
  metadataBase: new URL(site.url),
  title: {
    default: TITLE,
    template: "%s | Kestridge AI",
  },
  description: DESCRIPTION,
  applicationName: site.brand,
  keywords: [
    site.brand,
    "AI solutions",
    "AI consulting",
    "automation",
    "IT security",
    "cybersecurity",
    "data analytics",
    "Illinois",
    "United States",
  ],
  authors: [{ name: site.brand }],
  creator: site.brand,
  publisher: site.brand,
  alternates: { canonical: "/" },
  openGraph: {
    title: TITLE,
    description: DESCRIPTION,
    type: "website",
    locale: "en_US",
    url: site.url,
    siteName: site.brand,
  },
  twitter: {
    card: "summary_large_image",
    title: TITLE,
    description: DESCRIPTION,
  },
  robots: {
    index: true,
    follow: true,
    googleBot: { index: true, follow: true, "max-image-preview": "large" },
  },
  category: "technology",
};

export const viewport: Viewport = {
  themeColor: "#f5f6f9",
  width: "device-width",
  initialScale: 1,
};

const organizationSchema = {
  "@context": "https://schema.org",
  "@type": "Organization",
  name: site.brand,
  legalName: site.legalName,
  url: site.url,
  logo: `${site.url}/opengraph-image`,
  email: site.email,
  description: DESCRIPTION,
  areaServed: { "@type": "Country", name: "United States" },
  address: {
    "@type": "PostalAddress",
    addressRegion: site.region,
    addressCountry: "US",
  },
  sameAs: [site.linkedin],
  knowsAbout: [
    "Artificial Intelligence",
    "Machine Learning",
    "Business Automation",
    "Cybersecurity",
    "Data Analytics",
  ],
};

const websiteSchema = {
  "@context": "https://schema.org",
  "@type": "WebSite",
  name: site.brand,
  url: site.url,
  publisher: { "@type": "Organization", name: site.brand },
};

export default function RootLayout({
  children,
}: Readonly<{ children: React.ReactNode }>) {
  const gaId = process.env.NEXT_PUBLIC_GA_ID;

  return (
    <html lang="en" className={`${sans.variable} ${GeistMono.variable}`}>
      <body className="antialiased">
        {/* No-JS / hydration-failure fallback: reveal motion content that would
            otherwise stay at its hidden initial state. */}
        <noscript>
          <style>{`[style*="opacity:0"]{opacity:1!important;transform:none!important}`}</style>
        </noscript>

        <Providers>
          <SmoothAnchors />
          <Background />
          <ScrollProgress />
          <Navbar />
          <main id="main" tabIndex={-1} className="outline-none">
            {children}
          </main>
          <Footer />
          <MobileCTA />
        </Providers>

        <script
          type="application/ld+json"
          dangerouslySetInnerHTML={{
            __html: JSON.stringify([organizationSchema, websiteSchema]),
          }}
        />

        {/* Google Analytics is consent-gated: nothing renders or loads until
            NEXT_PUBLIC_GA_ID is set, and GA runs only after the visitor
            accepts the cookie notice. */}
        <CookieConsent gaId={gaId} />

        {/* Vercel Web Analytics - cookieless and anonymous, so it needs no
            consent banner. */}
        <Analytics />
      </body>
    </html>
  );
}
