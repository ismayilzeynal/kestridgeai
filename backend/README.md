# Kestridge AI - backend

ASP.NET Core 10 (`net10.0`) + EF Core 9 + Pomelo 9.0.0 + MySQL 8. One process,
two endpoints, three tables. It exists to receive the contact form on
`kestridge.com`, keep every inquiry, and tell the team about it.

`Contact.tsx` in the Next.js app is **not** modified by any of this. The only
frontend change is one Vercel environment variable.

## What it does

1. Accepts the existing contact form POST, unchanged, over CORS.
2. Stores each accepted submission in one MySQL table.
3. Emails a plain-text notification to the team, out of the request path, with
   durable retry driven off the submission row.
4. Deletes submissions after 24 months, skipping legal holds, and records each
   purge run as evidence.
5. Answers a health probe.

## What it deliberately does not do

Each of these was designed and rejected. The trigger named is the only thing
that reopens it.

| Not built | Why | Revisit when |
| --- | --- | --- |
| Content CMS tables for services, team, FAQ, companies | The four people who edit this copy are the four people who deploy. `src/data/services.ts` holds `icon: LucideIcon`, a React component reference that is not serializable and cannot cross the server/client boundary; `Hero.tsx` and `Services.tsx` are `"use client"` and would need splitting; `Services.tsx:252` is `grid-cols-4`, so "exactly four delivery steps" is a layout invariant no DDL can express; `nav[].href` must match a JSX `id` or the link silently no-ops through `scroll.ts`. Every table would also need a code fallback, or an unreachable database ships a blank marketing page. | A non-developer needs weekly copy edits without a deploy. Then add `faqs` **only**: one table, no icon, no asset, no layout invariant, no client boundary, and it fixes `FAQPage` JSON-LD drift as a side effect. Keep a literal `DEFAULT_FAQS` fallback in code. Never start with `services`. |
| Any `/api/content/*` read endpoint | Same reason, plus it would make `next build` on Vercel depend on this host being reachable. | Same as above. |
| An admin HTTP API and UI | Volume is a handful of submissions a week and one or two rights requests a year. `ops/status.sql` and the DSR scripts cover every published obligation. | Not at this volume. |
| Cookies of any kind | A cookie forces `CookieConsent.tsx` back on and a rewrite of Privacy Policy section 3. The endpoint is anonymous, so it needs none. | Never. |
| A separate outbox table | One message kind, one process. State on the row means no second copy of the message body to scrub and no orphan reaper. A DSR delete removes the pending notification for free. | A second message kind or a second app instance exists. |
| `ip_address`, `user_agent`, `referer` columns | The Privacy Policy discloses automatic technical data only as visit telemetry used in aggregate. On the same row as a name and an email it becomes identified personal data serving a purpose section 4 does not list. This is one line of code and the default in every tutorial, which is why `SchemaTests` asserts the columns do not exist. | Never, without amending two sections of the published policy first. |
| Storing honeypot hits as rows | Personal data collected for no disclosed purpose. A log counter answers every question the row would. | Never. |
| CAPTCHA / Turnstile | A new subprocessor needing a DPA, a frontend change, and possibly a cookie. Observed spam is zero, because the form has never had a live endpoint. | The honeypot counter and rate-limit rejections show real spam reaching the inbox. The integration shape is written down in RUNBOOK.md. |
| Autoresponder to the visitor | It turns the form into a relay: 500 submissions carrying a victim's address means the victim gets 500 mails from kestridge.com and the domain gets blocklisted. | Never. If it is ever forced, per-email rate limiting stops being optional. |

## Layout

