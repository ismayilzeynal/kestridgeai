using System.Text;
using Kestridge.Api.Data;
using Kestridge.Api.Email;
using Kestridge.Api.Options;
using MimeKit;

namespace Kestridge.Api.Tests;

public class NotificationMessageTests
{
    private const string CrLf = "\r\n";

    private static readonly ContactOptions Opts = new()
    {
        AllowedServices = ["ai"],
        ToAddress = "info@kestridge.test",
        FromAddress = "no-reply@kestridge.test",
        FromDisplayName = "Kestridge Website",
    };

    private static ContactSubmission Row(string name = "Jane Doe", string email = "jane@company.com") => new()
    {
        Id = 42,
        CreatedAt = new DateTime(2026, 9, 7, 12, 0, 0, DateTimeKind.Utc),
        Name = name,
        Email = email,
        Company = "",
        Phone = "",
        Service = "ai",
        Message = "We need an intake process for incoming orders.",
    };

    // Header injection has to be asserted against the wire bytes, not the DTO.
    // A folded continuation line starts with whitespace, so only lines that do
    // not are real header field starts.
    private static List<string> HeaderLines(ContactSubmission row)
    {
        using var stream = new MemoryStream();
        NotificationMessage.Build(row, Opts).WriteTo(stream);
        var raw = Encoding.UTF8.GetString(stream.ToArray());

        var headerBlock = raw.Split(CrLf + CrLf, 2, StringSplitOptions.None)[0];

        return headerBlock
            .Split(CrLf, StringSplitOptions.None)
            .Where(line => line.Length > 0 && !char.IsWhiteSpace(line[0]))
            .ToList();
    }

    private static int CountHeaders(List<string> lines, string name) =>
        lines.Count(line => line.StartsWith(name + ":", StringComparison.OrdinalIgnoreCase));

    // Never the visitor address: SPF and DKIM would fail and the notification
    // would be spam-foldered exactly when it matters.
    [Fact]
    public void From_IsAlwaysTheConfiguredNoReplyAddress()
    {
        var message = NotificationMessage.Build(Row(), Opts);
        var from = Assert.IsType<MailboxAddress>(Assert.Single(message.From));

        Assert.Equal("no-reply@kestridge.test", from.Address);
    }

    [Fact]
    public void To_IsTheConfiguredTeamAddress()
    {
        var message = NotificationMessage.Build(Row(), Opts);
        var to = Assert.IsType<MailboxAddress>(Assert.Single(message.To));

        Assert.Equal("info@kestridge.test", to.Address);
    }

    [Fact]
    public void ReplyTo_IsTheVisitorAddress_WhenValid()
    {
        var message = NotificationMessage.Build(Row(), Opts);
        var replyTo = Assert.IsType<MailboxAddress>(Assert.Single(message.ReplyTo));

        Assert.Equal("jane@company.com", replyTo.Address);
    }

    [Fact]
    public void NameWithCrLfBcc_ProducesNoBccHeader()
    {
        var lines = HeaderLines(Row(name: "Bob" + CrLf + "Bcc: attacker@example.com"));

        Assert.Equal(0, CountHeaders(lines, "Bcc"));
        Assert.Equal(1, CountHeaders(lines, "To"));
    }

    [Fact]
    public void NameWithCrLfCc_ProducesNoCcHeader()
    {
        var lines = HeaderLines(Row(name: "Bob" + CrLf + "Cc: attacker@example.com"));

        Assert.Equal(0, CountHeaders(lines, "Cc"));
    }

    [Fact]
    public void SubjectWithCrLf_ProducesExactlyOneSubjectHeader()
    {
        var row = Row(name: "Bob" + CrLf + "Subject: injected");
        var message = NotificationMessage.Build(row, Opts);
        var subject = message.Subject ?? string.Empty;

        Assert.Equal(1, CountHeaders(HeaderLines(row), "Subject"));
        Assert.DoesNotContain(CrLf, subject, StringComparison.Ordinal);

        // The injected text survives as data, separated by the space HeaderSafe
        // substitutes. That is the correct outcome: visible and inert.
        Assert.Contains("Bob Subject: injected", subject, StringComparison.Ordinal);
    }

    [Fact]
    public void ExactlyOneRecipient_EvenWithCcInjection()
    {
        var message = NotificationMessage.Build(Row(name: "Bob" + CrLf + "Cc: attacker@example.com"), Opts);

        Assert.Single(message.To);
        Assert.Empty(message.Cc);
        Assert.Empty(message.Bcc);
    }

    // Unreachable through the endpoint, since the validator rejects anything
    // malformed first. This pins the fallback so a future validator change
    // cannot put an unparseable value into a mail header.
    [Theory]
    [InlineData("not-an-address")]
    [InlineData("two words@example.com")]
    [InlineData("@example.com")]
    public void ReplyTo_IsOmitted_WhenAddressIsNotUsable(string email)
    {
        var row = Row(email: email);
        var message = NotificationMessage.Build(row, Opts);
        var body = message.TextBody ?? string.Empty;

        Assert.Empty(message.ReplyTo);
        Assert.Contains("Email:    " + email, body, StringComparison.Ordinal);
    }

    [Fact]
    public void Body_IsPlainTextOnly_WithNoHtmlPart()
    {
        var message = NotificationMessage.Build(Row(), Opts);

        Assert.Null(message.HtmlBody);
        Assert.NotNull(message.TextBody);
    }

    [Fact]
    public void Body_ContainsAllSixFieldsAndTheId()
    {
        var body = NotificationMessage.Build(Row(), Opts).TextBody ?? string.Empty;

        Assert.Contains("Submission #42", body, StringComparison.Ordinal);
        Assert.Contains("2026-09-07 12:00:00 UTC", body, StringComparison.Ordinal);
        Assert.Contains("Service:  ai", body, StringComparison.Ordinal);
        Assert.Contains("Name:     Jane Doe", body, StringComparison.Ordinal);
        Assert.Contains("Email:    jane@company.com", body, StringComparison.Ordinal);
        Assert.Contains("Company:  (not provided)", body, StringComparison.Ordinal);
        Assert.Contains("Phone:    (not provided)", body, StringComparison.Ordinal);
        Assert.Contains("We need an intake process for incoming orders.", body, StringComparison.Ordinal);
    }

    [Fact]
    public void Body_DoesNotContainHoneypotOrIpOrUserAgent()
    {
        var body = NotificationMessage.Build(Row(), Opts).TextBody ?? string.Empty;

        Assert.DoesNotContain("_gotcha", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("User-Agent", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Referer", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Subject_ContainsIdServiceAndName()
    {
        var message = NotificationMessage.Build(Row(), Opts);

        Assert.Equal("Kestridge contact #42 - ai - Jane Doe", message.Subject);
    }
}
