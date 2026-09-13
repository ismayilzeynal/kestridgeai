using Kestridge.Api.Data;
using Kestridge.Api.Infrastructure;
using Kestridge.Api.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace Kestridge.Api.Admin;

// First sign-in, for a user created in the panel or an account whose
// authenticator was reset. Two unauthenticated calls, mapped outside the token
// filter like /login, and each rate limited in a partition of its own.
//
// start takes the username and password and answers one of exactly two ways.
// Anything that is not a live enrolment with the right password gets the same
// bytes as everything else: an unknown user, a wrong password, a disabled or
// locked account, an expired invitation, and a normal account with the RIGHT
// password. That last one is what keeps start from becoming the password oracle
// that /login is careful not to be: for an enrolled account it confirms nothing,
// and the client goes on to /login, which needs the code as well. Each path runs
// exactly one password hash verification, so timing does not separate them
// either.
//
// enroll takes the setup token start issued, a code from the new secret and, for
// a new user, the password they choose. Only then does an admin_accounts row
// exist, or get its secret back.
public static class AdminEnroll
{
    private const int DuplicateKeyErrorNumber = 1062;

    public static async Task<IResult> Start(
        HttpContext http,
        LoginStartRequest request,
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
        var lookup = AdminUserRules.CanMatch(username);

        var account = lookup
            ? await db.AdminAccounts.FirstOrDefaultAsync(x => x.Username == username, http.RequestAborted)
            : null;

        if (account is not null)
        {
            if (await AdminAccess.IsDisabledAsync(db, account, http.RequestAborted) || AdminAccess.IsLocked(account, now))
            {
                hasher.VerifyHashedPassword(new AdminAccount(), AdminAccess.DummyHash(hasher), password);
                return NotEnrolling();
            }

            var reset = account.TotpSecret.Length == 0
                ? await db.AdminEnrollments.FirstOrDefaultAsync(
                    x => x.AccountId == account.Id && x.ExpiresAt > now, http.RequestAborted)
                : null;

            // An enrolled account, right password or wrong. Nothing is counted
            // here: the client goes on to /login, which counts, and counting
            // twice would halve the attempts before a lockout.
            if (reset is null)
            {
                hasher.VerifyHashedPassword(account, account.PasswordHash, password);
                return NotEnrolling();
            }

            // The password is the only factor between here and a setup token,
            // so the count is taken before it is checked. See ChargeAsync.
            if (!await AdminAccess.ChargeAsync(db.AdminAccounts.Where(x => x.Id == account.Id), now, options))
            {
                hasher.VerifyHashedPassword(new AdminAccount(), AdminAccess.DummyHash(hasher), password);
                return NotEnrolling();
            }

            if (hasher.VerifyHashedPassword(account, account.PasswordHash, password) == PasswordVerificationResult.Failed)
            {
                log.LogInformation("admin.enroll_failed");
                return NotEnrolling();
            }

            AdminAccess.ClearFailures(db, account);
            return await IssueAsync(db, reset, account.Username, setPassword: false, now, options, log);
        }

        var invitation = lookup
            ? await db.AdminEnrollments.FirstOrDefaultAsync(
                x => x.AccountId == null && x.Username == username, http.RequestAborted)
            : null;

        if (invitation is null || invitation.ExpiresAt <= now || AdminAccess.IsLocked(invitation, now))
        {
            hasher.VerifyHashedPassword(new AdminAccount(), AdminAccess.DummyHash(hasher), password);
            return NotEnrolling();
        }

        // Counted on the invitation, because there is no account row yet, and
        // before the password is checked. Without it the initial password could
        // be guessed for as long as the invitation lives, at the rate limiter's
        // pace, and the rate limiter is per client, so this count is the only
        // bound across addresses.
        if (!await AdminAccess.ChargeAsync(db.AdminEnrollments.Where(x => x.Id == invitation.Id), now, options))
        {
            hasher.VerifyHashedPassword(new AdminAccount(), AdminAccess.DummyHash(hasher), password);
            return NotEnrolling();
        }

        if (hasher.VerifyHashedPassword(new AdminAccount(), invitation.PasswordHash, password)
            == PasswordVerificationResult.Failed)
        {
            log.LogInformation("admin.enroll_failed");
            return NotEnrolling();
        }

        AdminAccess.ClearFailures(db, invitation);
        return await IssueAsync(db, invitation, username, setPassword: true, now, options, log);
    }

