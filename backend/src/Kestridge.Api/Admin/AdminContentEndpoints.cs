using Kestridge.Api.Data;
using Kestridge.Api.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Kestridge.Api.Admin;

public static class AdminContentEndpoints
{
    // The layout encodes these. lg:grid-cols-4 with delay={(i % 4) * 0.06} puts
    // a fifth founder alone on his own row, and the desktop stepper is
    // grid-cols-4 with an absolutely positioned connector across it.
    private const int RequiredFounders = 4;
    private const int RequiredSteps = 4;
    private const int MinHighlights = 3;
    private const int MaxHighlights = 6;

    // An empty FAQ also removes the FAQPage structured data from the page, so
    // the last row cannot be deleted from the panel.
    private const int MinFaq = 1;

    public static async Task<IResult> Get(HttpContext http, KestridgeDbContext db)
    {
        var ct = http.RequestAborted;

        var faq = await db.SiteFaq.AsNoTracking()
            .OrderBy(x => x.SortOrder).ThenBy(x => x.Id)
            .Select(x => new FaqAdminRow(x.Id, x.Question, x.Answer, x.SortOrder, x.UpdatedAt, x.UpdatedBy))
            .ToListAsync(ct);

        var team = await db.SiteTeam.AsNoTracking()
            .OrderBy(x => x.SortOrder).ThenBy(x => x.Id)
            .Select(x => new TeamAdminRow(
                x.Id, x.Name, x.Initials, x.Role, x.Focus, x.Photo, x.SortOrder, x.UpdatedAt, x.UpdatedBy))
            .ToListAsync(ct);

        // Hidden rows are included here and excluded from /api/content. The
        // panel is where you go to unhide one.
        var companies = await db.SiteCompanies.AsNoTracking()
            .OrderBy(x => x.SortOrder).ThenBy(x => x.Id)
            .Select(x => new CompanyAdminRow(
                x.Id, x.Name, x.LogoFile, x.Hidden, x.SortOrder, x.UpdatedAt, x.UpdatedBy))
            .ToListAsync(ct);

        var services = await ServiceRowsAsync(db, ct);

        // Computed from the lists already loaded rather than four more
        // round trips for one timestamp.
        var stamps = faq.Select(x => x.UpdatedAt)
            .Concat(team.Select(x => x.UpdatedAt))
            .Concat(companies.Select(x => x.UpdatedAt))
            .Concat(services.Select(x => x.UpdatedAt))
            .ToList();

        return Results.Json(new
        {
            ok = true,
            faq,
            team,
            companies,
            services,
            lastPublishedAt = stamps.Count == 0 ? (DateTime?)null : stamps.Max(),
        });
    }

    // A compiled allowlist, not a directory listing. The API runs on the VPS
    // and cannot see Vercel's filesystem, so it has no way to discover what is
    // actually deployed; a list it can be sure of is better than a guess.
    public static IResult Assets() => Results.Json(new
    {
        ok = true,
        photos = ContentValidation.Photos,
        logos = ContentValidation.Logos,
        icons = new { service = ContentValidation.ServiceIcons },
    });

    // ---- questions and answers: the one set with full create and delete ----

    public static async Task<IResult> FaqSave(
        HttpContext http, KestridgeDbContext db, TimeProvider clock, Revalidate revalidate, FaqSaveRequest request)
    {
        if (ContentValidation.Text("question", request.Question, 200, out var question) is { } q)
        {
            return Invalid(q);
        }

        if (ContentValidation.Text("answer", request.Answer, 600, out var answer) is { } a)
        {
            return Invalid(a);
        }

        SiteFaq row;

        if (request.Id is { } id)
        {
            var found = await db.SiteFaq.FirstOrDefaultAsync(x => x.Id == id, http.RequestAborted);
            if (found is null)
            {
                return JsonResults.Missing();
            }

            row = found;
        }
        else
        {
            row = new SiteFaq { SortOrder = await NextFaqOrderAsync(db, http.RequestAborted) };
            db.SiteFaq.Add(row);
        }

        row.Question = question;
        row.Answer = answer;
        (row.UpdatedAt, row.UpdatedBy) = Stamp(http, clock);

        await db.SaveChangesAsync(CancellationToken.None);

        return await SavedAsync(revalidate, new FaqAdminRow(
            row.Id, row.Question, row.Answer, row.SortOrder, row.UpdatedAt, row.UpdatedBy));
    }

    public static async Task<IResult> FaqDelete(
        HttpContext http, KestridgeDbContext db, Revalidate revalidate, ContentIdRequest request)
    {
        var row = await db.SiteFaq.FirstOrDefaultAsync(x => x.Id == request.Id, http.RequestAborted);
        if (row is null)
        {
            return JsonResults.Missing();
        }

        if (await db.SiteFaq.CountAsync(http.RequestAborted) <= MinFaq)
        {
            return Invalid(new ContentError("faq", "count"));
        }

        db.SiteFaq.Remove(row);
        await db.SaveChangesAsync(CancellationToken.None);

        return Results.Json(new { ok = true, published = await revalidate.PublishAsync() });
    }

