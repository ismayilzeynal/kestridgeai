using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Kestridge.Api.Admin;
using Kestridge.Api.Data;
using Microsoft.EntityFrameworkCore;
using MimeKit;

namespace Kestridge.Api.Tests;

// The users area of the panel: list, create, reset an authenticator, disable,
// and delete a user who never signed in. First sign-in itself is AdminEnrollTests.
public class AdminUserTests(MySqlFixture fixture) : DatabaseTestBase(fixture)
{
    private const string OperatorName = "Test Operator";
    private const string InitialPassword = "initial password 1";

    private ApiFactory Factory() => CreateFactory(AdminTestAccounts.FactorySettings());

    private static DateTime Now(ApiFactory factory) => factory.Clock.GetUtcNow().UtcDateTime;

    private async Task<(SeededAccount Account, HttpClient Client)> OperatorAsync(
        ApiFactory factory, Action<AdminAccount>? configure = null)
    {
        var account = await AdminTestAccounts.CreateAsync(Db, factory, "operator", OperatorName, configure);
        return (account, await AdminTestAccounts.SignedInAsync(Db, factory, account));
    }

    private static object CreateBody(string code, string username = "new.user", string displayName = "New Person",
        string password = InitialPassword)
        => new { username, displayName, password, code };

    private static async Task<(HttpStatusCode Status, JsonElement Body)> PostAsync(
        HttpClient client, string path, object? body = null)
    {
        using var response = body is null
            ? await client.PostAsync(path, null)
            : await client.PostAsJsonAsync(path, body);

        var text = await response.Content.ReadAsStringAsync();
        return (response.StatusCode, text.Length == 0 ? default : JsonDocument.Parse(text).RootElement.Clone());
    }

    private static void AssertInvalid((HttpStatusCode Status, JsonElement Body) result, string field, string? reason = null)
    {
        Assert.Equal(HttpStatusCode.BadRequest, result.Status);
        Assert.False(result.Body.GetProperty("ok").GetBoolean());
        Assert.Equal("invalid", result.Body.GetProperty("error").GetString());
        Assert.Equal(field, result.Body.GetProperty("field").GetString());

        if (reason is not null)
        {
            Assert.Equal(reason, result.Body.GetProperty("reason").GetString());
        }
    }

    private static string Id(long id) => id.ToString(CultureInfo.InvariantCulture);

    private async Task<AdminAccount> AccountAsync(long id)
    {
        await using var db = Db();
        return await db.AdminAccounts.AsNoTracking().SingleAsync(x => x.Id == id);
    }

    private async Task<AdminEnrollment> SeedEnrollmentAsync(Action<AdminEnrollment> configure)
    {
        await using var db = Db();

        var row = new AdminEnrollment { DisplayName = "Someone", CreatedBy = OperatorName };
        configure(row);

        db.AdminEnrollments.Add(row);
        await db.SaveChangesAsync();
        return row;
    }

    // ----------------------------------------------------------------- create

    [SkippableFact]
    public async Task Create_WritesAnEnrolmentAndMailsTheTeamOnce()
    {
        RequireDatabase();

        using var factory = Factory();
        var now = Now(factory);

        // Two earlier wrong codes, which a good one clears, as a sign-in does.
        var (op, client) = await OperatorAsync(factory, a =>
        {
            a.FailedAttempts = 2;
            a.FirstFailedAt = now.AddMinutes(-5);
        });

        var code = AdminTestAccounts.Code(factory, op.Secret);

        var result = await PostAsync(client, "/api/admin/users/create", CreateBody(code));

        Assert.Equal(HttpStatusCode.OK, result.Status);
        Assert.True(result.Body.GetProperty("ok").GetBoolean());
        Assert.True(result.Body.GetProperty("notified").GetBoolean());

        var pending = result.Body.GetProperty("pending");
        Assert.True(pending.GetProperty("id").GetInt64() > 0);
        Assert.Equal("new.user", pending.GetProperty("username").GetString());
        Assert.Equal("New Person", pending.GetProperty("displayName").GetString());
        Assert.Equal(OperatorName, pending.GetProperty("createdBy").GetString());
        Assert.Equal(now, pending.GetProperty("createdAt").GetDateTime().ToUniversalTime());
        Assert.Equal(now.AddHours(72), pending.GetProperty("expiresAt").GetDateTime().ToUniversalTime());
        Assert.False(pending.GetProperty("expired").GetBoolean());
        Assert.Equal(JsonValueKind.Null, pending.GetProperty("lockedUntil").ValueKind);

        await using (var db = Db())
        {
            var row = await db.AdminEnrollments.AsNoTracking().SingleAsync();

            Assert.Null(row.AccountId);
            Assert.Equal("new.user", row.Username);
            Assert.Equal("New Person", row.DisplayName);
            Assert.Equal(OperatorName, row.CreatedBy);
            Assert.Equal(op.Id, row.CreatedByAccountId);
            Assert.Equal(now.AddHours(72), row.ExpiresAt);
            Assert.True(AdminTestAccounts.Verifies(row.PasswordHash, InitialPassword));
            Assert.Equal(string.Empty, row.TotpSecret);
            Assert.Null(row.TokenHash);

            // Nothing is in admin_accounts until first sign-in completes.
            Assert.False(await db.AdminAccounts.AnyAsync(x => x.Username == "new.user"));
        }

        // The code is spent, and the step-up left no count behind.
        var after = await AccountAsync(op.Id);
        Assert.Equal((ulong)AdminTestAccounts.Step(factory), after.TotpLastStep);
        Assert.Equal(0, after.FailedAttempts);
        Assert.Null(after.FirstFailedAt);

        var mail = Assert.Single(factory.Email.Sent);
        Assert.Equal("team@kestridge.test", Assert.IsType<MailboxAddress>(Assert.Single(mail.To)).Address);
        Assert.Contains("new.user", mail.Subject, StringComparison.Ordinal);

        var body = mail.TextBody ?? string.Empty;
        Assert.DoesNotContain(InitialPassword, body, StringComparison.Ordinal);
        Assert.DoesNotContain(code, body, StringComparison.Ordinal);
        Assert.DoesNotContain(op.Secret, body, StringComparison.Ordinal);
    }

