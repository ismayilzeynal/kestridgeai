# Canlı sayt ilə brend sistemi arasındakı fərqlər (v3)

| | |
|---|---|
| Sayt | <https://testlogo-site.vercel.app/> |
| Saytın ölçmə tarixi | 2026-08-24 |
| Metod | brauzerdə `getComputedStyle`, `icon.svg` faylı endirildi |
| Bu buraxılış | v3, 2026-09-26: **iki rəngli nişan** (`color` / `color-knockout`); həndəsə, kilid nisbəti `M / C = 2 / 1` və reduktiv kəsik v2 ilə eyni |
| Əvvəlki buraxılış | v2, 2026-09-21, beş lövhəli nişan, kilid nisbəti `M / C = 2 / 1` (`archive/v2-2026-09-21/`) |
| Fayllar | `delivery/08-website/` - saytdakı fayllar birbaşa əvəz olunur |
| Generator və yoxlama | `tools/build/sitev2.py`, `tools/build/verify_site.py`; ölçülər `qa/08-website-measurements.json`; yenidən qurmaq: `python tools/build/pack.py` |

Sayt müvəqqəti yerləşdirmədir və dizayn işi bu paketin əhatəsində deyil. Bu
sənəd yalnız **ölçülmüş fərqləri** yazır və hazır əvəzləri göstərir.

**`testlogo-site.vercel.app` nə v2, nə v3 keçidində yenidən ölçülməyib.** v3 qərarında
yalnız `kestridge.com`-un `--accent` dəyəri oxunub (`#0E6A82`, bölmə 2-dəki v3 qeydi).
1-4 bölmələrindəki sayt rəqəmləri 2026-08-24 ölçməsindəndir və olduğu kimi saxlanılır. v2 keçidinin
öz ölçmələri 5-9 bölmələrindədir, v3 keçidininki bölmə 11-də və 1, 6.1, 7.3, 8, 9.1 bölmələrindəki v3 qeydlərindədir: onlar
**istehsal olunmuş fayllardan** oxunub, niyyətdən yox.

---

## v3-də nə dəyişdi (2026-09-26) - tətbiq edən üçün qısa siyahı

1. **Qovluq:** fayllar `delivery/08-website/`-dədir (v2-də `v2-ai/delivery/08-site/`,
   v1-də `delivery/08-site/` idi). Fayl adları dəyişməyib, doqquzu da yerindədir.
2. **Nişan iki rənglidir.** Gövdə ink, qalan dörd lövhə petrol: açıq fonda
   `color` (`#0F1317` + `#0E6A82`), tünd fonda `color-knockout` (`#FAFAF7` +
   `#2FA8C7`). Həndəsə dəyişməyib (bölmə 11.1, 11.2).
3. **Başlıq kilidləri:** `header-lockup.svg` = `color`, `header-lockup-dark.svg`
   = `color-knockout`. AR `6.1453`, `viewBox` və döşəmələr v2 ilə eynidir -
   bölmə 5 olduğu kimi qüvvədədir.
4. **İkonlar:** `icon.svg` tünd plitədə `color-knockout`; `icon-light.svg`
   `color`; `apple-icon.png` `color-knockout`, surface `#171D23` üzərində;
   `favicon.ico` `color`, `16 px` reduktiv kadr da iki rənglidir (bölmə 6.1).
5. **OpenGraph:** kilid `color-knockout`-dur. Ən zəif mürəkkəb `petrol-400`:
   `6.70:1` (`#0F1317` sahə), `7.55:1` (`#000000` sahə). v2-də kilid yalnız
   `paper` idi (bölmə 7.3).
6. **Tokenlər:** `kestridge-tokens.css`-in dəyərləri dəyişməyib. Nişanın
   petrolları faylda artıq var: `--k-petrol-600` və `--k-petrol-400`.
   `--k-accent` (`petrol-500`) nişanın rəngi **deyil** (bölmə 11.4).
7. **Şriftlər:** `06-fonts/` dəyişməyib (bölmə 9.1).

---

## 0. Fərqlər

Üçü saytdan gəlir və dəyişməyib. İkisi v2-dən gəlir və saytı tətbiq edənin
bilməli olduğu yeni qaydalardır. Altıncı v3-dən gəlir (2026-09-26, bölmə 11).

| # | Nə | Sayt / v1 | v2 sistemi | Ağırlıq |
|---|---|---|---|---|
| 1 | Nişan | **lələk**, yuvarlaq plitə `rx=80` | **beş lövhəli** `KAi`, radius `0` | **risk** |
| 2 | Aksent rəng | `#0B7A67` (hue 170; 2026-08-24, `testlogo-site`; `kestridge.com`-da v3 qərarında `#0E6A82` ölçülüb - bölmə 2, v3 qeydi) | `#1187A5` petrol-500 (hue 192) | orta |
| 3 | Sözmarka registri | qarışıq, `Kestridge AI` | tam BÖYÜK, `KESTRIDGE AI` | aşağı |
| 4 | Kilid nisbəti | `M / C = 4 / 3`, ink AR `8.6055` | `M / C = 2 / 1`, ink AR `6.1453` | **başlıq yenidən ölçülür** |
| 5 | Kiçik ölçü | tək kəsik | `24` lövhə pikselindən aşağı **reduktiv kəsik** | **favicon** |
| 6 | Nişanın rəngi | lələk, `#F3E9D2` / `#FFFDF6` | v2-də tək rəng; **v3-də iki rəng**: ink `K`, petrol `Ai` | **bütün nişan faylları** |

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

