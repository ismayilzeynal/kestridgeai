namespace Kestridge.Api.Data;

// Rows come from two places: the panel, when a new user completes first sign-in
// (AdminEnroll), and ops/admin-account.sql run as the migrator, which is how the
// first account and every recovery are made.
//
// kestridge_app holds SELECT and INSERT, plus a column-level UPDATE on the lockout
// counters, last_login_at, totp_last_step and totp_secret, and no DELETE. So a
// password is written once, by INSERT, and a compromised process can never
// rewrite a hash, rename an operator, re-enable a disabled account or delete one.
// It can clear or set totp_secret, which the authenticator reset needs; a new
// secret is useless without the password it cannot change.
//
// Never db.AdminAccounts.Update(entity): EF would emit every column and the
// grant refuses that with ERROR 1143. Mutate tracked properties only.
public sealed class AdminAccount : IFailureCounted
{
    private DateTime _createdAt;
    private DateTime? _firstFailedAt;
    private DateTime? _lockedUntil;
    private DateTime? _lastLoginAt;

    public long Id { get; set; }

    // ascii_bin, so the login handler lowercases before lookup, and both the
    // panel create path and admin-account.sql store lowercase. Same pattern as
    // contact email.
    public string Username { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    // Base32, as an authenticator app expects it. Empty means no authenticator:
    // Totp.Verify refuses an empty secret, so such an account cannot sign in
    // until a reset enrolment gives it one.
    public string TotpSecret { get; set; } = string.Empty;

    // Replay prevention. Without it a code seen over someone's shoulder is
    // reusable for the rest of its 30 second step.
    public ulong TotpLastStep { get; set; }

    public bool Disabled { get; set; }

    public ushort FailedAttempts { get; set; }

    public DateTime? FirstFailedAt
    {
        get => _firstFailedAt;
        set => _firstFailedAt = value is null ? null : ContactSubmission.Micro(value.Value);
    }

    // In a column, not in IMemoryCache and not in a limiter partition, so a
    // lockout survives the restart that every deploy performs.
    public DateTime? LockedUntil
    {
        get => _lockedUntil;
        set => _lockedUntil = value is null ? null : ContactSubmission.Micro(value.Value);
    }

    public DateTime CreatedAt
    {
        get => _createdAt;
        set => _createdAt = ContactSubmission.Micro(value);
    }

    public DateTime? LastLoginAt
    {
        get => _lastLoginAt;
        set => _lastLoginAt = value is null ? null : ContactSubmission.Micro(value.Value);
    }
}