    [SkippableFact]
    public async Task Create_WrongCode_Is400OnCodeAndCountsAFailure()
    {
        RequireDatabase();

        using var factory = Factory();
        var (op, client) = await OperatorAsync(factory);

        var result = await PostAsync(client, "/api/admin/users/create",
            CreateBody(AdminTestAccounts.WrongCode(factory, op.Secret)));

        AssertInvalid(result, "code");
        Assert.Equal(1, (await AccountAsync(op.Id)).FailedAttempts);

        await using (var db = Db())
        {
            Assert.False(await db.AdminEnrollments.AnyAsync());
        }

        Assert.Empty(factory.Email.Sent);

        // A refusal is never a 401: the session is still good.
        using var session = await client.GetAsync("/api/admin/session");
        Assert.Equal(HttpStatusCode.OK, session.StatusCode);
    }

    [SkippableFact]
    public async Task Create_ReplayedCode_IsRefused()
    {
        RequireDatabase();

        using var factory = Factory();
        var (op, client) = await OperatorAsync(factory);
        var code = AdminTestAccounts.Code(factory, op.Secret);

        Assert.Equal(HttpStatusCode.OK, (await PostAsync(client, "/api/admin/users/create", CreateBody(code))).Status);

        var replay = await PostAsync(client, "/api/admin/users/create", CreateBody(code, username: "second.user"));

        AssertInvalid(replay, "code");

        await using var db = Db();
        Assert.False(await db.AdminEnrollments.AnyAsync(x => x.Username == "second.user"));
    }

    [SkippableFact]
    public async Task Create_ByALockedOperator_IsRefusedEvenWithTheRightCode()
    {
        RequireDatabase();

        using var factory = Factory();
        var now = Now(factory);
        var (op, client) = await OperatorAsync(factory, a => a.LockedUntil = now.AddMinutes(10));

        var result = await PostAsync(client, "/api/admin/users/create",
            CreateBody(AdminTestAccounts.Code(factory, op.Secret)));

        AssertInvalid(result, "code");

        var after = await AccountAsync(op.Id);
        Assert.Equal(0UL, after.TotpLastStep);
        Assert.Equal(0, after.FailedAttempts);

        await using var db = Db();
        Assert.False(await db.AdminEnrollments.AnyAsync());
    }

    [SkippableFact]
    public async Task Create_RefusesEachInvalidField_BeforeSpendingTheCode()
    {
        RequireDatabase();

        using var factory = Factory();
        var (op, client) = await OperatorAsync(factory);
        var code = AdminTestAccounts.Code(factory, op.Secret);
        const string path = "/api/admin/users/create";

        AssertInvalid(await PostAsync(client, path, CreateBody(code, username: "a")), "username", "format");
        AssertInvalid(await PostAsync(client, path, CreateBody(code, username: "-emil")), "username", "format");
        AssertInvalid(await PostAsync(client, path, CreateBody(code, username: "em il")), "username", "format");
        AssertInvalid(await PostAsync(client, path, CreateBody(code, displayName: " ")), "displayName", "length");
        AssertInvalid(await PostAsync(client, path, CreateBody(code, displayName: new string('x', 65))), "displayName", "length");
        AssertInvalid(await PostAsync(client, path, CreateBody(code, displayName: "a <b>")), "displayName", "angle");
        AssertInvalid(await PostAsync(client, path, CreateBody(code, password: "eleven char")), "password", "length");
        AssertInvalid(await PostAsync(client, path, CreateBody(code, password: new string('p', 129))), "password", "length");

        // None of those reached the step-up, so the same code still works.
        Assert.Equal(0UL, (await AccountAsync(op.Id)).TotpLastStep);
        Assert.Equal(HttpStatusCode.OK, (await PostAsync(client, path, CreateBody(code))).Status);
    }

