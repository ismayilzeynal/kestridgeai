namespace Kestridge.Api.Data;

// Written by ops/dsr-log.sql on the manual path, and by AdminDsrEndpoints when
// a request is handled in the panel. Both must produce the same
// subject_email_hash, which is why DsrHash exists and is tested against the
// server's own SHA2 output.
//
// kestridge_app holds SELECT and INSERT only: the panel writes the record it is
// obliged to write and can never edit or erase one.
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
