using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Kestridge.Api.Tests;

// TestServer never sets Connection.RemoteIpAddress, so without this every
// request in the suite shares one rate-limit partition and neither the per-client
// limiter nor the forwarded-headers configuration is ever really exercised.
public sealed class ClientAddressStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) => app =>
    {
        app.Use(async (context, continuation) =>
        {
            var header = context.Request.Headers[ApiFactory.ClientAddressHeader].ToString();
            if (header.Length > 0 && IPAddress.TryParse(header, out var address))
            {
                context.Connection.RemoteIpAddress = address;
            }

            await continuation();
        });

        next(app);
    };
}
