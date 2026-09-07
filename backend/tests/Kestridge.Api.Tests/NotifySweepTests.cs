using Kestridge.Api.Data;
using Kestridge.Api.Email;
using Kestridge.Api.Maintenance;
using Kestridge.Api.Options;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;

namespace Kestridge.Api.Tests;

public class NotifySweepTests(MySqlFixture fixture) : DatabaseTestBase(fixture)
{
    private static readonly ContactOptions Contact = new()
    {
        AllowedServices = ["ai"],
        ToAddress = "info@kestridge.test",
        FromAddress = "no-reply@kestridge.test",
        FromDisplayName = "Kestridge Website",
    };

    private static readonly NotifyOptions Notify = new() { MaxAttempts = 7, BatchSize = 20, SweepSeconds = 30 };

    private readonly FakeTimeProvider _clock = new(new DateTimeOffset(2026, 9, 7, 12, 0, 0, TimeSpan.Zero));

    private readonly FakeEmailSender _mail = new();

    private async Task<long> SeedPendingAsync(string email = "jane@company.com")
    {
        var now = _clock.GetUtcNow().UtcDateTime;

        await using var db = Db();
        var row = new ContactSubmission
        {
            CreatedAt = now,
            Name = "Jane Doe",
            Email = email,
            Service = "ai",
            Message = "We need an intake process.",
            DedupeKey = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"),
            PurgeAfter = DateOnly.FromDateTime(now).AddMonths(24),
            NotifyNextAttemptAt = now,
        };

        db.ContactSubmissions.Add(row);
        await db.SaveChangesAsync();
        return row.Id;
    }

    private async Task<int> SweepAsync()
    {
        await using var db = Db();
        var sweep = new NotifySweep(db, _mail, Contact, Notify, _clock, NullLogger.Instance);
        return await sweep.RunAsync(CancellationToken.None);
    }

    private async Task<ContactSubmission> ReloadAsync(long id)
    {
        await using var db = Db();
        return await db.ContactSubmissions.AsNoTracking().SingleAsync(x => x.Id == id);
    }

    [SkippableFact]
    public async Task SuccessfulSend_SetsSentStateAndNotifiedAt()
    {
        RequireDatabase();

        var id = await SeedPendingAsync();
        Assert.Equal(1, await SweepAsync());

        var row = await ReloadAsync(id);

        Assert.Equal(NotifyState.Sent, row.NotifyState);
        Assert.Equal(_clock.GetUtcNow().UtcDateTime, row.NotifiedAt);
        Assert.Null(row.NotifyNextAttemptAt);
        Assert.Equal(string.Empty, row.NotifyError);
        Assert.Single(_mail.Sent);
    }