**Birinci - quş təsviri.** `docs/research/US-TRADEMARK-SCAN.md` bölmə 8 bənd 11:

> Loqoda tanınan yırtıcı quş forması işlətmə. CrowdStrike "the falcon logo"
> iddia edir və o sahə ABŞ kibertəhlükəsizliyində doludur.

Lələk quş təsviridir və şirkət **məhz IT təhlükəsizliyi satır**. Skanın
bütün nəticəsi bu formadan uzaq durmaq üzərində qurulub.

**İkinci - əyri seqment və yuvarlaq künc.** Nişanın spesifikasiyası künc
radiusunu `0` və `C`/`S`/`Q`/`A` komandalarını qadağan edir. Lələk hər iki
qaydanı pozur (`rx=80`, üç `C` komandası).

**Əvəz:** `08-website/icon.svg` (v2-də `08-site/icon.svg`) - tünd plitə,
knockout nişan, radius `0`. Ölçüldü: `5` yol, `rx` və `ry` yoxdur,
`M`/`L`/`Z`-dən başqa komanda yoxdur, mürəkkəb / plitə nisbəti `0.750000`.

Tək rəngli knockout nişan **v3-də əvəz olunub (2026-09-26)**: `icon.svg` indi
`#171D23` plitədə `color-knockout` daşıyır - gövdə `#FAFAF7`, qalan dörd lövhə
`#2FA8C7`. Həndəsə və yuxarıdakı ölçmələr dəyişməyib
(`qa/08-website-measurements.json`: `5` lövhə, `frac 0.75`, radius `0`). Bax
bölmə 11.

v1-dən fərq: yol sayı `3`-dən `5`-ə qalxdı. Nişan artıq `K`, `i` və `A`-nı
birdən oxuyur; əlavə iki lövhə **nöqtə** (`tittle`) və **tir** (`crossbar`)
adlanır və nişanın örtük qatıdır.

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

Kontrast (`DESIGN-LANGUAGE.md` 2.7 matrisindən): `petrol-500` kağızda `3.99`,
ink üzərində `4.47` - hər ikisi `3:1` həddini keçir, yəni ikon, sərhəd və fokus
halqası üçün icazəlidir. **Gövdə mətni deyil** (`4.5`-i keçmir).

**Əvəz:** `08-website/kestridge-tokens.css` (v2-də `08-site/`). v2-də tək
dəyər yox, **bütöv petrol pilləkəni** verilir - bax bölmə 8.

**v3 qeydi (2026-09-26).** v3 nişanı `petrol-500`-ü işlətmir: açıq fonda
`petrol-600 #0E6A82`, tünd fonda `petrol-400 #2FA8C7` (bölmə 11). v3 qərarında
`petrol-600` `kestridge.com`-un öz `--accent` dəyəri kimi ölçülüb. Bu bölmədəki
`#0B7A67` isə 2026-08-24-də `testlogo-site.vercel.app`-dan ölçülüb. İki ölçmə
fərqli ünvanlardandır və burada yenidən yoxlanmayıb; `kestridge.com`
ölçməsi 2026-09-26-dadır (`01-DECISION-RECORD.md` bölmə 12.5).

---

## 3. Neytral pilləkən - demək olar eyni, dəqiqləşdirilir

| Rol | Sayt | Sistem | Token |
|---|---|---|---|
| Ink | `#111821` | `#0F1317` | `neutral-950` |
| Muted | `#4A5360` | `#434D57` | `neutral-700` |
| Muted 2 | `#636C79` | `#5A646E` | `neutral-600` |
| Kağız / `theme-color` | `#F5F6F9` | `#FAFAF7` | `neutral-025` |

Fərqlər kiçikdir, amma pilləkən bütöv sistemdir - kontrast matrisi məhz bu
dəyərlər üçün hesablanıb. Yarısını götürmək matrisi etibarsız edir. v2-də
pilləkənin **hamısı** (16 neytral pillə) CSS-ə düşür, ona görə matrisin hər
sətri tətbiq edənin əlindədir.

---

## 4. Sözmarka registri

Sayt `Kestridge AI` yazır, sistem `KESTRIDGE AI` tələb edir. Arqument
`docs/concept/00-CONCEPT-BRIEF.md` 5.3-dədir və endən yox, **hündürlükdən** gəlir:

| | BÖYÜK | qarışıq |
|---|---|---|
| mürəkkəb hündürlüyü (cap = 100) | **103.5** | **131.9** |

Qarışıq registr şaquli qabaritı 27-37 faiz böyüdür və `g` descender-i markanın
öz alt kənarından aşağı düşür - favicon, avatar və möhürdə problem.

Bu, yalnız **loqo kilidinə** aiddir. Səhifə mətnində `Kestridge AI` normal
yazılışdır və dəyişmir.

---

## 5. Başlıq kilidi yenidən ölçülür: `M / C = 2 / 1`

Bu, v2-nin saytda ən çox iş tələb edən dəyişikliyidir. Sözmarkanın cap
hündürlüyü `5c = 13.5u`-dan `10c/3 = 9.0u`-ya düşdü. Nişan da, aralıq da
toxunulmadı, ona görə **bütün fərq mətn sütunundadır**.

İki fayldan ölçüldü. O vaxt v1 faylı `delivery/08-site/header-lockup.svg`,
v2 faylı `v2-ai/delivery/08-site/header-lockup.svg` idi; indi ikisi də
arxivdədir: `archive/v1-2026-08-24/delivery/08-site/header-lockup.svg` və
`archive/v2-2026-09-21/delivery/08-site/header-lockup.svg`. v3 faylı
`delivery/08-website/header-lockup.svg` v2 ilə **eyni həndəsədədir** - yalnız
rəng dəyişib (bölmə 11.2), ona görə aşağıdakı cədvəl v3 üçün də keçərlidir:

