using System.Net.Sockets;

using MailKit.Net.Smtp;
using MailKit.Security;

namespace Kestridge.Api.Email;

public enum SmtpOutcome
{
    Sent,
    TransientFailure,
    PermanentFailure,
}

public static class SmtpFailure
{
    // kestridge.com has no MX record today, so a hard bounce on the notification
    // address is the most likely first production failure. Retrying it seven
    // times over 41 hours only gets the sending IP throttled.
    public static SmtpOutcome Classify(Exception ex, int attemptsSoFar)
    {
        switch (ex)
        {
            case SmtpCommandException smtp:
                if (smtp.ErrorCode == SmtpErrorCode.RecipientNotAccepted)
                {
                    return SmtpOutcome.PermanentFailure;
                }

                var code = (int)smtp.StatusCode;
                return code >= 500 && code < 600
                    ? SmtpOutcome.PermanentFailure
                    : SmtpOutcome.TransientFailure;

            case System.Security.Authentication.AuthenticationException:
            case MailKit.Security.AuthenticationException:
                // Almost always a wrong password. Hammering it locks the account.
                return attemptsSoFar >= 3 ? SmtpOutcome.PermanentFailure : SmtpOutcome.TransientFailure;

            case SmtpProtocolException:
            case SslHandshakeException:
            case SocketException:
            case IOException:
            case TimeoutException:
            case OperationCanceledException:
                return SmtpOutcome.TransientFailure;

            default:
                return SmtpOutcome.TransientFailure;
        }
    }

    public static string Describe(Exception ex)
    {
        var status = ex is SmtpCommandException smtp
            ? ((int)smtp.StatusCode).ToString(System.Globalization.CultureInfo.InvariantCulture)
            : string.Empty;

        var reply = ex.Message.Length <= 120 ? ex.Message : ex.Message[..120];
        var text = $"{ex.GetType().Name}: {status}: {reply}";
        return text.Length <= 300 ? text : text[..300];
    }
}
