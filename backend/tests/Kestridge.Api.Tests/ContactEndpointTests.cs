using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Kestridge.Api.Tests;

// These need no database. The endpoint answers every one of them before it
// touches MySQL, or through the inline fallback when MySQL is unreachable.
// The dead connection string is deliberate: it is also the MySQL-down case.
public class ContactEndpointTests
{
    private const string DeadConnection =
        "Server=127.0.0.1;Port=1;Database=kestridge;User ID=nobody;Password=nobody;"
        + "SslMode=None;AllowPublicKeyRetrieval=True;DateTimeKind=Utc;DefaultCommandTimeout=2;ConnectionTimeout=2";

    private static ApiFactory Factory(Dictionary<string, string?>? settings = null) =>
        new() { ConnectionString = DeadConnection, Settings = settings ?? [] };

    // The canonical wire contract. This one test covers the two failure modes
    // that would otherwise break every submission in production: the minimal-API
    // JSON inference (415) and the antiforgery metadata that a form-bound
    // parameter attaches (400 with an empty body).
    [Fact]
    public async Task Post_WithAllSevenPartsInDomOrder_ReachesTheHandler()
    {
        using var factory = Factory();
        using var client = factory.CreateClient();

        using var response = await client.SendAsync(MultipartRequest.Contact());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(Bodies.Ok, await response.Content.ReadAsStringAsync());
        Assert.Single(factory.Email.Sent);
    }