| Kəmiyyət | v1 | v2 | Nisbət |
|---|---|---|---|
| Sözmarka cap, `u` | `13.5` | `9.0` | `2/3` |
| Sözmarka mürəkkəb eni, `u` | `132.849052` | `88.566035` | `0.6666666692` |
| Nişan eni, `u` | `18.0` | `18.0` | dəyişməz |
| Aralıq, `u` | `4.05` | `4.05` | dəyişməz |
| Kilid mürəkkəb qutusu, `u` | `154.899052 x 18` | `110.616035 x 18` | - |
| `viewBox` (2c təmiz sahə daxil) | `160.299052 x 23.4` | `116.016035 x 23.4` | - |
| **Mürəkkəb AR** | **`8.6055`** | **`6.1453`** | - |

Sözmarka eninin nisbəti `2/3`-ə `2.5e-9` dəqiqliklə düşür. Bu təsadüf deyil:
mətn sütununun içindəki hər uzunluq cap-a mütənasibdir, ona görə cap `2/3`-ə
kiçiləndə sütunun hamısı onunla birlikdə kiçilir.

**Nə üçün `2/1`.** `GAP / cap` v1-də `0.30`, v2-də `0.45` olur. `0.45` təsadüfi
rəqəm deyil - sözmarkanın **öz söz boşluğu** qayda üzrə `0.45 x cap`-dır. `2/1`
nişanla söz arasındakı aralığın hələ də sözün öz boşluğuna bərabər olduğu son
pillədir.

### 5.1 Saytı tətbiq edən nə etməlidir

1. **Başlıqdakı en/hündürlük nisbətini dəyişin.** `8.6055`-i `6.1453` ilə əvəz
   edin. CSS-də hazır token var: `--k-lockup-ar: 6.1453`.
2. **Dəyişən ölçünü hündürlükdən verin**, endən yox. SVG-nin `viewBox`-u
   kilidi **artı hər tərəfdən `c = 2.7u` təmiz sahəni** əhatə edir, yəni
   `height`-i `23.4u` qəbul edir. Faylı kəsməyin: təmiz sahə qaydadır.
3. **Döşəməni aşmayın.** Ölçüldü:

| | v1 | v2 |
|---|---|---|
| Bağlayıcı döşəmə | **nişanın özü** (`16 px`) | **sözmarka cap-ı** (`10 px`) |
| Ən kiçik nişan | `16.0 px` | `20.0 px` |
| Ən kiçik cap | `12.0 px` | `10.0 px` |
| Ən kiçik mürəkkəb eni | `137.7 px` | `122.9 px` |

Nisbət dəyişəndə **bağlayıcı döşəmə də dəyişdi**. v1-də ilk yıxılan nişan idi;
v2-də nişan böyüdüyü üçün ilk yıxılan sözmarkadır. Nəticə tətbiq edən üçün
sadədir: başlıq kilidi `20 px`-dən alçaq nişanla (`123 px`-dən dar) çəkilmir.
Ondan aşağıda kilid yox, **yalnız nişan** işlənir (`icon.svg`).

---

## 6. Reduktiv kəsik - favicon-u tətbiq edən bunu bilməlidir

Bu, v1-də olmayan yeni qaydadır və **səssizcə pozula bilən** yeganə qaydadır.

Nişanın beş lövhəsindən ikisi (nöqtə və tir) örtük qatıdır. Kiçik ölçüdə onlar
qonşu lövhəyə yapışır və nişan oxunmaz olur. Ona görə nişan bir həddən aşağı
**reduktiv kəsiyi** göndərir: eyni həndəsə, amma iki örtük lövhəsi çəkilmir.

```
mark.cut_for(px)  ->  "reduced"   px <  24
                      "main"      px >= 24
```

Funksiya `tools/build/mark.py`-dədir (`REDUCED_FLOOR_PX = 24`). v2-də
`markv2.cut_for` idi; v3-də ikinci nüsxə silinib, generator birdir.

`px` burada **lövhə pikselidir**, yəni `24u` lövhənin rendər ölçüsü. Nişanın
mürəkkəbi lövhənin `0.75`-ni tutur, ona görə hədd mürəkkəb dili ilə
**`18 px`**-dir.

Hədd seçilməyib, **ölçülüb**: `tools/build/reduced.py` (v2-də
`v2-ai/build/reduced.py`) hər tam ölçüdə `16..64 px` arasında iki kəsiyi
rendər edir, `50` və `25` faiz alfa həddində
`4` və `8` qonşuluqda bağlı komponentləri sayır. `24 px` elə ilk ölçüdür ki,
ondan yuxarı **istisnasız** tam kəsik heç bir cütdə reduktiv kəsikdən az
komponent vermir. Cədvəlin hamısı `delivery/00-docs/06-REDUCED-CUT.md`-dədir,
xam ölçülər `qa/reduced-cut.json`-dadır.

### 6.1 Göndərilən favicon-da nə var

`08-website/favicon.ico` yeddi kadrdır. Kəsik əl ilə seçilmir, hər kadr
`cut_for`-dan keçir. Fayldan geri oxundu və hər kadr generatorun çıxışı ilə
**piksel-piksel** müqayisə edildi:

