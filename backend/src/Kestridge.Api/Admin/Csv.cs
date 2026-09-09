using System.Text;

namespace Kestridge.Api.Admin;

// RFC 4180. Every field quoted, quotes doubled, CRLF line endings.
public static class Csv
{
    // Excel on Windows reads a BOM-less UTF-8 file as the system codepage and
    // mangles every non-ASCII name in it.
    public static readonly byte[] Bom = [0xEF, 0xBB, 0xBF];

    public static void AppendRow(StringBuilder sb, params string?[] fields)
    {
        for (var i = 0; i < fields.Length; i++)
        {
            if (i > 0)
            {
                sb.Append(',');
            }

            sb.Append('"').Append(Guard(fields[i] ?? string.Empty).Replace("\"", "\"\"")).Append('"');
        }

        sb.Append("\r\n");
    }

    // A submission is attacker-controlled text and a spreadsheet is a code
    // execution surface: a cell opening with = + - @ or a control character is
    // a formula to Excel, Sheets and LibreOffice. Prefixing an apostrophe makes
    // it text. Quoting alone does not stop this.
    public static string Guard(string value)
    {
        if (value.Length == 0)
        {
            return value;
        }

        var first = value[0];
        return first is '=' or '+' or '-' or '@' or '\t' or '\r' ? "'" + value : value;
    }
}
