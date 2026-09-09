# Data subject requests

The Privacy Policy at `kestridge.com/privacy` tells people they can ask for
access, correction, deletion, objection, and a portable copy, by writing to
`info@kestridge.com`. This is how those requests get handled once inquiries live
in a database.

Expected volume is one or two a year.

There are two routes. The SQL scripts in `ops/` are the process of record and
the fallback when the API is down. The admin panel at
`https://api.kestridge.com/admin/` is the second route, and it covers access,
correction and deletion end to end without an SSH session.

Both write `dsr_log`, with the identical peppered hash, so `prior_requests`
is correct whichever route was used. Getting that hash wrong in either
direction is silent and permanent, which is why a test asserts the exact bytes.

The panel sets **no cookie**. It holds an opaque bearer token in
`sessionStorage`, which dies with the tab, and it is served from
`api.kestridge.com` rather than `kestridge.com/admin` precisely because
`sessionStorage` is scoped per origin and not per path: the second would put
the admin token on the same origin as a page that loads Google Analytics on
consent. Privacy Policy section 3 is unaffected.

## Who watches the mailbox

```
Owner:  TO BE ASSIGNED
Backup: TO BE ASSIGNED
```

`info@kestridge.com` is a real mailbox as of 10 September 2026. The zone
carries Zoho's MX records and an SPF record, and delivery to it was tested by
hand. The channel the Privacy Policy publishes finally works, which is what
makes every deadline in this document meaningful: a request nobody can send is
a request nobody has to answer within 30 days.

Zoho Mail is therefore a **subprocessor** with access to the body of every
inquiry that is mailed to that address. Sign the DPA and add it to the register
before treating this as done. Its data centre is the US one, which matches
where the company and the database already are.

## Service level

Answer within **30 days** of receipt. That satisfies both the GDPR 30-day clock
and the CCPA 45-day one, so there is one number to remember.

**Never treat anyone differently for exercising a privacy right.** Do not
blocklist an address that made a request, and do not stop replying to their
inquiry.

## Verifying the requester

Answer only to the address on the submission, or make the requester identify the
submission (roughly when they wrote in, and what about). Do not create an
account system, and do not ask for a government ID: the policy explicitly tells
people not to send identity documents.

If the address does not match any row, say so plainly. "We hold nothing for this
address" is a complete and correct answer.

## What is held

One table, `contact_submissions`, one row per inquiry:

| Column | Content |
| --- | --- |
| `created_at` | when it arrived, UTC |
| `name`, `email`, `company`, `phone` | as typed by the sender |
| `service` | one of `ai`, `analytics`, `automation`, `security`, `general` |
| `message` | the free-text description |
| `purge_after` | the scheduled deletion date, `created_at` plus 24 months |
| `legal_hold` | set by hand when a row must survive the purge |
| `notify_*` | delivery state of the internal notification |
| `dedupe_key` | a SHA-256 used to suppress double submissions |

**No IP address, user agent or referer is stored**, by design. `SchemaTests`
asserts those columns do not exist.

Copies also exist in the notification email in the team mailbox, and in nightly
backups. Both are covered below.

## Setting the subject address, safely

Every script below addresses the subject through `@subject`. **Never paste the
address into a quoted SQL literal.** The address is chosen by the data subject
and the validator accepts an apostrophe in the local part, so
`o'brien@example.com` is a syntax error, and a crafted address such as
`a'/**/or/**/1=1/**/'b@c.d` turns the lookup into a constant: every statement
then matches zero rows and reports success, the requester's data is neither
exported nor deleted, and the log records the request as handled.

Generate the hex form instead. A hex literal has no delimiter to break out of:

```powershell
powershell -ExecutionPolicy Bypass -File ops/dsr-hex.ps1 "person@example.com"
```

Paste the `SET @subject = ...` line it prints into the script, run
`SELECT @subject;`, and confirm it prints the address exactly before going
further.

## Access or portability request

```powershell
mysql.exe -u kestridge_ops -p kestridge < ops/dsr-export.sql
```

Set `@subject` as above first. It emits pretty-printed JSON, which is a portable
format, plus the id list to record in the log.

## Deletion request

