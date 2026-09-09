namespace Kestridge.Api.Data;

// The six content tables. One file, because they are the same shape repeated
// and splitting them would be six files of eight properties each.
//
// Every one carries sort_order, updated_at and updated_by. Order is currently
// implicit in the array order of src/data/*.ts, and MySQL guarantees no order
// without an ORDER BY, so it has to become a column the moment the arrays move
// into a table.
//
// No class, className, style or colour column exists here, deliberately. The
// moment the database can carry a class name the design can no longer be
// changed from the design files.

// Implemented by the four reorderable sets so one reorder helper can serve all
// of them. Steps and highlights are not reorderable on their own: they are
// replaced wholesale as part of the service that owns them.
public interface ISortableContent
{
    long Id { get; }

    int SortOrder { get; set; }
}

public sealed class SiteFaq : ISortableContent
{
    public long Id { get; set; }

    public string Question { get; set; } = string.Empty;

    public string Answer { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = string.Empty;
}

public sealed class SiteTeamMember : ISortableContent
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Initials { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public string Focus { get; set; } = string.Empty;

    // A basename only, validated against a compiled allowlist. /api/content
    // assembles "/team/<photo>.jpg", so the column never holds a path and a
    // stored value can never point off the site.
    public string Photo { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = string.Empty;
}

public sealed class SiteCompany : ISortableContent
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string LogoFile { get; set; } = string.Empty;

    // Hidden rather than published, so the boolean convention used everywhere
    // else in this model holds: HasDefaultValue(false), and the default is the
    // visible state.
    public bool Hidden { get; set; }

    public int SortOrder { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = string.Empty;
}

public sealed class SiteService : ISortableContent
{
    public long Id { get; set; }

    // Not editable and not creatable. The same string is a DOM id, an ARIA
    // target, a CustomEvent payload, the contact form's select value and a
    // member of Kestridge:Contact:AllowedServices. A new one is a 400 at the
    // live API until someone edits appsettings.Production.json and restarts.
    public string Slug { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Tagline { get; set; } = string.Empty;

    public string CardLabel { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    // A key into SERVICE_ICONS in src/lib/icons.ts, not a component and not a
    // path. Validated against the same four names the client registry holds.
    public string IconName { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = string.Empty;
}

// Separate tables rather than a delimited column, so the per-item length cap
// and the per-item order are enforced by the schema instead of by parsing a
// blob. ServiceId carries no foreign key, matching this model's rule that there
// are no relationships anywhere: the reader issues three queries and joins in
// memory.
public sealed class SiteServiceStep
{
    public long Id { get; set; }

    public long ServiceId { get; set; }

    public string Phase { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    // Serialized as "what" to match the DeliveryStep type in src/data/services.ts.
    public string Detail { get; set; } = string.Empty;

    public int SortOrder { get; set; }
}

public sealed class SiteServiceHighlight
{
    public long Id { get; set; }

    public long ServiceId { get; set; }

    public string Text { get; set; } = string.Empty;

    public int SortOrder { get; set; }
}
