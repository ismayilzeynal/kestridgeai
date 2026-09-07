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
