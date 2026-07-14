# AI Solutions — Delivery Rules

The [General Rules](00-umumi.md) fully apply. Below are the operational
rules, tool stack and procedures specific to AI projects.

## 1. Tool stack (standard)

| Purpose | Tool | Note |
| --- | --- | --- |
| Language / environment | Python 3.11+, `venv`/`conda` | Versions pinned for reproducibility |
| Model | PyTorch / TensorFlow, Hugging Face | Chosen per task |
| Experiment tracking | **MLflow** | Every training run logged (params, metrics) |
| Data versioning | **DVC** | Data + model version tied to Git |
| Container | **Docker** (+ CUDA image) | Removes environment differences |
| Serving | **FastAPI** (+ vLLM / Triton if needed) | Model exposed as an API |
| Drift monitoring | **Evidently** | Performance/drift watched in production |
| GPU | NVIDIA + CUDA | Driver/CUDA version documented |

## 2. What we ask from the client

- **A data sample** — for the assessment (may be anonymized).
- **Direct contact with the data owner** — for schema/semantics questions.
- **Domain expert time** — at least 1–2 hours per week.
- **Working environment:** for on-prem work, a GPU VM (typical: NVIDIA
  24 GB+ VRAM, 8 vCPU, 32 GB RAM, 200 GB SSD); in the cloud, a separate
  project + GPU instance quota. A cost estimate is provided up front.

## 3. Data and model rules

- **Reproducibility is mandatory:** which model, which data (DVC hash),
  which parameters (MLflow run) — all recorded. The answer to "how did we
  get this result" must always exist.
- **Data lineage** is included in the handover package.
- PII is removed from / masked in training data wherever possible.
- The test dataset is separate from training; acceptance is judged only on
  test-set results.

## 4. Third-party LLMs (OpenAI / Anthropic / etc.) — data governance

This is the most-forgotten risk. The rule:

- **Default: client data (especially PII) is not sent to a third-party
  API.**
- If it will be sent: (1) **written consent in advance**, (2) which data,
  to which provider — documented, (3) **redaction/masking of PII** before
  sending wherever possible.
- The provider's **"zero-retention" / enterprise** mode is selected (data
  not retained for training).
- **Prompt-injection** defense: text coming from user input is not treated
  as a system instruction.
- Prompt/response logs are stored **securely** (no PII in logs).
- An **approved-provider + allowed-data-class matrix** is written per
  project.

## 5. Quality and acceptance

- The acceptance criterion is written in the agreement **as a number:**
  min. accuracy / precision-recall / response time.
- The metric is measured at the POC stage; if the target is realistic it
  is confirmed, if not it is revisited together — no surprises at the end.

## 6. Production rules

- When a model goes live, **monitoring:** performance, response time, data
  drift (Evidently). Who gets alerts is in the runbook.
- For high-stakes decisions, **human-in-the-loop** is designed in from the
  start.
- A new model version reaches production only after a **comparison test**
  against the current one; rollback is always possible.
- **GPU instances are shut down when idle** — they burn cost (a forgotten
  nuance).

## 7. Extra handover items

- **Model card:** what it does, what data it was trained on, its limits.
- **Retraining guide:** when it's needed, how to do it.
- Access to the monitoring dashboard + alerting rules.
- The approved-LLM-provider + data-class matrix.