    [SkippableFact]
    public async Task Create_UsernameOfAnExistingAccount_IsTaken()
    {
        RequireDatabase();

        using var factory = Factory();
        var (op, client) = await OperatorAsync(factory);
        await AdminTestAccounts.CreateAsync(Db, factory, "taken.name", "Already Here");

        // Normalised before the comparison, as login normalises.
        var result = await PostAsync(client, "/api/admin/users/create",
            CreateBody(AdminTestAccounts.Code(factory, op.Secret), username: "  Taken.Name "));

        AssertInvalid(result, "username", "taken");
        Assert.Empty(factory.Email.Sent);
    }

    [SkippableFact]
    public async Task Create_UsernameOfALivePendingUser_IsTaken()
    {
        RequireDatabase();

        using var factory = Factory();
        var (op, client) = await OperatorAsync(factory);

        var first = await PostAsync(client, "/api/admin/users/create",
            CreateBody(AdminTestAccounts.Code(factory, op.Secret), username: "dup"));
        Assert.Equal(HttpStatusCode.OK, first.Status);

        factory.Clock.Advance(TimeSpan.FromSeconds(Totp.StepSeconds));

        var second = await PostAsync(client, "/api/admin/users/create",
            CreateBody(AdminTestAccounts.Code(factory, op.Secret), username: "dup", displayName: "Other Person"));

        AssertInvalid(second, "username", "taken");

        await using var db = Db();
        Assert.Equal("New Person", (await db.AdminEnrollments.AsNoTracking().SingleAsync()).DisplayName);
    }

    [SkippableFact]
    public async Task Create_ReplacesAnExpiredPendingUserOfTheSameName()
    {
        RequireDatabase();

        using var factory = Factory();
        var (op, client) = await OperatorAsync(factory);
        var now = Now(factory);

        var old = await SeedEnrollmentAsync(e =>
        {
            e.Username = "again";
            e.DisplayName = "Old Invitation";
            e.PasswordHash = AdminTestAccounts.Hash("an old password");
            e.CreatedAt = now.AddHours(-80);
            e.ExpiresAt = now.AddHours(-8);
        });

        var result = await PostAsync(client, "/api/admin/users/create",
            CreateBody(AdminTestAccounts.Code(factory, op.Secret), username: "again"));

        Assert.Equal(HttpStatusCode.OK, result.Status);

        await using var db = Db();
        var row = await db.AdminEnrollments.AsNoTracking().SingleAsync(x => x.Username == "again");

        Assert.NotEqual(old.Id, row.Id);
        Assert.Equal("New Person", row.DisplayName);
        Assert.Equal(now.AddHours(72), row.ExpiresAt);
    }

    [SkippableFact]
    public async Task Create_WhenMailFails_StillSucceedsAndSaysSo()
    {
        RequireDatabase();

        using var factory = Factory();
        factory.Email.FailWith = _ => new InvalidOperationException("smtp down at mail.example.test");
        var (op, client) = await OperatorAsync(factory);

        var result = await PostAsync(client, "/api/admin/users/create",
            CreateBody(AdminTestAccounts.Code(factory, op.Secret)));

        Assert.Equal(HttpStatusCode.OK, result.Status);
        Assert.False(result.Body.GetProperty("notified").GetBoolean());

        await using (var db = Db())
        {
            Assert.True(await db.AdminEnrollments.AnyAsync(x => x.Username == "new.user"));
        }

        Assert.Contains(factory.Logs.Lines, l => l.Contains("admin.notice_failed kind=user_created", StringComparison.Ordinal));
        Assert.DoesNotContain(factory.Logs.Lines, l => l.Contains("mail.example.test", StringComparison.Ordinal));
    }

    // ---------------------------------------------------- reset authenticator

    [SkippableFact]
    public async Task Reset_OfYourself_IsRefused()
    {
        RequireDatabase();

        using var factory = Factory();
        var (op, client) = await OperatorAsync(factory);

        var result = await PostAsync(client, "/api/admin/users/" + Id(op.Id) + "/reset-authenticator",
            new { code = AdminTestAccounts.Code(factory, op.Secret) });

        AssertInvalid(result, "self");
        Assert.Equal(op.Secret, (await AccountAsync(op.Id)).TotpSecret);
    }

    [SkippableFact]
    public async Task Reset_OfADisabledAccount_IsRefused()
    {
        RequireDatabase();

        using var factory = Factory();
        var (op, client) = await OperatorAsync(factory);

        var byColumn = await AdminTestAccounts.CreateAsync(Db, factory, "by.column", "Column", a => a.Disabled = true);
        var byPanel = await AdminTestAccounts.CreateAsync(Db, factory, "by.panel", "Panel");

        await using (var db = Db())
        {
            db.AdminDisables.Add(new AdminDisable { AccountId = byPanel.Id, DisabledAt = Now(factory), DisabledBy = "x" });
            await db.SaveChangesAsync();
        }

        AssertInvalid(await PostAsync(client, "/api/admin/users/" + Id(byColumn.Id) + "/reset-authenticator",
            new { code = AdminTestAccounts.Code(factory, op.Secret) }), "disabled");
        AssertInvalid(await PostAsync(client, "/api/admin/users/" + Id(byPanel.Id) + "/reset-authenticator",
            new { code = AdminTestAccounts.Code(factory, op.Secret) }), "disabled");

        Assert.Equal(byPanel.Secret, (await AccountAsync(byPanel.Id)).TotpSecret);
    }

