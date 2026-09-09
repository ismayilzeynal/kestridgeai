-- Kestridge AI backend - per-table privileges.
--
-- Run as root, AFTER ops/migrate.sql has created the tables. MySQL refuses a
-- table-level GRANT for a table that does not exist (ERROR 1146), which is why
-- these are not in 01-provision.sql.
--
-- Idempotent: re-granting a privilege that is already held is a no-op.
--
-- These grants are the security boundary the design actually rests on. It moved
-- when the admin panel gained a DSR delete: the app now writes dsr_log, which it
-- used to have nothing on. What must still hold is that it can never change or
-- erase what it wrote, that it holds no privilege over the credential columns of
-- admin_accounts, and that neither account can touch the schema.

-- Application runtime.
GRANT SELECT, INSERT, UPDATE, DELETE ON kestridge.contact_submissions TO 'kestridge_app'@'127.0.0.1';
GRANT SELECT, INSERT, UPDATE         ON kestridge.job_runs            TO 'kestridge_app'@'127.0.0.1';

-- A named human doing data-subject requests and legal holds.
GRANT SELECT, UPDATE, DELETE ON kestridge.contact_submissions TO 'kestridge_ops'@'127.0.0.1';
GRANT SELECT                 ON kestridge.job_runs            TO 'kestridge_ops'@'127.0.0.1';
GRANT SELECT, INSERT, UPDATE ON kestridge.dsr_log             TO 'kestridge_ops'@'127.0.0.1';

-- Admin panel, running as the application account.
GRANT SELECT, INSERT, UPDATE, DELETE ON kestridge.admin_sessions TO 'kestridge_app'@'127.0.0.1';
GRANT SELECT                         ON kestridge.admin_accounts TO 'kestridge_app'@'127.0.0.1';

-- Column level, and this is the boundary that matters. A compromised web
-- process cannot rewrite a password hash, swap a TOTP secret, rename an
-- operator, or re-enable a disabled account. It fails loudly with ERROR 1143 if
-- code ever tries.
--
-- Consequence for anyone editing the C# side: never call
-- db.AdminAccounts.Update(entity). That marks every property modified and EF
-- emits a full column list, which this grant refuses. Mutate tracked properties
-- and let EF emit only what changed.
GRANT UPDATE (failed_attempts, first_failed_at, locked_until, last_login_at, totp_last_step)
                                     ON kestridge.admin_accounts TO 'kestridge_app'@'127.0.0.1';

-- Append only. The panel writes the deletion record it is obliged to write and
-- can never edit or erase one. No UPDATE and no DELETE here, deliberately:
-- that is stricter than kestridge_ops, which holds UPDATE for the manual path.
GRANT SELECT, INSERT                 ON kestridge.dsr_log        TO 'kestridge_app'@'127.0.0.1';

-- Website content. Full read and write for the panel, because editing the
-- copy is the entire feature. Nothing here is personal data: every row is
-- already shown to every visitor of kestridge.com.
GRANT SELECT, INSERT, UPDATE, DELETE ON kestridge.site_faq                TO 'kestridge_app'@'127.0.0.1';
GRANT SELECT, INSERT, UPDATE, DELETE ON kestridge.site_team               TO 'kestridge_app'@'127.0.0.1';
GRANT SELECT, INSERT, UPDATE, DELETE ON kestridge.site_companies          TO 'kestridge_app'@'127.0.0.1';
GRANT SELECT, INSERT, UPDATE, DELETE ON kestridge.site_services           TO 'kestridge_app'@'127.0.0.1';
GRANT SELECT, INSERT, UPDATE, DELETE ON kestridge.site_service_steps      TO 'kestridge_app'@'127.0.0.1';
GRANT SELECT, INSERT, UPDATE, DELETE ON kestridge.site_service_highlights TO 'kestridge_app'@'127.0.0.1';

-- Read only for the human path, so a founder can answer "what does the site
-- say right now" from mysql without going through the panel.
GRANT SELECT ON kestridge.site_faq                TO 'kestridge_ops'@'127.0.0.1';
GRANT SELECT ON kestridge.site_team               TO 'kestridge_ops'@'127.0.0.1';
GRANT SELECT ON kestridge.site_companies          TO 'kestridge_ops'@'127.0.0.1';
GRANT SELECT ON kestridge.site_services           TO 'kestridge_ops'@'127.0.0.1';
GRANT SELECT ON kestridge.site_service_steps      TO 'kestridge_ops'@'127.0.0.1';
GRANT SELECT ON kestridge.site_service_highlights TO 'kestridge_ops'@'127.0.0.1';

-- Keep the manual runbook path able to see who has an account and how many
-- sessions are open.
GRANT SELECT ON kestridge.admin_accounts TO 'kestridge_ops'@'127.0.0.1';
GRANT SELECT ON kestridge.admin_sessions TO 'kestridge_ops'@'127.0.0.1';

FLUSH PRIVILEGES;
