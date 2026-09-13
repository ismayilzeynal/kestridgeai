namespace Kestridge.Api.Data;

// A first sign-in that has not happened yet. Two kinds share the table:
//
// - A new user (AccountId null, Username set). The admin_accounts row does not
//   exist until its owner has signed in with the initial password, chosen a
//   new one and proved a working authenticator. Until then nothing about the
//   person is in admin_accounts, which is why deleting a pending user is a
//   DELETE here and never on admin_accounts, where the app holds none.
// - An authenticator reset (AccountId set, Username null). The account keeps
//   its row and its password; only the secret was cleared.
//
// The unique indexes on both columns allow any number of NULLs, so they mean
// one pending new user per username and one pending reset per account.
public sealed class AdminEnrollment : IFailureCounted
{
    private DateTime? _tokenExpiresAt;
    private DateTime _createdAt;
    private DateTime _expiresAt;
    private DateTime? _firstFailedAt;
    private DateTime? _lockedUntil;

    public long Id { get; set; }

    public long? AccountId { get; set; }

    // Lowercase, like AdminAccount.Username, and for the same ascii_bin reason.
    public string? Username { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    // The initial password of a new user, which it is never allowed to keep.
    // Empty for a reset, whose password stays on the account.
    public string PasswordHash { get; set; } = string.Empty;

    // Proposed, not yet proven. Replaced on every login/start, so a secret shown
    // on a screen someone walked away from dies with the next attempt.
    public string TotpSecret { get; set; } = string.Empty;

    // SHA-256 of the setup token, as for sessions: a dump yields nothing usable.
    public string? TokenHash { get; set; }

    public DateTime? TokenExpiresAt
    {
        get => _tokenExpiresAt;
        set => _tokenExpiresAt = value is null ? null : ContactSubmission.Micro(value.Value);
    }

    public string CreatedBy { get; set; } = string.Empty;

    // The operator who created the invitation or the reset. created_by is a
    // display name for people to read, and two people can share one, so this
    // is what lets a disable find the invitations its account made. Their
    // initial passwords are known to the person being disabled.
    public long? CreatedByAccountId { get; set; }

    public DateTime CreatedAt
    {
        get => _createdAt;
        set => _createdAt = ContactSubmission.Micro(value);
    }

    public DateTime ExpiresAt
    {
        get => _expiresAt;
        set => _expiresAt = ContactSubmission.Micro(value);
    }

    // Guessing a new user's initial password is counted here, because there is
    // no account row to count it on yet.
    public ushort FailedAttempts { get; set; }

    public DateTime? FirstFailedAt
    {
        get => _firstFailedAt;
        set => _firstFailedAt = value is null ? null : ContactSubmission.Micro(value.Value);
    }

    public DateTime? LockedUntil
    {
        get => _lockedUntil;
        set => _lockedUntil = value is null ? null : ContactSubmission.Micro(value.Value);
    }
}
