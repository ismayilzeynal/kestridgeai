# HANDOFF - Kestridge AI saytı

Son yenilənmə: 7 sentyabr 2026
Repo: https://github.com/ismayilzeynal/kestridgeai (əvvəl `testlogo-site` idi)
Branch: `main`. Canlı: https://kestridge.com (www apex-ə 308 yönlənir).
Build: `npx next build` təmiz. `npx tsc --noEmit` səhvsiz.
Vercel layihəsi: `kestridgeai`, GitHub `ismayilzeynal/kestridgeai` reposuna
bağlıdır - `main`-ə push avtomatik production deploy edir. Əl ilə deploy
lazım olsa: `npx vercel --prod`.

**Backend yazıldı:** `backend/` qovluğu, ASP.NET Core 10 + MySQL 8.
Bax: `backend/README.md` (qurulum), `backend/RUNBOOK.md` (əməliyyat),
`backend/DSR-PROCESS.md` (hüquqi sorğular). Formanın POST kontraktı `README.md`
-> "Backend" bölməsində dəqiqləşdirildi: 7 sahə göndərilir, 5 yox.

Mətnlərin köhnə/yeni müqayisəsi (sahibin təsdiqi üçün):
https://claude.ai/code/artifact/9fecfe06-c7f9-4613-8405-1ada916d16b6

---

## 1. Bitmiş işlər

**Domen: kestridge.com canlıdır.** NS-lər onsuz da Vercel-ə yönəlmişdi, amma
domen layihəyə əlavə edilmədiyi üçün Vercel-in nameserver-ləri öz hostlamadığı
zona üçün SERVFAIL qaytarırdı - domen heç yerə açılmırdı. Apex + www layihəyə
əlavə olundu, `www` apex-ə 308 yönləndirildi, Vercel layihəsi `testlogo-site` ->
`kestridgeai` adlandırıldı və köhnə `testlogo-site.vercel.app` silindi (indi 404).
`site.ts` -> `url` tək mənbədir: canonical, sitemap, robots, OG və JSON-LD
hamısı ondan törəyir.

**Brend** AIVanta -> Kestridge AI. `site.ts`, metadata, manifest, logo, brand SVG,
privacy/terms, senedler/, README, TAMAMLANACAQ-ISLER.txt. Kodda və sənədlərdə
`AIVanta` sıfır. Köhnə OG şəkli (üstündə AIVanta yazılıydı) silindi; indi
`app/opengraph-image.tsx` onu `site.brand`-dan çəkir.

**About birləşdirildi.** `/about` route silindi -> `Company` bölməsi (`#company`).
`/about` -> `/#company` 308 redirect. Sitemap və footer yeniləndi.

**Səhifə sırası:** Hero, TrustBar, Company, Services, Process, Founders,
Security, Questions, Contact. `CTABand` silindi (ən reklamvari blok idi).
Bölmə id-ləri: `top company services process founders security questions contact`.

**Hero:** "Start a project" düyməsi silindi (navbar-da var). Tək düymə:
"See our services".

**FAQ:** accordion tamamilə çıxarıldı. İndi həmişə açıq `dl/dt/dd` şəbəkəsi,
8 qısa sual, 1 sütun mobil / 2 sütun sm / 3 sütun xl.

**Təsisçilər:** LinkedIn-dən 4 real şəxs. Şəxsi profil linkləri qoyulmadı.
Faig və Chingiz-in şəkli var; Sarvjeet və Robert-in şəkli LinkedIn-də yoxdur,
baş hərfli avatar göstərilir. randomuser.me placeholder-ləri silindi.

**Loqo lenti:** 17 real qurum, loqolar lokal (`public/logos/`). Başlıq
"Where our founders have worked" (əvvəlki mətn onları müştəri kimi göstərirdi).

**Mətnlər:** 298 sətir yenidən yazıldı, sonra ikinci dəfə qısaldıldı.
Görünən mətnlərin yalnız 2-si 22 sözdən uzundur (hər ikisi 23 söz).
Bütün başlıqlar mövzunu adlandırır, düymələr hərfi mənada nə etdiyini deyir,
em dash/en dash sıfır. Tam qeyd: `copy-deck.json`.

**QA (42 təsdiqlənmiş tapıntı düzəldildi):**
- `public/logos` və `public/team` git-də deyildi -> deploy-da bütün şəkillər 404
  olacaqdı. Stage edildi.
