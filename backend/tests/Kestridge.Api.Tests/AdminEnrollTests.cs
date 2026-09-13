using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Kestridge.Api.Admin;
using Kestridge.Api.Data;
using Kestridge.Api.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Kestridge.Api.Tests;

// Sign-in: /login/start, /login/enroll, and the parts of /login they depend on.
public class AdminEnrollTests(MySqlFixture fixture) : DatabaseTestBase(fixture)
{
    private const string DefaultBody = "{\"ok\":true,\"enroll\":false}";
    private const string StaleBody = "{\"ok\":false,\"error\":\"stale\"}";
    private const string InitialPassword = "initial password 1";
    private const string ChosenPassword = "a password I chose myself";

    private ApiFactory Factory() => CreateFactory(AdminTestAccounts.FactorySettings());

    private static DateTime Now(ApiFactory factory) => factory.Clock.GetUtcNow().UtcDateTime;

    private async Task<AdminEnrollment> InvitationAsync(
        ApiFactory factory, string username, TimeSpan? expiresIn = null, string? passwordHash = null)
    {
        var now = Now(factory);
        await using var db = Db();

        var row = new AdminEnrollment
        {
            Username = username,
            DisplayName = "New Person",
            PasswordHash = passwordHash ?? AdminTestAccounts.Hash(InitialPassword),
            CreatedBy = "Test Operator",
            CreatedAt = now,
            ExpiresAt = now + (expiresIn ?? TimeSpan.FromHours(72)),
        };

        db.AdminEnrollments.Add(row);
        await db.SaveChangesAsync();
        return row;
    }

    private async Task<AdminEnrollment> ResetAsync(ApiFactory factory, long accountId, TimeSpan? expiresIn = null)
    {
        var now = Now(factory);
        await using var db = Db();

        var row = new AdminEnrollment
        {
            AccountId = accountId,
            DisplayName = "Reset Person",
            CreatedBy = "Test Operator",
            CreatedAt = now,
            ExpiresAt = now + (expiresIn ?? TimeSpan.FromHours(72)),
        };

        db.AdminEnrollments.Add(row);
        await db.SaveChangesAsync();
        return row;
    }

    private static async Task<(HttpStatusCode Status, string Text)> StartAsync(HttpClient client, string? username, string? password)
    {
        using var response = await client.PostAsJsonAsync("/api/admin/login/start", new { username, password });
        return (response.StatusCode, await response.Content.ReadAsStringAsync());
    }

    private static async Task<JsonElement> StartEnrollingAsync(HttpClient client, string username, string password)
    {
        var (status, text) = await StartAsync(client, username, password);

        Assert.Equal(HttpStatusCode.OK, status);
        var body = JsonDocument.Parse(text).RootElement.Clone();
        Assert.True(body.GetProperty("enroll").GetBoolean(), "start did not enrol: " + text);
        return body;
    }

    private static async Task<(HttpStatusCode Status, string Text)> EnrollAsync(
        HttpClient client, string? token, string? code, string? newPassword)
    {
        using var response = await client.PostAsJsonAsync("/api/admin/login/enroll", new { token, code, newPassword });
        return (response.StatusCode, await response.Content.ReadAsStringAsync());
    }

    private static async Task<HttpStatusCode> LoginAsync(HttpClient client, string username, string password, string code)
    {
        using var response = await client.PostAsJsonAsync("/api/admin/login", new { username, password, code });
        return response.StatusCode;
    }

    private static void AssertInvalid((HttpStatusCode Status, string Text) result, string field, string? reason = null)
    {
        Assert.Equal(HttpStatusCode.BadRequest, result.Status);

        var body = JsonDocument.Parse(result.Text).RootElement;
        Assert.Equal("invalid", body.GetProperty("error").GetString());
        Assert.Equal(field, body.GetProperty("field").GetString());

        if (reason is not null)
        {
            Assert.Equal(reason, body.GetProperty("reason").GetString());
        }
    }

    private async Task<AdminAccount?> AccountAsync(string username)
    {
        await using var db = Db();
        return await db.AdminAccounts.AsNoTracking().SingleOrDefaultAsync(x => x.Username == username);
    }

    // ------------------------------------------------------------------ start