    [SkippableFact]
    public async Task Reset_Unknown_Is404()
    {
        RequireDatabase();

        using var factory = Factory();
        var (op, client) = await OperatorAsync(factory);

        var result = await PostAsync(client, "/api/admin/users/999999/reset-authenticator",
            new { code = AdminTestAccounts.Code(factory, op.Secret) });

        Assert.Equal(HttpStatusCode.NotFound, result.Status);
    }

    [SkippableFact]
    public async Task Reset_RequiresAFreshCode()
    {
        RequireDatabase();

        using var factory = Factory();
        var (op, client) = await OperatorAsync(factory);
        var target = await AdminTestAccounts.CreateAsync(Db, factory, "target", "Target Person");

        var result = await PostAsync(client, "/api/admin/users/" + Id(target.Id) + "/reset-authenticator",
            new { code = AdminTestAccounts.WrongCode(factory, op.Secret) });

        AssertInvalid(result, "code");
        Assert.Equal(target.Secret, (await AccountAsync(target.Id)).TotpSecret);
        Assert.Empty(factory.Email.Sent);
    }

    [SkippableFact]
    public async Task Reset_ClearsTheSecretEndsSessionsAndTheOldCodeStopsWorking()
    {
        RequireDatabase();

        using var factory = Factory();
        var now = Now(factory);
        var (op, client) = await OperatorAsync(factory);

        var target = await AdminTestAccounts.CreateAsync(Db, factory, "target", "Target Person", a =>
        {
            a.FailedAttempts = 3;
            a.FirstFailedAt = now.AddMinutes(-5);
            a.TotpLastStep = 5;
        });

        using var targetClient = await AdminTestAccounts.SignedInAsync(Db, factory, target);

        // A leftover reset from earlier, with a setup started on it, which the
        // new one replaces in place.
        var leftover = await SeedEnrollmentAsync(e =>
        {
            e.AccountId = target.Id;
            e.CreatedBy = "Someone Else";
            e.CreatedAt = now.AddHours(-100);
            e.ExpiresAt = now.AddHours(-28);
            e.TokenHash = AdminSessions.Hash(AdminSessions.NewToken());
            e.TokenExpiresAt = now.AddHours(-99);
            e.TotpSecret = Totp.NewSecret();
            e.FailedAttempts = 4;
            e.FirstFailedAt = now.AddHours(-99);
            e.LockedUntil = now.AddHours(-98);
        });

        var result = await PostAsync(client, "/api/admin/users/" + Id(target.Id) + "/reset-authenticator",
            new { code = AdminTestAccounts.Code(factory, op.Secret) });

        Assert.Equal(HttpStatusCode.OK, result.Status);
        Assert.True(result.Body.GetProperty("ok").GetBoolean());
        Assert.True(result.Body.GetProperty("notified").GetBoolean());

        var after = await AccountAsync(target.Id);
        Assert.Equal(string.Empty, after.TotpSecret);
        Assert.Equal(0, after.FailedAttempts);
        Assert.Null(after.FirstFailedAt);
        Assert.Null(after.LockedUntil);
        Assert.Equal(5UL, after.TotpLastStep);
        Assert.True(AdminTestAccounts.Verifies(after.PasswordHash, target.Password));

        await using (var db = Db())
        {
            Assert.False(await db.AdminSessions.AnyAsync(x => x.AccountId == target.Id));

            var enrollment = await db.AdminEnrollments.AsNoTracking().SingleAsync();
            Assert.Equal(leftover.Id, enrollment.Id);
            Assert.Equal(target.Id, enrollment.AccountId);
            Assert.Null(enrollment.Username);
            Assert.Equal("Target Person", enrollment.DisplayName);
            Assert.Equal(string.Empty, enrollment.PasswordHash);
            Assert.Equal(OperatorName, enrollment.CreatedBy);
            Assert.Equal(op.Id, enrollment.CreatedByAccountId);
            Assert.Equal(now, enrollment.CreatedAt);
            Assert.Equal(now.AddHours(72), enrollment.ExpiresAt);
            Assert.Null(enrollment.TokenHash);
            Assert.Null(enrollment.TokenExpiresAt);
            Assert.Equal(string.Empty, enrollment.TotpSecret);
            Assert.Equal(0, enrollment.FailedAttempts);
            Assert.Null(enrollment.FirstFailedAt);
            Assert.Null(enrollment.LockedUntil);
        }

        using (var session = await targetClient.GetAsync("/api/admin/session"))
        {
            Assert.Equal(HttpStatusCode.Unauthorized, session.StatusCode);
        }

        using (var own = await client.GetAsync("/api/admin/session"))
        {
            Assert.Equal(HttpStatusCode.OK, own.StatusCode);
        }

        using var anonymous = factory.CreateClient();
        using var login = await anonymous.PostAsJsonAsync("/api/admin/login", new
        {
            username = target.Username,
            password = target.Password,
            code = AdminTestAccounts.Code(factory, target.Secret, 1),
        });

        Assert.Equal(HttpStatusCode.Unauthorized, login.StatusCode);

        var mail = Assert.Single(factory.Email.Sent);
        Assert.Equal("Kestridge admin: authenticator reset for target", mail.Subject);
        Assert.DoesNotContain(target.Secret, mail.TextBody ?? string.Empty, StringComparison.Ordinal);
    }

