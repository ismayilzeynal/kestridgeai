using System.Security.Cryptography;
using System.Text;

namespace Kestridge.Api.Contact;

public static class DedupeKey
{
    // Bucketed by integer division, so two identical submissions inside the
    // same window collide on a UNIQUE index instead of racing a SELECT.
    public static string Compute(string email, string message, DateTime createdAtUtc, int windowSeconds)
    {
        var bucket = new DateTimeOffset(DateTime.SpecifyKind(createdAtUtc, DateTimeKind.Utc))
            .ToUnixTimeSeconds() / windowSeconds;

        var material = string.Concat(
            email.Trim().ToLowerInvariant(), "\u001f",
            message.Trim(), "\u001f",
            bucket.ToString(System.Globalization.CultureInfo.InvariantCulture));

        return Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(material)));
    }
}
