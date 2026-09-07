using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Http;

namespace Kestridge.Api.Infrastructure;

public static class ClientPartitionKey
{
    public const string Unknown = "unknown";

    // In-memory rate-limiter dictionary key only. Never stored, never logged.
    // IPv6 is masked to /64 because one client is routinely handed a whole /64,
    // which would otherwise bypass per-address limiting for free.
    public static string For(HttpContext context) => For(context.Connection.RemoteIpAddress);

    public static string For(IPAddress? address)
    {
        if (address is null)
        {
            return Unknown;
        }

        if (address.AddressFamily != AddressFamily.InterNetworkV6)
        {
            return address.ToString();
        }

        if (address.IsIPv4MappedToIPv6)
        {
            return address.MapToIPv4().ToString();
        }

        var bytes = address.GetAddressBytes();
        Array.Clear(bytes, 8, 8);
        return new IPAddress(bytes).ToString() + "/64";
    }
}
