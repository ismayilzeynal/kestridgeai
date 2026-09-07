using Kestridge.Api.Contact;

namespace Kestridge.Api.Tests;

// Nothing may log a submission field. A log line is a second copy of personal
// data, held outside the retention job's reach, in the exact field the privacy
// policy warns senders about.
public class LoggingHygieneTests(MySqlFixture fixture) : DatabaseTestBase(fixture)
{
    private const string DeadConnection =
        "Server=127.0.0.1;Port=1;Database=kestridge;User ID=nobody;Password=nobody;"
        + "SslMode=None;AllowPublicKeyRetrieval=True;DateTimeKind=Utc;DefaultCommandTimeout=2;ConnectionTimeout=2";

    private const string SentinelName = "ZZNAMEZZ";
    private const string SentinelEmail = "zzmailzz@example.test";
    private const string SentinelCompany = "ZZCOMPZZ";
    private const string SentinelPhone = "ZZPHONEZZ";
    private const string SentinelMessage = "ZZMSGZZ and more text to pass validation.";

    private static readonly string[] Sentinels =
        [SentinelName, SentinelEmail, SentinelCompany, SentinelPhone, "ZZMSGZZ"];

    private static HttpRequestMessage Sentinel(string service = "ai") => MultipartRequest.Contact(
        name: SentinelName,
        email: SentinelEmail,
        company: SentinelCompany,
        phone: SentinelPhone,
        service: service,
        message: SentinelMessage);

    private static void AssertClean(ApiFactory factory)
    {
        foreach (var line in factory.Logs.Lines)
        {
            foreach (var sentinel in Sentinels)
            {
                Assert.DoesNotContain(sentinel, line, StringComparison.OrdinalIgnoreCase);
            }
        }
    }

    [Fact]
    public async Task ValidationFailurePath_LogsNoFieldValues()
    {
        using var factory = new ApiFactory { ConnectionString = DeadConnection };
        using var client = factory.CreateClient();

        await client.SendAsync(Sentinel(service: "junk"));

        Assert.NotEmpty(factory.Logs.Lines);
        AssertClean(factory);
    }

    [Fact]
    public async Task DatabaseFailurePath_LogsNoFieldValues()
    {
        using var factory = new ApiFactory { ConnectionString = DeadConnection };
        factory.Email.FailWith = _ => new InvalidOperationException("smtp down");
        using var client = factory.CreateClient();

        await client.SendAsync(Sentinel());

        Assert.NotEmpty(factory.Logs.Lines);
        AssertClean(factory);
    }

    [Fact]
    public async Task HoneypotPath_LogsNoFieldValues()
    {
        using var factory = new ApiFactory { ConnectionString = DeadConnection };
        using var client = factory.CreateClient();

        await client.SendAsync(MultipartRequest.Contact(name: SentinelName, gotcha: "bot"));

        AssertClean(factory);
    }

    // With EnableSensitiveDataLogging on, EF logs parameter values and the
    // message body lands in the console and the Event Log.
    [SkippableFact]
    public async Task SuccessPath_LogsNoFieldValues_IncludingEfParameters()
    {
        RequireDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        await client.SendAsync(Sentinel());

        AssertClean(factory);
    }

    // Guards against an accidental log.LogInformation("{Input}", input).
    [Fact]
    public void ContactInputToString_IsRedacted()
    {
        var input = new ContactInput(
            SentinelName, SentinelEmail, SentinelCompany, SentinelPhone, "ai", SentinelMessage, string.Empty);

        Assert.Equal("ContactInput(redacted)", input.ToString());
    }
}
