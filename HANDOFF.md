# HANDOFF - Kestridge AI saytı

Son yenilənmə: 24 avqust 2026
Branch: `main`. Commit edilməyib; `public/logos/` və `public/team/` **stage-dədir**.
Build: `npx next build` təmiz. `npx tsc --noEmit` səhvsiz.

Mətnlərin köhnə/yeni müqayisəsi (sahibin təsdiqi üçün):
https://claude.ai/code/artifact/9fecfe06-c7f9-4613-8405-1ada916d16b6

---

## 1. Bitmiş işlər

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

## 2. Sizdən gözlənilən

1. **Logo.** Hazır olanda dəyişməli fayllar: `public/brand/`, `src/app/icon.svg`,
   `src/app/apple-icon.png`, `src/app/opengraph-image.tsx`.
   `media-logo-assets/` bütünlüklə köhnə brenddir - **silmək lazımdır** (7 SVG-nin
   içində hələ də "AIVanta" yazısı var; TAMAMLANACAQ-ISLER.txt sizi o faylları
   LinkedIn-ə yükləməyə yönləndirir). Deyin, silim.
2. **Domen.** `site.ts` -> `url` hələ `testlogo-site.vercel.app`.
3. **Sarvjeet-in soyadı.** Digər üçü tam addır; o, tək adla və tək hərfli
   avatarla qalıb.
4. **Sarvjeet + Robert üçün şəkil** (istəsəniz).
5. **Telefon nömrəsi.** Form müştəridən nömrə istəyir, sayt özü nömrə vermir.
6. **Qiymət.** "How is cost determined?" var, rəqəm yoxdur. Səhifədəki ən böyük
   cavabsız sual.
7. **Dəstək şərtləri.** Canlıya çıxandan sonrakı dəstək pulsuz deyil, sayt bunu
   demir.
8. **Xaricdəki mütəxəssislər hansı ölkələrdədir, müştəri datası ABŞ-dan çıxırmı?**

---

## 3. Texniki qalıqlar

1. **Contact form saxtadır.** `Contact.tsx` -> `onSubmit` `setTimeout` ilə saxta
   "success" verir. Launch üçün real backend lazımdır (API route + Resend, ya da
   Formspree). Spam qoruması da (honeypot / Turnstile).
2. **Commit edilməyib.** `git status` ilə baxın. `public/logos` və `public/team`
   stage-dədir, qalanı işçi qovluqdadır.
3. **Mobil vizual yoxlama yarımçıqdır.** Kod səviyyəsində breakpoint-lər
   yoxlanıldı və düzəldildi; 375px-də real gözlə baxılmadı, çünki brauzer
   sizinlə paylaşılırdı.
4. `copy-deck.json` və `qa-plan` referans üçün kökdədir; lazım deyilsə silin.

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
cd "c:/Users/Asus/Desktop/ABŞ servis/only front"
npx next dev -p 3111
npx tsc --noEmit
npx next build
```

LinkedIn üçün CDP ilə Chrome (giriş yadda saxlanılıb):

```bash
"C:/Program Files/Google/Chrome/Application/chrome.exe" \
  --remote-debugging-port=9222 \
  --user-data-dir="$LOCALAPPDATA/Temp/claude/cdp-profile"
```
