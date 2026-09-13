using Kestridge.Api.Data;
using Kestridge.Api.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Kestridge.Api.Admin;

// A fresh authenticator code from the signed-in operator, asked for before an
// action that hands someone a way in: creating a user and resetting an
// authenticator. A session token lifted from a browser is then not enough on
// its own to mint an account.
//
// The same rules as sign-in, on the same row: the replay rule, the lockout
// counters and the disabled check. A code spent here cannot then be spent on
// /login, and guessing here counts towards the same lockout.
//
// A refusal is a 400 on field "code", never a 401. The panel treats every 401
// as a lost session and signs the operator out, which for a mistyped code would
// throw away the form they just filled in.
public static class AdminStepUp
{
    public static async Task<bool> VerifyAsync(
        HttpContext http, KestridgeDbContext db, string? code, AdminOptions options, DateTime now, ILogger log)
    {
        var who = AdminIdentity.Of(http);

        // Tracked: the filter already loaded this row into the same context, and
        // TotpLastStep and the counters are written back below.
        var account = await db.AdminAccounts.FirstOrDefaultAsync(x => x.Id == who.AccountId, CancellationToken.None);

        if (account is null || await AdminAccess.IsDisabledAsync(db, account, CancellationToken.None))
        {
            log.LogInformation("admin.step_up_blocked account={AccountId}", who.AccountId);
            return false;
        }

        // Counted before the code is checked, like /login. A locked account is
        // refused here, and a count saved after the check would let codes sent
        // together all see the same count.
        if (!await AdminAccess.ChargeAsync(db.AdminAccounts.Where(x => x.Id == account.Id), now, options))
        {
            log.LogInformation("admin.step_up_blocked account={AccountId}", account.Id);
            return false;
        }

        var step = Totp.Verify(account.TotpSecret, (code ?? string.Empty).Trim(), now, options.TotpSkewSteps);

        if (step is { } accepted && (ulong)accepted > account.TotpLastStep)
        {
            // Saved now rather than with the caller's work, so the code is spent
            // even when the action then fails validation and returns early. The
            // counters are cleared as a sign-in clears them, or five step-ups
            // in an hour would lock the operator out.
            account.TotpLastStep = (ulong)accepted;
            AdminAccess.ClearFailures(db, account);
            await db.SaveChangesAsync(CancellationToken.None);
            return true;
        }

        log.LogInformation("admin.step_up_failed account={AccountId}", account.Id);
        return false;
    }
}
