namespace Kestridge.Api.Contact;

// ToString is overridden so an accidental log.LogInformation("{Input}", input)
// cannot leak a submission. LoggingHygieneTests asserts it.
public sealed record ContactInput(
    string Name,
    string Email,
    string Company,
    string Phone,
    string Service,
    string Message,
    string Gotcha)
{
    public override string ToString() => "ContactInput(redacted)";
}
