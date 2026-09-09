-- Create or reset an admin account. Run as kestridge_migrator.
-- Generate the values first, on the server, and never on the command line:
--   cd /srv/kestridge-api && dotnet Kestridge.Api.dll --hash-password --username faig --display-name "Faig Garayev"
--
-- Through dotnet, not as ./Kestridge.Api. 03-deploy.sh chmods every file in
-- the install directory to 0640, so nothing there is executable, and the
-- unit starts the app the same way. That is deliberate: the apphost never
-- needs the bit, and not having it means a writable app directory cannot
-- become a way to run something.
-- Paste what it prints over the placeholder below, run this, then clear the
-- scrollback. kestridge_app cannot write these columns, which is the point.

-- REPLACE THIS LINE with the INSERT the CLI printed.

-- Password reset for an existing account:
-- UPDATE admin_accounts SET password_hash = '<from the CLI>' WHERE username = 'faig';

-- Rotating a TOTP secret also resets replay state, or the new codes are refused
-- until the step counter catches up:
-- UPDATE admin_accounts SET totp_secret = '<from the CLI>', totp_last_step = 0 WHERE username = 'faig';

-- Revoke access without deleting history:
-- UPDATE admin_accounts SET disabled = 1 WHERE username = 'faig';
-- DELETE FROM admin_sessions WHERE account_id = (SELECT id FROM admin_accounts WHERE username = 'faig');

SELECT id, username, display_name, disabled, failed_attempts, locked_until, last_login_at
  FROM admin_accounts ORDER BY id;