| Kadr | Kəsik | Lövhə | Yoxlama |
|---|---|---|---|
| `16 px` | **`reduced`** | 3 | `main` kəsiyindən fərqlidir |
| `24 px` | `main` | 5 | `reduced` kəsiyindən fərqlidir |
| `32 px` | `main` | 5 | eyni |
| `48 px` | `main` | 5 | eyni |
| `64 px` | `main` | 5 | eyni |
| `128 px` | `main` | 5 | eyni |
| `256 px` | `main` | 5 | eyni |

**v3 (2026-09-26):** kadrlar, kəsiklər və lövhə sayı dəyişməyib
(`qa/08-website-measurements.json`). Dəyişən rəngdir: v2-də kadrlar tək rəngli
`#0F1317` idi, v3-də `color` rəng yolundadır, fon əvvəlki kimi şəffafdır.
`16 px` kadrda nöqtə və tir düşür, amma gövdə (ink) ilə qol və ayaq (petrol)
qalır, ona görə ən kiçik kadr da iki rənglidir. Fayldan ölçüldü: `16 px`
kadrında tam qeyri-şəffaf pikseldən `24`-ü `#0F1317`, `7`-si `#0E6A82`-dir;
`256` pikseldən `167`-si şəffafdır. `tools/build/verify_site.py` hər kadrı yenə
generatorun `color` çıxışı ilə piksel-piksel tutuşdurur.

### 6.2 Qayda tətbiq edənə nə deyir

- **Favicon-u öz alətinizlə yenidən ölçüyə salmayın.** `16 px` kadrı `32 px`
  kadrının kiçildilməsi deyil - **başqa kəsikdir**. `.ico`-nu açıb tək kadr
  götürmək və ya PNG-dən yenidən generasiya etmək reduktiv qaydanı səssizcə
  ləğv edir.
- **`24 lövhə pikselindən` aşağı hər plitə, ikon və raster** `cut_for`-dan
  keçməlidir. Bura `16x16` favicon, kiçik avatar və `favicon` `sizes`
  siyahısındakı hər şey daxildir.
- **Başlıq kilidinə aid deyil.** Orada bağlayıcı hədd `floor_ok`-dur, bu qayda
  yox. Ölçüldü: kilidin öz döşəməsi nişanı `20 px`-də saxlayır, reduktiv hədd
  isə `18 px`-dir, yəni kilid `2 px` marja ilə həmişə tam kəsikdədir. Başlıq
  SVG-si tək fayldır və ona ikinci kəsik lazım deyil.
- **`apple-icon.png` və `icon.svg` həddin yuxarısındadır** (`1024` və `512`
  lövhə pikseli), ona görə tam kəsikdədir.

---

## 7. OpenGraph kartları - təsdiqlənmiş tünd istiqamət

v1 iki kart göndərirdi: kağız fonda mərkəzləşdirilmiş kilid və onun tünd
variantı. Sahib o vaxtdan banner istiqamətini seçdi - **dərin ink sahəsi, 45
dərəcəli tikişlə sağdan girən petrol paz, kənarla kəsilmiş böyüdülmüş
monoqram** - ona görə kartlar həmin kompozisiya ilə, bannerlərin öz mühərriki
(`tools/build/banner.py`; v2-də `v2-ai/build/banner.py`) ilə qurulur. Düz
kağız görünüşü qalmadı.

v3-də kompozisiya, tonlar və həndəsə eyni qalıb; dəyişən yalnız kilidin
rəngidir: `paper` əvəzinə `color-knockout` (bölmə 7.3, 11).

### 7.1 Cütün mənası dəyişdi

v1-də cüt **polyarlıq** cütü idi: biri açıq, biri tünd. Əsas istiqamət artıq
tünd olduğuna görə polyarlıq ox olmaqdan çıxdı. İndi cüt **dərinlik** cütüdür
və yalnız **bir açar** fərqlənir - sahə tonu:

| Fayl | Sahə | Rol |
|---|---|---|
| `opengraph-image.png` | `neutral-950` `#0F1317` | əsas kart, banner örtüyü ilə eyni |
| `opengraph-image-dark.png` | `neutral-1000` `#000000` | ehtiyat: fonu onsuz da qara olan yerlər |

Petrol tonlarının hamısı yerində qalır. `neutral-1000` yeni rəng deyil, neytral
onurğanın anker pilləsidir (`DESIGN-LANGUAGE.md` 2.5, "anchor, ehtiyat").
Ehtiyat kart X-in qaranlıq rejimi, iMessage və Slack dark üçündür: orada
`#0F1317` sahə səhifənin öz fonundan ayrılıb kartın kənarını göstərir.

### 7.2 Kompozisiya bannerdən necə köçürüldü

Banner `5.91:1`-dir, kart `1.90:1`. Üç rəqəm eyni qala bilmirdi və hər üçü
bannerin **ölçülmüş** dəyərindən yenidən hesablandı, gözlə miqyaslanmadı.

| Parametr | Banner | Kart | Səbəb |
|---|---|---|---|
| `mono_h` | `1.0` | `0.5` | Banner monoqramın mürəkkəb qutusunu kanvas hündürlüyünə bərabər edir. Bu, əslində **en** haqqında ifadədir: `2k` sağ kəsimdən sonra görünən açıqlıq `1512 px` örtükdə enin `15.7`, `1584 px` fonda `23.4` faizidir. `630 px` hündür kartda `1.0` həmin açıqlığı `48.6` faizə qaldırır - monoqram fakturadan mövzuya çevrilir. `0.5` bannerin **en payını** köçürür: `291.4 px` = `24.28` faiz. |
| `mono_top_u` | `2.0` | `-6.0` | Banner nişanı bir vahid aşağı salır ki, nöqtə üst kənarı keçsin və ayaqlar altdan çıxsın. `0.5`-də nişan heç bir kənara çatmır, qayda işsiz qalır. Əvəzində nişan kanvasa **mərkəzləşdirilir**: lövhənin üstündə altı, altında altı vahid kanvas. Ölçüldü: mərkəz `y = 315.0 px = H/2`. |
| `seam_x` | `1150` | `1200` | Tikiş `45` dərəcədir, ona görə kanvas boyu üfüqi gedişi **kanvas hündürlüyünə bərabərdir**. `630 px` kartda `1150` tikişi alt kənarda `x = 520`-yə, yəni düz kilidin üstünə gətirər. `seam_x = W` tikişi **sağ üst küncdən** daxil edir - bu kanvasda özünə ayrıca izah lazım olmayan yeganə dəyər. |

