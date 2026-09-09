using System.Security.Cryptography;
using System.Text;

namespace Kestridge.Api.Admin;

// Must match ops/dsr-log.sql byte for byte. That file computes
//   SHA2(CONCAT(@pepper, @subject), 256)
// over the lowercased address, into an ascii_bin char(64) column.
//
// UTF-8 because that is what CONCAT over utf8mb4 produces, and lowercase hex
// because SHA2() returns lowercase into a column where case is significant.
// Get the concatenation order or the lowercasing wrong and the two paths write
// hashes that never match, silently, forever, and "has this person written in
// before" answers no for the rest of the company's life.
public static class DsrHash
{
    public static string For(string pepper, string email)
        => Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(pepper + email.ToLowerInvariant())));
}
