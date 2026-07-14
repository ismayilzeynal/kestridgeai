# General Delivery Rules (all service areas)

This document applies to every project. Area-specific rules live in
separate files. Everything here is agreed with the client at kickoff, and
changes only with written approval.

## 1. Principles

- **Least privilege** — access only to the systems needed, at the minimum level.
- **Staging-first** — no direct changes in production; test environment first.
- **Written trail** — every decision and agreement is confirmed by email.
  A verbal agreement is not an agreement.
- **Confidentiality** — client information is never shared with third
  parties; an NDA can be signed at any stage.
- **We stay until it fully works** — handover only after stabilization.

## 2. Communication

- **One point of contact (POC)** is named on each side — all questions
  flow through them.
- Channels: email (formal decisions), video calls (Google Meet / Zoom),
  Slack / Teams for day-to-day if the client prefers.
- **Weekly status:** a fixed day/time. Format: what was done, what is
  being done, what is blocking. Never longer than 15–30 minutes.
- Our response times: routine question — 1 business day; blocking issue —
  4 business hours.

## 3. Access

- **We never ask for the password of a client's personal or admin
  account.** Named accounts are created for us (`firstname.lastname@ /
  -aivanta` style) or a single service account.
- **MFA must be enabled** on all accounts; access goes through a VPN
  where possible.
- Access is granted **for a limited term — 30 days by default.** For
  ongoing projects, an extension email is sent at the end of each month
  (who, which access, why, for how long). That email is the record for
  both sides.
- A single register of all access is kept for the life of the project:
  person, system, level, granted/expiry date.
- At project end, **offboarding:** every access on the register is
  revoked, and the revocation is confirmed in the handover document.

## 4. Working environment

- Standard request — **one dedicated VM** from the client for our work:
  minimum 4 vCPU / 16 GB RAM / 100 GB SSD, Ubuntu 22.04 LTS (or the
  client's own OS standard). Area-specific requirements (e.g. GPU for AI)
  are listed in the area documents.
- VM access **by SSH key only** (no passwords), behind an IP allowlist or
  VPN.
- If the client is in the cloud (AWS / Azure / GCP): a separate
  project / resource group instead of a VM + scoped IAM roles for us.
  Costs stay on the client's account — they keep full visibility.
- Production work happens only in a planned deployment window, with a
  rollback plan ready.

## 5. Data

- Transfer **only over encrypted channels:** SFTP, the client's cloud
  storage (a bucket/folder shared with us), or a time-limited encrypted
  link. **Data is never sent by email.**
- Wherever possible we work with **anonymized / masked data.** Production
  data only when strictly necessary, with written consent.
- Data lives only in the project environment — never copied to personal
  computers.
- After the project ends, **data is deleted within 30 days** and a
  deletion confirmation is emailed (unless the agreement specifies
  otherwise).

## 6. Scope and changes

- A new request = **a change request:** written by email, effort and
  schedule impact estimated, added to the plan after approval.
- "It's tiny, let's just add it now" — does not exist. Even a
  small-looking change is recorded; its estimation is simply fast.

## 7. Incident handling

- If something we built breaks: **critical (system down) — response
  within 4 hours; everything else — 1 business day.**
- A rollback plan exists before every deploy; in a critical incident we
  roll back first, investigate second.
- After an incident, a short post-mortem: what happened, why, and what
  was done so it doesn't repeat.

## 8. Handover package (identical on every project)

1. Architecture diagram and configuration document.
2. **Runbook** — how to start/stop the system, typical problems and their
   fixes, who to contact.
3. Secure transfer of secrets/passwords (password manager or encrypted
   channel — never in plain text).
4. Training session (1–2 hours, recorded).
5. Access revocation + data deletion confirmation.
6. Support terms: what's included, how to reach us, response times.
