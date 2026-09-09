using Microsoft.AspNetCore.Http;

namespace Kestridge.Api.Admin;

// The panel is same origin with the API, so the "site" CORS policy already
// blocks cross-origin admin calls: Authorization is not among its allowed
// headers and GET is not among its allowed methods, so every such call fails
// preflight.
//
// The hole that leaves is a SIMPLE cross-origin POST, which needs no preflight.
// A page on kestridge.com, an allowed origin, could post to /api/admin/login
// and read the reply. This closes it, mirroring OriginGuard rather than adding
// a second CORS policy.
//
// A browser always sends Origin on POST. GET carries none, so reads are
// unaffected and curl keeps working. UseForwardedHeaders has already restored
// the scheme and host from nginx by the time this runs.
public static class AdminOriginCheck
{
    public static bool SameOrigin(HttpRequest request)
    {
        var origin = request.Headers.Origin.ToString();

        return origin.Length == 0
               || string.Equals(origin, $"{request.Scheme}://{request.Host}", StringComparison.Ordinal);
    }
}
