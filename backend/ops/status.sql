-- Kestridge AI backend - what is happening right now.
-- Run as kestridge_ops:
--   mysql.exe -u kestridge_ops -p kestridge < ops/status.sql

SELECT 'submissions by notify state' AS section;
SELECT notify_state, COUNT(*) AS rows_count, MIN(created_at) AS oldest, MAX(created_at) AS newest
  FROM contact_submissions
 GROUP BY notify_state;

SELECT 'notifications still waiting' AS section;
SELECT id, created_at, service, notify_attempts, notify_next_attempt_at
  FROM contact_submissions
 WHERE notify_state = 'pending'
 ORDER BY notify_next_attempt_at
 LIMIT 20;

-- Anything here needs a human. See RUNBOOK.md, "notifications stuck".
SELECT 'dead letters (give up, need a human)' AS section;
SELECT id, created_at, service, notify_attempts, notify_error
  FROM contact_submissions
 WHERE notify_state = 'failed'
 ORDER BY created_at DESC
 LIMIT 20;

SELECT 'arriving in the last 7 days' AS section;
SELECT DATE(created_at) AS day, COUNT(*) AS rows_count
  FROM contact_submissions
 WHERE created_at >= UTC_TIMESTAMP() - INTERVAL 7 DAY
 GROUP BY DATE(created_at)
 ORDER BY day DESC;

SELECT 'retention' AS section;
SELECT COUNT(*)                                        AS total_rows,
       SUM(legal_hold = 1)                             AS on_legal_hold,
       SUM(legal_hold = 0 AND purge_after <= UTC_DATE) AS due_for_purge,
       MIN(purge_after)                                AS earliest_purge_after
  FROM contact_submissions;

SELECT 'last 10 retention runs' AS section;
SELECT id, started_at, finished_at, outcome, cutoff_date, rows_affected, duration_ms, detail
  FROM job_runs
 WHERE job_name = 'retention_purge'
 ORDER BY started_at DESC
 LIMIT 10;
