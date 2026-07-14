# AI Solutions — iş qaydaları

[Ümumi qaydalar](00-umumi.md) burada da keçərlidir. Aşağıdakılar AI
layihələrinə xas əlavələrdir.

## Müştəridən nə istəyirik

- **Data nümunəsi** — ilk görüşdən sonra, qiymətləndirmə üçün
  (anonimləşdirilmiş ola bilər).
- **Data sahibi ilə birbaşa əlaqə** — data ilə bağlı suallar POC üzərindən
  yox, birbaşa cavablandırılsın deyə.
- **Domain ekspertinin vaxtı** — həftədə minimum 1–2 saat. Modelin düzgün
  öyrənməsi üçün sahəni bilən adamın rəyi vacibdir.
- **İş mühiti:** on-prem işlənəcəksə GPU-lu VM (tələb layihəyə görə
  dəqiqləşir, tipik: NVIDIA 24 GB+ VRAM, 8 vCPU, 32 GB RAM, 200 GB SSD);
  cloud-dursa ayrıca project + GPU instance icazəsi. Xərc smetası
  əvvəlcədən verilir.

## Data və model qaydaları

- **Müştəri datası ilə paylaşılan (shared/global) model öyrətmirik.**
  Müştərinin datası yalnız onun öz həllində istifadə olunur.
- 3-cü tərəf API (OpenAI, Anthropic və s.) istifadə olunacaqsa:
  **əvvəlcədən yazılı razılıq** + hansı datanın API-yə gedəcəyi sənədləşir.
  Mümkün olan yerdə PII maskalanaraq göndərilir.
- PII (şəxsi məlumat) training datasından mümkün qədər çıxarılır və ya
  maskalanır.
- Data lineage saxlanılır: hansı model hansı data ilə, nə vaxt öyrədilib —
  təhvil sənədinə daxildir.

## Keyfiyyət və qəbul

- Qəbul kriteriyası müqavilədə **rəqəmlə** yazılır: minimum dəqiqlik /
  precision-recall / cavab müddəti — layihəyə uyğun metrik.
- Metrik POC mərhələsində ölçülür və hədəf realdırsa təsdiqlənir;
  deyilsə, hədəf birlikdə yenidən baxılır — sonda sürpriz olmur.
- Test dataseti training-dən ayrı saxlanılır; qəbul yalnız test datası
  üzərindəki nəticə ilə olur.

## Production qaydaları

- Model canlıya çıxanda **monitorinq qurulur:** performans, cavab müddəti,
  data drift. Alertlər kimə gedəcəyi runbook-da yazılır.
- Modelin cavablarına insan nəzarəti lazımdırsa (yüksək riskli qərarlar),
  human-in-the-loop axını dizaynda əvvəlcədən nəzərə alınır.
- Yeni model versiyası production-a yalnız köhnə ilə müqayisə testindən
  keçəndən sonra çıxır; köhnə versiyaya qayıtmaq (rollback) həmişə mümkün
  saxlanılır.

## Təhvildə əlavə olaraq

- Model kartı: nə edir, hansı data ilə öyrədilib, limitləri nədir.
- Yenidən öyrətmə (retraining) təlimatı: nə vaxt lazımdır, necə edilir.
- Monitorinq dashboard-una giriş və alert qaydaları.