- Navbar `md` -> `lg`: 768-930px arası "Contact us" düyməsi ekrandan kənarda
  qalırdı. MobileCTA `sm:hidden` -> `lg:hidden` (640-767px-də heç bir əlaqə
  düyməsi yox idi).
- Skip link və bütün anchor-lar scroll edirdi, amma klaviatura fokusunu
  aparmırdı. `lib/scroll.ts` indi fokusu da köçürür.
- Marquee `aria-hidden` bütün 17 şirkət adını ekran oxuyucusundan gizlədirdi ->
  `sr-only` siyahı əlavə olundu.
- Security sticky sütunu `overflow-hidden` ucbatından heç vaxt pinlənmirdi.
- Security kartları `Stagger` səbəbindən fərqli hündürlükdə idi.
- `--text-faint` 4.19:1 idi (4.5:1 lazımdır) -> düzəldildi.
- Privacy Policy "analitika yalnız razılıqla işləyir" deyirdi, amma Vercel Web
  Analytics həmişə işləyir -> açıqlama əlavə olundu, vəd dəqiqləşdirildi.
- Privacy/Terms öz OG metadata-sını almırdı (ana səhifəninkini götürürdü).
- Footer h2 -> h4 səviyyə atlayırdı -> h3.
- Mobil menyu `aria-modal` deyirdi, amma fokus çölə çıxırdı -> inert genişləndi.
- Accordion `aria-controls` bağlı vəziyyətdə mövcud olmayan id-ə işarə edirdi.
- Form xəta elanı heç vaxt oxunmurdu (live region sonradan yaranırdı).
- Ölü kod: `SectionHeading.tsx`, `next.config.mjs` içindəki `images` bloku.

---

## 1a. Backend (yeni)

**Backend yazıldı: `backend/` qovluğu.** ASP.NET Core 10 (`net10.0`) + EF Core 9 +
Pomelo 9.0.0 + MySQL 8. Bir proses, iki endpoint, üç cədvəl. Tam sənəd:
`backend/README.md`, əməliyyat: `backend/RUNBOOK.md`, hüquqi sorğular:
`backend/DSR-PROCESS.md`.

**Frontend heç dəyişmədi.** `Contact.tsx`-ə bir hərf də toxunulmayıb. Yeganə
frontend işi Vercel-də bir dəyişəni qoymaqdır:
`NEXT_PUBLIC_FORM_ENDPOINT = https://api.kestridge.com/api/contact`, sonra
**rebuild** (redeploy yox - `NEXT_PUBLIC_*` build vaxtı bundle-a yazılır).

Nə edir:

1. Formanın POST-unu olduğu kimi qəbul edir (7 multipart hissə, DOM sırası ilə).
2. Hər müraciəti `contact_submissions` cədvəlinə yazır.
3. Komandaya plain-text bildiriş göndərir - **sorğu yolundan kənarda**, sətir
   üzərində saxlanan davamlı retry ilə (1d, 5d, 15d, 1s, 4s, 12s, 24s; cəmi ~41
   saat, gecəni və həftəsonunu keçir).
4. 24 aydan sonra müraciətləri silir (legal hold istisna), hər silmə `job_runs`-a
   yazılır - vədin yerinə yetirildiyinin sübutu.
5. `GET /api/health` - uptime monitorinq üçün.

Qərəzli olaraq **yazılmayanlar** (səbəbləri `backend/README.md`-də cədvəldir):
kontent CMS (services/team/FAQ DB-də), admin API/UI, cookie, ayrıca outbox
cədvəli, `ip_address`/`user_agent` sütunları, CAPTCHA, ziyarətçiyə avtocavab.
Hər biri üçün "nə vaxt yenidən baxılsın" şərti yazılıb.

Testlər: `dotnet test`. MySQL qurulmayıbsa DB testləri **skip** olur (səbəb
yazılır), CI-də isə fixture exception atır - yəni yaşıl CI heç vaxt boş olmur.

İlk qurulum:

```powershell
cd backend
dotnet tool restore
powershell -ExecutionPolicy Bypass -File ops/setup-dev-db.ps1
```

Skript MySQL root parolunu soruşur, beş servis hesabı üçün parol generasiya edir,
DB-ləri və grant-ları qurur, migrasiyanı tətbiq edir, connection string-i
`dotnet user-secrets`-ə yazır və qalan parolları bir dəfə çap edir. Repoda heç
bir parol saxlanmır.

