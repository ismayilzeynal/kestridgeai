-- Create or repair an admin account by SQL. Run as kestridge_migrator.
--
-- The panel creates users now, in the Users tab, and resets authenticators and
-- disables accounts there too. This file is for what the panel cannot do by
-- construction: the first account, when nobody exists yet to sign in and create
-- one; recovery, when nobody who is left can sign in; and the changes
-- kestridge_app holds no grant for at all, which are a new password hash, a new
-- username or display name, and re-enabling an account. RUNBOOK.md has the
-- whole lifecycle.
--
-- Generate the values first, on the server, and never on the command line:
--   cd /srv/kestridge-api && dotnet Kestridge.Api.dll --hash-password --username faig --display-name "Faig Garayev"
--
-- Through dotnet, not as ./Kestridge.Api. 03-deploy.sh chmods every file in
-- the install directory to 0640, so nothing there is executable, and the
-- unit starts the app the same way. That is deliberate: the apphost never
-- needs the bit, and not having it means a writable app directory cannot
-- become a way to run something.
-- Paste what it prints over the placeholder below, run this, then clear the
-- scrollback.

-- REPLACE THIS LINE with the INSERT the CLI printed.

-- Password reset for an existing account. Nothing else can do this:
-- kestridge_app holds no UPDATE on password_hash, because a password is written
-- once, by the INSERT that creates the account. Update the row, never delete it
-- and insert a new one: the new row gets a new id, and a panel disable recorded
-- in admin_disables against the old id would silently stop applying.
-- UPDATE admin_accounts SET password_hash = '<from the CLI>' WHERE username = 'faig';

-- A new TOTP secret, for when nobody else can sign in to press "Reset
-- authenticator" in the panel, which is the normal way. totp_last_step goes back
-- to 0 with it. Replay state belongs to a secret: no step the old secret spent
-- can be replayed against the new one, and the first sign-in with the new
-- secret sets the step again. Left as it was, a value pushed far ahead, by a
-- compromised process or by anything else that wrote the row, would refuse
-- every code the new secret ever produces. A panel reset starts it afresh too:
-- finishing the new setup stores the step of its first code, whatever was
-- there before. The deletes end any session whoever holds the lost phone may
-- still have open, and any reset the panel left pending.
-- UPDATE admin_accounts SET totp_secret = '<from the CLI>', totp_last_step = 0 WHERE username = 'faig';
-- DELETE FROM admin_sessions WHERE account_id = (SELECT id FROM admin_accounts WHERE username = 'faig');
-- DELETE FROM admin_enrollments WHERE account_id = (SELECT id FROM admin_accounts WHERE username = 'faig');

-- Revoke access without deleting history. The panel's Disable does the same by
-- writing admin_disables instead; this is the path for when the panel is down
-- or nobody who is left can sign in. ops/admin-disable.sql is the same thing
-- with a check at the end. The last delete removes the new-user invitations the
-- person created from the panel: they chose those temporary passwords, and
-- completing one would give them a new account this disable does not reach.
-- UPDATE admin_accounts SET disabled = 1 WHERE username = 'faig';
-- DELETE FROM admin_sessions WHERE account_id = (SELECT id FROM admin_accounts WHERE username = 'faig');
-- DELETE FROM admin_enrollments WHERE account_id = (SELECT id FROM admin_accounts WHERE username = 'faig');
-- DELETE FROM admin_enrollments WHERE account_id IS NULL AND created_by_account_id = (SELECT id FROM admin_accounts WHERE username = 'faig');

-- Re-enable. Both halves, whichever way the account was disabled: the column is
-- what SQL sets, the admin_disables row is what the panel writes, and either one
-- alone keeps the account out. kestridge_app can undo neither, which is the point.
-- UPDATE admin_accounts SET disabled = 0 WHERE username = 'faig';
-- DELETE FROM admin_disables WHERE account_id = (SELECT id FROM admin_accounts WHERE username = 'faig');

-- Who has an account, and in what state. has_authenticator is 0 after a panel
-- reset, until the owner finishes setting up again. The secret itself is never
-- selected.
SELECT a.id, a.username, a.display_name, a.disabled,
       d.disabled_at AS panel_disabled_at, d.disabled_by AS panel_disabled_by,
       a.totp_secret <> '' AS has_authenticator,
       a.failed_attempts, a.locked_until, a.last_login_at
  FROM admin_accounts a
  LEFT JOIN admin_disables d ON d.account_id = a.id
 ORDER BY a.id;

-- Who is waiting: a new user's first sign-in when account_id is NULL, an
-- authenticator reset when it is set. A row past expires_at can no longer be
-- completed. created_by_account_id is the operator who created or reset it in
-- the panel, and NULL for a row that did not come from the panel.
SELECT id, account_id, username, display_name, created_by, created_by_account_id,
       created_at, expires_at, failed_attempts, locked_until
  FROM admin_enrollments
 ORDER BY id;
