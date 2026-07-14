# AIVanta — service site (frontend)

Marketing site for **AIVanta**, an American (Illinois, USA) technology company
offering **AI Solutions, Automation, IT Security, and Analytics** to US business.

Frontend only — **Next.js 14 (App Router) · TypeScript · Tailwind CSS ·
framer-motion**. No backend yet: the contact form is a simulated client-side
submit (wire it up later).

## Run

```bash
npm install
npm run dev      # http://localhost:3000
npm run build    # production build
npm run start    # serve the production build
```

## Structure

```
src/
  app/
    layout.tsx        chrome (Navbar/Footer/Background) + fonts + SEO metadata + JSON-LD + GA hook
    globals.css       light-theme design tokens (RGB-channel CSS vars) + utilities
    page.tsx          home section assembly
    about/page.tsx    About page
    sitemap.ts robots.ts manifest.ts   SEO route handlers
    icon.svg          favicon (placeholder mark)
  components/
    layout/           Navbar, Footer, Background, ScrollProgress, MobileCTA
    sections/         Hero, TrustBar, WhyChooseUs, Services, Team, Security, FAQ, CTABand, Contact
    ui/               Reveal, Logo, LinkedInButton, MarqueeLogos, SectionHeading
  data/               services.ts, team.ts, companies.ts
  lib/                site.ts (brand/contact/nav), scroll.ts (nav-offset scrolling)
```

## Placeholders to replace before launch

| What | Where | Note |
| --- | --- | --- |
| Logo artwork | `ui/Logo.tsx`, `app/icon.svg` | name is final (AIVanta); logo art is a temporary mark |
| Production domain | `lib/site.ts` (`url`) | currently the Vercel URL — update when the custom domain is live |
| Contact email | `lib/site.ts` (`email`) | `info@aivanta.com` placeholder |
| LinkedIn URL | `lib/site.ts` (`linkedin`) | placeholder company URL |
| Office address | `about/page.tsx`, `lib/site.ts` | "Illinois, USA" until a precise address is set |
| Team members + photos | `data/team.ts` | placeholder faces from randomuser.me |
| "Previously worked at" logos | `data/companies.ts` | placeholder brand marks via Simple Icons CDN |
| Form submit | `components/sections/Contact.tsx` (`onSubmit`) | replace the simulated `setTimeout` with a real API call |

## SEO & analytics

- **Metadata**: title/description/OpenGraph/Twitter + canonical per page (`layout.tsx`, `about/page.tsx`).
- **Structured data (JSON-LD)**: `Organization` + `WebSite` in `layout.tsx`; `FAQPage` in `components/sections/FAQ.tsx` — helps Google rich results and AI answer engines.
- **Crawling**: `sitemap.ts` → `/sitemap.xml`, `robots.ts` → `/robots.txt`, `manifest.ts` → `/manifest.webmanifest`.
- **Google Analytics / Ads**: set env var `NEXT_PUBLIC_GA_ID` (e.g. `G-XXXX` or `AW-XXXX`) in Vercel → the gtag script in `layout.tsx` activates automatically. Add conversion events at campaign time.
- **Done at campaign time (post-deploy)**: verify the domain in Google Search Console, submit the sitemap, create the Google Ads account + conversion pixel. Nothing else in the code blocks this.

## Notes

- Design is a **light**, serious/premium editorial theme: near-white surfaces,
  near-black (ink) primary buttons, one surgical deep-teal accent. Colors are CSS
  variables in `globals.css`; neutrals are RGB channels so Tailwind opacity
  modifiers (`bg-surface/70`) work.
- Fixed navbar offset for in-page anchors is handled globally via
  `scroll-padding-top` (`globals.css`) + `lib/scroll.ts` for JS-driven scrolls.
- Motion uses `whileInView` scroll reveals; a `<noscript>` fallback in `layout.tsx`
  keeps content visible without JS. Respects `prefers-reduced-motion`.
