using Kestridge.Api.Contact;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Kestridge.Api.Tests;

public class ContactFormReaderTests
{
    private const string CrLf = "\r\n";

    private static ContactInput Read(params (string Key, string Value)[] fields)
    {
        var values = new Dictionary<string, StringValues>(StringComparer.Ordinal);
        foreach (var (key, value) in fields)
        {
            values[key] = value;
        }

        return ContactFormReader.Read(new FormCollection(values));
    }

    [Fact]
    public void ReadsAllSevenParts()
    {
        var input = Read(
            ("name", "Jane Doe"),
            ("email", "jane@company.com"),
            ("company", "Acme Ltd"),
            ("phone", "(555) 000-0000"),
            ("service", "analytics"),
            ("message", "We need weekly reporting."),
            ("_gotcha", ""));

        Assert.Equal("Jane Doe", input.Name);
        Assert.Equal("jane@company.com", input.Email);
        Assert.Equal("Acme Ltd", input.Company);
        Assert.Equal("(555) 000-0000", input.Phone);
        Assert.Equal("analytics", input.Service);
        Assert.Equal("We need weekly reporting.", input.Message);
        Assert.Equal("", input.Gotcha);
    }

    [Fact]
    public void MissingKeysBecomeEmptyStrings()
    {
        var input = Read(("name", "Jane Doe"));

        Assert.Equal("", input.Company);
        Assert.Equal("", input.Phone);
        Assert.Equal("", input.Gotcha);
    }

    [Fact]
    public void TrimsEveryField()
    {
        var input = Read(
            ("name", "  Jane Doe  "),
            ("email", "  jane@company.com  "),
            ("company", "  Acme  "),
            ("phone", "  555  "),
            ("service", "  ai  "),
            ("message", "  Hello.  "));

        Assert.Equal("Jane Doe", input.Name);
        Assert.Equal("jane@company.com", input.Email);
        Assert.Equal("Acme", input.Company);
        Assert.Equal("555", input.Phone);
        Assert.Equal("ai", input.Service);
        Assert.Equal("Hello.", input.Message);
    }

    // The single-line fields end up in mail headers and as one-line body rows.
    // A surviving newline turns "Name: Bob" into two lines, the second of which
    // reads exactly like a header the sender chose.
    [Theory]
    [InlineData("name")]
    [InlineData("email")]
    [InlineData("company")]
    [InlineData("phone")]
    [InlineData("service")]
    public void StripsNewlinesFromSingleLineFields(string field)
    {
        var input = Read((field, "Bob" + CrLf + "Bcc: attacker@example.com"));

        var value = field switch
        {
            "name" => input.Name,
            "email" => input.Email,
            "company" => input.Company,
            "phone" => input.Phone,
            _ => input.Service,
        };

        Assert.DoesNotContain('\r', value);
        Assert.DoesNotContain('\n', value);

        // A space, not nothing: welding the two halves into one token makes a
        // pasted two-line company name unreadable and hides the seam.
        Assert.Equal("Bob Bcc: attacker@example.com", value);
    }

    [Fact]
    public void CollapsesWhitespaceRunsInSingleLineFields()
    {
        var input = Read(("company", "Acme" + CrLf + CrLf + "   Ltd"));

        Assert.Equal("Acme Ltd", input.Company);
    }

    [Fact]
    public void StripsOtherControlCharactersFromSingleLineFields()
    {
        var input = Read(("name", "Ja\u0000ne\u0007D\toe"));

        // NUL and BEL vanish. The tab is whitespace, so it collapses to a space.
        Assert.Equal("JaneD oe", input.Name);
    }

    [Fact]
    public void MessagePreservesNewlinesAndTabs()
    {
        var input = Read(("message", "First line." + CrLf + "\tSecond line."));

        Assert.Equal("First line.\n\tSecond line.", input.Message);
    }

    [Fact]
    public void MessageDropsOtherControlCharacters()
    {
        var input = Read(("message", "Hel\u0000lo\u000bthere."));

        Assert.Equal("Hellothere.", input.Message);
    }

    [Fact]
    public void NothingIsTruncated()
    {
        var name = new string('a', 500);
        var input = Read(("name", name));

        Assert.Equal(500, input.Name.Length);
    }
}
