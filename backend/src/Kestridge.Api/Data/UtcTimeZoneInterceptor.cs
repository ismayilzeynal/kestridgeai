using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Kestridge.Api.Data;

// Pins every session to UTC so an ad-hoc NOW() in a migration or a manual
// UPDATE cannot write local time. Named zones are not usable here: MySQL on
// Windows ships with the mysql.time_zone tables empty, so only offsets work.
public sealed class UtcTimeZoneInterceptor : DbConnectionInterceptor
{
    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SET time_zone = '+00:00'";
        cmd.ExecuteNonQuery();
    }

    public override async Task ConnectionOpenedAsync(
        DbConnection connection,
        ConnectionEndEventData eventData,
        CancellationToken cancellationToken = default)
    {
        await using var cmd = connection.CreateCommand();
        cmd.CommandText = "SET time_zone = '+00:00'";
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }
}
