using System.Threading.RateLimiting;
using Kestridge.Api.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;

namespace Kestridge.Api.Infrastructure;

public static class RateLimiting
{
    public static void Configure(RateLimiterOptions options, RateLimitOptions limits)
    {
        // Chained, and per-IP first, because the order is load bearing. As two
        // independent limiters (an endpoint policy plus a global one) the
        // middleware acquires the global lease first and merely disposes it when
        // the per-IP limiter refuses. FixedWindowRateLimiter does not refund a
        // disposed lease, so requests already rejected per IP still burn global
        // permits: one bot draining 200 in a few seconds would return 429 to
        // every real visitor for the rest of the hour, with /api/health still
        // green so nobody is paged. CreateChained never reaches the second
        // limiter once the first refuses.
        var perClient = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            IsExempt(context)
                ? RateLimitPartition.GetNoLimiter("exempt")
                : RateLimitPartition.GetFixedWindowLimiter(
                    ClientPartitionKey.For(context),
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = limits.PermitsPerWindow,
                        Window = TimeSpan.FromMinutes(limits.WindowMinutes),
                        QueueLimit = 0,
                    }));

        // Bounds a distributed run from many addresses, which the per-IP
        // partition cannot see.
        var wholeSite = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            IsExempt(context)
                ? RateLimitPartition.GetNoLimiter("exempt")
                : RateLimitPartition.GetFixedWindowLimiter(
                    "global",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = limits.GlobalPerHour,
                        Window = TimeSpan.FromHours(1),
                        QueueLimit = 0,
                    }));

        options.GlobalLimiter = PartitionedRateLimiter.CreateChained(perClient, wholeSite);

        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        options.OnRejected = (context, _) =>
        {
            context.HttpContext.Response.Headers.RetryAfter = "600";
            return ValueTask.CompletedTask;
        };
    }

    // Health is what the uptime monitor polls, so it is exempt in both limiters.
    // DisableRateLimiting on the endpoint would not do this: it suppresses
    // endpoint policies only, never the global limiter.
    private static bool IsExempt(HttpContext context) =>
        context.Request.Path.StartsWithSegments("/api/health");
}
