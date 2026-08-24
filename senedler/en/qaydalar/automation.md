# Automation - Delivery Rules

The [General Rules](00-umumi.md) fully apply. Below are the operational
rules, tool stack and procedures specific to automation projects.

## 1. Tool stack (standard)

| Purpose | Tool | When |
| --- | --- | --- |
| Low-code flows | **n8n** (self-hosted, privacy-friendly) | Default choice |
| Low-code (client stack) | Make / Power Automate / Zapier | If the client already uses it |
| Custom logic | **Python** | When API/low-code isn't enough |
| RPA (interface) | **Playwright** (or UiPath) | Only where there's no API |
| Secrets | Vault (1Password/Bitwarden) | Never kept in code/flows |
| Monitoring | Platform logs + alerts | Every run is tracked |

**The platform account is opened in the client's name** - nothing stays
dependent on us.

## 2. What we ask from the client

- **A process owner** - the specific person(s) who run the process today.
- **Test accounts** - in every system to be integrated (we never work
  through real users' accounts).
- **A test environment** - sandbox/test instance; where none exists, only
  agreed safe data.
- **A place to run** - where the flow will live: the client's VM (standard
  spec: General Rules) or a platform account.

## 3. Build rules (mandatory in every flow)

Every automation must have these 5 things - **no exceptions:**

1. **Idempotency** - if the same operation runs again, the result isn't
   duplicated (e.g. the same email isn't sent twice, the same payment
   isn't made twice).
2. **Retry + backoff** - on a transient error, retry with exponential
   delay; if it ultimately fails, a **dead-letter** (it lands somewhere,
   never lost).
3. **Kill switch** - the client can stop the process themselves at any
   moment; its location is in the runbook.
4. **Logging** - every operation: when, what was done, the result.
   Traceable backwards.
5. **Owner** - each flow has one responsible person.

Also:

- **API-first:** use the API if one exists; RPA only where there is none
  (it breaks when the interface changes).
- **Rate limit / quota** is respected - don't overload/flood the client
  system (a forgotten risk: an infinite loop → thousands of duplicate
  emails).
- Financial / irreversible operations (payments, deletions, emails to
  customers) are first built **with an approval step**; fully automatic
  mode only after the parallel run, with written consent.
- API auth: OAuth / scoped token, refresh handled properly; keys in the
  vault.

## 4. The parallel-run rule

- The new automation runs **alongside the old process for at least
  2 weeks.**
- Results are compared daily; if they diverge, no cutover happens - the
  period is extended instead.
- The old process is untouched - the way back is open.

## 5. Testing and deployment

- The flow is built in **test/staging** first, and reaches production only
  after it's confirmed working.
- Flows are versioned (export + Git); changes go through a change request.

## 6. Extra handover items

- **Workflow map:** every step, every integration, every notification
  (trigger → action → owner).
- **Runbook:** stop / restart / resolving typical errors.
- **Change guide:** when one of the systems changes (new field, new
  version), what to check.
