-- Kestridge AI backend - one-time database and account provisioning.
-- Run as root. Driven by ops/setup-dev-db.ps1 (Windows) or
-- ops/linux/02-provision-db.sh (Linux), which generate the passwords and
-- substitute them; the committed file carries placeholders only.
--
-- The boundaries that actually hold here are holder and lifetime, not privilege
-- arithmetic: migrator exists only during a deploy, ops is a named human,
-- backup is a scheduled task, and the app never has DDL.
--
-- ORDER MATTERS. This file creates the databases, the accounts, and only the
-- privileges that can be granted before any table exists. MySQL refuses a
-- table-level GRANT for a table that does not exist yet (ERROR 1146), so the
-- per-table privileges live in ops/04-table-grants.sql and are applied after
-- the schema. The sequence is:
--
--   1. ops/01-provision.sql     databases, users, database-level privileges
--   2. ops/migrate.sql          the schema
--   3. ops/04-table-grants.sql  per-table privileges
--   4. ops/03-verify-grants.sql check the result against the matrix

CREATE DATABASE IF NOT EXISTS kestridge
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_0900_ai_ci;

CREATE DATABASE IF NOT EXISTS kestridge_test
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_0900_ai_ci;

-- Application runtime. DML on two tables, no DDL, no other schema, nothing on
-- dsr_log. The table-level grants are in 04-table-grants.sql.
-- DELETE is deliberate: the same process runs the retention purge, so an
-- insert-only credential could not keep the published deletion promise.
CREATE USER IF NOT EXISTS 'kestridge_app'@'127.0.0.1'
  IDENTIFIED WITH caching_sha2_password BY '<APP_PASSWORD>';

-- Migrations. Held by the deploy step, never by the running process. This one
-- is database-level, so it can and must be granted before the schema exists:
-- it is the credential that creates the schema.
CREATE USER IF NOT EXISTS 'kestridge_migrator'@'127.0.0.1'
  IDENTIFIED WITH caching_sha2_password BY '<MIGRATOR_PASSWORD>';
GRANT ALL PRIVILEGES ON kestridge.*      TO 'kestridge_migrator'@'127.0.0.1';
GRANT ALL PRIVILEGES ON kestridge_test.* TO 'kestridge_migrator'@'127.0.0.1';

-- A named human doing data-subject requests and legal holds. Table-level, so
-- the grants are in 04-table-grants.sql.
CREATE USER IF NOT EXISTS 'kestridge_ops'@'127.0.0.1'
  IDENTIFIED WITH caching_sha2_password BY '<OPS_PASSWORD>';

-- mysqldump only. Database-level.
-- No PROCESS: it is a GLOBAL-only privilege, so naming it here fails the
-- whole script with ERROR 1221, and mysqldump only needs it to collect
-- tablespace information. ops/linux/backup.sh passes --no-tablespaces, so
-- it never asks for that, and granting a global privilege to a backup
-- account to work around a flag would be the wrong trade.
CREATE USER IF NOT EXISTS 'kestridge_backup'@'127.0.0.1'
  IDENTIFIED WITH caching_sha2_password BY '<BACKUP_PASSWORD>';
GRANT SELECT, LOCK TABLES, SHOW VIEW ON kestridge.* TO 'kestridge_backup'@'127.0.0.1';

-- Test runner. Owns kestridge_test outright, has nothing on kestridge.
CREATE USER IF NOT EXISTS 'kestridge_test'@'127.0.0.1'
  IDENTIFIED WITH caching_sha2_password BY '<TEST_PASSWORD>';
GRANT ALL PRIVILEGES ON kestridge_test.* TO 'kestridge_test'@'127.0.0.1';

FLUSH PRIVILEGES;
