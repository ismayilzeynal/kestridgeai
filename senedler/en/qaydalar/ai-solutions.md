# AI Solutions — Delivery Rules

The General Delivery Rules apply here too. Below are the additions
specific to AI projects.

## What we ask from the client

- **A data sample** — after the first meeting, for the assessment (may be
  anonymized).
- **Direct contact with the data owner** — so data questions are answered
  directly, not routed through the POC.
- **Domain expert time** — at least 1–2 hours per week. The judgment of
  someone who knows the field is essential for the model to learn the
  right things.
- **Working environment:** for on-prem work, a GPU VM (exact requirement
  set per project; typical: NVIDIA 24 GB+ VRAM, 8 vCPU, 32 GB RAM, 200 GB
  SSD); in the cloud, a separate project + GPU instance quota. A cost
  estimate is provided up front.

## Data and model rules

- **We never train shared/global models on client data.** A client's data
  is used only in their own solution.
- If a third-party API (OpenAI, Anthropic, etc.) will be used:
  **written consent in advance** + documentation of exactly what data
  goes to the API. PII is masked before sending wherever possible.
- PII is removed from or masked in training data wherever possible.
- Data lineage is maintained: which model was trained on which data,
  when — included in the handover package.

## Quality and acceptance

- The acceptance criterion is written in the agreement **as a number:**
  minimum accuracy / precision-recall / response time — whichever metric
  fits the project.
- The metric is measured at the POC stage; if the target is realistic it
  is confirmed, if not, it is revisited together — no surprises at the
  end.
- The test dataset is kept separate from training; acceptance is judged
  only on test-set results.

## Production rules

- When a model goes live, **monitoring is set up:** performance, response
  time, data drift. Who receives alerts is written in the runbook.
- If human oversight is needed on model outputs (high-stakes decisions),
  the human-in-the-loop flow is designed in from the start.
- A new model version reaches production only after a comparison test
  against the current one; rollback to the previous version is always
  possible.

## Extra handover items

- Model card: what it does, what data it was trained on, its limits.
- Retraining guide: when it's needed, how to do it.
- Access to the monitoring dashboard and the alerting rules.