`mono_right_u` bannerin öz kəsimidir və **dəyişməyib**: sağdan `2k = 1.35u`,
yəni nöqtənin yanındakı hava qədər. Ölçüldü: `23.6 px`.

Kilidin iki rəqəmi bundan sonra seçilmir, çıxır: `lk_ink_h = 72 px` vahid
başına düz `4 px` verir, ona görə `lk_x = 96 px` düz `24u`-dur - kilidin
mürəkkəbi sol kənardan **tam bir lövhə** içəridə başlayır. Aksent xətti ondan
`GAP + k = 4.725u` əvvəl, `77.1 px`-də oturur.

### 7.3 Ölçülmüş rəqəmlər

Şəkillərin özündən oxundu (`tools/build/verify_site.py`; v2-də
`v2-ai/build/verify_site.py`), konfiqurasiyadan yox. v3 kontrast və addım
rəqəmləri `qa/08-website-measurements.json`-dadır. v3 ton payları bu sənəd
üçün `delivery/08-website/`-dəki iki PNG-dən eyni metodla (dəqiq rəng sayı,
kanvasın `0.2%`-i və yuxarısı) yenidən sayıldı.

| | `opengraph-image.png` | `opengraph-image-dark.png` |
|---|---|---|
| Ölçü | `1200 x 630` | `1200 x 630` |
| Sahə tonu və payı | `#0F1317`, `71.31%` | `#000000`, `71.41%` |
| Paz `petrol-800` | `17.05%` | `17.05%` |
| Monoqram `petrol-700` | `4.38%` | `4.38%` |
| Faska `petrol-600` | `3.20%` | `3.20%` |
| Kilid `paper` | `0.78%` (v2: `0.90%`) | `0.78%` (v2: `0.90%`) |
| Kilid `petrol-400` (v3) | `938` piksel, `0.12%` - inventar həddindən aşağı | `938` piksel, `0.12%` |
| Monoqram sahədə `petrol-900` | `0.82%` | `0.81%` |
| **Kilid kontrastı, ən zəif mürəkkəb** | **`6.70:1`** (`petrol-400`) | **`7.55:1`** (`petrol-400`) |
| Kilid `paper` kontrastı | `17.84:1` | `20.08:1` |
| Paz / sahə | `1.645` | `1.852` |
| Monoqram / sahə | `1.259` | `1.417` |
| Aksent xətti / sahə | `3.022` | `3.403` |
| Tikiş xətti / sahə | `4.470` | `5.033` |

**v3-də əvəz olunub (2026-09-26):** v2-də kilid yalnız `paper` idi və "Kilid
kontrastı" sətri `17.84:1` / `20.08:1` göstərirdi. v3 kilidi `color-knockout`-dur
(gövdə və sözmarka `#FAFAF7`, qalan dörd lövhə `#2FA8C7`); ən zəif mürəkkəbi
`petrol-400`-dür. Qapı `4.5:1`-dir, hər iki kart keçir. Kilid hər iki kartda
tam sahə üzərində oturur (`tones_behind`: pay `1.0`). Bax bölmə 11.3.

Ölçülən tonların hamısı palitradandır - yad ton yoxdur. Ölçülən **hue**-lar
`192` və `193`: petrol yeganə xromatik ailədir. v3-də də eynidir; `petrol-400`
`0.2%` həddindən aşağı olduğu üçün inventara düşmür, onun öz hue-su da `192`-dir
(hex-dən hesablandı).

Həndəsə: nişan mürəkkəbi `72 px`, kilid mürəkkəb eni `442.5 px` (enin
`36.9` faizi), monoqram mürəkkəbi `315 px`, kiliddən `370.2 px` aralı, tikiş
kilidin blokundan `310.5 px` aralı. Beş lövhədən dördü tam kadrdadır,
`leg` sağ kənarla kəsilir, kadrdan kənarda qalan lövhə yoxdur.

### 7.4 Deskriptor xətti kartda yazılmır

Faylın öz enində deskriptorun cap-ı `10.80 px`-dir və `9 px` döşəməsini keçir.
Amma OG kartı heç vaxt fayl enində göstərilmir:

| Yer | Xidmət olunan en | Deskriptor cap-ı | Nişan | Sözmarka cap-ı |
|---|---|---|---|---|
| X `summary_large_image` | `500 px` | **`4.50 px`** | `30.0 px` | `15.0 px` |
| Facebook lent | `470 px` | **`4.23 px`** | `28.2 px` | `14.1 px` |

Deskriptor hər iki yerdə `9 px` döşəməsinin yarısındadır, ona görə
`02-BRAND-GUIDELINES.md` 5.4-ə əsasən **yazılmır**. Nişan (`16 px` döşəmə)
və sözmarka cap-ı (`10 px` döşəmə) hər iki ölçüdə döşəməni keçir.

---

## 8. `kestridge-tokens.css` - mənbədən generasiya olunur

