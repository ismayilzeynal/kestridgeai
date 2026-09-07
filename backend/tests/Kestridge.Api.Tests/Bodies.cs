namespace Kestridge.Api.Tests;

// The exact JSON the endpoint returns. Contact.tsx never reads the body, so
// these are pinned for curl and for the tests, not for the client.
public static class Bodies
{
    public const string Ok = "{\"ok\":true}";
    public const string Origin = "{\"ok\":false,\"error\":\"origin\"}";
    public const string Unavailable = "{\"ok\":false,\"error\":\"unavailable\"}";
    public const string Degraded = "{\"status\":\"degraded\"}";
    public const string Healthy = "{\"status\":\"ok\"}";
    public const string SomeJson = "{\"name\":\"Jane\"}";

    public static string Invalid(string field) =>
        "{\"ok\":false,\"error\":\"invalid\",\"field\":\"" + field + "\"}";
}