    [SkippableFact]
    public async Task TransientFailure_IncrementsAttemptsAndSchedulesNextAttempt()
    {
        RequireDatabase();

        _mail.FailWith = _ => new TimeoutException("smtp timed out");
        var id = await SeedPendingAsync();
        await SweepAsync();

        var row = await ReloadAsync(id);
        var now = _clock.GetUtcNow().UtcDateTime;

        Assert.Equal(NotifyState.Pending, row.NotifyState);
        Assert.Equal(1, row.NotifyAttempts);
        Assert.NotNull(row.NotifyNextAttemptAt);

        // One minute plus 0 to 20 percent jitter.
        var delay = row.NotifyNextAttemptAt!.Value - now;
        Assert.InRange(delay, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1.2));
    }

    [SkippableFact]
    public async Task SeventhTransientFailure_MarksFailed()
    {
        RequireDatabase();

        _mail.FailWith = _ => new TimeoutException("smtp timed out");
        var id = await SeedPendingAsync();

        for (var attempt = 1; attempt <= 7; attempt++)
        {
            await SweepAsync();
            _clock.Advance(TimeSpan.FromHours(48));
        }

        var row = await ReloadAsync(id);

        Assert.Equal(NotifyState.Failed, row.NotifyState);
        Assert.Equal(7, row.NotifyAttempts);
        Assert.Null(row.NotifyNextAttemptAt);
    }

    [SkippableFact]
    public async Task FailedRow_IsNotSelectedBySubsequentSweeps()
    {
        RequireDatabase();

        _mail.FailWith = _ => new SmtpCommandException(
            SmtpErrorCode.RecipientNotAccepted, SmtpStatusCode.MailboxUnavailable, "no such user");

        var id = await SeedPendingAsync();
        await SweepAsync();

        Assert.Equal(NotifyState.Failed, (await ReloadAsync(id)).NotifyState);

        _clock.Advance(TimeSpan.FromDays(7));
        Assert.Equal(0, await SweepAsync());
        Assert.Equal(1, (await ReloadAsync(id)).NotifyAttempts);
    }

    // kestridge.com has no MX record today, so a hard bounce is the most likely
    // first production failure. Retrying it 7 times only throttles the sender.
    [SkippableFact]
    public async Task PermanentSmtp550_MarksFailedAfterOneAttempt()
    {
        RequireDatabase();

        _mail.FailWith = _ => new SmtpCommandException(
            SmtpErrorCode.RecipientNotAccepted, SmtpStatusCode.MailboxUnavailable, "no such user");

        var id = await SeedPendingAsync();
        await SweepAsync();

        var row = await ReloadAsync(id);

        Assert.Equal(NotifyState.Failed, row.NotifyState);
        Assert.Equal(1, row.NotifyAttempts);
    }

    [SkippableFact]
    public async Task SuccessAfterOneFailure_SendsExactlyTwiceTotal()
    {
        RequireDatabase();

        var failures = 0;
        _mail.FailWith = _ => failures++ == 0 ? new TimeoutException("first attempt") : null;

        var id = await SeedPendingAsync();
        await SweepAsync();
        _clock.Advance(TimeSpan.FromMinutes(5));
        await SweepAsync();
        _clock.Advance(TimeSpan.FromMinutes(5));
        Assert.Equal(0, await SweepAsync());

        Assert.Equal(NotifyState.Sent, (await ReloadAsync(id)).NotifyState);
        Assert.Single(_mail.Sent);
        Assert.Equal(2, failures);
    }

    [SkippableFact]
    public async Task NotifyError_ContainsNoSubmissionFieldValues()
    {
        RequireDatabase();

        _mail.FailWith = _ => new TimeoutException("ZZSENTINELZZ is not a field value but the fields are not either");
        var id = await SeedPendingAsync("zzmailzz@example.test");
        await SweepAsync();

        var row = await ReloadAsync(id);

        Assert.DoesNotContain("zzmailzz", row.NotifyError, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Jane Doe", row.NotifyError, StringComparison.Ordinal);
        Assert.DoesNotContain("intake process", row.NotifyError, StringComparison.Ordinal);
        Assert.True(row.NotifyError.Length <= 300);
    }

    [SkippableFact]
    public async Task Sweep_HonoursBatchSize()
    {
        RequireDatabase();

        for (var i = 0; i < 25; i++)
        {
            await SeedPendingAsync($"person{i}@company.com");
        }

        Assert.Equal(20, await SweepAsync());
        Assert.Equal(5, await SweepAsync());
    }

    [SkippableFact]
    public async Task Sweep_DoesNotSelectRowsWithFutureNextAttemptAt()
    {
        RequireDatabase();

        var id = await SeedPendingAsync();

        await using (var db = Db())
        {
            var row = await db.ContactSubmissions.SingleAsync(x => x.Id == id);
            row.NotifyNextAttemptAt = _clock.GetUtcNow().UtcDateTime.AddHours(1);
            await db.SaveChangesAsync();
        }

        Assert.Equal(0, await SweepAsync());
        Assert.Empty(_mail.Sent);
    }
}
