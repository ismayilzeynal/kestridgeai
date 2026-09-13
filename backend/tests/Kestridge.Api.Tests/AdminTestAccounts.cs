using System.Globalization;
using Kestridge.Api.Admin;
using Kestridge.Api.Data;
using Kestridge.Api.Options;
using Microsoft.AspNetCore.Identity;

namespace Kestridge.Api.Tests;

public sealed record SeededAccount(long Id, string Username, string DisplayName, string Password, string Secret);

// The one place tests make admin accounts and sessions. It used to be a private
// helper copied into each test class, which is how a change to what an account
// needs gets fixed in one copy and not the other.
public static class AdminTestAccounts
{
    public const string Password = "correct horse battery";

    // A real IdentityV3 hash, so /login and /login/start can verify it, but at
    // the minimum iteration count. The count is stored in the hash itself, so
    // the application's hasher verifies it whatever its own setting is.
    private static readonly PasswordHasher<AdminAccount> Hasher = new(
        Microsoft.Extensions.Options.Options.Create(new PasswordHasherOptions
        {
            CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3,
            IterationCount = 1000,
        }));

    // The settings every test that signs in for real wants. The hasher cost is
    // cut for speed, and the sign-in budget is raised because a test makes more
    // attempts from one address than a person would.
    public static Dictionary<string, string?> FactorySettings() => new()
    {
        ["Kestridge:Admin:PasswordIterations"] = "1000",
        ["Kestridge:RateLimit:LoginPermitsPerWindow"] = "1000",
    };

    public static string Hash(string password) => Hasher.HashPassword(new AdminAccount(), password);

    public static bool Verifies(string hash, string password)
        => Hasher.VerifyHashedPassword(new AdminAccount(), hash, password) != PasswordVerificationResult.Failed;

    public static async Task<SeededAccount> CreateAsync(
        Func<KestridgeDbContext> db,
        ApiFactory factory,
        string username,
        string displayName,
        Action<AdminAccount>? configure = null)
    {
        await using var context = db();

        var account = new AdminAccount
        {
            Username = username,
            DisplayName = displayName,
            PasswordHash = Hash(Password),
            TotpSecret = Totp.NewSecret(),
            CreatedAt = factory.Clock.GetUtcNow().UtcDateTime,
        };

        configure?.Invoke(account);

        context.AdminAccounts.Add(account);
        await context.SaveChangesAsync();

        return new SeededAccount(account.Id, account.Username, account.DisplayName, Password, account.TotpSecret);
    }

    // The session row is written directly rather than driven through
    // /api/admin/login, so a test of some other surface does not fail because
    // sign-in changed shape. Sign-in has its own tests.
    public static async Task<HttpClient> SignedInAsync(
        Func<KestridgeDbContext> db, ApiFactory factory, SeededAccount account, string? clientAddress = null)
    {
        var token = AdminSessions.NewToken();

        await using (var context = db())
        {
            context.AdminSessions.Add(AdminSessions.Issue(
                account.Id, AdminSessions.Hash(token), factory.Clock.GetUtcNow().UtcDateTime, new AdminOptions()));
            await context.SaveChangesAsync();
        }

        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

        if (clientAddress is not null)
        {
            client.DefaultRequestHeaders.Add(ApiFactory.ClientAddressHeader, clientAddress);
        }

        return client;
    }

    public static async Task<HttpClient> SignedInAsync(
        Func<KestridgeDbContext> db, ApiFactory factory, string username, string displayName, string? clientAddress = null)
        => await SignedInAsync(db, factory, await CreateAsync(db, factory, username, displayName), clientAddress);

    // The code an authenticator shows at the factory clock, offset by whole
    // steps. An offset of +1 is still accepted with the default skew, which is
    // how a test gets "the next code" without moving the clock.
    public static string Code(ApiFactory factory, string secret, int stepOffset = 0)
        => Totp.Compute(Totp.FromBase32(secret), Totp.StepFor(factory.Clock.GetUtcNow().UtcDateTime) + stepOffset);

    public static long Step(ApiFactory factory, int stepOffset = 0)
        => Totp.StepFor(factory.Clock.GetUtcNow().UtcDateTime) + stepOffset;

    // Six digits that are none of the codes the skew window would accept, so a
    // "wrong code" test cannot pass by coincidence roughly one run in 333,000.
    public static string WrongCode(ApiFactory factory, string secret)
    {
        var valid = new HashSet<string>(StringComparer.Ordinal)
        {
            Code(factory, secret, -1), Code(factory, secret), Code(factory, secret, 1),
        };

        for (var candidate = 0; ; candidate++)
        {
            var code = candidate.ToString("D6", CultureInfo.InvariantCulture);
            if (!valid.Contains(code))
            {
                return code;
            }
        }
    }
}
