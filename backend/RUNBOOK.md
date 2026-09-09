# Kestridge AI API - runbook

**Deploying to a Linux VPS? Use [DEPLOY-LINUX.md](DEPLOY-LINUX.md).** It has the
ordered command list, the scripts in `ops/linux/`, and the failure modes. This
file covers the Windows service deployment; the two differ in more than paths
(systemd instead of `sc.exe`, journald instead of the Event Log, file mode
instead of ACLs, and `sql_mode` is left at the MySQL default on Linux).

Everything here assumes the API runs as the Windows service `KestridgeApi` on
the same host as MySQL, bound to `http://127.0.0.1:5199`, with a reverse proxy
terminating TLS for `api.kestridge.com`.

**Record here which proxy is in use once it is chosen:** IIS with ARR, nginx, or
Caddy. Whoever installs it writes the config path and the restart command in
this section.

```
Reverse proxy:   TO BE RECORDED
Config path:     TO BE RECORDED
Restart command: TO BE RECORDED
Last restore drill: never
```

## Service control

```powershell
Get-Service KestridgeApi
Start-Service KestridgeApi
Stop-Service KestridgeApi
Restart-Service KestridgeApi
```

Logs go to the Windows Event Log (source `KestridgeApi`, application log), since
a Windows service has no console.

```powershell
Get-EventLog -LogName Application -Source KestridgeApi -Newest 50
```

## Is it up

```bash
curl -s http://127.0.0.1:5199/api/health
curl -s https://api.kestridge.com/api/health
```

`{"status":"ok"}` means the process is running and MySQL answers.
`{"status":"degraded"}` means MySQL does not. The endpoint is exempt from rate
limiting, so an uptime monitor can poll it freely. Point UptimeRobot at the
public URL.

## Smoke test the real endpoint

```bash
curl -i -H "Origin: https://kestridge.com" \
  -F name="Runbook Test" -F email="you@example.com" -F company="" -F phone="" \
  -F service=general -F message="Runbook smoke test, please ignore." -F _gotcha="" \
  https://api.kestridge.com/api/contact
```

Expect `200` and `{"ok":true}`. Then:

```bash
# 403 by design: browsers always send Origin, scripts often do not.
curl -i -F name=x -F email=x@y.z -F service=ai -F message=hello \
  https://api.kestridge.com/api/contact

# 200 with nothing stored: the honeypot.
curl -i -H "Origin: https://kestridge.com" \
  -F name="Bot" -F email="bot@example.com" -F service=ai \
  -F message="spam spam spam" -F _gotcha="filled" \
  https://api.kestridge.com/api/contact
```

Always pass a real `Origin`. Bare curl reproduces neither the browser's request
nor its CORS behaviour.

## What is happening right now

```powershell
mysql.exe -u kestridge_ops -p kestridge < ops/status.sql
```

Reading the output:

- **submissions by notify state.** `sent` is the normal state. `pending` rows
  older than a few minutes mean SMTP is failing. Any `failed` row needs a human.
- **notifications still waiting.** `notify_next_attempt_at` in the future is the
  backoff working, not a fault.
- **dead letters.** See the next section.
- **retention.** `due_for_purge` should be near zero after 03:00 UTC each day.

## Notifications stuck

A notification reaches `failed` two ways: seven transient failures over about
41 hours, or one permanent rejection (typically a 5xx from the recipient server).

`EventId 5001` (`notify.dead_letter`) is logged at Error level, at most hourly,
whenever any failed row exists. Watch for it.

**No submission is ever lost by a failed notification.** The row is in
`contact_submissions` and is readable with `ops/status.sql`.

Fix the cause first, then re-arm. Re-arming before the cause is fixed just
bounces again and gets the sending address throttled by the provider, which is
a slower problem to recover from than the one you started with.

```powershell
mysql.exe -u kestridge_ops -p kestridge < ops/rearm-notifications.sql
```

Uncomment the UPDATE in that file to actually re-arm. The sweep picks the rows
up within one tick, 30 seconds. Re-arming before the mailbox exists just bounces
them again and gets the sending IP throttled.

## Backups

