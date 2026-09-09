# Kestridge AI - service site (frontend)

Marketing site for **Kestridge AI**, a United States (Illinois) technology company
offering **AI Solutions, Automation, IT Security, and Data Analytics**.

Frontend only - **Next.js 14 (App Router) · TypeScript · Tailwind CSS ·
framer-motion**. The contact form posts to a hosted form endpoint given by
`NEXT_PUBLIC_FORM_ENDPOINT` (Formspree, Web3Forms, Basin, anything that accepts
multipart POST and answers 2xx). With the variable unset the form does not
pretend to send: it opens the visitor's mail client with the message prefilled
and says so.

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
backend/        ASP.NET Core 10 + MySQL contact API (see backend/README.md)
```

Section ids used by the navigation: `top`, `company`, `services`, `process`,
`founders`, `security`, `questions`, `contact`.

## Open before launch

| What | Where | Note |
| --- | --- | --- |
| Office address | `lib/site.ts` (`location`) | "Illinois, United States" until a precise address is set |
| Two founder portraits | `data/team.ts` | Sarvjeet and Robert have no photo and render an initials avatar |
| Form endpoint | `.env` / Vercel env | the backend is in `backend/`; set `NEXT_PUBLIC_FORM_ENDPOINT` to its `/api/contact` URL and rebuild, otherwise the form falls back to a mailto handoff |
| DMARC policy | DNS zone | `p=none` while the reports are watched; tighten to `quarantine` and then `reject` once they come back clean |
| Spam protection | contact form | a `_gotcha` honeypot ships; add Turnstile if the endpoint gets abused |

## Backend

The backend lives in this repository, in `backend/`. ASP.NET Core 10 + MySQL 8,
one process, two endpoints. See `backend/README.md`.

It exists to receive this form, keep every inquiry, and notify the team. It
serves no page content: the copy in `src/data/*.ts` stays in code, and
`backend/README.md` records why.

`Contact.tsx` is unchanged and stays unchanged. It reads
`NEXT_PUBLIC_FORM_ENDPOINT` and posts the form to it:

| | |
| --- | --- |
| Method | `POST` |
| Body | `multipart/form-data` (a plain `FormData`) |
| Headers | `Accept: application/json` |
| Fields | `name`, `email`, `company`, `phone`, `service`, `message`, `_gotcha` |
| `service` values | `ai`, `analytics`, `automation`, `security`, `general` (see `serviceOptions` in `lib/site.ts`) |
| Success | any `2xx`. Anything else, or a network error, shows the error state |

All seven fields are always sent, in that DOM order. `company` and `phone` are
optional in the UI but still transmitted, as empty-string parts, so a backend
that rejects unknown or blank fields rejects every submission.

The client reads `Response.ok` and nothing else: no `.json()`, no timeout, no
retry. Both request headers are CORS-safelisted, so the POST is a **simple
request and the browser sends no preflight**. A response missing
`Access-Control-Allow-Origin` therefore does not stop the submission, it only
blocks the read: the message is stored and mailed, the visitor sees "That did
not go through", and resubmits. Duplicate mail plus a "broken form" report with
no matching server error is that bug.

`_gotcha` is a hidden honeypot. A real person always leaves it empty, so treat a
non-empty value as spam and drop the message (still answer `2xx`, or the bot
learns).

To point the site at the backend, set in Vercel:

```
NEXT_PUBLIC_FORM_ENDPOINT = https://api.kestridge.com/api/contact
```

and **trigger a rebuild**. `NEXT_PUBLIC_*` values are inlined at build time, so
redeploying the same artifact changes nothing. The variable ships to the browser,
so whatever sits behind it is public and needs its own rate limiting and abuse
protection; never put a secret in it.

A hosted form service (Formspree, Web3Forms, Basin) still works as a fallback:
set the full URL instead.

With the variable unset the form does not fake a send. It opens the visitor's
mail client with the message prefilled and tells them so, so nothing is ever
silently lost while the backend is being wired up.

## Deploying

The Vercel project is `kestridgeai`, linked to `ismayilzeynal/kestridgeai`
with `main` as the production branch: pushing to `main` deploys. To ship a
working tree without a push:

```bash
npx vercel --prod
```

The site serves on `kestridge.com`; `www.kestridge.com` 308-redirects to it.
`site.ts` -> `url` is the origin every canonical tag, the sitemap, the OG
image and the JSON-LD are built from, so it has to stay the apex.

## Copy

`copy-deck.json` at the repo root records the first copy rewrite, before and
after, with the reason for each change. It covers that pass only: later edits are
not in it, so the source files are the one authority for the current strings.

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
  (ink) primary buttons, one petrol accent used sparingly. Every colour comes from
  the brand kit in `Logo/` (see `Logo/03-SITE-NOTES.md` and
  `Logo/08-site/kestridge-tokens.css`) and lives as a CSS variable in
  `globals.css`; neutrals are RGB channels so Tailwind opacity modifiers
  (`bg-surface/70`) work. The contrast matrix is computed for that exact ramp, so
  the neutrals move as a set or not at all.
- Logo artwork is final. Source of truth is `Logo/`; the files the site actually
  serves were copied from `Logo/08-site/`.
- The navigation switches to its desktop row at `lg`, not `md`: six nav items plus
  the contact button need about 930px.
- In-page anchors go through `lib/scroll.ts`, which honors each section's
  `scroll-margin-top` exactly as the browser's own `#hash` jump does, and moves
  keyboard focus along with the scroll. Each section sets
  `scroll-mt = 6rem - its own padding-top` per breakpoint, so its heading lands
  2rem below the 4rem navbar. **Change a section's `py` and its `scroll-mt` has
  to move with it.**
- Motion uses scroll reveals; a `<noscript>` fallback in `layout.tsx` keeps
  content visible without JS. Respects `prefers-reduced-motion`.