    // The property that keeps start from being an oracle. Every case that is not
    // a live enrolment with the right password answers with the same bytes,
    // including a normal account given its correct password.
    [SkippableFact]
    public async Task Start_AnswersIdenticallyForEveryCaseThatIsNotAnEnrolment()
    {
        RequireDatabase();

        using var factory = Factory();
        var now = Now(factory);
        using var client = factory.CreateClient();

        var normal = await AdminTestAccounts.CreateAsync(Db, factory, "normal", "Normal");
        await AdminTestAccounts.CreateAsync(Db, factory, "by.column", "Column", a => a.Disabled = true);
        var byPanel = await AdminTestAccounts.CreateAsync(Db, factory, "by.panel", "Panel");
        await AdminTestAccounts.CreateAsync(Db, factory, "locked", "Locked", a => a.LockedUntil = now.AddMinutes(10));
        await AdminTestAccounts.CreateAsync(Db, factory, "bare", "Bare", a => a.TotpSecret = "");
        var lapsed = await AdminTestAccounts.CreateAsync(Db, factory, "lapsed", "Lapsed", a => a.TotpSecret = "");
        var disabledReset = await AdminTestAccounts.CreateAsync(Db, factory, "disabled.reset", "Disabled Reset", a =>
        {
            a.TotpSecret = "";
            a.Disabled = true;
        });
        var lockedReset = await AdminTestAccounts.CreateAsync(Db, factory, "locked.reset", "Locked Reset", a =>
        {
            a.TotpSecret = "";
            a.LockedUntil = now.AddMinutes(10);
        });

        // Disabled from the panel while a reset was live. The one case where the
        // admin_disables half of the check changes the answer: without it, the
        // right password here would be handed a setup token.
        var panelReset = await AdminTestAccounts.CreateAsync(Db, factory, "panel.reset", "Panel Reset", a => a.TotpSecret = "");

        await using (var db = Db())
        {
            db.AdminDisables.Add(new AdminDisable { AccountId = byPanel.Id, DisabledAt = now, DisabledBy = "x" });
            db.AdminDisables.Add(new AdminDisable { AccountId = panelReset.Id, DisabledAt = now, DisabledBy = "x" });
            await db.SaveChangesAsync();
        }

        await ResetAsync(factory, lapsed.Id, TimeSpan.FromHours(-1));
        await ResetAsync(factory, disabledReset.Id);
        await ResetAsync(factory, lockedReset.Id);
        await ResetAsync(factory, panelReset.Id);
        await InvitationAsync(factory, "too.late", TimeSpan.FromHours(-1));

        (string? Username, string? Password)[] cases =
        [
            ("nobody", AdminTestAccounts.Password),
            (null, null),
            ("normal", "not the password"),
            ("normal", AdminTestAccounts.Password),
            ("by.column", AdminTestAccounts.Password),
            ("by.panel", AdminTestAccounts.Password),
            ("locked", AdminTestAccounts.Password),
            ("bare", AdminTestAccounts.Password),
            ("lapsed", AdminTestAccounts.Password),
            ("disabled.reset", AdminTestAccounts.Password),
            ("locked.reset", AdminTestAccounts.Password),
            ("panel.reset", AdminTestAccounts.Password),
            ("too.late", InitialPassword),
        ];

        foreach (var (username, password) in cases)
        {
            var (status, text) = await StartAsync(client, username, password);

            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal(DefaultBody, text);
        }

        // A wrong password on an enrolled account is /login's to count.
        Assert.Equal(0, (await AccountAsync(normal.Username))!.FailedAttempts);

        await using (var db = Db())
        {
            Assert.False(await db.AdminEnrollments.AnyAsync(x => x.TokenHash != null));
        }
    }

    [SkippableFact]
    public async Task Start_NewUserWithTheRightPassword_GetsTheSetupData()
    {
        RequireDatabase();

        using var factory = Factory();
        var now = Now(factory);
        using var client = factory.CreateClient();
        var invitation = await InvitationAsync(factory, "new.user");

        // Normalised the way /login normalises.
        var body = await StartEnrollingAsync(client, "  New.User ", InitialPassword);

        Assert.True(body.GetProperty("ok").GetBoolean());
        Assert.True(body.GetProperty("setPassword").GetBoolean());
        Assert.Equal("new.user", body.GetProperty("username").GetString());

        var token = body.GetProperty("token").GetString()!;
        var secret = body.GetProperty("secret").GetString()!;

        Assert.True(AdminSessions.IsWellFormed(token));
        Assert.Equal(20, Totp.FromBase32(secret).Length);
        Assert.Equal(Totp.OtpauthUri("new.user", secret), body.GetProperty("otpauth").GetString());
        Assert.StartsWith("data:image/png;base64,", body.GetProperty("qr").GetString(), StringComparison.Ordinal);

        await using (var db = Db())
        {
            var row = await db.AdminEnrollments.AsNoTracking().SingleAsync(x => x.Id == invitation.Id);
            Assert.Equal(AdminSessions.Hash(token), row.TokenHash);
            Assert.Equal(now.AddMinutes(15), row.TokenExpiresAt);
            Assert.Equal(secret, row.TotpSecret);
        }

        // A new secret and a new token on every start. The first token is dead.
        var again = await StartEnrollingAsync(client, "new.user", InitialPassword);
        Assert.NotEqual(secret, again.GetProperty("secret").GetString());
        Assert.NotEqual(token, again.GetProperty("token").GetString());

        var stale = await EnrollAsync(client, token, AdminTestAccounts.Code(factory, secret), ChosenPassword);
        Assert.Equal(HttpStatusCode.Conflict, stale.Status);
    }

    [SkippableFact]
    public async Task Start_ResetWithTheRightPassword_GetsTheSetupDataWithoutAPasswordStep()
    {
        RequireDatabase();

        using var factory = Factory();
        using var client = factory.CreateClient();
        var account = await AdminTestAccounts.CreateAsync(Db, factory, "emil", "Emil", a => a.TotpSecret = "");
        await ResetAsync(factory, account.Id);

        var body = await StartEnrollingAsync(client, "emil", account.Password);

        Assert.False(body.GetProperty("setPassword").GetBoolean());
        Assert.Equal("emil", body.GetProperty("username").GetString());
        Assert.Equal(
            Totp.OtpauthUri("emil", body.GetProperty("secret").GetString()!),
            body.GetProperty("otpauth").GetString());
    }