```powershell
powershell -ExecutionPolicy Bypass -File ops/backup.ps1 -OffsitePath <path>
```

Schedule nightly with Task Scheduler, running as an account that can read
`KESTRIDGE_BACKUP_PASSWORD`. Local retention is 30 days; the script also copies
each archive offsite and refuses to run without an offsite target.

The dump contains every inquiry body. The output directory is personal data, and
its retention window has to match what `DSR-PROCESS.md` tells requesters.

Run the drill monthly and update the date at the top of this file:

```powershell
powershell -ExecutionPolicy Bypass -File ops/restore-drill.ps1
```

A backup that has never been restored is a hypothesis.

## Deploying a change

```powershell
cd backend
dotnet test
dotnet dotnet-ef migrations script --idempotent --project src/Kestridge.Api -o ops/migrate.sql
mysql.exe -u kestridge_migrator -p kestridge < ops/migrate.sql
powershell -ExecutionPolicy Bypass -File ops/install-service.ps1
curl -s http://127.0.0.1:5199/api/health
```

`install-service.ps1` re-publishes, re-registers and restarts. It prompts for the
secrets again and writes them to `appsettings.Production.json` inside the install
path, ACLed to Administrators and SYSTEM only. They are deliberately not machine
environment variables: that registry key is readable by `BUILTIN\Users` and by
`ALL APPLICATION PACKAGES`, so any unprivileged local account could read the
database credential, the SMTP password and the DSR pepper. Only
`ASPNETCORE_ENVIRONMENT` and `ASPNETCORE_URLS` live in the environment.

Verify after every install:

```powershell
icacls C:\Kestridge\api\appsettings.Production.json
```

## Locked out of the admin panel

Five failed sign-ins lock an account for 15 minutes. The lock does not escalate,
deliberately: anyone who knows a username could otherwise keep an operator
locked out permanently. To clear one early, as `kestridge_migrator`:

```bash
mysql -u kestridge_migrator -p kestridge < ops/admin-unlock.sql
```

The application credential cannot do this. It holds `UPDATE` on five named
columns of `admin_accounts` and nothing else, so a compromised web process can
never rewrite a password hash, swap a TOTP secret or re-enable a disabled
account. That is the boundary; do not widen it to fix a lockout.

A lost authenticator is not a lockout, it is a re-enrolment: run
`Kestridge.Api --hash-password` again for that user and replace the row.

## Website content

The panel writes to the six `site_*` tables and the site reads them through
`GET /api/content`. Two things follow that are not obvious.

**A content change rebuilds nothing.** `scripts/vercel-ignore.sh` sees no
commit, because there is no commit. The page updates through ISR within five
minutes, or immediately if the revalidate hook is configured. This is correct
and intended, and it is why the panel never claims a build ran.

**Quarterly: refresh the compiled fallback.** `src/data/*.ts` is what the site
renders whenever the API is unreachable. It does not update itself, so it drifts
from live content by exactly as much as gets edited.

```bash
curl -s https://api.kestridge.com/api/content
```

Copy the arrays back into `src/data/faq.ts`, `team.ts`, `companies.ts` and
`services.ts` and commit. Without this, a fallback that fires two years from now
shows two-year-old copy, and nothing anywhere will warn you.

## Grant drift

Monthly, as root:

```powershell
mysql.exe -u root -p kestridge < ops/03-verify-grants.sql
```

Compare against the matrix at the top of that file. This is what catches a grant
widened to `ALL` during a debugging session and never narrowed back.

## Known gaps, stated plainly

1. **A lead delivered by the MySQL-down fallback exists only in the inbox.** If
   the database write fails and the inline mail attempt succeeds, the endpoint
   answers `200` and logs `contact.db_bypass` at Warning level, but no row is
   stored. That message will not appear in a DSR export or a backup. Search the
   Event Log for `contact.db_bypass` when reconciling.
2. **Zoho Mail is a subprocessor with access to every inquiry body.**
   Register it and sign the DPA. The database is self-hosted, so there is no
   database subprocessor. Its US data centre matches where the company and the
   database already are.