```
backend/
  global.json                 SDK pinned to 9.0.x
  Directory.Build.props       net10.0, nullable, warnings as errors
  Directory.Packages.props    every package version, centrally
  Kestridge.sln
  src/Kestridge.Api/
    Program.cs                composition root only
    Options/                  one POCO per config section, validated on start
    Data/                     entities, DbContext, UTC interceptor, migrations
    Contact/                  the endpoint, form reader, validator, origin guard, dedupe
    Email/                    message builder, MailKit sender, failure classifier
    Maintenance/              the one BackgroundService: notify sweep + retention purge
    Health/                   the health endpoint
    Infrastructure/           rate limiting, partition key, JSON bodies
  tests/Kestridge.Api.Tests/
  ops/                        SQL, PowerShell, and the migration script
                              (dsr-hex.ps1 makes an address safe to paste)
```

## Prerequisites

- .NET SDK 10.0.x, pinned in `global.json`. The runtime is .NET 10 LTS,
  supported to 2028-11-14; .NET 9 goes out of support 2026-11-10 and is not in
  the Ubuntu 24.04 archive at all. EF Core and Pomelo stay on 9.0.x, which is
  the newest stable Pomelo and runs fine on the .NET 10 runtime.
- MySQL 8.0.x on `127.0.0.1:3306`, and the root password.
- An SMTP account, or a local catcher such as Papercut for development.

## First run

```powershell
cd backend
dotnet tool restore
powershell -ExecutionPolicy Bypass -File ops/setup-dev-db.ps1
```

The script prompts for the MySQL root password, generates a password for each of
the five service accounts, provisions databases and grants, applies
`ops/migrate.sql`, writes the app connection string and the DSR pepper into
`dotnet user-secrets`, and prints the remaining passwords once.

Then set the development secrets it does not know:

```powershell
dotnet user-secrets set "Kestridge:Smtp:Host" "localhost" --project src/Kestridge.Api
dotnet user-secrets set "Kestridge:Contact:ToAddress" "you@example.com" --project src/Kestridge.Api
dotnet user-secrets set "Kestridge:Contact:FromAddress" "you@example.com" --project src/Kestridge.Api
```

## Build, test, run

```powershell
dotnet build
dotnet test
dotnet run --project src/Kestridge.Api
```

Tests that need MySQL **skip** when it is not provisioned, with the reason
printed. To run them, export the test connection string the setup script printed:

```powershell
$env:KESTRIDGE_TEST_CONNECTION = "Server=127.0.0.1;Port=3306;Database=kestridge_test;User ID=kestridge_test;Password=...;SslMode=Preferred;AllowPublicKeyRetrieval=True;DateTimeKind=Utc;DefaultCommandTimeout=15"
dotnet test
```

Under CI the fixture throws instead of skipping, so a green CI build cannot be
vacuous. Set `CI=1` there.

Smoke test the running process. The `Origin` header is required by design:

```bash
curl -i -H "Origin: https://kestridge.com" \
  -F name="Jane Doe" -F email="jane@company.com" -F company="" -F phone="" \
  -F service=ai -F message="Testing the endpoint." -F _gotcha="" \
  http://localhost:5199/api/contact
```

Without `Origin` it answers `403`. That is the control, not a bug.

## Migrations

```powershell
dotnet dotnet-ef migrations add <Name> --project src/Kestridge.Api --output-dir Data/Migrations
dotnet dotnet-ef migrations script --idempotent --project src/Kestridge.Api -o ops/migrate.sql
```

Commit `ops/migrate.sql` with every release and apply it at deploy time as the
migrator credential:

```powershell
mysql.exe -u kestridge_migrator -p kestridge < ops/migrate.sql
```

`Database.Migrate()` is never called at startup, and it would throw if it were:
the application credential holds DML only. Granting it DDL to "fix" that breaks
the access model. MySQL DDL is also not transactional, so a failed multi-statement
migration leaves partial state; do not assume rollback.

## Configuration

Environment variables use `__` where the config path uses `:`.

