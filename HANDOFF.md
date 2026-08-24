# HANDOFF - Kestridge AI saytı

Son yenilənmə: 24 avqust 2026
Repo: https://github.com/ismayilzeynal/kestridgeai (əvvəl `testlogo-site` idi)
Branch: `main`. Canlı: https://testlogo-site.vercel.app
Build: `npx next build` təmiz. `npx tsc --noEmit` səhvsiz.
Deploy GitHub push ilə OLMUR - hər dəfə `npx vercel --prod --yes` lazımdır.

**Backend yazan üçün:** `README.md` -> "Backend" bölməsi. Formanın POST
kontraktı (sahələr, honeypot, gözlənilən cavab) orada yazılıb.

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

1. **Form endpoint.** Vercel-də `NEXT_PUBLIC_FORM_ENDPOINT` təyin edin
   (Formspree / Web3Forms / Basin). Kod hazırdır. Dəyişən boş olduğu müddətdə
   forma saxta "göndərildi" demir - istifadəçinin mail proqramını hazır mesajla
   açır.
2. **Domen.** `site.ts` -> `url` hələ `testlogo-site.vercel.app`.
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
   Cari mətnlərin yeganə mənbəyi `src/` fayllarıdır.

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
