# Kestridge AI - brend paketi

Loqonun, ikonların və materialların hamısı buradadır. Nömrəli qovluqlar istifadə sırasına görə düzülüb.

**Əsas loqo rənglidir:** gövdə qara, `i` və `A`-nı quran hissələr saytın petrol rəngi `#0E6A82`. Tünd fonda gövdə ağ, petrol `#2FA8C7`. Tək rəngli versiyalar `mono/` qovluqlarındadır - çap, oyma, faks üçün.

## Tez tapmaq üçün

| Nə lazımdır | Fayl |
|---|---|
| Loqo, ağ fonda, istənilən ölçü | `01-logo-svg/color/kestridge-lockup-horizontal-default-color.svg` |
| Loqo, tünd fonda | `01-logo-svg/color/kestridge-lockup-horizontal-default-color-knockout.svg` |
| Yalnız nişan (K) | `01-logo-svg/color/kestridge-mark-color.svg` |
| Loqo PNG, sənədə qoymaq üçün | `02-logo-png/color/kestridge-lockup-horizontal-default-color-512.png` |
| Sayt ikonu | `03-app-icons/favicon.ico` |
| LinkedIn örtüyü | `09-linkedin-banner/kestridge-banner-linkedin-cover-1512x256.png` |
| LinkedIn profil şəkli | `07-platforms/linkedin-shirket-sehifesi-company-page-logo-profile-400x400.png` |
| Vizit kartı | `05-stationery/business-card-front.png`, `business-card-back.png` |
| Qaydalar | `00-docs/02-BRAND-GUIDELINES.md` |

`default` = `KESTRIDGE AI`, `reserve` = `KESTRIDGE` (AI-siz). `horizontal` = nişan solda, `stacked` = nişan üstdə.

## Qovluqlar

| Qovluq | Nədir | Fayl |
|---|---|---|
| `00-docs/` | Sənədlər | 7 |
| `01-logo-svg/` | Loqo, vektor (SVG) | 34 |
| `02-logo-png/` | Loqo, şəkil (PNG, JPG, WebP) | 153 |
| `03-app-icons/` | Tətbiq ikonları | 7 |
| `04-logo-pdf/` | Loqo, PDF | 10 |
| `05-stationery/` | Ofis materialları | 9 |
| `06-fonts/` | Şriftlər | 6 |
| `07-platforms/` | Platforma şəkilləri | 35 |
| `08-website/` | Sayt | 9 |
| `09-linkedin-banner/` | LinkedIn banneri | 2 |

### `00-docs/` - Sənədlər

Əvvəl `02-BRAND-GUIDELINES.md`-i oxu: loqonu harada, hansı ölçüdə, hansı rəngdə işlətmək. Qalanları qərarların səbəbi və texniki qeydlərdir.

### `01-logo-svg/` - Loqo, vektor (SVG)

Çap və dizayn üçün əsl mənbə, istənilən ölçüyə keyfiyyət itkisiz böyüyür. `color/` - rəngli loqo, əsas versiya. `mono/` - tək rəngli: çap, oyma, faks.

### `02-logo-png/` - Loqo, şəkil (PNG, JPG, WebP)

Hazır ölçülərdə. `color/` və `mono/` ayrıdır. Nişan 16-1024 px, şəffaf fonda; kilidlər 64-1024 px hündürlükdə, açıq fonda.

### `03-app-icons/` - Tətbiq ikonları

`favicon.ico` və kvadrat / dairəvi plitələr.

### `04-logo-pdf/` - Loqo, PDF

Çapçıya veriləcək vektor fayllar. `color/` və `mono/` ayrıdır.

### `05-stationery/` - Ofis materialları

Vizit kartı, blank, zərf, slayd şablonları, e-poçt imzası.

### `06-fonts/` - Şriftlər

Archivo, Inter, JetBrains Mono - lisenziyaları ilə (OFL).

### `07-platforms/` - Platforma şəkilləri

LinkedIn, X, YouTube, GitHub və veb standartları üçün, hər biri öz ölçüsündə. Fayl adı platformanın adı ilə başlayır.

### `08-website/` - Sayt

Başlıq loqosu, sayt ikonları, OpenGraph şəkilləri və rəng tokenləri (CSS).

### `09-linkedin-banner/` - LinkedIn banneri

Şirkət səhifəsinin örtüyü (1512x256) və profil fonu (1584x396).

## Bu paketdə olmayanlar

- **Köhnə versiyalar** - `../archive/v1-2026-08-24/` (üç lövhəli nişan) və `../archive/v2-2026-09-21/` (beş lövhəli, tək rəngli). Silinməyib.
- **Ölçü qeydləri və yoxlama şəkilləri** - `../qa/`. Paketin iddialarının sübutudur, istifadə üçün deyil.
- **Generator** - `../tools/build/`. Paketi yenidən qurmaq (layihə kökündən): `python tools/build/pack.py`.

Cəmi 272 fayl. Hər faylın sha256-sı `MANIFEST.json`-dadır.
