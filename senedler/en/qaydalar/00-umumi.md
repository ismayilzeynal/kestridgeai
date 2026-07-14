# General Delivery Rules (all service areas)

This document applies to every project and is the operational manual for
the engineer doing the work — "in this situation, do this, with this tool,
under this rule". Area-specific additions live in separate files.
Everything here is agreed with the client at kickoff; changes only with
written approval.

> **Golden rules (never broken):** ① Least privilege — access only to
> what's needed, at the minimum level. ② Staging-first — production is
> never touched directly. ③ Every decision is confirmed in writing by
> email. ④ We never ask for a client's password. ⑤ We do not hand over
> and leave until we are sure it fully works.

---

## 1. Roles and responsibilities

| Role | Who | Responsible for |
| --- | --- | --- |
| Project lead (PM/POC) | 1 person from us | Overall progress, communication, reporting |
| Lead engineer | per area | Technical execution, code, deployment |
| Security officer | 1 person (can be shared) | Access, secrets, offboarding audit |
| Client POC | 1 person from client | Decisions, access approval, sign-off |

There is **one POC** on each side — all formal decisions flow through them.

---

## 2. Client onboarding checklist

Closed within the first 3 days after kickoff:

- [ ] NDA signed (if applicable)
- [ ] POCs named (both sides), communication channel set up
- [ ] Access method agreed (see Section 4) and granted
- [ ] Working environment (VM / cloud project) ready (see Section 6)
- [ ] Access register file created
- [ ] Shared vault folder created for secrets (see Section 5)
- [ ] Weekly status day/time set
- [ ] Data transfer channel agreed if needed (see Section 7)

---

## 3. Our work devices (endpoint security)

We connect to a client system only from a device that meets these terms:

- **Full-disk encryption** enabled (BitLocker / FileVault / LUKS).
- Screen lock ≤ 5 min, strong login password + device MFA.
- Antivirus / EDR active, OS and browser patched.
- **Client data is never stored on personal / unmanaged devices** — only
  in the project environment. Any local copy lives in an encrypted folder
  and is deleted when the work ends.
- No client data moved via personal USB or personal cloud (Google
  Drive/iCloud).
- When the work ends, anything client-related (config, keys, data) is
  wiped from the device.

---

## 4. Access and connectivity (MOST IMPORTANT SECTION)

### 4.1 General principles

- **We never ask for the password of a client's personal or admin
  account.** A **named account** (`firstname.lastname@client` style) or a
  single **service account** is created for us.
- **MFA is mandatory** on all accounts; a hardware key (YubiKey) or
  authenticator app where possible (not SMS).
- Access is **time-boxed — 30 days by default.** For ongoing projects, an
  extension email is sent at each month-end (who, which access, why, how
  long). That email is the record for both sides.
- All access is kept in an **access register:** who / which system /
  level / granted date / expiry date.

### 4.2 Connection method — order of preference

We choose **top-down** — the first viable option is taken.

| Order | Method | When | Controls |
| --- | --- | --- | --- |
| 1 | Client's own VPN | Client has a VPN | WireGuard / OpenVPN / IPsec; named profile for us |
| 2 | Zero-trust mesh | No VPN, need speed | Tailscale / Cloudflare Access / Twingate |
| 3 | Bastion / jump host | Entry into the internal network | Single entry point, logged, MFA |
| 4 | Direct (allowlist) | Only an isolated single server | Only our static IP open, rest closed |

**Never:** expose an RDP/SSH/DB port to the internet, shared passwords,
shared accounts, direct `0.0.0.0/0` access without a VPN.

### 4.3 SSH (Linux servers)

- **Key-only** access (password authentication disabled). Key type:
  `ed25519`. The key is passphrase-protected and stored in the vault.
- If there's a bastion, use `ProxyJump`, not a direct hop:

  ```
  # ~/.ssh/config
  Host client-bastion
      HostName bastion.client.com
      User aivanta.name
      IdentityFile ~/.ssh/client_ed25519
  Host client-app
      HostName 10.0.1.20
      User aivanta.name
      ProxyJump client-bastion
      IdentityFile ~/.ssh/client_ed25519
  ```

- Agent forwarding only when strictly necessary; sudo actions are logged.
- Server host keys are verified on first connect and recorded.

### 4.4 Windows servers (RDP)

- RDP **never from the internet directly** — only behind a VPN or bastion.
- **NLA (Network Level Authentication)** on, named account + MFA.
- Long sessions are logged; the session is closed (logout) when done.

### 4.5 Cloud consoles (AWS / Azure / GCP)

- **Login via SSO** (Google/Microsoft); the root/owner account is not used.
- We get a **scoped IAM role** — only the services the project needs.
- **We do not create long-lived access keys** — temporary credentials:
  `aws sso login` / STS, `az login`, `gcloud auth login`.
- Every action stays in the cloud audit log (CloudTrail / Activity Log).

### 4.6 Databases

- A **read-only user** for us (a separate, limited one if writes are
  required).
- The database is **not exposed to the internet** — via SSH tunnel or
  bastion:

  ```
  ssh -L 5432:db.internal:5432 client-bastion
  # then connect to local 127.0.0.1:5432
  ```

- Heavy queries on a prod DB only outside business hours / against a
  replica.

### 4.7 SaaS admin panels and APIs

- Named account + MFA; no shared login.
- API keys live in the vault; never kept in code / Git (see Section 5).
- Minimal scope on APIs (only the permissions needed).

