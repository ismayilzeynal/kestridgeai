-- Take away an operator's access. Run as kestridge_migrator, or as root.
--
-- This is the offboarding step. The panel reads every inquiry the company has
-- ever received, so an account that outlives the person is the single largest
-- standing exposure in this system.
--
-- kestridge_app cannot run this. Its UPDATE grant on admin_accounts names five
-- columns and "disabled" is not one of them, which is the same boundary that
-- stops a compromised web process re-enabling an account it just disabled.
--
-- Takes effect on the next request, not on the next login: AdminTokenFilter
-- checks Disabled on every call, so an open browser tab stops working within
-- one click. The session delete below is therefore belt and braces, not the
-- mechanism.

UPDATE admin_accounts
   SET disabled = 1
 WHERE username = 'REPLACE_WITH_USERNAME';

DELETE s
  FROM admin_sessions s
  JOIN admin_accounts a ON a.id = s.account_id
 WHERE a.username = 'REPLACE_WITH_USERNAME';

-- Expect disabled = 1 and no session rows for that username.
SELECT a.id, a.username, a.display_name, a.disabled, COUNT(s.token_hash) AS open_sessions
  FROM admin_accounts a
  LEFT JOIN admin_sessions s ON s.account_id = a.id
 GROUP BY a.id, a.username, a.display_name, a.disabled;

-- Two things this does NOT do, deliberately.
--
-- It does not delete the row. handled_by on contact_submissions and handled_by
-- on dsr_log carry the display name of whoever acted, and a deletion request
-- answered last year has to stay attributable to a real person.
--
-- It does not touch that person's mail. If notifications authenticate as their
-- Zoho account, see RUNBOOK.md: revoking their app password stops every
-- notification the site produces.
