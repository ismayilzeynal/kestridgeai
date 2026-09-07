using Kestridge.Api.Email;
using MimeKit;

namespace Kestridge.Api.Tests;

public sealed class FakeEmailSender : IEmailSender
{
    private readonly Lock _gate = new();
    private readonly List<MimeMessage> _sent = [];

    public Func<MimeMessage, Exception?>? FailWith { get; set; }

    public IReadOnlyList<MimeMessage> Sent
    {
        get
        {
            lock (_gate)
            {
                return _sent.ToArray();
            }
        }
    }

    public Task SendAsync(MimeMessage message, CancellationToken ct)
    {
        var failure = FailWith?.Invoke(message);
        if (failure is not null)
        {
            return Task.FromException(failure);
        }

        lock (_gate)
        {
            _sent.Add(message);
        }

        return Task.CompletedTask;
    }
}
