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
| A mailbox that actually receives mail | `info@kestridge.com` does not resolve today: the zone has no MX record. Until it does, point the notification address at something you read. |

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

52 tests skip because there is no test database on this box. That is expected.

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

sudo bash ops/linux/03-deploy.sh
```

The `sed` is not optional when you regenerate the script. EF Core writes it with
a UTF-8 BOM, which MySQL reports as a syntax error on line 1 of a file that is
byte-for-byte correct.

Migrations go in as `kestridge_migrator`, never as the app. The application
credential holds DML only, on purpose. If you "fix" a startup error by granting
the app DDL, you have broken the access model.

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

Until an SMTP provider is chosen, point the API at a local catcher so the
notification path is exercised and the message can be read. `ops/linux/` does not
ship one; any local SMTP sink on 127.0.0.1:2525 works, with
`Kestridge:Smtp:Host` set to `127.0.0.1`, `Port` 2525 and `UseStartTls` false.
Rows reach `notify_state='sent'` and the captured message is the exact text the
team would receive.

### Cutover, once DNS exists

1. Add the A record, wait for it, and run steps 6 and 7 above (certbot, then
   `nginx-api.conf` in place of the staging config).
2. `sudo systemctl disable --now kestridge-web`, so the site is served from
   Vercel only and there are not two copies live on different hosts.
3. Remove `AdditionalOrigins` from `appsettings.Production.json` and restart the
   API. Anything left in that list can post to the form.
4. Set `NEXT_PUBLIC_FORM_ENDPOINT` in Vercel to the full HTTPS URL and trigger a
   rebuild.
