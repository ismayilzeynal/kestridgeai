-- Kestridge AI backend - one-time database provisioning.
-- Run as root. Driven by ops/setup-dev-db.ps1, which generates the passwords
-- and substitutes them; the committed file carries placeholders only.
--
-- The boundaries that actually hold here are holder and lifetime, not privilege
-- arithmetic: migrator exists only during a deploy, ops is a named human,
-- backup is a scheduled task, and the app never has DDL.

CREATE DATABASE IF NOT EXISTS kestridge
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_0900_ai_ci;

CREATE DATABASE IF NOT EXISTS kestridge_test
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_0900_ai_ci;

-- Application runtime. DML on the three tables, no DDL, no other schema.
-- DELETE is deliberate: the same process runs the retention purge, so an
-- insert-only credential could not keep the published deletion promise.
CREATE USER IF NOT EXISTS 'kestridge_app'@'127.0.0.1'
  IDENTIFIED WITH caching_sha2_password BY '<APP_PASSWORD>';
GRANT SELECT, INSERT, UPDATE, DELETE ON kestridge.contact_submissions TO 'kestridge_app'@'127.0.0.1';
GRANT SELECT, INSERT, UPDATE         ON kestridge.job_runs            TO 'kestridge_app'@'127.0.0.1';

-- Migrations. Held by the deploy step, never by the running process.
CREATE USER IF NOT EXISTS 'kestridge_migrator'@'127.0.0.1'
  IDENTIFIED WITH caching_sha2_password BY '<MIGRATOR_PASSWORD>';
GRANT ALL PRIVILEGES ON kestridge.*      TO 'kestridge_migrator'@'127.0.0.1';
GRANT ALL PRIVILEGES ON kestridge_test.* TO 'kestridge_migrator'@'127.0.0.1';

-- A named human doing data-subject requests and legal holds.
CREATE USER IF NOT EXISTS 'kestridge_ops'@'127.0.0.1'
  IDENTIFIED WITH caching_sha2_password BY '<OPS_PASSWORD>';
GRANT SELECT, UPDATE, DELETE ON kestridge.contact_submissions TO 'kestridge_ops'@'127.0.0.1';
GRANT SELECT                 ON kestridge.job_runs            TO 'kestridge_ops'@'127.0.0.1';
GRANT SELECT, INSERT, UPDATE ON kestridge.dsr_log             TO 'kestridge_ops'@'127.0.0.1';

-- mysqldump only.
CREATE USER IF NOT EXISTS 'kestridge_backup'@'127.0.0.1'
  IDENTIFIED WITH caching_sha2_password BY '<BACKUP_PASSWORD>';
GRANT SELECT, LOCK TABLES, SHOW VIEW, PROCESS ON kestridge.* TO 'kestridge_backup'@'127.0.0.1';

-- Test runner. Owns kestridge_test outright, has nothing on kestridge.
CREATE USER IF NOT EXISTS 'kestridge_test'@'127.0.0.1'
  IDENTIFIED WITH caching_sha2_password BY '<TEST_PASSWORD>';
GRANT ALL PRIVILEGES ON kestridge_test.* TO 'kestridge_test'@'127.0.0.1';

FLUSH PRIVILEGES;
