using System.Security.Cryptography;
using System.Text;
using Kestridge.Api.Data;
using Kestridge.Api.Options;
using Microsoft.AspNetCore.WebUtilities;

namespace Kestridge.Api.Admin;

public static class AdminSessions
{
    public const int TokenChars = 43;

    // 256 bits of uniform randomness, base64url, 43 characters, no padding.
    public static string NewToken()
        => WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));

    // Plain SHA-256 is correct here and a slow KDF would be wrong: the input is
    // 256 bits of uniform randomness, so there is nothing to brute force. What
    // hashing buys is that a database dump, a backup, or a support SELECT
    // yields no live credential.
    public static string Hash(string token)
        => Convert.ToHexStringLower(SHA256.HashData(Encoding.ASCII.GetBytes(token)));

    public static AdminSession Issue(long accountId, string tokenHash, DateTime now, AdminOptions options) => new()
    {
        TokenHash = tokenHash,
        AccountId = accountId,
        CreatedAt = now,
        LastSeenAt = now,
        IdleExpiresAt = now.AddMinutes(options.IdleMinutes),

        // Never extended anywhere in this codebase. A stolen token is dead by
        // the next morning whatever the thief does with it.
        AbsoluteExpiresAt = now.AddHours(options.SessionHours),
    };

    // Only after SlideAfterSeconds, so ordinary browsing does not produce an
    // UPDATE per request.
    public static bool ShouldSlide(AdminSession session, DateTime now, AdminOptions options)
        => (now - session.LastSeenAt).TotalSeconds >= options.SlideAfterSeconds;

    public static void Slide(AdminSession session, DateTime now, AdminOptions options)
    {
        session.LastSeenAt = now;
        session.IdleExpiresAt = now.AddMinutes(options.IdleMinutes);
    }

    public static bool IsLive(AdminSession session, DateTime now)
        => session.AbsoluteExpiresAt > now && session.IdleExpiresAt > now;

    // Only accepts "Bearer " plus exactly the token shape this app issues, so a
    // malformed header never reaches a database lookup.
    public static string? TokenFrom(string? authorizationHeader)
    {
        if (authorizationHeader is null || !authorizationHeader.StartsWith("Bearer ", StringComparison.Ordinal))
        {
            return null;
        }

        var token = authorizationHeader["Bearer ".Length..];
        if (token.Length != TokenChars)
        {
            return null;
        }

        foreach (var c in token)
        {
            var ok = c is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or >= '0' and <= '9' or '-' or '_';
            if (!ok)
            {
                return null;
            }
        }

        return token;
    }
}