    // Resets clicked at the same moment. Of different accounts, they used to
    // deadlock on the gap lock a DELETE of a missing enrolment takes, and one
    // answered 500. Of the same account, the second insert hit the unique
    // index. Every one of them now answers 200 and leaves one enrolment per
    // account.
    [SkippableFact]
    public async Task Reset_ConcurrentResets_AllSucceed()
    {
        RequireDatabase();

        using var factory = Factory();
        const int pairs = 6;

        var operators = new List<(SeededAccount Account, HttpClient Client)>();
        var targets = new List<SeededAccount>();

        for (var i = 0; i < pairs + 2; i++)
        {
            var name = "op" + Id(i);
            var account = await AdminTestAccounts.CreateAsync(Db, factory, name, "Operator " + Id(i));
            operators.Add((account, await AdminTestAccounts.SignedInAsync(Db, factory, account)));
        }

        for (var i = 0; i < pairs + 1; i++)
        {
            targets.Add(await AdminTestAccounts.CreateAsync(Db, factory, "target" + Id(i), "Target " + Id(i)));
        }

        Task<(HttpStatusCode Status, JsonElement Body)> Reset(int op, SeededAccount target)
            => PostAsync(operators[op].Client, "/api/admin/users/" + Id(target.Id) + "/reset-authenticator",
                new { code = AdminTestAccounts.Code(factory, operators[op].Account.Secret) });

        // One reset per target, and the last target reset by two operators.
        var requests = Enumerable.Range(0, pairs).Select(i => Reset(i, targets[i]))
            .Append(Reset(pairs, targets[pairs]))
            .Append(Reset(pairs + 1, targets[pairs]))
            .ToList();

        foreach (var (status, _) in await Task.WhenAll(requests))
        {
            Assert.Equal(HttpStatusCode.OK, status);
        }

        await using var db = Db();

        foreach (var target in targets)
        {
            Assert.Equal(1, await db.AdminEnrollments.CountAsync(x => x.AccountId == target.Id));
            Assert.Equal(string.Empty, (await db.AdminAccounts.AsNoTracking().SingleAsync(x => x.Id == target.Id)).TotpSecret);
        }

        foreach (var pair in operators)
        {
            pair.Client.Dispose();
        }
    }

    // A race lost on both attempts is the one reset that answers 409, and
    // nothing else in the log would say it happened. The hook swaps each
    // enrolment insert for a SIGNAL of the server error, so the error comes
    // back from MySQL the way a real one does, wrapped however EF wraps it.
    [SkippableTheory]
    [InlineData(1062, "23000")]
    [InlineData(1213, "40001")]
    public async Task Reset_RaceLostTwice_Is409_ChangesNothing_AndLogsTheIdOnly(int errorNumber, string sqlState)
    {
        RequireDatabase();

        var hook = new CommandHook("INSERT INTO `admin_enrollments`");
        using var factory = Factory();
        factory.Interceptors.Add(hook);

        var (op, client) = await OperatorAsync(factory);
        var target = await AdminTestAccounts.CreateAsync(Db, factory, "target", "Target Person");
        using var targetClient = await AdminTestAccounts.SignedInAsync(Db, factory, target);

        hook.Arm(
            command =>
            {
                command.CommandText = "SIGNAL SQLSTATE '" + sqlState + "' SET MYSQL_ERRNO = "
                    + Id(errorNumber) + ", MESSAGE_TEXT = 'lost race'";
                return Task.CompletedTask;
            },
            times: 2);

        var result = await PostAsync(client, "/api/admin/users/" + Id(target.Id) + "/reset-authenticator",
            new { code = AdminTestAccounts.Code(factory, op.Secret) });

        Assert.Equal(2, hook.Fired);
        Assert.Equal(HttpStatusCode.Conflict, result.Status);
        Assert.Equal("stale", result.Body.GetProperty("error").GetString());

        Assert.Equal(target.Secret, (await AccountAsync(target.Id)).TotpSecret);

        await using (var db = Db())
        {
            Assert.False(await db.AdminEnrollments.AnyAsync());
            Assert.True(await db.AdminSessions.AnyAsync(x => x.AccountId == target.Id));
        }

        Assert.Empty(factory.Email.Sent);

        var line = Assert.Single(factory.Logs.Lines, l => l.Contains("admin.reset_conflict", StringComparison.Ordinal));
        Assert.Equal("Kestridge.Api.Data.AdminAccount Warning admin.reset_conflict account=" + Id(target.Id) + " ", line);
        Assert.DoesNotContain(factory.Logs.Lines, l => l.Contains("admin.authenticator_reset", StringComparison.Ordinal));
    }

