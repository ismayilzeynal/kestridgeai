namespace Kestridge.Api.Admin;

// Requests. Every id is nullable so that "create" and "edit" are the same
// endpoint, and the handler is what refuses a null id for the sets where
// creation is not allowed. That keeps the refusal in one readable place
// instead of in the shape of the type.
public sealed record FaqSaveRequest(long? Id, string? Question, string? Answer);

public sealed record ContentIdRequest(long Id);

public sealed record ReorderRequest(long[]? Ids);

public sealed record TeamSaveRequest(
    long? Id, string? Name, string? Initials, string? Role, string? Focus, string? Photo);

public sealed record CompanySaveRequest(long? Id, string? Name, bool Hidden);

public sealed record CompanyAddRequest(string? LogoFile, string? Name);

public sealed record ServiceStepInput(string? Phase, string? Summary, string? What);

// Slug is absent from this type on purpose, not merely ignored: a service slug
// is a DOM id, an ARIA target, a CustomEvent payload, the contact form's select
// value and a member of Kestridge:Contact:AllowedServices. There is no body
// shape that can carry a new one.
public sealed record ServiceSaveRequest(
    long? Id,
    string? Name,
    string? Tagline,
    string? CardLabel,
    string? Description,
    string? IconName,
    string[]? Highlights,
    ServiceStepInput[]? Steps);

// Responses. These carry id, sortOrder, updatedAt and updatedBy, which the
// public payload deliberately does not.
public sealed record FaqAdminRow(long Id, string Question, string Answer, int SortOrder, DateTime UpdatedAt, string UpdatedBy);

public sealed record TeamAdminRow(
    long Id, string Name, string Initials, string Role, string Focus, string Photo,
    int SortOrder, DateTime UpdatedAt, string UpdatedBy);

public sealed record CompanyAdminRow(
    long Id, string Name, string LogoFile, bool Hidden, int SortOrder, DateTime UpdatedAt, string UpdatedBy);

public sealed record ServiceStepRow(string Phase, string Summary, string What);

public sealed record ServiceAdminRow(
    long Id,
    string Slug,
    string Name,
    string Tagline,
    string CardLabel,
    string Description,
    string IconName,
    string[] Highlights,
    ServiceStepRow[] Steps,
    int SortOrder,
    DateTime UpdatedAt,
    string UpdatedBy);
