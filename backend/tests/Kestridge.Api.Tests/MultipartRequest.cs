using System.Net.Http.Headers;
using System.Text;

namespace Kestridge.Api.Tests;

public static class MultipartRequest
{
    public const string Origin = "https://kestridge.com";

    // The seven parts the browser sends, in DOM order. Blank optional fields
    // arrive as empty-string parts, never absent.
    public static HttpRequestMessage Contact(
        string name = "Jane Doe",
        string email = "jane@company.com",
        string company = "",
        string phone = "",
        string service = "ai",
        string message = "We need an intake process for incoming orders.",
        string gotcha = "",
        string? origin = Origin,
        string? clientAddress = null,
        IEnumerable<KeyValuePair<string, string>>? extra = null)
    {
        var content = new MultipartFormDataContent("----WebKitFormBoundaryTest");
        Add(content, "name", name);
        Add(content, "email", email);
        Add(content, "company", company);
        Add(content, "phone", phone);
        Add(content, "service", service);
        Add(content, "message", message);
        Add(content, "_gotcha", gotcha);

        if (extra is not null)
        {
            foreach (var pair in extra)
            {
                Add(content, pair.Key, pair.Value);
            }
        }

        return Build(content, origin, clientAddress);
    }

    public static HttpRequestMessage Build(HttpContent content, string? origin, string? clientAddress = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/contact") { Content = content };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        if (origin is not null)
        {
            request.Headers.Add("Origin", origin);
        }

        if (clientAddress is not null)
        {
            request.Headers.Add(ApiFactory.ClientAddressHeader, clientAddress);
        }

        return request;
    }

    private static void Add(MultipartFormDataContent content, string name, string value)
    {
        var part = new StringContent(value, Encoding.UTF8);
        part.Headers.ContentType = null;
        content.Add(part, name);
    }
}
