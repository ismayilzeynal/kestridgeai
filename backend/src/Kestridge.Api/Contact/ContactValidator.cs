using System.Text.RegularExpressions;
using Kestridge.Api.Options;
using MimeKit;

namespace Kestridge.Api.Contact;

public readonly record struct ValidationResult(bool Ok, string Field)
{
    public static readonly ValidationResult Valid = new(true, string.Empty);
    public static ValidationResult Invalid(string field) => new(false, field);
}

public static partial class ContactValidator
{
    public static ValidationResult Validate(ContactInput input, ContactOptions opts)
    {
        if (input.Name.Length == 0 || input.Name.Length > ContactFormReader.NameMax)
        {
            return ValidationResult.Invalid("name");
        }

        if (!IsEmail(input.Email))
        {
            return ValidationResult.Invalid("email");
        }

        if (input.Company.Length > ContactFormReader.CompanyMax)
        {
            return ValidationResult.Invalid("company");
        }

        if (input.Phone.Length > ContactFormReader.PhoneMax)
        {
            return ValidationResult.Invalid("phone");
        }

        if (Array.IndexOf(opts.AllowedServices, input.Service) < 0)
        {
            return ValidationResult.Invalid("service");
        }

        // No minimum length. The client's 10-character rule is a UX nudge;
        // rejecting "Call me" server-side loses a real lead for nothing.
        if (input.Message.Length == 0 || input.Message.Length > opts.MaxMessageLength)
        {
            return ValidationResult.Invalid("message");
        }

        return ValidationResult.Valid;
    }

    private static bool IsEmail(string value)
    {
        if (value.Length == 0 || value.Length > ContactFormReader.EmailMax)
        {
            return false;
        }

        if (!Shape().IsMatch(value))
        {
            return false;
        }

        // Rejects display-name syntax such as "Bob" <bob@x.com>, which would
        // otherwise produce a Reply-To pointing somewhere the visitor did not
        // type. MimeKit is the parser that will build the header later.
        return MailboxAddress.TryParse(ParserOptions.Default, value, out var mailbox)
               && string.Equals(mailbox.Address, value, StringComparison.Ordinal);
    }

    // Deliberately identical to the client regex in Contact.tsx, so nothing the
    // browser accepted is rejected here for shape.
    [GeneratedRegex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", RegexOptions.CultureInvariant)]
    private static partial Regex Shape();
}
