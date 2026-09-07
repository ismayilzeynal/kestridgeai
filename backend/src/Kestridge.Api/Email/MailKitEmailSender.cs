using Kestridge.Api.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Kestridge.Api.Email;

// A new client per call. MailKit's SmtpClient is not thread-safe and providers
// drop idle sessions, so a long-lived client fails on the message that matters.
public sealed class MailKitEmailSender(IOptions<SmtpOptions> options) : IEmailSender
{
    public async Task SendAsync(MimeMessage message, CancellationToken ct)
    {
        var o = options.Value;

        using var client = new SmtpClient { Timeout = o.TimeoutSeconds * 1000 };

        var security = o.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.None;
        await client.ConnectAsync(o.Host, o.Port, security, ct);

        if (o.User.Length > 0)
        {
            await client.AuthenticateAsync(o.User, o.Password, ct);
        }

        try
        {
            await client.SendAsync(message, ct);
        }
        finally
        {
            await client.DisconnectAsync(true, CancellationToken.None);
        }
    }
}