v1 faylı on iki hex dəyərini əl ilə saxlayırdı və **petrol pilləkənini heç
vermirdi**: saytı tətbiq edənin qaranlıq rejim linki üçün `petrol-400`-ü, işıqlı
rejim başlığı üçün `petrol-700`-ü yox idi.

v2 faylı `projects/brand-identity-2026/DESIGN-LANGUAGE.md` bölmə 2.5 cədvəlindən
**generasiya olunur** (`tools/build/sitev2.py`; v2-də `v2-ai/build/sitev2.py`).
Dəyər orada dəyişsə, fayl yenidən qurulur. Modulda palitranın ikinci nüsxəsi yoxdur, ona görə sürüşəcək
bir şey də yoxdur.

### 8.1 Sürüşmə yoxlaması

Tələb edilən yoxlama aparıldı: hər iki CSS faylı parse edildi, `var()`
istinadları həll edildi, hər hex dəyəri mənbə cədvəli ilə tutuşduruldu.

| | v1 faylı | v2 faylı |
|---|---|---|
| Token sayı | `12` | `56` |
| Mənbə cədvəli ilə tutuşdurulan pilləkən dəyəri | `12` | `38` |
| **Sürüşmə** | **yoxdur** | **yoxdur** |
| Cədvəldə olmayan yad hex | yoxdur | yoxdur |
| Həll olunmayan `var()` | yoxdur | yoxdur |

**v1 faylı düzgün idi.** Orada olan on iki dəyərin hamısı mənbə ilə üst-üstə
düşür. Problem yanlış dəyər deyil, **əskik dəyər** idi: pilləkənin `12/38`-i
verilirdi.

### 8.2 v2 faylında nə var

| Blok | Say | Nə |
|---|---|---|
| Neytral onurğa | `16` | `neutral-1000` -> `neutral-000`, bütöv |
| **Petrol** | **`10`** | `petrol-050` -> `petrol-900`, bütöv |
| Semantik | `12` | `success/warning/danger/info`, hər biri `600/500/400` |
| Semantik rol | `4` | `--k-success` və s., yalnız istinad |
| Rol | `14` | `--k-ink`, `--k-accent`, `--k-accent-link-dark` və s. |
| Kilid | `5` | AR, döşəmələr, reduktiv hədd |

Rollar pilləkənə `var()` ilə istinad edir, hex-i təkrarlamır. Bir faktın bir
yeri var: `--k-accent: var(--k-petrol-500)`.

Kilid tokenləri:

```css
--k-lockup-ar:        6.1453;   /* yatiq kilid, ink qutusu 110.616u x 18u */
--k-lockup-min-h:     20px;     /* ink hundurluyu doshemesi */
--k-lockup-min-w:     123px;    /* hemin doshemede ink eni */
--k-mark-min-px:      16px;     /* nishanin oz doshemesi */
--k-mark-reduced-px:  24px;     /* bundan ashagi REDUKTIV KESIK */
```

`.kestridge-descriptor` sinfi də əlavə olundu: Archivo `wght 500`, tracking
`180 pm em` = `0.18em`. v1-də deskriptor üçün CSS yox idi.

**v3:** fayl yenidən qurulub, amma dəyərləri dəyişməyib. v2 arxivindəki
faylla (`archive/v2-2026-09-21/delivery/08-site/kestridge-tokens.css`) fərq
yalnız üç şərh sətridir - generatorun, sənədin və `tools/build/lockupv2.py`-nin yolu
(ölçüldü, `diff`). Nişanın rəng tokenləri bölmə 11.4-dədir.

---

## 9. Hazır fayllar

Hamısı `delivery/08-website/`-dədir. "Nə" sütunu v3 vəziyyətini yazır; v2
dəyişiklikləri tarixçə kimi öz sütununda qalır.

| Fayl | Sayta hara düşür | Nə | v2-də nə dəyişdi | v3-də nə dəyişdi |
|---|---|---|---|---|
| `icon.svg` | `app/icon.svg` | tünd plitə `#171D23`, `color-knockout` nişan, radius `0` | 3 lövhə -> **5 lövhə** | knockout -> **`color-knockout`** |
| `icon-light.svg` | ehtiyat | açıq plitə `#FAFAF7`, `color` nişan | eyni | müsbət -> **`color`** |
| `apple-icon.png` | `app/apple-icon.png` | `1024 x 1024`, ink / plitə `0.75`, `#171D23` plitədə `color-knockout` | eyni | knockout -> **`color-knockout`** |
| `favicon.ico` | `public/favicon.ico` | 7 kadr: `16/24/32/48/64/128/256`, `color`, şəffaf fon | **`16 px` reduktiv kəsik** | müsbət -> **`color`**; `16 px` kadr da iki rəngli |
| `header-lockup.svg` | başlıq komponenti | yatıq kilid, `color` | **AR `8.6055` -> `6.1453`** | müsbət -> **`color`**, həndəsə eyni |
| `header-lockup-dark.svg` | başlıq, tünd fon | yatıq kilid, `color-knockout` | **eyni AR dəyişikliyi** | knockout -> **`color-knockout`**, həndəsə eyni |
| `opengraph-image.png` | `app/opengraph-image.png` | `1200 x 630`, kilid `color-knockout` | **kağız -> təsdiqlənmiş tünd istiqamət** | kilid `paper` -> **`color-knockout`**, `6.70:1` |
| `opengraph-image-dark.png` | ehtiyat | `1200 x 630`, qara sahə, kilid `color-knockout` | **polyarlıq cütü -> dərinlik cütü** | kilid `paper` -> **`color-knockout`**, `7.55:1` |
| `kestridge-tokens.css` | `styles/` | CSS dəyişənləri | **`12` -> `56` token, mənbədən generasiya** | dəyərlər eyni, yalnız 3 şərh sətri |

