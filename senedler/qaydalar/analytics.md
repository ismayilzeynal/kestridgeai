# Analytics — iş qaydaları

[Ümumi qaydalar](00-umumi.md) burada da keçərlidir. Aşağıdakılar
analitika layihələrinə xas əlavələrdir.

## Müştəridən nə istəyirik

- **Data mənbələrinə read-only giriş:** mümkünsə read-only replica və ya
  ayrıca analytics istifadəçisi. **Production bazaya yazma girişi
  istəmirik** — heç vaxt.
- Mənbə sistemlərin sahibləri ilə əlaqə (CRM admini, DB admini) — sxem
  sualları birbaşa cavablansın deyə.
- **KPI sahibi:** hər metrikin biznes tərəfdən sahibi — tərifi o təsdiqləyir.
- **İş mühiti:** warehouse haradadır — müştərinin cloud-u (ayrıca project)
  və ya VM (standart: ümumi qaydalardakı kimi; data həcminə görə disk
  artırıla bilər). BI alət seçimi mövcud lisenziyalara görə (Power BI /
  Looker / Metabase / Grafana).

## Data qaydaları

- Mənbə sistemlərdən data **oxunur, dəyişdirilmir.** ETL yalnız bizim
  mühitdə transformasiya edir.
- Sorğular mənbə sistemi yormamalıdır: ağır sorğular iş saatlarından
  kənar / replica üzərində; ilk dəfə işə salınmadan əvvəl DB admini ilə
  razılaşdırılır.
- PII ehtiva edən sahələr dashboard-larda yalnız zərurət halında və
  rol-əsaslı giriş ilə göstərilir.

## Metrik qaydaları

- **Metric definitions sənədi məcburidir:** hər KPI üçün — ad, düstur,
  mənbə sahələr, filtrlər, istisna hallar. KPI sahibi yazılı təsdiqləyir.
- Tərif dəyişəndə sənəd yenilənir və dəyişiklik tarixi qeyd olunur —
  "keçən ay bu rəqəm başqa idi" situasiyasının qarşısı belə alınır.
- Hər dashboard-da datanın **nə vaxt yeniləndiyi görünür** (last refresh).

## Validasiya qaydası

- Hər dashboard canlıya çıxmazdan əvvəl rəqəmlər mənbə sistemlə
  üzləşdirilir (ən azı 3 fərqli dövr / kəsim üzrə).
- Müştəri **sign-off verir: "rəqəmlər düzdür".** Bu təsdiq olmadan
  dashboard istifadəçilərə açılmır.

## Refresh və alertlər

- Refresh cədvəli sənədləşir: hansı data, nə tezliklə, nə qədər gecikmə
  normaldır (freshness SLA).
- Pipeline xətası halında alert kimə gedir, nə edilməlidir — runbook-da.
- Refresh pəncərələri mənbə sistemlərin pik saatlarından kənar seçilir.

## Təhvildə əlavə olaraq

- Metric definitions sənədinin son versiyası.
- Pipeline sxemi: mənbə → transformasiya → warehouse → dashboard.
- Yeni istifadəçi əlavə etmə / giriş vermə təlimatı.
- Yeni metrik əlavə etmək üçün qısa yol xəritəsi (gələcəkdə özləri və ya
  biz — hər iki halda eyni qayda ilə).
