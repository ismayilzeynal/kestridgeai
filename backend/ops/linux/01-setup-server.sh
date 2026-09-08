#!/usr/bin/env bash
# Kestridge AI backend - base server setup for Ubuntu 24.04 LTS.
#
# Installs the .NET 10 runtime and SDK, MySQL 8.0, nginx and certbot, opens the
# firewall, and creates the service account and directories. Idempotent: safe to
# re-run.
#
# Run as a user with sudo. Nothing here prompts.
#
#   sudo bash ops/linux/01-setup-server.sh

set -euo pipefail

SERVICE_USER=kestridge
APP_DIR=/srv/kestridge-api
STATE_DIR=/var/lib/kestridge-api

if [ "$(id -u)" -ne 0 ]; then
    echo "Run with sudo." >&2
    exit 1
fi

. /etc/os-release
if [ "${VERSION_ID:-}" != "24.04" ]; then
    echo "WARNING: this script was written for Ubuntu 24.04, found ${PRETTY_NAME:-unknown}." >&2
    echo "Package names and versions below may differ. Continuing in 5 seconds." >&2
    sleep 5
fi

export DEBIAN_FRONTEND=noninteractive

echo "==> SSH before firewall, in that order. Reversing it locks you out."
ufw allow OpenSSH
# By port, not by the 'Nginx Full' application profile. That profile is
# registered by the nginx package, which is not installed yet at this point in
# the script, so naming it here fails with "Could not find a profile matching"
# and set -e aborts the run. 80 and 443 is exactly what the profile opens.
ufw allow 80/tcp
ufw allow 443/tcp
ufw --force enable
ufw status verbose

echo "==> apt update"
apt-get update
apt-get upgrade -y

echo "==> .NET 10 (LTS, in the Ubuntu 24.04 main archive)"
# Not the PPA and not packages.microsoft.com. Microsoft publishes no .NET
# packages for Ubuntu 24.04 at all; .NET 9 exists only in ppa:dotnet/backports
# and goes out of support 2026-11-10. .NET 10 is LTS to 2028-11-14 and is
# security-supported by Canonical for the life of 24.04.
apt-get install -y aspnetcore-runtime-10.0 dotnet-sdk-10.0

echo "==> MySQL 8.0"
apt-get install -y mysql-server

echo "==> nginx and certbot"
# apt, not snap. One subdomain over HTTP-01; the only things noble's certbot
# lacks are features we do not want. Installing both would give you two
# renewal timers fighting each other.
apt-get install -y nginx certbot python3-certbot-nginx

echo "==> service account"
# System account, no login shell, no home directory. It owns nothing it does
# not need to.
if ! id -u "$SERVICE_USER" >/dev/null 2>&1; then
    useradd --system --no-create-home --home-dir /nonexistent \
            --shell /usr/sbin/nologin "$SERVICE_USER"
fi

install -d -o root -g "$SERVICE_USER" -m 0750 "$APP_DIR"
install -d -o "$SERVICE_USER" -g "$SERVICE_USER" -m 0750 "$STATE_DIR"

echo
echo "==> versions actually installed"
dotnet --list-runtimes | sed 's/^/    /'
mysql --version | sed 's/^/    /'
nginx -v 2>&1 | sed 's/^/    /'
certbot --version 2>&1 | sed 's/^/    /'

echo
echo "==> MySQL defaults worth knowing (verify, do not assume)"
mysql -e "SELECT @@bind_address AS bind_address, @@log_bin AS log_bin, @@sql_mode AS sql_mode\G" || true

echo
echo "Done. Next: ops/linux/02-provision-db.sh"
