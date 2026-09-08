-- Kestridge AI backend - grant drift check. Run as root after provisioning and
-- monthly thereafter. This is what catches a grant widened to ALL during a
-- debugging session and never narrowed back.
--
-- Read the output by eye against the matrix below. Anything extra is drift.
--
--   kestridge_app        SELECT, INSERT, UPDATE, DELETE on kestridge.contact_submissions
--                        SELECT, INSERT, UPDATE         on kestridge.job_runs
--                        nothing on kestridge.dsr_log, nothing on kestridge_test
--   kestridge_migrator   ALL on kestridge.*, ALL on kestridge_test.*
--   kestridge_ops        SELECT, UPDATE, DELETE on kestridge.contact_submissions
--                        SELECT                 on kestridge.job_runs
--                        SELECT, INSERT, UPDATE on kestridge.dsr_log
--   kestridge_backup     SELECT, LOCK TABLES, SHOW VIEW on kestridge.* (no PROCESS: global only)
--   kestridge_test       ALL on kestridge_test.* only

SHOW GRANTS FOR 'kestridge_app'@'127.0.0.1';
SHOW GRANTS FOR 'kestridge_migrator'@'127.0.0.1';
SHOW GRANTS FOR 'kestridge_ops'@'127.0.0.1';
SHOW GRANTS FOR 'kestridge_backup'@'127.0.0.1';
SHOW GRANTS FOR 'kestridge_test'@'127.0.0.1';

-- Two things that must always be empty. Any row here is a finding.

SELECT 'app must not reach dsr_log' AS finding, GRANTEE, TABLE_NAME, PRIVILEGE_TYPE
  FROM information_schema.TABLE_PRIVILEGES
 WHERE GRANTEE LIKE '%kestridge_app%' AND TABLE_NAME = 'dsr_log';

SELECT 'app must hold no schema-wide grant' AS finding, GRANTEE, TABLE_SCHEMA, PRIVILEGE_TYPE
  FROM information_schema.SCHEMA_PRIVILEGES
 WHERE GRANTEE LIKE '%kestridge_app%';

-- The server settings the application depends on. Compare against 02-hardening.cnf.
SELECT @@sql_mode                   AS sql_mode,
       @@character_set_server       AS charset,
       @@collation_server           AS collation,
       @@general_log                AS general_log,
       @@log_bin                    AS log_bin,
       @@binlog_expire_logs_seconds AS binlog_expire_seconds,
       @@innodb_default_row_format  AS row_format;