    public static Task<IResult> FaqReorder(
        HttpContext http, KestridgeDbContext db, Revalidate revalidate, ReorderRequest request)
        => ReorderAsync(http, db, db.SiteFaq, revalidate, request, "faq");

    // ---- founders: edit and reorder only ----

    public static async Task<IResult> TeamSave(
        HttpContext http, KestridgeDbContext db, TimeProvider clock, Revalidate revalidate, TeamSaveRequest request)
    {
        // No creation, and there is no delete endpoint at all. The founders
        // grid is lg:grid-cols-4 and a fifth person is an orphan on his own
        // row; the panel says so under the list rather than offering a button
        // that always refuses.
        if (request.Id is not { } id)
        {
            return Invalid(new ContentError("id", "immutable"));
        }

        var row = await db.SiteTeam.FirstOrDefaultAsync(x => x.Id == id, http.RequestAborted);
        if (row is null)
        {
            return JsonResults.Missing();
        }

        if (ContentValidation.Text("name", request.Name, 80, out var name) is { } n)
        {
            return Invalid(n);
        }

        if (ContentValidation.Initials("initials", request.Initials, out var initials) is { } i)
        {
            return Invalid(i);
        }

        if (ContentValidation.Text("role", request.Role, 60, out var role) is { } r)
        {
            return Invalid(r);
        }

        if (ContentValidation.Text("focus", request.Focus, 120, out var focus) is { } f)
        {
            return Invalid(f);
        }

        // An empty photo is legal and means the initials avatar, which is the
        // documented behaviour of the Member type.
        var photo = string.Empty;
        if (!string.IsNullOrWhiteSpace(request.Photo)
            && ContentValidation.Asset("photo", request.Photo, ContentValidation.Photos, out photo) is { } p)
        {
            return Invalid(p);
        }

        if (await db.SiteTeam.CountAsync(http.RequestAborted) != RequiredFounders)
        {
            return Invalid(new ContentError("team", "count"));
        }

        row.Name = name;
        row.Initials = initials;
        row.Role = role;
        row.Focus = focus;
        row.Photo = photo;
        (row.UpdatedAt, row.UpdatedBy) = Stamp(http, clock);

        await db.SaveChangesAsync(CancellationToken.None);

        return await SavedAsync(revalidate, new TeamAdminRow(
            row.Id, row.Name, row.Initials, row.Role, row.Focus, row.Photo,
            row.SortOrder, row.UpdatedAt, row.UpdatedBy));
    }

    public static Task<IResult> TeamReorder(
        HttpContext http, KestridgeDbContext db, Revalidate revalidate, ReorderRequest request)
        => ReorderAsync(http, db, db.SiteTeam, revalidate, request, "team");

    // ---- client logos: edit, hide, reorder, and add from an unused file ----

    public static async Task<IResult> CompanySave(
        HttpContext http, KestridgeDbContext db, TimeProvider clock, Revalidate revalidate, CompanySaveRequest request)
    {
        if (request.Id is not { } id)
        {
            return Invalid(new ContentError("id", "immutable"));
        }

        var row = await db.SiteCompanies.FirstOrDefaultAsync(x => x.Id == id, http.RequestAborted);
        if (row is null)
        {
            return JsonResults.Missing();
        }

        if (ContentValidation.Text("name", request.Name, 80, out var name) is { } n)
        {
            return Invalid(n);
        }

        row.Name = name;
        row.Hidden = request.Hidden;
        (row.UpdatedAt, row.UpdatedBy) = Stamp(http, clock);

        await db.SaveChangesAsync(CancellationToken.None);

        return await SavedAsync(revalidate, new CompanyAdminRow(
            row.Id, row.Name, row.LogoFile, row.Hidden, row.SortOrder, row.UpdatedAt, row.UpdatedBy));
    }

    public static async Task<IResult> CompanyAdd(
        HttpContext http, KestridgeDbContext db, TimeProvider clock, Revalidate revalidate, CompanyAddRequest request)
    {
        if (ContentValidation.Asset("logoFile", request.LogoFile, ContentValidation.Logos, out var file) is { } l)
        {
            return Invalid(l);
        }

        if (ContentValidation.Text("name", request.Name, 80, out var name) is { } n)
        {
            return Invalid(n);
        }

        // The unique index would refuse this anyway, with a 500. Saying so
        // plainly is better than an exception the operator cannot read.
        if (await db.SiteCompanies.AnyAsync(x => x.LogoFile == file, http.RequestAborted))
        {
            return Invalid(new ContentError("logoFile", "asset"));
        }

        var row = new SiteCompany
        {
            Name = name,
            LogoFile = file,
            SortOrder = await NextCompanyOrderAsync(db, http.RequestAborted),
        };

        (row.UpdatedAt, row.UpdatedBy) = Stamp(http, clock);
        db.SiteCompanies.Add(row);
        await db.SaveChangesAsync(CancellationToken.None);

        return await SavedAsync(revalidate, new CompanyAdminRow(
            row.Id, row.Name, row.LogoFile, row.Hidden, row.SortOrder, row.UpdatedAt, row.UpdatedBy));
    }

