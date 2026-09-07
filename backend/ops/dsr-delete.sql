-- Kestridge AI backend - data subject deletion request.
-- A hard DELETE, not a soft-delete flag: a flag is not deletion, and the
-- Privacy Policy promises deletion or anonymization.
--
-- Run as kestridge_ops. Order of operations:
--   1. Run ops/dsr-export.sql first and keep the id list. Once these rows are
--      gone there is no record of what was removed except dsr_log.
--   2. Run the SELECT below and confirm the count with the requester.
--   3. Run the DELETE.
--   4. Run ops/dsr-log.sql with the id list.
--
-- Deleting the submission also removes any pending notification for it, because
-- the notification state lives on the row.

-- The address below is chosen by the data subject, and the validator accepts an
-- apostrophe in the local part, so it must NOT be pasted into a quoted literal:
-- o'brien@example.com is a syntax error, and a crafted address can turn the
-- lookup into a constant that matches nothing while every statement reports
-- success. Generate the hex form first, which has no delimiter to break out of:
--
--   powershell -File ops/dsr-hex.ps1 "person@example.com"
--
-- Paste the line it prints in place of the SET below, then run SELECT @subject;
-- and confirm it prints the address exactly before going further.
SET @subject = LOWER(CONVERT(0x5245504c414345 USING utf8mb4));  -- placeholder, replace with ops/dsr-hex.ps1 output
SELECT @subject AS subject_as_parsed;

-- Step 2. Look before deleting.
SELECT id, created_at, service, legal_hold
  FROM contact_submissions
 WHERE email = @subject;

-- A row on legal hold is not deletable while the hold stands. Tell the
-- requester it is retained under a legal obligation, and why.
SELECT COUNT(*) AS blocked_by_legal_hold
  FROM contact_submissions
 WHERE email = @subject AND legal_hold = 1;

-- Step 3. Uncomment to run.
-- DELETE FROM contact_submissions WHERE email = @subject AND legal_hold = 0;

-- Step 3b. Confirm.
SELECT COUNT(*) AS remaining FROM contact_submissions WHERE email = @subject;
