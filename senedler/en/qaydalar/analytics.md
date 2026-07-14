# Analytics — Delivery Rules

The General Delivery Rules apply here too. Below are the additions
specific to analytics projects.

## What we ask from the client

- **Read-only access to data sources:** a read-only replica or a
  dedicated analytics user where possible. **We never ask for write
  access to a production database.**
- Contact with the owners of the source systems (CRM admin, DB admin) —
  so schema questions get answered directly.
- **A KPI owner:** a business-side owner for every metric — they approve
  its definition.
- **Working environment:** where the warehouse lives — the client's cloud
  (separate project) or a VM (standard spec: see General Rules; disk can
  be increased for data volume). The BI tool is chosen to match existing
  licenses (Power BI / Looker / Metabase / Grafana).

## Data rules

- Data in source systems is **read, never modified.** ETL transforms only
  inside our environment.
- Queries must not strain the source systems: heavy queries run outside
  business hours / against a replica; the first run is agreed with the DB
  admin in advance.
- Fields containing PII appear in dashboards only when strictly needed,
  behind role-based access.

## Metric rules

- **A metric definitions document is mandatory:** for every KPI — name,
  formula, source fields, filters, exceptions. The KPI owner approves it
  in writing.
- When a definition changes, the document is updated and the change is
  dated — that is how "this number was different last month" situations
  are prevented.
- Every dashboard **shows when its data was last refreshed.**

## The validation rule

- Before any dashboard goes live, its numbers are reconciled with the
  source system (across at least 3 different periods / slices).
- The client gives a **sign-off: "the numbers are correct."** Without
  this approval the dashboard is not opened to users.

## Refresh and alerts

- The refresh schedule is documented: which data, how often, how much lag
  is normal (freshness SLA).
- On a pipeline failure: who gets the alert and what to do — in the
  runbook.
- Refresh windows are scheduled away from the source systems' peak hours.

## Extra handover items

- The final version of the metric definitions document.
- Pipeline diagram: source → transformation → warehouse → dashboard.
- A guide for adding users / granting access.
- A short path for adding a new metric in the future (whether they do it
  or we do — same rules either way).
