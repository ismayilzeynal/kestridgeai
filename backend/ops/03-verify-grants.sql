-- Kestridge AI backend - grant drift check. Run as root after provisioning and
-- monthly thereafter. This is what catches a grant widened to ALL during a
-- debugging session and never narrowed back.
--
-- Read the output by eye against the matrix below. Anything extra is drift.
--
--   kestridge_app        SELECT, INSERT, UPDATE, DELETE on kestridge.contact_submissions
--                        SELECT, INSERT, UPDATE         on kestridge.job_runs
--                        SELECT, INSERT                 on kestridge.dsr_log (append only)
--                        SELECT                         on kestridge.admin_accounts
--                        UPDATE (failed_attempts, first_failed_at, locked_until,
--                                last_login_at, totp_last_step) on kestridge.admin_accounts
--                        SELECT, INSERT, UPDATE, DELETE on kestridge.admin_sessions
--                        SELECT, INSERT, UPDATE, DELETE on the six kestridge.site_* tables
--                        nothing on kestridge_test
--   kestridge_migrator   ALL on kestridge.*, ALL on kestridge_test.*
--   kestridge_ops        SELECT, UPDATE, DELETE on kestridge.contact_submissions
--                        SELECT                 on kestridge.job_runs
--                        SELECT, INSERT, UPDATE on kestridge.dsr_log
--                        SELECT                 on admin_accounts, admin_sessions, site_*
--   kestridge_backup     SELECT, LOCK TABLES, SHOW VIEW on kestridge.* (no PROCESS: global only)
--   kestridge_test       ALL on kestridge_test.* only

SHOW GRANTS FOR 'kestridge_app'@'127.0.0.1';
SHOW GRANTS FOR 'kestridge_migrator'@'127.0.0.1';
SHOW GRANTS FOR 'kestridge_ops'@'127.0.0.1';
SHOW GRANTS FOR 'kestridge_backup'@'127.0.0.1';
SHOW GRANTS FOR 'kestridge_test'@'127.0.0.1';

-- Two things that must always be empty. Any row here is a finding.

-- The app writes dsr_log now, because the panel performs deletions and is
-- obliged to record them. What must still hold is that it can never change or
-- erase what it wrote: append only, stricter than kestridge_ops.
SELECT 'app must not edit or erase a dsr_log entry' AS finding, GRANTEE, TABLE_NAME, PRIVILEGE_TYPE
  FROM information_schema.TABLE_PRIVILEGES
 WHERE GRANTEE LIKE '%kestridge_app%' AND TABLE_NAME = 'dsr_log'
   AND PRIVILEGE_TYPE IN ('UPDATE', 'DELETE');

-- The credential columns. A compromised web process must not be able to
-- rewrite a password hash, swap a TOTP secret, or re-enable a disabled account.
SELECT 'app must hold no table-wide UPDATE on admin_accounts' AS finding, GRANTEE, PRIVILEGE_TYPE
  FROM information_schema.TABLE_PRIVILEGES
 WHERE GRANTEE LIKE '%kestridge_app%' AND TABLE_NAME = 'admin_accounts'
   AND PRIVILEGE_TYPE IN ('UPDATE', 'INSERT', 'DELETE');

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
