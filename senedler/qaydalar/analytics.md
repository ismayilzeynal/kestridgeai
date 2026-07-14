# Analytics — İş Qaydaları

[Ümumi qaydalar](00-umumi.md) tam keçərlidir. Aşağıdakılar analitika
layihələrinə xas əməli qaydalar, tool stack və prosedurlardır.

## 1. Tool stack (standart)

| Məqsəd | Tool |
| --- | --- |
| Transformasiya | **dbt** (test + docs + lineage) |
| Orkestrasiya | **Airflow** (kiçik işlərdə cron / Dagster) |
| Warehouse | **Snowflake** / **BigQuery** / **Postgres** |
| BI / dashboard | **Power BI** / **Looker** / **Metabase** |
| Maskalama | Faker / warehouse masking policy |

## 2. Müştəridən nə istəyirik

- **Read-only giriş:** read-only replica və ya ayrıca analytics
  istifadəçisi. **Production bazaya yazma girişi istəmirik — heç vaxt.**
- Mənbə sistem sahibləri ilə əlaqə (CRM admin, DB admin) — sxem sualları
  üçün.
- **KPI sahibi:** hər metrikin biznes tərəfdən sahibi (tərifi o təsdiqləyir).
- **İş mühiti:** warehouse harada — müştərinin cloud-u (ayrıca project)
  və ya VM (standart spec; data həcminə görə disk artırıla bilər). BI
  aləti mövcud lisenziyalara görə.

## 3. Data qaydaları

- Mənbə sistemlərdən data **oxunur, dəyişdirilmir.** ETL yalnız bizim
  mühitdə transformasiya edir.
- Sorğular mənbəni **yormamalıdır:** ağır sorğu iş saatından kənar /
  replica üzərində; ilk işə salma DB admini ilə razılaşdırılır.
- **Runaway xərc** nəzarəti: Snowflake/BigQuery-də nəzarətsiz ağır sorğu
  böyük hesab yarada bilər — sorğular optimallaşdırılır, limit qoyulur
  (unudulan risk).
- PII sahələr dashboard-larda yalnız zərurət + **rol-əsaslı giriş (RLS)**
  ilə; lazım olanda **maskalama**.

## 4. Metrik qaydaları (semantic layer)

- **Metric definitions sənədi məcburidir:** hər KPI — ad, düstur, mənbə
  sahələr, filtrlər, istisnalar. KPI sahibi yazılı təsdiqləyir.
- Vahid mənbə (dbt / semantic layer) — **hər dashboard eyni tərifi
  işlədir.** Əks halda rəhbərlik fərqli rəqəm görür (ən çox rast gəlinən
  problem).
- Tərif dəyişəndə sənəd yenilənir + tarix qeyd olunur.
- Hər dashboard-da **son yenilənmə vaxtı (last refresh)** görünür.

## 5. Validasiya qaydası

- Hər dashboard canlıya çıxmazdan əvvəl rəqəmlər mənbə sistemlə üzləşdirilir
  (ən azı 3 fərqli dövr / kəsim).
- Müştəri **sign-off verir: "rəqəmlər düzdür".** Bu təsdiq olmadan
  dashboard açılmır.

## 6. Refresh və test

- **dbt testləri:** freshness, uniqueness, not-null — pipeline-a daxil.
- Refresh cədvəli sənədləşir: hansı data, nə tezliklə, nə qədər gecikmə
  normaldır (freshness SLA).
- Pipeline xətası → alert kimə gedir, nə edilməli — runbook-da.
- Refresh pəncərələri mənbənin pik saatlarından kənar.

## 7. Təhvildə əlavə olaraq

- Metric definitions sənədinin son versiyası.
- **Pipeline sxemi + lineage:** mənbə → transformasiya → warehouse →
  dashboard (dbt docs).
- Yeni istifadəçi / giriş vermə təlimatı.
- Yeni metrik əlavə etmək üçün qısa yol (gələcəkdə özləri və ya biz — eyni
  qayda ilə).
