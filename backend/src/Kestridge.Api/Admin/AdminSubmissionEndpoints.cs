using System.Globalization;
using System.Text;
using Kestridge.Api.Data;
using Kestridge.Api.Infrastructure;
using Kestridge.Api.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kestridge.Api.Admin;

public static class AdminSubmissionEndpoints
{
    private const int PreviewChars = 120;
    private const int MaxPageSize = 100;

    public static async Task<IResult> List(
        HttpContext http, KestridgeDbContext db, string? state, long? before, int? size)
    {
        var pageSize = Math.Clamp(size ?? 50, 1, MaxPageSize);

        var query = Filtered(db.ContactSubmissions.AsNoTracking(), state);

        // Keyset on id DESC, not OFFSET. The retention purge removes rows while
        // someone is paging, and an offset silently skips a row when it does.
        if (before is { } cursor)
        {
            query = query.Where(x => x.Id < cursor);
        }

        var rows = await query
            .OrderByDescending(x => x.Id)
            .Take(pageSize)
            .Select(x => new SubmissionRow(
                x.Id,
                x.CreatedAt,
                x.Name,
                x.Company,
                x.Service,
                x.Message.Length > PreviewChars ? x.Message.Substring(0, PreviewChars) : x.Message,
                x.NotifyState == NotifyState.Sent ? "sent" : x.NotifyState == NotifyState.Failed ? "failed" : "pending",
                x.HandledAt != null,
                x.LegalHold))
            .ToListAsync(http.RequestAborted);

        return Results.Json(new
        {
            ok = true,
            counts = await CountsAsync(db, http.RequestAborted),
            rows,
            nextBefore = rows.Count == pageSize ? rows[^1].Id : (long?)null,
        });
    }

    // A POST, and that is not stylistic. GET /api/admin/submissions?q=person@example.com
    // is written in full to /var/log/nginx/access.log, a file outside the
    // retention purge, outside ops/dsr-delete.sql, and outside the table in
    // DSR-PROCESS.md headed "These are the only copies". It is the single most
    // likely way an address escapes the deletion promise.
    public static async Task<IResult> Search(HttpContext http, KestridgeDbContext db, SearchRequest request)
    {
        var q = (request.Q ?? string.Empty).Trim();
        if (q.Length == 0)
        {
            return JsonResults.Invalid("q");
        }

        var query = Filtered(db.ContactSubmissions.AsNoTracking(), request.State);

        if (q.Contains('@', StringComparison.Ordinal))
        {
            // Exact, never LIKE. The column is utf8mb4_0900_as_cs and a loosened
            // match is how a DSR operation reaches a different person's mailbox.
            var email = q.ToLowerInvariant();
            query = query.Where(x => x.Email == email);
        }
        else
        {
            var pattern = "%" + Escape(q) + "%";
            query = query.Where(x =>
                EF.Functions.Like(x.Name, pattern, "\\") || EF.Functions.Like(x.Company, pattern, "\\"));
        }

        var rows = await query
            .OrderByDescending(x => x.Id)
            .Take(MaxPageSize)
            .Select(x => new SubmissionRow(
                x.Id,
                x.CreatedAt,
                x.Name,
                x.Company,
                x.Service,
                x.Message.Length > PreviewChars ? x.Message.Substring(0, PreviewChars) : x.Message,
                x.NotifyState == NotifyState.Sent ? "sent" : x.NotifyState == NotifyState.Failed ? "failed" : "pending",
                x.HandledAt != null,
                x.LegalHold))
            .ToListAsync(http.RequestAborted);

        return Results.Json(new { ok = true, rows });
    }

    public static async Task<IResult> Detail(HttpContext http, KestridgeDbContext db, long id)
    {
        var row = await db.ContactSubmissions.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, http.RequestAborted);

