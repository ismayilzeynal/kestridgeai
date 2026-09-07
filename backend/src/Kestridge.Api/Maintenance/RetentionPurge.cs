using Kestridge.Api.Data;
using Kestridge.Api.Options;
using Microsoft.EntityFrameworkCore;

namespace Kestridge.Api.Maintenance;

public sealed class RetentionPurge(KestridgeDbContext db, RetentionOptions options, TimeProvider clock)
{
    public const string JobName = "retention_purge";

    public async Task<int> RunAsync(CancellationToken ct)
    {
        var now = clock.GetUtcNow().UtcDateTime;
        var today = DateOnly.FromDateTime(now);

        var alreadyRan = await db.JobRuns.AnyAsync(
            j => j.JobName == JobName && j.Outcome == "ok" && j.CutoffDate == today, ct);

        if (alreadyRan)
        {
            return 0;
        }

        var run = new JobRun
        {
            JobName = JobName,
            StartedAt = now,
            Outcome = "running",
            CutoffDate = today,
        };

        db.JobRuns.Add(run);
        await db.SaveChangesAsync(ct);

        var started = clock.GetTimestamp();
        var total = 0;

        try
        {
            int deleted;
            do
            {
                // Cutoff comes from the injected clock, not from the server's
                // UTC_TIMESTAMP(), so the job is testable against a fake clock.
                deleted = await db.ContactSubmissions
                    .Where(x => !x.LegalHold && x.PurgeAfter <= today)
                    .OrderBy(x => x.Id)
                    .Take(options.BatchSize)
                    .ExecuteDeleteAsync(ct);

                total += deleted;
            }
            while (deleted == options.BatchSize);

            run.Outcome = "ok";
        }
        catch (Exception ex)
        {
            run.Outcome = "error";
            run.Detail = ex.GetType().Name;
            throw;
        }
        finally
        {
            run.FinishedAt = clock.GetUtcNow().UtcDateTime;
            run.RowsAffected = (uint)total;
            run.DurationMs = (uint)clock.GetElapsedTime(started).TotalMilliseconds;
            await db.SaveChangesAsync(CancellationToken.None);
        }

        return total;
    }
}
