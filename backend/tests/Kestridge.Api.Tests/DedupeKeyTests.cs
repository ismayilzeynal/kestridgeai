using System.Text.RegularExpressions;
using Kestridge.Api.Contact;

namespace Kestridge.Api.Tests;

public class DedupeKeyTests
{
    private static readonly DateTime Base = new(2026, 9, 7, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void DedupeKey_IsLowercaseHex64()
    {
        var key = DedupeKey.Compute("jane@company.com", "hello", Base, 600);
        Assert.Equal(64, key.Length);
        Assert.Matches(new Regex("^[0-9a-f]{64}$"), key);
    }

    [Fact]
    public void SameInputsInSameWindow_ProduceTheSameKey()
    {
        var a = DedupeKey.Compute("jane@company.com", "hello", Base, 600);
        var b = DedupeKey.Compute("jane@company.com", "hello", Base.AddSeconds(59), 600);
        Assert.Equal(a, b);
    }

    [Fact]
    public void CrossingTheWindowBoundary_ProducesADifferentKey()
    {
        var a = DedupeKey.Compute("jane@company.com", "hello", Base, 600);
        var b = DedupeKey.Compute("jane@company.com", "hello", Base.AddSeconds(600), 600);
        Assert.NotEqual(a, b);
    }

    [Fact]
    public void EmailCaseDifference_ProducesTheSameKey()
    {
        var a = DedupeKey.Compute("Jane@Company.COM", "hello", Base, 600);
        var b = DedupeKey.Compute("jane@company.com", "hello", Base, 600);
        Assert.Equal(a, b);
    }

    [Fact]
    public void DifferentMessage_ProducesADifferentKey()
    {
        var a = DedupeKey.Compute("jane@company.com", "hello", Base, 600);
        var b = DedupeKey.Compute("jane@company.com", "hello there", Base, 600);
        Assert.NotEqual(a, b);
    }

    [Fact]
    public void DifferentEmail_ProducesADifferentKey()
    {
        var a = DedupeKey.Compute("jane@company.com", "hello", Base, 600);
        var b = DedupeKey.Compute("john@company.com", "hello", Base, 600);
        Assert.NotEqual(a, b);
    }
}
