-- Kestridge AI backend - put dead-lettered notifications back in the queue.
--
-- Run this after fixing the cause, never before. A notification reaches
-- notify_state = 'failed' in two ways:
--   1. Seven transient failures over about 41 hours (a long provider outage).
--   2. One permanent rejection, typically a 5xx from the recipient server.
--
-- Case 2 is the likely one while kestridge.com has no MX record: the address
-- the site publishes does not resolve, so every notification hard-bounces.
-- Re-arming before the mailbox exists just bounces them again and gets the
-- sending IP throttled.
--
-- The submissions themselves are never lost by a failed notification. They are
-- in contact_submissions and are readable with ops/status.sql.
--
-- Run as kestridge_ops.

-- Look first.
SELECT id, created_at, service, notify_attempts, notify_error
  FROM contact_submissions
 WHERE notify_state = 'failed'
 ORDER BY created_at;

-- Then re-arm. The sweep picks these up within one tick (30 seconds).
-- UPDATE contact_submissions
--    SET notify_state           = 'pending',
--        notify_attempts        = 0,
--        notify_next_attempt_at = UTC_TIMESTAMP(6),
--        notify_error           = ''
--  WHERE notify_state = 'failed';

SELECT COUNT(*) AS still_failed FROM contact_submissions WHERE notify_state = 'failed';
