-- Clear a lockout. Run as kestridge_migrator.
--
-- An account locks for 15 minutes after 5 failed attempts. Anyone who knows a
-- username can keep it locked, so this is the documented way back in rather
-- than a reason to weaken the lockout.

UPDATE admin_accounts
   SET failed_attempts = 0, first_failed_at = NULL, locked_until = NULL
 WHERE username = 'REPLACE_WITH_USERNAME';

SELECT username, failed_attempts, locked_until FROM admin_accounts;
