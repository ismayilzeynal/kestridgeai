using Kestridge.Api.Data;
using Kestridge.Api.Infrastructure;
using Kestridge.Api.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Kestridge.Api.Admin;

public static class AdminEndpoints
{
    public static void MapAdmin(this IEndpointRouteBuilder app)
    {
        // Outside the group on purpose: the token filter must not run on the
        // endpoint whose job is to issue the token.
        app.MapPost("/api/admin/login", AdminLogin.Handle);

        // RequireCors is called on nothing here. The panel is same origin, so
        // the CORS middleware has nothing to do, and the "site" policy already
        // makes cross-origin admin calls impossible: Authorization is not in its
        // allowed headers and GET is not in its allowed methods. Do not widen it.
        var admin = app.MapGroup("/api/admin").AddEndpointFilter<AdminTokenFilter>();

        admin.MapGet("/session", (HttpContext http, KestridgeDbContext db) => Session(http, db));
        admin.MapPost("/logout", Logout);
        admin.MapGet("/jobs", Jobs);

        admin.MapGet("/submissions", AdminSubmissionEndpoints.List);
        admin.MapPost("/submissions/search", AdminSubmissionEndpoints.Search);
        admin.MapGet("/submissions/{id:long}", AdminSubmissionEndpoints.Detail);
        admin.MapPost("/submissions/{id:long}/handled", AdminSubmissionEndpoints.SetHandled);
        admin.MapPost("/submissions/{id:long}/legal-hold", AdminSubmissionEndpoints.SetLegalHold);
        admin.MapPost("/submissions/export", AdminSubmissionEndpoints.Export);

        admin.MapPost("/dsr/preview", AdminDsrEndpoints.Preview);
        admin.MapPost("/dsr/access", AdminDsrEndpoints.Access);
        admin.MapPost("/dsr/delete", AdminDsrEndpoints.Delete);
        admin.MapGet("/dsr/log", AdminDsrEndpoints.Log);

        admin.MapGet("/content", AdminContentEndpoints.Get);
        admin.MapGet("/assets", () => AdminContentEndpoints.Assets());

        // Delete is a POST like everything else. nginx-api.conf answers 405 to
        // PUT, PATCH and DELETE and that 405 carries no Access-Control-Allow-Origin,
        // so one non-POST mutation is an opaque browser failure with nothing in
        // the application log.
        admin.MapPost("/content/faq/save", AdminContentEndpoints.FaqSave);
        admin.MapPost("/content/faq/delete", AdminContentEndpoints.FaqDelete);
        admin.MapPost("/content/faq/reorder", AdminContentEndpoints.FaqReorder);

        admin.MapPost("/content/team/save", AdminContentEndpoints.TeamSave);
        admin.MapPost("/content/team/reorder", AdminContentEndpoints.TeamReorder);

        admin.MapPost("/content/companies/save", AdminContentEndpoints.CompanySave);
        admin.MapPost("/content/companies/add", AdminContentEndpoints.CompanyAdd);
        admin.MapPost("/content/companies/reorder", AdminContentEndpoints.CompanyReorder);

        admin.MapGet("/content/services", AdminContentEndpoints.ServicesGet);
        admin.MapPost("/content/services/save", AdminContentEndpoints.ServiceSave);
        admin.MapPost("/content/services/reorder", AdminContentEndpoints.ServiceReorder);

        // No MapFallbackToFile. It would catch an unmatched /api/admin/* and
        // answer HTML with a 200 where a JSON 404 was intended.
    }

    private static async Task<IResult> Session(HttpContext http, KestridgeDbContext db)
    {
        var who = AdminIdentity.Of(http);

        var session = await db.AdminSessions.AsNoTracking()
            .FirstOrDefaultAsync(x => x.TokenHash == who.TokenHash, http.RequestAborted);

        return session is null
            ? JsonResults.Auth()
            : Results.Json(new
            {
                ok = true,
                username = who.Username,
                displayName = who.DisplayName,
                absoluteExpiresAt = session.AbsoluteExpiresAt,
                idleExpiresAt = session.IdleExpiresAt,
            });
    }

    private static async Task<IResult> Logout(HttpContext http, KestridgeDbContext db, LogoutRequest? request)
    {
        var who = AdminIdentity.Of(http);

        if (request?.Everywhere == true)
        {
            await db.AdminSessions.Where(x => x.AccountId == who.AccountId).ExecuteDeleteAsync(CancellationToken.None);
        }
        else
        {
            await db.AdminSessions.Where(x => x.TokenHash == who.TokenHash).ExecuteDeleteAsync(CancellationToken.None);
        }

        return JsonResults.Ok();
    }

    // Answers "is it working" without an SSH session. No re-arm button:
    // ops/rearm-notifications.sql stays the process of record, because
    // re-arming before the cause is fixed just gets the sending IP throttled.
    private static async Task<IResult> Jobs(HttpContext http, KestridgeDbContext db)
    {
        var retention = await db.JobRuns.AsNoTracking()
            .Where(x => x.JobName == "retention_purge")
            .OrderByDescending(x => x.Id)
            .Take(10)
            .Select(x => new
            {
                startedAt = x.StartedAt,
                finishedAt = x.FinishedAt,
                outcome = x.Outcome,
                cutoffDate = x.CutoffDate,
                rowsAffected = x.RowsAffected,
                durationMs = x.DurationMs,
            })
            .ToListAsync(http.RequestAborted);

        var all = db.ContactSubmissions.AsNoTracking();

        return Results.Json(new
        {
            ok = true,
            retention,
            notify = new
            {
                pending = await all.CountAsync(x => x.NotifyState == NotifyState.Pending, http.RequestAborted),
                sent = await all.CountAsync(x => x.NotifyState == NotifyState.Sent, http.RequestAborted),
                failed = await all.CountAsync(x => x.NotifyState == NotifyState.Failed, http.RequestAborted),
            },
        });
    }
}