    // ---------------------------------------------------------------- disable

    [SkippableFact]
    public async Task Disable_OfYourself_IsRefused()
    {
        RequireDatabase();

        using var factory = Factory();
        var (op, client) = await OperatorAsync(factory);

        AssertInvalid(await PostAsync(client, "/api/admin/users/" + Id(op.Id) + "/disable"), "self");

        await using var db = Db();
        Assert.False(await db.AdminDisables.AnyAsync());
    }

    [SkippableFact]
    public async Task Disable_Unknown_Is404()
    {
        RequireDatabase();

        using var factory = Factory();
        var (_, client) = await OperatorAsync(factory);

        Assert.Equal(HttpStatusCode.NotFound, (await PostAsync(client, "/api/admin/users/999999/disable")).Status);
    }

    [SkippableFact]
    public async Task Disable_EndsTheSessionBlocksSignInAndIsIdempotent()
    {
        RequireDatabase();

        using var factory = Factory();
        var now = Now(factory);
        var (_, client) = await OperatorAsync(factory);
        var target = await AdminTestAccounts.CreateAsync(Db, factory, "target", "Target Person");
        using var targetClient = await AdminTestAccounts.SignedInAsync(Db, factory, target);

        await SeedEnrollmentAsync(e =>
        {
            e.AccountId = target.Id;
            e.CreatedAt = now;
            e.ExpiresAt = now.AddHours(72);
        });

        var first = await PostAsync(client, "/api/admin/users/" + Id(target.Id) + "/disable");
        Assert.Equal(HttpStatusCode.OK, first.Status);
        Assert.True(first.Body.GetProperty("ok").GetBoolean());

        using (var session = await targetClient.GetAsync("/api/admin/session"))
        {
            Assert.Equal(HttpStatusCode.Unauthorized, session.StatusCode);
        }

        await using (var db = Db())
        {
            var row = await db.AdminDisables.AsNoTracking().SingleAsync();
            Assert.Equal(target.Id, row.AccountId);
            Assert.Equal(OperatorName, row.DisabledBy);
            Assert.Equal(now, row.DisabledAt);

            Assert.False(await db.AdminSessions.AnyAsync(x => x.AccountId == target.Id));
            Assert.False(await db.AdminEnrollments.AnyAsync(x => x.AccountId == target.Id));

            // The column is untouched. The app has no UPDATE on it.
            Assert.False((await db.AdminAccounts.AsNoTracking().SingleAsync(x => x.Id == target.Id)).Disabled);
        }

        using var anonymous = factory.CreateClient();

        using (var login = await anonymous.PostAsJsonAsync("/api/admin/login", new
        {
            username = target.Username,
            password = target.Password,
            code = AdminTestAccounts.Code(factory, target.Secret),
        }))
        {
            Assert.Equal(HttpStatusCode.Unauthorized, login.StatusCode);
        }

        // Start answers an account with a secret the default body whatever its
        // state, so that alone would prove nothing. Put the disabled account
        // back into a live reset, the one state where start hands out a setup
        // token for the right password, and it must still be refused.
        await using (var db = Db())
        {
            await db.AdminAccounts.Where(x => x.Id == target.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.TotpSecret, string.Empty));
        }

        await SeedEnrollmentAsync(e =>
        {
            e.AccountId = target.Id;
            e.CreatedAt = now;
            e.ExpiresAt = now.AddHours(72);
        });

        using (var start = await anonymous.PostAsJsonAsync("/api/admin/login/start", new
        {
            username = target.Username,
            password = target.Password,
        }))
        {
            Assert.Equal("{\"ok\":true,\"enroll\":false}", await start.Content.ReadAsStringAsync());
        }

        await using (var db = Db())
        {
            Assert.False(await db.AdminEnrollments.AnyAsync(x => x.TokenHash != null));
        }

        var again = await PostAsync(client, "/api/admin/users/" + Id(target.Id) + "/disable");
        Assert.Equal(HttpStatusCode.OK, again.Status);

