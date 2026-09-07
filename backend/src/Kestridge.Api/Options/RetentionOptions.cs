using System.ComponentModel.DataAnnotations;

namespace Kestridge.Api.Options;

public sealed class RetentionOptions
{
    public const string Section = "Kestridge:Retention";

    [Range(1, 600)]
    public int Months { get; set; } = 24;

    [Range(0, 23)]
    public int RunHourUtc { get; set; } = 3;

    [Range(1, 100000)]
    public int BatchSize { get; set; } = 500;
}
