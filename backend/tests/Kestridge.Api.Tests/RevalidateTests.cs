using System.Net;
using Kestridge.Api.Admin;
using Kestridge.Api.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kestridge.Api.Tests;

// The one component in this application that makes an outbound network call.
// It runs after a save has already committed, so every failure mode has to end
// in "false", never in an exception that reaches the operator as a 500 for a
// change that was in fact saved.
public class RevalidateTests
{
    private const string Secret = "ZZSECRETZZ";
    private const string Url = "https://kestridge.test/api/revalidate";

    [Fact]
    public async Task WithNoUrlConfigured_ReportsFalseAndCallsNothing()
    {
        var handler = new Recording(HttpStatusCode.OK);
        var revalidate = Build(handler, new AdminOptions { RevalidateSecret = Secret });

        Assert.False(await revalidate.PublishAsync());
        Assert.Equal(0, handler.Calls);
    }

    // Half-configured is treated as unconfigured, not as "post it without the
    // header". The route on the other end answers 401 to a request with no key,
    // so the alternative is a call that can only ever fail.
    [Fact]
    public async Task WithNoSecretConfigured_ReportsFalseAndCallsNothing()
    {
        var handler = new Recording(HttpStatusCode.OK);
        var revalidate = Build(handler, new AdminOptions { RevalidateUrl = Url });

        Assert.False(await revalidate.PublishAsync());
        Assert.Equal(0, handler.Calls);
    }

    [Fact]
    public async Task OnSuccess_PostsTheKeyHeaderAndReportsTrue()
    {
        var handler = new Recording(HttpStatusCode.OK);
        var revalidate = Build(handler, Configured());

        Assert.True(await revalidate.PublishAsync());
        Assert.Equal(1, handler.Calls);
        Assert.Equal(HttpMethod.Post, handler.Method);
        Assert.Equal(Url, handler.Uri);
        Assert.Equal(Secret, handler.Key);
    }

    [Theory]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData(HttpStatusCode.BadGateway)]
    public async Task OnAnyNonSuccess_ReportsFalseWithoutThrowing(HttpStatusCode status)
    {
        var revalidate = Build(new Recording(status), Configured());

        Assert.False(await revalidate.PublishAsync());
    }

    [Fact]
    public async Task WhenTheCallThrows_ReportsFalseWithoutThrowing()
    {
        var revalidate = Build(new Throwing(), Configured());

        Assert.False(await revalidate.PublishAsync());
    }

    // The URL carries a project hostname and the header carries a shared
    // secret. Neither belongs in journald, and the failure path is exactly
    // where an exception message would otherwise drag them in.
    [Fact]
    public async Task TheFailureLog_CarriesNeitherTheSecretNorTheUrl()
    {
        var logs = new CapturingLoggerProvider();
        var revalidate = Build(new Throwing(), Configured(), logs);

        Assert.False(await revalidate.PublishAsync());
        Assert.NotEmpty(logs.Lines);

        foreach (var line in logs.Lines)
        {
            Assert.DoesNotContain(Secret, line, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("kestridge.test", line, StringComparison.OrdinalIgnoreCase);
        }
    }

    private static AdminOptions Configured() =>
        new() { RevalidateUrl = Url, RevalidateSecret = Secret };

    private static Revalidate Build(
        HttpMessageHandler handler, AdminOptions options, CapturingLoggerProvider? logs = null)
    {
        var factory = LoggerFactory.Create(builder =>
        {
            builder.ClearProviders();
            if (logs is not null)
            {
                builder.AddProvider(logs);
            }
        });

        // Fully qualified: Kestridge.Api.Options shadows the Options class.
        return new Revalidate(
            new HttpClient(handler),
            Microsoft.Extensions.Options.Options.Create(options),
            factory.CreateLogger<Revalidate>());
    }

    private sealed class Recording(HttpStatusCode status) : HttpMessageHandler
    {
        public int Calls { get; private set; }

        public HttpMethod? Method { get; private set; }

        public string? Uri { get; private set; }

        public string? Key { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Calls++;
            Method = request.Method;
            Uri = request.RequestUri?.ToString();
            Key = request.Headers.TryGetValues("x-revalidate-key", out var values)
                ? string.Join(",", values)
                : null;

            return Task.FromResult(new HttpResponseMessage(status));
        }
    }

    private sealed class Throwing : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
            => throw new HttpRequestException("connection refused to " + request.RequestUri);
    }
}
