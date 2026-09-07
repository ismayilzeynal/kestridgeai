using System.Net;
using Microsoft.EntityFrameworkCore;

namespace Kestridge.Api.Tests;

// Duplicates are a documented failure mode of this exact client: a missing
// Access-Control-Allow-Origin makes fetch reject after the row is already
// stored, and Contact.tsx re-enables submission on both success and error.
public class DedupeTests(MySqlFixture fixture) : DatabaseTestBase(fixture)
{
    [SkippableFact]
    public async Task TwoIdenticalPostsInSameWindow_Return200_AndStoreOneRow()
    {
        RequireDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        using var first = await client.SendAsync(MultipartRequest.Contact());
        using var second = await client.SendAsync(MultipartRequest.Contact());

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.OK, second.StatusCode);

        await using var db = Db();
        Assert.Equal(1, await db.ContactSubmissions.CountAsync());

        // The only thing separating the 1062 duplicate branch from the generic
        // database-failure branch: both return 200 and store nothing extra, but
        // the failure branch sends the notification inline.
        Assert.Empty(factory.Email.Sent);
    }

    [SkippableFact]
    public async Task TwoIdenticalPostsInDifferentWindows_StoreTwoRows()
    {
        RequireDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        await client.SendAsync(MultipartRequest.Contact());
        factory.Clock.Advance(TimeSpan.FromMinutes(11));
        await client.SendAsync(MultipartRequest.Contact());

        await using var db = Db();
        Assert.Equal(2, await db.ContactSubmissions.CountAsync());
    }

    [SkippableFact]
    public async Task DifferentMessagesSameEmail_StoreTwoRows()
    {
        RequireDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        await client.SendAsync(MultipartRequest.Contact(message: "First message about reporting."));
        await client.SendAsync(MultipartRequest.Contact(message: "Second message about automation."));

        await using var db = Db();
        Assert.Equal(2, await db.ContactSubmissions.CountAsync());
    }

    [SkippableFact]
    public async Task SameMessageDifferentEmail_StoreTwoRows()
    {
        RequireDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        await client.SendAsync(MultipartRequest.Contact(email: "jane@company.com"));
        await client.SendAsync(MultipartRequest.Contact(email: "john@company.com"));

        await using var db = Db();
        Assert.Equal(2, await db.ContactSubmissions.CountAsync());
    }

    [SkippableFact]
    public async Task EmailCaseDifference_IsTreatedAsDuplicate()
    {
        RequireDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        await client.SendAsync(MultipartRequest.Contact(email: "jane@company.com"));
        await client.SendAsync(MultipartRequest.Contact(email: "JANE@Company.com"));

        await using var db = Db();
        Assert.Equal(1, await db.ContactSubmissions.CountAsync());
        Assert.Empty(factory.Email.Sent);
    }

    // The email column is accent-sensitive on purpose: an accented spelling is a
    // different mailbox belonging to a different person, and a DSR delete that
    // matched both would erase a third party's data.
    [SkippableFact]
    public async Task AccentDifferenceInEmail_IsADifferentPerson()
    {
        RequireDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        await client.SendAsync(MultipartRequest.Contact(email: "jose@company.com"));
        await client.SendAsync(MultipartRequest.Contact(email: "josé@company.com"));

        await using var db = Db();
        Assert.Equal(2, await db.ContactSubmissions.CountAsync());
        Assert.Equal(1, await db.ContactSubmissions.CountAsync(x => x.Email == "jose@company.com"));
    }
}
