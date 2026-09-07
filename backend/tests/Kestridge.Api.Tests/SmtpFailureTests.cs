using System.Net.Sockets;
using Kestridge.Api.Email;
using Kestridge.Api.Maintenance;
using MailKit.Net.Smtp;

namespace Kestridge.Api.Tests;

public class SmtpFailureTests
{
    [Fact]
    public void Smtp550_IsPermanent() =>
        Assert.Equal(
            SmtpOutcome.PermanentFailure,
            SmtpFailure.Classify(new SmtpCommandException(SmtpErrorCode.RecipientNotAccepted, SmtpStatusCode.MailboxUnavailable, "no such user"), 0));

    [Fact]
    public void Smtp451_IsTransient() =>
        Assert.Equal(
            SmtpOutcome.TransientFailure,
            SmtpFailure.Classify(new SmtpCommandException(SmtpErrorCode.MessageNotAccepted, (SmtpStatusCode)451, "try later"), 0));

    [Fact]
    public void SocketException_IsTransient() =>
        Assert.Equal(SmtpOutcome.TransientFailure, SmtpFailure.Classify(new SocketException(), 0));

    [Fact]
    public void TimeoutException_IsTransient() =>
        Assert.Equal(SmtpOutcome.TransientFailure, SmtpFailure.Classify(new TimeoutException(), 0));

    [Fact]
    public void AuthenticationException_RetriesThenBecomesPermanent()
    {
        var ex = new MailKit.Security.AuthenticationException("bad password");

        Assert.Equal(SmtpOutcome.TransientFailure, SmtpFailure.Classify(ex, 0));
        Assert.Equal(SmtpOutcome.TransientFailure, SmtpFailure.Classify(ex, 2));
        Assert.Equal(SmtpOutcome.PermanentFailure, SmtpFailure.Classify(ex, 3));
    }

    [Fact]
    public void Describe_NeverExceeds300Chars()
    {
        var text = SmtpFailure.Describe(new TimeoutException(new string('x', 5000)));
        Assert.True(text.Length <= 300);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 5)]
    [InlineData(3, 15)]
    public void BackoffTable_MinutesForEarlyAttempts(int attempt, int minutes) =>
        Assert.Equal(TimeSpan.FromMinutes(minutes), NotifySweep.DelayFor(attempt));

    [Theory]
    [InlineData(4, 1)]
    [InlineData(5, 4)]
    [InlineData(6, 12)]
    [InlineData(7, 24)]
    public void BackoffTable_HoursForLaterAttempts(int attempt, int hours) =>
        Assert.Equal(TimeSpan.FromHours(hours), NotifySweep.DelayFor(attempt));

    // 1m + 5m + 15m + 1h + 4h + 12h + 24h is about 41.3 hours, which survives an
    // overnight or weekend provider outage.
    [Fact]
    public void BackoffTable_TotalCoverageExceeds40Hours()
    {
        var total = TimeSpan.Zero;
        for (var attempt = 1; attempt <= 7; attempt++)
        {
            total += NotifySweep.DelayFor(attempt);
        }

        Assert.True(total > TimeSpan.FromHours(40), $"total was {total}");
    }
}
