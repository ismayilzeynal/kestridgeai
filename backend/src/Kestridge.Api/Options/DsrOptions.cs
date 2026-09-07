using System.ComponentModel.DataAnnotations;

namespace Kestridge.Api.Options;

public sealed class DsrOptions
{
    public const string Section = "Kestridge:Dsr";

    // Peppers the SHA-256 of a subject address in dsr_log, so a deletion record
    // cannot be used to confirm that a guessed address ever wrote in.
    [Required, MinLength(16)]
    public string EmailHashPepper { get; set; } = string.Empty;
}
