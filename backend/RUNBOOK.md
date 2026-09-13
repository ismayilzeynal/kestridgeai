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
mysql.exe -u root -p < ops/04-table-grants.sql
powershell -ExecutionPolicy Bypass -File ops/install-service.ps1
curl -s http://127.0.0.1:5199/api/health
```

The grants go in after the migration and before the install, every time. They
are idempotent, and a release that adds a table without its grants fails every
request that touches it. Admin user management is such a release: without its
grants nobody can sign in to the panel, while `/api/health` stays green.

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

## Admin accounts

Every account has the same access to the whole panel, the Users tab included,
so anyone who can sign in can create, reset and disable the others. Nobody can
reset or disable their own account from the panel.

There is no password change and no "forgot password" flow, on purpose. A
person chooses their password once, at their first sign-in, and the
application credential holds no `UPDATE` on `password_hash`, so a compromised
web process can never rewrite one. The cost is that changing a password after
that is a terminal and a SQL script. What else the database does and does not
guarantee is under [The account boundary](#the-account-boundary).

**Create.** Users tab, "Create a user": a username, a display name, a temporary
password, and the current code from your own authenticator. The code is asked
for so that a session left open on a desk cannot create an account on its own.
Neither the username nor the display name can be changed later without SQL.
The account then waits under "Waiting for first sign-in" for 72 hours
(`Kestridge:Admin:EnrollHours`), and a notice mail goes to the team address.

Give the person the temporary password directly, not by email. It stops
working at their first sign-in, but until then whoever knows it can complete
that sign-in, and that includes you.

**First sign-in.** The person opens `https://api.kestridge.com/admin/`, types
the username and the temporary password, and presses Continue. Instead of the
code step they get "Set up your authenticator": a QR code, the same key as text
for typing in by hand, two fields for a password of their own, and a code
field. They scan, choose a password (12 to 128 characters, not the temporary
one), type the code the app now shows, and press Finish setup. That signs them
in. From then on it is the normal password and code.

Two things trip people up here. The setup screen is good for 15 minutes
(`Kestridge:Admin:EnrollTokenMinutes`); after that Finish setup says "Setup
timed out" and they sign in again. And every Continue issues a new key, so an
entry scanned from an earlier setup screen never produces a working code:
delete it from the app and scan the one on screen.

If a person who has never signed in gets the code step instead of the setup
screen, one of four things is true.

1. The username or the password was mistyped. Capitals in the username do not
   matter, but a letter outside plain ASCII, such as the Azerbaijani dotted
   capital I or schwa, never matches any username and is answered like an
   unknown one.
2. The invitation is gone. An expired one shows as "Expired": delete it and
   create it again. One that was deleted is not listed at all, and disabling
   the operator who created it deletes it too: create it again.
