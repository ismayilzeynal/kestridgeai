using System.Text.RegularExpressions;

namespace Kestridge.Api.Contact;

// Single source of truth for the CORS policy and for the handler's 403 check.
// The handler check is the one that matters: the browser POST is a CORS simple
// request, so it reaches the handler whatever the response headers say.
// An instance, not statics: two WebApplicationFactory hosts run in parallel in
// the test suite with different settings.
public sealed partial class OriginGuard
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

    private readonly bool allowVercelPreviews;
    private readonly bool allowLocalhost;
    private readonly string[] additional;

    public OriginGuard(bool allowVercelPreviews, bool allowLocalhost, string[]? additionalOrigins = null)
    {
        this.allowVercelPreviews = allowVercelPreviews;
        this.allowLocalhost = allowLocalhost;

        // Exact strings only, no pattern, no wildcard. This exists so a staging
        // host can be reached before its real name and certificate exist, which
        // is the one case the fixed lists above cannot cover. Anything listed
        // here can post to the form, so it is emptied at cutover.
        additional = (additionalOrigins ?? [])
            .Select(o => o.Trim())
            .Where(o => o.Length > 0)
            .ToArray();
    }

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

        if (Array.IndexOf(additional, origin) >= 0)
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
