using System.Globalization;
using Kestridge.Api.Data;
using Kestridge.Api.Email;
using Kestridge.Api.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Kestridge.Api.Maintenance;

public sealed class NotifySweep(
    KestridgeDbContext db,
    IEmailSender mail,
    ContactOptions contactOptions,
    NotifyOptions notifyOptions,
    TimeProvider clock,
    ILogger logger)
{
    // Indexed by the new attempt count. Total unattended coverage is about
    // 41 hours, which survives an overnight or weekend provider outage.
    private static readonly TimeSpan[] Backoff =
    [
        TimeSpan.FromMinutes(1),
        TimeSpan.FromMinutes(5),
        TimeSpan.FromMinutes(15),
        TimeSpan.FromHours(1),
        TimeSpan.FromHours(4),
        TimeSpan.FromHours(12),
        TimeSpan.FromHours(24),
    ];

    public static TimeSpan DelayFor(int attempt) => Backoff[Math.Clamp(attempt, 1, Backoff.Length) - 1];

    public async Task<int> RunAsync(CancellationToken ct)
    {
        var now = clock.GetUtcNow().UtcDateTime;

        var batch = await db.ContactSubmissions
            .Where(x => x.NotifyState == NotifyState.Pending && x.NotifyNextAttemptAt <= now)
            .OrderBy(x => x.NotifyNextAttemptAt).ThenBy(x => x.Id)
            .Take(notifyOptions.BatchSize)
            .ToListAsync(ct);

        if (batch.Count == 0)
        {
            return 0;
        }

        var sent = 0;

        foreach (var row in batch)
        {
            // break, not throw: the loop has to leave cleanly so the row it just
            // sent is already committed. Throwing here would skip the save.
            if (ct.IsCancellationRequested)
            {
                break;
            }

            try
            {
                // Not ct. A SIGTERM landing after the SMTP server has accepted
                // DATA would cancel the transaction we cannot un-send: the row
                // stays pending, the retry delivers a second copy, and the
                // attempt is burned against MaxAttempts. Cancellation is handled
                // between rows by the check above; MailKit's own 15 second
                // timeout bounds a single send, so the drain stays short.
                await mail.SendAsync(NotificationMessage.Build(row, contactOptions), CancellationToken.None);

                row.NotifyState = NotifyState.Sent;
                row.NotifiedAt = clock.GetUtcNow().UtcDateTime;
                row.NotifyNextAttemptAt = null;
                row.NotifyError = string.Empty;
                sent++;
            }
            catch (Exception ex)
            {
                var outcome = SmtpFailure.Classify(ex, row.NotifyAttempts);
                row.NotifyAttempts = (byte)(row.NotifyAttempts + 1);
                row.NotifyError = SmtpFailure.Describe(ex);

                if (outcome == SmtpOutcome.PermanentFailure || row.NotifyAttempts >= notifyOptions.MaxAttempts)
                {
                    row.NotifyState = NotifyState.Failed;
                    row.NotifyNextAttemptAt = null;
                }
                else
                {
                    var delay = DelayFor(row.NotifyAttempts);
                    var jitter = 1.0 + (Random.Shared.NextDouble() * 0.2);
                    row.NotifyNextAttemptAt = clock.GetUtcNow().UtcDateTime + (delay * jitter);
                }

                logger.LogWarning(
                    "notify.attempt_failed id={Id} attempt={Attempt} outcome={Outcome}",
                    row.Id.ToString(CultureInfo.InvariantCulture),
                    row.NotifyAttempts,
                    outcome);
            }

            // Per row, and uncancellable. Saving the whole batch at the end
            // would mean a shutdown, or one failed save, forgets every send
            // already made: those rows stay pending and the same notifications
            // go out again on the next sweep. This narrows that window to a
            // single message. Twenty single-row updates cost nothing here.
            await db.SaveChangesAsync(CancellationToken.None);
        }

        return sent;
    }

    public Task<int> CountFailedAsync(CancellationToken ct) =>
        db.ContactSubmissions.CountAsync(x => x.NotifyState == NotifyState.Failed, ct);
}
