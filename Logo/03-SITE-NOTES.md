# Canlı sayt ilə brend sistemi arasındakı fərqlər

| | |
|---|---|
| Sayt | <https://testlogo-site.vercel.app/> |
| Ölçmə tarixi | 2026-08-24 |
| Metod | brauzerdə `getComputedStyle`, `icon.svg` faylı endirildi |
| Fayllar | `delivery/08-site/` - saytdakı fayllar birbaşa əvəz olunur |

Sayt müvəqqəti yerləşdirmədir və dizayn işi bu paketin əhatəsində deyil. Bu
sənəd yalnız **ölçülmüş fərqləri** yazır və hazır əvəzləri göstərir.

---

## 0. Üç fərq, biri risk

| # | Nə | Sayt | Brend sistemi | Ağırlıq |
|---|---|---|---|---|
| 1 | Nişan | **lələk**, yuvarlaq plitə `rx=80` | üç lövhəli `K`, radius `0` | **risk** |
| 2 | Aksent rəng | `#0B7A67` (hue 170) | `#1187A5` petrol-500 (hue 192) | orta |
| 3 | Sözmarka registri | qarışıq, `Kestridge AI` | tam BÖYÜK, `KESTRIDGE AI` | aşağı |

Uyğun gələnlər: **şrift ailəsi onsuz da Archivo-dur**, neytral pilləkən demək
olar eynidir, ad `Kestridge AI` kimi düzgün yazılıb.

---

## 1. Lələk götürülməlidir

Saytın cari `icon.svg`-i:

```
<rect width="512" height="512" rx="80" fill="#0B0B0D"/>
<path d="M60 20C70 45 70 75 60 100C50 75 50 45 60 20Z" fill="#F3E9D2"/>
<path d="M60 34C65 51 65 69 60 86C55 69 55 51 60 34Z" fill="#FFFDF6"/>
```

İki problem, hər ikisi layihənin öz sənədlərindən:

**Birinci - quş təsviri.** `US-TRADEMARK-SCAN.md` bölmə 8 bənd 11:

> Loqoda tanınan yırtıcı quş forması işlətmə. CrowdStrike "the falcon logo"
> iddia edir və o sahə ABŞ kibertəhlükəsizliyində doludur.

Lələk quş təsviridir və şirkət **məhz IT təhlükəsizliyi satır**. Skanın
bütün nəticəsi bu formadan uzaq durmaq üzərində qurulub.

**İkinci - əyri seqment və yuvarlaq künc.** Nişanın spesifikasiyası künc
radiusunu `0` və `C`/`S`/`Q`/`A` komandalarını qadağan edir. Lələk hər iki
qaydanı pozur (`rx=80`, üç `C` komandası).

**Əvəz:** `delivery/08-site/icon.svg` - tünd plitə, knockout nişan, radius `0`.

---

## 2. Aksent rəng - ölçülmüş toqquşma

Saytın aksenti `#0B7A67`. Palitranın brend aksenti `petrol-500 #1187A5`.

Bu, zövq məsələsi deyil. `DESIGN-LANGUAGE.md` bölmə 2.11 bənd 2:

> **Semantik rənglər brend deyil.** `success/warning/danger/info` interfeys
> mülkiyyətidir.

Ölçmə:

| Rəng | Rol | Hue | `success-600`-dən fərq |
|---|---|---|---|
| `#0B7A67` | saytın aksenti | **170** | **20 dərəcə** |
| `#007439` | `success-600`, semantik | 149 | - |
| **`#1187A5`** | **`petrol-500`, brend** | **192** | **43 dərəcə** |

20 dərəcə yaxındır. Şirkət dashboard və monitorinq satır; orada yaşıl
"sağlam / keçdi" deməkdir. Brend rəngi status rəngi ilə eyni ailədən olsa,
məhsulun öz interfeysində brend "hər şey qaydasındadır" siqnalı kimi oxunur.

Petrol o problemi daşımır və palitranın ölçülmüş çalarıdır: WCAG matrisi,
Pantone yaxınlığı və rəng korluğu təhlili onun üzərində aparılıb.

Kontrast (ölçülmüş): `petrol-500` kağızda `3.99`, ink üzərində `4.47` - hər
ikisi `3:1` həddini keçir, yəni ikon, sərhəd və fokus halqası üçün icazəlidir.
**Gövdə mətni deyil** (`4.5`-i keçmir).

**Əvəz:** `delivery/08-site/kestridge-tokens.css`.

---

## 3. Neytral pilləkən - demək olar eyni, dəqiqləşdirilir

| Rol | Sayt | Sistem | Token |
|---|---|---|---|
| Ink | `#111821` | `#0F1317` | `neutral-950` |
| Muted | `#4A5360` | `#434D57` | `neutral-700` |
| Muted 2 | `#636C79` | `#5A646E` | `neutral-600` |
| Kağız / `theme-color` | `#F5F6F9` | `#FAFAF7` | `neutral-025` |

Fərqlər kiçikdir, amma pilləkən bütöv sistemdir - kontrast matrisi məhz bu
dəyərlər üçün hesablanıb. Yarısını götürmək matrisi etibarsız edir.

---

## 4. Sözmarka registri

Sayt `Kestridge AI` yazır, sistem `KESTRIDGE AI` tələb edir. Arqument
`00-CONCEPT-BRIEF.md` 5.3-dədir və endən yox, **hündürlükdən** gəlir:

| | BÖYÜK | qarışıq |
|---|---|---|
| mürəkkəb hündürlüyü (cap = 100) | **103.5** | **131.9** |

Qarışıq registr şaquli qabaritı 27-37 faiz böyüdür və `g` descender-i markanın
öz alt kənarından aşağı düşür - favicon, avatar və möhürdə problem.

Bu, yalnız **loqo kilidinə** aiddir. Səhifə mətnində `Kestridge AI` normal
yazılışdır və dəyişmir.

---

## 5. Hazır fayllar

| Fayl | Sayta hara düşür | Nə |
|---|---|---|
| `icon.svg` | `app/icon.svg` | tünd plitə, knockout nişan, radius `0` |
| `apple-icon.png` | `app/apple-icon.png` | `1024 x 1024` - saytın özünün elan etdiyi ölçü |
| `opengraph-image.png` | `app/opengraph-image.png` | `1200 x 630` |
| `opengraph-image-dark.png` | ehtiyat | tünd variant |
| `favicon.ico` | `public/favicon.ico` | 7 kadr: 16 / 24 / 32 / 48 / 64 / 128 / 256 |
| `header-lockup.svg` | başlıq komponenti | yatıq kilid, AR `8.6055` |
| `header-lockup-dark.svg` | başlıq, tünd fon | eyni |
| `icon-light.svg` | ehtiyat | açıq plitə, müsbət nişan |
| `kestridge-tokens.css` | `styles/` | CSS dəyişənləri |

Ölçülər saytın **öz elan etdiyi** dəyərlərdir: `apple-touch-icon` üçün
`sizes="1024x1024"`, OG üçün doğrulanmış `1200 x 630`.

---

## 6. Toxunulmayan

- Səhifə tərtibatı, bölmə ardıcıllığı, mətn.
- `GeistMono` - sistem `JetBrains Mono` deyir, amma mono yalnız kiçik etiketlərdə
  (`ILLINOIS, UNITED STATES`, `01`, `Areas of work`) işlənir. Dəyişdirmək
  məcburi deyil; dəyişilsə sistemlə uyğunlaşar.
- Şirkət iddiaları ("Companies our team has worked with" siyahısı). Bu paket
  onları yoxlamayıb - `unverified`.
