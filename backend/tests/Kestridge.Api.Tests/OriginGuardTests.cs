using Kestridge.Api.Contact;

namespace Kestridge.Api.Tests;

public class OriginGuardTests
{
    private static OriginGuard Production => new(allowVercelPreviews: false, allowLocalhost: false);

    private static OriginGuard Preview => new(allowVercelPreviews: true, allowLocalhost: false);

    private static OriginGuard Development => new(allowVercelPreviews: true, allowLocalhost: true);

    [Fact]
    public void Allows_ProductionApex() => Assert.True(Production.IsAllowed("https://kestridge.com"));

    [Fact]
    public void Allows_ProductionWww() => Assert.True(Production.IsAllowed("https://www.kestridge.com"));

    [Fact]
    public void Rejects_UnrelatedOrigin() => Assert.False(Production.IsAllowed("https://evil.com"));

    [Fact]
    public void Rejects_EmptyOrigin() => Assert.False(Production.IsAllowed(""));

    [Fact]
    public void Rejects_NullOrigin() => Assert.False(Production.IsAllowed(null));

    // An unanchored pattern passes every other test in this class.
    [Fact]
    public void Rejects_SuffixAttackOnVercelPattern() =>
        Assert.False(Preview.IsAllowed("https://kestridgeai.vercel.app.attacker.com"));

    [Fact]
    public void Rejects_PrefixAttackOnVercelPattern() =>
        Assert.False(Preview.IsAllowed("https://evil.com/kestridgeai.vercel.app"));

    [Theory]
    [InlineData("https://kestridgeai.vercel.app")]
    [InlineData("https://kestridgeai-git-main-abc.vercel.app")]
    [InlineData("https://kestridgeai-9x8y7z-team.vercel.app")]
    public void Allows_VercelPreview_WhenPreviewsEnabled(string origin) => Assert.True(Preview.IsAllowed(origin));

    [Fact]
    public void Rejects_VercelPreview_WhenPreviewsDisabled() =>
        Assert.False(Production.IsAllowed("https://kestridgeai-git-main-abc.vercel.app"));

    [Fact]
    public void Rejects_HttpApexEvenWhenHttpsAllowed() => Assert.False(Production.IsAllowed("http://kestridge.com"));

    [Theory]
    [InlineData("http://localhost:3000")]
    [InlineData("http://localhost:3111")]
    [InlineData("http://127.0.0.1:3000")]
    [InlineData("http://127.0.0.1:3111")]
    public void Allows_LocalhostOrigins_OnlyInDevelopment(string origin)
    {
        Assert.True(Development.IsAllowed(origin));
        Assert.False(Production.IsAllowed(origin));
    }


    // The staging escape hatch. Exact strings only, so it cannot widen into a
    // pattern by accident, and it is emptied at cutover.
    [Fact]
    public void Allows_AnAdditionalOrigin_WhenConfigured()
    {
        var guard = new OriginGuard(false, false, ["http://195.26.245.188"]);
        Assert.True(guard.IsAllowed("http://195.26.245.188"));
    }

    [Fact]
    public void Rejects_AnAdditionalOrigin_WhenNotConfigured()
    {
        var guard = new OriginGuard(false, false);
        Assert.False(guard.IsAllowed("http://195.26.245.188"));
    }

    [Fact]
    public void AdditionalOrigins_AreNotTreatedAsPrefixes()
    {
        var guard = new OriginGuard(false, false, ["http://195.26.245.188"]);
        Assert.False(guard.IsAllowed("http://195.26.245.188.attacker.com"));
        Assert.False(guard.IsAllowed("http://195.26.245.1"));
    }

    [Fact]
    public void AdditionalOrigins_IgnoreBlankEntries()
    {
        var guard = new OriginGuard(false, false, ["", "   "]);
        Assert.False(guard.IsAllowed(""));
        Assert.False(guard.IsAllowed("   "));
    }
}
