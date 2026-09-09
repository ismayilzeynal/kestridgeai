import { faqs as FALLBACK_FAQ, type Faq } from "@/data/faq";
import { team as FALLBACK_TEAM, type Member } from "@/data/team";
import { companies as FALLBACK_COMPANIES, type Company } from "@/data/companies";
import { services as FALLBACK_SERVICES, type Service } from "@/data/services";

export type Content = {
  faq: Faq[];
  team: Member[];
  companies: Company[];
  services: Service[];
};

const FALLBACK: Content = {
  faq: FALLBACK_FAQ,
  team: FALLBACK_TEAM,
  companies: FALLBACK_COMPANIES,
  services: FALLBACK_SERVICES,
};

/**
 * The site content, from the API when it answers and from the committed
 * constants when it does not.
 *
 * revalidate plus a fallback is the only configuration where no failure mode
 * produces either a blank page or a failed build:
 *
 *   Build-time fetch with nothing caught - the API being down blocks every
 *   deploy, including unrelated marketing-copy commits.
 *   cache: "no-store" - opts the route out of static generation, puts one VPS
 *   on the hot path of every page view, and turns an outage into an error
 *   boundary on every request.
 *   revalidate with no fallback - the steady state is right, but the first
 *   build after a cleared cache must still fetch successfully or it fails.
 *   Fallback with no revalidate - either refetches per request or caches
 *   forever.
 *
 * API down at build time, DNS or TLS failing, a 502 with an nginx HTML body,
 * malformed JSON, an empty set, or a hung connection all land in the same
 * place: the constants render and the build succeeds. If the API was up at
 * build time and goes down afterwards, the prerendered HTML already holds the
 * database content and keeps being served; background revalidations fail
 * silently and the content simply stops updating.
 *
 * Rollback for this whole feature is removing API_ORIGIN from Vercel and
 * redeploying. That puts every section back on the compiled constants without
 * touching the database or the API.
 */
export async function getContent(): Promise<Content> {
  const origin = process.env.API_ORIGIN;

  // Server only, no NEXT_PUBLIC_ prefix, so the browser never calls the VPS
  // from the marketing site and no CORS surface is added. Unset is the correct
  // configuration for local development and for preview deployments.
  if (!origin) {
    return FALLBACK;
  }

  try {
    const response = await fetch(`${origin}/api/content`, {
      next: { revalidate: 300 },
      signal: AbortSignal.timeout(3000),
    });

    if (!response.ok) {
      return FALLBACK;
    }

    return validate(await response.json()) ?? FALLBACK;
  } catch {
    return FALLBACK;
  }
}

const str = (value: unknown): value is string => typeof value === "string" && value.length > 0;

/**
 * A partial failure falls back for the whole payload, never per set, so the
 * page is never a mix of one stale section and one fresh one.
 *
 * The layout invariants are checked here and not only on the write path,
 * because the write path is not the only way a row can reach this function: a
 * hand-run UPDATE, a restore from a backup taken mid-change, or a half-applied
 * seed all arrive the same way.
 */
function validate(payload: unknown): Content | null {
  if (typeof payload !== "object" || payload === null) {
    return null;
  }

  const raw = payload as Record<string, unknown>;

  const faq = raw.faq;
  const team = raw.team;
  const companies = raw.companies;
  const services = raw.services;

  if (!Array.isArray(faq) || !Array.isArray(team) || !Array.isArray(companies) || !Array.isArray(services)) {
    return null;
  }

  // Exactly four founders and exactly four services: the founders grid is four
  // across with an (i % 4) stagger, and the desktop stepper is a four column
  // grid with one connector drawn across it.
  if (faq.length < 1 || companies.length < 1 || team.length !== 4 || services.length !== 4) {
    return null;
  }

  if (!faq.every((f) => str(f?.q) && str(f?.a))) {
    return null;
  }

  if (!team.every((m) => str(m?.name) && str(m?.initials) && str(m?.role) && str(m?.focus)
    && typeof m?.photo === "string")) {
    return null;
  }

  if (!companies.every((c) => str(c?.name) && str(c?.file))) {
    return null;
  }

  const servicesOk = services.every((s) =>
    str(s?.id) && str(s?.name) && str(s?.tagline) && str(s?.cardLabel)
    && str(s?.description) && str(s?.icon)
    && Array.isArray(s?.highlights) && s.highlights.length > 0 && s.highlights.every(str)
    && Array.isArray(s?.steps) && s.steps.length === 4
    && s.steps.every((t: unknown) => {
      const step = t as Record<string, unknown>;
      return str(step?.phase) && str(step?.summary) && str(step?.what);
    }));

  if (!servicesOk) {
    return null;
  }

  return {
    faq: faq as Faq[],
    team: team as Member[],
    companies: companies as Company[],
    services: services as Service[],
  };
}
