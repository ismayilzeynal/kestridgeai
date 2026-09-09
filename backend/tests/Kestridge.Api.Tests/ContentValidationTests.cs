using Kestridge.Api.Admin;

namespace Kestridge.Api.Tests;

// Pure unit tests, no database. This is the rule set that stands between an
// operator's keyboard and the marketing page, and it is the one part of the
// content feature that can be proven on any machine.
public class ContentValidationTests
{
    public static TheoryData<string> Fields => new() { "question", "answer", "name", "tagline" };

    [Theory]
    [InlineData('\u2012')]
    [InlineData('\u2013')]
    [InlineData('\u2014')]
    [InlineData('\u2015')]
    public void EachDashCodepoint_IsRejected(char dash)
    {
        var error = ContentValidation.Text("answer", "before" + dash + "after", 600, out _);

        Assert.Equal(new ContentError("answer", "dash"), error);
    }

    [Theory]
    [MemberData(nameof(Fields))]
    public void TheFieldName_IsCarriedBackUnchanged(string field)
    {
        var error = ContentValidation.Text(field, "an em dash\u2014here", 600, out _);

        Assert.Equal(field, error?.Field);
    }

    [Theory]
    [InlineData("\U0001F600")]
    [InlineData("\u2705")]
    [InlineData("text\uFE0F")]
    public void Emoji_IsRejected(string value)
        => Assert.Equal("emoji", ContentValidation.Text("answer", value, 600, out _)?.Reason);

    [Theory]
    [InlineData("two\nlines")]
    [InlineData("two\r\nlines")]
    [InlineData("carriage\rreturn")]
    public void NewlineAndCarriageReturn_AreRejected(string value)
        => Assert.Equal("newline", ContentValidation.Text("answer", value, 600, out _)?.Reason);


    // The boundary, and it is deliberate: a break in the middle changes the
    // rendered text and is refused, while one hanging off the end is just
    // whitespace and is trimmed like any other. Rejecting a stray trailing
    // newline would make pasting out of a text editor fail for no visible reason.
    [Fact]
    public void ATrailingNewline_IsTrimmedRatherThanRejected()
    {
        Assert.Null(ContentValidation.Text("answer", "trailing\r\n", 600, out var clean));
        Assert.Equal("trailing", clean);
    }

    // The reason is newline rather than control, because that is the one an
    // operator can act on: the other says nothing about what to change.
    [Fact]
    public void ANewline_IsReportedAsNewlineNotAsControl()
        => Assert.Equal("newline", ContentValidation.Text("answer", "a\nb", 600, out _)?.Reason);

    [Theory]
    [InlineData("a < b")]
    [InlineData("a > b")]
    [InlineData("</script><script>alert(1)</script>")]
    public void AngleBrackets_AreRejected(string value)
        => Assert.Equal("angle", ContentValidation.Text("answer", value, 600, out _)?.Reason);

    [Theory]
    [InlineData("bell\u0007here")]
    [InlineData("tab\there")]
    [InlineData("c1\u0085here")]
    public void ControlCharacters_AreRejected(string value)
        => Assert.Equal("control", ContentValidation.Text("answer", value, 600, out _)?.Reason);

    [Fact]
    public void LeadingAndTrailingWhitespace_IsTrimmedNotRejected()
    {
        Assert.Null(ContentValidation.Text("name", "   Ecolab  ", 80, out var clean));
        Assert.Equal("Ecolab", clean);
    }

    [Fact]
    public void OverLength_IsRejectedNamingTheField()
    {
        var error = ContentValidation.Text("answer", new string('a', 601), 600, out _);

        Assert.Equal(new ContentError("answer", "length"), error);
    }

    [Fact]
    public void ExactlyTheLimit_IsAccepted()
        => Assert.Null(ContentValidation.Text("answer", new string('a', 600), 600, out _));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Blank_IsRejectedAsLength(string? value)
        => Assert.Equal("length", ContentValidation.Text("name", value, 80, out _)?.Reason);

    [Theory]
    [InlineData("CA")]
    [InlineData("S")]
    [InlineData("ABC")]
    public void Initials_AcceptOneToThreeCapitals(string value)
        => Assert.Null(ContentValidation.Initials("initials", value, out _));

    [Theory]
    [InlineData("")]
    [InlineData("ca")]
    [InlineData("ABCD")]
    [InlineData("C.A")]
    [InlineData("C A")]
    public void Initials_RejectAnythingElse(string value)
        => Assert.Equal("length", ContentValidation.Initials("initials", value, out _)?.Reason);

    [Fact]
    public void Photo_MustBeInTheAssetAllowlist()
    {
        Assert.Null(ContentValidation.Asset("photo", "faig-garayev", ContentValidation.Photos, out _));
        Assert.Equal("asset", ContentValidation.Asset("photo", "someone-else", ContentValidation.Photos, out _)?.Reason);
    }

    [Fact]
    public void LogoFile_MustBeInTheAssetAllowlist()
    {
        Assert.Null(ContentValidation.Asset("logoFile", "synovate", ContentValidation.Logos, out _));
        Assert.Equal("asset", ContentValidation.Asset("logoFile", "acme", ContentValidation.Logos, out _)?.Reason);
    }

    // A basename, never a path. The column holds one and /api/content builds
    // "/team/<photo>.jpg" from it, so a traversal or an absolute URL must not
    // survive validation even if it were somehow in the allowlist.
    [Theory]
    [InlineData("../../etc/passwd")]
    [InlineData("/team/faig-garayev.jpg")]
    [InlineData("https://example.com/x")]
    [InlineData("Faig-Garayev")]
    public void Asset_RejectsAnythingThatIsNotALowercaseBasename(string value)
        => Assert.Equal("asset", ContentValidation.Asset("photo", value, ContentValidation.Photos, out _)?.Reason);

    [Fact]
    public void IconName_MustBeOneOfTheFourServiceIcons()
    {
        Assert.Equal(4, ContentValidation.ServiceIcons.Length);

        foreach (var name in ContentValidation.ServiceIcons)
        {
            Assert.Null(ContentValidation.Icon("iconName", name, out _));
        }

        Assert.Equal("asset", ContentValidation.Icon("iconName", "Rocket", out _)?.Reason);
    }

    // Every name in the two allowlists must be a file that is actually
    // committed under /public. Without this the panel offers a photo the
    // website does not have and the founder card renders a broken image.
    [Fact]
    public void EveryAllowlistedAsset_ExistsInThePublicFolder()
    {
        var repo = RepositoryRoot();

        foreach (var photo in ContentValidation.Photos)
        {
            Assert.True(File.Exists(Path.Combine(repo, "public", "team", photo + ".jpg")), photo);
        }

        foreach (var logo in ContentValidation.Logos)
        {
            Assert.True(File.Exists(Path.Combine(repo, "public", "logos", logo + ".jpg")), logo);
        }
    }

    private static string RepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, "public", "logos")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Could not locate the repository root from " + AppContext.BaseDirectory);
    }
}
