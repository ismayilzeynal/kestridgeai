using System.ComponentModel.DataAnnotations;

namespace Kestridge.Api.Options;

// Nothing here is [Required] and nothing lacks a default, deliberately. A
// required member with no default plus ValidateOnStart makes every test in the
// suite fail at host startup until ApiFactory supplies a UseSetting, the way it
// already has to for Kestridge:Dsr:EmailHashPepper. There are no admin secrets
// in configuration at all: the credential is a database row.
public sealed class AdminOptions
{
    public const string Section = "Kestridge:Admin";

    [Range(1, 168)]
    public int SessionHours { get; set; } = 12;

    [Range(1, 1440)]
    public int IdleMinutes { get; set; } = 30;

    [Range(1, 20)]
    public int MaxFailedAttempts { get; set; } = 5;

    // Fixed, and it does not escalate. Anyone who knows a username can keep the
    // account locked, so a growing lock would be a denial of service lever
    // against the owner. RUNBOOK.md documents ops/admin-unlock.sql.
    [Range(1, 1440)]
    public int LockMinutes { get; set; } = 15;

    // Only slide the idle expiry once a minute, so browsing does not produce an
    // UPDATE per request.
    [Range(1, 600)]
    public int SlideAfterSeconds { get; set; } = 60;

    [Range(0, 3)]
    public int TotpSkewSteps { get; set; } = 1;

    [Range(1000, 2000000)]
    public int PasswordIterations { get; set; } = 210_000;

    // How long a new user, or an account whose authenticator was reset, has to
    // complete first sign-in. Long enough to cover a weekend, short enough that
    // an initial password handed over and forgotten does not stay live.
    [Range(1, 720)]
    public int EnrollHours { get; set; } = 72;

    // Life of the setup token login/start hands out with the QR code. Covers
    // installing an authenticator app; anything slower signs in again and gets
    // a fresh secret.
    [Range(1, 60)]
    public int EnrollTokenMinutes { get; set; } = 15;

    [Range(1, 100000)]
    public int ExportMaxRows { get; set; } = 5000;

    // Both empty by default, and empty means never call out. That is what
    // lets the revalidate hook be genuinely optional: cut it and every save
    // still works, the panel just says the change appears within 5 minutes.
    // The secret lives in appsettings.Production.json (0640 root:kestridge,
    // already outside rsync --delete) and nowhere in git.
    public string RevalidateUrl { get; set; } = string.Empty;

    public string RevalidateSecret { get; set; } = string.Empty;
}