3. **Notification mail authenticates as a person, not as a service.**
   `Kestridge:Smtp:User` is `chingiz@kestridge.com` with a Zoho app password,
   because `info@kestridge.com` is an alias on that account and an alias cannot
   log in. Two consequences: revoking that person's app password stops every
   notification, and the alias is what the message is From, so it must stay an
   alias of whichever account the credential belongs to. Rotating the app
   password is a one-line change in `appsettings.Production.json` plus a
   restart.

## If spam starts arriving

In order of what actually stops bots, everything below the line is already
built:

1. Origin allowlist (403 without a known `Origin`).
2. Per-IP rate limit, 5 per 10 minutes, IPv6 masked to /64.
3. Global backstop, 200 per hour, which bounds a distributed run. It is chained
   after the per-client limiter, so a request already rejected per IP never
   burns a global permit.
4. Honeypot `_gotcha`.
5. Duplicate suppression on a 10-minute window.

If the honeypot counter (`contact.honeypot` in the Event Log) and 429 rejections
show real spam still reaching the inbox, add Cloudflare Turnstile. The shape,
written down now so it is a two-hour job later:

- Render the widget in `Contact.tsx` and put the token in a **hidden form
  field** named `cf-turnstile-response`. **Never a request header**: a custom
  header creates a CORS preflight that this client never sends, which breaks the
  form outright.
- Verify server-side against
  `https://challenges.cloudflare.com/turnstile/v0/siteverify` with a 3-second
  timeout, reading `Kestridge:Turnstile:SecretKey`.
- **On verification timeout, accept the submission.** A Cloudflare outage must
  not take the contact form down.
- Confirm in devtools that the chosen widget mode sets no cookie, or
  `CookieConsent.tsx` has to be reactivated and the Privacy Policy amended.
- Add Cloudflare to the subprocessor register with a DPA before go-live.

## Go-live checklist

```
DNS AND MAIL
[ ] api.kestridge.com record added in the Vercel-hosted zone
[ ] TLS certificate issued, reverse proxy serving it, recorded at the top of this file
[ ] SMTP provider chosen; MX, SPF, DKIM published; DMARC p=none with a reporting address
[ ] info@kestridge.com exists and is monitored                      BLOCKER
[ ] SMTP provider in the subprocessor register with a signed DPA    BLOCKER

DATABASE
[ ] ops/01-provision.sql run; ops/03-verify-grants.sql clean
[ ] ops/02-hardening.cnf merged into my.ini; MySQL restarted; binlog window confirmed
[ ] ops/migrate.sql applied as kestridge_migrator
[ ] dotnet test passes with KESTRIDGE_TEST_CONNECTION set (no skips)

APPLICATION
[ ] Service installed, automatic start, restart-on-failure, depends on MySQL80
[ ] ASPNETCORE_ENVIRONMENT=Production
[ ] Kestridge__Cors__AllowVercelPreviews=false
[ ] Kestridge__Contact__RequireOrigin=true
[ ] Every secret in appsettings.Production.json, ACLed; none in the repo; none NEXT_PUBLIC_
[ ] icacls on that file shows Administrators and SYSTEM only
[ ] GET /api/health returns 200 {"status":"ok"}

FRONTEND
[ ] NEXT_PUBLIC_FORM_ENDPOINT set in Vercel and a REBUILD triggered
[ ] git diff is empty for src/components/sections/Contact.tsx

VERIFY FROM https://kestridge.com IN A REAL BROWSER
[ ] Submit the form; the success screen appears
[ ] The notification email arrives
[ ] The row is in contact_submissions with notify_state='sent'
[ ] Network tab: zero Set-Cookie on the /api/contact response
[ ] Network tab: no OPTIONS preflight was sent
[ ] curl with the honeypot filled: 200 and zero rows
[ ] curl with no Origin header: 403

OPS
[ ] backup.ps1 scheduled nightly; the first dump verified
[ ] restore-drill.ps1 run once; date recorded at the top of this file
[ ] TAMAMLANACAQ-ISLER.txt section 5 cookie item ticked
[ ] Privacy Policy retention and sharing edits published, UPDATED date bumped
```