---

## 5. Secrets (passwords, keys, tokens) management

- Central: a **password manager / vault** — 1Password or Bitwarden; a
  separate shared folder (vault) per client.
- **Never lands in Git.** `.env` files in `.gitignore`; an `.env.example`
  (valueless) for reference.
- **Secret scanning** before commit: `gitleaks` (as a pre-commit hook
  where possible). If something slips in — rotate immediately and purge
  from history.
- Secrets are transferred **over encrypted channels** (vault share /
  encrypted link) — never in plain text via email, message or Slack.
- Rotation: at project end and whenever a team member changes, all shared
  secrets are rotated.

---

## 6. Working environment and infrastructure

- **Standard request — one dedicated VM:** min. 4 vCPU / 16 GB RAM /
  100 GB SSD, Ubuntu 22.04 LTS (or the client's OS standard).
  Area-specific needs (e.g. GPU for AI) are in the area document.
- Environment tiers are separate: **dev / staging / production.** Testing
  in staging; production only via approved deployment.
- **Naming:** `aivanta-<client>-<env>-<role>` (e.g.
  `aivanta-acme-staging-app`).
- **Infrastructure as Code** where possible: Terraform (resources),
  Ansible (server config) — manual "click-ops" is minimized so things can
  be rebuilt.
- Containerization: **Docker** — removes environment differences.
- **Cloud cost stays on the client's account** — they keep full
  visibility.

---

## 7. Working with data

- **Classification:** each dataset is rated "public / internal /
  confidential / PII"; PII and confidential get the strictest handling.
- **Encryption:** in transit (TLS/SSH) and at rest (disk encryption).
- **Transfer methods (only these):** SFTP, the client's cloud storage (a
  bucket/folder shared with us), a time-limited encrypted link. **Data is
  never sent by email.**
- **Anonymization / masking:** we work with masked data where possible;
  production data only when necessary + with written consent.
- **Retention and deletion:** data lives only in the project environment;
  after the work ends it is **deleted within 30 days**, and the deletion
  is confirmed by email (unless the agreement says otherwise).
- **Data residency:** for US clients, data is kept in a US region (unless
  the client requires otherwise).

---

## 8. Source control and change management

- All code / IaC / config **in Git** (GitHub / GitLab, private repo).
- Work in branches; **mandatory PR review** into `main` (at least 1
  reviewer).
- **No direct push to production** — only in an approved deployment
  window.
- Before every deploy, a **snapshot / backup** (see Section 9) and a ready
  rollback plan.
- **CI/CD** where possible: automated test + deploy pipeline (GitHub
  Actions).

---

## 9. Backup, rollback and disaster recovery

- **Before any change:** a VM snapshot / DB dump / config copy.
- A rollback plan is written before every deploy: what to revert, how, in
  how many minutes.
- On critical systems, backups are automated and **restore-tested** (an
  untested backup is not a backup).

---

## 10. Logging, monitoring and audit

- **Session logs** on the bastion and servers — who, when, what.
- Cloud audit trails on (CloudTrail / Activity Log / Cloud Audit Logs).
- Monitoring + alerts on critical systems (see area documents).
- Logs belong to the client and are confidential; only the project team
  sees them on our side.

---

## 11. Communication and status

- Channels: **email** (formal decisions), **video** (Google Meet / Zoom),
  **Slack / Teams** for day-to-day if the client prefers.
- **Weekly status:** a fixed day. Format — what was done / what is being
  done / what is blocking. 15–30 minutes.
- Our response times: routine question — 1 business day; blocking issue —
  4 business hours.
- Work tracking: an **issue tracker** (Jira / Linear / GitHub Issues) —
  every task is tracked.

---

## 12. Scope and changes

- A new request = **a change request:** written by email, effort and
  schedule impact estimated, added to the plan after approval.
- "It's tiny, let's just add it now" — does not exist. Even a small change
  is recorded; its estimation is simply fast.

---

## 13. Incident handling

| Severity | Example | Response |
| --- | --- | --- |
| Critical | System down, data leak | Within 4 hours, immediate notice |
| High | Core function broken | 1 business day |
| Medium / low | Minor fault | Next status / per plan |

- In a critical incident we **roll back first, investigate second.**
- After an incident, a short **post-mortem:** what happened, why, and what
  was done so it doesn't repeat.

---

## 14. Legal / compliance — what the engineer must know

- An **NDA** can be signed at any stage; client information is never
  shared with third parties.
- In security work, **nothing starts without a written authorization
  letter** (see IT Security rules).
- **Scope discipline:** a system that isn't authorized is not touched —
  even if it looks interesting.

---

## 15. Offboarding (at project end)

- [ ] **All access** in the register is revoked
- [ ] Shared secrets are rotated (changed on the client side)
- [ ] Client data is wiped from our devices (30-day rule)
- [ ] Revocation and deletion are **confirmed by email**
- [ ] The handover package is delivered (see Section 16)

---

## 16. Handover package (identical on every project)

1. Architecture diagram + configuration document.
2. **Runbook** — how to start/stop the system, typical problems and their
   fixes, who to contact.
3. Secure transfer of secrets/passwords (vault or encrypted channel).
4. Training session (1–2 hours, recorded).
5. Access revocation + data deletion confirmation.
6. Support terms: what's included, how to reach us, response times.
