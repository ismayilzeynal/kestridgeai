using Microsoft.Extensions.Logging;

namespace Kestridge.Api.Tests;

public sealed class CapturingLoggerProvider : ILoggerProvider
{
    private readonly Lock _gate = new();
    private readonly List<string> _lines = [];

    public IReadOnlyList<string> Lines
    {
        get
        {
            lock (_gate)
            {
                return _lines.ToArray();
            }
        }
    }

    public ILogger CreateLogger(string categoryName) => new Capturing(this, categoryName);

    public void Dispose()
    {
    }

    private void Add(string line)
    {
        lock (_gate)
        {
            _lines.Add(line);
        }
    }

    private sealed class Capturing(CapturingLoggerProvider owner, string category) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            owner.Add($"{category} {logLevel} {formatter(state, exception)} {exception}");
        }
    }
}