| Key | Secret | Development | Production |
| --- | --- | --- | --- |
| `ConnectionStrings:Default` | yes | user-secrets | `appsettings.Production.json`, ACLed |
| `Kestridge:Cors:AllowVercelPreviews` | no | `true` | `false` |
| `Kestridge:Contact:RequireOrigin` | no | `false` | `true` |
| `Kestridge:Contact:AllowedServices` | no | `ai,analytics,automation,security,general` | same |
| `Kestridge:Contact:MaxMessageLength` | no | `5000` | `5000` |
| `Kestridge:Contact:DedupeWindowSeconds` | no | `600` | `600` |
| `Kestridge:Contact:ToAddress` | no | a mailbox you can read | `info@kestridge.com` |
| `Kestridge:Contact:FromAddress` | no | same | `no-reply@kestridge.com` |
| `Kestridge:Smtp:Host` / `Port` / `UseStartTls` / `User` | no | `localhost` / `25` / `false` / empty | provider values |
| `Kestridge:Smtp:Password` | **yes** | user-secrets | `appsettings.Production.json`, ACLed |
| `Kestridge:RateLimit:PermitsPerWindow` | no | `100` | `5` |
| `Kestridge:RateLimit:GlobalPerHour` | no | `100000` | `200` |
| `Kestridge:Notify:MaxAttempts` / `SweepSeconds` / `BatchSize` | no | `7` / `30` / `20` | same |
| `Kestridge:Retention:Months` / `RunHourUtc` / `BatchSize` | no | `24` / `3` / `500` | same |
| `Kestridge:Dsr:EmailHashPepper` | **yes** | user-secrets | `appsettings.Production.json`, ACLed |

Every section is bound with `ValidateDataAnnotations().ValidateOnStart()`, so a
missing SMTP host or `ToAddress` crashes the process at boot rather than failing
on the first inquiry six weeks later.

**No secret may ever go into a `NEXT_PUBLIC_*` variable.** Next inlines those
into the client bundle at build time. `grep -r NEXT_PUBLIC .next/static` after a
build proves it.

### Connection strings

Development and production on this host:

```
Server=127.0.0.1;Port=3306;Database=kestridge;User ID=kestridge_app;Password=***;
SslMode=Preferred;AllowPublicKeyRetrieval=True;DateTimeKind=Utc;
DefaultCommandTimeout=15;Pooling=true;MaximumPoolSize=20
```

`SslMode=Preferred` is correct here **only** because MySQL is bound to
`127.0.0.1` on the same machine, and `AllowPublicKeyRetrieval=True` is only
acceptable on that loopback: on any other network it is a man-in-the-middle
vector. This is a decision, not an oversight. If MySQL ever moves to another
host, the string becomes:

```
Server=db.internal;...;SslMode=VerifyFull;AllowPublicKeyRetrieval=False;DateTimeKind=Utc;...
```

`DateTimeKind=Utc` is mandatory everywhere. Without it MySqlConnector returns
`DateTime` with `Kind = Unspecified`, and one later `ToUniversalTime()` shifts
every value by the local offset. On a UTC+4 machine that is a four-hour error in
the retention cutoff that does not reproduce on a UTC CI runner.

## Frontend wiring

In Vercel set:

```
NEXT_PUBLIC_FORM_ENDPOINT = https://api.kestridge.com/api/contact
```

exactly, no trailing slash, then **trigger a rebuild**. `NEXT_PUBLIC_*` is
inlined at build time, so redeploying the same artifact changes nothing.

## The wire contract

Frozen by `src/components/sections/Contact.tsx`. Seven multipart parts, always
present, in DOM order, with blank optional fields arriving as empty-string parts:

`name`, `email`, `company`, `phone`, `service`, `message`, `_gotcha`

The client reads `Response.ok` and nothing else: no `.json()`, no timeout, no
retry. Both request headers are CORS-safelisted, so the POST is a **simple
request and no preflight is sent**.

