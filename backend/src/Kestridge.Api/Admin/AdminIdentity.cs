using Microsoft.AspNetCore.Http;

namespace Kestridge.Api.Admin;

// Put in HttpContext.Items by AdminTokenFilter. Not a ClaimsPrincipal: nothing
// in this app calls AddAuthentication, on purpose, because doing so makes
// WebApplication auto-insert UseAuthentication ahead of UseExceptionHandler and
// UseCors, which would send 401s out with no Access-Control-Allow-Origin and
// outside the exception handler.
public readonly record struct AdminIdentity(long AccountId, string Username, string DisplayName, string TokenHash)
{
    public const string ItemKey = "admin";

    public static AdminIdentity Of(HttpContext context)
        => (AdminIdentity)context.Items[ItemKey]!;
}
