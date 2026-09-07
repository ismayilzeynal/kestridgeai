using Kestridge.Api.Data;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace Kestridge.Api.Tests;

// Runs against the real local MySQL. If it is unreachable and CI is unset every
// database test is skipped with a clear message; under CI the fixture throws, so
// a green build cannot be vacuous.
public sealed class MySqlFixture : IAsyncLifetime
{
    public const string DatabaseName = "kestridge_test";

    public string? ConnectionString { get; private set; }
    public string? SkipReason { get; private set; }

    public bool Available => ConnectionString is not null;

    public async Task InitializeAsync()
    {
        var candidate = Environment.GetEnvironmentVariable("KESTRIDGE_TEST_CONNECTION")
                        ?? $"Server=127.0.0.1;Port=3306;Database={DatabaseName};User ID=kestridge_test;Password=kestridge_test;SslMode=Preferred;AllowPublicKeyRetrieval=True;DateTimeKind=Utc;DefaultCommandTimeout=15";

        var builder = new MySqlConnectionStringBuilder(candidate);
        if (!string.Equals(builder.Database, DatabaseName, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Refusing to run tests against database '{builder.Database}'. Expected '{DatabaseName}'.");
        }

        try
        {
            await using var connection = new MySqlConnection(candidate);
            await connection.OpenAsync();

            ConnectionString = candidate;
            await using var db = Create();
            await db.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            ConnectionString = null;
            SkipReason =
                $"MySQL is not reachable for the test credential ({ex.GetType().Name}). "
                + "Run backend/ops/setup-dev-db.ps1, or set KESTRIDGE_TEST_CONNECTION.";

            if (Environment.GetEnvironmentVariable("CI") is { Length: > 0 })
            {
                throw;
            }
        }
    }

    public KestridgeDbContext Create()
    {
        var options = new DbContextOptionsBuilder<KestridgeDbContext>()
            .UseMySql(ConnectionString, new MySqlServerVersion(new Version(8, 0, 43)))
            .AddInterceptors(new UtcTimeZoneInterceptor())
            .Options;

        return new KestridgeDbContext(options);
    }

    // DELETE, not TRUNCATE: TRUNCATE needs the DROP privilege that a
    // least-privileged test credential should not hold.
    public async Task ResetAsync()
    {
        await using var db = Create();
        await db.Database.ExecuteSqlRawAsync("DELETE FROM contact_submissions");
        await db.Database.ExecuteSqlRawAsync("DELETE FROM job_runs");
        await db.Database.ExecuteSqlRawAsync("DELETE FROM dsr_log");
    }

    public Task DisposeAsync() => Task.CompletedTask;
}

[CollectionDefinition(Name)]
public sealed class DatabaseCollection : ICollectionFixture<MySqlFixture>
{
    public const string Name = "mysql";
}
