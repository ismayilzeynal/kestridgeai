using System.ComponentModel.DataAnnotations;

namespace Kestridge.Api.Options;

public sealed class ContactOptions
{
    public const string Section = "Kestridge:Contact";

    public bool RequireOrigin { get; set; } = true;

    [MinLength(1)]
    public string[] AllowedServices { get; set; } = [];

    [Range(1, 65535)]
    public int MaxMessageLength { get; set; } = 5000;

    [Range(1, 86400)]
    public int DedupeWindowSeconds { get; set; } = 600;

    [Required, EmailAddress]
    public string ToAddress { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string FromAddress { get; set; } = string.Empty;

    [Required]
    public string FromDisplayName { get; set; } = "Kestridge Website";
}