        await using (var db = Db())
        {
            Assert.Equal(1, await db.AdminDisables.CountAsync());
        }
    }

    // The person being disabled chose the initial passwords of the invitations
    // they created, so each one would let them back in as a new account. Those
    // go with the disable. Other people's invitations stay, and so do resets
    // the person started, which need the other account's own password.
    [SkippableFact]
    public async Task Disable_DeletesTheInvitationsTheAccountCreated()
    {
        RequireDatabase();

        using var factory = Factory();
        var (op, client) = await OperatorAsync(factory);
        var leaver = await AdminTestAccounts.CreateAsync(Db, factory, "leaver", "Leaving Operator");
        using var leaverClient = await AdminTestAccounts.SignedInAsync(Db, factory, leaver);
        var other = await AdminTestAccounts.CreateAsync(Db, factory, "other", "Other Person");

        var byLeaver = await PostAsync(leaverClient, "/api/admin/users/create",
            CreateBody(AdminTestAccounts.Code(factory, leaver.Secret), username: "by.leaver"));
        Assert.Equal(HttpStatusCode.OK, byLeaver.Status);

        var byOperator = await PostAsync(client, "/api/admin/users/create",
            CreateBody(AdminTestAccounts.Code(factory, op.Secret), username: "by.operator"));
        Assert.Equal(HttpStatusCode.OK, byOperator.Status);

        factory.Clock.Advance(TimeSpan.FromSeconds(Totp.StepSeconds));

        var reset = await PostAsync(leaverClient, "/api/admin/users/" + Id(other.Id) + "/reset-authenticator",
            new { code = AdminTestAccounts.Code(factory, leaver.Secret) });
        Assert.Equal(HttpStatusCode.OK, reset.Status);

        await using (var db = Db())
        {
            Assert.Equal(leaver.Id, (await db.AdminEnrollments.AsNoTracking().SingleAsync(x => x.Username == "by.leaver")).CreatedByAccountId);
            Assert.Equal(leaver.Id, (await db.AdminEnrollments.AsNoTracking().SingleAsync(x => x.AccountId == other.Id)).CreatedByAccountId);
        }

        Assert.Equal(HttpStatusCode.OK, (await PostAsync(client, "/api/admin/users/" + Id(leaver.Id) + "/disable")).Status);

        await using (var db = Db())
        {
            Assert.False(await db.AdminEnrollments.AnyAsync(x => x.Username == "by.leaver"));
            Assert.True(await db.AdminEnrollments.AnyAsync(x => x.Username == "by.operator"));
            Assert.True(await db.AdminEnrollments.AnyAsync(x => x.AccountId == other.Id));
        }
    }

    // The filter checks admin_disables itself on every request, not only
    // through the sessions the disable endpoint deletes.
    [SkippableFact]
    public async Task TokenFilter_RefusesASessionOfAnAccountInAdminDisables()
    {
        RequireDatabase();

        using var factory = Factory();
        var target = await AdminTestAccounts.CreateAsync(Db, factory, "target", "Target Person");
        using var targetClient = await AdminTestAccounts.SignedInAsync(Db, factory, target);

        using (var before = await targetClient.GetAsync("/api/admin/session"))
        {
            Assert.Equal(HttpStatusCode.OK, before.StatusCode);
        }

        await using (var db = Db())
        {
            db.AdminDisables.Add(new AdminDisable { AccountId = target.Id, DisabledAt = Now(factory), DisabledBy = "ops" });
            await db.SaveChangesAsync();
        }

        using var after = await targetClient.GetAsync("/api/admin/session");
        Assert.Equal(HttpStatusCode.Unauthorized, after.StatusCode);
    }

    // --------------------------------------------------------- pending delete

    [SkippableFact]
    public async Task DeletePending_DeletesNewUsersOnly()
    {
        RequireDatabase();

        using var factory = Factory();
        var now = Now(factory);
        var (_, client) = await OperatorAsync(factory);
        var target = await AdminTestAccounts.CreateAsync(Db, factory, "target", "Target Person", a => a.TotpSecret = "");

        var invitation = await SeedEnrollmentAsync(e =>
        {
            e.Username = "never.came";
            e.PasswordHash = AdminTestAccounts.Hash(InitialPassword);
            e.CreatedAt = now;
            e.ExpiresAt = now.AddHours(72);
        });

        var reset = await SeedEnrollmentAsync(e =>
        {
            e.AccountId = target.Id;
            e.CreatedAt = now;
            e.ExpiresAt = now.AddHours(72);
        });

        var deleted = await PostAsync(client, "/api/admin/users/pending/" + Id(invitation.Id) + "/delete");
        Assert.Equal(HttpStatusCode.OK, deleted.Status);

        Assert.Equal(HttpStatusCode.NotFound,
            (await PostAsync(client, "/api/admin/users/pending/" + Id(reset.Id) + "/delete")).Status);
        Assert.Equal(HttpStatusCode.NotFound,
            (await PostAsync(client, "/api/admin/users/pending/" + Id(invitation.Id) + "/delete")).Status);

        await using var db = Db();
        Assert.False(await db.AdminEnrollments.AnyAsync(x => x.Id == invitation.Id));
        Assert.True(await db.AdminEnrollments.AnyAsync(x => x.Id == reset.Id));
        Assert.True(await db.AdminAccounts.AnyAsync(x => x.Id == target.Id));
    }

    // ------------------------------------------------------------------- list

    [SkippableFact]
    public async Task List_ReportsEveryStatusAndThePendingUsers()
    {
        RequireDatabase();

        using var factory = Factory();
        var now = Now(factory);
        var (op, client) = await OperatorAsync(factory);

        var locked = await AdminTestAccounts.CreateAsync(Db, factory, "locked", "Locked", a => a.LockedUntil = now.AddMinutes(5));
        var column = await AdminTestAccounts.CreateAsync(Db, factory, "column", "Column", a => a.Disabled = true);
        var panel = await AdminTestAccounts.CreateAsync(Db, factory, "panel", "Panel");
        var reset = await AdminTestAccounts.CreateAsync(Db, factory, "reset", "Reset", a => a.TotpSecret = "");
        var bare = await AdminTestAccounts.CreateAsync(Db, factory, "bare", "Bare", a => a.TotpSecret = "");
        var lapsed = await AdminTestAccounts.CreateAsync(Db, factory, "lapsed", "Lapsed", a => a.TotpSecret = "");

        await using (var db = Db())
        {
            db.AdminDisables.Add(new AdminDisable { AccountId = panel.Id, DisabledAt = now, DisabledBy = OperatorName });
            await db.SaveChangesAsync();
        }

        await SeedEnrollmentAsync(e => { e.AccountId = reset.Id; e.CreatedAt = now; e.ExpiresAt = now.AddHours(10); });
        await SeedEnrollmentAsync(e => { e.AccountId = lapsed.Id; e.CreatedAt = now.AddHours(-80); e.ExpiresAt = now.AddHours(-8); });

        var live = await SeedEnrollmentAsync(e =>
        {
            e.Username = "invited";
            e.DisplayName = "Invited Person";
            e.PasswordHash = AdminTestAccounts.Hash(InitialPassword);
            e.CreatedAt = now;
            e.ExpiresAt = now.AddHours(72);
            e.LockedUntil = now.AddMinutes(5);
        });

        // A lock that has already run out is not reported.
        var expired = await SeedEnrollmentAsync(e =>
        {
            e.Username = "too.late";
            e.PasswordHash = AdminTestAccounts.Hash(InitialPassword);
            e.CreatedAt = now.AddHours(-80);
            e.ExpiresAt = now.AddHours(-8);
            e.LockedUntil = now.AddMinutes(-1);
        });

        using var response = await client.GetAsync("/api/admin/users");
        var text = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Never the credential columns, under any name.
        Assert.DoesNotContain("secret", text, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("hash", text, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("token", text, StringComparison.OrdinalIgnoreCase);

        var root = JsonDocument.Parse(text).RootElement;
        Assert.Equal(op.Id, root.GetProperty("self").GetInt64());

        var accounts = root.GetProperty("accounts").EnumerateArray().ToList();
        Assert.Equal(7, accounts.Count);
        Assert.Equal(accounts.Select(a => a.GetProperty("id").GetInt64()).Order(), accounts.Select(a => a.GetProperty("id").GetInt64()));

        JsonElement Row(long id) => accounts.Single(a => a.GetProperty("id").GetInt64() == id);
        string? Status(long id) => Row(id).GetProperty("status").GetString();

        Assert.Equal("active", Status(op.Id));
        Assert.Equal("active", Status(locked.Id));
        Assert.Equal("disabled", Status(column.Id));
        Assert.Equal("disabled", Status(panel.Id));
        Assert.Equal("reset", Status(reset.Id));
        Assert.Equal("no_authenticator", Status(bare.Id));
        Assert.Equal("no_authenticator", Status(lapsed.Id));

        Assert.Equal(JsonValueKind.String, Row(locked.Id).GetProperty("lockedUntil").ValueKind);
        Assert.Equal(JsonValueKind.Null, Row(op.Id).GetProperty("lockedUntil").ValueKind);
        Assert.Equal(now.AddHours(10), Row(reset.Id).GetProperty("resetExpiresAt").GetDateTime().ToUniversalTime());
        Assert.Equal(JsonValueKind.Null, Row(lapsed.Id).GetProperty("resetExpiresAt").ValueKind);
        Assert.Equal("operator", Row(op.Id).GetProperty("username").GetString());
        Assert.Equal(OperatorName, Row(op.Id).GetProperty("displayName").GetString());

        var pending = root.GetProperty("pending").EnumerateArray().ToList();
        Assert.Equal(2, pending.Count);
        Assert.Equal(live.Id, pending[0].GetProperty("id").GetInt64());
        Assert.Equal("invited", pending[0].GetProperty("username").GetString());
        Assert.Equal("Invited Person", pending[0].GetProperty("displayName").GetString());
        Assert.False(pending[0].GetProperty("expired").GetBoolean());
        Assert.Equal(now.AddMinutes(5), pending[0].GetProperty("lockedUntil").GetDateTime().ToUniversalTime());
        Assert.Equal(expired.Id, pending[1].GetProperty("id").GetInt64());
        Assert.True(pending[1].GetProperty("expired").GetBoolean());
        Assert.Equal(JsonValueKind.Null, pending[1].GetProperty("lockedUntil").ValueKind);
    }

    [SkippableFact]
    public async Task UsersEndpoints_RefuseAnUnauthenticatedCaller()
    {
        RequireDatabase();

        using var factory = Factory();
        using var client = factory.CreateClient();

        using var list = await client.GetAsync("/api/admin/users");
        using var create = await client.PostAsJsonAsync("/api/admin/users/create", CreateBody("123456"));

        Assert.Equal(HttpStatusCode.Unauthorized, list.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, create.StatusCode);
    }
}
