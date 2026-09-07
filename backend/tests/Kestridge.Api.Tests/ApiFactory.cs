using Kestridge.Api.Data;
using Kestridge.Api.Email;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;

namespace Kestridge.Api.Tests;

public sealed class ApiFactory : WebApplicationFactory<Program>
{
    public FakeEmailSender Email { get; } = new();

    public FakeTimeProvider Clock { get; } = new(new DateTimeOffset(2026, 9, 7, 12, 0, 0, TimeSpan.Zero));

    public CapturingLoggerProvider Logs { get; } = new();

    public string ConnectionString { get; init; } = string.Empty;

    public Dictionary<string, string?> Settings { get; init; } = [];

    // TestServer leaves Connection.RemoteIpAddress null, so without this every
    // request in the suite lands in the same rate-limit partition and the
    // per-client limiter is never actually exercised.
    public const string ClientAddressHeader = "X-Test-Client-Address";

    // Set to make the handler throw where nothing catches it, so the
    // UseExceptionHandler branch and the CORS safety net get real coverage.
    public bool ThrowFromClock { get; init; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(Environments.Production);

        builder.UseSetting("ConnectionStrings:Default", ConnectionString);
        builder.UseSetting("Kestridge:Contact:ToAddress", "team@kestridge.test");
        builder.UseSetting("Kestridge:Contact:FromAddress", "no-reply@kestridge.test");
        builder.UseSetting("Kestridge:Smtp:Host", "localhost");
        builder.UseSetting("Kestridge:Dsr:EmailHashPepper", "test-pepper-value-0123456789");
        builder.UseSetting("Kestridge:RateLimit:PermitsPerWindow", "100000");
        builder.UseSetting("Kestridge:RateLimit:GlobalPerHour", "10000000");
        // The maintenance loop is driven explicitly in the tests that need it.
        builder.UseSetting("Kestridge:Notify:SweepSeconds", "3600");

        foreach (var pair in Settings)
        {
            builder.UseSetting(pair.Key, pair.Value);
        }

        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.SetMinimumLevel(LogLevel.Trace);
            logging.AddProvider(Logs);
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IEmailSender>();
            services.AddSingleton<IEmailSender>(Email);

            services.RemoveAll<TimeProvider>();
            if (ThrowFromClock)
            {
                services.AddSingleton<TimeProvider>(new ThrowingTimeProvider());
            }
            else
            {
                services.AddSingleton<TimeProvider>(Clock);
            }

            services.RemoveAll<IHostedService>();

            // An IStartupFilter runs before the application pipeline, which is
            // the only way to set the remote address early enough for the rate
            // limiter to partition on it.
            services.AddSingleton<IStartupFilter, ClientAddressStartupFilter>();
        });
    }

    public KestridgeDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<KestridgeDbContext>()
            .UseMySql(ConnectionString, new MySqlServerVersion(new Version(8, 0, 43)))
            .AddInterceptors(new UtcTimeZoneInterceptor())
            .Options;

        return new KestridgeDbContext(options);
    }
}
