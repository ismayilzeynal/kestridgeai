using System.Globalization;
using System.Text.Json;
using Kestridge.Api.Data;
using Kestridge.Api.Infrastructure;
using Kestridge.Api.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kestridge.Api.Admin;

public static class AdminDsrEndpoints
{
    private const int AffectedIdsMax = 500;

    private static readonly string[] RequestTypes = ["delete", "correct", "object"];

    public static async Task<IResult> Preview(
        HttpContext http, KestridgeDbContext db, IOptions<DsrOptions> dsr, DsrPreviewRequest request)
    {
        if (Normalize(request.Email) is not { } email)
        {
            return JsonResults.Invalid("email");
        }

        var rows = await db.ContactSubmissions.AsNoTracking()
            .Where(x => x.Email == email)
            .OrderByDescending(x => x.Id)
            .Select(x => new { x.Id, x.LegalHold })
            .ToListAsync(http.RequestAborted);

        var hash = DsrHash.For(dsr.Value.EmailHashPepper, email);
        var prior = await db.DsrLog.AsNoTracking().CountAsync(x => x.SubjectEmailHash == hash, http.RequestAborted);

        return Results.Json(new
        {
            ok = true,
            subject = email,
            ids = rows.Select(r => r.Id).ToArray(),
            rowsFound = rows.Count,
            blockedByLegalHold = rows.Count(r => r.LegalHold),

            // Answers "has this subject asked before" without the log ever
            // holding the address, exactly as ops/dsr-log.sql does.
            priorRequests = prior,
        });
    }

    public static async Task<IResult> Access(
        HttpContext http,
        KestridgeDbContext db,
        IOptions<DsrOptions> dsr,
        TimeProvider clock,
        DsrAccessRequest request)
    {
        if (Normalize(request.Email) is not { } email)
        {
            return JsonResults.Invalid("email");
        }

        if (ParseDate(request.ReceivedOn) is not { } receivedOn)
        {
            return JsonResults.Invalid("receivedOn");
        }

        var rows = await db.ContactSubmissions.AsNoTracking()
            .Where(x => x.Email == email)
            .OrderByDescending(x => x.Id)
            .ToListAsync(http.RequestAborted);

        // The same object shape ops/dsr-export.sql produces, so a subject gets
        // the same answer whichever path the operator used.
        var record = JsonSerializer.Serialize(
            rows.Select(x => new
            {
                id = x.Id,
                received_utc = x.CreatedAt.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture),
                name = x.Name,
                email = x.Email,
                company = x.Company,
                phone = x.Phone,
                service = x.Service,
                message = x.Message,
                deleted_after = x.PurgeAfter.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            }),
            new JsonSerializerOptions { WriteIndented = true });

        var who = AdminIdentity.Of(http);

        db.DsrLog.Add(new DsrLogEntry
        {
            ReceivedOn = receivedOn,
            RequestType = "access",
            SubjectEmailHash = DsrHash.For(dsr.Value.EmailHashPepper, email),
            RowsAffected = (uint)rows.Count,
            AffectedIds = JoinIds(rows.Select(r => r.Id), out _),
            HandledBy = Truncate(who.DisplayName, 64),
            ClosedOn = DateOnly.FromDateTime(clock.GetUtcNow().UtcDateTime),
        });

        await db.SaveChangesAsync(CancellationToken.None);

