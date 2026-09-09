namespace Kestridge.Api.Data;

// Rows are created by ops/admin-account.sql as the migrator, never by the
// application. kestridge_app holds SELECT plus a column-level UPDATE on the
// five counters below, so a compromised process cannot rewrite a password hash,
// swap a TOTP secret, rename an operator, or re-enable a disabled account.
public sealed class AdminAccount
{
    private DateTime _createdAt;
    private DateTime? _firstFailedAt;
    private DateTime? _lockedUntil;
    private DateTime? _lastLoginAt;

    public long Id { get; set; }

    // ascii_bin, so the login handler lowercases before lookup and
    // admin-account.sql stores lowercase. Same pattern as contact email.
    public string Username { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    // Base32, as an authenticator app expects it.
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
