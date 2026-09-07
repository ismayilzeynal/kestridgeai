using System.Net;
using Kestridge.Api.Infrastructure;

namespace Kestridge.Api.Tests;

public class ClientPartitionKeyTests
{
    [Fact]
    public void IPv4_ReturnsExactAddress() =>
        Assert.Equal("203.0.113.9", ClientPartitionKey.For(IPAddress.Parse("203.0.113.9")));

    [Fact]
    public void IPv6_IsMaskedToSlash64()
    {
        var a = ClientPartitionKey.For(IPAddress.Parse("2001:db8:1:2:aaaa:bbbb:cccc:dddd"));
        var b = ClientPartitionKey.For(IPAddress.Parse("2001:db8:1:2:1111:2222:3333:4444"));

        Assert.Equal(a, b);
        Assert.EndsWith("/64", a, StringComparison.Ordinal);
    }

    [Fact]
    public void DifferentIPv6Prefixes_AreDifferentPartitions()
    {
        var a = ClientPartitionKey.For(IPAddress.Parse("2001:db8:1:2::1"));
        var b = ClientPartitionKey.For(IPAddress.Parse("2001:db8:1:3::1"));

        Assert.NotEqual(a, b);
    }

    [Fact]
    public void IPv4MappedToIPv6_ReturnsTheIPv4Address() =>
        Assert.Equal("203.0.113.9", ClientPartitionKey.For(IPAddress.Parse("::ffff:203.0.113.9")));

    // TestServer gives a null RemoteIpAddress, so the fallback has to be
    // deterministic rather than throwing.
    [Fact]
    public void NullRemoteAddress_ReturnsUnknownConstant() =>
        Assert.Equal(ClientPartitionKey.Unknown, ClientPartitionKey.For((IPAddress?)null));
}
