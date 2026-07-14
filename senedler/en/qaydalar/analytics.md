# Analytics — Delivery Rules

The [General Rules](00-umumi.md) fully apply. Below are the operational
rules, tool stack and procedures specific to analytics projects.

## 1. Tool stack (standard)

| Purpose | Tool |
| --- | --- |
| Transformation | **dbt** (tests + docs + lineage) |
| Orchestration | **Airflow** (cron / Dagster for small jobs) |
| Warehouse | **Snowflake** / **BigQuery** / **Postgres** |
| BI / dashboards | **Power BI** / **Looker** / **Metabase** |
| Masking | Faker / warehouse masking policy |

## 2. What we ask from the client

- **Read-only access:** a read-only replica or a dedicated analytics user.
  **We never ask for write access to a production database — never.**
- Contact with the source-system owners (CRM admin, DB admin) — for schema
  questions.
- **A KPI owner:** a business-side owner for every metric (they approve
  its definition).
- **Working environment:** where the warehouse lives — the client's cloud
  (separate project) or a VM (standard spec; disk can grow with data
  volume). The BI tool follows existing licenses.

## 3. Data rules

- Data in source systems is **read, never modified.** ETL transforms only
  inside our environment.
- Queries must not **strain the source:** heavy queries run outside
  business hours / against a replica; the first run is agreed with the DB
  admin.
- **Runaway-cost** control: an uncontrolled heavy query on
  Snowflake/BigQuery can create a large bill — queries are optimized and
  limited (a forgotten risk).
- PII fields appear in dashboards only when necessary + behind
  **role-based access (RLS)**; with **masking** where needed.

## 4. Metric rules (semantic layer)

- **A metric definitions document is mandatory:** for every KPI — name,
  formula, source fields, filters, exceptions. The KPI owner approves it
  in writing.
- A single source (dbt / semantic layer) — **every dashboard uses the same
  definition.** Otherwise executives see conflicting numbers (the most
  common problem).
- When a definition changes, the document is updated + dated.
- Every dashboard **shows when its data was last refreshed.**

## 5. Validation rule

- Before any dashboard goes live, its numbers are reconciled with the
  source system (across at least 3 different periods / slices).
- The client gives a **sign-off: "the numbers are correct."** Without this
  approval the dashboard is not opened.

## 6. Refresh and testing

- **dbt tests:** freshness, uniqueness, not-null — part of the pipeline.
- The refresh schedule is documented: which data, how often, how much lag
  is normal (freshness SLA).
- On a pipeline failure → who gets the alert and what to do — in the
  runbook.
- Refresh windows are away from the source's peak hours.

## 7. Extra handover items

- The final version of the metric definitions document.
- **Pipeline diagram + lineage:** source → transformation → warehouse →
  dashboard (dbt docs).
- A guide for adding a user / granting access.
- A short path for adding a new metric in the future (whether they do it
  or we do — same rules).
