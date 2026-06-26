# test_logo — service site (frontend)

Marketing/landing site for a Chicago-based IT engineering team offering **AI
Solutions, Automation, IT Security, and Analytics** to US businesses.

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
    layout.tsx        fonts (Fraunces / Hanken Grotesk / JetBrains Mono) + metadata
    globals.css       design tokens (RGB-channel CSS vars) + utilities + components
    page.tsx          section assembly
    icon.svg          favicon (placeholder mark)
  components/
    layout/           Navbar, Footer, Background, ScrollProgress
    sections/         Hero, TrustBar, WhyChooseUs, Services, Team, Security, CTABand, Contact
    ui/               Reveal, Logo, LinkedInButton, MarqueeLogos, SectionHeading
  data/               services.ts, team.ts, companies.ts
  lib/                site.ts  (brand, email, LinkedIn, nav — single source of truth)
```

## Placeholders to replace before launch

| What | Where | Note |
| --- | --- | --- |
| Brand name + logo | `lib/site.ts` (`brand`), `ui/Logo.tsx`, `app/icon.svg` | currently `test_logo` |
| Contact email | `lib/site.ts` (`email`) | `hello@testlogo.com` placeholder |
| LinkedIn URL | `lib/site.ts` (`linkedin`) | placeholder company URL |
| Team members + photos | `data/team.ts` | placeholder faces from randomuser.me |
| "Previously worked at" logos | `data/companies.ts` | placeholder brand marks via Simple Icons CDN |
| Form submit | `components/sections/Contact.tsx` (`onSubmit`) | replace the simulated `setTimeout` with a real API call |

## Notes

- Design is dark/premium/editorial with one surgical teal-cyan accent. Colors are
  CSS variables in `globals.css`; neutrals are RGB channels so Tailwind opacity
  modifiers (`bg-surface/70`) work.
- Motion uses `whileInView` scroll reveals; a `<noscript>` fallback in `layout.tsx`
  keeps content visible if JS is unavailable.
- Respects `prefers-reduced-motion`.
