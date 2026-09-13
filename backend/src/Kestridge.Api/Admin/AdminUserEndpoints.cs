using Kestridge.Api.Data;
using Kestridge.Api.Email;
using Kestridge.Api.Infrastructure;
using Kestridge.Api.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;
using MimeKit;

namespace Kestridge.Api.Admin;

// Accounts from the panel. What the database still refuses kestridge_app, whatever
// this file does: rewriting a password hash, renaming an operator, re-enabling a
// disabled account, deleting an account row. See ops/04-table-grants.sql. What
// this file adds on top is application level: a fresh code from the operator
// before anyone is given a way in, and a mail to the team mailbox when they are.
//
// There is no rename, no password change and no re-enable here, and there
// cannot be without widening the grants. Those stay migrator operations.
public static class AdminUserEndpoints
{
    private const int DuplicateKeyErrorNumber = 1062;
    private const int DeadlockErrorNumber = 1213;

    public static async Task<IResult> List(HttpContext http, KestridgeDbContext db, TimeProvider clock)
    {
        var ct = http.RequestAborted;
        var now = clock.GetUtcNow().UtcDateTime;
        var who = AdminIdentity.Of(http);

        // Projected: totp_secret is only ever compared with empty here and
        // never leaves the process.
        var accounts = await db.AdminAccounts.AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new
            {
                x.Id,
                x.Username,
                x.DisplayName,
                x.Disabled,
                HasAuthenticator = x.TotpSecret != "",
                x.LockedUntil,
                x.LastLoginAt,
                x.CreatedAt,
            })
            .ToListAsync(ct);

        var disabled = await db.AdminDisables.AsNoTracking().Select(x => x.AccountId).ToListAsync(ct);

        var resets = await db.AdminEnrollments.AsNoTracking()
            .Where(x => x.AccountId != null && x.ExpiresAt > now)
            .Select(x => new { AccountId = x.AccountId!.Value, x.ExpiresAt })
            .ToListAsync(ct);

        var pending = await db.AdminEnrollments.AsNoTracking()
            .Where(x => x.AccountId == null)
            .OrderBy(x => x.Id)
            .ToListAsync(ct);

