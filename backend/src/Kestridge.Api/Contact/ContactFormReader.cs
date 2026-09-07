using System.Text;
using Microsoft.AspNetCore.Http;

namespace Kestridge.Api.Contact;

public static class ContactFormReader
{
    public const int NameMax = 200;
    public const int EmailMax = 254;
    public const int CompanyMax = 200;
    public const int PhoneMax = 64;

    // The client always transmits all seven parts, blank optional fields as
    // empty-string parts. A missing key is treated the same as an empty value,
    // and any extra part is ignored rather than rejected.
    //
    // Nothing is truncated here. The client sets no maxLength attribute and the
    // form is noValidate, so a truncated value would be a corrupted lead; the
    // validator answers 400 instead. FormOptions.ValueLengthLimit bounds the
    // memory before this runs.
    public static ContactInput Read(IFormCollection form) => new(
        SingleLine(form, "name"),
        SingleLine(form, "email"),
        SingleLine(form, "company"),
        SingleLine(form, "phone"),
        SingleLine(form, "service"),
        Multiline(form, "message"),
        SingleLine(form, "_gotcha"));

    // Collapsed to one line. These values go into mail headers and into one-line
    // rows of the notification body, where a surviving newline renders the text
    // after it as its own line that reads exactly like a header the sender
    // chose. Line breaks become a single space rather than vanishing: dropping
    // them welds "Bob" and "Bcc: attacker@example.com" into one token, and welds
    // a pasted two-line company name into nonsense.
    private static string SingleLine(IFormCollection form, string key)
        => Clean(Raw(form, key), keepNewlines: false).Trim();

    // The message keeps its shape: newlines and tabs survive, CRLF collapses to
    // one LF, everything else in C0 goes.
    private static string Multiline(IFormCollection form, string key)
        => Clean(Raw(form, key), keepNewlines: true).Trim();

    private static string Raw(IFormCollection form, string key)
        => form.TryGetValue(key, out var value) ? value.ToString() : string.Empty;

    private static string Clean(string value, bool keepNewlines)
    {
        if (value.Length == 0)
        {
            return value;
        }

        var sb = new StringBuilder(value.Length);
        var lastWasSpace = false;

        for (var i = 0; i < value.Length; i++)
        {
            var c = value[i];

            if (keepNewlines)
            {
                if (c == '\r')
                {
                    // CRLF is one line break, not two.
                    if (i + 1 < value.Length && value[i + 1] == '\n')
                    {
                        i++;
                    }

                    sb.Append('\n');
                    continue;
                }

                if (c == '\n' || c == '\t')
                {
                    sb.Append(c);
                    continue;
                }
            }
            else if (char.IsWhiteSpace(c))
            {
                // One space for any run of whitespace, whatever it was made of.
                if (!lastWasSpace && sb.Length > 0)
                {
                    lastWasSpace = true;
                    sb.Append(' ');
                }

                continue;
            }

            if (char.IsControl(c))
            {
                continue;
            }

            lastWasSpace = false;
            sb.Append(c);
        }

        return sb.ToString();
    }
}
