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
            HasItsOwnPartition(context)
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
            HasItsOwnPartition(context)
                ? RateLimitPartition.GetNoLimiter("exempt")
                : RateLimitPartition.GetFixedWindowLimiter(
                    "global",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = limits.GlobalPerHour,
                        Window = TimeSpan.FromHours(1),
                        QueueLimit = 0,
                    }));

        // Its own partition, keyed on the client and never on a constant. A
        // constant key would let one attacker lock the owner out of his own
        // panel simply by spending the permits.
        var adminLogin = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            IsLoginAttempt(context)
                ? RateLimitPartition.GetFixedWindowLimiter(
                    "login:" + ClientPartitionKey.For(context),
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = limits.LoginPermitsPerWindow,
                        Window = TimeSpan.FromMinutes(limits.WindowMinutes),
                        QueueLimit = 0,
                    })
                : RateLimitPartition.GetNoLimiter("not-login"));

        var adminApi = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            IsAdminSurface(context)
                ? RateLimitPartition.GetFixedWindowLimiter(
                    "admin:" + ClientPartitionKey.For(context),
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = limits.AdminPermitsPerWindow,
                        Window = TimeSpan.FromMinutes(limits.WindowMinutes),
                        QueueLimit = 0,
                    })
                : RateLimitPartition.GetNoLimiter("not-admin"));

        // adminLogin precedes adminApi for the same refund reason as above: a
        // login request matches both, and the tighter limiter has to refuse
        // first. Note that /api/admin is never given a no-limiter partition,
        // which would leave the public password endpoint unbounded.
        options.GlobalLimiter = PartitionedRateLimiter.CreateChained(
            perClient, wholeSite, adminLogin, adminApi);

        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        options.OnRejected = (context, _) =>
        {
            context.HttpContext.Response.Headers.RetryAfter = "600";
            return ValueTask.CompletedTask;
        };
    }

    // Health is what the uptime monitor polls, so it gets no limiter at all.
    // The admin surface gets its own partitions below. What they share is that
    // none of them may spend a permit from the contact form's buckets.
    //
    // Without this the regression is severe and quiet: every panel click would
    // charge the contact form's per-IP bucket of 5 per 10 minutes AND the
    // site-wide 200 per hour, so the owner reading thirty submissions makes the
    // public form answer 429 to real visitors, while /api/health stays green so
    // nothing is paged. Loading the panel alone is four asset requests.
    //
    // DisableRateLimiting on an endpoint would not do this: it suppresses
    // endpoint policies only, never the global limiter.
    private static bool HasItsOwnPartition(HttpContext context) =>
        context.Request.Path.StartsWithSegments("/api/health")
        || context.Request.Path.StartsWithSegments("/api/admin")
        || context.Request.Path.StartsWithSegments("/admin");

    // OPTIONS is excluded on purpose. It is unreachable while the panel is same
    // origin, but charging a preflight would halve the real budget if that ever
    // changed. The contact form needs no such carve out: a multipart POST with
    // no custom headers is a simple request and is never preflighted.
    private static bool IsAdminSurface(HttpContext context) =>
        (context.Request.Path.StartsWithSegments("/api/admin")
         || context.Request.Path.StartsWithSegments("/admin"))
        && !HttpMethods.IsOptions(context.Request.Method);

    private static bool IsLoginAttempt(HttpContext context) =>
        HttpMethods.IsPost(context.Request.Method)
        && context.Request.Path.Equals("/api/admin/login", StringComparison.Ordinal);
}
