using System.Reflection;
using System.Text;
using Kestridge.Api.Admin;
using Kestridge.Api.Data;
using Kestridge.Api.Email;
using Kestridge.Api.Options;
using Microsoft.AspNetCore.Identity;
using MimeKit;

namespace Kestridge.Api.Tests;

public class AdminUserRulesTests
{
    [Theory]
    [InlineData("ab", "ab")]
    [InlineData("emil", "emil")]
    [InlineData("  Emil.Huseynov ", "emil.huseynov")]
    [InlineData("a_b-c.d", "a_b-c.d")]
    [InlineData("0day", "0day")]
    public void NewUsername_AcceptsAndNormalises(string value, string expected)
        => Assert.Equal(expected, AdminUserRules.NewUsername(value));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("a")]
    [InlineData(".emil")]
    [InlineData("-emil")]
    [InlineData("_emil")]
    [InlineData("em il")]
    [InlineData("emil@kestridge")]
    [InlineData("emil:admin")]
    [InlineData("\u00e9mil")]

    // Turkish dotted capital I lowercases to i plus a combining dot, which is
    // not ASCII and would not survive the ascii column.
    [InlineData("EM\u0130L")]
    public void NewUsername_RejectsAnythingOutsideTheRule(string? value)
        => Assert.Null(AdminUserRules.NewUsername(value));

    [Fact]
    public void NewUsername_AllowsSixtyFourCharactersAndNotSixtyFive()
    {
        Assert.NotNull(AdminUserRules.NewUsername(new string('a', 64)));
        Assert.Null(AdminUserRules.NewUsername(new string('a', 65)));
    }

    [Theory]
    [InlineData(11, false)]
    [InlineData(12, true)]
    [InlineData(128, true)]
    [InlineData(129, false)]
    public void PasswordLength_IsTwelveToOneHundredTwentyEight(int length, bool ok)
        => Assert.Equal(ok, AdminUserRules.PasswordLengthOk(new string('x', length)));

    [Fact]
    public void PasswordLength_CountsSpacesAndRefusesNull()
    {
        // Never trimmed: twelve spaces are twelve characters the person typed.
        Assert.True(AdminUserRules.PasswordLengthOk(new string(' ', 12)));
        Assert.False(AdminUserRules.PasswordLengthOk(null));
    }
}

public class OtpauthAndQrTests
{
    private const string Secret = "JBSWY3DPEHPK3PXPJBSWY3DPEHPK3PXP";

    [Fact]
    public void OtpauthUri_IsTheFormatTheCliAlwaysPrinted()
        => Assert.Equal(
            "otpauth://totp/Kestridge:emil?secret=" + Secret + "&issuer=Kestridge&algorithm=SHA1&digits=6&period=30",
            Totp.OtpauthUri("emil", Secret));

    // Only an account made by SQL can have such a name, and the label must
    // still parse as one label.
    [Fact]
    public void OtpauthUri_EscapesTheUsername()
        => Assert.StartsWith("otpauth://totp/Kestridge:a%20b%3Ac%3Fd?secret=", Totp.OtpauthUri("a b:c?d", Secret), StringComparison.Ordinal);

    [Fact]
    public void Qr_IsAPngDataUri()
    {
        const string prefix = "data:image/png;base64,";
        var uri = AdminQr.DataUri(Totp.OtpauthUri("emil", Secret));

        Assert.StartsWith(prefix, uri, StringComparison.Ordinal);

        var png = Convert.FromBase64String(uri[prefix.Length..]);
        Assert.Equal(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, png[..8]);
    }

    // No randomness in the encoder: the same URI gives the same image, so the
    // panel can never show a code that differs from the secret beside it.
    [Fact]
    public void Qr_IsStableForTheSameInput()
        => Assert.Equal(AdminQr.DataUri("otpauth://totp/x"), AdminQr.DataUri("otpauth://totp/x"));
}

