# AI Solutions - İş Qaydaları

[Ümumi qaydalar](00-umumi.md) tam keçərlidir. Aşağıdakılar AI
layihələrinə xas əməli qaydalar, tool stack və prosedurlardır.

## 1. Tool stack (standart)

| Məqsəd | Tool | Qeyd |
| --- | --- | --- |
| Dil / mühit | Python 3.11+, `venv`/`conda` | Reproduksiya üçün versiyalar sabit |
| Model | PyTorch / TensorFlow, Hugging Face | Tapşırığa görə seçilir |
| Eksperiment izləmə | **MLflow** | Hər training run loglanır (parametr, metrik) |
| Data versiyalama | **DVC** | Data + model versiyası Git-ə bağlı |
| Konteyner | **Docker** (+ CUDA image) | Mühit fərqini aradan qaldırır |
| Serving | **FastAPI** (+ vLLM / Triton lazımdırsa) | Model API kimi verilir |
| Drift monitorinq | **Evidently** | Canlıda performans/drift izləmə |
| GPU | NVIDIA + CUDA | Draytver/CUDA versiyası sənədləşir |

## 2. Müştəridən nə istəyirik

- **Data nümunəsi** - qiymətləndirmə üçün (anonimləşdirilmiş ola bilər).
- **Data sahibi ilə birbaşa əlaqə** - sxem/məna sualları üçün.
- **Domain ekspertinin vaxtı** - həftədə min. 1-2 saat.
- **İş mühiti:** on-prem işlənəcəksə GPU-lu VM (tipik: NVIDIA 24 GB+ VRAM,
  8 vCPU, 32 GB RAM, 200 GB SSD); cloud-dursa ayrıca project + GPU instance
  kvotası. Smeta əvvəlcədən verilir.

## 3. Data və model qaydaları

- **Reproduksiya məcburidir:** hansı model, hansı data (DVC hash), hansı
  parametrlə (MLflow run) - hamısı qeyddə. "Bu nəticəni necə aldıq"
  sualının cavabı həmişə olmalıdır.
- **Data lineage** təhvil paketinə daxildir.
- PII training datasından mümkün qədər çıxarılır / maskalanır.
- Test dataseti training-dən ayrı; qəbul yalnız test datası nəticəsi ilə.

## 4. Üçüncü tərəf LLM (OpenAI / Anthropic / və s.) - data governance

Bu, ən çox unudulan risk. Qayda:

- **Standart: müştəri datası (xüsusən PII) 3-cü tərəf API-yə göndərilmir.**
- Göndəriləcəksə: (1) **əvvəlcədən yazılı razılıq**, (2) hansı data,
  hansı provayderə - sənədləşir, (3) mümkün olan yerdə PII göndərişdən
  əvvəl **redaksiya/maskalama**.
- Provayderin **"zero-retention" / enterprise** rejimi seçilir (data
  training üçün saxlanmasın).
- **Prompt injection** müdafiəsi: istifadəçi girişindən gələn mətn sistem
  təlimatı kimi qəbul edilmir.
- Prompt/cavab logları **təhlükəsiz** saxlanılır (PII log-a düşməsin).
- **Təsdiqli provayder + icazəli data sinfi matrisi** hər layihədə yazılır.

## 5. Keyfiyyət və qəbul

- Qəbul kriteriyası müqavilədə **rəqəmlə**: min. dəqiqlik / precision-recall
  / cavab müddəti.
- Metrik POC-da ölçülür; hədəf realdırsa təsdiqlənir, deyilsə birlikdə
  yenidən baxılır - sonda sürpriz olmur.

## 6. Production qaydaları

- Model canlıya çıxanda **monitorinq:** performans, cavab müddəti, data
  drift (Evidently). Alert kimə gedir - runbook-da.
- Yüksək riskli qərarlarda **human-in-the-loop** dizaynda əvvəlcədən var.
- Yeni model versiyası prod-a yalnız köhnə ilə **müqayisə testindən** sonra;
  rollback həmişə mümkün.
- **GPU instansları boş qalanda söndürülür** - xərc yeyir (unudulan nüans).

## 7. Təhvildə əlavə olaraq

- **Model kartı:** nə edir, hansı data ilə öyrədilib, limitləri nədir.
- **Retraining təlimatı:** nə vaxt lazımdır, necə edilir.
- Monitorinq dashboard-una giriş + alert qaydaları.
- Təsdiqli LLM provayder + data sinfi matrisi.
