-- Clear a lockout. Run as kestridge_migrator.
--
-- An account locks for 15 minutes after 5 failed attempts. Anyone who knows a
-- username can keep it locked, so this is the documented way back in rather
-- than a reason to weaken the lockout.
--
-- A user created in the panel who has not finished their first sign-in has no
-- row in admin_accounts yet. Their counters are on the invitation, and the
-- UPDATE below does not reach them. Wait out the lock, delete the invitation in
-- the panel and create it again, or run this instead:
--
--   UPDATE admin_enrollments
--      SET failed_attempts = 0, first_failed_at = NULL, locked_until = NULL
--    WHERE username = 'REPLACE_WITH_USERNAME';

UPDATE admin_accounts
   SET failed_attempts = 0, first_failed_at = NULL, locked_until = NULL
 WHERE username = 'REPLACE_WITH_USERNAME';

SELECT username, failed_attempts, locked_until FROM admin_accounts;
