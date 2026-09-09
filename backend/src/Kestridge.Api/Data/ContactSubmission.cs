namespace Kestridge.Api.Data;

public enum NotifyState
{
    Pending,
    Sent,
    Failed,
}

public sealed class ContactSubmission
{
    private DateTime _createdAt;
    private DateTime? _notifyNextAttemptAt;
    private DateTime? _notifiedAt;
    private DateTime? _handledAt;

    public long Id { get; set; }

    // datetime(6) holds microseconds, DateTime holds 100ns ticks. Truncating on
    // the way in keeps the in-memory value equal to what the row will return.
    public DateTime CreatedAt
    {
        get => _createdAt;
        set => _createdAt = Micro(value);
    }

    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Service { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string DedupeKey { get; set; } = string.Empty;

    public bool LegalHold { get; set; }
    public DateOnly PurgeAfter { get; set; }

    public NotifyState NotifyState { get; set; } = NotifyState.Pending;
    public byte NotifyAttempts { get; set; }

    public DateTime? NotifyNextAttemptAt
    {
        get => _notifyNextAttemptAt;
        set => _notifyNextAttemptAt = value is null ? null : Micro(value.Value);
    }

    public DateTime? NotifiedAt
    {
        get => _notifiedAt;
        set => _notifiedAt = value is null ? null : Micro(value.Value);
    }

    public string NotifyError { get; set; } = string.Empty;

    // Set from the admin panel when someone has replied. Null means new. Not an
    // enum: "handled" is a timestamp and a person, and the two are always set
    // and cleared together.
    public DateTime? HandledAt
    {
        get => _handledAt;
        set => _handledAt = value is null ? null : Micro(value.Value);
    }

    public string HandledBy { get; set; } = string.Empty;

    public static DateTime Micro(DateTime value)
        => new(value.Ticks - (value.Ticks % 10), value.Kind);
}
