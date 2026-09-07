using Kestridge.Api.Data;
using Kestridge.Api.Maintenance;
using Kestridge.Api.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;

namespace Kestridge.Api.Tests;

public class RetentionPurgeTests(MySqlFixture fixture) : DatabaseTestBase(fixture)
{
    private readonly FakeTimeProvider _clock = new(new DateTimeOffset(2026, 9, 7, 3, 30, 0, TimeSpan.Zero));

    private static RetentionOptions Options(int batchSize = 500) =>
        new() { Months = 24, RunHourUtc = 3, BatchSize = batchSize };

    private async Task SeedAsync(DateOnly purgeAfter, bool legalHold = false, int count = 1)
    {
        var now = _clock.GetUtcNow().UtcDateTime;

        await using var db = Db();
        for (var i = 0; i < count; i++)
        {
            db.ContactSubmissions.Add(new ContactSubmission
            {
                CreatedAt = now,
                Name = "Jane Doe",
                Email = $"jane{Guid.NewGuid():N}@company.com",
                Service = "ai",
                Message = "We need an intake process.",
                DedupeKey = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"),
                PurgeAfter = purgeAfter,
                LegalHold = legalHold,
                NotifyState = NotifyState.Sent,
                NotifyNextAttemptAt = null,
            });
        }

        await db.SaveChangesAsync();
    }

    private async Task<int> PurgeAsync(int batchSize = 500)
    {
        await using var db = Db();
        return await new RetentionPurge(db, Options(batchSize), _clock).RunAsync(CancellationToken.None);
    }

    private DateOnly Today => DateOnly.FromDateTime(_clock.GetUtcNow().UtcDateTime);

    [SkippableFact]
    public async Task RowWithPurgeAfterYesterday_IsDeleted()
    {
        RequireDatabase();

        await SeedAsync(Today.AddDays(-1));
        Assert.Equal(1, await PurgeAsync());

        await using var db = Db();
        Assert.Equal(0, await db.ContactSubmissions.CountAsync());
    }

    // The boundary is inclusive.
    [SkippableFact]
    public async Task RowWithPurgeAfterToday_IsDeleted()
    {
        RequireDatabase();

        await SeedAsync(Today);
        Assert.Equal(1, await PurgeAsync());
    }

    [SkippableFact]
    public async Task RowWithPurgeAfterTomorrow_Survives()
    {
        RequireDatabase();

        await SeedAsync(Today.AddDays(1));
        Assert.Equal(0, await PurgeAsync());

        await using var db = Db();
        Assert.Equal(1, await db.ContactSubmissions.CountAsync());
    }

    // Makes the policy phrase "plus any period required by law" operative.
    [SkippableFact]
    public async Task RowWithLegalHold_SurvivesRegardlessOfAge()
    {
        RequireDatabase();

        await SeedAsync(Today.AddYears(-5), legalHold: true);
        Assert.Equal(0, await PurgeAsync());

        await using var db = Db();
        Assert.Equal(1, await db.ContactSubmissions.CountAsync());
    }

    [SkippableFact]
    public async Task Purge_WritesJobRunRowWithCountAndCutoff()
    {
        RequireDatabase();

        await SeedAsync(Today.AddDays(-1), count: 3);
        await PurgeAsync();

        await using var db = Db();
        var run = await db.JobRuns.SingleAsync();

        Assert.Equal("retention_purge", run.JobName);
        Assert.Equal("ok", run.Outcome);
        Assert.Equal(3u, run.RowsAffected);
        Assert.Equal(Today, run.CutoffDate);
        Assert.NotNull(run.FinishedAt);
    }

    [SkippableFact]
    public async Task Purge_DoesNotRunTwiceInOneUtcDay()
    {
        RequireDatabase();

        await SeedAsync(Today.AddDays(-1));
        Assert.Equal(1, await PurgeAsync());

        await SeedAsync(Today.AddDays(-1));
        Assert.Equal(0, await PurgeAsync());

        await using var db = Db();
        Assert.Equal(1, await db.JobRuns.CountAsync());
        Assert.Equal(1, await db.ContactSubmissions.CountAsync());
    }

    [SkippableFact]
    public async Task Purge_RunsAgainOnTheNextUtcDay()
    {
        RequireDatabase();

        await SeedAsync(Today.AddDays(-1));
        await PurgeAsync();

        _clock.Advance(TimeSpan.FromDays(1));
        await SeedAsync(Today.AddDays(-1));
        Assert.Equal(1, await PurgeAsync());

        await using var db = Db();
        Assert.Equal(2, await db.JobRuns.CountAsync());
    }

    [SkippableFact]
    public async Task Purge_BatchesDeletesAtConfiguredSize()
    {
        RequireDatabase();

        await SeedAsync(Today.AddDays(-1), count: 12);
        Assert.Equal(12, await PurgeAsync(batchSize: 5));

        await using var db = Db();
        Assert.Equal(0, await db.ContactSubmissions.CountAsync());
        Assert.Equal(12u, (await db.JobRuns.SingleAsync()).RowsAffected);
    }

    // job_runs is the evidence that the deletion promise is kept, so it has to
    // outlive the data it deletes.
    [SkippableFact]
    public async Task JobRunsTable_IsNeverPurged()
    {
        RequireDatabase();

        await using (var db = Db())
        {
            db.JobRuns.Add(new JobRun
            {
                JobName = "retention_purge",
                StartedAt = _clock.GetUtcNow().UtcDateTime.AddYears(-5),
                Outcome = "ok",
                CutoffDate = Today.AddYears(-5),
            });
            await db.SaveChangesAsync();
        }

        await SeedAsync(Today.AddDays(-1));
        await PurgeAsync();

        await using var check = Db();
        Assert.Equal(2, await check.JobRuns.CountAsync());
    }
}