        return Results.Json(new
        {
            ok = true,
            self = who.AccountId,
            accounts = accounts.Select(a =>
            {
                var reset = resets.Find(r => r.AccountId == a.Id);

                // Disabled wins over everything, because it is the one status
                // that says the person cannot get in whatever else is true.
                var status = a.Disabled || disabled.Contains(a.Id) ? "disabled"
                    : a.HasAuthenticator ? "active"
                    : reset is not null ? "reset"
                    : "no_authenticator";

                return new AccountRow(
                    a.Id,
                    a.Username,
                    a.DisplayName,
                    status,
                    a.LockedUntil is { } until && until > now ? until : null,
                    a.LastLoginAt,
                    a.CreatedAt,
                    reset?.ExpiresAt);
            }),
            pending = pending.Select(x => PendingRow(x, now)),
        });
    }

    public static async Task<IResult> Create(
        HttpContext http,
        KestridgeDbContext db,
        IPasswordHasher<AdminAccount> hasher,
        IEmailSender mail,
        IOptions<AdminOptions> adminOptions,
        IOptions<ContactOptions> contactOptions,
        TimeProvider clock,
        ILogger<AdminAccount> log,
        UserCreateRequest request)
    {
        var options = adminOptions.Value;
        var now = clock.GetUtcNow().UtcDateTime;

        if (AdminUserRules.NewUsername(request.Username) is not { } username)
        {
            return JsonResults.Invalid("username", "format");
        }

        if (ContentValidation.Text("displayName", request.DisplayName, 64, out var displayName) is { } d)
        {
            return JsonResults.Invalid(d.Field, d.Reason);
        }

        if (!AdminUserRules.PasswordLengthOk(request.Password))
        {
            return JsonResults.Invalid("password", "length");
        }

        // After the cheap checks, so a typo in the form does not spend the
        // operator's code, and before anything that reveals whether a username
        // exists, so that answer costs a code too.
        if (!await AdminStepUp.VerifyAsync(http, db, request.Code, options, now, log))
        {
            return JsonResults.Invalid("code");
        }

        if (await db.AdminAccounts.AnyAsync(x => x.Username == username, CancellationToken.None))
        {
            return JsonResults.Invalid("username", "taken");
        }

        var previous = await db.AdminEnrollments
            .FirstOrDefaultAsync(x => x.AccountId == null && x.Username == username, CancellationToken.None);

        if (previous is not null && previous.ExpiresAt > now)
        {
            return JsonResults.Invalid("username", "taken");
        }

        var who = AdminIdentity.Of(http);
        var row = new AdminEnrollment
        {
            Username = username,
            DisplayName = displayName,
            PasswordHash = hasher.HashPassword(new AdminAccount(), request.Password!),
            CreatedBy = AdminAccess.Attribution(who.DisplayName),
            CreatedByAccountId = who.AccountId,
            CreatedAt = now,
            ExpiresAt = now.AddHours(options.EnrollHours),
        };

        await using (var tx = await db.Database.BeginTransactionAsync(CancellationToken.None))
        {
            try
            {
                // An expired invitation holds the unique username index until it
                // is gone. Removed and saved first, so the insert below never
                // depends on the order EF chooses for the two statements.
                if (previous is not null)
                {
                    db.AdminEnrollments.Remove(previous);
                    await db.SaveChangesAsync(CancellationToken.None);
                }

                db.AdminEnrollments.Add(row);
                await db.SaveChangesAsync(CancellationToken.None);
                await tx.CommitAsync(CancellationToken.None);
            }
            catch (DbUpdateException ex) when (
                ex is DbUpdateConcurrencyException
                || ex.InnerException is MySqlException { Number: DuplicateKeyErrorNumber })
            {
                // Two operators creating the same name at once. The loser either
                // hits the unique index or finds the expired invitation it meant
                // to replace already deleted by the winner. Either way it is told
                // the truth, and the transaction rolls back on dispose.
                db.Entry(row).State = EntityState.Detached;

                if (previous is not null)
                {
                    db.Entry(previous).State = EntityState.Detached;
                }

                return JsonResults.Invalid("username", "taken");
            }
        }

        log.LogInformation("admin.user_created enrollment={EnrollmentId}", row.Id);

        var notified = await NotifyAsync(
            mail,
            AccountNotice.UserCreated(
                username, displayName, who.DisplayName, now, row.ExpiresAt, contactOptions.Value),
            log,
            "user_created");

        return Results.Json(new { ok = true, pending = PendingRow(row, now), notified });
    }

    public static async Task<IResult> ResetAuthenticator(
        long id,
        HttpContext http,
        KestridgeDbContext db,
        IEmailSender mail,
        IOptions<AdminOptions> adminOptions,
        IOptions<ContactOptions> contactOptions,
        TimeProvider clock,
        ILogger<AdminAccount> log,
        StepUpRequest request)
    {
        var options = adminOptions.Value;
        var now = clock.GetUtcNow().UtcDateTime;
        var who = AdminIdentity.Of(http);

        var account = await db.AdminAccounts.FirstOrDefaultAsync(x => x.Id == id, http.RequestAborted);
        if (account is null)
        {
            return JsonResults.Missing();
        }

        // Your own authenticator is not reset from a session that proves you
        // still have it. Losing it is exactly the case where you cannot sign
        // in to do this, so it is always somebody else's click.
        if (account.Id == who.AccountId)
        {
            return JsonResults.Invalid("self");
        }

        // A reset would put a disabled account back into a sign-in flow.
        if (await AdminAccess.IsDisabledAsync(db, account, http.RequestAborted))
        {
            return JsonResults.Invalid("disabled");
        }

        if (!await AdminStepUp.VerifyAsync(http, db, request.Code, options, now, log))
        {
            return JsonResults.Invalid("code");
        }

        var expiresAt = now.AddHours(options.EnrollHours);
        var username = account.Username;
        var displayName = account.DisplayName;

        // Only the transaction is retried. The step-up above has already spent
        // the operator's code, once, and a retry must not ask for it again.
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                await ApplyResetAsync(db, account, who, now, expiresAt);
                break;
            }
            catch (Exception ex) when (LostRace(ex))
            {
                db.ChangeTracker.Clear();

                if (attempt == 2)
                {
                    log.LogWarning("admin.reset_conflict account={AccountId}", id);
                    return JsonResults.Stale();
                }

                // Read again: the failed attempt changed the tracked copy, and
                // the changes it saved were rolled back.
                if (await db.AdminAccounts.FirstOrDefaultAsync(x => x.Id == id, CancellationToken.None) is not { } again)
                {
                    return JsonResults.Missing();
                }

                account = again;
            }
        }

        log.LogInformation("admin.authenticator_reset account={AccountId}", id);

        var notified = await NotifyAsync(
            mail,
            AccountNotice.AuthenticatorReset(
                username, displayName, who.DisplayName, now, expiresAt, contactOptions.Value),
            log,
            "authenticator_reset");

        return Results.Json(new { ok = true, notified });
    }

    private static async Task ApplyResetAsync(
        KestridgeDbContext db, AdminAccount account, AdminIdentity who, DateTime now, DateTime expiresAt)
    {
        await using var tx = await db.Database.BeginTransactionAsync(CancellationToken.None);

        // totp_last_step is left alone here. The account cannot pass a code
        // while its secret is empty, and the enrolment sets the step afresh
        // from the first code of the new secret.
        //
        // The secret is marked modified whatever the tracked value is. The row
        // was read before this transaction, and for an account already waiting
        // on a reset it was read empty, so assigning empty would save nothing.
        // A setup completed between that read and here would then keep its
        // secret through the reset meant to take it away.
        account.TotpSecret = string.Empty;
        db.Entry(account).Property(x => x.TotpSecret).IsModified = true;
        AdminAccess.ClearFailures(db, account);
        await db.SaveChangesAsync(CancellationToken.None);

        // Whoever holds the lost phone may also hold an open session.
        await db.AdminSessions.Where(x => x.AccountId == account.Id).ExecuteDeleteAsync(CancellationToken.None);

        // A plain read, then an update in place or an insert. Never a DELETE by
        // account_id followed by an INSERT: when no row exists, that DELETE
        // takes a gap lock on uk_admin_enrollments_account, two resets of
        // different accounts in the same gap each hold one, and each INSERT
        // then waits for the other's until InnoDB kills one with a deadlock.
        var existing = await db.AdminEnrollments.FirstOrDefaultAsync(
            x => x.AccountId == account.Id, CancellationToken.None);

        if (existing is null)
        {
            db.AdminEnrollments.Add(new AdminEnrollment
            {
                AccountId = account.Id,
                DisplayName = account.DisplayName,
                CreatedBy = AdminAccess.Attribution(who.DisplayName),
                CreatedByAccountId = who.AccountId,
                CreatedAt = now,
                ExpiresAt = expiresAt,
            });
        }
        else
        {
            // Everything a fresh row would have. The token and the proposed
            // secret go, so a setup started before this reset is dead.
            existing.DisplayName = account.DisplayName;
            existing.CreatedBy = AdminAccess.Attribution(who.DisplayName);
            existing.CreatedByAccountId = who.AccountId;
            existing.CreatedAt = now;
            existing.ExpiresAt = expiresAt;
            existing.TokenHash = null;
            existing.TokenExpiresAt = null;
            existing.TotpSecret = string.Empty;
            AdminAccess.ClearFailures(db, existing);
        }

        await db.SaveChangesAsync(CancellationToken.None);
        await tx.CommitAsync(CancellationToken.None);
    }

    // A reset of the same account that inserted first (1062), or InnoDB
    // choosing this transaction as a deadlock victim (1213). Both are a race
    // lost, not a fault: the whole transaction was rolled back, and a second
    // attempt reads what the winner wrote. SaveChanges wraps the server error
    // in a DbUpdateException and ExecuteDelete does not, and the execution
    // strategy wraps a transient error, which a deadlock is, in an
    // InvalidOperationException on top of either. So the whole chain is
    // searched: checking one level down never saw a deadlock at all.
    private static bool LostRace(Exception ex)
    {
        for (Exception? e = ex; e is not null; e = e.InnerException)
        {
            if (e is MySqlException { Number: DuplicateKeyErrorNumber or DeadlockErrorNumber })
            {
                return true;
            }
        }

        return false;
    }

    // No step-up. Disabling only ever takes access away, and asking for a code
    // would slow down the one action you want to be fast when an account is
    // being misused.
    public static async Task<IResult> Disable(
        long id, HttpContext http, KestridgeDbContext db, TimeProvider clock, ILogger<AdminAccount> log)
    {
        var who = AdminIdentity.Of(http);

        var account = await db.AdminAccounts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, http.RequestAborted);
        if (account is null)
        {
            return JsonResults.Missing();
        }

        // Nobody can disable themselves, so the panel always keeps at least
        // the person who is using it.
        if (account.Id == who.AccountId)
        {
            return JsonResults.Invalid("self");
        }

        if (await AdminAccess.IsDisabledAsync(db, account, http.RequestAborted))
        {
            return JsonResults.Ok();
        }

        try
        {
            await using var tx = await db.Database.BeginTransactionAsync(CancellationToken.None);

            db.AdminDisables.Add(new AdminDisable
            {
                AccountId = account.Id,
                DisabledAt = clock.GetUtcNow().UtcDateTime,
                DisabledBy = AdminAccess.Attribution(who.DisplayName),
            });

            await db.SaveChangesAsync(CancellationToken.None);

            // The token filter would refuse these on the next request anyway.
            // Deleting them as well means nothing depends on that check alone.
            await db.AdminSessions.Where(x => x.AccountId == account.Id).ExecuteDeleteAsync(CancellationToken.None);
            await db.AdminEnrollments.Where(x => x.AccountId == account.Id).ExecuteDeleteAsync(CancellationToken.None);

            // New-user invitations this account created. The person being
            // disabled chose their initial passwords and may still know them,
            // so each one is a way back in, as a new account, for as long as
            // the invitation lives. Read first and deleted by primary key, for
            // the locking reason ApplyResetAsync gives.
            var invitations = await db.AdminEnrollments
                .Where(x => x.AccountId == null && x.CreatedByAccountId == account.Id)
                .Select(x => x.Id)
                .ToListAsync(CancellationToken.None);

            if (invitations.Count > 0)
            {
                await db.AdminEnrollments
                    .Where(x => x.AccountId == null && invitations.Contains(x.Id))
                    .ExecuteDeleteAsync(CancellationToken.None);
            }

            await tx.CommitAsync(CancellationToken.None);
        }
        catch (DbUpdateException ex) when (ex.InnerException is MySqlException { Number: DuplicateKeyErrorNumber })
        {
            // Another operator disabled the same account a moment earlier.
            return JsonResults.Ok();
        }

        log.LogInformation("admin.user_disabled account={AccountId}", account.Id);

        return JsonResults.Ok();
    }

    // New users only. A pending reset belongs to an account, and an account is
    // never deleted from the panel: handled_by and dsr_log carry its name.
    public static async Task<IResult> DeletePending(long id, KestridgeDbContext db, ILogger<AdminAccount> log)
    {
        var deleted = await db.AdminEnrollments
            .Where(x => x.Id == id && x.AccountId == null)
            .ExecuteDeleteAsync(CancellationToken.None);

        if (deleted == 0)
        {
            return JsonResults.Missing();
        }

        log.LogInformation("admin.pending_deleted enrollment={EnrollmentId}", id);

        return JsonResults.Ok();
    }

    private static PendingUserRow PendingRow(AdminEnrollment row, DateTime now) => new(
        row.Id,
        row.Username ?? string.Empty,
        row.DisplayName,
        row.CreatedBy,
        row.CreatedAt,
        row.ExpiresAt,
        row.ExpiresAt <= now,

        // The same rule as accounts: only a lock that still holds. A locked
        // invitation refuses even the right temporary password, and without
        // this nothing in the panel says why.
        row.LockedUntil is { } until && until > now ? until : null);

    // After the commit, and best effort. The account exists whether or not
    // SMTP is up, so a mail failure is reported as notified=false rather than as
    // an error that would tempt the operator to create the user twice.
    private static async Task<bool> NotifyAsync(IEmailSender mail, MimeMessage message, ILogger log, string kind)
    {
        try
        {
            // Not RequestAborted: closing the tab must not cancel the one
            // guard that tells the rest of the team.
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            await mail.SendAsync(message, timeout.Token);
            return true;
        }
        catch (Exception)
        {
            // No exception text. MailKit messages can carry addresses and
            // server responses, neither of which belongs in journald.
            log.LogWarning("admin.notice_failed kind={Kind}", kind);
            return false;
        }
    }
}
