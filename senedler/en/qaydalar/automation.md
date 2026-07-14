# Automation — Delivery Rules

The General Delivery Rules apply here too. Below are the additions
specific to automation projects.

## What we ask from the client

- **A process owner** — the specific person(s) who run the process today.
  Mapping is done with them; approval comes from them.
- **Test accounts** — a dedicated test account for us in every system to
  be integrated (CRM, ERP, email, etc.). We never work through real
  users' accounts.
- **A test environment** — sandbox/test instances of the systems where
  possible. Where none exists, test scenarios run only with agreed safe
  data.
- **A place to run** — where the automation will live: the client's VM
  (standard spec: see General Rules) or an account on the platform the
  client already uses (n8n / Make / Power Automate / Zapier — chosen to
  fit the existing stack). The platform account is opened in the
  client's name — nothing stays dependent on us.

## Build rules

- **API-first:** if a system has an API, we use it; RPA (screen-level
  automation) only where there is no API — because it breaks when the
  interface changes.
- **Error handling is mandatory in every workflow:** what happens on
  failure, who gets notified (email/message), how many retries — all
  documented.
- **Kill switch:** the client must be able to stop the process themselves
  at any moment. Its location is shown in the runbook.
- The automation **logs every operation** it performs: when, what was
  done, what the result was. Problems can be traced backwards.
- Financial or irreversible operations (payments, deletions, emails to
  customers) are first built **with an approval step**; fully automatic
  mode only after the parallel run, with written consent.

## The parallel-run rule

- The new automation runs alongside the old process for **at least
  2 weeks**.
- Results are compared daily; if they diverge, no cutover happens until
  the cause is found — the period is extended instead.
- The old process is untouched during this period — the way back is
  always open.

## Extra handover items

- Workflow map: every step, every integration, every notification.
- Runbook: stop / restart / resolving typical errors.
- Change guide: when one of the systems changes (new field, new version),
  what to check.
