-- Kestridge AI backend - data subject access and portability request.
-- Answers "give me a copy of everything you hold about me" in a portable
-- format, which is what the Privacy Policy promises.
--
-- Run as kestridge_ops. See the note below on how to set the address.
--
-- The email column is accent-sensitive and case-sensitive by design, and the
-- application lowercases on write, so the lookup must lowercase too and must
-- not be loosened to a LIKE.

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

SELECT COALESCE(
         JSON_PRETTY(JSON_ARRAYAGG(JSON_OBJECT(
           'id',           id,
           'received_utc', DATE_FORMAT(created_at, '%Y-%m-%dT%H:%i:%sZ'),
           'name',         name,
           'email',        email,
           'company',      company,
           'phone',        phone,
           'service',      service,
           'message',      message,
           'deleted_after', purge_after
         ))),
         JSON_ARRAY()
       ) AS export_json
  FROM contact_submissions
 WHERE email = @subject;

-- The ids to record in dsr_log.affected_ids.
SELECT GROUP_CONCAT(id ORDER BY id) AS affected_ids, COUNT(*) AS rows_found
  FROM contact_submissions
 WHERE email = @subject;