// The lockout counters themselves are a single SQL UPDATE now, so they are
// tested against MySQL in AdminEnrollTests rather than here.
public class AdminAccessTests
{
    [Theory]
    [InlineData("emil", true)]
    [InlineData("new.user", true)]
    [InlineData("", false)]
    [InlineData("\u0130lkin", false)]
    [InlineData("\u0259li", false)]
    public void CanMatch_RefusesEmptyAndAnythingOutsideAscii(string username, bool expected)
        => Assert.Equal(expected, AdminUserRules.CanMatch(username));

    [Fact]
    public void CanMatch_RefusesMoreThanTheColumnHolds()
    {
        Assert.True(AdminUserRules.CanMatch(new string('a', 64)));
        Assert.False(AdminUserRules.CanMatch(new string('a', 65)));
    }

    // The iteration count sits in the hash, bytes 5 to 8 big-endian after the
    // format marker and the PRF, and verification uses that count. A dummy made
    // at the library default would answer an unknown user faster than a real
    // one at the configured count.
    [Fact]
    public void DummyHash_UsesTheConfiguredIterationCount_AndIsMadeOncePerHasher()
    {
        var hasher = new PasswordHasher<AdminAccount>(Microsoft.Extensions.Options.Options.Create(
            new PasswordHasherOptions
            {
                CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3,
                IterationCount = 12345,
            }));

        var dummy = AdminAccess.DummyHash(hasher);
        var bytes = Convert.FromBase64String(dummy);

        Assert.Equal(0x01, bytes[0]);
        Assert.Equal(12345u, System.Buffers.Binary.BinaryPrimitives.ReadUInt32BigEndian(bytes.AsSpan(5, 4)));
        Assert.Same(dummy, AdminAccess.DummyHash(hasher));
    }

    [Fact]
    public void SetupToken_UsesTheSessionTokenShape()
    {
        Assert.True(AdminSessions.IsWellFormed(AdminSessions.NewToken()));
        Assert.False(AdminSessions.IsWellFormed(null));
        Assert.False(AdminSessions.IsWellFormed("short"));
        Assert.False(AdminSessions.IsWellFormed(new string('!', AdminSessions.TokenChars)));
    }
}

public class AccountNoticeTests
{
    private const string CrLf = "\r\n";

    private static readonly ContactOptions Opts = new()
    {
        AllowedServices = ["ai"],
        ToAddress = "info@kestridge.test",
        FromAddress = "no-reply@kestridge.test",
        FromDisplayName = "Kestridge Website",
    };

    private static readonly DateTime At = new(2026, 9, 7, 12, 0, 0, DateTimeKind.Utc);

    private static MimeMessage Created(string username = "new.user", string displayName = "New Person")
        => AccountNotice.UserCreated(username, displayName, "Faig Garayev", At, At.AddHours(72), Opts);

    private static MimeMessage Reset(string username = "emil", string displayName = "Emil Huseynov")
        => AccountNotice.AuthenticatorReset(username, displayName, "Faig Garayev", At, At.AddHours(72), Opts);

    private static List<string> HeaderLines(MimeMessage message)
    {
        using var stream = new MemoryStream();

        // CRLF pinned for the reason NotificationMessageTests gives.
        var format = FormatOptions.Default.Clone();
        format.NewLineFormat = NewLineFormat.Dos;
        message.WriteTo(format, stream);

        var raw = Encoding.UTF8.GetString(stream.ToArray());

        return raw.Split(CrLf + CrLf, 2, StringSplitOptions.None)[0]
            .Split(CrLf, StringSplitOptions.None)
            .Where(line => line.Length > 0 && !char.IsWhiteSpace(line[0]))
            .ToList();
    }

