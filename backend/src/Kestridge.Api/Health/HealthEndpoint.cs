using Kestridge.Api.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace Kestridge.Api.Health;

public static class HealthEndpoint
{
    public static void MapHealth(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/health", Handle).DisableRateLimiting();
    }

    // No counts, no version, no host name, no exception text. This is what the
    // uptime monitor watches and what the deploy script smoke-tests.
    private static async Task<IResult> Handle(KestridgeDbContext db, CancellationToken ct)
    {
        try
        {
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
            timeout.CancelAfter(TimeSpan.FromSeconds(3));

            return await db.Database.CanConnectAsync(timeout.Token)
                ? Results.Json(new { status = "ok" })
                : Results.Json(new { status = "degraded" }, statusCode: StatusCodes.Status503ServiceUnavailable);
        }
        catch (Exception)
        {
            return Results.Json(new { status = "degraded" }, statusCode: StatusCodes.Status503ServiceUnavailable);
        }
    }
}