    [Fact]
    public async Task Post_WithOptionalPartsAbsentEntirely_IsAccepted()
    {
        using var factory = Factory();
        using var client = factory.CreateClient();

        var content = new MultipartFormDataContent("----WebKitFormBoundaryTest");
        content.Add(new StringContent("Jane Doe", Encoding.UTF8), "name");
        content.Add(new StringContent("jane@company.com", Encoding.UTF8), "email");
        content.Add(new StringContent("ai", Encoding.UTF8), "service");
        content.Add(new StringContent("We need an intake process.", Encoding.UTF8), "message");

        using var response = await client.SendAsync(MultipartRequest.Build(content, MultipartRequest.Origin));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Post_WithUnknownExtraPart_IsAccepted()
    {
        using var factory = Factory();
        using var client = factory.CreateClient();

        var extra = new[] { new KeyValuePair<string, string>("utm_source", "linkedin") };
        using var response = await client.SendAsync(MultipartRequest.Contact(extra: extra));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Post_SetsNoCookieHeader()
    {
        using var factory = Factory();
        using var client = factory.CreateClient();

        using var response = await client.SendAsync(MultipartRequest.Contact());

        Assert.False(response.Headers.Contains("Set-Cookie"));
    }

    [Fact]
    public async Task Post_ResponseHasCacheControlNoStore()
    {
        using var factory = Factory();
        using var client = factory.CreateClient();

        using var response = await client.SendAsync(MultipartRequest.Contact());

        Assert.True(response.Headers.CacheControl?.NoStore);
    }

    // fetch follows redirects, and a 3xx converts the POST to a GET and drops
    // the body, so a redirect silently loses the message.
    [Theory]
    [InlineData("ai", "", "success")]
    [InlineData("ai", "bot", "honeypot")]
    [InlineData("junk", "", "validation failure")]
    public async Task Post_NeverReturnsRedirect(string service, string gotcha, string path)
    {
        using var factory = Factory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        using var response = await client.SendAsync(MultipartRequest.Contact(service: service, gotcha: gotcha));

        var code = (int)response.StatusCode;
        Assert.False(code is >= 300 and < 400, path + " returned " + code);
    }

    [Fact]
    public async Task Post_WithApplicationJsonBody_Returns415()
    {
        using var factory = Factory();
        using var client = factory.CreateClient();

        var content = new StringContent(Bodies.SomeJson, Encoding.UTF8, "application/json");
        using var response = await client.SendAsync(MultipartRequest.Build(content, MultipartRequest.Origin));

        Assert.Equal(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
    }

    // The browser never sends this shape, but urlencoded is still a form, so it
    // must not fall into the JSON path.
    [Fact]
    public async Task Post_WithUrlEncodedBody_IsNotRejectedAsJson()
    {
        using var factory = Factory();
        using var client = factory.CreateClient();

        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["name"] = "Jane Doe",
            ["email"] = "jane@company.com",
            ["service"] = "ai",
            ["message"] = "We need an intake process.",
        });

        using var response = await client.SendAsync(MultipartRequest.Build(content, MultipartRequest.Origin));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Post_WithFilePart_Returns400()
    {
        using var factory = Factory();
        using var client = factory.CreateClient();

        var content = new MultipartFormDataContent("----WebKitFormBoundaryTest");
        content.Add(new StringContent("Jane Doe", Encoding.UTF8), "name");
        content.Add(new StringContent("jane@company.com", Encoding.UTF8), "email");
        content.Add(new StringContent("ai", Encoding.UTF8), "service");
        content.Add(new StringContent("We need an intake process.", Encoding.UTF8), "message");

        var file = new ByteArrayContent([1, 2, 3]);
        file.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        content.Add(file, "attachment", "payload.bin");

        using var response = await client.SendAsync(MultipartRequest.Build(content, MultipartRequest.Origin));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // The point is that a limit breach never surfaces as a 500 with no log line.
    [Fact]
    public async Task Post_WithOversizedBody_IsRejectedWithoutA500()
    {
        using var factory = Factory();
        using var client = factory.CreateClient();

        using var response = await client.SendAsync(
            MultipartRequest.Contact(message: new string('x', 200 * 1024)));

        var code = (int)response.StatusCode;
        Assert.True(code is 413 or 400, "got " + code);
    }

    [Fact]
    public async Task Post_WithHoneypotFilled_Returns200AndSendsNothing()
    {
        using var factory = Factory();
        using var client = factory.CreateClient();

        using var response = await client.SendAsync(MultipartRequest.Contact(gotcha: "bot"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Empty(factory.Email.Sent);
    }

    // The honeypot check runs before validation, so a bot filling every field
    // with garbage cannot tell which field is the trap.
    [Fact]
    public async Task Post_WithHoneypotFilledAndInvalidFields_Returns200()
    {
        using var factory = Factory();
        using var client = factory.CreateClient();

        using var response = await client.SendAsync(
            MultipartRequest.Contact(service: "junk", message: "hi", gotcha: "bot"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Empty(factory.Email.Sent);
    }

    [Theory]
    [InlineData("", "jane@company.com", "ai", "A real message.", "name")]
    [InlineData("Jane", "nope", "ai", "A real message.", "email")]
    [InlineData("Jane", "jane@company.com", "junk", "A real message.", "service")]
    [InlineData("Jane", "jane@company.com", "ai", "", "message")]
    public async Task Post_WithInvalidField_Returns400AndNamesTheField(
        string name, string email, string service, string message, string field)
    {
        using var factory = Factory();
        using var client = factory.CreateClient();

        using var response = await client.SendAsync(
            MultipartRequest.Contact(name: name, email: email, service: service, message: message));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(Bodies.Invalid(field), await response.Content.ReadAsStringAsync());
        Assert.Empty(factory.Email.Sent);
    }

    [Fact]
    public async Task Post_WhenDatabaseUnreachableAndMailSucceeds_Returns200()
    {
        using var factory = Factory();
        using var client = factory.CreateClient();

        using var response = await client.SendAsync(MultipartRequest.Contact());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Single(factory.Email.Sent);
    }

    [Fact]
    public async Task Post_WhenDatabaseUnreachableAndMailFails_Returns503()
    {
        using var factory = Factory();
        factory.Email.FailWith = _ => new InvalidOperationException("smtp down");
        using var client = factory.CreateClient();

        using var response = await client.SendAsync(MultipartRequest.Contact());

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.Equal(Bodies.Unavailable, await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Get_Health_WhenDatabaseDown_Returns503Degraded()
    {
        using var factory = Factory();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/api/health");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.Equal(Bodies.Degraded, body);
        Assert.DoesNotContain("nobody", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("127.0.0.1", body, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Get_Health_IsNotRateLimited()
    {
        using var factory = Factory(new Dictionary<string, string?>
        {
            ["Kestridge:RateLimit:PermitsPerWindow"] = "1",
            ["Kestridge:RateLimit:GlobalPerHour"] = "1",
        });
        using var client = factory.CreateClient();

        for (var i = 0; i < 20; i++)
        {
            using var response = await client.GetAsync("/api/health");
            Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        }
    }
}
