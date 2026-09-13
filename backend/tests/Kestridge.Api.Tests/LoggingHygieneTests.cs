using System.Globalization;
using System.Net.Http.Json;
using Kestridge.Api.Admin;
using Kestridge.Api.Contact;
using Kestridge.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySqlConnector;

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

        // With EF's own error lines off, this line is the only record of what
        // the database said when the mail fails too. The pattern is the whole
        // line, so it proves the line carries the type and the error number
        // and nothing else: no exception, and no server message that could
        // quote a field. 1042 is MySqlConnector's number for a host it cannot
        // reach, which is what DeadConnection is.
        var stored = Assert.Single(factory.Logs.Lines, l => l.Contains("contact.store_error", StringComparison.Ordinal));
        Assert.Matches(
            @"^Kestridge\.Api\.Contact Error contact\.store_error type=[A-Za-z]+ number=1042 code=UnableToConnectToHost $",
            stored);
        Assert.Contains(factory.Logs.Lines, l => l.Contains("contact.store_failed", StringComparison.Ordinal));
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

    // A username, a display name and two passwords, all sentinels, driven
    // through every users and first sign-in path. The secret and both tokens are
    // random, so they are checked by value after the fact.
    [SkippableFact]
    public async Task AdminUserPaths_LogNoUsernameDisplayNamePasswordSecretOrToken()
    {
        RequireDatabase();

        const string username = "zzuserzz";
        const string displayName = "ZZDISPLAYZZ";
        const string initialPassword = "ZZINITIALPASSZZ";
        const string chosenPassword = "ZZCHOSENPASSZZ";

        using var factory = CreateFactory(AdminTestAccounts.FactorySettings());
        var op = await AdminTestAccounts.CreateAsync(Db, factory, "logtester", OperatorName);
        using var client = await AdminTestAccounts.SignedInAsync(Db, factory, op);
        using var anonymous = factory.CreateClient();

        var created = await client.PostAsJsonAsync("/api/admin/users/create", new
        {
            username,
            displayName,
            password = initialPassword,
            code = AdminTestAccounts.Code(factory, op.Secret),
        });
        Assert.Equal(System.Net.HttpStatusCode.OK, created.StatusCode);

        await client.GetAsync("/api/admin/users");

        // A wrong password, then the right one, which returns the setup data.
        await anonymous.PostAsJsonAsync("/api/admin/login/start", new { username, password = chosenPassword });
        var started = await anonymous.PostAsJsonAsync("/api/admin/login/start", new { username, password = initialPassword });
        var setup = System.Text.Json.JsonDocument.Parse(await started.Content.ReadAsStringAsync()).RootElement;
        var setupToken = setup.GetProperty("token").GetString()!;
        var secret = setup.GetProperty("secret").GetString()!;

        // A refused password, then success.
        await anonymous.PostAsJsonAsync("/api/admin/login/enroll", new
        {
            token = setupToken,
            code = AdminTestAccounts.Code(factory, secret),
            newPassword = initialPassword,
        });

        var enrolled = await anonymous.PostAsJsonAsync("/api/admin/login/enroll", new
        {
            token = setupToken,
            code = AdminTestAccounts.Code(factory, secret),
            newPassword = chosenPassword,
        });
        Assert.Equal(System.Net.HttpStatusCode.OK, enrolled.StatusCode);

        var sessionToken = System.Text.Json.JsonDocument.Parse(await enrolled.Content.ReadAsStringAsync())
            .RootElement.GetProperty("token").GetString()!;

        long id;
        await using (var db = Db())
        {
            id = db.AdminAccounts.Single(x => x.Username == username).Id;
        }

        await anonymous.PostAsJsonAsync("/api/admin/login", new
        {
            username,
            password = chosenPassword,
            code = AdminTestAccounts.Code(factory, secret, 1),
        });

        factory.Clock.Advance(TimeSpan.FromSeconds(Totp.StepSeconds));

        await client.PostAsJsonAsync(
            "/api/admin/users/" + id.ToString(CultureInfo.InvariantCulture) + "/reset-authenticator",
            new { code = AdminTestAccounts.Code(factory, op.Secret) });
        await client.PostAsync("/api/admin/users/" + id.ToString(CultureInfo.InvariantCulture) + "/disable", null);

        factory.Clock.Advance(TimeSpan.FromSeconds(Totp.StepSeconds));

        await client.PostAsJsonAsync("/api/admin/users/create", new
        {
            username = username + "2",
            displayName,
            password = initialPassword,
            code = AdminTestAccounts.Code(factory, op.Secret),
        });

        await using (var db = Db())
        {
            var pending = db.AdminEnrollments.Single(x => x.Username == username + "2").Id;
            await client.PostAsync(
                "/api/admin/users/pending/" + pending.ToString(CultureInfo.InvariantCulture) + "/delete", null);
        }

        Assert.NotEmpty(factory.Logs.Lines);

        string[] forbidden = [username, displayName, initialPassword, chosenPassword, OperatorName, secret, setupToken, sessionToken];
        foreach (var line in factory.Logs.Lines)
        {
            foreach (var value in forbidden)
            {
                Assert.DoesNotContain(value, line, StringComparison.OrdinalIgnoreCase);
            }
        }
    }

    // EF logs a failed save and a failed command at Error with the server's
    // message, before any catch in the application runs, and for a unique
    // username index that message is "Duplicate entry '<username>'". Program.cs
    // turns both events off. Forced through the application's own registered
    // DbContext, so what is tested is the options the app runs with, not a copy.
    [SkippableFact]
    public async Task DuplicateUsername_IsNotLoggedByEntityFramework()
    {
        RequireDatabase();

        const string username = "zzdupuserzz";

        using var factory = CreateFactory();
        var now = factory.Clock.GetUtcNow().UtcDateTime;

        AdminEnrollment Invitation() => new()
        {
            Username = username,
            DisplayName = "Someone",
            CreatedAt = now,
            ExpiresAt = now.AddHours(72),
        };

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<KestridgeDbContext>();

            db.AdminEnrollments.Add(Invitation());
            await db.SaveChangesAsync();

            db.AdminEnrollments.Add(Invitation());
            var ex = await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
            var server = Assert.IsType<MySqlException>(ex.InnerException);
            Assert.Equal(1062, server.Number);

            // The value this guards against: the server's message names it.
            Assert.Contains(username, server.Message, StringComparison.Ordinal);

            // The configured levels let nothing else from EF through here, so an
            // empty capture would prove nothing. Show it would have seen an
            // Error in both of EF's categories. The silence asserted next is
            // then the suppression, not a filter.
            var loggers = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            loggers.CreateLogger("Microsoft.EntityFrameworkCore.Update").LogError("zzcapturecheckzz update");
            loggers.CreateLogger("Microsoft.EntityFrameworkCore.Database.Command").LogError("zzcapturecheckzz command");
        }

        Assert.Equal(2, factory.Logs.Lines.Count(l => l.Contains("zzcapturecheckzz", StringComparison.Ordinal)));
        Assert.DoesNotContain(factory.Logs.Lines, l => l.Contains(username, StringComparison.OrdinalIgnoreCase));
    }

    // The other half of turning those events off: a database error that nothing
    // catches still reaches UseExceptionHandler, which logs it.
    [Fact]
    public async Task UnhandledDatabaseError_IsStillLoggedByTheExceptionHandler()
    {
        using var factory = new ApiFactory { ConnectionString = DeadConnection };
        using var client = factory.CreateClient();

        using var response = await client.PostAsJsonAsync(
            "/api/admin/login", new { username = "emil", password = "x", code = "123456" });

        Assert.Equal(System.Net.HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Contains(factory.Logs.Lines, l =>
            l.StartsWith("Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware Error", StringComparison.Ordinal)
            && l.Contains("MySqlException", StringComparison.Ordinal));
    }

    private Task<HttpClient> SignedInAsync(ApiFactory factory)
        => AdminTestAccounts.SignedInAsync(Db, factory, "logtester", OperatorName);

    // Guards against an accidental log.LogInformation("{Input}", input).
    [Fact]
    public void ContactInputToString_IsRedacted()
    {
        var input = new ContactInput(
            SentinelName, SentinelEmail, SentinelCompany, SentinelPhone, "ai", SentinelMessage, string.Empty);

        Assert.Equal("ContactInput(redacted)", input.ToString());
    }
}
