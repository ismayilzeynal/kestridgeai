namespace Kestridge.Api.Data;

// Evidence that the published deletion promise is being kept. Counts only,
// never content, and never purged: it has to outlive the rows it deletes.
public sealed class JobRun
{
    private DateTime _startedAt;
    private DateTime? _finishedAt;

    public long Id { get; set; }

    public string JobName { get; set; } = string.Empty;

    public DateTime StartedAt
    {
        get => _startedAt;
        set => _startedAt = ContactSubmission.Micro(value);
    }

    public DateTime? FinishedAt
    {
        get => _finishedAt;
        set => _finishedAt = value is null ? null : ContactSubmission.Micro(value.Value);
    }

    public string Outcome { get; set; } = "running";
    public DateOnly? CutoffDate { get; set; }
    public uint RowsAffected { get; set; }
    public uint DurationMs { get; set; }
    public string Detail { get; set; } = string.Empty;
}
