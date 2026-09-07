using Kestridge.Api.Data;
using Kestridge.Api.Email;
using Kestridge.Api.Infrastructure;
using Kestridge.Api.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace Kestridge.Api.Contact;

public static class ContactEndpoint
{
    private const int DuplicateKeyErrorNumber = 1062;

    public static void MapContact(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/contact", Handle)
            .RequireCors("site")
            // Rate limiting is applied by the chained global limiter, not by an
            // endpoint policy. See Infrastructure/RateLimiting.cs for why the
            // order of the two limiters is load bearing.
            // Binding HttpRequest rather than a form parameter is what actually
            // keeps antiforgery metadata off this endpoint. DisableAntiforgery
            // states the intent for whoever refactors the signature later.
            .DisableAntiforgery()
            .WithMetadata(new RequestSizeLimitAttribute(64 * 1024));
    }

    private static async Task<IResult> Handle(
        HttpRequest request,
        KestridgeDbContext db,
        IEmailSender mail,
        OriginGuard originGuard,
        IOptions<ContactOptions> contactOptions,
        IOptions<RetentionOptions> retentionOptions,
        TimeProvider clock,
        ILoggerFactory loggerFactory,
        CancellationToken ct)
    {
        var log = loggerFactory.CreateLogger("Kestridge.Api.Contact");
        var opts = contactOptions.Value;

        var origin = request.Headers.Origin.ToString();
        if (origin.Length == 0)
        {
            if (opts.RequireOrigin)
            {
                log.LogInformation("contact.origin_missing");
                return JsonResults.Origin();
            }
        }
        else if (!originGuard.IsAllowed(origin))
        {
            log.LogInformation("contact.origin_rejected");
            return JsonResults.Origin();
        }

        if (!request.HasFormContentType)
        {
            return JsonResults.ContentType();
        }

        IFormCollection form;
        try
        {
            form = await request.ReadFormAsync(ct);
        }
        catch (InvalidDataException)
        {
            return JsonResults.TooLarge();
        }
        catch (BadHttpRequestException ex)
        {
            return ex.StatusCode == StatusCodes.Status413PayloadTooLarge
                ? JsonResults.TooLarge()
                : JsonResults.Invalid("form");
        }

        // The form has no file input. A file part is an attack or a
        // misconfiguration either way.
        if (form.Files.Count > 0)
        {
            return JsonResults.Invalid("form");
        }

        var input = ContactFormReader.Read(form);

        // Before validation, so a bot that fills every field with garbage still
        // gets the identical 200 rather than a 400 that reveals the trap.
        if (input.Gotcha.Length > 0)
        {
            log.LogInformation("contact.honeypot");
            return JsonResults.Ok();
        }

        var validation = ContactValidator.Validate(input, opts);
        if (!validation.Ok)
        {
            log.LogInformation("contact.invalid field={Field}", validation.Field);
            return JsonResults.Invalid(validation.Field);
        }

        var now = clock.GetUtcNow().UtcDateTime;
        var email = input.Email.ToLowerInvariant();

        var row = new ContactSubmission
        {
            CreatedAt = now,
            Name = input.Name,
            Email = email,
            Company = input.Company,
            Phone = input.Phone,
            Service = input.Service,
            Message = input.Message,
            DedupeKey = DedupeKey.Compute(email, input.Message, now, opts.DedupeWindowSeconds),
            LegalHold = false,
            PurgeAfter = DateOnly.FromDateTime(now).AddMonths(retentionOptions.Value.Months),
            NotifyState = NotifyState.Pending,
            NotifyAttempts = 0,
            NotifyNextAttemptAt = now,
        };

        db.ContactSubmissions.Add(row);

        try
        {
            // Deliberately not ct. The minimal-API CancellationToken is
            // HttpContext.RequestAborted, and by this point the whole submission
            // is in memory: the visitor closing the tab or losing the connection
            // between the last body byte and the response must not discard a
            // message the server already has. DefaultCommandTimeout bounds it.
            await db.SaveChangesAsync(CancellationToken.None);
            return JsonResults.Ok();
        }
        catch (DbUpdateException ex) when (ex.InnerException is MySqlException { Number: DuplicateKeyErrorNumber })
        {
            db.Entry(row).State = EntityState.Detached;
            log.LogInformation("contact.duplicate");
            return JsonResults.Ok();
        }
        catch (Exception ex)
        {
            db.Entry(row).State = EntityState.Detached;

            // The whole justification for this backend is not losing inquiries,
            // and the client has no retry. One inline attempt, hard-capped, so a
            // MySQL restart does not silently lose a lead.
            try
            {
                // Not linked to ct either: if ct is what killed the write, a
                // linked token would already be cancelled and this last chance
                // to keep the message would throw before touching the socket.
                using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                await mail.SendAsync(NotificationMessage.Build(row, opts), timeout.Token);
                log.LogWarning(ex, "contact.db_bypass");
                return JsonResults.Ok();
            }
            catch (Exception mailEx)
            {
                log.LogError(mailEx, "contact.store_failed");
                return JsonResults.Unavailable();
            }
        }
    }
}
