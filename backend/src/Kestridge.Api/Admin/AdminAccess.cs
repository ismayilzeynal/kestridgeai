using System.Runtime.CompilerServices;
using Kestridge.Api.Data;
using Kestridge.Api.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Kestridge.Api.Admin;

// The account state checks that every door into the panel has to agree on:
// password sign-in, first sign-in, the token filter and the step-up code. One
// copy, because a check applied on four paths and forgotten on the fifth is the
// fifth path's vulnerability.
public static class AdminAccess
{
    private static readonly ConditionalWeakTable<IPasswordHasher<AdminAccount>, string> DummyHashes = new();

    // Verified against whenever there is no real hash to check, so the response
    // time does not distinguish a real account from a guess. A Task.Delay would
    // be worse than useless: it holds a connection open and is itself a denial
    // of service lever on a public endpoint.
    //
    // Made by the application's own hasher, once per hasher, because the
    // iteration count is stored inside the hash and verification uses that
    // count. A hash made with the library default would verify in half the
    // time of a real account at the configured 210,000, and the difference is
    // exactly what this exists to hide.
    public static string DummyHash(IPasswordHasher<AdminAccount> hasher)
        => DummyHashes.GetValue(hasher, h => h.HashPassword(new AdminAccount(), "timing-equalizer"));

    // Two sources, both honoured. admin_accounts.disabled is what the migrator's
    // ops/admin-disable.sql sets. admin_disables is what the panel writes,
    // because kestridge_app holds no UPDATE on the disabled column and can only
    // append to that table, so it can never undo a disable it made.
    public static async Task<bool> IsDisabledAsync(KestridgeDbContext db, AdminAccount account, CancellationToken ct)
        => account.Disabled || await db.AdminDisables.AnyAsync(x => x.AccountId == account.Id, ct);

    public static bool IsLocked(IFailureCounted row, DateTime now)
        => row.LockedUntil is { } until && until > now;

    // Counts one attempt against the row before the password or code is
    // checked, and answers whether the attempt may go on. False means the row
    // is locked, or gone, and the caller refuses without looking further.
    //
    // One conditional UPDATE, not a read and a write. Counted in memory and
    // saved, every attempt that read the row before the first of them saved
    // wrote the same count, so fifty guesses sent together moved it by one and
    // the lockout never came. Here the database serialises the attempts on the
    // row lock, the WHERE refuses every attempt once the limit is reached, and
    // the attempt that passes is already counted whatever happens next. The
    // caller clears the counters on success.
    //
    // The counter is a one hour window, not a lifetime total, so a wrong code
    // last March does not contribute to a lockout today.
    //
    // MySQL applies SET assignments left to right, and an expression sees the
    // new value of every column assigned before it. So the statement has to
    // assign locked_until first, then failed_attempts, then first_failed_at:
    // each expression reads only its own column and columns assigned after it,
    // so every one of them sees the row as it was. In any other order the lock
    // lands one attempt early and an expired window is never restarted.
    //
    // EF Core 9 emits the SET list in the reverse of the order the SetProperty
    // calls are chained, which is why they are written here as first_failed_at,
    // failed_attempts, locked_until. Read the statement in the MySQL general
    // log after any EF upgrade. Start_WrongPasswords_CountInAnHourWindowAndLockAtTheLimit
    // fails if the order ever comes out wrong.
    //
    // Only the three counter columns are written, which keeps it inside
    // kestridge_app's column grant on admin_accounts.
    public static async Task<bool> ChargeAsync<T>(IQueryable<T> row, DateTime now, AdminOptions options)
        where T : class, IFailureCounted
    {
        var windowStart = now.AddHours(-1);
        var lockUntil = now.AddMinutes(options.LockMinutes);
        var max = options.MaxFailedAttempts;

        var charged = await row
            .Where(x => x.LockedUntil == null || x.LockedUntil <= now)
            .ExecuteUpdateAsync(
                s => s
                    .SetProperty(
                        x => x.FirstFailedAt,
                        x => x.FirstFailedAt == null || x.FirstFailedAt <= windowStart ? now : x.FirstFailedAt)
                    .SetProperty(
                        x => x.FailedAttempts,
                        x => x.FirstFailedAt == null || x.FirstFailedAt <= windowStart
                            ? (ushort)1
                            : x.FailedAttempts < ushort.MaxValue ? (ushort)(x.FailedAttempts + 1) : x.FailedAttempts)
                    .SetProperty(
                        x => x.LockedUntil,
                        x => (x.FirstFailedAt == null || x.FirstFailedAt <= windowStart ? 1 : x.FailedAttempts + 1) >= max
                            ? lockUntil
                            : x.LockedUntil),
                CancellationToken.None);

        return charged == 1;
    }

    // Marked modified whatever the tracked values are. ChargeAsync writes past
    // the change tracker, so the row in memory may still hold the zeros it was
    // read with, and assigning zero to a zero would otherwise save nothing and
    // leave the charge in place.
    public static void ClearFailures(KestridgeDbContext db, IFailureCounted row)
    {
        row.FailedAttempts = 0;
        row.FirstFailedAt = null;
        row.LockedUntil = null;

        var entry = db.Entry(row);
        entry.Property(nameof(IFailureCounted.FailedAttempts)).IsModified = true;
        entry.Property(nameof(IFailureCounted.FirstFailedAt)).IsModified = true;
        entry.Property(nameof(IFailureCounted.LockedUntil)).IsModified = true;
    }

    // The same cut to 64 as handled_by and updated_by, for the same varchar(64)
    // column width, so a stamp can never fail an insert under strict mode.
    public static string Attribution(string displayName)
        => displayName.Length <= 64 ? displayName : displayName[..64];
}
