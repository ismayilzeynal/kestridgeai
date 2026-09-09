using Kestridge.Api.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kestridge.Api.Admin;

// Tells the marketing site that a page it prerendered is now stale. Without
// this an edit is live within the 300 second ISR window; with it, on the next
// request after the save.
//
// It is deliberately the weakest link in the chain. The save has already
// committed by the time this runs, and a failure here is reported to the
// operator as "saved, the website did not confirm" rather than as an error,
// because the content is in fact saved and will appear on its own.
//
// No cookie, no session, one header. Empty configuration means never call out,
// which is what every test and every developer machine runs with.
public sealed class Revalidate(HttpClient http, IOptions<AdminOptions> options, ILogger<Revalidate> log)
{
    public async Task<bool> PublishAsync()
    {
        var settings = options.Value;

        if (settings.RevalidateUrl.Length == 0 || settings.RevalidateSecret.Length == 0)
        {
            return false;
        }

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, settings.RevalidateUrl);
            request.Headers.TryAddWithoutValidation("x-revalidate-key", settings.RevalidateSecret);

            // Its own token, not RequestAborted. An operator who closes the tab
            // the instant they click Save must not leave the site serving a
            // page it has already been told is stale.
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            using var response = await http.SendAsync(request, timeout.Token);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            // The type only. The URL carries a project hostname and the header
            // carries a shared secret, and neither belongs in journald.
            log.LogWarning("content.revalidate_failed reason={Reason}", ex.GetType().Name);
            return false;
        }
    }
}
