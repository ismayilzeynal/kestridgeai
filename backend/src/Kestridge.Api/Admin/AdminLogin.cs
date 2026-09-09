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
    // Verified against on an unknown username so the response time does not
    // distinguish a real account from a guess. A Task.Delay would be worse than
    // useless: it holds a connection open and is itself a denial of service
    // lever on a public endpoint.
    private static readonly string DummyHash =
        new PasswordHasher<AdminAccount>().HashPassword(new AdminAccount(), "timing-equalizer");

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

        var username = (request.Username ?? string.Empty).Trim().ToLowerInvariant();
        var password = request.Password ?? string.Empty;
        var code = (request.Code ?? string.Empty).Trim();

        AdminAccount? account = null;

        if (username.Length is > 0 and <= 64)
        {
            account = await db.AdminAccounts.FirstOrDefaultAsync(a => a.Username == username, http.RequestAborted);
        }

        if (account is null)
        {
            hasher.VerifyHashedPassword(new AdminAccount(), DummyHash, password);
            log.LogInformation("admin.login_failed");
            return JsonResults.Auth();
        }

        // Before verifying anything, so a locked or disabled account leaks
        // nothing through timing either.
        if (account.Disabled || (account.LockedUntil is { } until && until > now))
        {
            log.LogInformation("admin.login_blocked");
            return JsonResults.Auth();
        }

        var passwordOk = hasher.VerifyHashedPassword(account, account.PasswordHash, password)
                         != PasswordVerificationResult.Failed;

        // SuccessRehashNeeded is deliberately ignored: password_hash is not in
        // the column-level UPDATE grant, so the app cannot rewrite it. Raising
        // the iteration count is an ops/admin-account.sql operation.

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

        if (!passwordOk || acceptedStep is null)
        {
            RecordFailure(account, now, options);
            await db.SaveChangesAsync(CancellationToken.None);
            log.LogInformation("admin.login_failed");
            return JsonResults.Auth();
        }

        account.FailedAttempts = 0;
        account.FirstFailedAt = null;
        account.LockedUntil = null;
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

    private static void RecordFailure(AdminAccount account, DateTime now, AdminOptions options)
    {
        // The counter is a one hour window, not a lifetime total, so a wrong
        // code last March does not contribute to a lockout today.
        if (account.FirstFailedAt is not { } first || (now - first).TotalHours >= 1)
        {
            account.FailedAttempts = 1;
            account.FirstFailedAt = now;
        }
        else
        {
            account.FailedAttempts = (ushort)Math.Min(account.FailedAttempts + 1, ushort.MaxValue);
        }

        if (account.FailedAttempts >= options.MaxFailedAttempts)
        {
            account.LockedUntil = now.AddMinutes(options.LockMinutes);
        }
    }
}
