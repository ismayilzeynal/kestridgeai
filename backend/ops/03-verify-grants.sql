-- Kestridge AI backend - grant drift check. Run as root after provisioning and
-- monthly thereafter. This is what catches a grant widened to ALL during a
-- debugging session and never narrowed back.
--
-- Read the output by eye against the matrix below. Anything extra is drift.
--
--   kestridge_app        SELECT, INSERT, UPDATE, DELETE on kestridge.contact_submissions
--                        SELECT, INSERT, UPDATE         on kestridge.job_runs
--                        SELECT, INSERT                 on kestridge.dsr_log (append only)
--                        SELECT, INSERT                 on kestridge.admin_accounts
--                        UPDATE (failed_attempts, first_failed_at, locked_until,
--                                last_login_at, totp_last_step, totp_secret)
--                                                       on kestridge.admin_accounts
--                        SELECT, INSERT, UPDATE, DELETE on kestridge.admin_sessions
--                        SELECT, INSERT, UPDATE, DELETE on kestridge.admin_enrollments
--                        SELECT, INSERT                 on kestridge.admin_disables (append only)
--                        SELECT, INSERT, UPDATE, DELETE on the six kestridge.site_* tables
--                        nothing on kestridge_test
--   kestridge_migrator   ALL on kestridge.*, ALL on kestridge_test.*
--   kestridge_ops        SELECT, UPDATE, DELETE on kestridge.contact_submissions
--                        SELECT                 on kestridge.job_runs
--                        SELECT, INSERT, UPDATE on kestridge.dsr_log
--                        SELECT                 on admin_accounts, admin_sessions,
--                                                  admin_enrollments, admin_disables, site_*
--   kestridge_backup     SELECT, LOCK TABLES, SHOW VIEW on kestridge.* (no PROCESS: global only)
--   kestridge_test       ALL on kestridge_test.* only

SHOW GRANTS FOR 'kestridge_app'@'127.0.0.1';
SHOW GRANTS FOR 'kestridge_migrator'@'127.0.0.1';
SHOW GRANTS FOR 'kestridge_ops'@'127.0.0.1';
SHOW GRANTS FOR 'kestridge_backup'@'127.0.0.1';
SHOW GRANTS FOR 'kestridge_test'@'127.0.0.1';

-- Five queries that must always return nothing. Any row here is a finding.

-- The app writes dsr_log now, because the panel performs deletions and is
-- obliged to record them. What must still hold is that it can never change or
-- erase what it wrote: append only, stricter than kestridge_ops.
SELECT 'app must not edit or erase a dsr_log entry' AS finding, GRANTEE, TABLE_NAME, PRIVILEGE_TYPE
  FROM information_schema.TABLE_PRIVILEGES
 WHERE GRANTEE LIKE '%kestridge_app%' AND TABLE_NAME = 'dsr_log'
   AND PRIVILEGE_TYPE IN ('UPDATE', 'DELETE');

-- The credential columns. The app creates accounts, so INSERT is expected here
-- and is not a finding. What it must never hold is a table-wide UPDATE, which
-- would reach password_hash, or DELETE, which would erase an operator whose
-- name handled_by and dsr_log still carry.
SELECT 'app must hold no table-wide UPDATE and no DELETE on admin_accounts' AS finding, GRANTEE, PRIVILEGE_TYPE
  FROM information_schema.TABLE_PRIVILEGES
 WHERE GRANTEE LIKE '%kestridge_app%' AND TABLE_NAME = 'admin_accounts'
   AND PRIVILEGE_TYPE IN ('UPDATE', 'DELETE');

-- Column grants appear only in COLUMN_PRIVILEGES, never in TABLE_PRIVILEGES, so
-- the query above cannot see a column added to the UPDATE list. These four are
-- the boundary: rewrite a password hash, rename an operator, re-enable an
-- account. totp_secret is expected in the list and is not a finding.
SELECT 'app must not UPDATE these admin_accounts columns' AS finding, GRANTEE, COLUMN_NAME, PRIVILEGE_TYPE
  FROM information_schema.COLUMN_PRIVILEGES
 WHERE GRANTEE LIKE '%kestridge_app%' AND TABLE_NAME = 'admin_accounts'
   AND PRIVILEGE_TYPE = 'UPDATE'
   AND COLUMN_NAME IN ('password_hash', 'username', 'display_name', 'disabled');

-- Panel disables are append only. With UPDATE or DELETE the process that wrote
-- a disable could undo it.
SELECT 'app must hold no UPDATE or DELETE on admin_disables' AS finding, GRANTEE, TABLE_NAME, PRIVILEGE_TYPE
  FROM information_schema.TABLE_PRIVILEGES
 WHERE GRANTEE LIKE '%kestridge_app%' AND TABLE_NAME = 'admin_disables'
   AND PRIVILEGE_TYPE IN ('UPDATE', 'DELETE');

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