3. The invitation is locked. Five wrong passwords within an hour lock it for 15
   minutes, and until then even the right password gets the code step. The row
   under "Waiting for first sign-in" shows "locked", with the time it ends when
   you point at it. Wait it out, or clear it early as described under
   [Locked out of the admin panel](#locked-out-of-the-admin-panel). If the
   person did not type five wrong passwords, somebody else did: delete the
   invitation and create it again with a new temporary password instead.
4. Somebody else already completed the first sign-in: the username has moved
   to Accounts with a last sign-in time. Disable it and create the person a new
   account under a different username, because a disabled account keeps its
   name.

To check the second and third from the server:

```bash
sudo mysql kestridge -e "SELECT username, created_by, expires_at, failed_attempts, locked_until, UTC_TIMESTAMP(6) AS now_utc FROM admin_enrollments WHERE account_id IS NULL;"
```

Times are stored in UTC, hence `now_utc`. A username missing from the result
has no invitation, and `locked_until` later than `now_utc` is a lock.

**Reset an authenticator** (a lost or replaced phone). "Reset authenticator" on
that person's row, confirmed with your own code. Their sessions end at once,
their lockout counters clear, the row shows "authenticator reset", and a notice
mail goes to the team. At their next sign-in, within 72 hours, they use their
existing password and get the same setup screen without the password fields.
A reset nobody completes in time leaves the row at "no authenticator", which
cannot sign in at all; reset it again.

A reset that collides twice in a row with another change to the same account,
such as other operators resetting it at the same moment, says "The
authenticator was not reset." Nothing changed and no mail went out, and the
journal gets `admin.reset_conflict account=<id>`. Reload the list before
clicking again: the other reset may already be there.

Your own authenticator cannot be reset from the panel. Losing it is exactly
when you cannot sign in to click the button, so it is always somebody else's
click. When there is nobody else, use the break-glass path below.

**Disable.** "Disable" on that person's row. This is the offboarding step and
it is not optional: the panel reads every inquiry the company has ever
received, so an account that outlives the person is the largest standing
exposure here. It takes effect on the next request, not the next login: the
token filter checks on every call, and the disable also deletes the person's
sessions and any pending reset. No code is asked for, because disabling only
ever takes access away and it is the action to be fast at when an account is
being misused.

The disable also deletes every invitation that person created that is still
under "Waiting for first sign-in". They chose its temporary password and may
still know it, and completing it would give them a new account that disabling
this one does not reach. What it cannot find is an account they created that
has already finished its first sign-in, because nothing in the database
records who created an account once its invitation is gone. So offboarding
has a second half: go through the "account created" mails that name the
person under "Created by", and disable any of those accounts that nobody else
can vouch for.

`ops/admin-disable.sql` does the same as `kestridge_migrator`, invitations
included, for when the panel is down or nobody who is left can sign in.

**Re-enable.** SQL only, as `kestridge_migrator`, and it has two halves. A
panel disable is a row in `admin_disables`, which the application can add and
never remove; a SQL disable is `admin_accounts.disabled = 1`. Either one alone
keeps the account out, so clear both:

```sql
UPDATE admin_accounts SET disabled = 0 WHERE username = 'faig';
DELETE FROM admin_disables WHERE account_id = (SELECT id FROM admin_accounts WHERE username = 'faig');
```

The person signs in with the password and authenticator they had. If the
authenticator is what they lost, reset it from the panel after re-enabling:
the panel refuses to reset a disabled account.

**Delete a user who never signed in.** "Delete" under "Waiting for first
sign-in". The temporary password stops working and the username is free again.
An expired invitation stays listed until someone deletes it, or creates the
same username again, which replaces it. Only invitations can be deleted. An
account is never deleted from the panel, and `kestridge_app` holds no `DELETE`
on `admin_accounts` at all: `handled_by` on `contact_submissions` and on
`dsr_log` carries the display name of whoever acted, and a request answered
last year has to stay attributable to a real person.

**The notice mail.** Creating a user and resetting an authenticator each send
one plain text mail to `Kestridge:Contact:ToAddress`, with the subject
`Kestridge admin: account created for <username>` or
`Kestridge admin: authenticator reset for <username>`. It names the username,
the display name, who did it, when, and the deadline for the first sign-in,
and never a password, key, token or code. Its job is that nobody is handed a
way into the panel without the rest of the team hearing about it.

Each mail ends with what to do if nobody expected it, and then to follow this
file. An "account created" mail says to delete the pending user in the panel,
under "Waiting for first sign-in". If it is no longer there and the username
is under Accounts, the first sign-in has already happened: disable the account
instead. An "authenticator reset" mail says to disable the account, since
whoever reset it may be about to enrol a phone of their own with a password
they learned somewhere. Then find out why.

It goes out through the same `Kestridge:Smtp` credential as the submission
notifications, so known gap 3 below applies to it. Unlike a notification it is
sent once, inline, after the change is committed, and never retried, because
there is no row to retry from. When it fails the change still stands, the
operator who clicked sees "The notice mail to the team address was not sent",
and the journal gets `admin.notice_failed`. Tell the team yourself. Disabling,
deleting an invitation and completing a first sign-in send no mail.

**Break-glass: the CLI.** The panel cannot make the first account, because
nobody exists yet to sign in and create one, and it cannot help when nobody
who is left can sign in. Both cases go through the command line on the server.
The person whose account it is should be the one at the keyboard, because the
CLI reads their password and prints their TOTP secret. Someone else running it
knows both, and the second factor stops being a second factor.

```bash
cd /srv/kestridge-api && dotnet Kestridge.Api.dll --hash-password --username faig --display-name "Faig Garayev"
```

Note the `dotnet`: nothing in the install directory carries an execute bit. It
prints an `INSERT` and an `otpauth://` URI, and never opens MySQL. Scan the URI
into an authenticator **before** clearing the screen; it is shown once. Then
apply the insert without putting it through shell history:

```bash
umask 077 && cat > /root/admin-account.sql   # paste, then Ctrl+D
sudo mysql kestridge < /root/admin-account.sql && shred -u /root/admin-account.sql
history -c && clear
```

A forgotten password, or a lost phone when nobody else can reset it, is the
same command followed by the matching `UPDATE` in `ops/admin-account.sql`
instead of the `INSERT`. Update the row in place; do not delete it and insert a
new one. The new row gets a new id, and a panel disable recorded against the
old id would silently stop applying.

**Unlock.** `ops/admin-unlock.sql`, below.

**"Sign in failed" straight after signing in or out** is almost always a reused
code, not a wrong password. `totp_last_step` records the step that was accepted
and anything at or below it is refused, so the six digits on screen are dead
the moment they are used once. Wait for them to change. The panel says so under
the code field; the server cannot say it in the error, because a message that
distinguished a reused code from a wrong password would confirm that the
username and password were right.

Continue does ask the server one question before the code step, through
`/api/admin/login/start`: are these the username and password of an account
waiting for its first sign-in or for a reset? The answer is byte for byte the
same, after one password hash check, for an unknown user (a username with a
letter outside ASCII included), a wrong password, the right password of an
account that is not waiting, a disabled or locked account, and an expired or
locked invitation. So the code step still appears whatever was typed, and a
failure still arrives only after the code. The one different answer, the setup
screen, needs the right password of an account that is waiting. It does
confirm that password, for an account that has no working second factor yet to
protect, and what it hands over is the setup that account needs anyway.

That makes a sign-in two requests, `/login/start` and `/login`, and each has
its own budget of 5 per 10 minutes per client address, shared by everyone
behind that address. Over the first, Continue says "Too many attempts from this
network". Over the second, the code step says "Sign in failed" like any other
failure, and the journal shows no `admin.login_failed` for it, because the
limiter refused the request before it reached the login handler. Finish setup,
`/login/enroll`, spends from the first budget, so a first sign-in costs two of
its five. The limiter matches these paths the way routing does, ignoring case
and one trailing slash, so respelling the URL does not get round a budget.

To tell a reused code from a real failure on the server:

```bash
sudo mysql kestridge -e "SELECT username, totp_last_step, last_login_at, failed_attempts, locked_until FROM admin_accounts;"
sudo mysql kestridge -e "SELECT username, account_id, expires_at, failed_attempts, locked_until FROM admin_enrollments;"
sudo journalctl -u kestridge-api --since "10 min ago" --no-pager | grep -i 'admin\.'
```

`admin.login_ok` followed by `admin.login_failed` within the same 30 second
window is a reused code and nothing is wrong. `admin.enroll_started` with no
`admin.enrolled` after it is a setup screen that was abandoned or timed out.
`admin.enroll_failed` is a wrong password against an account or invitation
that is waiting, and it counts towards that row's lockout. The panel still
moves on to the code step, so it is usually followed by `admin.login_failed`
for the same attempt. That one counts nothing: a person who is waiting has no
authenticator that `/login` could accept, and the attempt was already counted
once.

## Locked out of the admin panel

Five failed sign-ins within an hour lock an account for 15 minutes. A wrong
code given to "Create a user" or "Reset authenticator" counts as one against
the operator's own account, and the Users tab says so when that locks it. The
lock does not escalate, deliberately: anyone who knows a username could
otherwise keep an operator locked out permanently. Attempts sent at the same
moment are counted one by one in the database, so sending many at once does not
get more guesses in before the lock. To clear one early, as
`kestridge_migrator`:

```bash
mysql -h 127.0.0.1 -u kestridge_migrator -p kestridge < ops/admin-unlock.sql
```

A person waiting for their first sign-in has no account row yet, so their
counters are on the invitation in `admin_enrollments`, and the statement
`admin-unlock.sql` runs does not reach them. The invitation's row shows
"locked" under "Waiting for first sign-in". Wait out the 15 minutes, delete the
invitation in the panel and create it again, or run the commented
`admin_enrollments` statement in that file instead.

There is no unlock in the panel. The application credential could clear the
counters, since it writes them to count failures in the first place, so what
keeps this a SQL step is only that no endpoint does it. The boundary that
matters is described below; do not widen it to fix a lockout.

A lost authenticator is not a lockout, it is a reset: "Reset authenticator" in
the Users tab, clicked by someone else. With nobody else to click it, use the
CLI above.

## The account boundary

The grants in `ops/04-table-grants.sql` are the part of this that still holds
when the web process does not. For `kestridge_app` they guarantee:

- It can never rewrite an existing password hash. It holds no `UPDATE` on
  `password_hash`; a password is written once, by the `INSERT` that creates the
  account when a new user finishes their first sign-in.
- It can never rename an operator: no `UPDATE` on `username` or `display_name`.
- It can never re-enable a disabled account: no `UPDATE` on `disabled`, and
  only `SELECT` and `INSERT` on `admin_disables`, so the process that wrote a
  disable cannot take it back.
- It can never delete an account row: no `DELETE` on `admin_accounts`. What the
  panel deletes is an invitation in `admin_enrollments`, which is not an
  account.
- It holds nothing on the schema.

What it can do, and so what a compromised process can do too:

- Insert a new account into `admin_accounts`, with a password and a key of its
  own choosing. The code the panel asks for and the notice mail are checks in
  application code. A compromised process skips both, sends no mail, and the
  database accepts the row.
- Clear or set any account's `totp_secret`, which the reset cannot work
  without. Clearing it locks the owner out until someone resets it. Setting it
  gains nothing without the account's password, and that hash it cannot change.
- Write the lockout counters, `last_login_at` and `totp_last_step`, which
  sign-in needs. So it can clear a lockout, and it can also set one that lasts
  for years, or push `totp_last_step` into the future, which refuses every code
  for that account, at sign-in and at every step-up. `ops/admin-unlock.sql`
  clears such a lock. The step stays pushed until that authenticator is reset
  from the panel and set up again, or rotated with `ops/admin-account.sql`:
  both set it afresh for the new secret, and the panel reset clears the lock
  as well.
- Write any row in `admin_enrollments`, including an invitation of its own,
  with a temporary password it knows or a setup token and key it already
  holds, and an `expires_at` as far off as it likes. Completing one creates an
  account with no operator's code and no mail.
- Everything it could before: read every submission, see each password typed
  at sign-in, and write `admin_sessions`. A session row is a way into the panel
  as the account it names, with whatever expiry it was written with, so a row
  inserted while the process was compromised keeps working after it is clean,
  until somebody deletes it. A row naming an account id that does not exist
  yet comes alive when a new account is given that id.

So the grants do not stop a compromised process from reading what the panel
reads. What they decide is what it can leave behind once it has been cleaned
up, and everything it leaves stays until somebody removes it: an extra
account; a replaced authenticator on an account whose password it saw at
sign-in, which the owner notices the next time their own codes stop working;
session rows that still open the panel; invitations and setup tokens in
`admin_enrollments` that become accounts whenever they are completed; and a
`totp_last_step` or a lock pushed into the future. It can never leave a changed
password hash or an undone disable.

After a suspected compromise, and once the process is clean, treat every
operator's password and authenticator as known, and every session and every
waiting enrolment as planted. First end every session, yours included, as
`kestridge_migrator`, and then look at what is waiting and at the step and lock
of each account:

```bash
mysql -h 127.0.0.1 -u kestridge_migrator -p kestridge -e "DELETE FROM admin_sessions;"
sudo mysql kestridge -e "SELECT id, account_id, username, display_name, created_by, created_by_account_id, created_at, expires_at, token_expires_at FROM admin_enrollments;"
sudo mysql kestridge -e "SELECT username, totp_last_step, UNIX_TIMESTAMP() DIV 30 AS current_step, locked_until, UTC_TIMESTAMP(6) AS now_utc FROM admin_accounts;"
```

Delete, as `kestridge_migrator`, every `admin_enrollments` row that no "account
created" or "authenticator reset" mail accounts for
(`DELETE FROM admin_enrollments WHERE id = ...;`), or delete them all and
create the real invitations again. An account whose reset row goes shows "no
authenticator" and gets reset again below anyway. A `totp_last_step` more than
one above `current_step` was pushed there, because sign-in accepts a code at
most one step ahead (`Kestridge:Admin:TotpSkewSteps`, default 1). A
`locked_until` more than 15 minutes after `now_utc` was set by hand, because a
real lockout never lasts longer (`Kestridge:Admin:LockMinutes`, default 15).

Then new passwords through the CLI and SQL, then a reset of each authenticator
from the panel, which puts right the lock and, once its owner has set up
again, the step. An account whose step was pushed cannot sign in to click
anything: have somebody else reset it first, or rotate it with
`ops/admin-account.sql`. And list the accounts, matching each one to an
"account created" mail or to a CLI run somebody remembers:

```bash
sudo mysql kestridge -e "SELECT a.id, a.username, a.display_name, a.created_at, a.last_login_at, a.disabled, d.disabled_at FROM admin_accounts a LEFT JOIN admin_disables d ON d.account_id = a.id ORDER BY a.created_at;"
```

For a panel account `created_at` is when the first sign-in finished, which can
be up to 72 hours after its mail. An account with neither a mail nor a
remembered CLI run is the one to disable first and ask about second.

This is done with grants and two tables rather than MySQL triggers. The
idempotent `migrate.sql` wraps every statement in a stored procedure and MySQL
refuses `CREATE TRIGGER` inside one; with binary logging on, creating a trigger
needs `SUPER`, which no account here holds except root; and the Windows backup
dumps with `--triggers=FALSE`, so a restore would drop the guard without a
word. Grants have none of those problems.

Do not widen the grants to add a feature. A password change page needs
`UPDATE` on `password_hash`, and that one grant is what would let a
compromised process quietly take over the existing accounts instead of adding a
new one that shows up in the list.

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

Compare the `SHOW GRANTS` output against the matrix at the top of that file, and
expect the five finding queries after it to return no rows. This is what
catches a grant widened to `ALL` during a debugging session and never narrowed
back, and the one query that reads `COLUMN_PRIVILEGES` is the only thing that
notices `password_hash` or `disabled` added to the app's column list.

## Known gaps, stated plainly

1. **A lead delivered by the MySQL-down fallback exists only in the inbox.** If
   the database write fails and the inline mail attempt succeeds, the endpoint
   answers `200` and logs `contact.db_bypass` at Warning level, but no row is
   stored. That message will not appear in a DSR export or a backup. Search the
   Event Log for `contact.db_bypass` when reconciling.

   Every failed write, whether the mail then goes out or not, also logs
   `contact.store_error` at Error level with the exception type and the MySQL
   error number, and never the message: `1042` is MySQL not reachable, `1142`
   a missing grant, `1205` a lock wait timeout. If the mail failed too,
   `contact.store_failed` follows it, the visitor was answered `503`, and the
   inquiry is in neither place. EF's own error lines are turned off, so in that
   case this line is the only record of what the database said.
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
   restart. The admin account notice mail rides the same credential, so it
   stops too, and unlike a notification it is never retried: a user created
   while the credential is broken is announced to nobody but the operator who
   saw the warning.

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
[ ] ops/01-provision.sql run
[ ] ops/02-hardening.cnf merged into my.ini; MySQL restarted; binlog window confirmed
[ ] ops/migrate.sql applied as kestridge_migrator
[ ] ops/04-table-grants.sql applied as root after it; ops/03-verify-grants.sql clean
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
[ ] First admin account made with the CLI; every later one created in the Users tab
[ ] An account notice mail seen arriving at the team address
[ ] TAMAMLANACAQ-ISLER.txt section 5 cookie item ticked
[ ] Privacy Policy retention and sharing edits published, UPDATED date bumped
```