    [SkippableFact]
    public async Task Start_WrongPassword_CountsOnTheInvitationOrTheResetAccount()
    {
        RequireDatabase();

        using var factory = Factory();
        using var client = factory.CreateClient();
        var invitation = await InvitationAsync(factory, "new.user");
        var account = await AdminTestAccounts.CreateAsync(Db, factory, "emil", "Emil", a => a.TotpSecret = "");
        var reset = await ResetAsync(factory, account.Id);

        Assert.Equal(DefaultBody, (await StartAsync(client, "new.user", "not the password")).Text);
        Assert.Equal(DefaultBody, (await StartAsync(client, "emil", "not the password")).Text);

        await using var db = Db();
        Assert.Equal(1, (await db.AdminEnrollments.AsNoTracking().SingleAsync(x => x.Id == invitation.Id)).FailedAttempts);
        Assert.Equal(1, (await db.AdminAccounts.AsNoTracking().SingleAsync(x => x.Id == account.Id)).FailedAttempts);
        Assert.Equal(0, (await db.AdminEnrollments.AsNoTracking().SingleAsync(x => x.Id == reset.Id)).FailedAttempts);
    }

    [SkippableFact]
    public async Task Start_LockedInvitation_RefusesEvenTheRightPassword()
    {
        RequireDatabase();

        using var factory = Factory();
        using var client = factory.CreateClient();
        var invitation = await InvitationAsync(factory, "new.user");

        for (var i = 0; i < 5; i++)
        {
            Assert.Equal(DefaultBody, (await StartAsync(client, "new.user", "guess " + i)).Text);
        }

        Assert.Equal(DefaultBody, (await StartAsync(client, "new.user", InitialPassword)).Text);

        await using var db = Db();
        var row = await db.AdminEnrollments.AsNoTracking().SingleAsync(x => x.Id == invitation.Id);
        Assert.NotNull(row.LockedUntil);
        Assert.Null(row.TokenHash);
    }

    // The counting rules, read back from the row the single UPDATE wrote: a one
    // hour window that starts afresh, and a lock at the limit inside it.
    [SkippableFact]
    public async Task Start_WrongPasswords_CountInAnHourWindowAndLockAtTheLimit()
    {
        RequireDatabase();

        using var factory = Factory();
        var start = Now(factory);
        var options = new AdminOptions();
        using var client = factory.CreateClient();
        var invitation = await InvitationAsync(factory, "new.user");

        async Task<AdminEnrollment> RowAsync()
        {
            await using var db = Db();
            return await db.AdminEnrollments.AsNoTracking().SingleAsync(x => x.Id == invitation.Id);
        }

        for (var i = 0; i < options.MaxFailedAttempts - 1; i++)
        {
            await StartAsync(client, "new.user", "guess " + i);
        }

        var before = await RowAsync();
        Assert.Equal(options.MaxFailedAttempts - 1, before.FailedAttempts);
        Assert.Equal(start, before.FirstFailedAt);
        Assert.Null(before.LockedUntil);

        // An hour later the old failures no longer count.
        factory.Clock.Advance(TimeSpan.FromHours(1));
        var later = Now(factory);

        await StartAsync(client, "new.user", "a guess an hour later");

        var fresh = await RowAsync();
        Assert.Equal(1, fresh.FailedAttempts);
        Assert.Equal(later, fresh.FirstFailedAt);
        Assert.Null(fresh.LockedUntil);

        for (var i = 1; i < options.MaxFailedAttempts; i++)
        {
            await StartAsync(client, "new.user", "another guess " + i);
        }

        var locked = await RowAsync();
        Assert.Equal(options.MaxFailedAttempts, locked.FailedAttempts);
        Assert.Equal(later, locked.FirstFailedAt);
        Assert.Equal(later.AddMinutes(options.LockMinutes), locked.LockedUntil);

        // A locked row is refused before it is counted again.
        await StartAsync(client, "new.user", InitialPassword);
        Assert.Equal(options.MaxFailedAttempts, (await RowAsync()).FailedAttempts);
    }