    public static async Task<IResult> Complete(
        HttpContext http,
        LoginEnrollRequest request,
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

        // 409 rather than 401 for every "this setup is gone" case. The panel
        // reads any 401 as a lost session; this one should send the person back
        // to sign in again, which is a different screen with a different
        // sentence. A malformed token never reaches the database.
        if (!AdminSessions.IsWellFormed(request.Token))
        {
            return JsonResults.Stale();
        }

        var tokenHash = AdminSessions.Hash(request.Token!);
        var enrollment = await db.AdminEnrollments.FirstOrDefaultAsync(x => x.TokenHash == tokenHash, http.RequestAborted);

        if (enrollment is null
            || enrollment.TokenExpiresAt is not { } tokenExpiresAt
            || tokenExpiresAt <= now
            || enrollment.ExpiresAt <= now)
        {
            return JsonResults.Stale();
        }

        // Proves the app was set up from the secret this token was issued with.
        // Not counted towards a lockout: whoever holds the token was also sent
        // the secret, so a guessed code gains them nothing they do not have.
        if (Totp.Verify(enrollment.TotpSecret, (request.Code ?? string.Empty).Trim(), now, options.TotpSkewSteps)
            is not { } step)
        {
            return JsonResults.Invalid("code");
        }

        var newUser = enrollment.AccountId is null;

        // The initial password was chosen by somebody else and travelled to this
        // person outside the panel, so it is never allowed to stay. A reset
        // keeps the account's own password and ignores the field.
        if (newUser)
        {
            if (!AdminUserRules.PasswordLengthOk(request.NewPassword))
            {
                return JsonResults.Invalid("newPassword", "length");
            }

            if (hasher.VerifyHashedPassword(new AdminAccount(), enrollment.PasswordHash, request.NewPassword!)
                != PasswordVerificationResult.Failed)
            {
                return JsonResults.Invalid("newPassword", "same");
            }
        }

        AdminAccount account;
        string token;
        AdminSession session;

        await using (var tx = await db.Database.BeginTransactionAsync(CancellationToken.None))
        {
            if (newUser)
            {
                // Someone made this username by SQL while the invitation waited.
                if (await db.AdminAccounts.AnyAsync(x => x.Username == enrollment.Username, CancellationToken.None))
                {
                    return JsonResults.Stale();
                }

                // An INSERT, the only way this app ever writes a password hash.
                // Disabled and the counters take their column defaults.
                account = new AdminAccount
                {
                    Username = enrollment.Username!,
                    DisplayName = enrollment.DisplayName,
                    PasswordHash = hasher.HashPassword(new AdminAccount(), request.NewPassword!),
                    TotpSecret = enrollment.TotpSecret,
                    TotpLastStep = (ulong)step,
                    CreatedAt = now,
                    LastLoginAt = now,
                };

                db.AdminAccounts.Add(account);
            }
            else
            {
                var found = await db.AdminAccounts.FirstOrDefaultAsync(
                    x => x.Id == enrollment.AccountId, CancellationToken.None);

                // Disabled, or given a secret by another route since the reset.
                // Either way this setup no longer describes the account.
                if (found is null
                    || await AdminAccess.IsDisabledAsync(db, found, CancellationToken.None)
                    || found.TotpSecret.Length != 0)
                {
                    return JsonResults.Stale();
                }

                account = found;
                account.TotpSecret = enrollment.TotpSecret;

                // The accepted step, even when the stored one is higher. Replay
                // state belongs to a secret, and the secret it guarded is gone,
                // so no step it spent can be replayed against this one. Keeping
                // a higher value would keep whatever pushed it there: a row
                // tampered far ahead would refuse every code after this first
                // one, and the reset meant to recover the account would not.
                account.TotpLastStep = (ulong)step;
                AdminAccess.ClearFailures(db, account);
                account.LastLoginAt = now;
            }

            try
            {
                await db.SaveChangesAsync(CancellationToken.None);
            }
            catch (DbUpdateException ex) when (ex.InnerException is MySqlException { Number: DuplicateKeyErrorNumber })
            {
                return JsonResults.Stale();
            }
            catch (DbUpdateConcurrencyException)
            {
                // The account row was deleted, by SQL, after it was read above.
                return JsonResults.Stale();
            }

            // Deleted only while it still carries the token presented, not by id
            // alone. A new reset updates the row in place and a new start gives
            // it a new token, so either one, committed after the lookup above,
            // leaves the id the same and the token different. Nothing deleted
            // means the setup this token described is gone, and returning here
            // rolls back the account written above. After the account, not
            // before it, so the row locks are taken in the order a reset takes
            // them.
            var deleted = await db.AdminEnrollments
                .Where(x => x.Id == enrollment.Id && x.TokenHash == tokenHash)
                .ExecuteDeleteAsync(CancellationToken.None);

            if (deleted == 0)
            {
                return JsonResults.Stale();
            }

            token = AdminSessions.NewToken();
            session = AdminSessions.Issue(account.Id, AdminSessions.Hash(token), now, options);
            db.AdminSessions.Add(session);

            await db.SaveChangesAsync(CancellationToken.None);
            await tx.CommitAsync(CancellationToken.None);
        }

        log.LogInformation("admin.enrolled account={AccountId}", account.Id);

        // The same body as /login, so the panel has one success path.
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

    private static async Task<IResult> IssueAsync(
        KestridgeDbContext db,
        AdminEnrollment enrollment,
        string username,
        bool setPassword,
        DateTime now,
        AdminOptions options,
        ILogger log)
    {
        var token = AdminSessions.NewToken();

        enrollment.TokenHash = AdminSessions.Hash(token);
        enrollment.TokenExpiresAt = now.AddMinutes(options.EnrollTokenMinutes);

        // A new secret every time, so a QR code left on a screen, or captured
        // from one, is dead as soon as anyone signs in again.
        enrollment.TotpSecret = Totp.NewSecret();

        try
        {
            await db.SaveChangesAsync(CancellationToken.None);
        }
        catch (DbUpdateConcurrencyException)
        {
            // The invitation or reset was deleted between the read and this
            // save. There is nothing left to set up, which is the default answer.
            return NotEnrolling();
        }

        log.LogInformation("admin.enroll_started enrollment={EnrollmentId}", enrollment.Id);

        var otpauth = Totp.OtpauthUri(username, enrollment.TotpSecret);

        return Results.Json(new
        {
            ok = true,
            enroll = true,
            setPassword,
            token,
            username,
            secret = enrollment.TotpSecret,
            otpauth,
            qr = AdminQr.DataUri(otpauth),
        });
    }

    private static IResult NotEnrolling() => Results.Json(new { ok = true, enroll = false });
}