Ölçülər saytın **öz elan etdiyi** dəyərlərdir: `apple-touch-icon` üçün
`sizes="1024x1024"`, OG üçün doğrulanmış `1200 x 630`.

### 9.1 Şriftlər (`06-fonts/`)

Şriftlər nişanla birlikdə dəyişmədi. `delivery/06-fonts/` bayt-bayt köçürüldü
və hər faylın `sha256`-sı mənbə ilə tutuşduruldu - altısı da eyni. Yenidən
generasiya edilməyib, subset alınmayıb.

**v3:** qovluq adı `06-fonts/` olaraq qalır, fayllar dəyişməyib. v2-də mənbə
o vaxt v1-i saxlayan `delivery/06-fonts/` idi; v3-də mənbə
`archive/v1-2026-08-24/delivery/06-fonts/`-dir. Nəticə
`qa/08-website-measurements.json`, `fonts` blokundadır: altı faylın altısı
`identical: true`, bayt sayları və `sha256` aşağıdakı cədvəllə eynidir.

| Fayl | Bayt | `sha256` (ilk 16) |
|---|---|---|
| `Archivo-Variable.ttf` | `658596` | `0e094a7d3c7c4c25` |
| `Inter-Variable.ttf` | `879708` | `4989b125924991b9` |
| `JetBrainsMono-Variable.ttf` | `187208` | `48715a42ec242c21` |
| `OFL-Archivo.txt` | `4388` | `108b4e57c9c796d3` |
| `OFL-Inter.txt` | `4377` | `5b9321a4298cfeb6` |
| `OFL-JetBrainsMono.txt` | `4399` | `b2fe5e8987594e9f` |

Üç `OFL` faylı şriftlərlə **birlikdə** gedir. OFL lisenziyanın paylanan hər
nüsxə ilə səyahət etməsini tələb edir; lisenziyasız şrift yerləşdirmək lisenziya
pozuntusudur.

Sözmarka `Archivo wght 600`-dur, saytın mətn yığını onsuz da Archivo-dur.
`Inter` interfeys mətni, `JetBrains Mono` kod və kiçik etiketlər üçündür.

---

## 10. Toxunulmayan

- Səhifə tərtibatı, bölmə ardıcıllığı, mətn.
- `GeistMono` - sistem `JetBrains Mono` deyir, amma mono yalnız kiçik etiketlərdə
  (`ILLINOIS, UNITED STATES`, `01`, `Areas of work`) işlənir. Dəyişdirmək
  məcburi deyil; dəyişilsə sistemlə uyğunlaşar.
- Şirkət iddiaları ("Companies our team has worked with" siyahısı). Bu paket
  onları yoxlamayıb - `unverified`.
- Saytın 2026-08-24 ölçməsi. `testlogo-site.vercel.app` nə v2, nə v3 keçidində
  yenidən açılmayıb; v3 qərarında yalnız `kestridge.com`-un `--accent` dəyəri oxunub
  (bölmə 2-dəki v3 qeydi). 1-4 bölmələrindəki sayt dəyərləri 2026-08-24 ölçməsindəndir.

---

## 11. v3: iki rəngli nişan saytda (2026-09-26)

Nişanın həndəsəsi (beş lövhə), kilid nisbəti `M / C = 2 / 1` və reduktiv kəsik
v2 ilə eynidir. Dəyişən yalnız rəngdir: nişan iki rəngdə göndərilir. Rəng
qaydalarının özü `02-BRAND-GUIDELINES.md` bölmə 3-dədir; burada saytı tətbiq
edənə lazım olanlar var.

### 11.1 İki rəng yolu

Mənbə: `tools/build/mark.py`, `COLOUR` və `COLOUR_KNOCKOUT`.

| Hissə | `color` (açıq fon) | `color-knockout` (tünd fon) |
|---|---|---|
| gövdə (`stem`) | `#0F1317` ink | `#FAFAF7` paper |
| qol, nöqtə, ayaq, tir (`arm`, `tittle`, `leg`, `crossbar`) | `#0E6A82` petrol-600 | `#2FA8C7` petrol-400 |
| sözmarka | `#0F1317` | `#FAFAF7` |
| deskriptor | `#0E6A82` | `#2FA8C7` |

Bölgü hərf qrupu üzrədir, lövhə üzrə yox. Gövdə yalnız `K`-ya aiddir; qol həm
də `i`-nin gövdəsidir, nöqtə `i`-nin nöqtəsi, ayaq `A`-nın diaqonalı, tir
`A`-nın tiridir. Yəni ink `K`, petrol `Ai`. `petrol-600` `kestridge.com`-un öz aksentidir
(`--accent`, v3 qərarında ölçülüb). Bu sənəddə "sayt" `testlogo-site.vercel.app`-dır;
onun 2026-08-24 ölçməsində aksent `#0B7A67` idi (bölmə 2 və oradakı v3 qeydi).

Tətbiq edən üçün qaydalar:

- **Tünd fonda yalnız `color-knockout`.** Açıq fonda başqa petrol pilləsi yox,
  petroldan başqa ton yox - bölgü yenidən rənglənmir.
- **Düz iki rəng.** Beş lövhə beş rəng deyil; nöqtə və ya tir ayrıca rənglənmir,
  onlar `Ai` qrupunun bir hissəsi kimi petroldur.
