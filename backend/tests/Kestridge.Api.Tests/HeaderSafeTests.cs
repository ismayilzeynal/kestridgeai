using Kestridge.Api.Email;

namespace Kestridge.Api.Tests;

public class HeaderSafeTests
{
    [Fact]
    public void StripsCrLfAndNul() =>
        Assert.Equal("Bob Bcc: x", HeaderSafe.Clean("Bob\r\nBcc:\u0000 x", 200));

    [Fact]
    public void CollapsesWhitespaceRuns() =>
        Assert.Equal("a b", HeaderSafe.Clean("a    \t   b", 200));

    [Fact]
    public void TrimsAndCaps() =>
        Assert.Equal("abcde", HeaderSafe.Clean("   abcdefghij   ", 5));

    [Fact]
    public void EmptyStaysEmpty() => Assert.Equal("", HeaderSafe.Clean("", 10));
}
