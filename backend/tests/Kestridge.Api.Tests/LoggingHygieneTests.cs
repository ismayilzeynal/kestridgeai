using System.Globalization;
using System.Net.Http.Json;
using Kestridge.Api.Admin;
using Kestridge.Api.Contact;
using Kestridge.Api.Data;
using Kestridge.Api.Options;

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

    // The sentinel sweep used to cover the contact paths only, so the admin
    // surface was not covered by anything: the first
    // log.LogInformation("admin.viewed {Email}", row.Email) would have walked
    // straight through. The panel reads every submission the company has ever
    // received, which makes it the largest single opportunity to write personal
    // data into journald.
    //
    // Every admin log line is expected to carry a numeric id and a flag and
    // nothing else. This drives the endpoints that touch a real submission and
    // proves it.
    [SkippableFact]
    public async Task AdminPaths_LogNoFieldValues()
    {
        RequireDatabase();

        using var factory = CreateFactory();
        using var anonymous = factory.CreateClient();

        // A real submission, stored through the public endpoint, so the row the
        // panel then reads is the row a visitor would have created.
        await anonymous.SendAsync(Sentinel());

        long id;
        await using (var db = Db())
        {
            id = db.ContactSubmissions.OrderByDescending(x => x.Id).First().Id;
        }

        using var client = await SignedInAsync(factory);

        await client.GetAsync("/api/admin/submissions?state=all");
        await client.GetAsync("/api/admin/submissions/" + id.ToString(CultureInfo.InvariantCulture));
        await client.PostAsJsonAsync("/api/admin/submissions/search", new { q = SentinelEmail, state = "all" });
        await client.PostAsJsonAsync(
            "/api/admin/submissions/" + id.ToString(CultureInfo.InvariantCulture) + "/handled",
            new { handled = true });
        await client.PostAsJsonAsync(
            "/api/admin/submissions/" + id.ToString(CultureInfo.InvariantCulture) + "/legal-hold",
            new { legalHold = true });
        await client.PostAsJsonAsync("/api/admin/submissions/export", new { state = "all", includeMessage = true });
        await client.PostAsJsonAsync("/api/admin/dsr/preview", new { email = SentinelEmail });
        await client.GetAsync("/api/admin/content");

        Assert.NotEmpty(factory.Logs.Lines);
        AssertClean(factory);
    }

    // The display name is not a submission field, but it is a person's name
    // that the panel stamps onto every row it touches, and it has exactly the
    // same reason to stay out of the log as any other name.
    [SkippableFact]
    public async Task AdminPaths_LogNoOperatorName()
    {
        RequireDatabase();

        using var factory = CreateFactory();
        using var client = await SignedInAsync(factory);

        await client.GetAsync("/api/admin/session");
        await client.GetAsync("/api/admin/content");
        await client.PostAsJsonAsync("/api/admin/content/faq/save",
            new { id = (long?)null, question = "A question", answer = "An answer." });

        foreach (var line in factory.Logs.Lines)
        {
            Assert.DoesNotContain(OperatorName, line, StringComparison.OrdinalIgnoreCase);
        }
    }

    private const string OperatorName = "ZZOPERATORZZ";

    private async Task<HttpClient> SignedInAsync(ApiFactory factory)
    {
        var now = factory.Clock.GetUtcNow().UtcDateTime;
        var token = AdminSessions.NewToken();

        await using (var db = Db())
        {
            var account = new AdminAccount
            {
                Username = "logtester",
                DisplayName = OperatorName,
                PasswordHash = "not-used-on-this-path",
                TotpSecret = Totp.NewSecret(),
                CreatedAt = now,
            };

            db.AdminAccounts.Add(account);
            await db.SaveChangesAsync();

            db.AdminSessions.Add(AdminSessions.Issue(account.Id, AdminSessions.Hash(token), now, new AdminOptions()));
            await db.SaveChangesAsync();
        }

        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
        return client;
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
