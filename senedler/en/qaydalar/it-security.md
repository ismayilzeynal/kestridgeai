# IT Security — Delivery Rules

The General Delivery Rules apply here too. Below are the additions
specific to security work. Rules are strictest in this area — no
exceptions.

## Mandatory preconditions

- **Written authorization (authorization letter):** signed by the
  client's management, with the exact scope list attached (IPs, domains,
  applications). **Without this document no testing starts — not even a
  port scan.**
- If systems are hosted by third parties (hosting, SaaS), the client
  confirms in writing that the third party's testing rules are satisfied
  (e.g. the cloud provider's pentest policy).
- **Emergency contacts:** a 24/7 phone number on each side. If anything
  unexpected happens during testing — immediate call + testing stops.

## Testing rules

- Tests run only in the **agreed windows** (usually outside business
  hours). Nothing runs outside a window.
- **Nothing outside the scope is touched** — even if it looks interesting
  mid-test. Scope expands only with new written authorization.
- Tests that could cause a service outage (DoS-like, brute force, etc.)
  run only with separate explicit consent.
- A discovered vulnerability is used only minimally, to prove it exists —
  no data exfiltration, no system changes, no backdoors.
- Everything obtained during testing (screenshots, logs, data samples)
  stays in the project environment and is deleted after the report.

## Findings and reporting rules

- **Critical vulnerability → direct notice within 24 hours.** We do not
  wait for the report; the client gets the chance to close it
  immediately.
- Report format: for each finding — severity (Critical/High/Medium/Low),
  description, evidence, business risk, a concrete remediation step. A
  non-technical executive summary on top.
- Reports and findings travel **only over encrypted channels** — never as
  a plain email attachment.
- The report goes only to the people the client designates; on our side,
  only the project team sees it.

## Remediation and retest

- Remediation may sit with the client or with us — the agreement says
  which. Either way we support questions throughout.
- **One retest is included at no extra charge**; its results appear in
  the final report as closed / partial / open per finding.

## For the continuous monitoring service

- The list of connected log sources and the retention period are
  documented.
- **Escalation matrix:** which severity notifies whom, over which
  channel, within what time (including night hours) — signed off at
  kickoff.
- The first 2–4 weeks are a tuning period: false alarms are cleared;
  alert volume may be above normal during this time.
- Monthly report: incidents, trends, recommendations.

## Extra handover items

- Final report + retest results.
- Confirmation that all of our access has been revoked (especially
  critical in this area).
- Recommended next steps (annual retest, monitoring, etc.).