| Status | Body | When |
| --- | --- | --- |
| `200` | `{"ok":true}` | stored, or honeypot tripped, or duplicate suppressed, or delivered by the MySQL-down fallback |
| `400` | `{"ok":false,"error":"invalid","field":"..."}` | validation failed, or a file part was present |
| `403` | `{"ok":false,"error":"origin"}` | `Origin` missing or not allowlisted |
| `413` | `{"ok":false,"error":"too_large"}` | over the body or value cap |
| `415` | `{"ok":false,"error":"content_type"}` | not a form content type |
| `429` | `{"ok":false,"error":"rate"}` plus `Retry-After: 600` | rate limited |
| `503` | `{"ok":false,"error":"unavailable"}` | the write failed **and** the inline mail fallback also failed |

Three rules behind that table:

- **200 on business outcomes is absolute.** A honeypot trip returning 4xx would
  teach the bot which field is the trap and show a real visitor a false failure.
- **503 when nothing was stored and nothing was mailed is equally absolute.** The
  message is genuinely lost, and the visitor has to be told so they use the
  fallback address.
- **Never a 3xx.** `fetch` follows redirects, and a 301/302/303 turns the POST
  into a GET and drops the body.

`GET /api/health` answers `200 {"status":"ok"}` or `503 {"status":"degraded"}`.
No counts, no version, no host name, no exception text.

## Notes for whoever changes this next

- The handler binds `HttpRequest` and calls `ReadFormAsync` itself. Do **not**
  replace that with a form-bound parameter: any `[FromForm]`, `IFormCollection`
  or `IFormFile` parameter attaches antiforgery metadata, `WebApplication`
  auto-inserts `UseAntiforgery()`, and every submission returns 400 with an empty
  body. An unannotated complex type is worse: it infers JSON and returns 415.
  `.DisableAntiforgery()` is on the endpoint as a statement of intent. Antiforgery
  is not merely unnecessary here, it is impossible: it needs a cookie, and this
  design forbids one. The Origin allowlist is the stronger control anyway.
- `UseCors` runs before `UseRateLimiter`, or a 429 goes out with no
  `Access-Control-Allow-Origin` and the browser reports an opaque network
  failure. `UseExceptionHandler` is registered before `UseCors` for the same
  reason: it clears the response, but CORS still runs on the way out and
  re-applies the header, so a 500 keeps it. `CorsTests` pins all three cases
  (400, 429, 500) because a response without that header reaches the browser as
  an opaque network failure for a submission the server may already have stored,
  and the visitor resubmits.
- The rate limiter is one chained limiter, per-client first, not an endpoint
  policy plus a separate global one. As two independent limiters the middleware
  takes the global lease first and merely disposes it when the per-client
  limiter refuses, and a fixed window does not refund a disposed lease: one bot
  draining 200 permits in seconds would 429 every real visitor for the rest of
  the hour, with `/api/health` still green so nobody is paged.
- The Vercel preview regex is anchored at both ends. Unanchored it also matches
  `https://kestridgeai.vercel.app.attacker.com`. Even anchored, any Vercel user
  can create a project named `kestridgeai-something`, which is why previews are
  off in production.
- Never call `EnableSensitiveDataLogging()`. It logs parameter values, which puts
  the message body into the console and the Event Log: a second undisclosed copy
  outside the retention job's reach.
- `ServerVersion.AutoDetect` is not used. It opens a blocking connection during
  DI registration, so the process would fail to start whenever MySQL is not up
  yet, and `dotnet ef` would need a live database.
- The `email` column is `utf8mb4_0900_as_cs`. Under the schema default
  (`_ai_ci`), `WHERE email = 'jose@x.com'` also matches an accented spelling,
  which is a different mailbox belonging to a different person: a DSR delete
  would erase a third party's data. Case is handled by lowercasing in the
  application instead.
- No em dash, en dash, figure dash or horizontal bar in any file, and no emoji.
  `StyleTests` enforces it across `backend/**`.

See `RUNBOOK.md` for operations and `DSR-PROCESS.md` for rights requests.
