using System.Text.RegularExpressions;

namespace Kestridge.Api.Contact;

// Single source of truth for the CORS policy and for the handler's 403 check.
// The handler check is the one that matters: the browser POST is a CORS simple
// request, so it reaches the handler whatever the response headers say.
// An instance, not statics: two WebApplicationFactory hosts run in parallel in
// the test suite with different settings.
public sealed partial class OriginGuard(bool allowVercelPreviews, bool allowLocalhost)
{
    private static readonly string[] Production =
    [
        "https://kestridge.com",
        "https://www.kestridge.com",
    ];

    private static readonly string[] Development =
    [
        "http://localhost:3000",
        "http://localhost:3111",
        "http://127.0.0.1:3000",
        "http://127.0.0.1:3111",
    ];

    public bool IsAllowed(string? origin)
    {
        if (string.IsNullOrEmpty(origin))
        {
            return false;
        }

        if (Array.IndexOf(Production, origin) >= 0)
        {
            return true;
        }

        if (allowLocalhost && Array.IndexOf(Development, origin) >= 0)
        {
            return true;
        }

        return allowVercelPreviews && VercelPreview().IsMatch(origin);
    }

    // Anchored at both ends. Unanchored, this also matches
    // https://kestridgeai.vercel.app.attacker.com.
    [GeneratedRegex(@"^https://kestridgeai(-[a-z0-9-]+)?\.vercel\.app$", RegexOptions.CultureInvariant)]
    private static partial Regex VercelPreview();
}
