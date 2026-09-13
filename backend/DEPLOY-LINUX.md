# Deploying the backend to a Linux VPS

Written for **Ubuntu 24.04 LTS**, and run end to end on one: Contabo, 8 GB,
Ubuntu 24.04.4, with .NET 10.0.11, MySQL 8.0.46, nginx 1.24.0 and certbot 2.9.0
from the distribution feeds. Paste the blocks in order. Every command is
non-interactive except the ones marked **YOU**, which ask for values only you
have.

The frontend stays on Vercel. The only frontend change is one environment
variable, in step 10.

> **If DNS is not ready yet**, skip to [Staging from the bare IP](#staging-from-the-bare-ip)
> at the end. It brings the API and a copy of the site up on one origin over
> plain HTTP so the contact form can be exercised, and leaves a three-step
> cutover for when the DNS record exists.

---

## Before you start

| Need | Why |
| --- | --- |
| Ubuntu 24.04 LTS, 2 GB RAM minimum | `dotnet publish` on 1 GB gets OOM-killed with no useful message. If you only have 1 GB, add 2 GB of swap first, or publish on your Windows box and copy the output up. |
| SSH access as a sudo user | Everything below is `sudo`. |
| `api.kestridge.com` A record pointing at the VPS | **Publish it before step 7.** certbot fails without it and each failure burns one of your five hourly authorization attempts for that name. |
| SMTP provider account | Host, port, username, password. Step 4 asks for them. |
| A mailbox that actually receives mail | `info@kestridge.com` has received mail since 10 September 2026 (Zoho, US data centre). Point the notification address anywhere else only while testing. |

Do **not** publish an AAAA record unless the VPS IPv6 is fully working.
Let's Encrypt tries IPv6 first and a half-configured address fails the challenge.

---

## 1. Base server

```bash
sudo apt-get update && sudo apt-get install -y git
git clone -b backend https://github.com/ismayilzeynal/kestridgeai.git ~/kestridgeai
cd ~/kestridgeai/backend
```

`-b backend` is required until the branch is merged into `main`. The backend
does not exist on `main`.

```bash
sudo bash ops/linux/01-setup-server.sh
```

Installs .NET 10 (runtime + SDK), MySQL 8.0, nginx, certbot, opens the firewall
for SSH and nginx, and creates the `kestridge` service account.

It opens SSH in the firewall **before** enabling it. If you ever reorder that,
you lock yourself out and need the provider's console.

Check the versions it prints. `dotnet --list-runtimes` must show
`Microsoft.AspNetCore.App 10.0.x`.

> **Why .NET 10 and not 9.** Microsoft publishes no .NET packages for Ubuntu
> 24.04 at all, so every guide starting with `packages-microsoft-prod.deb` fails
> here with `Unable to locate package`. .NET 9 exists only in
> `ppa:dotnet/backports` and goes out of support **2026-11-10**, about two
> months from now. .NET 10 is LTS to 2028-11-14 and sits in Ubuntu's main
> archive, so Canonical patches it for the life of 24.04.

## 2. Database, accounts and secrets

**YOU** - this one asks for the SMTP details and the notification mailbox.

```bash
sudo bash ops/linux/02-provision-db.sh
```

It creates the databases and four service accounts, generates a password for
each, applies the schema, installs the MySQL drop-in, and writes
`/srv/kestridge-api/appsettings.Production.json` as `0640 root:kestridge`.

**It prints three passwords once. Put them in the password manager before you
close the terminal.** `kestridge_app` and the DSR pepper are written straight
into the secrets file and are never printed.

There is no MySQL root password and you do not need one. Ubuntu authenticates
`root@localhost` by OS user over the Unix socket, so `sudo mysql` is the
credential. If you hit `ERROR 1698 (28000)`, you are trying to connect over TCP
or without sudo. Do not "fix" it by switching root to `mysql_native_password`:
that plugin is gone in MySQL 8.4.

## 3. Build, install, start

```bash
sudo bash ops/linux/03-deploy.sh
```

Runs the tests, publishes Release, rsyncs into `/srv/kestridge-api` (excluding
the secrets file), installs the systemd unit, starts it, and polls
`/api/health` until it answers 200.

Nothing is skipped. The script runs the suite with `CI=1`, so if `kestridge_test`
is not provisioned (see "The test database is not optional" below) the suite
fails and the deploy stops here. A plain `dotnet test` on a machine with no test
database reports 280 passed and 128 skipped of 408 instead.

If it fails, the script prints the last 40 journal lines. The most common cause
by far is options validation: a missing value in the secrets file. Fix it, then
`sudo systemctl reset-failed kestridge-api` before retrying, or the start limit
keeps refusing you.

```bash
journalctl -u kestridge-api -n 50 --no-pager
journalctl -u kestridge-api -p err          # only works because of the systemd log formatter
```

## 4. nginx, HTTP only

```bash
sudo cp ops/linux/nginx-bootstrap.conf /etc/nginx/sites-available/api.kestridge.com
sudo ln -sf /etc/nginx/sites-available/api.kestridge.com /etc/nginx/sites-enabled/
sudo rm -f /etc/nginx/sites-enabled/default
sudo nginx -t && sudo systemctl reload nginx
```

## 5. Prove DNS and port 80 before touching certbot

From **your own machine**, not the server:

```bash
dig +short A api.kestridge.com
curl -sS -o /dev/null -w '%{http_code}\n' http://api.kestridge.com/.well-known/acme-challenge/probe
```

The first must print the VPS address. The second must print `404`, which means
nginx served the request. Anything else (timeout, `000`, `502`) means certbot
will fail. Fix it here.

## 6. Certificate

**YOU** - replace the email.

```bash
sudo certbot certonly --webroot -w /var/www/html -d api.kestridge.com \
  --non-interactive --agree-tos --email you@example.com \
  --deploy-hook "systemctl reload nginx" --dry-run
```

`--dry-run` first, always. It uses the staging environment and costs nothing.
Only when it succeeds:

```bash
sudo certbot certonly --webroot -w /var/www/html -d api.kestridge.com \
  --non-interactive --agree-tos --email you@example.com \
  --deploy-hook "systemctl reload nginx"
```

Five failed authorizations for one name per hour and you are locked out of that
name until it refills. Never put certbot in a retry loop.

## 7. nginx, the real config

```bash
sudo cp ops/linux/nginx-api.conf /etc/nginx/sites-available/api.kestridge.com
sudo nginx -t && sudo systemctl reload nginx
```

`nginx -t` failing with `unknown directive "http2"` means you are on a newer
nginx than 24.04's 1.24.0 and should switch the two `listen 443 ssl http2;`
lines to `listen 443 ssl;` plus `http2 on;`.

Then prove renewal actually reloads nginx, because Let's Encrypt stopped sending
expiry warnings in June 2025:

```bash
sudo certbot renew --cert-name api.kestridge.com --dry-run --run-deploy-hooks
systemctl list-timers | grep -i certbot
```

## 8. Verify from outside

From your own machine:

```bash
curl -s https://api.kestridge.com/api/health
```

Expect `{"status":"ok"}`. `{"status":"degraded"}` means the process is up but
MySQL is not answering.

```bash
# A real submission. Expect 200 and {"ok":true}.
curl -i -H "Origin: https://kestridge.com" \
  -F name="Deploy Test" -F email="you@example.com" -F company="" -F phone="" \
  -F service=general -F message="Deployment smoke test, please ignore." -F _gotcha="" \
  https://api.kestridge.com/api/contact

# No Origin. Expect 403. This is the control, not a bug.
curl -i -F name=x -F email=x@y.z -F service=ai -F message="hello there" \
  https://api.kestridge.com/api/contact

# Honeypot filled. Expect 200 with nothing stored and no mail.
curl -i -H "Origin: https://kestridge.com" \
  -F name="Bot" -F email="bot@example.com" -F service=ai \
  -F message="spam spam spam" -F _gotcha="filled" \
  https://api.kestridge.com/api/contact

# Kestrel must not be reachable from outside. Expect a timeout or refusal.
curl -m 5 -i http://api.kestridge.com:5199/api/health
```

Then confirm the row and the notification:

```bash
sudo mysql kestridge -e "SELECT id, created_at, service, notify_state, notified_at FROM contact_submissions ORDER BY id DESC LIMIT 5;"
```

`notify_state` should reach `sent` within about 30 seconds. If it stays
`pending` with rising `notify_attempts`, SMTP is the problem, not the app.

## 9. Prove the rate limiter sees the real client

This is worth doing once, because if it is wrong the symptom is that five
submissions from anywhere on earth lock out every other visitor for ten minutes,
while `/api/health` stays green and nothing pages you.

Send six submissions quickly from your machine. The sixth must be `429`. Then
ask a colleague on a different network to send one: theirs must be `200`. If
theirs is also `429`, `X-Forwarded-For` is not reaching the app and every
visitor is sharing one bucket.

nginx's own access log always shows the real IP and proves nothing about this.

## 10. Point the frontend at it

In the Vercel dashboard for the `kestridgeai` project:

```
NEXT_PUBLIC_FORM_ENDPOINT = https://api.kestridge.com/api/contact
```

No trailing slash. Then **trigger a rebuild**. `NEXT_PUBLIC_*` values are
inlined into the bundle at build time, so redeploying the existing artifact
changes nothing.

Then submit the real form on `https://kestridge.com` and confirm in devtools:

- the success screen appears
- the notification email arrives
- there is **no** `Set-Cookie` on the `/api/contact` response
- there is **no** `OPTIONS` preflight

## 11. Backups

**YOU** - paste the `kestridge_backup` password from step 2.

```bash
sudo install -d -m 0700 /etc/kestridge
sudo tee /etc/kestridge/backup.cnf >/dev/null <<'EOF'
[client]
user=kestridge_backup
password=PASTE_IT_HERE
host=127.0.0.1
EOF
sudo chmod 0600 /etc/kestridge/backup.cnf

sudo bash ops/linux/backup.sh /srv/backups/kestridge
```

Then schedule it nightly and add an offsite copy. The dump contains every
inquiry body: the retention window has to match what `DSR-PROCESS.md` tells
people who ask for their data to be deleted.

---

## Redeploying later

```bash
cd ~/kestridgeai && git pull
cd backend

# Only when the model changed:
dotnet dotnet-ef migrations script --idempotent --project src/Kestridge.Api -o ops/migrate.sql
sed -e '1s/^\xEF\xBB\xBF//' -e 's/\r$//' ops/migrate.sql | mysql -h 127.0.0.1 -u kestridge_migrator -p kestridge

# And when it added a table, or ops/04-table-grants.sql changed:
sudo mysql < ops/04-table-grants.sql

sudo bash ops/linux/03-deploy.sh
```

The `sed` is not optional when you regenerate the script. EF Core writes it with
a UTF-8 BOM, which MySQL reports as a syntax error on line 1 of a file that is
byte-for-byte correct.

Migrations go in as `kestridge_migrator`, never as the app. The application
credential holds DML only, on purpose. If you "fix" a startup error by granting
the app DDL, you have broken the access model.

**Migrate first, always. The order is not symmetric.** An old build against a
new schema is fine: it selects the columns it knows and ignores the rest. A new
build against an old schema breaks every contact form INSERT, because EF names
every column in the statement and MySQL refuses the whole thing. So the window
between the two steps is a window where the public form is either fine or
completely broken, depending only on which one you did first.

Table-level grants come after the migration, never with it. MySQL refuses a
`GRANT` for a table that does not exist yet (`ERROR 1146`), which is the whole
reason `ops/04-table-grants.sql` is a separate file from `ops/01-provision.sql`.

### Deploying the content tables (stage 2)

```bash
cd ~/kestridgeai/backend

# 1. Schema, as the migrator.
mysql -h 127.0.0.1 -u kestridge_migrator -p kestridge < ops/migrate.sql

# 2. Grants, after the migration or ERROR 1146.
sudo mysql < ops/04-table-grants.sql

# 3. Seed the tables with the copy the site already ships. Once. Every block
#    is guarded on emptiness, so a second run after real edits does nothing.
mysql -h 127.0.0.1 -u kestridge_migrator -p kestridge < ops/05-seed-content.sql

# 4. Only if the revalidate hook is wanted. Both keys, or neither: an empty
#    value means the API never calls out and the panel says so honestly.
sudo nano /srv/kestridge-api/appsettings.Production.json
#   "Kestridge": { "Admin": {
#       "RevalidateUrl": "https://kestridge.com/api/revalidate",
#       "RevalidateSecret": "<the same value you put in Vercel>" } }

# 5. Deploy.
sudo bash ops/linux/03-deploy.sh

# 6. Populated arrays before Vercel is told anything.
curl -s https://api.kestridge.com/api/content | head -c 400

# 7. Grants again, because step 2 is easy to skip.
sudo mysql < ops/03-verify-grants.sql
```

Then, and only then, set `API_ORIGIN=https://api.kestridge.com` in the Vercel
project environment and redeploy. Setting it before the seed is harmless: empty
arrays fail validation in `src/lib/content.ts` and the compiled constants
render, which are the same words.

**Rollback for the whole content feature is removing `API_ORIGIN` from Vercel
and redeploying.** That puts every section back on the constants in
`src/data/*.ts` without touching the database or the API, with no build failure
and no blank page.

### Deploying admin user management (stage 3)

The panel gains a Users tab and first sign-in enrolment. The new build reads two
new tables, `admin_enrollments` and `admin_disables`, on every sign-in and on
every panel request, so the tables and their grants have to exist before the
build does. The contact form and `/api/content` never touch them.

```bash
cd ~/kestridgeai && git pull
cd backend

# 1. Schema, as the migrator. ops/migrate.sql is committed already regenerated
#    with the AdminUserManagement migration. The sed is harmless on it, and not
#    optional if you regenerate the script here, because EF writes a BOM.
sed -e '1s/^\xEF\xBB\xBF//' -e 's/\r$//' ops/migrate.sql | mysql -h 127.0.0.1 -u kestridge_migrator -p kestridge
sudo mysql kestridge -e "SHOW TABLES LIKE 'admin%';"
#    Expect four: admin_accounts, admin_disables, admin_enrollments, admin_sessions.

# 2. Grants, as root, after the migration or ERROR 1146. This is also the step
#    that lets kestridge_app create accounts; RUNBOOK.md, "The account
#    boundary", says exactly what it can and cannot do from here on.
sudo mysql < ops/04-table-grants.sql

# 3. Check them before any code depends on them. The five finding queries
#    must print nothing.
sudo mysql < ops/03-verify-grants.sql

# 4. Deploy.
sudo bash ops/linux/03-deploy.sh
```

**Step 4 before steps 1 and 2 locks everyone out of the panel.** Continue on
the sign-in form answers "Sign in is not available right now", an open panel
fails on its next click, `/api/health` stays green, and
`journalctl -u kestridge-api -p err` shows a `MySqlException` naming one of
the two tables. Run steps 1 to 3; no redeploy is needed after them.

Then smoke test it in a browser. It needs your own account and a second entry
in an authenticator app, which can be on your own phone.

1. **Create a user.** Users tab, "Create a user": username `smoketest`, display
   name `Smoke Test`, a temporary password, and your current code. Expect a
   confirmation under the form, `smoketest` under "Waiting for first sign-in",
   and a mail at `info@kestridge.com` with the subject
   `Kestridge admin: account created for smoketest`. A warning that the notice
   mail was not sent means SMTP, not the feature.
2. **First sign-in.** In a private window, sign in as `smoketest` with the
   temporary password. Expect "Set up your authenticator" with a QR code
   instead of the code step. Scan it, choose a new password, type the code,
   Finish setup. Expect the inbox, signed in as Smoke Test. This is the first
   time the QR code is drawn on Linux: if Continue answers "Sign in is not
   available right now" for this user only, the journal has the exception.
3. **Reset.** Back in your own window, "Reset authenticator" on `smoketest`,
   with your next code, not the one step 1 spent. Expect the second mail. The
   private window's next click lands on the sign-in form. Signing in there with
   the password chosen in step 2 shows the setup screen again, this time
   without the password fields; finish it with a fresh scan.
4. **Disable.** "Disable" on `smoketest`. The private window's next click lands
   on the sign-in form, and signing in again goes to the code step and fails.

Both browser windows are one client address to the rate limiter, which allows
it, per 10 minutes, 10 presses of Continue, 5 sign-in codes and 30 presses of
Finish setup, each counted separately. Your own sign-in and steps 2 to 4 spend
at most 4, 2 and 2 of them, so retries fit. If Continue still answers "Too many
attempts from this network", that says nothing about the feature: wait 10
minutes and sign in again.

```bash
sudo journalctl -u kestridge-api --since "30 min ago" --no-pager | grep 'admin\.'
```

Expect these among the lines, in this order: `admin.user_created`,
`admin.enroll_started`, `admin.enrolled`, `admin.authenticator_reset`,
`admin.enroll_started`, `admin.enrolled`, `admin.user_disabled` and
`admin.login_blocked`, each carrying a number or nothing, never a name, and no
`admin.notice_failed`. Then delete the `smoketest` entries from the
authenticator app.

Do nothing else as `smoketest`, and the account can go afterwards. It stays
listed as disabled, because the application can never delete an account row,
but it handled nothing, so no `handled_by` carries its name and the migrator
may remove it:

```bash
mysql -h 127.0.0.1 -u kestridge_migrator -p kestridge -e "DELETE FROM admin_disables WHERE account_id = (SELECT id FROM admin_accounts WHERE username = 'smoketest'); DELETE FROM admin_accounts WHERE username = 'smoketest';"
```

**Rollback is redeploying the previous commit, with two steps first.** The old
build is fine against the new tables and grants, but it knows nothing about
`admin_disables`, so every account disabled from the panel would sign in again
under it. Copy those disables into the column it does read, as the migrator,
before it starts:

```bash
mysql -h 127.0.0.1 -u kestridge_migrator -p kestridge -e "UPDATE admin_accounts a JOIN admin_disables d ON d.account_id = a.id SET a.disabled = 1;"
```

The second step is for the test database, not production. The new build's
test run already migrated `kestridge_test`, so it holds 13 tables, and the
previous commit's `SchemaTests` counts every table there and expects 11.
`03-deploy.sh` runs that suite before it publishes, so without this the
rollback stops at the tests with "Expected: 11, Actual: 13" and the new build
keeps running. Take the two tables and their migration row back out, as
`kestridge_test`, which owns that schema:

```bash
mysql -h 127.0.0.1 -u kestridge_test -p kestridge_test -e "DROP TABLE admin_enrollments, admin_disables; DELETE FROM __EFMigrationsHistory WHERE MigrationId = '20260913122030_AdminUserManagement';"
```

Only `kestridge_test` is touched. Production keeps both tables through the
rollback, and nothing needs undoing when the new build is deployed again: its
test run migrates `kestridge_test` back to 13 tables by itself.

An account waiting for a reset cannot sign in under the old build, and a user
waiting for their first sign-in cannot finish it. Both need the CLI in
`RUNBOOK.md` until the new build is back. The new grants can stay through a
short rollback, since the old build never uses them. If the rollback becomes
permanent, take back the two that widened the boundary:

```bash
sudo mysql -e "REVOKE INSERT ON kestridge.admin_accounts FROM 'kestridge_app'@'127.0.0.1'; REVOKE UPDATE (totp_secret) ON kestridge.admin_accounts FROM 'kestridge_app'@'127.0.0.1';"
```

---

## Where it breaks

Ordered by how much time it costs.

**1. `systemctl start` succeeds, then the unit fails.**
`journalctl` shows `OptionsValidationException`. A required value is missing
from `appsettings.Production.json`: `Kestridge:Dsr:EmailHashPepper` (minimum 16
characters), `Kestridge:Smtp:Host`, `Kestridge:Contact:ToAddress` or
`FromAddress`. Fix the file, then `sudo systemctl reset-failed kestridge-api`.

**2. Same exception, but the file is correct.**
The content root is wrong, so no `appsettings*.json` is found at all and every
option binds to its default. `systemctl show kestridge-api -p WorkingDirectory`
must print `/srv/kestridge-api`.

**3. `UnauthorizedAccessException` on the secrets file.**
Directory must be `0750 root:kestridge` and the file `0640 root:kestridge`.
Verify with `sudo -u kestridge test -r /srv/kestridge-api/appsettings.Production.json`.
A redeploy that used to work and now fails usually means an rsync deleted the
file: `03-deploy.sh` excludes it, ad-hoc rsync commands do not.

**4. certbot fails.**
Almost always DNS not published, port 80 blocked, or an AAAA record for an IPv6
address that does not work. Step 5 exists to catch all three before you spend an
authorization attempt.

**5. The form shows "That did not go through" but rows appear in the database.**
Something stripped `Access-Control-Allow-Origin` from an error response. Check
that `proxy_intercept_errors` is `off` and that there is no `error_page` for
5xx. The browser cannot read the response, so it reports a network failure and
the visitor resubmits: duplicate rows and duplicate mail.

**6. Everyone gets 429 after five submissions.**
`X-Forwarded-For` is not reaching the app, so every visitor partitions as
`127.0.0.1`. See step 9.

**7. HTTP works, email silently stops.**
Rows sit at `notify_state='pending'` with rising attempts. Check the mundane
cause first: MX record for the notification mailbox, SPF and DKIM for the
`FromAddress` domain. If someone has edited the systemd unit, check that
`RestrictAddressFamilies` still lists `AF_NETLINK`: glibc opens a netlink socket
during DNS resolution, so dropping it breaks outbound SMTP while HTTP keeps
working. After fixing the cause, re-arm with `ops/rearm-notifications.sql`.

**8. The service dies with `Failed to create CoreCLR` after a hardening pass.**
Someone added `MemoryDenyWriteExecute=yes` because `systemd-analyze security`
suggested it. It blocks the JIT. The unit file says so in a comment; put it
back.

**9. SELinux commands do nothing.**
There is no SELinux on Ubuntu. Every `semanage`, `setsebool` and `chcon`
instruction you find is from a RHEL guide. AppArmor is what is present, and the
only profile that matters here is `usr.sbin.mysqld`, which only bites if you
move MySQL's data directory. Check with `sudo aa-status`.

**10. .NET security updates never arrive.**
Not applicable on the .NET 10 path, since it comes from the Ubuntu archive and
`unattended-upgrades` covers it. It would apply if you had used the .NET 9 PPA,
whose origin is excluded by default.

---

## Things to verify on the box rather than trust this document

Package versions drift. Before relying on anything version-gated, check:

```bash
apt-cache policy aspnetcore-runtime-10.0 mysql-server nginx certbot
nginx -v && certbot --version
dotnet --list-runtimes
mysql -e "SELECT @@version, @@bind_address, @@log_bin, @@sql_mode\G"
sudo aa-status | head -20
```

Two specific claims worth confirming rather than assuming:

- `RequestSizeLimitAttribute` on a minimal-API endpoint. Post 100 KB straight at
  `http://127.0.0.1:5199/api/contact` and see what status comes back. The 64 KB
  ceiling holds either way through `FormOptions`, but the code may differ.
- Whether nginx has an enforcing AppArmor profile on your image.

---

## Staging from the bare IP

Use this only while `api.kestridge.com` does not resolve. It exists because the
live site is HTTPS: a form on `https://kestridge.com` cannot post to
`http://<ip>/api/contact` at all, the browser blocks it as mixed content. Serving
the site and the API from one origin sidesteps that and lets the whole path be
exercised for real.

**This is not production.** There is no TLS, so submissions cross the network in
cleartext. Do not put real client data through it.

Run steps 1 to 3 above first, then:

```bash
# Node, for the site
curl -fsSL https://deb.nodesource.com/setup_22.x -o /tmp/nodesource.sh
sudo bash /tmp/nodesource.sh && sudo apt-get install -y nodejs

# Build the site with a relative endpoint, so the POST is same-origin
cd ~/kestridgeai
npm ci
NEXT_PUBLIC_FORM_ENDPOINT=/api/contact npm run build

# Install it where the service account can read it
sudo install -d -o root -g kestridge -m 0750 /srv/kestridge-web
sudo rsync -a --exclude .git --exclude backend ~/kestridgeai/ /srv/kestridge-web/
sudo chown -R root:kestridge /srv/kestridge-web
sudo find /srv/kestridge-web -type d -exec chmod 0750 {} +
sudo find /srv/kestridge-web -type f -exec chmod 0640 {} +
sudo chown -R kestridge:kestridge /srv/kestridge-web/.next
sudo find /srv/kestridge-web/.next -type d -exec chmod 0750 {} +

sudo install -m 0644 backend/ops/linux/kestridge-web.service /etc/systemd/system/
sudo systemctl daemon-reload && sudo systemctl enable --now kestridge-web
```

Then nginx, and the one configuration change the API needs:

```bash
cd ~/kestridgeai/backend
sudo cp ops/linux/nginx-staging-ip.conf /etc/nginx/sites-available/kestridge-staging
sudo rm -f /etc/nginx/sites-enabled/default
sudo ln -sf /etc/nginx/sites-available/kestridge-staging /etc/nginx/sites-enabled/
sudo nginx -t && sudo systemctl reload nginx
```

The origin guard allows the real domain and localhost, and nothing else. Add the
staging origin, exactly, with no scheme or port drift:

```bash
sudo python3 - <<'PY'
import json
p = "/srv/kestridge-api/appsettings.Production.json"
d = json.load(open(p))
d["Kestridge"]["Cors"]["AdditionalOrigins"] = ["http://YOUR.VPS.IP.HERE"]
json.dump(d, open(p, "w"), indent=2)
PY
sudo chown root:kestridge /srv/kestridge-api/appsettings.Production.json
sudo chmod 0640 /srv/kestridge-api/appsettings.Production.json
sudo systemctl restart kestridge-api
```

Verify from your own machine:

```bash
curl -s -o /dev/null -w '%{http_code}\n' http://YOUR.VPS.IP.HERE/
curl -s http://YOUR.VPS.IP.HERE/api/health
curl -i -H "Origin: http://YOUR.VPS.IP.HERE" \
  -F name="Staging" -F email="you@example.com" -F company="" -F phone="" \
  -F service=general -F message="Staging check." -F _gotcha="" \
  http://YOUR.VPS.IP.HERE/api/contact
curl -m 5 -i http://YOUR.VPS.IP.HERE:5199/api/health   # must NOT answer
```

### Seeing the notification without a mail provider

Production sends through Zoho Mail as of 10 September 2026, so this is only for
a server that has no credential yet. Point the API at a local catcher and the
notification path is exercised end to end and the message can be read.
`ops/linux/` does not ship one; any local SMTP sink on 127.0.0.1:2525 works,
with `Kestridge:Smtp:Host` set to `127.0.0.1`, `Port` 2525 and `UseStartTls`
false. Rows reach `notify_state='sent'` and the captured message is the exact
text the team would receive.

**A catcher makes rows say `sent` when nothing was delivered.** That is the
point of it, and it is also the trap: after switching to a real provider, those
rows still read `sent` and `ops/rearm-notifications.sql` will not pick them up,
because it looks for `failed`. Re-send them by hand or accept that they were
only ever test traffic.

### The test database is not optional

`03-deploy.sh` runs the suite with `CI=1`, which makes `MySqlFixture` throw
rather than skip when it cannot reach `kestridge_test`. Provision it, or the
deploy stops:

```bash
sudo mysql -e "ALTER USER 'kestridge_test'@'127.0.0.1' IDENTIFIED BY 'kestridge_test';"
mysql -h 127.0.0.1 -u kestridge_test -pkestridge_test -e "SELECT 1;"
```

That is the credential `MySqlFixture` defaults to, and a weak password is
acceptable for it and only for it: the account holds `ALL` on
`kestridge_test.*` and `USAGE` on everything else, so it cannot read one row of
production data. Override it with `KESTRIDGE_TEST_CONNECTION` if you would
rather not.

Without this the suite reports "Passed" while skipping every database test,
128 of 408 as of 13 September 2026, including every test that checks the
schema against the model. It did exactly that from
the first deploy until 10 September 2026, and it was hiding two real failures.

### The production mail settings

```json
"Kestridge": {
  "Smtp": {
    "Host": "smtp.zoho.com",
    "Port": 587,
    "UseStartTls": true,
    "User": "chingiz@kestridge.com",
    "Password": "<Zoho app password, not the account password>"
  },
  "Contact": {
    "ToAddress": "info@kestridge.com",
    "FromAddress": "info@kestridge.com"
  }
}
```

`User` is a real account and `FromAddress` is an alias on it. Zoho refuses to
send as an address the authenticated account does not own, so `no-reply@`, which
exists nowhere, fails with a relaying error that reads like a network problem.
The app password comes from `accounts.zoho.com` under Security, and only works
when generated on the account named in `User`.

Replying to a notification does not reply to `info@`: `NotificationMessage`
sets `Reply-To` to the visitor's own address.

### Cutover, once DNS exists

1. Add the A record, wait for it, and run steps 6 and 7 above (certbot, then
   `nginx-api.conf` in place of the staging config).
2. `sudo systemctl disable --now kestridge-web`, so the site is served from
   Vercel only and there are not two copies live on different hosts.
3. Remove `AdditionalOrigins` from `appsettings.Production.json` and restart the
   API. Anything left in that list can post to the form.
4. Set `NEXT_PUBLIC_FORM_ENDPOINT` in Vercel to the full HTTPS URL and trigger a
   rebuild.
