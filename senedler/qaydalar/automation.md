# Automation - İş Qaydaları

[Ümumi qaydalar](00-umumi.md) tam keçərlidir. Aşağıdakılar
avtomatlaşdırma layihələrinə xas əməli qaydalar, tool stack və
prosedurlardır.

## 1. Tool stack (standart)

| Məqsəd | Tool | Nə vaxt |
| --- | --- | --- |
| Low-code axın | **n8n** (self-host, data-privacy dostu) | Standart seçim |
| Low-code (müştəri stack-i) | Make / Power Automate / Zapier | Müştəri artıq istifadə edirsə |
| Xüsusi məntiq | **Python** | API/low-code çatmayanda |
| RPA (interfeys) | **Playwright** (və ya UiPath) | Yalnız API olmayanda |
| Secrets | Vault (1Password/Bitwarden) | Kodda/axında açıq saxlanmır |
| Monitorinq | Axın platformasının logu + alert | Hər run izlənir |

**Platforma hesabı müştərinin adına açılır** - bizdən asılı qalmır.

## 2. Müştəridən nə istəyirik

- **Prosesin sahibi** - prosesi bu gün icra edən konkret adam(lar).
- **Test hesabları** - inteqrasiya olunacaq hər sistemdə (real istifadəçi
  hesabı ilə işləmirik).
- **Test mühiti** - sandbox/test instansı; yoxdursa yalnız razılaşdırılmış
  təhlükəsiz data.
- **İş mühiti** - axın harada işləyəcək: müştərinin VM-i (standart spec:
  Ümumi qaydalar) və ya platforma hesabı.

## 3. Qurulma qaydaları (hər axında məcburi)

Hər avtomatlaşdırma bu 5 şeyə malik olmalıdır - **istisna yoxdur:**

1. **Idempotency** - eyni əməliyyat təkrar işləsə, nəticə dublikat olmur
   (məs. eyni email iki dəfə getmir, eyni ödəniş iki dəfə olmur).
2. **Retry + backoff** - müvəqqəti xəta olanda eksponensial gecikmə ilə
   yenidən cəhd; sonda uğursuzsa **dead-letter** (bir yerə düşür, itmir).
3. **Kill switch** - müştəri istənilən an prosesi özü dayandıra bilir;
   yeri runbook-da.
4. **Loglama** - hər əməliyyat: nə vaxt, nə edildi, nəticə. Geriyə izlənir.
5. **Sahib (owner)** - hər axının bir məsulu var.

Əlavə:

- **API-first:** API varsa API ilə; RPA yalnız API olmayanda (interfeys
  dəyişəndə sınır).
- **Rate limit / kvota** hörmət edilir - müştəri sistemini yükləmə/flood
  etmə (unudulan risk: sonsuz döngə → minlərlə dublikat email).
- Maliyyə / geri dönməz əməliyyatlar (ödəniş, silmə, müştəriyə email) ilk
  mərhələdə **təsdiq addımı ilə**; tam avtomata keçid yalnız paralel dövr
  bitəndən sonra, yazılı razılıqla.
- API auth: OAuth / scoped token, refresh düzgün idarə olunur; açarlar
  vault-da.

## 4. Paralel dövr qaydası

- Yeni avtomatlaşdırma **min. 2 həftə** köhnə proseslə yanaşı işləyir.
- Nəticələr gündəlik tutuşdurulur; fərq çıxarsa cutover olmur, dövr uzanır.
- Köhnə prosesə toxunulmur - geri dönüş yolu açıq.

## 5. Test və deploy

- Axın əvvəl **test/staging**-də qurulur, prod-a yalnız işlədiyi
  təsdiqlənəndən sonra.
- Axın versiyalanır (export + Git); dəyişiklik change request ilə.

## 6. Təhvildə əlavə olaraq

- **Axın xəritəsi:** hər addım, hər inteqrasiya, hər bildiriş
  (trigger → action → owner).
- **Runbook:** dayandırma / yenidən başlatma / tipik xətaların həlli.
- **Dəyişiklik təlimatı:** sistemlərdən biri dəyişəndə (yeni sahə, yeni
  versiya) nəyə baxmaq lazımdır.
