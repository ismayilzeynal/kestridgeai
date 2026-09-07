using Kestridge.Api.Data;

namespace Kestridge.Api.Tests;

[Collection(DatabaseCollection.Name)]
public abstract class DatabaseTestBase(MySqlFixture fixture) : IAsyncLifetime
{
    protected MySqlFixture Fixture { get; } = fixture;

    // Skips rather than fails when MySQL is not provisioned on this machine.
    // MySqlFixture rethrows under CI, so a green CI build cannot be vacuous.
    protected void RequireDatabase() => Skip.IfNot(Fixture.Available, Fixture.SkipReason);

    protected ApiFactory CreateFactory(Dictionary<string, string?>? settings = null) =>
        new() { ConnectionString = Fixture.ConnectionString ?? string.Empty, Settings = settings ?? [] };

    protected KestridgeDbContext Db() => Fixture.Create();

    public async Task InitializeAsync()
    {
        if (Fixture.Available)
        {
            await Fixture.ResetAsync();
        }
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
