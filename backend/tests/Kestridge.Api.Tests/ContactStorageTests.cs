using System.Net;
using Kestridge.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Kestridge.Api.Tests;

public class ContactStorageTests(MySqlFixture fixture) : DatabaseTestBase(fixture)
{
    [SkippableFact]
    public async Task Post_StoresOneRowWithEveryFieldByteEqual()
    {
        RequireDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        using var response = await client.SendAsync(MultipartRequest.Contact(
            name: "Jane Doe",
            email: "jane@company.com",
            company: "Acme Ltd",
            phone: "(555) 000-0000",
            service: "analytics",
            message: "We need weekly reporting from the order system."));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(Bodies.Ok, await response.Content.ReadAsStringAsync());

        await using var db = Db();
        var row = await db.ContactSubmissions.SingleAsync();

        Assert.Equal("Jane Doe", row.Name);
        Assert.Equal("jane@company.com", row.Email);
        Assert.Equal("Acme Ltd", row.Company);
        Assert.Equal("(555) 000-0000", row.Phone);
        Assert.Equal("analytics", row.Service);
        Assert.Equal("We need weekly reporting from the order system.", row.Message);
    }

    // Empty parts mean "not provided". Modelling that as NULL would invent a
    // distinction the wire protocol does not carry.
    [SkippableFact]
    public async Task Post_StoresEmptyStringsNotNullsForOptionalFields()
    {
        RequireDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        using var response = await client.SendAsync(MultipartRequest.Contact());
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await using var db = Db();
        var row = await db.ContactSubmissions.SingleAsync();

        Assert.Equal(string.Empty, row.Company);
        Assert.Equal(string.Empty, row.Phone);
    }

    [SkippableFact]
    public async Task Post_StoresEmailLowercased()
    {
        RequireDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        await client.SendAsync(MultipartRequest.Contact(email: "Ali@Example.COM"));

        await using var db = Db();
        var row = await db.ContactSubmissions.SingleAsync();

        Assert.Equal("ali@example.com", row.Email);
    }

    // The site publishes Azerbaijani copy and takes free-text messages. A column
    // that is not really utf8mb4 fails here and nowhere else.
    [SkippableFact]
    public async Task Post_AzerbaijaniAndEmojiRoundTripByteEqual()
    {
        RequireDatabase();

        const string Text = "Sifarislerin qeydiyyati ucun sistem lazimdir. Azerbaijani: eogisculdGI. Emoji: \U0001F680";

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        await client.SendAsync(MultipartRequest.Contact(name: "Emil Huseynov", message: Text));

        await using var db = Db();
        var row = await db.ContactSubmissions.SingleAsync();

        Assert.Equal(Text, row.Message);
        Assert.Equal("Emil Huseynov", row.Name);
    }

    [SkippableFact]
    public async Task Post_SetsPurgeAfterToCreatedAtPlusRetentionMonths()
    {
        RequireDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        await client.SendAsync(MultipartRequest.Contact());

        await using var db = Db();
        var row = await db.ContactSubmissions.SingleAsync();

        Assert.Equal(DateOnly.FromDateTime(row.CreatedAt).AddMonths(24), row.PurgeAfter);
    }

    [SkippableFact]
    public async Task Post_SetsNotifyStatePendingAndNextAttemptAtCreatedAt()
    {
        RequireDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        await client.SendAsync(MultipartRequest.Contact());

        await using var db = Db();
        var row = await db.ContactSubmissions.SingleAsync();

        Assert.Equal(NotifyState.Pending, row.NotifyState);
        Assert.Equal(0, row.NotifyAttempts);
        Assert.Equal(row.CreatedAt, row.NotifyNextAttemptAt);
        Assert.Null(row.NotifiedAt);
        Assert.Equal(string.Empty, row.NotifyError);
    }

    // The happy path does not send inline: the client has no timeout, so an SMTP
    // handshake in the request path would lock the submit button on "Sending".
    [SkippableFact]
    public async Task Post_DoesNotSendMailInline_OnTheHappyPath()
    {
        RequireDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        await client.SendAsync(MultipartRequest.Contact());

        Assert.Empty(factory.Email.Sent);
    }

    [SkippableFact]
    public async Task Post_WithHoneypotFilled_StoresNothing()
    {
        RequireDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        using var response = await client.SendAsync(MultipartRequest.Contact(gotcha: "bot"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await using var db = Db();
        Assert.Equal(0, await db.ContactSubmissions.CountAsync());
    }

    [SkippableFact]
    public async Task Post_WithInvalidField_StoresNothing()
    {
        RequireDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        await client.SendAsync(MultipartRequest.Contact(service: "junk"));

        await using var db = Db();
        Assert.Equal(0, await db.ContactSubmissions.CountAsync());
    }

    // Nothing was truncated client-side: no maxLength attribute exists anywhere
    // and the form is noValidate. Over-length has to be a 400, never a silent
    // truncation into the column.
    [SkippableFact]
    public async Task Post_WithMessageOverTheCap_Returns400AndStoresNothing()
    {
        RequireDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        using var response = await client.SendAsync(MultipartRequest.Contact(message: new string('x', 5001)));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        await using var db = Db();
        Assert.Equal(0, await db.ContactSubmissions.CountAsync());
    }

    [SkippableFact]
    public async Task Post_WithMessageExactlyAtTheCap_IsStored()
    {
        RequireDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        using var response = await client.SendAsync(MultipartRequest.Contact(message: new string('x', 5000)));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await using var db = Db();
        var row = await db.ContactSubmissions.SingleAsync();
        Assert.Equal(5000, row.Message.Length);
    }

    [SkippableFact]
    public async Task Get_Health_WhenDatabaseUp_Returns200Ok()
    {
        RequireDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/api/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(Bodies.Healthy, await response.Content.ReadAsStringAsync());
    }

    // A datetime(6) column holds microseconds and DateTime holds 100ns ticks.
    // Without truncation on write, the in-memory value never equals the row.
    [SkippableFact]
    public async Task Post_CreatedAtRoundTripsExactly()
    {
        RequireDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        await client.SendAsync(MultipartRequest.Contact());

        await using var db = Db();
        var row = await db.ContactSubmissions.SingleAsync();

        Assert.Equal(DateTimeKind.Utc, row.CreatedAt.Kind);
        Assert.Equal(factory.Clock.GetUtcNow().UtcDateTime, row.CreatedAt);
    }
}
