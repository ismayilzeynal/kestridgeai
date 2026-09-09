using System.Text;
using Kestridge.Api.Admin;

namespace Kestridge.Api.Tests;

public class TotpTests
{
    // RFC 6238 appendix B, the SHA-1 vector. "12345678901234567890" in Base32.
    private const string RfcSecret = "GEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQ";

    [Theory]
    [InlineData(59L, "287082")]
    [InlineData(1111111109L, "081804")]
    [InlineData(1111111111L, "050471")]
    [InlineData(1234567890L, "005924")]
    [InlineData(2000000000L, "279037")]
    public void Compute_MatchesRfc6238Vectors(long unixSeconds, string expected)
    {
        var key = Totp.FromBase32(RfcSecret);
        Assert.Equal(expected, Totp.Compute(key, unixSeconds / Totp.StepSeconds));
    }

    [Fact]
    public void Verify_AcceptsTheCurrentCodeAndReturnsItsStep()
    {
        var now = new DateTime(2026, 9, 9, 12, 0, 0, DateTimeKind.Utc);
        var step = Totp.StepFor(now);
        var code = Totp.Compute(Totp.FromBase32(RfcSecret), step);

        Assert.Equal(step, Totp.Verify(RfcSecret, code, now, 1));
    }

    [Fact]
    public void Verify_AcceptsOneStepOfSkewOnEitherSide()
    {
        var now = new DateTime(2026, 9, 9, 12, 0, 0, DateTimeKind.Utc);
        var key = Totp.FromBase32(RfcSecret);

        Assert.NotNull(Totp.Verify(RfcSecret, Totp.Compute(key, Totp.StepFor(now) - 1), now, 1));
        Assert.NotNull(Totp.Verify(RfcSecret, Totp.Compute(key, Totp.StepFor(now) + 1), now, 1));
    }

    [Fact]
    public void Verify_RejectsTwoStepsAwayWithSkewOfOne()
    {
        var now = new DateTime(2026, 9, 9, 12, 0, 0, DateTimeKind.Utc);
        var far = Totp.Compute(Totp.FromBase32(RfcSecret), Totp.StepFor(now) + 2);

        Assert.Null(Totp.Verify(RfcSecret, far, now, 1));
    }

    [Theory]
    [InlineData("")]
    [InlineData("12345")]
    [InlineData("1234567")]
    [InlineData("abcdef")]
    public void Verify_RejectsAnythingThatIsNotSixDigits(string code)
        => Assert.Null(Totp.Verify(RfcSecret, code, DateTime.UtcNow, 1));

    [Fact]
    public void Base32_RoundTrips()
    {
        var bytes = new byte[] { 1, 2, 3, 250, 255, 0, 128, 64 };
        Assert.Equal(bytes, Totp.FromBase32(Totp.ToBase32(bytes)));
    }

    [Fact]
    public void NewSecret_Is160BitsAndDecodable()
        => Assert.Equal(20, Totp.FromBase32(Totp.NewSecret()).Length);
}

public class DsrHashTests
{
    // The manual path computes SHA2(CONCAT(@pepper, @subject), 256) over the
    // lowercased address. This asserts the exact bytes, so a change to the
    // concatenation order or the casing cannot pass silently and leave the two
    // paths writing hashes that never match.
    [Fact]
    public void For_IsSha256OfPepperThenLowercasedEmail()
    {
        var expected = Convert.ToHexStringLower(
            System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes("pepper" + "jane@company.com")));

        Assert.Equal(expected, DsrHash.For("pepper", "JANE@Company.com"));
    }

    [Fact]
    public void For_IsLowercaseHex64()
    {
        var hash = DsrHash.For("pepper", "jane@company.com");

        Assert.Equal(64, hash.Length);
        Assert.Equal(hash.ToLowerInvariant(), hash);
    }

    [Fact]
    public void For_DiffersByPepper()
        => Assert.NotEqual(DsrHash.For("a", "jane@x.com"), DsrHash.For("b", "jane@x.com"));
}

public class CsvTests
{
    [Theory]
    [InlineData("=1+1", "'=1+1")]
    [InlineData("+x", "'+x")]
    [InlineData("-x", "'-x")]
    [InlineData("@SUM(A1)", "'@SUM(A1)")]
    public void Guard_NeutralisesFormulaLeaders(string input, string expected)
        => Assert.Equal(expected, Csv.Guard(input));

    [Fact]
    public void Guard_LeavesOrdinaryTextAlone()
        => Assert.Equal("Jane Doe", Csv.Guard("Jane Doe"));

    [Fact]
    public void AppendRow_QuotesEveryFieldAndDoublesQuotes()
    {
        var sb = new StringBuilder();
        Csv.AppendRow(sb, "a", "say \"hi\"", "c,d");

        Assert.Equal("\"a\",\"say \"\"hi\"\"\",\"c,d\"\r\n", sb.ToString());
    }

    [Fact]
    public void Bom_IsTheUtf8Bom()
        => Assert.Equal(new byte[] { 0xEF, 0xBB, 0xBF }, Csv.Bom);
}

public class AdminSessionTokenTests
{
    [Fact]
    public void NewToken_Is43Base64UrlChars()
    {
        var token = AdminSessions.NewToken();

        Assert.Equal(AdminSessions.TokenChars, token.Length);
        Assert.DoesNotContain('=', token);
        Assert.DoesNotContain('+', token);
        Assert.DoesNotContain('/', token);
    }

    [Fact]
    public void Hash_IsLowercaseHex64AndStable()
    {
        var hash = AdminSessions.Hash("token");

        Assert.Equal(64, hash.Length);
        Assert.Equal(hash, AdminSessions.Hash("token"));
        Assert.NotEqual(hash, AdminSessions.Hash("token2"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Basic abc")]
    [InlineData("Bearer short")]
    [InlineData("Bearer ")]
    public void TokenFrom_RejectsAnythingButTheIssuedShape(string? header)
        => Assert.Null(AdminSessions.TokenFrom(header));

    [Fact]
    public void TokenFrom_RejectsRightLengthWrongAlphabet()
        => Assert.Null(AdminSessions.TokenFrom("Bearer " + new string('!', AdminSessions.TokenChars)));

    [Fact]
    public void TokenFrom_AcceptsAnIssuedToken()
    {
        var token = AdminSessions.NewToken();
        Assert.Equal(token, AdminSessions.TokenFrom("Bearer " + token));
    }
}
