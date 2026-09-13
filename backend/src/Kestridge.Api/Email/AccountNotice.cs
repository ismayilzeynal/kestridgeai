using System.Globalization;
using System.Text;
using Kestridge.Api.Options;
using MimeKit;

namespace Kestridge.Api.Email;

// Tells the team mailbox that someone was given a way into the panel. Whoever
// clicked, with whatever session, the rest of the team hears about it, so an
// account nobody expected is noticed by someone who can delete or disable it.
//
// Pure function, no I/O, like NotificationMessage. It takes no password, secret,
// token or code, so none can ever end up in a mailbox.
public static class AccountNotice
{
    public static MimeMessage UserCreated(
        string username, string displayName, string createdBy, DateTime at, DateTime completesBy, ContactOptions opts)
        => Build(
            "Kestridge admin: account created for " + username,
            "An admin panel account was created. It cannot be used until its owner signs in, chooses their own "
            + "password and sets up an authenticator.",
            username,
            displayName,
            "Created by:",
            createdBy,
            at,
            completesBy,

            // Not "disable": there is no account to disable yet, only an
            // invitation, and deleting it is what stops the first sign-in.
            "If this was not expected, delete the pending user in the admin panel (Users, Waiting for first "
            + "sign-in) and follow RUNBOOK.md.",
            opts);

    public static MimeMessage AuthenticatorReset(
        string username, string displayName, string resetBy, DateTime at, DateTime completesBy, ContactOptions opts)
        => Build(
            "Kestridge admin: authenticator reset for " + username,
            "The authenticator of an admin panel account was reset. Its sessions were ended, and it cannot sign in "
            + "until its owner signs in with their password and sets up a new authenticator.",
            username,
            displayName,
            "Reset by:",
            resetBy,
            at,
            completesBy,
            "If this was not expected, disable the account in the admin panel and follow RUNBOOK.md.",
            opts);

    private static MimeMessage Build(
        string subject,
        string lead,
        string username,
        string displayName,
        string actorLabel,
        string actor,
        DateTime at,
        DateTime completesBy,
        string ifUnexpected,
        ContactOptions opts)
    {
        var message = new MimeMessage();

        // Exactly as NotificationMessage: the configured sending-domain address
        // to the configured team address, so SPF and DKIM pass.
        message.From.Add(new MailboxAddress(HeaderSafe.Clean(opts.FromDisplayName, 80), opts.FromAddress));
        message.To.Add(MailboxAddress.Parse(opts.ToAddress));
        message.Subject = HeaderSafe.Clean(subject, 180);

        var body = new StringBuilder();
        body.Append(lead).Append('\n');
        body.Append('\n');
        body.Append("Username:      ").Append(username).Append('\n');
        body.Append("Display name:  ").Append(displayName).Append('\n');
        body.Append(actorLabel.PadRight(15)).Append(actor).Append('\n');
        body.Append("When:          ").Append(Utc(at)).Append('\n');
        body.Append("Completes by:  ").Append(Utc(completesBy)).Append('\n');
        body.Append('\n');
        body.Append(ifUnexpected).Append('\n');

        // Plain text only, for the same reason as the contact notification.
        message.Body = new TextPart("plain") { Text = body.ToString() };
        return message;
    }

    private static string Utc(DateTime value)
        => value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + " UTC";
}