**Bloklayıcı:** `info@kestridge.com` hələ də açılmır (zonada MX yoxdur). Ona görə
həm bildiriş ünvanı, həm də Privacy Policy-nin elan etdiyi hüquqi sorğu kanalı
işləmir. Poçt qurulana qədər `Kestridge__Contact__ToAddress`-i real oxunan bir
qutuya yönəldin.

**Cookie yoxdur.** Backend heç bir cookie yaratmır - `CookieConsent.tsx`
toxunulmamış qalır və hələ də yalnız `NEXT_PUBLIC_GA_ID`-ə bağlıdır. Bunu
devtools-da təsdiqləyin (`/api/contact` cavabında sıfır `Set-Cookie`), sonra
TAMAMLANACAQ-ISLER.txt bölmə 5-dəki bəndi işarələyin.

**Privacy Policy-yə iki düzəliş lazımdır, amma mən onları etmədim** (dərc olunmuş
hüquqi mətndir, hüquqşünas yoxlaması hələ gözlənilir). Dəqiq mətn:
`backend/DSR-PROCESS.md` sonundakı siyahı. Qısaca: (a) saxlama müddəti kimi 24 ay
adlandırılmalıdır, (b) "service providers" siyahısına **database hosting** və
**email delivery** əlavə olunmalıdır. Hər ikisi yerləşəndə `privacy/page.tsx`
sətir 31-dəki `UPDATED` tarixi yenilənməlidir.

Repo tərəfi dəyişiklikləri:

- `tsconfig.json` -> `exclude` siyahısına `backend` əlavə olundu. **Bu vacibdir:**
  `include` qlobu `**/*.ts` repo kökündən başlayır, yəni `backend/` altına düşən
  istənilən `.ts` faylı Next-in tip proqramına girir və oradakı bir tip səhvi
  Vercel-də `next build`-i sındırır.
- `.gitignore` -> .NET artefaktları, user-secrets qalıqları, backup qovluğu.
  Həm də köhnə boşluq bağlandı: `.env.production` və `.env.development`
  ignore olunmurdu, indi `.env*` + `!.env.example`.
- `.vercelignore` (yeni) -> `npx vercel --prod` işçi qovluğu yükləyir, `.gitignore`
  ona təsir etmir. Bunsuz hər əl ilə deploy-da `bin/`, `obj/` və NuGet keşi
  Vercel-ə gedirdi.
- `vercel.json` + `scripts/vercel-ignore.sh` (yeni) -> yalnız `backend/`,
  `senedler/`, `Logo/` dəyişəndə build atlanır. Exit kodları tərsdir: Vercel
  **0-da build-i atlayır**. Skript bütün naməlum hallarda build tərəfə düşür.

**Diqqət:** `vercel.json` deploy davranışını dəyişir. İstəməsəniz `vercel.json`
və `scripts/vercel-ignore.sh` fayllarını silin, qalan hər şey işləməyə davam edir.

---

## 2. Sizdən gözlənilən

1. **Poçt.** Provayder hələ seçilməyib (cavab gözlənilir). Seçiləndən sonra
   onun MX + SPF + DKIM yazıları Vercel DNS zonasına əlavə olunmalıdır - zona
   indi boşdur, ona görə `info@kestridge.com`-a yazılan məktub hazırda geri
   qayıdır. Domen işi bunu pozmayıb, poçt heç vaxt qurulmamışdı.
   `npx vercel dns add kestridge.com "" MX <host> <priority>`
   Bu, launch üçün maneədir: sayt həmin ünvanı Contact bölməsində, footer-də və
   JSON-LD-də elan edir, forma da endpoint boş olduğu müddətdə ora `mailto:` ilə
   yönəldir.
2. **Admin hesabı.** Panel `https://api.kestridge.com/admin/` ünvanında
   işləyir, amma bir dənə də hesab yoxdur. Parolu maşın yaza bilməz:
   `cd /srv/kestridge-api && dotnet Kestridge.Api.dll --hash-password --username <ad>
   --display-name "Ad Soyad"` (məhz dotnet ilə: quraşdırma qovluğundakı
   heç bir faylda icra biti yoxdur), çap olunan `INSERT`-i `ops/admin-account.sql`
   ilə `kestridge_migrator` kimi işlədin, `otpauth://` linkini authenticator
   tətbiqinə skan edin. TOTP məcburidir.
   (Form endpoint artıq qoşulub və canlı yoxlanılıb.)