    [Fact]
    public void FromAndTo_AreTheConfiguredAddresses()
    {
        foreach (var message in new[] { Created(), Reset() })
        {
            Assert.Equal("no-reply@kestridge.test", Assert.IsType<MailboxAddress>(Assert.Single(message.From)).Address);
            Assert.Equal("info@kestridge.test", Assert.IsType<MailboxAddress>(Assert.Single(message.To)).Address);
            Assert.Empty(message.Cc);
            Assert.Empty(message.Bcc);
        }
    }

    [Fact]
    public void Subjects_NameTheKindAndTheUsername()
    {
        Assert.Equal("Kestridge admin: account created for new.user", Created().Subject);
        Assert.Equal("Kestridge admin: authenticator reset for emil", Reset().Subject);
    }

    // The whole body, pinned. A body that is exactly these lines has no room for
    // a password, a secret, a setup token or a code to appear in.
    [Fact]
    public void UserCreated_BodyIsExactlyTheseLines()
    {
        var expected =
            "An admin panel account was created. It cannot be used until its owner signs in, chooses their own "
            + "password and sets up an authenticator.\n"
            + "\n"
            + "Username:      new.user\n"
            + "Display name:  New Person\n"
            + "Created by:    Faig Garayev\n"
            + "When:          2026-09-07 12:00:00 UTC\n"
            + "Completes by:  2026-09-10 12:00:00 UTC\n"
            + "\n"
            + "If this was not expected, delete the pending user in the admin panel (Users, Waiting for first "
            + "sign-in) and follow RUNBOOK.md.\n";

        var message = Created();

        Assert.Null(message.HtmlBody);

        // MimeKit hands the text back with the platform's line endings.
        Assert.Equal(expected, message.TextBody?.ReplaceLineEndings("\n"));
    }

    [Fact]
    public void AuthenticatorReset_BodyIsExactlyTheseLines()
    {
        var expected =
            "The authenticator of an admin panel account was reset. Its sessions were ended, and it cannot sign in "
            + "until its owner signs in with their password and sets up a new authenticator.\n"
            + "\n"
            + "Username:      emil\n"
            + "Display name:  Emil Huseynov\n"
            + "Reset by:      Faig Garayev\n"
            + "When:          2026-09-07 12:00:00 UTC\n"
            + "Completes by:  2026-09-10 12:00:00 UTC\n"
            + "\n"
            + "If this was not expected, disable the account in the admin panel and follow RUNBOOK.md.\n";

        var message = Reset();

        Assert.Null(message.HtmlBody);

        // MimeKit hands the text back with the platform's line endings.
        Assert.Equal(expected, message.TextBody?.ReplaceLineEndings("\n"));
    }

    // The builder cannot leak what it is never given. This keeps it that way:
    // adding a parameter for any of these fails here first.
    [Fact]
    public void Builders_TakeNoPasswordSecretTokenOrCode()
    {
        var names = typeof(AccountNotice)
            .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
            .SelectMany(m => m.GetParameters())
            .Select(p => p.Name ?? string.Empty)
            .ToList();

        Assert.NotEmpty(names);
        foreach (var forbidden in new[] { "password", "secret", "token", "code", "hash" })
        {
            Assert.DoesNotContain(names, n => n.Contains(forbidden, StringComparison.OrdinalIgnoreCase));
        }
    }

    // Unreachable for a panel-created user, whose name passes the username rule.
    // An account made by SQL is held to nothing, so the builder must be safe on
    // its own.
    [Fact]
    public void UsernameWithCrLfBcc_ProducesNoBccHeader()
    {
        var lines = HeaderLines(Created(username: "bob" + CrLf + "Bcc: attacker@example.com"));

        Assert.Equal(0, lines.Count(l => l.StartsWith("Bcc:", StringComparison.OrdinalIgnoreCase)));
        Assert.Equal(1, lines.Count(l => l.StartsWith("Subject:", StringComparison.OrdinalIgnoreCase)));
        Assert.Equal(1, lines.Count(l => l.StartsWith("To:", StringComparison.OrdinalIgnoreCase)));
    }
}
