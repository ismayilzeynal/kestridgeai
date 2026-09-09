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

    // The admin surface gets its own partitions. Without them every panel click
    // spends the contact form's per-IP budget and the site-wide hourly one, so
    // the owner reading thirty submissions makes the public form answer 429 to
    // real visitors while /api/health stays green and nobody is paged.
    //
    // Both reuse WindowMinutes rather than declaring their own, because
    // RateLimiting.OnRejected hardcodes Retry-After: 600 and two tests assert
    // that exact value. One window everywhere keeps the header truthful.
    [Range(1, 1000)]
    public int LoginPermitsPerWindow { get; set; } = 5;

    [Range(1, 100000)]
    public int AdminPermitsPerWindow { get; set; } = 600;

    // Vercel's ISR revalidation calls /api/content from one egress address. In
    // the contact partition that is 5 calls per 10 minutes before a 429, after
    // which every revalidation falls back to the compiled constants forever and
    // nobody notices, because the site keeps rendering perfectly good copy.
    [Range(1, 100000)]
    public int ContentPermitsPerWindow { get; set; } = 120;
}
