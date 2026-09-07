using System.ComponentModel.DataAnnotations;

namespace Kestridge.Api.Options;

public sealed class NotifyOptions
{
    public const string Section = "Kestridge:Notify";

    [Range(1, 7)]
    public int MaxAttempts { get; set; } = 7;

    [Range(1, 3600)]
    public int SweepSeconds { get; set; } = 30;

    [Range(1, 500)]
    public int BatchSize { get; set; } = 20;
}
