namespace Kestridge.Api.Data;

// Written by hand from ops/dsr-log.sql. No application code writes this table.
public sealed class DsrLogEntry
{
    public long Id { get; set; }
    public DateOnly ReceivedOn { get; set; }
    public string RequestType { get; set; } = string.Empty;
    public string SubjectEmailHash { get; set; } = string.Empty;
    public uint RowsAffected { get; set; }
    public string AffectedIds { get; set; } = string.Empty;
    public string HandledBy { get; set; } = string.Empty;
    public DateOnly? ClosedOn { get; set; }
}