        return Results.Json(new { ok = true, rowsFound = rows.Count, record });
    }

    public static async Task<IResult> Delete(
        HttpContext http,
        KestridgeDbContext db,
        IOptions<DsrOptions> dsr,
        TimeProvider clock,
        ILogger<DsrLogEntry> log,
        DsrDeleteRequest request)
    {
        if (Normalize(request.Email) is not { } email)
        {
            return JsonResults.Invalid("email");
        }

        // Byte for byte, server side, so a client bug cannot bypass the
        // confirmation the operator was shown.
        if (!string.Equals(request.Email, request.ConfirmEmail, StringComparison.Ordinal))
        {
            return JsonResults.Invalid("confirmEmail");
        }

        if (ParseDate(request.ReceivedOn) is not { } receivedOn)
        {
            return JsonResults.Invalid("receivedOn");
        }

        var requestType = (request.RequestType ?? string.Empty).Trim().ToLowerInvariant();
        if (Array.IndexOf(RequestTypes, requestType) < 0)
        {
            return JsonResults.Invalid("requestType");
        }

        var rows = await db.ContactSubmissions
            .Where(x => x.Email == email)
            .OrderByDescending(x => x.Id)
            .ToListAsync(http.RequestAborted);

        // The id set must match what the operator was shown. A submission that
        // arrived between the preview and the click cannot be deleted unseen.
        var current = rows.Select(r => r.Id).OrderBy(id => id).ToArray();
        var echoed = (request.Ids ?? []).OrderBy(id => id).ToArray();

        if (!current.SequenceEqual(echoed))
        {
            return JsonResults.Stale();
        }

        var deletable = rows.Where(r => !r.LegalHold).ToList();
        var blocked = rows.Count - deletable.Count;

        db.ContactSubmissions.RemoveRange(deletable);

        var who = AdminIdentity.Of(http);
        var affectedIds = JoinIds(deletable.Select(r => r.Id), out var idsTruncated);

        var entry = new DsrLogEntry
        {
            ReceivedOn = receivedOn,
            RequestType = requestType,
            SubjectEmailHash = DsrHash.For(dsr.Value.EmailHashPepper, email),

            // The true count even when the id list had to be cut.
            RowsAffected = (uint)deletable.Count,
            AffectedIds = affectedIds,
            HandledBy = Truncate(who.DisplayName, 64),
            ClosedOn = DateOnly.FromDateTime(clock.GetUtcNow().UtcDateTime),
        };

        db.DsrLog.Add(entry);

        // One transaction: the rows and the record of their deletion commit
        // together or neither does. EnableRetryOnFailure is not configured, so
        // no execution strategy wrapper is needed here.
        await using var tx = await db.Database.BeginTransactionAsync(CancellationToken.None);
        await db.SaveChangesAsync(CancellationToken.None);
        await tx.CommitAsync(CancellationToken.None);

        log.LogInformation("admin.dsr_delete rows={Rows} blocked={Blocked}", deletable.Count, blocked);

        return Results.Json(new
        {
            ok = true,
            deleted = deletable.Count,
            blockedByLegalHold = blocked,
            dsrLogId = entry.Id,
            idsTruncated,
        });
    }

    public static async Task<IResult> Log(HttpContext http, KestridgeDbContext db, TimeProvider clock, int? months)
    {
        var window = Math.Clamp(months ?? 12, 1, 120);
        var from = DateOnly.FromDateTime(clock.GetUtcNow().UtcDateTime.AddMonths(-window));

        var rows = await db.DsrLog.AsNoTracking()
            .Where(x => x.ReceivedOn >= from)
            .OrderByDescending(x => x.Id)
            .Select(x => new
            {
                x.Id,
                receivedOn = x.ReceivedOn,
                requestType = x.RequestType,
                rowsAffected = x.RowsAffected,
                affectedIds = x.AffectedIds,
                handledBy = x.HandledBy,
                closedOn = x.ClosedOn,
            })
            .ToListAsync(http.RequestAborted);

        // subject_email_hash is deliberately not returned. It is useless to a
        // human and shipping it makes the panel a place to confirm a guessed
        // address against the pepper.
        return Results.Json(new { ok = true, rows });
    }

    private static string? Normalize(string? email)
    {
        var value = (email ?? string.Empty).Trim().ToLowerInvariant();
        return value.Length is > 0 and <= 254 && value.Contains('@', StringComparison.Ordinal) ? value : null;
    }

    private static DateOnly? ParseDate(string? value)
        => DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed)
            ? parsed
            : null;

    private static string JoinIds(IEnumerable<long> ids, out bool truncated)
    {
        var sb = new System.Text.StringBuilder();
        truncated = false;

        foreach (var id in ids)
        {
            var next = sb.Length == 0 ? id.ToString(CultureInfo.InvariantCulture) : "," + id.ToString(CultureInfo.InvariantCulture);
            if (sb.Length + next.Length > AffectedIdsMax)
            {
                truncated = true;
                break;
            }

            sb.Append(next);
        }

        return sb.ToString();
    }

    private static string Truncate(string value, int max)
        => value.Length <= max ? value : value[..max];
}
