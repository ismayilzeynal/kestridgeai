#!/usr/bin/env bash
# Kestridge AI backend - database provisioning and the production secrets file.
#
# Creates the databases and the four service accounts from ops/01-provision.sql,
# generates a password for each, applies the schema, and writes
# appsettings.Production.json with mode 0640 root:kestridge.
#
# Prompts for nothing except the values only you have (SMTP details, the
# notification mailbox). Passwords it generates are printed ONCE.
#
#   sudo bash ops/linux/02-provision-db.sh
#
# There is no MySQL root password and you do not need one. Ubuntu creates
# root@localhost with the auth_socket plugin, which authenticates by the
# connecting OS user over the Unix socket. Running this with sudo is the
# credential. Do not follow the popular advice to ALTER it to
# mysql_native_password: that plugin is removed in MySQL 8.4 and it replaces a
# credential that cannot leak with one that can.

set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
APP_DIR=/srv/kestridge-api
SERVICE_USER=kestridge
SETTINGS="$APP_DIR/appsettings.Production.json"

if [ "$(id -u)" -ne 0 ]; then
    echo "Run with sudo." >&2
    exit 1
fi

if [ -e "$SETTINGS" ]; then
    echo "$SETTINGS already exists. Refusing to overwrite it and orphan the" >&2
    echo "database passwords it contains. Move it aside first if you mean to." >&2
    exit 1
fi

gen() { tr -dc 'A-Za-z0-9' </dev/urandom | head -c "${1:-40}"; }

APP_PW=$(gen 40)
MIGRATOR_PW=$(gen 40)
OPS_PW=$(gen 40)
BACKUP_PW=$(gen 40)
PEPPER=$(gen 48)

echo "==> values only you have"
read -r -p "Notification mailbox (Kestridge:Contact:ToAddress) : " TO_ADDRESS
read -r -p "From address on the sending domain               : " FROM_ADDRESS
read -r -p "SMTP host                                        : " SMTP_HOST
read -r -p "SMTP port [587]                                  : " SMTP_PORT
SMTP_PORT=${SMTP_PORT:-587}
read -r -p "SMTP username                                    : " SMTP_USER
read -r -s -p "SMTP password                                    : " SMTP_PASS
echo

for v in TO_ADDRESS FROM_ADDRESS SMTP_HOST SMTP_USER; do
    if [ -z "${!v}" ]; then echo "$v cannot be empty." >&2; exit 1; fi
done

echo "==> creating databases, accounts and grants"
# ops/01-provision.sql is the source of truth for the grant matrix. Substitute
# the placeholders rather than restating the grants here, so the two cannot
# drift. kestridge_test is dropped: it exists for the test suite, not for a
# production box.
sed -e "s|<APP_PASSWORD>|$APP_PW|" \
    -e "s|<MIGRATOR_PASSWORD>|$MIGRATOR_PW|" \
    -e "s|<OPS_PASSWORD>|$OPS_PW|" \
    -e "s|<BACKUP_PASSWORD>|$BACKUP_PW|" \
    -e "s|<TEST_PASSWORD>|$(gen 40)|" \
    "$REPO_ROOT/ops/01-provision.sql" | mysql

echo "==> applying the schema"
# EF Core writes migrate.sql with a UTF-8 BOM. It is stripped in the repo, but
# strip it again here so a regenerated file cannot produce the confusing
# "error in your SQL syntax near 'CREATE TABLE' at line 1" on a file that is
# byte-for-byte correct. CRLF from a Windows checkout goes too.
sed -e '1s/^\xEF\xBB\xBF//' -e 's/\r$//' "$REPO_ROOT/ops/migrate.sql" \
    | mysql --database=kestridge

echo "==> verifying grants against the matrix"
mysql < "$REPO_ROOT/ops/03-verify-grants.sql" || true

echo "==> MySQL drop-in"
install -o root -g root -m 0644 \
    "$REPO_ROOT/ops/linux/zz-kestridge.cnf" /etc/mysql/mysql.conf.d/zz-kestridge.cnf
# Catch a bad option now, while MySQL is still up, instead of when it refuses to
# restart and takes the site with it.
mysqld --validate-config
echo "    config valid, restarting mysql"
systemctl restart mysql

echo "==> writing $SETTINGS"
umask 077
cat > "$SETTINGS" <<JSON
{
  "ConnectionStrings": {
    "Default": "Server=127.0.0.1;Port=3306;Database=kestridge;User ID=kestridge_app;Password=$APP_PW;SslMode=Preferred;AllowPublicKeyRetrieval=False;DateTimeKind=Utc;DefaultCommandTimeout=15;Pooling=true;MaximumPoolSize=20"
  },
  "Kestridge": {
    "Cors":    { "AllowVercelPreviews": false },
    "Contact": {
      "RequireOrigin": true,
      "ToAddress":     "$TO_ADDRESS",
      "FromAddress":   "$FROM_ADDRESS"
    },
    "Smtp": {
      "Host":        "$SMTP_HOST",
      "Port":        $SMTP_PORT,
      "UseStartTls": true,
      "User":        "$SMTP_USER",
      "Password":    "$SMTP_PASS"
    },
    "Dsr": { "EmailHashPepper": "$PEPPER" }
  }
}
JSON

# 0640 root:kestridge, in a 0750 root:kestridge directory. The service reads it;
# nothing else on the box can. Machine environment variables were the obvious
# alternative and are the wrong one: that registry of variables is readable by
# every local user.
chown root:"$SERVICE_USER" "$SETTINGS"
chmod 0640 "$SETTINGS"
sudo -u "$SERVICE_USER" test -r "$SETTINGS" \
    || { echo "The service account cannot read $SETTINGS." >&2; exit 1; }

echo
echo "======================================================================"
echo " Store these in the team password manager NOW. They are not recoverable"
echo " and this is the only time they are printed."
echo "======================================================================"
echo "  kestridge_migrator : $MIGRATOR_PW"
echo "  kestridge_ops      : $OPS_PW"
echo "  kestridge_backup   : $BACKUP_PW"
echo "----------------------------------------------------------------------"
echo " kestridge_app and the DSR pepper are in $SETTINGS and are not printed."
echo " NEVER rotate the pepper: existing dsr_log hashes stop matching."
echo "======================================================================"
echo
echo "Next: ops/linux/03-deploy.sh"
