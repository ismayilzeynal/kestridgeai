using System.Text;

namespace Kestridge.Api.Email;

public static class HeaderSafe
{
    // CR, LF and TAB collapse to a single space rather than vanishing. Dropping
    // them outright welds "Bob" and "Bcc: attacker@example.com" into one token
    // that reads like a header in the rendered display name. Every other C0
    // control is removed. MimeKit encoding is the second layer, not the first.
    public static string Clean(string value, int max)
    {
        var sb = new StringBuilder(value.Length);
        var lastWasSpace = false;

        foreach (var c in value)
        {
            var isSpace = char.IsWhiteSpace(c);

            if (!isSpace && (char.IsControl(c) || c == '\u007f'))
            {
                continue;
            }

            if (isSpace)
            {
                if (lastWasSpace || sb.Length == 0)
                {
                    continue;
                }

                lastWasSpace = true;
                sb.Append(' ');
                continue;
            }

            lastWasSpace = false;
            sb.Append(c);
        }

        var result = sb.ToString().TrimEnd();
        return result.Length <= max ? result : result[..max].TrimEnd();
    }
}