    // Guesses sent together. Each is counted before its password is checked, in
    // one UPDATE, so none of them can read a count another has not yet saved.
    // The invitations carry a slow hash, which widens the window a counter that
    // reads, verifies and then saves would lose its updates in: with that
    // counter the burst below the limit ends at one, and the large burst never
    // locks. The same holds for wrong codes at /login.
    [SkippableFact]
    public async Task ConcurrentWrongAttempts_AreEachCountedAndLockAtTheLimit()
    {
        RequireDatabase();

        using var factory = Factory();
        var options = new AdminOptions();
        using var client = factory.CreateClient();

        var slow = new PasswordHasher<AdminAccount>(Microsoft.Extensions.Options.Options.Create(new PasswordHasherOptions
        {
            CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3,
            IterationCount = 100_000,
        }));

        var slowHash = slow.HashPassword(new AdminAccount(), InitialPassword);
        var few = await InvitationAsync(factory, "few.guesses", passwordHash: slowHash);
        var many = await InvitationAsync(factory, "many.guesses", passwordHash: slowHash);
        var account = await AdminTestAccounts.CreateAsync(Db, factory, "emil", "Emil");

        var below = options.MaxFailedAttempts - 2;
        const int burst = 20;

        var wrongCode = AdminTestAccounts.WrongCode(factory, account.Secret);

        var starts = Enumerable.Range(0, below).Select(i => StartAsync(client, "few.guesses", "guess " + i))
            .Concat(Enumerable.Range(0, burst).Select(i => StartAsync(client, "many.guesses", "guess " + i)))
            .ToList();

        var logins = Enumerable.Range(0, burst)
            .Select(_ => LoginAsync(client, "emil", account.Password, wrongCode))
            .ToList();

        foreach (var result in await Task.WhenAll(starts))
        {
            Assert.Equal(DefaultBody, result.Text);
        }

        foreach (var status in await Task.WhenAll(logins))
        {
            Assert.Equal(HttpStatusCode.Unauthorized, status);
        }

        await using var db = Db();

        var fewRow = await db.AdminEnrollments.AsNoTracking().SingleAsync(x => x.Id == few.Id);
        Assert.Equal(below, fewRow.FailedAttempts);
        Assert.Null(fewRow.LockedUntil);

        var manyRow = await db.AdminEnrollments.AsNoTracking().SingleAsync(x => x.Id == many.Id);
        Assert.True(manyRow.FailedAttempts >= options.MaxFailedAttempts, "counted " + manyRow.FailedAttempts);
        Assert.NotNull(manyRow.LockedUntil);

        var accountRow = await db.AdminAccounts.AsNoTracking().SingleAsync(x => x.Id == account.Id);
        Assert.True(accountRow.FailedAttempts >= options.MaxFailedAttempts, "counted " + accountRow.FailedAttempts);
        Assert.NotNull(accountRow.LockedUntil);
    }

    // Both username columns are ascii_bin, and MySQL refuses to compare one with
    // a parameter outside ASCII (ERROR 1267) instead of matching nothing. A name
    // a phone keyboard capitalised gets the ordinary answers, not a 500.
    [SkippableFact]
    public async Task NonAsciiUsername_GetsTheOrdinaryAnswers()
    {
        RequireDatabase();

        using var factory = Factory();
        using var client = factory.CreateClient();
        var account = await AdminTestAccounts.CreateAsync(Db, factory, "ilkin", "Ilkin");

        foreach (var username in new[] { "\u0130lkin", "\u0259li" })
        {
            var (status, text) = await StartAsync(client, username, account.Password);
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal(DefaultBody, text);

            Assert.Equal(HttpStatusCode.Unauthorized,
                await LoginAsync(client, username, account.Password, AdminTestAccounts.Code(factory, account.Secret)));
        }

        Assert.Equal(0, (await AccountAsync("ilkin"))!.FailedAttempts);
    }

    // ----------------------------------------------------------------- enroll

    [SkippableFact]
    public async Task Enroll_MalformedOrUnknownToken_IsStale()
    {
        RequireDatabase();

        using var factory = Factory();
        using var client = factory.CreateClient();

        var malformed = await EnrollAsync(client, "not-a-token", "123456", ChosenPassword);
        var missing = await EnrollAsync(client, null, "123456", ChosenPassword);
        var unknown = await EnrollAsync(client, AdminSessions.NewToken(), "123456", ChosenPassword);

        foreach (var result in new[] { malformed, missing, unknown })
        {
            Assert.Equal(HttpStatusCode.Conflict, result.Status);
            Assert.Equal(StaleBody, result.Text);
        }
    }

    [SkippableFact]
    public async Task Enroll_AfterTheTokenExpires_IsStale()
    {
        RequireDatabase();

        using var factory = Factory();
        using var client = factory.CreateClient();
        await InvitationAsync(factory, "new.user");

        var body = await StartEnrollingAsync(client, "new.user", InitialPassword);
        var token = body.GetProperty("token").GetString();
        var secret = body.GetProperty("secret").GetString()!;

        factory.Clock.Advance(TimeSpan.FromMinutes(16));

        var result = await EnrollAsync(client, token, AdminTestAccounts.Code(factory, secret), ChosenPassword);

        Assert.Equal(HttpStatusCode.Conflict, result.Status);
        Assert.Null(await AccountAsync("new.user"));
    }

    [SkippableFact]
    public async Task Enroll_AfterTheInvitationExpires_IsStaleEvenWithALiveToken()
    {
        RequireDatabase();

        using var factory = Factory();
        using var client = factory.CreateClient();
        await InvitationAsync(factory, "new.user", TimeSpan.FromMinutes(10));

        var body = await StartEnrollingAsync(client, "new.user", InitialPassword);
        var secret = body.GetProperty("secret").GetString()!;

        factory.Clock.Advance(TimeSpan.FromMinutes(11));

        var result = await EnrollAsync(
            client, body.GetProperty("token").GetString(), AdminTestAccounts.Code(factory, secret), ChosenPassword);

        Assert.Equal(HttpStatusCode.Conflict, result.Status);
    }

    [SkippableFact]
    public async Task Enroll_WrongCode_Is400OnCode()
    {
        RequireDatabase();

        using var factory = Factory();
        using var client = factory.CreateClient();
        await InvitationAsync(factory, "new.user");

        var body = await StartEnrollingAsync(client, "new.user", InitialPassword);
        var secret = body.GetProperty("secret").GetString()!;

        var result = await EnrollAsync(
            client, body.GetProperty("token").GetString(), AdminTestAccounts.WrongCode(factory, secret), ChosenPassword);

        AssertInvalid(result, "code");
        Assert.Null(await AccountAsync("new.user"));
    }

