using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Kestridge.Api.Admin;
using Kestridge.Api.Data;
using Kestridge.Api.Options;
using MySqlConnector;

namespace Kestridge.Api.Tests;

// The database-backed half of stage 2. These skip when MySQL is not reachable
// and run for real on the server, where 03-deploy.sh fails the deploy on any
// failure.
public class ContentTests(MySqlFixture fixture) : DatabaseTestBase(fixture)
{
    // ------------------------------------------------------------ public feed

    [SkippableFact]
    public async Task Payload_MatchesTheTypeScriptFieldNames()
    {
        RequireDatabase();
        await SeedAsync();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var json = await client.GetStringAsync("/api/content");
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        // q, a, file, cardLabel and what are what src/data/*.ts calls these,
        // and matching them exactly is what lets those files be the fallback
        // with no adapter between them and the fetched payload.
        Assert.Equal("What does Kestridge AI do?", root.GetProperty("faq")[0].GetProperty("q").GetString());
        Assert.NotNull(root.GetProperty("faq")[0].GetProperty("a").GetString());
        Assert.Equal("avanade", root.GetProperty("companies")[0].GetProperty("file").GetString());
        Assert.Equal("CA", root.GetProperty("team")[0].GetProperty("initials").GetString());

        var service = root.GetProperty("services")[0];
        Assert.Equal("ai", service.GetProperty("id").GetString());
        Assert.Equal("Brain", service.GetProperty("icon").GetString());
        Assert.NotNull(service.GetProperty("cardLabel").GetString());
        Assert.NotNull(service.GetProperty("steps")[0].GetProperty("what").GetString());

        // index is not a field. Hero.tsx and Services.tsx compute it from the
        // position, so a stored number can never disagree with the page.
        Assert.False(service.TryGetProperty("index", out _));
    }

    [SkippableFact]
    public async Task Photo_IsReturnedAsAFullPublicPath()
    {
        RequireDatabase();
        await SeedAsync();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var json = await client.GetStringAsync("/api/content");
        using var document = JsonDocument.Parse(json);

        Assert.Equal("/team/chingiz-abdilov.jpg",
            document.RootElement.GetProperty("team")[0].GetProperty("photo").GetString());
    }

    [SkippableFact]
    public async Task HiddenCompanies_AreExcluded()
    {
        RequireDatabase();
        await SeedAsync();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var json = await client.GetStringAsync("/api/content");
        using var document = JsonDocument.Parse(json);
        var files = document.RootElement.GetProperty("companies")
            .EnumerateArray().Select(x => x.GetProperty("file").GetString()).ToList();

        Assert.Equal(17, files.Count);
        Assert.DoesNotContain("synovate", files);
        Assert.DoesNotContain("sahara-india", files);
    }

    [SkippableFact]
    public async Task EverySet_IsOrderedBySortOrderThenId()
    {
        RequireDatabase();
        await SeedAsync();

        // The first question, moved to the end. Order is the whole point of the
        // sort_order column: MySQL guarantees none without an ORDER BY.
        await using (var db = Db())
        {
            var first = db.SiteFaq.OrderBy(x => x.SortOrder).First();
            first.SortOrder = 99;
            await db.SaveChangesAsync();
        }

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var json = await client.GetStringAsync("/api/content");
        using var document = JsonDocument.Parse(json);
        var questions = document.RootElement.GetProperty("faq")
            .EnumerateArray().Select(x => x.GetProperty("q").GetString()).ToList();

        Assert.Equal("What does Kestridge AI do?", questions[^1]);
    }

    [SkippableFact]
    public async Task EmptyTables_Return200WithEmptyArraysNot500()
    {
        RequireDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/content");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(0, document.RootElement.GetProperty("faq").GetArrayLength());
        Assert.Equal(0, document.RootElement.GetProperty("services").GetArrayLength());
    }

    [SkippableFact]
    public async Task Endpoint_RequiresNoToken()
    {
        RequireDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/content")).StatusCode);
    }

    [SkippableFact]
    public async Task Response_CarriesPublicMaxAgeWhileContactStillCarriesNoStore()
    {
        RequireDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var content = await client.GetAsync("/api/content");
        Assert.Equal("public, max-age=60, stale-while-revalidate=600",
            string.Join(", ", content.Headers.GetValues("Cache-Control")));

        var health = await client.GetAsync("/api/health");
        Assert.Equal("no-store", string.Join(", ", health.Headers.GetValues("Cache-Control")));
    }