    public static Task<IResult> CompanyReorder(
        HttpContext http, KestridgeDbContext db, Revalidate revalidate, ReorderRequest request)
        => ReorderAsync(http, db, db.SiteCompanies, revalidate, request, "companies");

    // ---- services: edit and reorder only, slug untouchable ----

    public static async Task<IResult> ServicesGet(HttpContext http, KestridgeDbContext db)
        => Results.Json(new { ok = true, services = await ServiceRowsAsync(db, http.RequestAborted) });

    public static async Task<IResult> ServiceSave(
        HttpContext http,
        KestridgeDbContext db,
        TimeProvider clock,
        Revalidate revalidate,
        ILogger<SiteService> log,
        ServiceSaveRequest request)
    {
        if (request.Id is not { } id)
        {
            return Invalid(new ContentError("id", "immutable"));
        }

        var row = await db.SiteServices.FirstOrDefaultAsync(x => x.Id == id, http.RequestAborted);
        if (row is null)
        {
            return JsonResults.Missing();
        }

        if (ContentValidation.Text("name", request.Name, 60, out var name) is { } n)
        {
            return Invalid(n);
        }

        if (ContentValidation.Text("tagline", request.Tagline, 120, out var tagline) is { } t)
        {
            return Invalid(t);
        }

        if (ContentValidation.Text("cardLabel", request.CardLabel, 60, out var cardLabel) is { } c)
        {
            return Invalid(c);
        }

        if (ContentValidation.Text("description", request.Description, 400, out var description) is { } d)
        {
            return Invalid(d);
        }

        if (ContentValidation.Icon("iconName", request.IconName, out var icon) is { } ic)
        {
            return Invalid(ic);
        }

        var highlights = request.Highlights ?? [];
        if (highlights.Length is < MinHighlights or > MaxHighlights)
        {
            return Invalid(new ContentError("highlights", "count"));
        }

        var cleanHighlights = new string[highlights.Length];
        for (var i = 0; i < highlights.Length; i++)
        {
            if (ContentValidation.Text("highlights", highlights[i], 80, out var text) is { } h)
            {
                return Invalid(h);
            }

            cleanHighlights[i] = text;
        }

        var steps = request.Steps ?? [];
        if (steps.Length != RequiredSteps)
        {
            return Invalid(new ContentError("steps", "count"));
        }

        var cleanSteps = new ServiceStepRow[steps.Length];
        for (var i = 0; i < steps.Length; i++)
        {
            if (ContentValidation.Text("steps.phase", steps[i].Phase, 40, out var phase) is { } p)
            {
                return Invalid(p);
            }

            if (ContentValidation.Text("steps.summary", steps[i].Summary, 80, out var summary) is { } s)
            {
                return Invalid(s);
            }

            if (ContentValidation.Text("steps.what", steps[i].What, 240, out var what) is { } w)
            {
                return Invalid(w);
            }

            cleanSteps[i] = new ServiceStepRow(phase, summary, what);
        }

        row.Name = name;
        row.Tagline = tagline;
        row.CardLabel = cardLabel;
        row.Description = description;
        row.IconName = icon;
        (row.UpdatedAt, row.UpdatedBy) = Stamp(http, clock);

        // Replaced wholesale rather than diffed. The order of a step is its
        // meaning, and matching rows up by position and editing in place is
        // more code for a set of four.
        await using var transaction = await db.Database.BeginTransactionAsync(CancellationToken.None);

        db.SiteServiceSteps.RemoveRange(await db.SiteServiceSteps.Where(x => x.ServiceId == row.Id)
            .ToListAsync(CancellationToken.None));
        db.SiteServiceHighlights.RemoveRange(await db.SiteServiceHighlights.Where(x => x.ServiceId == row.Id)
            .ToListAsync(CancellationToken.None));

        await db.SaveChangesAsync(CancellationToken.None);

        for (var i = 0; i < cleanSteps.Length; i++)
        {
            db.SiteServiceSteps.Add(new SiteServiceStep
            {
                ServiceId = row.Id,
                Phase = cleanSteps[i].Phase,
                Summary = cleanSteps[i].Summary,
                Detail = cleanSteps[i].What,
                SortOrder = i,
            });
        }

        for (var i = 0; i < cleanHighlights.Length; i++)
        {
            db.SiteServiceHighlights.Add(new SiteServiceHighlight
            {
                ServiceId = row.Id,
                Text = cleanHighlights[i],
                SortOrder = i,
            });
        }

        await db.SaveChangesAsync(CancellationToken.None);
        await transaction.CommitAsync(CancellationToken.None);

        log.LogInformation("admin.service_saved id={Id}", row.Id);

        return await SavedAsync(revalidate, new ServiceAdminRow(
            row.Id, row.Slug, row.Name, row.Tagline, row.CardLabel, row.Description, row.IconName,
            cleanHighlights, cleanSteps, row.SortOrder, row.UpdatedAt, row.UpdatedBy));
    }