    [SkippableFact]
    public async Task Enroll_NewPasswordTooShortOrUnchanged_IsRefused()
    {
        RequireDatabase();

        using var factory = Factory();
        using var client = factory.CreateClient();
        await InvitationAsync(factory, "new.user");

        var body = await StartEnrollingAsync(client, "new.user", InitialPassword);
        var token = body.GetProperty("token").GetString();
        var code = AdminTestAccounts.Code(factory, body.GetProperty("secret").GetString()!);

        AssertInvalid(await EnrollAsync(client, token, code, "short"), "newPassword", "length");
        AssertInvalid(await EnrollAsync(client, token, code, null), "newPassword", "length");
        AssertInvalid(await EnrollAsync(client, token, code, new string('p', 129)), "newPassword", "length");
        AssertInvalid(await EnrollAsync(client, token, code, InitialPassword), "newPassword", "same");

        Assert.Null(await AccountAsync("new.user"));
    }

    [SkippableFact]
    public async Task Enroll_NewUser_CreatesTheAccountAndSignsIn()
    {
        RequireDatabase();

        using var factory = Factory();
        var now = Now(factory);
        using var client = factory.CreateClient();
        var invitation = await InvitationAsync(factory, "new.user");

        var body = await StartEnrollingAsync(client, "new.user", InitialPassword);
        var secret = body.GetProperty("secret").GetString()!;
        var code = AdminTestAccounts.Code(factory, secret);

        var (status, text) = await EnrollAsync(client, body.GetProperty("token").GetString(), code, ChosenPassword);

        Assert.Equal(HttpStatusCode.OK, status);

        var signedIn = JsonDocument.Parse(text).RootElement;
        Assert.True(signedIn.GetProperty("ok").GetBoolean());
        Assert.Equal("new.user", signedIn.GetProperty("username").GetString());
        Assert.Equal("New Person", signedIn.GetProperty("displayName").GetString());
        Assert.Equal(1800, signedIn.GetProperty("idleTimeoutSeconds").GetInt32());
        Assert.Equal(now.AddHours(12), signedIn.GetProperty("absoluteExpiresAt").GetDateTime().ToUniversalTime());

        var account = await AccountAsync("new.user");
        Assert.NotNull(account);
        Assert.Equal("New Person", account.DisplayName);
        Assert.Equal(secret, account.TotpSecret);
        Assert.Equal((ulong)AdminTestAccounts.Step(factory), account.TotpLastStep);
        Assert.Equal(now, account.LastLoginAt);
        Assert.False(account.Disabled);
        Assert.True(AdminTestAccounts.Verifies(account.PasswordHash, ChosenPassword));
        Assert.False(AdminTestAccounts.Verifies(account.PasswordHash, InitialPassword));

        await using (var db = Db())
        {
            Assert.False(await db.AdminEnrollments.AnyAsync(x => x.Id == invitation.Id));
        }

        // The returned token is a real session.
        using (var authed = factory.CreateClient())
        {
            authed.DefaultRequestHeaders.Add("Authorization", "Bearer " + signedIn.GetProperty("token").GetString());
            using var session = await authed.GetAsync("/api/admin/session");
            Assert.Equal(HttpStatusCode.OK, session.StatusCode);
        }

        // The initial password is gone, the enrolment code is spent, and the
        // next code works with the chosen password.
        Assert.Equal(HttpStatusCode.Unauthorized,
            await LoginAsync(client, "new.user", InitialPassword, AdminTestAccounts.Code(factory, secret, 1)));
        Assert.Equal(HttpStatusCode.Unauthorized, await LoginAsync(client, "new.user", ChosenPassword, code));
        Assert.Equal(HttpStatusCode.OK,
            await LoginAsync(client, "new.user", ChosenPassword, AdminTestAccounts.Code(factory, secret, 1)));

        // An enrolled account now: start gives it the default answer.
        Assert.Equal(DefaultBody, (await StartAsync(client, "new.user", ChosenPassword)).Text);
    }

    [SkippableFact]
    public async Task Enroll_Reset_RestoresTheAccountWithItsOwnPassword()
    {
        RequireDatabase();

        using var factory = Factory();
        var now = Now(factory);
        using var client = factory.CreateClient();

        var account = await AdminTestAccounts.CreateAsync(Db, factory, "emil", "Emil Huseynov", a =>
        {
            a.TotpSecret = "";
            a.FailedAttempts = 2;
            a.FirstFailedAt = now.AddMinutes(-3);
        });

        var reset = await ResetAsync(factory, account.Id);

        var body = await StartEnrollingAsync(client, "emil", account.Password);
        var secret = body.GetProperty("secret").GetString()!;
        var code = AdminTestAccounts.Code(factory, secret);

        // newPassword is ignored for a reset, even one that would be refused.
        var (status, text) = await EnrollAsync(client, body.GetProperty("token").GetString(), code, "x");

        Assert.Equal(HttpStatusCode.OK, status);
        Assert.Equal("Emil Huseynov", JsonDocument.Parse(text).RootElement.GetProperty("displayName").GetString());

        var after = await AccountAsync("emil");
        Assert.NotNull(after);
        Assert.Equal(account.Id, after.Id);
        Assert.Equal(secret, after.TotpSecret);
        Assert.Equal((ulong)AdminTestAccounts.Step(factory), after.TotpLastStep);
        Assert.Equal(0, after.FailedAttempts);
        Assert.Null(after.FirstFailedAt);
        Assert.Equal(now, after.LastLoginAt);
        Assert.True(AdminTestAccounts.Verifies(after.PasswordHash, account.Password));

        await using (var db = Db())
        {
            Assert.False(await db.AdminEnrollments.AnyAsync(x => x.Id == reset.Id));
        }

        Assert.Equal(HttpStatusCode.OK,
            await LoginAsync(client, "emil", account.Password, AdminTestAccounts.Code(factory, secret, 1)));
    }

