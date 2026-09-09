using Kestridge.Api.Admin;
using Kestridge.Api.Contact;
using Kestridge.Api.Data;
using Kestridge.Api.Email;
using Kestridge.Api.Health;
using Kestridge.Api.Infrastructure;
using Kestridge.Api.Maintenance;
using Kestridge.Api.Options;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// A CLI mode, before any host is built, so it needs no database and no
// configuration beyond the iteration count. It prints SQL for a human to run as
// the migrator; it never writes to MySQL itself. See AdminBootstrap for why.
if (args.Contains(AdminBootstrap.Flag))
{
    return AdminBootstrap.Run(
        args,
        builder.Configuration.GetSection(AdminOptions.Section).Get<AdminOptions>()?.PasswordIterations
            ?? new AdminOptions().PasswordIterations);
}

builder.Host.UseWindowsService();

if (OperatingSystem.IsWindows())
{
    builder.Logging.AddSimpleConsole();
    builder.Logging.AddEventLog();
}
else
{
    // Not AddSimpleConsole. Under systemd it writes two lines per record with no
    // syslog priority prefix, so journald files every line as its own entry at
    // PRIORITY=6 and "journalctl -p err" returns nothing, ever. The systemd
    // formatter emits one line carrying the priority. Setting
    // Logging:Console:FormatterName in configuration is not enough: the
    // Add*Console call registers its own Configure<ConsoleLoggerOptions> after
    // the configuration binding, and the last registration wins.
    builder.Logging.AddSystemdConsole();
}

builder.Services.AddOptions<CorsOptions>().Bind(builder.Configuration.GetSection(CorsOptions.Section))
    .ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<ContactOptions>().Bind(builder.Configuration.GetSection(ContactOptions.Section))
    .ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<SmtpOptions>().Bind(builder.Configuration.GetSection(SmtpOptions.Section))
    .ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<RateLimitOptions>().Bind(builder.Configuration.GetSection(RateLimitOptions.Section))
    .ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<RetentionOptions>().Bind(builder.Configuration.GetSection(RetentionOptions.Section))
    .ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<NotifyOptions>().Bind(builder.Configuration.GetSection(NotifyOptions.Section))
    .ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<DsrOptions>().Bind(builder.Configuration.GetSection(DsrOptions.Section))
    .ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<AdminOptions>().Bind(builder.Configuration.GetSection(AdminOptions.Section))
    .ValidateDataAnnotations().ValidateOnStart();

// Read eagerly, the same documented way rateLimits is below: this is consumed
// at registration time by Configure<PasswordHasherOptions>, before any request.
var adminSettings = builder.Configuration.GetSection(AdminOptions.Section).Get<AdminOptions>() ?? new AdminOptions();

builder.Services.Configure<PasswordHasherOptions>(o =>
{
    o.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3;
    o.IterationCount = adminSettings.PasswordIterations;
});
builder.Services.AddSingleton<IPasswordHasher<AdminAccount>, PasswordHasher<AdminAccount>>();
builder.Services.AddScoped<AdminTokenFilter>();

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<UtcTimeZoneInterceptor>();
builder.Services.AddSingleton<IEmailSender, MailKitEmailSender>();

var originGuard = new OriginGuard(
    allowVercelPreviews: builder.Configuration.GetValue<bool>($"{CorsOptions.Section}:AllowVercelPreviews"),
    allowLocalhost: builder.Environment.IsDevelopment(),
    additionalOrigins: builder.Configuration
        .GetSection($"{CorsOptions.Section}:AdditionalOrigins").Get<string[]>());
builder.Services.AddSingleton(originGuard);

// AutoDetect would open a blocking connection during DI registration, so the
// process would fail to start whenever MySQL is not up yet - likely, since both
// run on this box and service start order is not guaranteed.
builder.Services.AddDbContext<KestridgeDbContext>((sp, options) =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("Default"),
        new MySqlServerVersion(new Version(8, 0, 43)),
        mysql => mysql.CommandTimeout(15));
    options.AddInterceptors(sp.GetRequiredService<UtcTimeZoneInterceptor>());
});

builder.Services.AddCors(options => options.AddPolicy("site", policy => policy
    .SetIsOriginAllowed(originGuard.IsAllowed)
    .WithMethods("POST")
    .WithHeaders("Accept", "Content-Type")
    .SetPreflightMaxAge(TimeSpan.FromHours(1))));

var rateLimits = builder.Configuration.GetSection(RateLimitOptions.Section).Get<RateLimitOptions>()
                 ?? new RateLimitOptions();
