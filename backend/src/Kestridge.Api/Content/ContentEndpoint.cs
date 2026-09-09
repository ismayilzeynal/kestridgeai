using Kestridge.Api.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Kestridge.Api.Content;

public static class ContentEndpoint
{
    public static void MapContent(this IEndpointRouteBuilder app)
    {
        // No token, and no RequireCors. This is called server to server by
        // Vercel's ISR revalidation, never from a browser, and it carries only
        // what the public website already shows to everyone.
        app.MapGet("/api/content", Handle);
    }

    private static async Task<IResult> Handle(HttpContext http, KestridgeDbContext db, TimeProvider clock)
    {
        var ct = http.RequestAborted;

        var faq = await db.SiteFaq.AsNoTracking()
            .OrderBy(x => x.SortOrder).ThenBy(x => x.Id)
            .Select(x => new FaqPayload(x.Question, x.Answer))
            .ToArrayAsync(ct);

        var team = await db.SiteTeam.AsNoTracking()
            .OrderBy(x => x.SortOrder).ThenBy(x => x.Id)
            .Select(x => new TeamPayload(x.Name, x.Initials, x.Role, x.Focus, x.Photo))
            .ToArrayAsync(ct);

        // The column holds a basename. The public path is assembled here so a
        // stored value can never point at another origin, and Team.tsx keeps
        // receiving the full path it already expects.
        team = [.. team.Select(m => m with { Photo = m.Photo.Length == 0 ? string.Empty : $"/team/{m.Photo}.jpg" })];

        var companies = await db.SiteCompanies.AsNoTracking()
            .Where(x => !x.Hidden)
            .OrderBy(x => x.SortOrder).ThenBy(x => x.Id)
            .Select(x => new CompanyPayload(x.Name, x.LogoFile))
            .ToArrayAsync(ct);

        var services = await db.SiteServices.AsNoTracking()
            .OrderBy(x => x.SortOrder).ThenBy(x => x.Id)
            .ToListAsync(ct);

        // Three queries and a join in memory rather than an Include: there is
        // no navigation property and no foreign key anywhere in this model, and
        // four services is not a set worth an N+1 discussion.
        var steps = await db.SiteServiceSteps.AsNoTracking()
            .OrderBy(x => x.ServiceId).ThenBy(x => x.SortOrder).ThenBy(x => x.Id)
            .ToListAsync(ct);

        var highlights = await db.SiteServiceHighlights.AsNoTracking()
            .OrderBy(x => x.ServiceId).ThenBy(x => x.SortOrder).ThenBy(x => x.Id)
            .ToListAsync(ct);

        var payload = new ContentPayload(
            clock.GetUtcNow().UtcDateTime,
            faq,
            team,
            companies,
            [.. services.Select(s => new ServicePayload(
                s.Slug,
                s.Name,
                s.Tagline,
                s.CardLabel,
                s.Description,
                s.IconName,
                [.. highlights.Where(h => h.ServiceId == s.Id).Select(h => h.Text)],
                [.. steps.Where(t => t.ServiceId == s.Id)
                    .Select(t => new ServiceStepPayload(t.Phase, t.Summary, t.Detail))]))]);

        // Empty tables return 200 with empty arrays, never a 500. The frontend
        // decides to fall back, not the API: getContent() rejects an empty set
        // and renders the committed constants, which is the correct answer
        // before ops/05-seed-content.sql has ever been run.
        return Results.Json(payload);
    }
}
