-- Kestridge AI backend - per-table privileges.
--
-- Run as root, AFTER ops/migrate.sql has created the tables. MySQL refuses a
-- table-level GRANT for a table that does not exist (ERROR 1146), which is why
-- these are not in 01-provision.sql.
--
-- Idempotent: re-granting a privilege that is already held is a no-op.
--
-- These grants are the security boundary the design actually rests on:
-- kestridge_app can read and write inquiries but has nothing at all on dsr_log,
-- and neither account can touch the schema.

-- Application runtime.
GRANT SELECT, INSERT, UPDATE, DELETE ON kestridge.contact_submissions TO 'kestridge_app'@'127.0.0.1';
GRANT SELECT, INSERT, UPDATE         ON kestridge.job_runs            TO 'kestridge_app'@'127.0.0.1';

-- A named human doing data-subject requests and legal holds.
GRANT SELECT, UPDATE, DELETE ON kestridge.contact_submissions TO 'kestridge_ops'@'127.0.0.1';
GRANT SELECT                 ON kestridge.job_runs            TO 'kestridge_ops'@'127.0.0.1';
GRANT SELECT, INSERT, UPDATE ON kestridge.dsr_log             TO 'kestridge_ops'@'127.0.0.1';

FLUSH PRIVILEGES;