    // Replay state belongs to a secret. A value pushed far ahead, by tampering
    // or otherwise, must not outlive the secret it was stored against, or the
    // reset meant to recover the account would leave every later code refused.
    [SkippableFact]
    public async Task Enroll_Reset_SetsTheLastStepToTheAcceptedStep()
    {
        RequireDatabase();

        using var factory = Factory();
        using var client = factory.CreateClient();
        var ahead = (ulong)AdminTestAccounts.Step(factory, 1_000_000);

        var account = await AdminTestAccounts.CreateAsync(Db, factory, "emil", "Emil", a =>
        {
            a.TotpSecret = "";
            a.TotpLastStep = ahead;
        });

        await ResetAsync(factory, account.Id);

        var body = await StartEnrollingAsync(client, "emil", account.Password);
        var secret = body.GetProperty("secret").GetString()!;
        var code = AdminTestAccounts.Code(factory, secret);

        Assert.Equal(HttpStatusCode.OK, (await EnrollAsync(client, body.GetProperty("token").GetString(), code, null)).Status);
        Assert.Equal((ulong)AdminTestAccounts.Step(factory), (await AccountAsync("emil"))!.TotpLastStep);

        // The next code signs in, which the old value would have refused.
        Assert.Equal(HttpStatusCode.OK,
            await LoginAsync(client, "emil", account.Password, AdminTestAccounts.Code(factory, secret, 1)));
    }

    [SkippableFact]
    public async Task Enroll_ResetOfAnAccountDisabledMeanwhile_IsStale()
    {
        RequireDatabase();

        using var factory = Factory();
        using var client = factory.CreateClient();
        var account = await AdminTestAccounts.CreateAsync(Db, factory, "emil", "Emil", a => a.TotpSecret = "");
        await ResetAsync(factory, account.Id);

        var body = await StartEnrollingAsync(client, "emil", account.Password);

        await using (var db = Db())
        {
            db.AdminDisables.Add(new AdminDisable { AccountId = account.Id, DisabledAt = Now(factory), DisabledBy = "x" });
            await db.SaveChangesAsync();
        }

        var result = await EnrollAsync(
            client,
            body.GetProperty("token").GetString(),
            AdminTestAccounts.Code(factory, body.GetProperty("secret").GetString()!),
            null);

        Assert.Equal(HttpStatusCode.Conflict, result.Status);
        Assert.Equal(string.Empty, (await AccountAsync("emil"))!.TotpSecret);
    }

    [SkippableFact]
    public async Task Enroll_NewUserWhoseNameWasTakenMeanwhile_IsStale()
    {
        RequireDatabase();

        using var factory = Factory();
        using var client = factory.CreateClient();
        await InvitationAsync(factory, "new.user");

        var body = await StartEnrollingAsync(client, "new.user", InitialPassword);
        var bySql = await AdminTestAccounts.CreateAsync(Db, factory, "new.user", "Made By SQL");

        var result = await EnrollAsync(
            client,
            body.GetProperty("token").GetString(),
            AdminTestAccounts.Code(factory, body.GetProperty("secret").GetString()!),
            ChosenPassword);

        Assert.Equal(HttpStatusCode.Conflict, result.Status);
        Assert.Equal(bySql.Secret, (await AccountAsync("new.user"))!.TotpSecret);
    }