```powershell
mysql.exe -u kestridge_ops -p kestridge < ops/dsr-delete.sql
```

Order matters:

1. Run `ops/dsr-export.sql` first and keep the id list. Once the rows are gone
   there is no record of what was removed except `dsr_log`.
2. Run the SELECT in `dsr-delete.sql` and confirm the count with the requester.
3. Uncomment and run the DELETE. It is a hard `DELETE`, not a flag: a flag is
   not deletion, and the policy promises deletion or anonymization.
4. Record it with `ops/dsr-log.sql`.

Deleting the row also removes any pending notification for it, because the
notification state lives on the row.

A row with `legal_hold = 1` is not deleted. Tell the requester it is retained
under a legal obligation, and say which.

## Recording the request

```powershell
mysql.exe -u kestridge_ops -p kestridge < ops/dsr-log.sql
```

The subject address is stored as `SHA2(CONCAT(pepper, LOWER(email)), 256)`,
never in plaintext. Writing the address into a deletion log would undo the
deletion the log records; the pepper stops anyone confirming from the log alone
that a guessed address ever wrote in. The pepper is
`Kestridge:Dsr:EmailHashPepper`, held in the ACLed `appsettings.Production.json`
and the team password manager.

Never record the requester's message text in `dsr_log`.

## Windows where deleted data can still exist

Tell requesters this when it applies. These are the only copies.

| Copy | Window | Notes |
| --- | --- | --- |
| Exported CSV | until the operator deletes it | Created only by hand, from the admin panel. It is a copy of personal data in a place no retention job, no `ops/dsr-delete.sql` and no backup rotation reaches. Delete it when the reason for it ends. Never mail it and never put it in shared storage. |
| Admin browser tab | while the tab is open | An access record shown on screen during a request. Nothing is written to disk. |
| Nightly backups | 30 days local, plus the offsite copy | `ops/backup.ps1`. Not selectively editable; the copy ages out. |
| MySQL binary log | 7 days | `binlog_expire_logs_seconds = 604800` in `ops/02-hardening.cnf`. If `skip-log-bin` is used instead, this row is zero and this table must say so. |
| Notification email | the team mailbox retention | The one copy no SQL script reaches. Delete it from the mailbox by hand as part of a deletion request. |
| Inquiry replies | the team mailbox | Ordinary correspondence, outside this process. |

Do not claim "deleted everywhere immediately". Say the row is deleted now and
the backup copy ages out within the stated window.

**Before answering a deletion request, confirm that no CSV export containing
this address is still on anybody's laptop.** The export is the one copy this
document cannot account for, because nothing on the server knows it exists.

## Automatic deletion

Every submission carries `purge_after = created_at + 24 months`. A job runs
daily at 03:00 UTC, deletes rows past that date unless `legal_hold` is set, and
writes a `job_runs` row with the count, the cutoff and the duration.

`job_runs` is never purged. It is the evidence that the promise is kept, so it
has to outlive the data it deletes.

```sql
SELECT * FROM job_runs WHERE job_name = 'retention_purge' ORDER BY started_at DESC LIMIT 30;
```

## Setting a legal hold

```sql
UPDATE contact_submissions SET legal_hold = 1 WHERE id = <id>;
```

Record why, and where, outside the database. Clear it when the obligation ends,
or the row is retained forever by accident.

## Correction and objection

Correction is an `UPDATE` on the named row by `kestridge_ops`, logged with
`request_type = 'correct'`. Objection to processing, for a service that only
replies to inquiries, is in practice a deletion request: handle it as one and
log it as `'object'`.

## Policy statements this process has to keep true

If any of these changes, `src/app/privacy/page.tsx` changes with it and the
`UPDATED` date at line 31 is bumped.

- Only name, email, company, phone, service and message are collected through
  the form.
- Personal information is kept only as long as needed, and contact form
  submissions are deleted after 24 months unless a legal obligation applies.
- Information is not sold and not shared for cross-context behavioural
  advertising.
- Service providers are limited to website hosting, database hosting, email
  delivery and analytics, all under confidentiality obligations.
- Data is processed and stored in the United States.
- Encryption in transit, access limited to those who need it.
- No automated decision-making, and no marketing email unless asked for.
