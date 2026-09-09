using Kestridge.Api.Data;
using Kestridge.Api.Infrastructure;
using Kestridge.Api.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Kestridge.Api.Admin;

// An endpoint filter, not authentication middleware. Calling AddAuthentication
// makes WebApplication auto-insert UseAuthentication and UseAuthorization at
// the head of the pipeline, ahead of UseExceptionHandler and UseCors. A 401
// would then leave with no Access-Control-Allow-Origin and outside the
// exception handler, and a MySQL token lookup would run before UseRateLimiter.
// A filter runs inside endpoint invocation, which is after both by construction.
//
// Because it short-circuits before the handler body runs, a 401 guarantees the
// mutation did not happen. The whole client-side retry story rests on that.
public sealed class AdminTokenFilter(
    KestridgeDbContext db,
    IOptions<AdminOptions> options,
    TimeProvider clock) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var http = context.HttpContext;

        if (!AdminOriginCheck.SameOrigin(http.Request))
        {
            return JsonResults.Origin();
        }

        var token = AdminSessions.TokenFrom(http.Request.Headers.Authorization.ToString());
        if (token is null)
        {
            return JsonResults.Auth();
        }

        var now = clock.GetUtcNow().UtcDateTime;
        var hash = AdminSessions.Hash(token);

        // Lookup by primary key. An indexed lookup on a hash is constant time by
        // construction, which is why FixedTimeEquals is moot here. Never scan
        // the table and compare in memory.
        var found = await (
            from s in db.AdminSessions
            join a in db.AdminAccounts on s.AccountId equals a.Id
            where s.TokenHash == hash
            select new { Session = s, Account = a }).FirstOrDefaultAsync(http.RequestAborted);

        if (found is null || found.Account.Disabled || !AdminSessions.IsLive(found.Session, now))
        {
            return JsonResults.Auth();
        }

        if (AdminSessions.ShouldSlide(found.Session, now, options.Value))
        {
            AdminSessions.Slide(found.Session, now, options.Value);
            await db.SaveChangesAsync(CancellationToken.None);
        }

        http.Items[AdminIdentity.ItemKey] = new AdminIdentity(
            found.Account.Id, found.Account.Username, found.Account.DisplayName, hash);

        return await next(context);
    }
}