        return row is null ? JsonResults.Missing() : Results.Json(new { ok = true, submission = Detail(row) });
    }

    public static async Task<IResult> SetHandled(
        HttpContext http, KestridgeDbContext db, TimeProvider clock, long id, HandledRequest request)
    {
        var row = await db.ContactSubmissions.FirstOrDefaultAsync(x => x.Id == id, http.RequestAborted);
        if (row is null)
        {
            return JsonResults.Missing();
        }

        var who = AdminIdentity.Of(http);

        row.HandledAt = request.Handled ? clock.GetUtcNow().UtcDateTime : null;
        row.HandledBy = request.Handled ? Truncate(who.DisplayName, 64) : string.Empty;

        await db.SaveChangesAsync(CancellationToken.None);

        return Results.Json(new { ok = true, submission = Detail(row) });
    }

    public static async Task<IResult> SetLegalHold(
        HttpContext http, KestridgeDbContext db, ILogger<ContactSubmission> log, long id, LegalHoldRequest request)
    {
        var row = await db.ContactSubmissions.FirstOrDefaultAsync(x => x.Id == id, http.RequestAborted);
        if (row is null)
        {
            return JsonResults.Missing();
        }

        row.LegalHold = request.LegalHold;
        await db.SaveChangesAsync(CancellationToken.None);

        // No hold_reason column anywhere: staff free text about a data subject
        // is itself personal data, which would then have to appear in an access
        // response. DSR-PROCESS.md says record the reason outside the database.
        log.LogInformation("admin.legal_hold id={Id} value={Value}", id, request.LegalHold);

        return Results.Json(new { ok = true, submission = Detail(row) });
    }

    public static async Task<IResult> Export(
        HttpContext http,
        KestridgeDbContext db,
        IOptions<AdminOptions> adminOptions,
        TimeProvider clock,
        ILogger<ContactSubmission> log,
        ExportRequest request)
    {
        var max = adminOptions.Value.ExportMaxRows;

        var rows = await Filtered(db.ContactSubmissions.AsNoTracking(), request.State)
            .OrderByDescending(x => x.Id)
            .Take(max + 1)
            .ToListAsync(http.RequestAborted);

        var truncated = rows.Count > max;
        if (truncated)
        {
            rows.RemoveAt(rows.Count - 1);
        }

        var sb = new StringBuilder();
        var header = new List<string>
        {
            "id", "received_utc", "name", "email", "company", "phone",
            "service", "notify_state", "handled_at", "handled_by", "legal_hold", "purge_after",
        };

        if (request.IncludeMessage)
        {
            header.Add("message");
        }

        Csv.AppendRow(sb, [.. header]);

        foreach (var r in rows)
        {
            var fields = new List<string?>
            {
                r.Id.ToString(CultureInfo.InvariantCulture),
                r.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                r.Name,
                r.Email,
                r.Company,
                r.Phone,
                r.Service,
                r.NotifyState.ToString().ToLowerInvariant(),
                r.HandledAt?.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) ?? string.Empty,
                r.HandledBy,
                r.LegalHold ? "1" : "0",
                r.PurgeAfter.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            };

            if (request.IncludeMessage)
            {
                fields.Add(r.Message);
            }

            Csv.AppendRow(sb, [.. fields]);
        }

        if (truncated)
        {
            sb.Append("#truncated\r\n");
        }

        // Counts only. LoggingHygieneTests fails the build if a field value ever
        // reaches a log line on this path.
        log.LogInformation("admin.export rows={Count} withMessage={Flag}", rows.Count, request.IncludeMessage);

        var day = clock.GetUtcNow().UtcDateTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        var body = Csv.Bom.Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();

        return Results.File(body, "text/csv; charset=utf-8", $"kestridge-submissions-{day}.csv");
    }

    private static IQueryable<ContactSubmission> Filtered(IQueryable<ContactSubmission> query, string? state) => state switch
    {
        "new" => query.Where(x => x.HandledAt == null),
        "handled" => query.Where(x => x.HandledAt != null),
        "held" => query.Where(x => x.LegalHold),
        "notify_failed" => query.Where(x => x.NotifyState == NotifyState.Failed),
        _ => query,
    };

    private static async Task<object> CountsAsync(KestridgeDbContext db, CancellationToken ct)
    {
        var all = db.ContactSubmissions.AsNoTracking();

        return new
        {
            all = await all.CountAsync(ct),
            @new = await all.CountAsync(x => x.HandledAt == null, ct),
            handled = await all.CountAsync(x => x.HandledAt != null, ct),
            held = await all.CountAsync(x => x.LegalHold, ct),
            notifyFailed = await all.CountAsync(x => x.NotifyState == NotifyState.Failed, ct),
        };
    }

    private static SubmissionDetail Detail(ContactSubmission x) => new(
        x.Id, x.CreatedAt, x.Name, x.Email, x.Company, x.Phone, x.Service, x.Message,
        x.NotifyState == NotifyState.Sent ? "sent" : x.NotifyState == NotifyState.Failed ? "failed" : "pending",
        x.NotifyAttempts, x.NotifiedAt, x.NotifyError,
        x.HandledAt != null, x.HandledAt, x.HandledBy, x.LegalHold, x.PurgeAfter);

    // Backslash first, or it escapes the escapes added after it.
    private static string Escape(string value)
        => value.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");

    private static string Truncate(string value, int max)
        => value.Length <= max ? value : value[..max];
}
