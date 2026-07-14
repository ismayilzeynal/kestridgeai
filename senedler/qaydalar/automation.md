# Automation — iş qaydaları

[Ümumi qaydalar](00-umumi.md) burada da keçərlidir. Aşağıdakılar
avtomatlaşdırma layihələrinə xas əlavələrdir.

## Müştəridən nə istəyirik

- **Prosesin sahibi** — prosesi bu gün icra edən konkret adam(lar).
  Xəritələnmə onlarla aparılır, təsdiq onlardan alınır.
- **Test hesabları** — inteqrasiya olunacaq hər sistemdə (CRM, ERP, email
  və s.) bizim üçün ayrıca test hesabı. Real istifadəçilərin hesabı ilə
  işləmirik.
- **Test mühiti** — mümkünsə sistemlərin sandbox/test instansı. Yoxdursa,
  test ssenariləri yalnız razılaşdırılmış təhlükəsiz data ilə.
- **İş mühiti** — avtomatlaşdırma harada işləyəcək: müştərinin VM-i
  (standart: ümumi qaydalardakı kimi) və ya müştərinin istifadə etdiyi
  platforma hesabı (n8n / Make / Power Automate / Zapier — mövcud stack-ə
  uyğun seçilir). Platforma hesabı müştərinin adına açılır — bizdən asılı
  qalmır.

## Qurulma qaydaları

- **API-first:** sistemin API-si varsa API ilə; RPA (ekran üzərindən
  avtomatlaşdırma) yalnız API olmayanda — çünki interfeys dəyişəndə sınır.
- Hər workflow-da **error handling məcburidir:** xəta halında nə baş verir,
  kimə bildiriş gedir (email/mesaj), neçə dəfə retry olunur — hamısı
  sənədləşir.
- **Kill switch:** müştəri istənilən an prosesi özü dayandıra bilməlidir.
  Düymənin/qaydanın yeri runbook-da göstərilir.
- Avtomatlaşdırma etdiyi hər əməliyyatın **logunu** saxlayır: nə vaxt, nə
  edildi, nəticə nə oldu. Problem çıxanda geriyə izləmək mümkün olur.
- Maliyyə və ya geri dönməz əməliyyatlar (ödəniş, silmə, müştəriyə email)
  ilk mərhələdə **təsdiq addımı ilə** qurulur; tam avtomata keçid yalnız
  paralel dövr bitdikdən sonra, yazılı razılıqla.

## Paralel dövr qaydası

- Yeni avtomatlaşdırma **minimum 2 həftə** köhnə proseslə yanaşı işləyir.
- Nəticələr gündəlik tutuşdurulur; fərq çıxarsa, səbəbi tapılmadan cutover
  olmur, dövr uzadılır.
- Köhnə prosesə bu dövrdə toxunulmur — geri dönüş yolu həmişə açıqdır.

## Təhvildə əlavə olaraq

- Workflow xəritəsi: hər addım, hər inteqrasiya, hər bildiriş.
- Runbook: dayandırma / yenidən başlatma / tipik xətaların həlli.
- Dəyişiklik təlimatı: sistemlərdən biri dəyişəndə (yeni sahə, yeni versiya)
  nəyə baxmaq lazımdır.