3. **Sarvjeet-in soyadı** (özündən dəqiqləşdiriləcək) və **Sarvjeet + Robert
   üçün şəkil**. Hazırda baş hərfli avatar göstərilir.
4. **Dəstək şərtləri.** Canlıya çıxandan sonrakı dəstək pulsuz deyil, sayt bunu
   demir.

---

## 3. Texniki qalıqlar

1. **Anchor offset-ləri padding-ə bağlıdır.** Hər bölmə
   `scroll-mt = 6rem - öz padding-top-u` daşıyır (breakpoint başına). Bölmənin
   `py` dəyərini dəyişsəniz, `scroll-mt` də dəyişməlidir. Səbəbi: brauzerin öz
   `#hash` sıçrayışı ilə JS animatoru eyni nəticəni verməlidir.
2. **Bütün ölçülər rem-dir.** Klasslarda arbitrary `px` qalmayıb (yalnız 1-3px
   hairline-lar). Root `106.25%`-dir, yəni oxucunun brauzer şrift ölçüsünü izləyir.
3. `copy-deck.json` yalnız ilk yazılış qeydidir, sonrakı redaktələr orada yoxdur.
4. **`src/data/*.ts` artıq yeganə mənbə deyil.** Xidmətlər, banilər, suallar və
   loqolar MySQL-dədir və `getContent()` ilə oxunur; həmin fayllar API
   əlçatmaz olanda işə düşən fallback-dir. Onlar öz-özünə yenilənmir, ona görə
   `RUNBOOK.md`-də rüblük yeniləmə addımı var. Vercel-dən `API_ORIGIN`-i
   silmək bütün sayt üçün geri qaytarmadır.
5. **`senedler/` və bu fayl açıq repodadır.** Repo public-dir; daxili sənədlərin
   orada qalması qərarı hələ verilməyib.

---

## 4. Əsas qərarlar

| Qərar | Səbəb |
|---|---|
| CTABand silindi | Ən reklamvari blok, altında onsuz da Contact var |
| Bütün bölmə eyebrow-ları silindi | Başlıqlar tək başına təmiz plan yaradır |
| Loqo lenti saxlanıldı, başlığı dəqiqləşdirildi | Siz istəmişdiniz; yanlış təəssürat riski başlıqla həll olundu |
| Təsisçi adları dərc edildi | Real məlumat gəldi |
| Loqolar lokal yükləndi | LinkedIn CDN URL-ləri 2027-də bitir |
| twitter-image route silindi | Next opengraph-image-i Twitter üçün də işlədir |
| Hero-nun 3 sətrlik animasiyası sadələşdirildi | Yeni cümlə bəzi enlərdə sətir bölgüsünü sındırırdı |
| Navbar `lg`-də açılır, `md`-də yox | 6 nav elementi + düymə ~930px istəyir |

---

## 5. Uğursuz yanaşmalar (təkrarlamayın)

- LinkedIn-də `shrink_100_100` -> `shrink_400_400` = 403. Token yola bağlıdır.
  Bu interfeys versiyası yalnız 100x100 verir.
- LinkedIn şirkət səhifələrini `fetch` ilə oxumaq: SPA qabığı gəlir, `og:image`
  yoxdur. Loqoları iş təcrübəsi səhifəsindən DOM-dan götürmək lazımdır.
- Profil şəklini `document.querySelector('img')` ilə tapmaq: ilk şəkil sizin öz
  naviqasiya avatarınızdır. Düzgün üsul: `main` daxilində, en >= 100px, yuxarıda.
- `twitter-image.tsx`-i re-export kimi yazmaq: Next `runtime` sahəsini literal
  görmür, node runtime-a düşür, `@vercel/og` qovluq adındakı "Ş" hərfinə görə
  `Invalid URL` atır.

---

## 6. Davam etmək

```bash
cd "C:/Users/Emil/Desktop/Projects/Kestridge"
npx next dev -p 3111
npx next build

# Məcburi qəbul yoxlaması: ölü API ünvanı ilə build keçməli və marşrut
# cədvəlində "/" hələ də static (dairə) qalmalıdır.
API_ORIGIN=http://127.0.0.1:1 npm run build

cd backend && dotnet test
```

LinkedIn üçün CDP ilə Chrome (giriş yadda saxlanılıb):

```bash
"C:/Program Files/Google/Chrome/Application/chrome.exe" \
  --remote-debugging-port=9222 \
  --user-data-dir="$LOCALAPPDATA/Temp/claude/cdp-profile"
```
