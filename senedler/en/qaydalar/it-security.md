# IT Security - Delivery Rules

The [General Rules](00-umumi.md) fully apply. Rules are strictest in this
area - **no exceptions.** Below are the methodology, tool stack and
procedures.

## 1. Methodology

We work from recognized frameworks - not "however we see fit":

- **OWASP** (WSTG + Top 10) - web applications
- **PTES** - the penetration-testing process
- **NIST 800-115** - technical testing methodology
- **MITRE ATT&CK** - attack-technique reference

## 2. Tool stack (standard)

| Phase | Tool |
| --- | --- |
| Discovery / scanning | **Nmap**, nuclei, ffuf |
| Web application | **Burp Suite Pro**, sqlmap |
| Vulnerability scan | **Nessus** / **OpenVAS** |
| Exploitation | **Metasploit** |
| Reporting / scoring | **CVSS 3.1/4.0** |

## 3. Mandatory preconditions before starting

- **Written authorization (authorization letter):** from the client's
  management, signed. The in-scope systems (IPs, domains, applications)
  are attached. **Without this document no testing starts - not even a
  port scan.**
  *(Testing without written authorization = potential CFAA crime in the
  US - never.)*
- **Rules of Engagement (RoE):** in/out-of-scope IPs, the test window,
  allowed techniques, **prohibited actions** (e.g. DoS), an emergency-stop
  contact.
- If systems are hosted by third parties (hosting, SaaS) - the client
  confirms in writing that the third party's testing rules are satisfied.
- **Emergency contact:** a 24/7 number on each side.

## 4. Testing rules

- Tests run only in the **agreed windows** (usually outside business
  hours). Nothing outside a window.
- **Nothing outside the scope is touched** - even if it looks
  interesting. Expansion only with new written authorization.
- Tests that could cause an outage (DoS, brute force) run only with
  separate explicit consent.
- A discovered vulnerability is used only **minimally, to prove it** - no
  data exfiltration, no system changes, no backdoors.

## 5. Evidence handling

- Screenshots, logs, data samples - stored **encrypted** in the project
  environment.
- Chain of custody: which evidence, when, by whom.
- All evidence is deleted after the report (data destruction).

## 6. Reporting rules

- **Critical vulnerability → direct notice within 24 hours** (the report
  is not waited for).
- Format: each finding - severity (**CVSS**), description, evidence,
  business risk, a concrete remediation step. A short executive summary on
  top.
- **Reports travel only over encrypted channels** - PGP / age /
  password-protected + a separate channel. Never as a plain email
  attachment (full of live vulnerabilities) - **never**.
- The report goes only to the people the client designates; on our side,
  only the project team sees it.

## 7. Remediation and retest

- Remediation sits with the client or with us - per the agreement. We keep
  supporting questions throughout.
- **One retest is included at no extra charge**; the final report shows
  closed / partial / open per finding.

## 8. Continuous monitoring service

- The connected log sources + retention period are documented.
- **Escalation matrix:** which severity → whom, over which channel, within
  what time (including night hours) - signed off at kickoff.
- The first 2 to 4 weeks are tuning (clearing false alarms); alert volume may
  be high during this time.
- Monthly report: incidents, trends, recommendations.

## 9. Extra handover items

- Final report + retest results.
- Confirmation that all access is revoked (especially critical here) +
  evidence destruction.
- Recommended next steps (annual retest, monitoring).
