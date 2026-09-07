using System.Globalization;
using System.Text;
using Kestridge.Api.Data;
using Kestridge.Api.Options;
using MimeKit;

namespace Kestridge.Api.Email;

public static class NotificationMessage
{
    // Pure function, no I/O. Every header-injection assertion is made against
    // the serialized output of this.
    public static MimeMessage Build(ContactSubmission row, ContactOptions opts)
    {
        var message = new MimeMessage();

        // Always the configured sending-domain address, never the visitor's:
        // otherwise SPF and DKIM fail and the notification is spam-foldered.
        message.From.Add(new MailboxAddress(HeaderSafe.Clean(opts.FromDisplayName, 80), opts.FromAddress));
        message.To.Add(MailboxAddress.Parse(opts.ToAddress));

        // TryParse, not the MailboxAddress constructor: the constructor stores
        // whatever it is given, so a malformed address would reach the header.
        // Omitting Reply-To is fine, the address is printed in the body anyway.
        if (row.Email.Contains('@', StringComparison.Ordinal)
            && MailboxAddress.TryParse(ParserOptions.Default, row.Email, out var visitor)
            && string.Equals(visitor.Address, row.Email, StringComparison.Ordinal))
        {
            message.ReplyTo.Add(new MailboxAddress(HeaderSafe.Clean(row.Name, 120), visitor.Address));
        }

        // Id 0 means the row never committed and this is the inline fallback
        // from a failed database write. That copy exists only in this mailbox:
        // it will not appear in a DSR export or a backup, so the message has to
        // say so rather than quietly claim to be submission zero.
        var stored = row.Id != 0;
        var reference = stored ? "#" + row.Id.ToString(CultureInfo.InvariantCulture) : "(not stored)";

        message.Subject = HeaderSafe.Clean($"Kestridge contact {reference} - {row.Service} - {row.Name}", 180);

        var body = new StringBuilder();
        body.Append("Submission ").Append(reference).Append('\n');

        if (!stored)
        {
            body.Append("WARNING: the database write failed, so this email is the only copy.\n");
        }

        body.Append("Received: ").Append(row.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)).Append(" UTC\n");
        body.Append("Service:  ").Append(row.Service).Append('\n');
        body.Append("Name:     ").Append(row.Name).Append('\n');
        body.Append("Email:    ").Append(row.Email).Append('\n');
        body.Append("Company:  ").Append(Or(row.Company)).Append('\n');
        body.Append("Phone:    ").Append(Or(row.Phone)).Append('\n');
        body.Append('\n');
        body.Append("------------------------------------------------------------\n");
        body.Append(row.Message).Append('\n');
        body.Append("------------------------------------------------------------\n");

        // Plain text only. HTML mail built from untrusted input is a rendering
        // surface with no benefit to four people reading it in a mail client.
        message.Body = new TextPart("plain") { Text = body.ToString() };
        return message;
    }

    private static string Or(string value) => value.Length == 0 ? "(not provided)" : value;
}
