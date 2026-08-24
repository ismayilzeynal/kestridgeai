# Kestridge AI - service site (frontend)

Marketing site for **Kestridge AI**, a United States (Illinois) technology company
offering **AI Solutions, Automation, IT Security, and Data Analytics** to US
businesses.

Frontend only - **Next.js 14 (App Router) · TypeScript · Tailwind CSS ·
framer-motion**. No backend yet: the contact form is a simulated client-side
submit (wire it up later).

The site is one scrolling page plus two legal pages. `/about` used to be its own
route; it now redirects to the `#company` section on the home page.

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
    layout.tsx           chrome (Navbar/Footer/Background) + fonts + SEO metadata + JSON-LD + GA hook
    globals.css          light-theme design tokens (RGB-channel CSS vars) + utilities
    page.tsx             home section assembly
    privacy/ terms/      legal pages
    opengraph-image.tsx  social preview image, rendered per request by next/og on the edge
    sitemap.ts robots.ts manifest.ts   SEO route handlers
    icon.svg             favicon (placeholder mark)
  components/
    layout/     Navbar, Footer, Background, ScrollProgress, MobileCTA
    sections/   Hero, TrustBar, Company, Services, WhyChooseUs, Team, Security, FAQ, Contact
    legal/      shared layout for the privacy and terms pages
    ui/         Reveal, Logo, LinkedInButton, LinkedInIcon, MarqueeLogos
    CookieConsent.tsx  consent-gated Google Analytics loader
    SmoothAnchors.tsx  routes in-page hash links through lib/scroll.ts
    Providers.tsx      framer-motion reduced-motion config
  data/         services.ts, team.ts, companies.ts
  lib/          site.ts (brand/contact/nav), scroll.ts (nav-offset scrolling + focus move)
public/
  brand/        logo mark and lockups (placeholder art)
  logos/        company logos for the marquee, sourced from each organization
  team/         founder portraits
```

Section ids used by the navigation: `top`, `company`, `services`, `process`,
`founders`, `security`, `questions`, `contact`.

## Open before launch

| What | Where | Note |
| --- | --- | --- |
| Logo artwork | `public/brand/`, `app/icon.svg`, `app/apple-icon.png` | name is final (Kestridge AI); logo art is a temporary mark |
| Production domain | `lib/site.ts` (`url`) | currently the Vercel URL, update when the custom domain is live |
| Office address | `lib/site.ts` (`location`) | "Illinois, United States" until a precise address is set |
| Two founder portraits | `data/team.ts` | Sarvjeet and Robert have no photo and render an initials avatar |
| Form submit | `components/sections/Contact.tsx` (`onSubmit`) | replace the simulated `setTimeout` with a real API call |
| Spam protection | contact form | honeypot field or Cloudflare Turnstile |

## Copy

All user-facing strings were rewritten in one pass. The full before/after record,
with the reason for each change, is in `copy-deck.json` at the repo root.

House rules the copy follows: no marketing language, no slogans, no rhetorical
question headings, plain American business English, sentence case for headings
and buttons, and **no em dash or en dash characters anywhere** (plain hyphens
only).

## SEO & analytics

- **Metadata**: title/description/OpenGraph/Twitter + canonical per page
  (`layout.tsx`, `privacy/page.tsx`, `terms/page.tsx`).
- **Social image**: `app/opengraph-image.tsx` renders it from `site.brand`, so it
  tracks the brand name automatically. There is no separate `twitter-image` route;
  Next reuses the OpenGraph image for Twitter/X cards.
- **Structured data (JSON-LD)**: `Organization` + `WebSite` in `layout.tsx`;
  `FAQPage` in `components/sections/FAQ.tsx`.
- **Crawling**: `sitemap.ts` → `/sitemap.xml`, `robots.ts` → `/robots.txt`,
  `manifest.ts` → `/manifest.webmanifest`.
- **Google Analytics**: set env var `NEXT_PUBLIC_GA_ID` in Vercel and the gtag
  script in `layout.tsx` activates. It is consent-gated: nothing loads until the
  visitor accepts the cookie notice, and the Global Privacy Control signal keeps
  it off.
- **Vercel Web Analytics** runs on every visit. It is cookieless, so it needs no
  consent, and it is disclosed in the Privacy Policy.

## Notes

- Design is a **light**, serious editorial theme: near-white surfaces, near-black
  (ink) primary buttons, one deep-teal accent used sparingly. Colors are CSS
  variables in `globals.css`; neutrals are RGB channels so Tailwind opacity
  modifiers (`bg-surface/70`) work.
- The navigation switches to its desktop row at `lg`, not `md`: six nav items plus
  the contact button need about 930px.
- In-page anchors go through `lib/scroll.ts`, which honors each section's
  `scroll-margin-top` and moves keyboard focus along with the scroll.
- Motion uses scroll reveals; a `<noscript>` fallback in `layout.tsx` keeps
  content visible without JS. Respects `prefers-reduced-motion`.
