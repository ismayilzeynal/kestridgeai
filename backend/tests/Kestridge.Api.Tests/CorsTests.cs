using System.Net;

namespace Kestridge.Api.Tests;

// A missing Access-Control-Allow-Origin does not stop the POST, because it is a
// CORS simple request: the server stores and mails it, and only the response
// read is blocked. The visitor then sees "That did not go through" and
// resubmits. The symptom is duplicate mail plus a "broken form" report with no
// matching server error, so every one of these paths is asserted.
public class CorsTests
{
    private const string DeadConnection =
        "Server=127.0.0.1;Port=1;Database=kestridge;User ID=nobody;Password=nobody;"
        + "SslMode=None;AllowPublicKeyRetrieval=True;DateTimeKind=Utc;DefaultCommandTimeout=2;ConnectionTimeout=2";

    private static ApiFactory Factory(Dictionary<string, string?>? settings = null) =>
        new() { ConnectionString = DeadConnection, Settings = settings ?? [] };

    private static string? Acao(HttpResponseMessage response) =>
        response.Headers.TryGetValues("Access-Control-Allow-Origin", out var values)
            ? values.FirstOrDefault()
            : null;

    [Fact]
    public async Task Post_WithAllowedOrigin_EchoesAccessControlAllowOrigin()
    {
        using var factory = Factory();
        using var client = factory.CreateClient();

        using var response = await client.SendAsync(MultipartRequest.Contact());

        Assert.Equal(MultipartRequest.Origin, Acao(response));
        Assert.Contains("Origin", response.Headers.Vary, StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Post_NeverEmitsAccessControlAllowCredentials()
    {
        using var factory = Factory();
        using var client = factory.CreateClient();

        using var response = await client.SendAsync(MultipartRequest.Contact());

        Assert.False(response.Headers.Contains("Access-Control-Allow-Credentials"));
    }

    // The control that stops another site posting the form from inside a
    // visitor's browser. CORS alone would not: the request still arrives.
    [Fact]
    public async Task Post_WithDisallowedOrigin_Returns403()
    {
        using var factory = Factory();
        using var client = factory.CreateClient();

        using var response = await client.SendAsync(MultipartRequest.Contact(origin: "https://evil.com"));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(Bodies.Origin, await response.Content.ReadAsStringAsync());
        Assert.Empty(factory.Email.Sent);
    }

    [Fact]
    public async Task Post_WithVercelSuffixAttackOrigin_Returns403()
    {
        using var factory = Factory(new Dictionary<string, string?>
        {
            ["Kestridge:Cors:AllowVercelPreviews"] = "true",
        });
        using var client = factory.CreateClient();

        using var response = await client.SendAsync(
            MultipartRequest.Contact(origin: "https://kestridgeai.vercel.app.attacker.com"));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Post_WithNoOriginHeader_Returns403_WhenRequireOriginTrue()
    {
        using var factory = Factory();
        using var client = factory.CreateClient();

        using var response = await client.SendAsync(MultipartRequest.Contact(origin: null));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    // The escape hatch for a server-to-server proxy, which sends no Origin.
    [Fact]
    public async Task Post_WithNoOriginHeader_IsAccepted_WhenRequireOriginFalse()
    {
        using var factory = Factory(new Dictionary<string, string?>
        {
            ["Kestridge:Contact:RequireOrigin"] = "false",
        });
        using var client = factory.CreateClient();

        using var response = await client.SendAsync(MultipartRequest.Contact(origin: null));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AccessControlAllowOrigin_IsPresent_OnValidationFailure()
    {
        using var factory = Factory();
        using var client = factory.CreateClient();

        using var response = await client.SendAsync(MultipartRequest.Contact(service: "junk"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MultipartRequest.Origin, Acao(response));
    }

    // Proves UseCors runs before UseRateLimiter.
    [Fact]
    public async Task AccessControlAllowOrigin_IsPresent_OnRateLimitRejection()
    {
        using var factory = Factory(new Dictionary<string, string?>
        {
            ["Kestridge:RateLimit:PermitsPerWindow"] = "1",
            ["Kestridge:RateLimit:WindowMinutes"] = "10",
        });
        using var client = factory.CreateClient();

        using var first = await client.SendAsync(MultipartRequest.Contact());
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);

        using var second = await client.SendAsync(MultipartRequest.Contact(message: "A second message."));

        Assert.Equal(HttpStatusCode.TooManyRequests, second.StatusCode);
        Assert.Equal("600", second.Headers.GetValues("Retry-After").Single());
        Assert.Equal(MultipartRequest.Origin, Acao(second));
    }

    // UseExceptionHandler clears response headers before re-executing, which is
    // why CorsHeaderSafetyNet exists as the outermost middleware. The throwing
    // clock is used because every exception the handler itself raises is caught
    // and turned into a 503, which never reaches the exception handler at all.
    [Fact]
    public async Task UnhandledException_Returns500WithTheFixedBodyAndCorsHeader()
    {
        using var factory = new ApiFactory { ConnectionString = DeadConnection, ThrowFromClock = true };
        using var client = factory.CreateClient();

        using var response = await client.SendAsync(MultipartRequest.Contact());

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal("{\"ok\":false,\"error\":\"server\"}", await response.Content.ReadAsStringAsync());
        Assert.Equal(MultipartRequest.Origin, Acao(response));
    }

    [Fact]
    public async Task UnhandledException_LeaksNoStackTrace()
    {
        using var factory = new ApiFactory { ConnectionString = DeadConnection, ThrowFromClock = true };
        using var client = factory.CreateClient();

        using var response = await client.SendAsync(MultipartRequest.Contact());
        var body = await response.Content.ReadAsStringAsync();

        Assert.DoesNotContain("BadImageFormatException", body, StringComparison.Ordinal);
        Assert.DoesNotContain("at Kestridge", body, StringComparison.Ordinal);
    }
}
