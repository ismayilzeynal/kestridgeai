using System.Globalization;
using Kestridge.Api.Data;
using Kestridge.Api.Email;
using Kestridge.Api.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kestridge.Api.Maintenance;

// The only BackgroundService. Drives the notify sweep every tick and the
// retention purge at most once per UTC day.
public sealed class MaintenanceService(
    IServiceScopeFactory scopeFactory,
    IOptions<NotifyOptions> notifyOptions,
    IOptions<ContactOptions> contactOptions,
    IOptions<RetentionOptions> retentionOptions,
    TimeProvider clock,
    ILogger<MaintenanceService> logger) : BackgroundService
{
    private static readonly EventId DeadLetter = new(5001, "notify.dead_letter");
    private static readonly TimeSpan PurgeRetryInterval = TimeSpan.FromMinutes(30);

    private DateTime _lastDeadLetterAlertUtc = DateTime.MinValue;
    private DateTime _lastPurgeAttemptUtc = DateTime.MinValue;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var period = TimeSpan.FromSeconds(notifyOptions.Value.SweepSeconds);
        using var timer = new PeriodicTimer(period, clock);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await timer.WaitForNextTickAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            await TickAsync(stoppingToken);
        }
    }

    public async Task TickAsync(CancellationToken ct)
    {
        await RunNotifySweepAsync(ct);
        await RunRetentionPurgeAsync(ct);
    }

    private async Task RunNotifySweepAsync(CancellationToken ct)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<KestridgeDbContext>();
            var mail = scope.ServiceProvider.GetRequiredService<IEmailSender>();

            var sweep = new NotifySweep(db, mail, contactOptions.Value, notifyOptions.Value, clock, logger);
            await sweep.RunAsync(ct);

            var failed = await sweep.CountFailedAsync(ct);
            var now = clock.GetUtcNow().UtcDateTime;

            if (failed > 0 && now - _lastDeadLetterAlertUtc >= TimeSpan.FromMinutes(60))
            {
                _lastDeadLetterAlertUtc = now;
                logger.LogError(DeadLetter, "notify.dead_letter count={Count}", failed.ToString(CultureInfo.InvariantCulture));
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "notify.sweep_failed");
        }
    }

    private async Task RunRetentionPurgeAsync(CancellationToken ct)
    {
        var now = clock.GetUtcNow().UtcDateTime;

        if (now.Hour < retentionOptions.Value.RunHourUtc)
        {
            return;
        }

        // The per-day guard inside RetentionPurge only suppresses a run that
        // succeeded. Without this throttle a failing purge re-runs on every
        // 30-second tick until midnight: about 2500 attempts, each committing a
        // job_runs row into the one table that is never purged, and each
        // re-issuing the statement that just failed.
        if (now - _lastPurgeAttemptUtc < PurgeRetryInterval)
        {
            return;
        }

        _lastPurgeAttemptUtc = now;

        try
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<KestridgeDbContext>();

            var purge = new RetentionPurge(db, retentionOptions.Value, clock);
            var removed = await purge.RunAsync(ct);

            if (removed > 0)
            {
                logger.LogInformation("retention.purged count={Count}", removed.ToString(CultureInfo.InvariantCulture));
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "retention.purge_failed");
        }
    }
}
