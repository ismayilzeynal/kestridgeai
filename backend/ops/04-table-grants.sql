-- Kestridge AI backend - per-table privileges.
--
-- Run as root, AFTER ops/migrate.sql has created the tables. MySQL refuses a
-- table-level GRANT for a table that does not exist (ERROR 1146), which is why
-- these are not in 01-provision.sql.
--
-- Idempotent: re-granting a privilege that is already held is a no-op.
--
-- These grants are the security boundary the design actually rests on. It has
-- moved twice. When the admin panel gained a DSR delete, the app began writing
-- dsr_log. When the panel gained user management, the app began creating
-- accounts: it now holds INSERT on admin_accounts and UPDATE on totp_secret.
--
-- What must still hold for kestridge_app:
--   - it can never change or erase a dsr_log entry it wrote;
--   - it can never rewrite a password hash, rename an operator, re-enable a
--     disabled account, or delete an account row;
--   - it holds nothing on the schema.
--
-- What it can do, stated plainly: insert a new account, and clear or set an
-- account's TOTP secret, which the authenticator reset cannot work without. A
-- secret it sets is useless without the account's password, and that hash it
-- cannot change. The fresh code the panel asks the operator for and the mail it
-- sends to the team are application guards, not database ones.
--
-- No triggers. They cannot run inside the procedure wrappers of the idempotent
-- migrate.sql, need SUPER with binary logging on, and are left out of the
-- Windows backup, so a restore would silently drop them. Grants have none of
-- those problems.

-- Application runtime.
GRANT SELECT, INSERT, UPDATE, DELETE ON kestridge.contact_submissions TO 'kestridge_app'@'127.0.0.1';
GRANT SELECT, INSERT, UPDATE         ON kestridge.job_runs            TO 'kestridge_app'@'127.0.0.1';

-- A named human doing data-subject requests and legal holds.
GRANT SELECT, UPDATE, DELETE ON kestridge.contact_submissions TO 'kestridge_ops'@'127.0.0.1';
GRANT SELECT                 ON kestridge.job_runs            TO 'kestridge_ops'@'127.0.0.1';
GRANT SELECT, INSERT, UPDATE ON kestridge.dsr_log             TO 'kestridge_ops'@'127.0.0.1';

-- Admin panel, running as the application account.
GRANT SELECT, INSERT, UPDATE, DELETE ON kestridge.admin_sessions TO 'kestridge_app'@'127.0.0.1';

-- INSERT, because a new user's row is written when they complete first sign-in.
-- That INSERT is the only time the app ever writes a password hash. No DELETE:
-- handled_by and dsr_log carry an operator's name, so an account that ever
-- existed stays, disabled if need be.
GRANT SELECT, INSERT                 ON kestridge.admin_accounts TO 'kestridge_app'@'127.0.0.1';

-- Column level, and this is the boundary that matters. password_hash, username,
-- display_name and disabled are not in the list, so a compromised web process
-- cannot rewrite a password hash, rename an operator, or re-enable a disabled
-- account. It fails loudly with ERROR 1143 if code ever tries. totp_secret is
-- in the list for the authenticator reset, and only for that.
--
-- Consequence for anyone editing the C# side: never call
-- db.AdminAccounts.Update(entity). That marks every property modified and EF
-- emits a full column list, which this grant refuses. Mutate tracked properties
-- and let EF emit only what changed.
GRANT UPDATE (failed_attempts, first_failed_at, locked_until, last_login_at, totp_last_step, totp_secret)
                                     ON kestridge.admin_accounts TO 'kestridge_app'@'127.0.0.1';

-- Pending first sign-ins: new users not yet in admin_accounts, and pending
-- authenticator resets. Full read and write, because these rows are invitations
-- rather than accounts, and deleting a user who never signed in is a DELETE here.
GRANT SELECT, INSERT, UPDATE, DELETE ON kestridge.admin_enrollments TO 'kestridge_app'@'127.0.0.1';

-- Panel disables. Append only, like dsr_log: the process that disabled an
-- account can never delete the row that disabled it, so a compromised one
-- cannot undo a disable. Re-enabling is a migrator operation.
GRANT SELECT, INSERT                 ON kestridge.admin_disables    TO 'kestridge_app'@'127.0.0.1';

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

-- Keep the manual runbook path able to see who has an account, who is waiting
-- for first sign-in, who was disabled from the panel, and how many sessions are
-- open.
GRANT SELECT ON kestridge.admin_accounts    TO 'kestridge_ops'@'127.0.0.1';
GRANT SELECT ON kestridge.admin_sessions    TO 'kestridge_ops'@'127.0.0.1';
GRANT SELECT ON kestridge.admin_enrollments TO 'kestridge_ops'@'127.0.0.1';
GRANT SELECT ON kestridge.admin_disables    TO 'kestridge_ops'@'127.0.0.1';

FLUSH PRIVILEGES;