    public static Task<IResult> ServiceReorder(
        HttpContext http, KestridgeDbContext db, Revalidate revalidate, ReorderRequest request)
        => ReorderAsync(http, db, db.SiteServices, revalidate, request, "services");

    // ---- shared ----

    // Requires the complete set of ids for that set, exactly once each. A
    // partial or foreign list is a 400, which makes reorder idempotent and
    // impossible to half apply: there is no ordering the caller can send that
    // leaves two rows sharing a position.
    private static async Task<IResult> ReorderAsync<T>(
        HttpContext http,
        KestridgeDbContext db,
        DbSet<T> set,
        Revalidate revalidate,
        ReorderRequest request,
        string field)
        where T : class, ISortableContent
    {
        var wanted = request.Ids ?? [];
        var rows = await set.ToListAsync(http.RequestAborted);

        if (wanted.Length != rows.Count
            || wanted.Distinct().Count() != wanted.Length
            || wanted.Any(id => rows.TrueForAll(r => r.Id != id)))
        {
            return Invalid(new ContentError(field, "count"));
        }

        for (var i = 0; i < wanted.Length; i++)
        {
            rows.First(r => r.Id == wanted[i]).SortOrder = i;
        }

        await db.SaveChangesAsync(CancellationToken.None);

        // Reports published the same way a save does, so the panel can say the
        // same true thing about a reorder as about an edit.
        return Results.Json(new { ok = true, published = await revalidate.PublishAsync() });
    }

    private static async Task<List<ServiceAdminRow>> ServiceRowsAsync(KestridgeDbContext db, CancellationToken ct)
    {
        var services = await db.SiteServices.AsNoTracking()
            .OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync(ct);

        var steps = await db.SiteServiceSteps.AsNoTracking()
            .OrderBy(x => x.ServiceId).ThenBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync(ct);

        var highlights = await db.SiteServiceHighlights.AsNoTracking()
            .OrderBy(x => x.ServiceId).ThenBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync(ct);

        return
        [
            .. services.Select(s => new ServiceAdminRow(
                s.Id, s.Slug, s.Name, s.Tagline, s.CardLabel, s.Description, s.IconName,
                [.. highlights.Where(h => h.ServiceId == s.Id).Select(h => h.Text)],
                [.. steps.Where(t => t.ServiceId == s.Id).Select(t => new ServiceStepRow(t.Phase, t.Summary, t.Detail))],
                s.SortOrder, s.UpdatedAt, s.UpdatedBy)),
        ];
    }

    // Not generic over ISortableContent. EF would be asked to translate a
    // member access whose declaring type is the interface, and the shape of
    // that failure is a runtime exception on a query that compiles cleanly.
    private static async Task<int> NextFaqOrderAsync(KestridgeDbContext db, CancellationToken ct)
        => await db.SiteFaq.AsNoTracking().Select(x => x.SortOrder)
            .OrderByDescending(x => x).FirstOrDefaultAsync(ct) + 1;

    private static async Task<int> NextCompanyOrderAsync(KestridgeDbContext db, CancellationToken ct)
        => await db.SiteCompanies.AsNoTracking().Select(x => x.SortOrder)
            .OrderByDescending(x => x).FirstOrDefaultAsync(ct) + 1;

    private static (DateTime At, string By) Stamp(HttpContext http, TimeProvider clock)
    {
        var who = AdminIdentity.Of(http);

        return (clock.GetUtcNow().UtcDateTime,
            who.DisplayName.Length > 64 ? who.DisplayName[..64] : who.DisplayName);
    }

    // published reports only what the API actually knows. The save has already
    // committed either way, and the panel never claims a build ran or a CDN was
    // purged, because neither happens: a database write triggers no Vercel
    // build at all, which is correct and intended.
    private static async Task<IResult> SavedAsync(Revalidate revalidate, object row)
        => Results.Json(new { ok = true, row, published = await revalidate.PublishAsync() });

    private static IResult Invalid(ContentError error) => Results.Json(
        new { ok = false, error = "invalid", field = error.Field, reason = error.Reason },
        statusCode: StatusCodes.Status400BadRequest);
}
