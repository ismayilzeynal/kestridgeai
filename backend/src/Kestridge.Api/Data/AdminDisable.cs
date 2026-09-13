namespace Kestridge.Api.Data;

// A disable done from the panel. A row here, rather than admin_accounts.disabled,
// because kestridge_app holds SELECT and INSERT on this table and nothing else:
// the process that disabled an account can never delete the row that disabled
// it. Re-enabling stays a migrator operation. admin_accounts.disabled is still
// honoured, and is what ops/admin-disable.sql sets.
public sealed class AdminDisable
{
    private DateTime _disabledAt;

    public long AccountId { get; set; }

    public DateTime DisabledAt
    {
        get => _disabledAt;
        set => _disabledAt = ContactSubmission.Micro(value);
    }

    public string DisabledBy { get; set; } = string.Empty;
}
