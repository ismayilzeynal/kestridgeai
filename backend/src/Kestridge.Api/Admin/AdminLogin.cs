using Kestridge.Api.Data;
using Kestridge.Api.Infrastructure;
using Kestridge.Api.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kestridge.Api.Admin;

public static class AdminLogin
{
    public static async Task<IResult> Handle(
        HttpContext http,
        LoginRequest request,
        KestridgeDbContext db,
        IPasswordHasher<AdminAccount> hasher,
        IOptions<AdminOptions> adminOptions,
        TimeProvider clock,
        ILogger<AdminAccount> log)
    {
        if (!AdminOriginCheck.SameOrigin(http.Request))
        {
            return JsonResults.Origin();
        }

        var options = adminOptions.Value;
        var now = clock.GetUtcNow().UtcDateTime;

        var username = AdminUserRules.ForLookup(request.Username);
        var password = request.Password ?? string.Empty;
        var code = (request.Code ?? string.Empty).Trim();

        AdminAccount? account = null;

        if (AdminUserRules.CanMatch(username))
        {
            account = await db.AdminAccounts.FirstOrDefaultAsync(a => a.Username == username, http.RequestAborted);
        }

        if (account is null)
        {
            // Against AdminAccess.DummyHash, so an unknown username still runs a
            // hash verification. The reason is written next to it.
            hasher.VerifyHashedPassword(new AdminAccount(), AdminAccess.DummyHash(hasher), password);
            log.LogInformation("admin.login_failed");
            return JsonResults.Auth();
        }

        // Disabled covers both the column and a panel disable in admin_disables.
        // Every refusal still runs exactly one hash verification, against the
        // dummy hash where the account's own must not be consulted, so a
        // blocked account answers in the time an unknown one does.
        if (await AdminAccess.IsDisabledAsync(db, account, http.RequestAborted))
        {
            hasher.VerifyHashedPassword(new AdminAccount(), AdminAccess.DummyHash(hasher), password);
            log.LogInformation("admin.login_blocked");
            return JsonResults.Auth();
        }

        // An account whose authenticator was reset has an empty secret, and
        // Verify refuses an empty key, so it can never pass here whatever the
        // code. Its way back in is /login/start, and start is the one place its
        // password guesses are counted. The panel sends every enroll:false
        // answer on to this step, so counting here as well would charge one
        // mistyped password twice and lock the account after three attempts.
        // The answer is the same 401 either way, so skipping the count opens no
        // oracle.
        if (account.TotpSecret.Length == 0)
        {
            hasher.VerifyHashedPassword(
                new AdminAccount(),
                AdminAccess.IsLocked(account, now) ? AdminAccess.DummyHash(hasher) : account.PasswordHash,
                password);
            log.LogInformation("admin.login_failed");
            return JsonResults.Auth();
        }

        // Counted before anything is verified. See ChargeAsync for why a count
        // saved after the check is no lockout at all.
        if (!await AdminAccess.ChargeAsync(db.AdminAccounts.Where(a => a.Id == account.Id), now, options))
        {
            hasher.VerifyHashedPassword(new AdminAccount(), AdminAccess.DummyHash(hasher), password);
            log.LogInformation("admin.login_blocked");
            return JsonResults.Auth();
        }

        var passwordOk = hasher.VerifyHashedPassword(account, account.PasswordHash, password)
                         != PasswordVerificationResult.Failed;

        // SuccessRehashNeeded is deliberately ignored: kestridge_app holds no
        // UPDATE on password_hash, because a password is written once, by the
        // INSERT that creates the account. Raising the iteration count is an
        // ops/admin-account.sql operation.

        long? acceptedStep = null;
        if (passwordOk)
        {
            acceptedStep = Totp.Verify(account.TotpSecret, code, now, options.TotpSkewSteps);

            // Replay: a code is valid once. Without this, one shoulder-surfed
            // code works for the rest of its 30 second step.
            if (acceptedStep is { } step && (ulong)step <= account.TotpLastStep)
            {
                acceptedStep = null;
            }
        }

        // Already counted by the charge above.
        if (!passwordOk || acceptedStep is null)
        {
            log.LogInformation("admin.login_failed");
            return JsonResults.Auth();
        }

        AdminAccess.ClearFailures(db, account);
        account.LastLoginAt = now;
        account.TotpLastStep = (ulong)acceptedStep.Value;

        var token = AdminSessions.NewToken();
        var session = AdminSessions.Issue(account.Id, AdminSessions.Hash(token), now, options);
        db.AdminSessions.Add(session);

        await db.SaveChangesAsync(CancellationToken.None);

        log.LogInformation("admin.login_ok account={AccountId}", account.Id);

        return Results.Json(new
        {
            ok = true,
            token,
            username = account.Username,
            displayName = account.DisplayName,
            absoluteExpiresAt = session.AbsoluteExpiresAt,
            idleTimeoutSeconds = options.IdleMinutes * 60,
        });
    }
}
