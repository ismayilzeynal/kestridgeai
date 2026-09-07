using System.ComponentModel.DataAnnotations;

namespace Kestridge.Api.Options;

public sealed class RateLimitOptions
{
    public const string Section = "Kestridge:RateLimit";

    [Range(1, 100000)]
    public int PermitsPerWindow { get; set; } = 5;

    [Range(1, 1440)]
    public int WindowMinutes { get; set; } = 10;

    [Range(1, 10000000)]
    public int GlobalPerHour { get; set; } = 200;
}
