-- Take away an operator's access by SQL. Run as kestridge_migrator, or as root.
--
-- This is the offboarding step. The panel reads every inquiry the company has
-- ever received, so an account that outlives the person is the single largest
-- standing exposure in this system.
--
-- The normal way is "Disable" in the panel's Users tab, which any other operator
-- can click. The panel does not touch the column below: kestridge_app holds no
-- UPDATE on "disabled", so it appends a row to admin_disables instead, a table
-- it can insert into and never update or delete from. Both are honoured
-- everywhere, and the process that disabled an account can undo neither. This
-- script is for when the panel is down, or nobody who is left can sign in.
--
-- Takes effect on the next request, not on the next login: AdminTokenFilter
-- checks both on every call, so an open browser tab stops working within
-- one click. The session delete below is therefore belt and braces, not the
-- mechanism. So is the first enrolment delete: a pending authenticator reset
-- for a disabled account is refused anyway, and the panel removes it the same
-- way.
--
-- The second enrolment delete is not belt and braces. A new user's invitation
-- that this person created from the panel carries a temporary password they
-- chose and may still know, and completing it creates a new account that
-- nothing about this one reaches. The panel's Disable deletes those too.
--
-- What neither can find is an account this person created that has already
-- finished its first sign-in: nothing in the database records its creator once
-- the invitation is gone. Go through the "account created" mails that name this
-- person under "Created by", and disable any account nobody else can vouch for.

UPDATE admin_accounts
   SET disabled = 1
 WHERE username = 'REPLACE_WITH_USERNAME';

DELETE s
  FROM admin_sessions s
  JOIN admin_accounts a ON a.id = s.account_id
 WHERE a.username = 'REPLACE_WITH_USERNAME';

DELETE e
  FROM admin_enrollments e
  JOIN admin_accounts a ON a.id = e.account_id
 WHERE a.username = 'REPLACE_WITH_USERNAME';

DELETE e
  FROM admin_enrollments e
  JOIN admin_accounts a ON a.id = e.created_by_account_id
 WHERE e.account_id IS NULL
   AND a.username = 'REPLACE_WITH_USERNAME';

-- Expect disabled = 1, no session rows and no invitations created by that
-- username. panel_disabled is 1 for an account that was also disabled from the
-- panel; this script leaves that row alone. created_at is there for the review
-- of accounts this person created.
SELECT a.id, a.username, a.display_name, a.disabled,
       d.account_id IS NOT NULL AS panel_disabled,
       COUNT(s.token_hash) AS open_sessions,
       (SELECT COUNT(*) FROM admin_enrollments e
         WHERE e.account_id IS NULL AND e.created_by_account_id = a.id) AS invitations_created,
       a.created_at
  FROM admin_accounts a
  LEFT JOIN admin_disables d ON d.account_id = a.id
  LEFT JOIN admin_sessions s ON s.account_id = a.id
 GROUP BY a.id, a.username, a.display_name, a.disabled, d.account_id, a.created_at
 ORDER BY a.created_at;

-- Re-enabling is a migrator operation, and it has two halves. Clear only the
-- column and a panel disable still keeps the account out; delete only the row
-- and a SQL disable does.
--
--   UPDATE admin_accounts SET disabled = 0 WHERE username = 'REPLACE_WITH_USERNAME';
--   DELETE d FROM admin_disables d JOIN admin_accounts a ON a.id = d.account_id
--    WHERE a.username = 'REPLACE_WITH_USERNAME';

-- Two things this does NOT do, deliberately.
--
-- It does not delete the row. handled_by on contact_submissions and handled_by
-- on dsr_log carry the display name of whoever acted, and a deletion request
-- answered last year has to stay attributable to a real person. The panel
-- cannot delete it either: kestridge_app holds no DELETE on admin_accounts.
--
-- It does not touch that person's mail. If notifications authenticate as their
-- Zoho account, see RUNBOOK.md: revoking their app password stops every
-- notification the site produces, and every admin account notice mail too.
