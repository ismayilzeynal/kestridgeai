using System.Net;

namespace Kestridge.Api.Tests;

// Without a real remote address these tests are the only thing standing between
// the configured limiter and a constant partition key, which would let one
// visitor's five submissions lock out every other visitor on the internet.
public class RateLimitTests
{
    private const string DeadConnection =
        "Server=127.0.0.1;Port=1;Database=kestridge;User ID=nobody;Password=nobody;"
        + "SslMode=None;AllowPublicKeyRetrieval=True;DateTimeKind=Utc;DefaultCommandTimeout=2;ConnectionTimeout=2";

    private static ApiFactory Factory(int perClient, int global) =>
        new()
        {
            ConnectionString = DeadConnection,
            Settings = new Dictionary<string, string?>
            {
                ["Kestridge:RateLimit:PermitsPerWindow"] = perClient.ToString(System.Globalization.CultureInfo.InvariantCulture),
                ["Kestridge:RateLimit:WindowMinutes"] = "10",
                ["Kestridge:RateLimit:GlobalPerHour"] = global.ToString(System.Globalization.CultureInfo.InvariantCulture),
            },
        };

    private static readonly string[] PasswordPaths =
        ["/api/admin/login", "/api/admin/login/start", "/api/admin/login/enroll"];

    private static ApiFactory PasswordFactory(int login, int start, int enroll) =>
        new()
        {
            ConnectionString = DeadConnection,
            Settings = new Dictionary<string, string?>
            {
                ["Kestridge:RateLimit:LoginPermitsPerWindow"] = login.ToString(System.Globalization.CultureInfo.InvariantCulture),
                ["Kestridge:RateLimit:StartPermitsPerWindow"] = start.ToString(System.Globalization.CultureInfo.InvariantCulture),
                ["Kestridge:RateLimit:EnrollPermitsPerWindow"] = enroll.ToString(System.Globalization.CultureInfo.InvariantCulture),
                ["Kestridge:RateLimit:WindowMinutes"] = "10",
            },
        };

