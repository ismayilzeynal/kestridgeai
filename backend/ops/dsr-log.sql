-- Kestridge AI backend - record that a rights request was handled.
--
-- The subject address is stored as a peppered SHA-256, never in plaintext:
-- writing the address into a deletion log would undo the deletion the log
-- records. The pepper stops anyone confirming, from the log alone, that a
-- guessed address ever wrote in.
--
-- The pepper is the same value as Kestridge__Dsr__EmailHashPepper. Read it from
-- appsettings.Production.json on the server, or the team password manager. Do not paste it into a
-- file, and do not record the requester's message text here.
--
-- Run as kestridge_ops.

SET @pepper  = 'REPLACE_WITH_Kestridge__Dsr__EmailHashPepper';
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

INSERT INTO dsr_log (received_on, request_type, subject_email_hash, rows_affected, affected_ids, handled_by, closed_on)
VALUES (
  '2026-01-01',                                   -- date the request arrived
  'delete',                                       -- access | export | delete | correct | object
  SHA2(CONCAT(@pepper, @subject), 256),
  0,                                              -- rows exported or deleted
  '',                                             -- comma-separated contact_submissions.id values
  'REPLACE_WITH_YOUR_NAME',
  NULL                                            -- date the request was closed
);

-- Everything handled in the last 12 months. Non-retaliation: never blocklist an
-- address that made a request.
SELECT id, received_on, request_type, rows_affected, affected_ids, handled_by, closed_on
  FROM dsr_log
 WHERE received_on >= UTC_DATE - INTERVAL 12 MONTH
 ORDER BY received_on DESC;

-- Has this subject asked before? Answers without ever storing the address.
SELECT COUNT(*) AS prior_requests
  FROM dsr_log
 WHERE subject_email_hash = SHA2(CONCAT(@pepper, @subject), 256);