builder.Services.AddRateLimiter(options => RateLimiting.Configure(options, rateLimits));

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
    options.KnownProxies.Add(IPAddress.Loopback);
    options.KnownProxies.Add(IPAddress.IPv6Loopback);
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 64 * 1024;
    options.ValueLengthLimit = 32 * 1024;
    options.ValueCountLimit = 16;
    options.KeyLengthLimit = 128;
    options.MultipartHeadersLengthLimit = 8 * 1024;
    options.MultipartBoundaryLengthLimit = 128;
});

builder.WebHost.ConfigureKestrel(kestrel =>
{
    kestrel.Limits.MaxRequestBodySize = 128 * 1024;
    kestrel.Limits.MaxRequestHeadersTotalSize = 16 * 1024;
    kestrel.AddServerHeader = false;
});

builder.Services.AddHostedService<MaintenanceService>();

// A notify sweep sends one message at a time with an uncancellable SMTP call,
// bounded by MailKit's 15 second timeout. Set explicitly rather than relying on
// the host default, which has changed between releases, and keep systemd's
// TimeoutStopSec strictly above it so systemd never SIGKILLs a drain in progress.
builder.Services.Configure<HostOptions>(options =>
    options.ShutdownTimeout = TimeSpan.FromSeconds(60));

var app = builder.Build();

app.UseForwardedHeaders();

// Registered before UseCors on purpose. The exception handler clears the
// response, but the CORS middleware still runs on the way out and re-applies
// Access-Control-Allow-Origin, so a 500 keeps the header. CorsTests pins that:
// without it a 500 reaches the browser as an opaque network failure for a
// submission the server may already have stored, and the visitor resubmits.
app.UseExceptionHandler(branch => branch.Run(async context =>
{
    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    context.Response.ContentType = "application/json; charset=utf-8";
    await context.Response.WriteAsync("""{"ok":false,"error":"server"}""");
}));

// Before UseRateLimiter, or a 429 goes out with no Access-Control-Allow-Origin
// and the browser reports an opaque network failure instead of a rejection.
app.UseCors("site");
app.UseRateLimiter();

app.Use(async (context, next) =>
{
    context.Response.Headers.CacheControl = "no-store";
    await next();
});

// Rooted at wwwroot/admin with RequestPath "/admin", not at wwwroot, so nothing
// else in the publish output is reachable. Guarded so that
// "rm -rf /srv/kestridge-api/wwwroot/admin" is a valid UI-only rollback that
// leaves /api/contact serving.
//
// Registered after the no-store middleware, so the panel's own documents are
// no-store too. Correct for an admin page, and it costs one refetch per load
// for four users. Do not scope that middleware to /api to avoid the refetch:
// the risk of accidentally weakening no-store on submission JSON is larger.
var adminRoot = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "admin");

if (Directory.Exists(adminRoot))
{
    // UseDefaultFiles only rewrites a path that already ends in a slash, so
    // GET /admin would 404 without this. It has to be middleware, not a mapped
    // route: routing normalises the trailing slash, so MapGet("/admin") also
    // matches "/admin/" and redirects it to itself forever. PathString compares
    // exactly and keeps the two apart.
    app.Use(async (context, next) =>
    {
        if (context.Request.Path == "/admin")
        {
            context.Response.Redirect("/admin/", permanent: true);
            return;
        }

        await next();
    });

    var adminFiles = new PhysicalFileProvider(adminRoot);

    app.UseDefaultFiles(new DefaultFilesOptions { FileProvider = adminFiles, RequestPath = "/admin" });
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = adminFiles,
        RequestPath = "/admin",
        ServeUnknownFileTypes = false,
        OnPrepareResponse = ctx =>
        {
            var headers = ctx.Context.Response.Headers;

            // require-trusted-types-for 'script' is the load bearing one: it
            // means the panel can never assign innerHTML. Submission text is
            // attacker controlled, arriving from a public form, and rendering it
            // that way would be stored XSS on the origin that holds the token.
            headers["Content-Security-Policy"] =
                "default-src 'none'; script-src 'self'; style-src 'self'; img-src 'self' data:; "
                + "font-src 'self'; connect-src 'self'; base-uri 'none'; form-action 'none'; "
                + "frame-ancestors 'none'; object-src 'none'; require-trusted-types-for 'script'";
            headers["Referrer-Policy"] = "no-referrer";
            headers["X-Frame-Options"] = "DENY";
            headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";

            // X-Content-Type-Options and Strict-Transport-Security already come
            // from the server level add_header in nginx-api.conf. Setting them
            // here as well would emit each twice.
        },
    });
}
else
{
    app.Logger.LogWarning("admin.static_missing path={Path}", adminRoot);
}

app.MapContact();
app.MapHealth();
app.MapAdmin();

app.Run();
return 0;

public partial class Program;
