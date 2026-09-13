using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Kestridge.Api.Tests;

// Puts a step of a test at an exact point inside a request: just before the
// application sends a command, inside a transaction, whose SQL contains the
// given text. The request waits for the step to finish, so an interleaving a
// real race hits once in a thousand runs happens on every run.
//
// Does nothing until armed, and fires for as many commands as it was armed
// for, so the requests a test makes to set the scene pass straight through. A
// step that makes requests of its own is not intercepted again unless it was
// armed for more than one.
public sealed class CommandHook(string sqlFragment) : DbCommandInterceptor
{
    private readonly Lock _gate = new();
    private Func<DbCommand, Task>? _step;
    private int _remaining;

    public int Fired { get; private set; }

    public void Arm(Func<DbCommand, Task> step, int times = 1)
    {
        lock (_gate)
        {
            _step = step;
            _remaining = times;
        }
    }

    public override async ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        await RunAsync(command);
        return result;
    }

    public override async ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        await RunAsync(command);
        return result;
    }

    private async Task RunAsync(DbCommand command)
    {
        if (command.Transaction is null || !command.CommandText.Contains(sqlFragment, StringComparison.Ordinal))
        {
            return;
        }

        Func<DbCommand, Task>? step = null;

        lock (_gate)
        {
            if (_remaining > 0)
            {
                _remaining--;
                Fired++;
                step = _step;
            }
        }

        if (step is not null)
        {
            await step(command);
        }
    }
}
