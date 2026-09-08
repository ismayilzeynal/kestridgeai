#!/usr/bin/env bash
# Kestridge AI backend - nightly dump.
#
# The dump contains every inquiry body. The output directory is personal data:
# its retention window has to match what DSR-PROCESS.md tells requesters, which
# is 30 days.
#
#   sudo bash ops/linux/backup.sh /srv/backups/kestridge
#
# The password comes from /etc/kestridge/backup.cnf, never from the command
# line: an argument vector is readable by any local process through /proc and is
# captured permanently by auditd and most EDR agents.
#
# Create that file once, mode 0600 root:root:
#
#   sudo install -d -m 0700 /etc/kestridge
#   sudo tee /etc/kestridge/backup.cnf >/dev/null <<'EOF'
#   [client]
#   user=kestridge_backup
#   password=THE_PASSWORD_FROM_02_PROVISION
#   host=127.0.0.1
#   EOF
#   sudo chmod 0600 /etc/kestridge/backup.cnf

set -euo pipefail

DEST="${1:-}"
KEEP_DAYS=30
DEFAULTS=/etc/kestridge/backup.cnf

if [ -z "$DEST" ]; then
    echo "Usage: backup.sh <destination-directory>" >&2
    exit 1
fi

if [ ! -r "$DEFAULTS" ]; then
    echo "$DEFAULTS is missing or unreadable. See the header of this script." >&2
    exit 1
fi

install -d -m 0700 "$DEST"

# Passed in via args by the caller, because Date.now equivalents inside a cron
# environment are fine but the timezone is not: force UTC so filenames sort and
# match what job_runs and the logs say.
STAMP=$(date -u +%Y%m%dT%H%M%SZ)
FILE="$DEST/kestridge-$STAMP.sql.gz"

# --single-transaction gives a consistent snapshot of InnoDB without locking
# writers out, which matters because the contact endpoint writes at any hour.
# --no-tablespaces is required, not cosmetic: without it mysqldump asks for the
# PROCESS privilege, which is GLOBAL-only and which kestridge_backup is
# deliberately not given.
mysqldump --defaults-extra-file="$DEFAULTS" \
    --single-transaction \
    --no-tablespaces \
    --default-character-set=utf8mb4 \
    --databases kestridge \
    | gzip -9 > "$FILE"

chmod 0600 "$FILE"

# A dump that gunzip cannot read is not a backup. Check now, not during an
# incident.
gzip -t "$FILE"

SIZE=$(stat -c %s "$FILE")
if [ "$SIZE" -lt 1024 ]; then
    echo "Dump is only $SIZE bytes. Treating that as a failure." >&2
    exit 1
fi

echo "wrote $FILE ($SIZE bytes)"

find "$DEST" -maxdepth 1 -name 'kestridge-*.sql.gz' -mtime +$KEEP_DAYS -print -delete

echo
echo "REMINDER: this is the local copy only. Copy it offsite, and remember the"
echo "offsite copy is inside the deletion window DSR-PROCESS.md discloses."