    // The regression this prevents is silent: Vercel's revalidation comes from
    // one address, and in the contact partition that is five calls per ten
    // minutes before a 429, after which the site quietly serves the compiled
    // constants forever.
    [SkippableFact]
    public async Task ContentCalls_DoNotConsumeTheContactBudget()
    {
        RequireDatabase();

        using var factory = CreateFactory(new Dictionary<string, string?>
        {
            ["Kestridge:RateLimit:PermitsPerWindow"] = "2",
            ["Kestridge:RateLimit:ContentPermitsPerWindow"] = "50",
        });

        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiFactory.ClientAddressHeader, "198.51.100.7");

        for (var i = 0; i < 10; i++)
        {
            Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/content")).StatusCode);
        }

        // The contact form's own budget is untouched: two permits still there.
        var submission = await client.SendAsync(MultipartRequest.Contact());
        Assert.NotEqual(HttpStatusCode.TooManyRequests, submission.StatusCode);
    }

    [SkippableFact]
    public async Task ContentCalls_Are429AfterTheirOwnBudget()
    {
        RequireDatabase();

        using var factory = CreateFactory(new Dictionary<string, string?>
        {
            ["Kestridge:RateLimit:ContentPermitsPerWindow"] = "3",
        });

        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiFactory.ClientAddressHeader, "198.51.100.8");

        for (var i = 0; i < 3; i++)
        {
            Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/content")).StatusCode);
        }

        var refused = await client.GetAsync("/api/content");
        Assert.Equal(HttpStatusCode.TooManyRequests, refused.StatusCode);

        // RateLimiting.OnRejected hardcodes this, which is why every partition
        // shares WindowMinutes.
        Assert.Equal("600", string.Join(", ", refused.Headers.GetValues("Retry-After")));
    }

    // ----------------------------------------------------------- admin writes

    [SkippableFact]
    public async Task FaqCreateEditAndDelete_RoundTrip()
    {
        RequireDatabase();
        await SeedAsync();

        using var factory = CreateFactory();
        using var client = await SignedInAsync(factory);

        var created = await Post(client, "/api/admin/content/faq/save",
            new { id = (long?)null, question = "Do you sign an NDA?", answer = "Yes, for every project." });

        var id = created.GetProperty("row").GetProperty("id").GetInt64();
        Assert.True(id > 0);
        Assert.Equal("Test Operator", created.GetProperty("row").GetProperty("updatedBy").GetString());

        // No revalidate URL is configured in the tests, so published is false
        // and the save still happened. That is the honest answer.
        Assert.False(created.GetProperty("published").GetBoolean());

        var edited = await Post(client, "/api/admin/content/faq/save",
            new { id, question = "Do you sign an NDA?", answer = "Yes, before any work starts." });
        Assert.Equal("Yes, before any work starts.", edited.GetProperty("row").GetProperty("answer").GetString());

        await using (var db = Db())
        {
            Assert.Equal(9, db.SiteFaq.Count());
        }

        await Post(client, "/api/admin/content/faq/delete", new { id });

        await using (var db = Db())
        {
            Assert.Equal(8, db.SiteFaq.Count());
        }
    }

    [SkippableFact]
    public async Task DeletingTheLastFaq_IsRefused()
    {
        RequireDatabase();

        long id;
        await using (var db = Db())
        {
            var only = new SiteFaq { Question = "Only one", Answer = "Only answer", UpdatedAt = DateTime.UtcNow };
            db.SiteFaq.Add(only);
            await db.SaveChangesAsync();
            id = only.Id;
        }

        using var factory = CreateFactory();
        using var client = await SignedInAsync(factory);

        var response = await client.PostAsJsonAsync("/api/admin/content/faq/delete", new { id });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("count", document.RootElement.GetProperty("reason").GetString());
    }

    [SkippableFact]
    public async Task Reorder_WritesContiguousSortOrder()
    {
        RequireDatabase();
        await SeedAsync();

        long[] ids;
        await using (var db = Db())
        {
            ids = [.. db.SiteFaq.OrderBy(x => x.SortOrder).Select(x => x.Id)];
        }

        var reversed = ids.Reverse().ToArray();

        using var factory = CreateFactory();
        using var client = await SignedInAsync(factory);

        var response = await client.PostAsJsonAsync("/api/admin/content/faq/reorder", new { ids = reversed });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await using (var db = Db())
        {
            var order = db.SiteFaq.OrderBy(x => x.SortOrder).Select(x => x.Id).ToArray();
            Assert.Equal(reversed, order);
            Assert.Equal(Enumerable.Range(0, ids.Length), db.SiteFaq.OrderBy(x => x.SortOrder).Select(x => x.SortOrder));
        }
    }

    [SkippableFact]
    public async Task Reorder_RejectsAPartialOrForeignIdList()
    {
        RequireDatabase();
        await SeedAsync();

        long[] ids;
        await using (var db = Db())
        {
            ids = [.. db.SiteFaq.OrderBy(x => x.SortOrder).Select(x => x.Id)];
        }

        using var factory = CreateFactory();
        using var client = await SignedInAsync(factory);

        var partial = await client.PostAsJsonAsync("/api/admin/content/faq/reorder", new { ids = ids.Take(3) });
        Assert.Equal(HttpStatusCode.BadRequest, partial.StatusCode);

        var foreign = ids.ToArray();
        foreign[0] = 999999;
        var wrong = await client.PostAsJsonAsync("/api/admin/content/faq/reorder", new { ids = foreign });
        Assert.Equal(HttpStatusCode.BadRequest, wrong.StatusCode);

        // A duplicate would otherwise leave two rows sharing a position.
        var duplicated = ids.ToArray();
        duplicated[1] = duplicated[0];
        var repeated = await client.PostAsJsonAsync("/api/admin/content/faq/reorder", new { ids = duplicated });
        Assert.Equal(HttpStatusCode.BadRequest, repeated.StatusCode);
    }

    [SkippableFact]
    public async Task TeamCreate_IsRefusedAndThereIsNoTeamDelete()
    {
        RequireDatabase();
        await SeedAsync();

        using var factory = CreateFactory();
        using var client = await SignedInAsync(factory);

        var created = await client.PostAsJsonAsync("/api/admin/content/team/save",
            new { id = (long?)null, name = "New Person", initials = "NP", role = "Founder", focus = "Something", photo = "" });
        Assert.Equal(HttpStatusCode.BadRequest, created.StatusCode);

        using var document = JsonDocument.Parse(await created.Content.ReadAsStringAsync());
        Assert.Equal("immutable", document.RootElement.GetProperty("reason").GetString());

        // Not merely disabled: the route does not exist.
        var deleted = await client.PostAsJsonAsync("/api/admin/content/team/delete", new { id = 1 });
        Assert.Equal(HttpStatusCode.NotFound, deleted.StatusCode);
    }

    [SkippableFact]
    public async Task CompanyHide_RemovesItFromPublicContentButNotFromTheAdminList()
    {
        RequireDatabase();
        await SeedAsync();

        long id;
        await using (var db = Db())
        {
            id = db.SiteCompanies.First(x => x.LogoFile == "ecolab").Id;
        }

        using var factory = CreateFactory();
        using var client = await SignedInAsync(factory);

        await Post(client, "/api/admin/content/companies/save", new { id, name = "Ecolab", hidden = true });

        var admin = await Get(client, "/api/admin/content");
        Assert.Equal(20, admin.GetProperty("companies").GetArrayLength());

        using var anonymous = factory.CreateClient();
        using var feed = JsonDocument.Parse(await anonymous.GetStringAsync("/api/content"));
        var files = feed.RootElement.GetProperty("companies")
            .EnumerateArray().Select(x => x.GetProperty("file").GetString()).ToList();

        Assert.DoesNotContain("ecolab", files);
    }

    [SkippableFact]
    public async Task CompanyAdd_AcceptsOnlyAnUnusedAllowlistedLogo()
    {
        RequireDatabase();
        await SeedAsync();

        using var factory = CreateFactory();
        using var client = await SignedInAsync(factory);

        // Already seeded, hidden. Adding it again would put the same image in
        // the marquee twice.
        var duplicate = await client.PostAsJsonAsync("/api/admin/content/companies/add",
            new { logoFile = "synovate", name = "Synovate" });
        Assert.Equal(HttpStatusCode.BadRequest, duplicate.StatusCode);

        var unknown = await client.PostAsJsonAsync("/api/admin/content/companies/add",
            new { logoFile = "acme-corp", name = "Acme" });
        Assert.Equal(HttpStatusCode.BadRequest, unknown.StatusCode);

        await using (var db = Db())
        {
            db.SiteCompanies.RemoveRange(db.SiteCompanies.Where(x => x.LogoFile == "synovate"));
            await db.SaveChangesAsync();
        }

        var added = await Post(client, "/api/admin/content/companies/add",
            new { logoFile = "synovate", name = "Synovate" });
        Assert.Equal("synovate", added.GetProperty("row").GetProperty("logoFile").GetString());
    }

    [SkippableFact]
    public async Task ServiceSave_RefusesCreationIgnoresSlugAndPinsTheCounts()
    {
        RequireDatabase();
        await SeedAsync();

        long id;
        await using (var db = Db())
        {
            id = db.SiteServices.First(x => x.Slug == "ai").Id;
        }

        using var factory = CreateFactory();
        using var client = await SignedInAsync(factory);

        var steps = new[]
        {
            new { phase = "One", summary = "First", what = "The first step." },
            new { phase = "Two", summary = "Second", what = "The second step." },
            new { phase = "Three", summary = "Third", what = "The third step." },
            new { phase = "Four", summary = "Fourth", what = "The fourth step." },
        };

        string[] highlights = ["One thing", "Another thing", "A third thing"];

        var creation = await client.PostAsJsonAsync("/api/admin/content/services/save",
            new { id = (long?)null, name = "New", tagline = "t", cardLabel = "c", description = "d",
                  iconName = "Brain", highlights, steps });
        Assert.Equal(HttpStatusCode.BadRequest, creation.StatusCode);

        var threeSteps = await client.PostAsJsonAsync("/api/admin/content/services/save",
            new { id, name = "AI Solutions", tagline = "t", cardLabel = "c", description = "d",
                  iconName = "Brain", highlights, steps = steps.Take(3) });
        Assert.Equal(HttpStatusCode.BadRequest, threeSteps.StatusCode);

        var twoHighlights = await client.PostAsJsonAsync("/api/admin/content/services/save",
            new { id, name = "AI Solutions", tagline = "t", cardLabel = "c", description = "d",
                  iconName = "Brain", highlights = highlights.Take(2), steps });
        Assert.Equal(HttpStatusCode.BadRequest, twoHighlights.StatusCode);

        var badIcon = await client.PostAsJsonAsync("/api/admin/content/services/save",
            new { id, name = "AI Solutions", tagline = "t", cardLabel = "c", description = "d",
                  iconName = "Rocket", highlights, steps });
        Assert.Equal(HttpStatusCode.BadRequest, badIcon.StatusCode);

        // A slug in the body is not an error, it simply has no effect. There is
        // no shape of request that can rename one.
        var saved = await Post(client, "/api/admin/content/services/save",
            new { id, slug = "renamed", name = "AI Solutions", tagline = "New tagline", cardLabel = "c",
                  description = "d", iconName = "Workflow", highlights, steps });

        Assert.Equal("ai", saved.GetProperty("row").GetProperty("slug").GetString());

        await using (var db = Db())
        {
            Assert.Equal(4, db.SiteServiceSteps.Count(x => x.ServiceId == id));
            Assert.Equal(3, db.SiteServiceHighlights.Count(x => x.ServiceId == id));

            // Replaced, not appended. Sixteen steps in total across four
            // services is what the seed produced and what must remain.
            Assert.Equal(16, db.SiteServiceSteps.Count());
        }
    }

    [SkippableFact]
    public async Task ContentEndpoints_RefuseAnUnauthenticatedCaller()
    {
        RequireDatabase();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/admin/content")).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized,
            (await client.PostAsJsonAsync("/api/admin/content/faq/save", new { question = "q", answer = "a" })).StatusCode);
    }

    // ------------------------------------------------------------------- seed

    [SkippableFact]
    public async Task AfterSeeding_TheCountsAreTheOnesTheSiteShips()
    {
        RequireDatabase();
        await SeedAsync();

        await using var db = Db();

        Assert.Equal(8, db.SiteFaq.Count());
        Assert.Equal(4, db.SiteTeam.Count());
        Assert.Equal(20, db.SiteCompanies.Count());
        Assert.Equal(3, db.SiteCompanies.Count(x => x.Hidden));
        Assert.Equal(4, db.SiteServices.Count());
        Assert.Equal(16, db.SiteServiceSteps.Count());
        Assert.Equal(21, db.SiteServiceHighlights.Count());
    }

    [SkippableFact]
    public async Task RunningTheSeedTwice_ChangesNothing()
    {
        RequireDatabase();
        await SeedAsync();

        await using (var db = Db())
        {
            var first = db.SiteFaq.OrderBy(x => x.SortOrder).First();
            first.Question = "Edited by a human";
            await db.SaveChangesAsync();
        }

        await SeedAsync();

        await using (var db = Db())
        {
            Assert.Equal(8, db.SiteFaq.Count());
            Assert.Equal("Edited by a human", db.SiteFaq.OrderBy(x => x.SortOrder).First().Question);
        }
    }

    // Every seeded string has to survive the same validator the panel writes
    // through. Otherwise the site ships copy that the panel would refuse to
    // save back, and the first person to edit a row cannot save it.
    [SkippableFact]
    public async Task EverySeededString_PassesTheWriteValidator()
    {
        RequireDatabase();
        await SeedAsync();

        await using var db = Db();

        foreach (var row in db.SiteFaq)
        {
            Assert.Null(ContentValidation.Text("question", row.Question, 200, out _));
            Assert.Null(ContentValidation.Text("answer", row.Answer, 600, out _));
        }

        foreach (var row in db.SiteTeam)
        {
            Assert.Null(ContentValidation.Text("name", row.Name, 80, out _));
            Assert.Null(ContentValidation.Initials("initials", row.Initials, out _));
            Assert.Null(ContentValidation.Text("role", row.Role, 60, out _));
            Assert.Null(ContentValidation.Text("focus", row.Focus, 120, out _));
            Assert.Null(ContentValidation.Asset("photo", row.Photo, ContentValidation.Photos, out _));
        }

        foreach (var row in db.SiteCompanies)
        {
            Assert.Null(ContentValidation.Text("name", row.Name, 80, out _));
            Assert.Null(ContentValidation.Asset("logoFile", row.LogoFile, ContentValidation.Logos, out _));
        }

        foreach (var row in db.SiteServices)
        {
            Assert.Null(ContentValidation.Text("name", row.Name, 60, out _));
            Assert.Null(ContentValidation.Text("tagline", row.Tagline, 120, out _));
            Assert.Null(ContentValidation.Text("cardLabel", row.CardLabel, 60, out _));
            Assert.Null(ContentValidation.Text("description", row.Description, 400, out _));
            Assert.Null(ContentValidation.Icon("iconName", row.IconName, out _));
        }

        foreach (var row in db.SiteServiceSteps)
        {
            Assert.Null(ContentValidation.Text("phase", row.Phase, 40, out _));
            Assert.Null(ContentValidation.Text("summary", row.Summary, 80, out _));
            Assert.Null(ContentValidation.Text("detail", row.Detail, 240, out _));
        }

        foreach (var row in db.SiteServiceHighlights)
        {
            Assert.Null(ContentValidation.Text("text", row.Text, 80, out _));
        }
    }

    // The four slugs are also the four values in Kestridge:Contact:AllowedServices
    // and the four DOM ids the hero cards scroll to. Nothing can change them,
    // and this is what proves the seed did not invent a fifth.
    [SkippableFact]
    public async Task SeededSlugs_AreTheFourTheContactFormAccepts()
    {
        RequireDatabase();
        await SeedAsync();

        await using var db = Db();

        Assert.Equal(["ai", "analytics", "automation", "security"],
            db.SiteServices.OrderBy(x => x.Slug).Select(x => x.Slug).ToArray());
    }

    // ---------------------------------------------------------------- helpers

    private async Task SeedAsync()
    {
        var path = Path.Combine(OpsDirectory(), "05-seed-content.sql");
        var script = await File.ReadAllTextAsync(path);

        await using var connection = new MySqlConnection(Fixture.ConnectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(script, connection);
        await command.ExecuteNonQueryAsync();
    }

    private static string OpsDirectory()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            var ops = Path.Combine(directory.FullName, "ops");
            if (File.Exists(Path.Combine(ops, "05-seed-content.sql")))
            {
                return ops;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Could not locate backend/ops from " + AppContext.BaseDirectory);
    }

    // The session row is written directly rather than driven through
    // /api/admin/login. What is under test here is the content surface, and a
    // login that changed shape should fail AdminLogin's own tests, not these.
    private async Task<HttpClient> SignedInAsync(ApiFactory factory)
    {
        var now = factory.Clock.GetUtcNow().UtcDateTime;
        var token = AdminSessions.NewToken();

        await using (var db = Db())
        {
            var account = new AdminAccount
            {
                Username = "tester",
                DisplayName = "Test Operator",
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
        client.DefaultRequestHeaders.Add(ApiFactory.ClientAddressHeader, "203.0.113.9");
        return client;
    }

    private static async Task<JsonElement> Post(HttpClient client, string path, object body)
    {
        var response = await client.PostAsJsonAsync(path, body);
        var text = await response.Content.ReadAsStringAsync();

        Assert.True(response.IsSuccessStatusCode, path + " answered " + (int)response.StatusCode + ": " + text);

        return JsonDocument.Parse(text).RootElement.Clone();
    }

    private static async Task<JsonElement> Get(HttpClient client, string path)
        => JsonDocument.Parse(await client.GetStringAsync(path)).RootElement.Clone();
}