    // A foreign Origin makes each handler answer 403 before it touches the dead
    // database, so what is measured is only the limiter.
    private static async Task<HttpStatusCode> PostPasswordAsync(HttpClient client, string path, string clientAddress)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, path)
        {
            Content = System.Net.Http.Json.JsonContent.Create(new { username = "x", password = "y" }),
        };

        request.Headers.Add("Origin", "https://attacker.example");
        request.Headers.Add(ApiFactory.ClientAddressHeader, clientAddress);

        using var response = await client.SendAsync(request);
        return response.StatusCode;
    }

    [Fact]
    public async Task OneClientHittingItsLimit_DoesNotBlockAnotherClient()
    {
        using var factory = Factory(perClient: 1, global: 1000);
        using var client = factory.CreateClient();

        using var firstFromA = await client.SendAsync(
            MultipartRequest.Contact(message: "Message one.", clientAddress: "203.0.113.10"));
        Assert.Equal(HttpStatusCode.OK, firstFromA.StatusCode);

        using var secondFromA = await client.SendAsync(
            MultipartRequest.Contact(message: "Message two.", clientAddress: "203.0.113.10"));
        Assert.Equal(HttpStatusCode.TooManyRequests, secondFromA.StatusCode);

        // The bug this pins: with a constant partition key, or with the global
        // limiter charged before the per-client one, this is 429 as well.
        using var firstFromB = await client.SendAsync(
            MultipartRequest.Contact(message: "Message three.", clientAddress: "198.51.100.7"));
        Assert.Equal(HttpStatusCode.OK, firstFromB.StatusCode);
    }

    // A per-client rejection must not burn a global permit, or one bot draining
    // the hourly budget in a few seconds takes the form down for everyone until
    // the window rolls, with /api/health still green so nobody is paged.
    [Fact]
    public async Task RejectedRequests_DoNotConsumeTheGlobalBudget()
    {
        using var factory = Factory(perClient: 1, global: 3);
        using var client = factory.CreateClient();

        for (var i = 0; i < 10; i++)
        {
            using var flood = await client.SendAsync(
                MultipartRequest.Contact(message: $"Flood {i}.", clientAddress: "203.0.113.10"));

            Assert.Equal(
                i == 0 ? HttpStatusCode.OK : HttpStatusCode.TooManyRequests,
                flood.StatusCode);
        }

        using var victim = await client.SendAsync(
            MultipartRequest.Contact(message: "A real inquiry.", clientAddress: "198.51.100.7"));

        Assert.Equal(HttpStatusCode.OK, victim.StatusCode);
    }

    [Fact]
    public async Task GlobalLimiter_RejectsOnceItsBudgetIsGone()
    {
        using var factory = Factory(perClient: 1000, global: 2);
        using var client = factory.CreateClient();

        for (var i = 0; i < 2; i++)
        {
            using var allowed = await client.SendAsync(
                MultipartRequest.Contact(message: $"Message {i}.", clientAddress: $"203.0.113.{i + 1}"));
            Assert.Equal(HttpStatusCode.OK, allowed.StatusCode);
        }

        using var blocked = await client.SendAsync(
            MultipartRequest.Contact(message: "Message three.", clientAddress: "203.0.113.99"));

        Assert.Equal(HttpStatusCode.TooManyRequests, blocked.StatusCode);
    }

    // Health is what the uptime monitor polls. DisableRateLimiting on the
    // endpoint would not cover this: it suppresses endpoint policies only.
    [Fact]
    public async Task Health_StaysExempt_EvenWhenTheGlobalBudgetIsExhausted()
    {
        using var factory = Factory(perClient: 1, global: 1);
        using var client = factory.CreateClient();

        using var first = await client.SendAsync(
            MultipartRequest.Contact(clientAddress: "203.0.113.10"));
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);

        for (var i = 0; i < 5; i++)
        {
            using var health = await client.GetAsync("/api/health");
            Assert.Equal(HttpStatusCode.ServiceUnavailable, health.StatusCode);
        }
    }

    [Fact]
    public async Task IPv6ClientsInTheSameSlash64_ShareAPartition()
    {
        using var factory = Factory(perClient: 1, global: 1000);
        using var client = factory.CreateClient();

        using var first = await client.SendAsync(
            MultipartRequest.Contact(message: "Message one.", clientAddress: "2001:db8:1:2:aaaa::1"));
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);

        using var sameSlash64 = await client.SendAsync(
            MultipartRequest.Contact(message: "Message two.", clientAddress: "2001:db8:1:2:bbbb::9"));
        Assert.Equal(HttpStatusCode.TooManyRequests, sameSlash64.StatusCode);

        using var differentSlash64 = await client.SendAsync(
            MultipartRequest.Contact(message: "Message three.", clientAddress: "2001:db8:1:3::1"));
        Assert.Equal(HttpStatusCode.OK, differentSlash64.StatusCode);
    }

    // Continue (start), Finish setup (enroll) and the code step (login) each
    // spend a budget of their own. When start and enroll shared five, a few
    // ordinary sign-ins and one mistyped setup code answered 429 to everyone
    // behind the address. The limits differ so that each 429 also shows which
    // option its limiter reads.
    [Theory]
    [InlineData("/api/admin/login", 2)]
    [InlineData("/api/admin/login/start", 3)]
    [InlineData("/api/admin/login/enroll", 4)]
    public async Task PasswordEndpoints_RefuseAtTheirOwnLimit_WithoutSpendingEachOther(string exhausted, int limit)
    {
        using var factory = PasswordFactory(login: 2, start: 3, enroll: 4);
        using var client = factory.CreateClient();

        for (var i = 0; i < limit; i++)
        {
            Assert.Equal(HttpStatusCode.Forbidden, await PostPasswordAsync(client, exhausted, "203.0.113.20"));
        }

        Assert.Equal(HttpStatusCode.TooManyRequests, await PostPasswordAsync(client, exhausted, "203.0.113.20"));

        foreach (var other in PasswordPaths.Where(p => p != exhausted))
        {
            Assert.Equal(HttpStatusCode.Forbidden, await PostPasswordAsync(client, other, "203.0.113.20"));
        }
    }

    // appsettings.json does not set these, so the defaults are what production
    // runs. They are the budgets the owner's first sign-in has to fit in.
    [Fact]
    public async Task PasswordEndpoints_DefaultBudgets_AreFiveTenAndThirty()
    {
        using var factory = new ApiFactory { ConnectionString = DeadConnection };
        using var client = factory.CreateClient();

        foreach (var (path, limit) in new[] { ("/api/admin/login", 5), ("/api/admin/login/start", 10), ("/api/admin/login/enroll", 30) })
        {
            for (var i = 0; i < limit; i++)
            {
                Assert.Equal(HttpStatusCode.Forbidden, await PostPasswordAsync(client, path, "203.0.113.40"));
            }

            Assert.Equal(HttpStatusCode.TooManyRequests, await PostPasswordAsync(client, path, "203.0.113.40"));
        }
    }

    // Routing ignores case and one trailing slash, so the limiters have to as
    // well. A variant that reaches the handler without spending a permit from
    // the tight partition is a way round its budget. Each variant is sent first,
    // to a fresh factory with a budget of one: the 403 shows routing took it to
    // the handler, and the 429 on the plain path straight after, from the same
    // address, shows it spent the permit the plain path needed.
    [Theory]
    [InlineData("/api/admin/LOGIN/Start", "/api/admin/login/start")]
    [InlineData("/api/admin/login/start/", "/api/admin/login/start")]
    [InlineData("/api/admin/login/ENROLL", "/api/admin/login/enroll")]
    [InlineData("/api/admin/Login/Enroll/", "/api/admin/login/enroll")]
    [InlineData("/api/admin/LOGIN", "/api/admin/login")]
    [InlineData("/api/admin/login/", "/api/admin/login")]
    public async Task PasswordEndpointLimiters_MatchPathsTheWayRoutingDoes(string variant, string plain)
    {
        using var factory = PasswordFactory(login: 1, start: 1, enroll: 1);
        using var client = factory.CreateClient();

        Assert.Equal(HttpStatusCode.Forbidden, await PostPasswordAsync(client, variant, "203.0.113.30"));
        Assert.Equal(HttpStatusCode.TooManyRequests, await PostPasswordAsync(client, plain, "203.0.113.30"));
    }

    [Fact]
    public async Task RateLimitedResponse_CarriesRetryAfterAndCorsHeaders()
    {
        using var factory = Factory(perClient: 1, global: 1000);
        using var client = factory.CreateClient();

        await client.SendAsync(MultipartRequest.Contact(message: "Message one.", clientAddress: "203.0.113.10"));

        using var blocked = await client.SendAsync(
            MultipartRequest.Contact(message: "Message two.", clientAddress: "203.0.113.10"));

        Assert.Equal(HttpStatusCode.TooManyRequests, blocked.StatusCode);
        Assert.Equal("600", blocked.Headers.GetValues("Retry-After").Single());
        Assert.Equal(
            MultipartRequest.Origin,
            blocked.Headers.GetValues("Access-Control-Allow-Origin").Single());
    }
}
