using System.Globalization;
using Kestridge.Api.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Kestridge.Api.Admin;

// A CLI mode in the same binary, run on the server. It NEVER opens MySQL: it
// prints values for a human to paste into ops/admin-account.sql, which the
// migrator runs.
//
// That indirection is the point. It means kestridge_app needs no INSERT on
// admin_accounts and no privilege over password_hash or totp_secret, so a
// compromised web process cannot mint itself an account. A bootstrap HTTP
// endpoint, or an "if the table is empty create one from config" startup path,
// both re-open exactly that door the moment someone deletes the last row.
public static class AdminBootstrap
{
    public const string Flag = "--hash-password";

    public static int Run(string[] args, int iterations)
    {
        var username = ValueAfter(args, "--username")?.Trim().ToLowerInvariant();
        var displayName = ValueAfter(args, "--display-name")?.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(displayName))
        {
            Console.Error.WriteLine("Usage: Kestridge.Api --hash-password --username <name> --display-name \"Full Name\"");
            Console.Error.WriteLine("The password is read from the terminal and never appears on the command line.");
            return 2;
        }

        Console.Error.Write("Password: ");
        var password = ReadHidden();
        Console.Error.WriteLine();

        if (password.Length < 12)
        {
            Console.Error.WriteLine("Refusing: use at least 12 characters.");
            return 2;
        }

        Console.Error.Write("Repeat:   ");
        var again = ReadHidden();
        Console.Error.WriteLine();

        if (!string.Equals(password, again, StringComparison.Ordinal))
        {
            Console.Error.WriteLine("Refusing: the two entries differ.");
            return 2;
        }

        var hasherOptions = new PasswordHasherOptions
        {
            CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3,
            IterationCount = iterations,
        };

        var hasher = new PasswordHasher<AdminAccount>(new OptionsWrapper<PasswordHasherOptions>(hasherOptions));
        var hash = hasher.HashPassword(new AdminAccount(), password);
        var secret = Totp.NewSecret();

        Console.WriteLine();
        Console.WriteLine("-- Paste into ops/admin-account.sql and run as kestridge_migrator.");
        Console.WriteLine("INSERT INTO admin_accounts");
        Console.WriteLine("  (username, display_name, password_hash, totp_secret, created_at)");
        Console.WriteLine(string.Format(
            CultureInfo.InvariantCulture,
            "VALUES ('{0}', '{1}', '{2}', '{3}', UTC_TIMESTAMP(6));",
            username,
            displayName.Replace("'", "''"),
            hash,
            secret));
        Console.WriteLine();
        Console.WriteLine("-- Scan this into an authenticator app, then clear the scrollback.");
        Console.WriteLine($"otpauth://totp/Kestridge:{username}?secret={secret}&issuer=Kestridge&algorithm=SHA1&digits=6&period=30");
        Console.WriteLine();

        return 0;
    }

    private static string? ValueAfter(string[] args, string name)
    {
        var index = Array.IndexOf(args, name);
        return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
    }

    // Console.ReadLine would echo the password into the scrollback and, over
    // SSH, into anything recording the session.
    private static string ReadHidden()
    {
        var buffer = new System.Text.StringBuilder();

        while (true)
        {
            var key = Console.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Enter)
            {
                return buffer.ToString();
            }

            if (key.Key == ConsoleKey.Backspace)
            {
                if (buffer.Length > 0)
                {
                    buffer.Length--;
                }

                continue;
            }

            if (!char.IsControl(key.KeyChar))
            {
                buffer.Append(key.KeyChar);
            }
        }
    }
}
