using System.Text;

namespace Kestridge.Api.Admin;

public sealed record ContentError(string Field, string Reason);

// Text typed into the panel and stored in MySQL bypasses every check in the
// repository and renders straight into the marketing page. StyleTests walks
// source files; it cannot see a database row. This is the same rule set,
// applied on the write path, and it is the only place it can be applied.
//
// Reject, never silently rewrite, with one exception: the ends are trimmed. A
// server that quietly rewrote text would show the operator something different
// from what they typed after the next reload, and they would never find out
// which of their words the machine changed. The browser normalizes visibly
// before it ever gets here and says what it changed.
public static class ContentValidation
{
    // The identical code point list as StyleTests.Dashes.
    // Escaped, not literal, for the obvious reason: this file is itself scanned
    // by the test that forbids them.
    private static readonly char[] Dashes = ['\u2012', '\u2013', '\u2014', '\u2015'];

    // Mirrors what is committed under /public/team and /public/logos. The API
    // cannot see Vercel's filesystem, so this is a compiled list on purpose: a
    // typo then fails at the panel instead of shipping a broken image. Adding a
    // photo is a git commit in two places, which it already was, because the
    // JPEG has to be committed anyway.
    public static readonly string[] Photos =
        ["chingiz-abdilov", "faig-garayev", "robert-tomczyk", "sarvjeet"];

    public static readonly string[] Logos =
    [
        "anthem", "aon", "avanade", "clark-university", "cleveland-clinic",
        "community-health-systems", "constellation-energy", "discovery", "ecolab",
        "eigroup", "ey", "infosys", "ipsos", "ministry-of-taxes-az",
        "northwestern-university", "robert-morris-university", "sahara-india",
        "sears", "sopra-steria", "synovate",
    ];

    // Exactly the four keys in SERVICE_ICONS in src/lib/icons.ts. The picker
    // serves this list and the write path validates against it, so the panel
    // cannot produce a name the site does not know.
    public static readonly string[] ServiceIcons =
        ["Brain", "BarChart3", "Workflow", "ShieldCheck"];

    /// <summary>Trims the ends, then rejects. Returns null when the value is usable.</summary>
    public static ContentError? Text(string field, string? value, int max, out string clean)
    {
        clean = (value ?? string.Empty).Trim();

        // Before the control check, because \r and \n are themselves C0
        // controls and "newline" is the reason an operator can act on. Nothing
        // on the site renders white-space: pre-wrap, so a stored newline
        // collapses to a space in the HTML while appearing verbatim in the FAQ
        // structured data. Rejecting it keeps the two identical.
        if (clean.Contains('\n', StringComparison.Ordinal) || clean.Contains('\r', StringComparison.Ordinal))
        {
            return new ContentError(field, "newline");
        }

        if (clean.IndexOfAny(Dashes) >= 0)
        {
            return new ContentError(field, "dash");
        }

        // JSON.stringify does not escape </script>, and FAQ.tsx feeds its
        // output to dangerouslySetInnerHTML. FAQ.tsx escapes "<" at render as
        // well; both halves are required, and neither is sufficient alone.
        if (clean.Contains('<', StringComparison.Ordinal) || clean.Contains('>', StringComparison.Ordinal))
        {
            return new ContentError(field, "angle");
        }

        foreach (var rune in clean.EnumerateRunes())
        {
            if (IsEmoji(rune.Value))
            {
                return new ContentError(field, "emoji");
            }

            if (IsControl(rune.Value))
            {
                return new ContentError(field, "control");
            }
        }

        if (clean.Length == 0 || clean.Length > max)
        {
            return new ContentError(field, "length");
        }

        return null;
    }

    /// <summary>A basename, lowercase, that names a file committed to /public.</summary>
    public static ContentError? Asset(string field, string? value, string[] allowlist, out string clean)
    {
        clean = (value ?? string.Empty).Trim();

        if (clean.Length == 0 || clean.Length > 60 || !clean.All(IsBasenameChar))
        {
            return new ContentError(field, "asset");
        }

        return allowlist.Contains(clean, StringComparer.Ordinal) ? null : new ContentError(field, "asset");
    }

    public static ContentError? Initials(string field, string? value, out string clean)
    {
        clean = (value ?? string.Empty).Trim();

        return clean.Length is >= 1 and <= 3 && clean.All(c => c is >= 'A' and <= 'Z')
            ? null
            : new ContentError(field, "length");
    }

    public static ContentError? Icon(string field, string? value, out string clean)
    {
        clean = (value ?? string.Empty).Trim();

        return ServiceIcons.Contains(clean, StringComparer.Ordinal) ? null : new ContentError(field, "asset");
    }

    // The identical ranges as StyleTests.IsEmoji.
    private static bool IsEmoji(int value) =>
        (value >= 0x1F300 && value <= 0x1FAFF)
        || (value >= 0x2600 && value <= 0x27BF)
        || value == 0xFE0F;

    // C0 and C1. Tab is included: it is invisible in the panel, survives into
    // the column, and renders as a space, so it is a difference nobody can see.
    private static bool IsControl(int value) =>
        value < 0x20 || (value >= 0x7F && value <= 0x9F);

    private static bool IsBasenameChar(char c) =>
        c is (>= 'a' and <= 'z') or (>= '0' and <= '9') or '-';
}
