using System.Buffers.Binary;
using System.Security.Cryptography;

namespace Kestridge.Api.Admin;

// RFC 6238, SHA-1, 6 digits, 30 second step: what every authenticator app
// defaults to. No package. The BCL has HMACSHA1 and that is the whole algorithm.
//
// SHA-1 here is not a weakness. TOTP uses HMAC-SHA1, whose security rests on
// the MAC construction rather than on collision resistance, and RFC 6238 still
// specifies it. Choosing SHA-256 would only break compatibility with the apps
// the operators actually have.
public static class Totp
{
    public const int StepSeconds = 30;
    private const int Digits = 6;

    public static long StepFor(DateTime utcNow)
        => new DateTimeOffset(utcNow, TimeSpan.Zero).ToUnixTimeSeconds() / StepSeconds;

    // Returns the step the code was accepted at, or null. The caller must reject
    // a step that is not greater than the account's totp_last_step, or a code
    // seen over someone's shoulder stays usable for the rest of its window.
    public static long? Verify(string base32Secret, string code, DateTime utcNow, int skewSteps)
    {
        if (code.Length != Digits)
        {
            return null;
        }

        foreach (var c in code)
        {
            if (c is < '0' or > '9')
            {
                return null;
            }
        }

        byte[] key;
        try
        {
            key = FromBase32(base32Secret);
        }
        catch (FormatException)
        {
            return null;
        }

        if (key.Length == 0)
        {
            return null;
        }

        var current = StepFor(utcNow);

        for (var offset = -skewSteps; offset <= skewSteps; offset++)
        {
            var step = current + offset;
            if (step < 0)
            {
                continue;
            }

            // Fixed time compare so a near miss cannot be walked digit by digit
            // through response timing.
            if (CryptographicOperations.FixedTimeEquals(
                    System.Text.Encoding.ASCII.GetBytes(Compute(key, step)),
                    System.Text.Encoding.ASCII.GetBytes(code)))
            {
                return step;
            }
        }

        return null;
    }

    public static string Compute(byte[] key, long step)
    {
        Span<byte> counter = stackalloc byte[8];
        BinaryPrimitives.WriteInt64BigEndian(counter, step);

        Span<byte> mac = stackalloc byte[20];
        HMACSHA1.HashData(key, counter, mac);

        // Dynamic truncation, RFC 4226 section 5.3.
        var offset = mac[^1] & 0x0F;
        var binary = ((mac[offset] & 0x7F) << 24)
                     | (mac[offset + 1] << 16)
                     | (mac[offset + 2] << 8)
                     | mac[offset + 3];

        return (binary % 1_000_000).ToString("D6", System.Globalization.CultureInfo.InvariantCulture);
    }

    // 160 bits, the RFC 4226 recommendation, as 32 Base32 characters.
    public static string NewSecret()
        => ToBase32(RandomNumberGenerator.GetBytes(20));

    private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

    public static string ToBase32(byte[] data)
    {
        var output = new System.Text.StringBuilder((data.Length * 8 + 4) / 5);
        int buffer = 0, bits = 0;

        foreach (var b in data)
        {
            buffer = (buffer << 8) | b;
            bits += 8;

            while (bits >= 5)
            {
                output.Append(Alphabet[(buffer >> (bits - 5)) & 31]);
                bits -= 5;
            }
        }

        if (bits > 0)
        {
            output.Append(Alphabet[(buffer << (5 - bits)) & 31]);
        }

        // No padding. Authenticator apps accept unpadded secrets and the
        // padding characters only invite transcription errors.
        return output.ToString();
    }

    public static byte[] FromBase32(string value)
    {
        var bytes = new List<byte>(value.Length * 5 / 8);
        int buffer = 0, bits = 0;

        foreach (var raw in value)
        {
            if (raw is ' ' or '-' or '=')
            {
                continue;
            }

            var index = Alphabet.IndexOf(char.ToUpperInvariant(raw));
            if (index < 0)
            {
                throw new FormatException("Not Base32.");
            }

            buffer = (buffer << 5) | index;
            bits += 5;

            if (bits >= 8)
            {
                bytes.Add((byte)((buffer >> (bits - 8)) & 0xFF));
                bits -= 8;
            }
        }

        return [.. bytes];
    }
}
