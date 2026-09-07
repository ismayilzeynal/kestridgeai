using MimeKit;

namespace Kestridge.Api.Email;

public interface IEmailSender
{
    Task SendAsync(MimeMessage message, CancellationToken ct);
}
