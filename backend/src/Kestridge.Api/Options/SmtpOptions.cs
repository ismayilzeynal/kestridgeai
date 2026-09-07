using System.ComponentModel.DataAnnotations;

namespace Kestridge.Api.Options;

public sealed class SmtpOptions
{
    public const string Section = "Kestridge:Smtp";

    [Required]
    public string Host { get; set; } = string.Empty;

    [Range(1, 65535)]
    public int Port { get; set; } = 587;

    public bool UseStartTls { get; set; } = true;

    public string User { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    [Range(1, 120)]
    public int TimeoutSeconds { get; set; } = 15;
}
