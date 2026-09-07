using Kestridge.Api.Contact;
using Kestridge.Api.Options;

namespace Kestridge.Api.Tests;

public class ContactValidatorTests
{
    private static readonly ContactOptions Opts = new()
    {
        AllowedServices = ["ai", "analytics", "automation", "security", "general"],
        MaxMessageLength = 5000,
        ToAddress = "team@kestridge.test",
        FromAddress = "no-reply@kestridge.test",
    };

    private static ContactInput Input(
        string name = "Jane Doe",
        string email = "jane@company.com",
        string company = "",
        string phone = "",
        string service = "ai",
        string message = "We need an intake process.")
        => new(name, email, company, phone, service, message, string.Empty);

    private static string Field(ContactInput input) => ContactValidator.Validate(input, Opts).Field;

    private static bool Ok(ContactInput input) => ContactValidator.Validate(input, Opts).Ok;

    [Fact]
    public void Name_Empty_IsRejected() => Assert.Equal("name", Field(Input(name: "")));

    [Fact]
    public void Name_Over200Chars_IsRejected() => Assert.Equal("name", Field(Input(name: new string('a', 201))));

    [Fact]
    public void Name_Exactly200Chars_IsAccepted() => Assert.True(Ok(Input(name: new string('a', 200))));

    [Fact]
    public void Email_Empty_IsRejected() => Assert.Equal("email", Field(Input(email: "")));

    [Fact]
    public void Email_WithoutDot_IsRejected() => Assert.Equal("email", Field(Input(email: "a@b")));

    [Fact]
    public void Email_MinimalValid_IsAccepted() => Assert.True(Ok(Input(email: "a@b.c")));

    [Fact]
    public void Email_WithDisplayNameSyntax_IsRejected() =>
        Assert.Equal("email", Field(Input(email: "\"Bob\" <bob@x.com>")));

    [Fact]
    public void Email_Over254Chars_IsRejected()
    {
        var local = new string('a', 250);
        Assert.Equal("email", Field(Input(email: $"{local}@example.com")));
    }

    [Fact]
    public void Service_NotInConfiguredList_IsRejected() => Assert.Equal("service", Field(Input(service: "hacker")));

    [Fact]
    public void Service_Empty_IsRejected() => Assert.Equal("service", Field(Input(service: "")));

    [Theory]
    [InlineData("ai")]
    [InlineData("analytics")]
    [InlineData("automation")]
    [InlineData("security")]
    [InlineData("general")]
    public void Service_EachOfTheFiveConfiguredValues_IsAccepted(string service) =>
        Assert.True(Ok(Input(service: service)));

    [Fact]
    public void Message_Empty_IsRejected() => Assert.Equal("message", Field(Input(message: "")));

    // The client's 10-character rule is a UX nudge. Enforcing it here would lose
    // a real lead that says "Call me".
    [Fact]
    public void Message_SixCharacters_IsAccepted() => Assert.True(Ok(Input(message: "Call m")));

    [Fact]
    public void Message_Exactly5000Chars_IsAccepted() => Assert.True(Ok(Input(message: new string('x', 5000))));

    [Fact]
    public void Message_5001Chars_IsRejected() => Assert.Equal("message", Field(Input(message: new string('x', 5001))));

    [Fact]
    public void Phone_Over64Chars_IsRejected() => Assert.Equal("phone", Field(Input(phone: new string('1', 65))));

    [Fact]
    public void Company_Over200Chars_IsRejected() => Assert.Equal("company", Field(Input(company: new string('c', 201))));
}