- **Tək rəngli variantlar saytda yoxdur.** `positive`, `knockout`,
  `mono-black`, `mono-white` çap, oyma, faks üçündür:
  `01-logo-svg/mono/`.

### 11.2 Fayl-fayl

| Fayl | Rəng yolu | Fon | Fayldan ölçüldü |
|---|---|---|---|
| `header-lockup.svg` | `color` | şəffaf, açıq səhifə üçün | `6` yol (5 lövhə + sözmarka); fill: `#0F1317`, `#0E6A82` |
| `header-lockup-dark.svg` | `color-knockout` | şəffaf, tünd səhifə üçün | `6` yol; fill: `#FAFAF7`, `#2FA8C7` |
| `icon.svg` | `color-knockout` | `#171D23` plitə (`neutral-900`) | `5` lövhə; fill: `#FAFAF7`, `#2FA8C7` |
| `icon-light.svg` | `color` | `#FAFAF7` plitə | `5` lövhə; fill: `#0F1317`, `#0E6A82` |
| `apple-icon.png` | `color-knockout` | `#171D23` | `1024 x 1024`; piksel: `#171D23` `789174`, `#FAFAF7` `130560`, `#2FA8C7` `125187` |
| `favicon.ico` | `color` | şəffaf | 7 kadr; `16 px` reduktiv kadr da iki rəngli (bölmə 6.1) |
| `opengraph-image.png` | kilid `color-knockout` | `#0F1317` sahə | ən zəif mürəkkəb `6.70:1` |
| `opengraph-image-dark.png` | kilid `color-knockout` | `#000000` sahə | ən zəif mürəkkəb `7.55:1` |

Həndəsə yoxlaması: dörd SVG-nin `fill` atributları çıxarıldıqdan sonra mətni
`archive/v2-2026-09-21/delivery/08-site/`-dəki cütü ilə **eynidir** (ölçüldü).
Yəni v3 yalnız rəngi dəyişib; bölmə 5-dəki AR, `viewBox` və döşəmələr, bölmə
6-dakı kəsik qaydası olduğu kimi qüvvədədir. Başlığın ölçü tokenləri
(`--k-lockup-ar: 6.1453` və s.) dəyişmir.

### 11.3 Kontrast - ölçülmüş

WCAG 2 nisbi parlaqlıq. Qrafik döşəmə `3.0:1` (WCAG 1.4.11), mətn döşəməsi
`4.5:1`.

| Cüt | Nisbət | Saytda harada |
|---|---|---|
| `petrol-600` / paper `#FAFAF7` | `5.90:1` | başlıq açıq səhifədə, `icon-light.svg` |
| `petrol-600` / ağ `#FFFFFF` | `6.17:1` | başlıq ağ fonlu səhifədə |
| `petrol-400` / surface `#171D23` | `6.10:1` | `icon.svg`, `apple-icon.png`, tünd başlıq `--k-surface` fonunda |
| `petrol-400` / ink `#0F1317` | `6.70:1` | `opengraph-image.png` |
| `petrol-400` / qara `#000000` | `7.55:1` | `opengraph-image-dark.png` |
| paper / ink | `17.84:1` | gövdə və sözmarka, tünd fonda |
| `petrol-600` / ink (gövdə ilə qol - bölgünün özü) | `3.02:1` | `color` yolunun içində |

Son sətir vacibdir: iki mürəkkəb yalnız ton ilə yox, **parlaqlıq** ilə də
fərqlənir, ona görə bölgü boz çapda və rəng görmə pozuntularının əksəriyyətində
qalır. OpenGraph rəqəmləri `qa/08-website-measurements.json`-dan
(`min_contrast`), qalanları v3 qərarının ölçmələrindəndir.

### 11.4 Tokenlər - nişanın rəngləri CSS-də artıq var

`delivery/08-website/kestridge-tokens.css`-də yoxlanıldı:

| Rəng | Pillə tokeni | Ona `var()` ilə istinad edən rol |
|---|---|---|
| `#0E6A82` petrol-600 | `--k-petrol-600` | `--k-accent-link` (işıqlı rejimdə link) |
| `#2FA8C7` petrol-400 | `--k-petrol-400` | `--k-accent-link-dark` (qaranlıq rejimdə link) |
| `#0F1317` ink | `--k-neutral-950` | `--k-ink` |
| `#FAFAF7` paper | `--k-neutral-025` | `--k-paper` |
| `#171D23` surface (ikon plitəsi) | `--k-neutral-900` | `--k-surface` |

Nişan üçün ayrıca rol tokeni yoxdur və lazım da deyil: nişan SVG / PNG kimi
gəlir, rəngi faylın içindədir. Nişanın hissəsini CSS ilə çəkmək lazım olarsa,
pillə tokenlərini (`--k-petrol-600`, `--k-petrol-400`) birbaşa işlədin, link
rollarını yox - rol dəyişsə, nişan onunla birlikdə dəyişməməlidir.

**`--k-accent` nişanın rəngi deyil.** O, `--k-petrol-500` `#1187A5`-ə istinad
edir və faylın şərhi onu "yalniz nishan, ikon, serhed, fokus" adlandırır. Bu
şərh v2-dən qalıb (fayl `tools/build/sitev2.py`-dən generasiya olunur və bu
keçiddə şərhə toxunulmayıb). v3 nişanı `petrol-500` işlətmir; nişanı
`--k-accent` ilə rəngləmək bölgünü başqa petrol pilləsinə köçürür və v3
qaydası ilə qadağandır (bölmə 11.1). Faylın başlıq şərhi də hələ "v2" yazır -
dəyərlərə təsiri yoxdur.
