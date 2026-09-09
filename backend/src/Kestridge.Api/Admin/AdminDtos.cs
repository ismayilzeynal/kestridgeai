namespace Kestridge.Api.Admin;

public sealed record LoginRequest(string? Username, string? Password, string? Code);

public sealed record LogoutRequest(bool Everywhere);

public sealed record HandledRequest(bool Handled);

public sealed record LegalHoldRequest(bool LegalHold);

public sealed record SearchRequest(string? Q, string? State);

public sealed record ExportRequest(string? State, bool IncludeMessage);

public sealed record DsrPreviewRequest(string? Email);

public sealed record DsrAccessRequest(string? Email, string? ReceivedOn);

public sealed record DsrDeleteRequest(
    string? Email,
    string? ConfirmEmail,
    string? ReceivedOn,
    string? RequestType,
    long[]? Ids);

// The list is a projection, never the entity. email, phone, message,
// dedupe_key, notify_error and the notify scheduling columns are not shipped to
// a grid that does not display them.
public sealed record SubmissionRow(
    long Id,
    DateTime CreatedAt,
    string Name,
    string Company,
    string Service,
    string Preview,
    string NotifyState,
    bool Handled,
    bool LegalHold);

public sealed record SubmissionDetail(
    long Id,
    DateTime CreatedAt,
    string Name,
    string Email,
    string Company,
    string Phone,
    string Service,
    string Message,
    string NotifyState,
    int NotifyAttempts,
    DateTime? NotifiedAt,
    string NotifyError,
    bool Handled,
    DateTime? HandledAt,
    string HandledBy,
    bool LegalHold,
    DateOnly PurgeAfter);