    // A second reset, committed while a setup is between reading its token and
    // saving. The reset updates the enrolment in place, so the row enroll read
    // still has the same id and only its token is gone. Deleted by id alone,
    // enroll took the new reset with it and gave the account the secret from
    // before it, the one the second reset was clicked to kill. The hook runs the
    // whole reset, through the endpoint, just before enroll's transaction
    // reads the account.
    [SkippableFact]
    public async Task Enroll_ResetAgainBeforeTheSetupSaves_IsStaleAndTheNewResetStands()
    {
        RequireDatabase();

        var hook = new CommandHook("FROM `admin_accounts`");
        using var factory = Factory();
        factory.Interceptors.Add(hook);

        var op = await AdminTestAccounts.CreateAsync(Db, factory, "operator", "Operator");
        var account = await AdminTestAccounts.CreateAsync(Db, factory, "emil", "Emil", a => a.TotpSecret = "");
        var reset = await ResetAsync(factory, account.Id);

        using var operatorClient = await AdminTestAccounts.SignedInAsync(Db, factory, op);
        using var client = factory.CreateClient();

        var setup = await StartEnrollingAsync(client, "emil", account.Password);
        var oldSecret = setup.GetProperty("secret").GetString()!;

        HttpStatusCode? resetStatus = null;
        hook.Arm(async _ =>
        {
            using var response = await operatorClient.PostAsJsonAsync(
                "/api/admin/users/" + account.Id.ToString(CultureInfo.InvariantCulture)
                + "/reset-authenticator",
                new { code = AdminTestAccounts.Code(factory, op.Secret) });
            resetStatus = response.StatusCode;
        });

        var result = await EnrollAsync(
            client, setup.GetProperty("token").GetString(), AdminTestAccounts.Code(factory, oldSecret), null);

        Assert.Equal(1, hook.Fired);
        Assert.Equal(HttpStatusCode.OK, resetStatus);
        Assert.Equal(HttpStatusCode.Conflict, result.Status);
        Assert.Equal(StaleBody, result.Text);
        Assert.Equal(string.Empty, (await AccountAsync("emil"))!.TotpSecret);

        await using (var db = Db())
        {
            var row = await db.AdminEnrollments.AsNoTracking().SingleAsync(x => x.AccountId == account.Id);
            Assert.Equal(reset.Id, row.Id);
            Assert.Null(row.TokenHash);
            Assert.False(await db.AdminSessions.AnyAsync(x => x.AccountId == account.Id));
        }

        // The reset that stands is the one that works.
        var again = await StartEnrollingAsync(client, "emil", account.Password);
        var newSecret = again.GetProperty("secret").GetString()!;
        Assert.NotEqual(oldSecret, newSecret);

        var enrolled = await EnrollAsync(
            client, again.GetProperty("token").GetString(), AdminTestAccounts.Code(factory, newSecret), null);
        Assert.Equal(HttpStatusCode.OK, enrolled.Status);
        Assert.Equal(newSecret, (await AccountAsync("emil"))!.TotpSecret);
    }

    // The other order. The second reset reads the account while the setup is
    // still waiting, the setup completes, and only then does the reset's
    // transaction run. The account was read with an empty secret, so unless
    // the reset writes the empty secret anyway, the setup it was clicked to
    // kill keeps working. The hook runs the whole enroll, through the endpoint,
    // just before the reset's transaction writes the account.
    [SkippableFact]
    public async Task Reset_OfASetupThatCompletesMeanwhile_StillTakesTheSecretAway()
    {
        RequireDatabase();

        var hook = new CommandHook("UPDATE `admin_accounts`");
        using var factory = Factory();
        factory.Interceptors.Add(hook);

        var op = await AdminTestAccounts.CreateAsync(Db, factory, "operator", "Operator");
        var account = await AdminTestAccounts.CreateAsync(Db, factory, "emil", "Emil", a => a.TotpSecret = "");
        await ResetAsync(factory, account.Id);

        using var operatorClient = await AdminTestAccounts.SignedInAsync(Db, factory, op);
        using var client = factory.CreateClient();

        var setup = await StartEnrollingAsync(client, "emil", account.Password);
        var secret = setup.GetProperty("secret").GetString()!;

        HttpStatusCode? enrollStatus = null;
        hook.Arm(async _ => enrollStatus = (await EnrollAsync(
            client, setup.GetProperty("token").GetString(), AdminTestAccounts.Code(factory, secret), null)).Status);

        using var reset = await operatorClient.PostAsJsonAsync(
            "/api/admin/users/" + account.Id.ToString(CultureInfo.InvariantCulture) + "/reset-authenticator",
            new { code = AdminTestAccounts.Code(factory, op.Secret) });

        Assert.Equal(1, hook.Fired);
        Assert.Equal(HttpStatusCode.OK, enrollStatus);
        Assert.Equal(HttpStatusCode.OK, reset.StatusCode);
        Assert.Equal(string.Empty, (await AccountAsync("emil"))!.TotpSecret);

        await using (var db = Db())
        {
            Assert.Equal(1, await db.AdminEnrollments.CountAsync(x => x.AccountId == account.Id));
            Assert.False(await db.AdminSessions.AnyAsync(x => x.AccountId == account.Id));
        }

        Assert.Equal(HttpStatusCode.Unauthorized,
            await LoginAsync(client, "emil", account.Password, AdminTestAccounts.Code(factory, secret, 1)));
    }

    // The same for a new user, where what replaces the token is a second start
    // on the same invitation. The setup screen that read the old token must not
    // create the account, or the QR code a second start is meant to kill would
    // still work.
    [SkippableFact]
    public async Task Enroll_StartedAgainBeforeTheSetupSaves_IsStale()
    {
        RequireDatabase();

        var hook = new CommandHook("FROM `admin_accounts`");
        using var factory = Factory();
        factory.Interceptors.Add(hook);
        using var client = factory.CreateClient();
        await InvitationAsync(factory, "new.user");

        var first = await StartEnrollingAsync(client, "new.user", InitialPassword);

        JsonElement second = default;
        hook.Arm(async _ => second = await StartEnrollingAsync(client, "new.user", InitialPassword));

        var result = await EnrollAsync(
            client,
            first.GetProperty("token").GetString(),
            AdminTestAccounts.Code(factory, first.GetProperty("secret").GetString()!),
            ChosenPassword);

        Assert.Equal(1, hook.Fired);
        Assert.Equal(HttpStatusCode.Conflict, result.Status);
        Assert.Equal(StaleBody, result.Text);
        Assert.Null(await AccountAsync("new.user"));

        var enrolled = await EnrollAsync(
            client,
            second.GetProperty("token").GetString(),
            AdminTestAccounts.Code(factory, second.GetProperty("secret").GetString()!),
            ChosenPassword);
        Assert.Equal(HttpStatusCode.OK, enrolled.Status);
    }

