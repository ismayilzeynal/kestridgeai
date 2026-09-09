namespace Kestridge.Api.Data;

// No Id column. The token hash is the lookup key, so a surrogate key plus a
// unique index would be a second index for nothing. This is the one deliberate
// deviation from the long Id convention in this model.
//
// No ip_address and no user_agent, matching the claim in DSR-PROCESS.md that
// none is stored anywhere. SchemaTests proves it for this table too.
public sealed class AdminSession
{
    private DateTime _createdAt;
    private DateTime _lastSeenAt;
    private DateTime _idleExpiresAt;
    private DateTime _absoluteExpiresAt;

    // Lowercase hex SHA-256 of the token. The token itself is returned to the
    // browser once and never stored, so a database dump, a backup, or a support
    // SELECT yields no live credential.
    public string TokenHash { get; set; } = string.Empty;

    public long AccountId { get; set; }

    public DateTime CreatedAt
    {
        get => _createdAt;
        set => _createdAt = ContactSubmission.Micro(value);
    }

    public DateTime LastSeenAt
    {
        get => _lastSeenAt;
        set => _lastSeenAt = ContactSubmission.Micro(value);
    }

    // Slides on use. Thirty minutes of inactivity ends the session.
    public DateTime IdleExpiresAt
    {
        get => _idleExpiresAt;
        set => _idleExpiresAt = ContactSubmission.Micro(value);
    }

    // Never extended. A stolen token is dead by the next morning whatever the
    // thief does with it.
    public DateTime AbsoluteExpiresAt
    {
        get => _absoluteExpiresAt;
        set => _absoluteExpiresAt = ContactSubmission.Micro(value);
    }
}
