namespace Kestridge.Api.Admin;

// Applied when a user is created from the panel, and to the password a new user
// chooses. Existing accounts are never revalidated against these: an account
// made by SQL with a name outside the rule still signs in.
public static class AdminUserRules
{
    public const int PasswordMin = 12;
    public const int PasswordMax = 128;

    // The same normalisation login applies before its lookup, so a name that is
    // stored is a name that can be typed.
    public static string ForLookup(string? username)
        => (username ?? string.Empty).Trim().ToLowerInvariant();

    // Whether a normalised name is worth a query at all. Both username columns
    // are ascii_bin, and MySQL refuses to compare one with a parameter holding
    // anything outside ASCII: ERROR 1267, an illegal mix of collations, not an
    // empty result. ToLowerInvariant leaves the Azerbaijani dotted capital I as
    // it is, so a name a phone keyboard capitalised would answer 500. No stored
    // name can contain such a character, so the caller treats it as an unknown
    // user, dummy hash and all.
    public static bool CanMatch(string username)
        => username.Length is > 0 and <= 64 && System.Text.Ascii.IsValid(username);

    // ^[a-z0-9][a-z0-9._-]{1,63}$ after trimming and lowercasing. Nothing that
    // needs escaping in an otpauth label, a mail subject or a SQL literal, and
    // no leading punctuation that reads as a flag on a command line.
    public static string? NewUsername(string? value)
    {
        var username = ForLookup(value);

        if (username.Length is < 2 or > 64 || !IsAlphanumeric(username[0]))
        {
            return null;
        }

        foreach (var c in username)
        {
            if (!IsAlphanumeric(c) && c is not ('.' or '_' or '-'))
            {
                return null;
            }
        }

        return username;
    }

    // Length only, by string.Length, and never trimmed: a space the person
    // typed is part of the password. No composition rules, which push people
    // towards Password1! and add nothing a length floor does not.
    public static bool PasswordLengthOk(string? password)
        => password is { Length: >= PasswordMin and <= PasswordMax };

    private static bool IsAlphanumeric(char c) => c is (>= 'a' and <= 'z') or (>= '0' and <= '9');
}