    // ------------------------------------------------------------------ login

    // After a reset the account has totp_secret = '' and must not be able to
    // sign in through /login with any code at all. HMAC-SHA1 accepts an empty
    // key, so the codes that matter are the ones an empty secret really has,
    // which anyone can compute, at every step the skew accepts.
    [SkippableFact]
    public async Task Login_AccountWithNoAuthenticator_NeverSignsIn()
    {
        RequireDatabase();

        using var factory = Factory();
        using var client = factory.CreateClient();
        var account = await AdminTestAccounts.CreateAsync(Db, factory, "emil", "Emil", a => a.TotpSecret = "");

        var codes = new List<string> { "000000", "123456", AdminTestAccounts.Code(factory, Totp.NewSecret()) };
        for (var k = -1; k <= 1; k++)
        {
            codes.Add(Totp.Compute(Array.Empty<byte>(), AdminTestAccounts.Step(factory, k)));
        }

        foreach (var code in codes)
        {
            Assert.Equal(HttpStatusCode.Unauthorized, await LoginAsync(client, "emil", account.Password, code));
        }

        var after = await AccountAsync("emil");
        Assert.Equal(0UL, after!.TotpLastStep);
        Assert.Equal(string.Empty, after.TotpSecret);
    }

    // A mistyped password on a reset account reaches /login too, because the
    // panel moves on to the code step after every enroll:false. Start is the
    // one place that counts it, so one mistake costs one attempt, not two.
    [SkippableFact]
    public async Task ResetAccount_WrongPasswordAtStartThenLogin_CountsOnce()
    {
        RequireDatabase();

        using var factory = Factory();
        using var client = factory.CreateClient();
        var account = await AdminTestAccounts.CreateAsync(Db, factory, "emil", "Emil", a => a.TotpSecret = "");
        await ResetAsync(factory, account.Id);

        Assert.Equal(DefaultBody, (await StartAsync(client, "emil", "not the password")).Text);
        Assert.Equal(HttpStatusCode.Unauthorized, await LoginAsync(client, "emil", "not the password", "123456"));

        Assert.Equal(1, (await AccountAsync("emil"))!.FailedAttempts);
    }

    // Counted before the check, so a success has to clear the count it was
    // charged, and a locked account is refused without being counted again.
    [SkippableFact]
    public async Task Login_CountsWrongCodes_ClearsOnSuccess_AndRefusesWhenLocked()
    {
        RequireDatabase();

        using var factory = Factory();
        var options = new AdminOptions();
        using var client = factory.CreateClient();
        var account = await AdminTestAccounts.CreateAsync(Db, factory, "emil", "Emil");
        var wrong = AdminTestAccounts.WrongCode(factory, account.Secret);

        Assert.Equal(HttpStatusCode.Unauthorized, await LoginAsync(client, "emil", account.Password, wrong));
        Assert.Equal(HttpStatusCode.Unauthorized, await LoginAsync(client, "emil", "not the password", wrong));
        Assert.Equal(2, (await AccountAsync("emil"))!.FailedAttempts);

        Assert.Equal(HttpStatusCode.OK,
            await LoginAsync(client, "emil", account.Password, AdminTestAccounts.Code(factory, account.Secret)));

        var cleared = await AccountAsync("emil");
        Assert.Equal(0, cleared!.FailedAttempts);
        Assert.Null(cleared.FirstFailedAt);
        Assert.Null(cleared.LockedUntil);

        for (var i = 0; i < options.MaxFailedAttempts; i++)
        {
            Assert.Equal(HttpStatusCode.Unauthorized, await LoginAsync(client, "emil", account.Password, wrong));
        }

        Assert.Equal(HttpStatusCode.Unauthorized,
            await LoginAsync(client, "emil", account.Password, AdminTestAccounts.Code(factory, account.Secret, 1)));

        var locked = await AccountAsync("emil");
        Assert.Equal(options.MaxFailedAttempts, locked!.FailedAttempts);
        Assert.NotNull(locked.LockedUntil);
        Assert.Equal((ulong)AdminTestAccounts.Step(factory), locked.TotpLastStep);
    }

    [SkippableFact]
    public async Task Login_DisabledInAdminDisables_IsRefused()
    {
        RequireDatabase();

        using var factory = Factory();
        using var client = factory.CreateClient();
        var account = await AdminTestAccounts.CreateAsync(Db, factory, "emil", "Emil");

        await using (var db = Db())
        {
            db.AdminDisables.Add(new AdminDisable { AccountId = account.Id, DisabledAt = Now(factory), DisabledBy = "x" });
            await db.SaveChangesAsync();
        }

        Assert.Equal(HttpStatusCode.Unauthorized,
            await LoginAsync(client, "emil", account.Password, AdminTestAccounts.Code(factory, account.Secret)));
        Assert.Equal(0UL, (await AccountAsync("emil"))!.TotpLastStep);
    }
}
