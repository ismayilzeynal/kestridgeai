using Microsoft.AspNetCore.Http;

namespace Kestridge.Api.Infrastructure;

// The client never reads the body: Contact.tsx inspects Response.ok only.
// These exist for curl and for the tests.
public static class JsonResults
{
    public static IResult Ok() => Results.Json(new { ok = true }, statusCode: StatusCodes.Status200OK);

    public static IResult Invalid(string field) =>
        Results.Json(new { ok = false, error = "invalid", field }, statusCode: StatusCodes.Status400BadRequest);

    public static IResult Origin() =>
        Results.Json(new { ok = false, error = "origin" }, statusCode: StatusCodes.Status403Forbidden);

    public static IResult TooLarge() =>
        Results.Json(new { ok = false, error = "too_large" }, statusCode: StatusCodes.Status413PayloadTooLarge);

    public static IResult ContentType() =>
        Results.Json(new { ok = false, error = "content_type" }, statusCode: StatusCodes.Status415UnsupportedMediaType);

    public static IResult Unavailable() =>
        Results.Json(new { ok = false, error = "unavailable" }, statusCode: StatusCodes.Status503ServiceUnavailable);

    // One shape for every admin auth failure: wrong password, unknown user,
    // wrong code, replayed code, disabled, locked. A distinct code for "locked"
    // would turn the endpoint into an account existence oracle, because only a
    // real account can be locked.
    public static IResult Auth() =>
        Results.Json(new { ok = false, error = "auth" }, statusCode: StatusCodes.Status401Unauthorized);

    public static IResult Missing() =>
        Results.Json(new { ok = false, error = "not_found" }, statusCode: StatusCodes.Status404NotFound);

    // The id set changed between the preview and the confirm, so the operator
    // would be deleting something they never saw.
    public static IResult Stale() =>
        Results.Json(new { ok = false, error = "stale" }, statusCode: StatusCodes.Status409Conflict);
}
