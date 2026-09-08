#!/usr/bin/env bash
# Kestridge AI backend - build, install and restart.
#
# Run from the repo checkout on the server, with sudo. Safe to re-run: this is
# also the redeploy path.
#
#   sudo bash ops/linux/03-deploy.sh
#
# What it does NOT do: apply migrations. Those go in as kestridge_migrator, on
# purpose, because the application credential holds DML only. See DEPLOY-LINUX.md.

set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
APP_DIR=/srv/kestridge-api
SERVICE_USER=kestridge
UNIT=kestridge-api
SETTINGS="$APP_DIR/appsettings.Production.json"
PUBLISH_DIR=$(mktemp -d)
trap 'rm -rf "$PUBLISH_DIR"' EXIT

if [ "$(id -u)" -ne 0 ]; then
    echo "Run with sudo." >&2
    exit 1
fi

if [ ! -f "$SETTINGS" ]; then
    echo "$SETTINGS is missing. Run ops/linux/02-provision-db.sh first." >&2
    exit 1
fi

echo "==> tests"
# If the box has no test database this skips 52 of them and still runs the rest.
dotnet test "$REPO_ROOT/Kestridge.sln" --nologo -v q

echo "==> publish"
dotnet publish "$REPO_ROOT/src/Kestridge.Api/Kestridge.Api.csproj" \
    -c Release -o "$PUBLISH_DIR" --nologo

echo "==> installing to $APP_DIR"
systemctl stop "$UNIT" 2>/dev/null || true

# --delete keeps the directory honest, but the secrets file lives here and is
# not part of the build output. Excluding it is the difference between a
# redeploy and an outage.
rsync -a --delete --exclude 'appsettings.Production.json' \
    "$PUBLISH_DIR"/ "$APP_DIR"/

# Set the modes explicitly, do not just narrow them. "rsync -a src/ dst/" also
# syncs the source directory's own attributes onto dst, and the source here is a
# mktemp -d, which is 0700. That leaves /srv/kestridge-api at 0700 root:kestridge
# so the service account cannot even chdir into it, and systemd reports
# status=200/CHDIR "Changing to the requested working directory failed", which
# reads like a unit-file problem rather than a permissions one.
chown -R root:"$SERVICE_USER" "$APP_DIR"
find "$APP_DIR" -type d -exec chmod 0750 {} +
find "$APP_DIR" -type f -exec chmod 0640 {} +

# Re-assert after the recursive pass above, which would otherwise have set it.
chown root:"$SERVICE_USER" "$SETTINGS"
chmod 0640 "$SETTINGS"

sudo -u "$SERVICE_USER" test -x "$APP_DIR" \
    || { echo "The service account cannot enter $APP_DIR." >&2; exit 1; }

echo "==> systemd unit"
install -o root -g root -m 0644 \
    "$REPO_ROOT/ops/linux/kestridge-api.service" \
    /etc/systemd/system/kestridge-api.service
systemctl daemon-reload
systemctl enable "$UNIT"

# Clears any start-limit lockout from an earlier failed attempt, which otherwise
# refuses to start and reports something unrelated.
systemctl reset-failed "$UNIT" 2>/dev/null || true
systemctl start "$UNIT"

echo "==> waiting for health"
for i in $(seq 1 30); do
    code=$(curl -s -o /dev/null -w '%{http_code}' http://127.0.0.1:5199/api/health || true)
    if [ "$code" = "200" ]; then
        echo "    /api/health 200 ok"
        break
    fi
    if [ "$i" = "30" ]; then
        echo
        echo "Health never returned 200. Last code: ${code:-none}" >&2
        echo "503 means the process is up but MySQL is not answering." >&2
        echo >&2
        journalctl -u "$UNIT" -n 40 --no-pager >&2
        exit 1
    fi
    sleep 1
done

curl -s http://127.0.0.1:5199/api/health; echo
systemctl --no-pager --lines=0 status "$UNIT"

echo
echo "Deployed. Logs: journalctl -u $UNIT -f"
