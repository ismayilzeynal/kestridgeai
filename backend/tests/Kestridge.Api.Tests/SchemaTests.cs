using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace Kestridge.Api.Tests;

// The schema carries compliance guarantees that no C# test can assert. These
// read them back from information_schema on the server the app actually uses.
public class SchemaTests(MySqlFixture fixture) : DatabaseTestBase(fixture)
{
    private async Task<string> ScalarAsync(string sql)
    {
        await using var db = Db();
        await using var connection = new MySqlConnection(db.Database.GetConnectionString());
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        var value = await command.ExecuteScalarAsync();
        return value?.ToString() ?? string.Empty;
    }

    private async Task<List<string>> ListAsync(string sql)
    {
        var rows = new List<string>();

        await using var db = Db();
        await using var connection = new MySqlConnection(db.Database.GetConnectionString());
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            rows.Add(reader.GetValue(0)?.ToString() ?? string.Empty);
        }

        return rows;
    }

    // A schema created by hand on a server with character-set-server=latin1
    // accepts Azerbaijani and emoji only to throw error 1366 or truncate.
    [SkippableFact]
    public async Task Database_DefaultCharsetIsUtf8mb4()
    {
        RequireDatabase();

        var charset = await ScalarAsync(
            "SELECT DEFAULT_CHARACTER_SET_NAME FROM information_schema.SCHEMATA WHERE SCHEMA_NAME = DATABASE()");

        Assert.Equal("utf8mb4", charset);
    }

    // Accent-sensitive on purpose: under the schema default, a DSR delete for
    // jose@x.com would also delete an accented spelling, a different mailbox
    // belonging to a different data subject.
    [SkippableFact]
    public async Task EmailColumn_CollationIsAccentAndCaseSensitive()
    {
        RequireDatabase();

        var collation = await ScalarAsync(
            "SELECT COLLATION_NAME FROM information_schema.COLUMNS "
            + "WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'contact_submissions' AND COLUMN_NAME = 'email'");

        Assert.Equal("utf8mb4_0900_as_cs", collation);
    }

    [SkippableTheory]
    [InlineData("service")]
    [InlineData("dedupe_key")]
    [InlineData("notify_state")]
    public async Task AsciiColumns_HaveAsciiCharsetAndBinCollation(string column)
    {
        RequireDatabase();

        var charset = await ScalarAsync(
            "SELECT CHARACTER_SET_NAME FROM information_schema.COLUMNS "
            + $"WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'contact_submissions' AND COLUMN_NAME = '{column}'");

        var collation = await ScalarAsync(
            "SELECT COLLATION_NAME FROM information_schema.COLUMNS "
            + $"WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'contact_submissions' AND COLUMN_NAME = '{column}'");

        Assert.Equal("ascii", charset);
        Assert.Equal("ascii_bin", collation);
    }

    // Guards the 3072-byte index key limit. Under COMPACT it drops to 767.
    [SkippableFact]
    public async Task AllTables_UseInnoDbAndDynamicRowFormat()
    {
        RequireDatabase();

        var rows = await ListAsync(
            "SELECT CONCAT(TABLE_NAME, ':', ENGINE, ':', ROW_FORMAT) FROM information_schema.TABLES "
            + "WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME IN ('contact_submissions', 'job_runs', 'dsr_log')");

        Assert.Equal(3, rows.Count);
        foreach (var row in rows)
        {
            Assert.Contains(":InnoDB:Dynamic", row, StringComparison.OrdinalIgnoreCase);
        }
    }

    // This is what makes varchar(5000) a real cap rather than a silent truncation.
    [SkippableFact]
    public async Task SqlMode_IncludesStrictTransTables()
    {
        RequireDatabase();

        var mode = await ScalarAsync("SELECT @@SESSION.sql_mode");

        Assert.Contains("STRICT_TRANS_TABLES", mode, StringComparison.Ordinal);
    }

    // The privacy policy discloses IP and user agent only as aggregate visit
    // telemetry. Storing either next to a name and an email would make it
    // identified personal data serving an undisclosed purpose.
    [SkippableTheory]
    [InlineData("ip_address")]
    [InlineData("user_agent")]
    [InlineData("referer")]
    [InlineData("remote_addr")]
    public async Task ContactSubmissions_HasNoNetworkIdentityColumn(string column)
    {
        RequireDatabase();

        var found = await ScalarAsync(
            "SELECT COUNT(*) FROM information_schema.COLUMNS "
            + $"WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'contact_submissions' AND COLUMN_NAME = '{column}'");

        Assert.Equal("0", found);
    }

    [SkippableFact]
    public async Task ContactSubmissions_HasExactlyTheExpectedColumns()
    {
        RequireDatabase();

        var columns = await ListAsync(
            "SELECT COLUMN_NAME FROM information_schema.COLUMNS "
            + "WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'contact_submissions' ORDER BY COLUMN_NAME");

        string[] expected =
        [
            "company", "created_at", "dedupe_key", "email", "id", "legal_hold", "message", "name",
            "notified_at", "notify_attempts", "notify_error", "notify_next_attempt_at", "notify_state",
            "phone", "purge_after", "service",
        ];

        Assert.Equal(expected, columns);
    }

    [SkippableFact]
    public async Task AllIndexKeyLengths_AreUnder3072Bytes()
    {
        RequireDatabase();

        var rows = await ListAsync(
            "SELECT CONCAT(s.INDEX_NAME, ':', SUM(c.CHARACTER_OCTET_LENGTH + 8)) "
            + "FROM information_schema.STATISTICS s "
            + "JOIN information_schema.COLUMNS c "
            + "  ON c.TABLE_SCHEMA = s.TABLE_SCHEMA AND c.TABLE_NAME = s.TABLE_NAME AND c.COLUMN_NAME = s.COLUMN_NAME "
            + "WHERE s.TABLE_SCHEMA = DATABASE() GROUP BY s.TABLE_NAME, s.INDEX_NAME");

        Assert.NotEmpty(rows);
        foreach (var row in rows)
        {
            var bytes = int.Parse(row.Split(':')[1], System.Globalization.CultureInfo.InvariantCulture);
            Assert.True(bytes < 3072, row);
        }
    }

    [SkippableFact]
    public async Task NoPendingMigrations()
    {
        RequireDatabase();

        await using var db = Db();
        Assert.Empty(await db.Database.GetPendingMigrationsAsync());
    }
}
