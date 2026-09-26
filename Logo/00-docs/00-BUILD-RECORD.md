# Kestridge AI - build qeydi (v2 və v3)

| | |
|---|---|
| Layihə | `kestridge-ai-brand` |
| Sənəd | `delivery/00-docs/00-BUILD-RECORD.md` (v2-də `v2-ai/delivery/00-BUILD-RECORD.md` idi) |
| Tarix | v2: 2026-09-21; v3: 2026-09-26 |
| Rol | icra və müstəqil ölçmə |
| Giriş | `01-DECISION-RECORD.md` (v2: D1 nişan, D2 kilid nisbəti; v3: D7 rəng bölgüsü, D8 platforma miqyası, D9 banner, D10 qovluqlanma), `02-BRAND-GUIDELINES.md` |
| Çıxış (v3) | `delivery/` - 272 indekslənmiş fayl + `README.md` + `MANIFEST.json`; `qa/` - 31 fayl; generator `tools/build/`, giriş nöqtəsi `tools/build/pack.py`; bu qeyd |
| Çıxış (v2, tarixçə) | o vaxt `v2-ai/delivery/` - 200 fayl, 5.40 MB, və `v2-ai/build/` - 11 modul. v2 paketi indi `archive/v2-2026-09-21/delivery/`-dadır (202 fayl: `MANIFEST.json` və onun indekslədiyi 201 fayl), modullar `tools/build/`-a köçüb, `markv2.py` istisna (bölmə 11.7, qüsur 4) |

Bu sənəd nə çəkildiyini yox, **nəyin ölçülüb təsdiqləndiyini** yazır. Hər rəqəm
ya generatorun özündən, ya da çəkilmiş faylın üzərindən götürülüb. Uyğun
gəlməyən və bağlanmayan yerlər bölmə 8, 10 və 11.9-dadır və gizlədilmir.

**Bölmə 0-10 v2 build-inin qeydidir (2026-09-21). v2 mətni və rəqəmləri yerində saxlanılır; v3 fərqləri tarixli "v3 qeydi" bəndləri, mətn içindəki "v3-də ..." qeydləri, "v3-də əvəz olunub" işarələri, bölmə 5.2-dəki "v3-də yeri" sütunu və bölmə 2, 5.1-dəki keçmiş zaman düzəlişləri ilə verilib.**
Orada adı çəkilən qovluqlar (`01-master/`, `02-raster/`, `03-icons/`, `04-pdf/`,
`05-collateral/`, `07-platform/`, `08-site/`, `09-banner/`) v2
paketinin o vaxtkı adlarıdır; o fayllar indi `archive/v2-2026-09-21/delivery/`
altındadır. `v2-ai/build/` modulları isə `tools/build/`-a köçüb (`markv2.py`
istisna, bölmə 11.7 qüsur 4); `v2-ai/` qovluğu silinib; onun PNG və JPG faylları (`.gitignore`: `*.png`, `*.jpg`) git-də heç vaxt olmayıb və bərpa olunmur, qalan faylları (mətn, kod, JSON, SVG, WebP, PDF) b6f3328-dən əvvəlki git
tarixindən bərpa olunur. v3-dəki yerləri bölmə 11.4-dəki xəritədədir. v3 build-i
(2026-09-26) bölmə 11-dədir.

v2 işi boyunca v1 paketi (o vaxt `delivery/` və `tools/`) **yalnız oxunub**.
Bir bayt dəyişdirilməyib, silinməyib, adı dəyişdirilməyib. (Bu, v2 vaxtına
aiddir. İndi v1 paketi `archive/v1-2026-08-24/delivery/`-dədir; v1
generatorlarından `mark`, `lockup`, `package`, `sitegen`, `browsercheck`
`archive/v1-2026-08-24/tools-build-*.py` kimi saxlanılır; qalan v1 mühərrik
modulları `tools/build/`-da işləyir və onlardan bəziləri sonradan dəyişib (məs.
`raster.py`, `vector.py`, `platform.py`; bölmə 1, v3 qeydi); birdəfəlik analiz
skriptləri silinib.)

---

## 0. Bir baxışda

| Nə | Nəticə |
|---|---|
| Nişan generatoru `markv2.py` öz-sınağı | **PASS** - namizədlə təpə-təpə, qapanış eynilikləri, aralıq pillələri |
| İnk qutusu v1 ilə eynidir | **təsdiqləndi** - `18 x 18`, `[3,21] x [3,21]`, piksel şəbəkəsi `16 / 32 / 48 / 128 / 512 px`-də tam |
| Kilid nisbəti `M/C = 2/1` | **çəkilib** - ideal `2.000000`, faktiki çəkilən `1.999929` (bölmə 4) |
| Deskriptorun `2/3` miqyası | **məcburi nəticədir**, yeni qərar deyil - `C_d = 4.05u -> 2.70u` |
| Reduktiv kəsik həddi | **`24 px`**, ölçülmüş; `23 px`-də son uğursuzluq, `16 px`-də ən pisi |
| Reduktiv qayda ilə çəkilən fayl | **3 raster** + iki `favicon.ico`-nun `16 px` kadrı (bölmə 5) |
| Reduktiv kəsik **müstəqil yoxlandı** | `02-raster/...-16.png` və hər iki `favicon.ico`-nun `16 px` kadrı təzə render ilə **bayt-bayt / kanal-kanal eynidir** (fərq `0`, əsas kəsiklə fərq `255`) |
| Plitə nisbətləri | kvadrat `0.75`, dairəvi `0.70` dəyişmir; **maskable `0.56` yenidir** (bölmə 6) |
| PWA maskable kəsməsi | **bağlandı** - safe zone xaricində `21.37%` -> **`0.00%`** |
| PDF-lər saf vektordur | **təsdiqləndi** - şəkil `0`, mətn operatoru `0`, nişan `5` alt yol / `25` təpə |
| v1-in beş çatışmayan qovluğu | **hamısı quruldu** - `03-icons`, `04-pdf`, `05-collateral`, `06-fonts`, `08-site` |
| v1-də olub v2-də olmayan fayl | **yoxdur** - fayl-fayl tutuşdurma bölmə 9 |
| Silinən | banner deskriptor sətri + `alt-texture` faylı (bölmə 7) |

---

## 1. Mühəndislik qaydası: v1-ə bir bayt toxunulmadı

Bütün v2 qurğusu **enjeksiya** ilə işləyir. `tools/build/` altındakı hər modul
`import mark` yazır; v2 sürücüləri `markv2`-ni `sys.modules["mark"]`-a qoyur,
ona görə `lockup.py`, `wordmark.py`, `raster.py`, `vector.py`, `platform.py`,
`collateral.py`, `templates.py` və `sitegen.py` beş lövhəli nişana **öz
mənbələri dəyişmədən** bağlanır.

Bir sabit dəyişir və o da **çağırış yerində** dəyişir, faylda yox:

```
CAP_H_v1 = 5 * C      = 13.5u      M / CAP_H = 18 / 13.5 = 4 / 3
CAP_H_v2 = 10 * C / 3 =  9.0u      M / CAP_H = 18 /  9.0 = 2 / 1
```

Bağlanma hər sürücüdə `assert` ilə yoxlanılır:

```python
assert LK.MARK is markv2, "lockup.py did not bind to markv2"
assert R.MARK  is markv2, "raster.py did not bind to markv2"
```

Bu assert-lər bəzək deyil. `platformv2.py` idxal olunanda `tools/build`-i
`sys.path`-dan **çıxarır** (v1-dəki `platform.py` standart kitabxananın
`platform` modulunu ad üzrə kölgələyir). Ona görə `manifest.py` `raster`-i
`platformv2`-dən **əvvəl** idxal edir; əks halda idxal səssizcə uğursuz olurdu
və parlaqlıq sütunu boş qalırdı. Bu, bu tapşırıqda tapılmış və düzəldilmiş
qüsurdur - bölmə 8.

**v3 qeydi (2026-09-26).** `markv2.py` artıq yoxdur. Enjeksiya naxışı qalır,
amma indi `tools/build/mark.py`-nin özünü bağlayır - bölmə 11.1.
**v3-də əvəz olunub (2026-09-26):** yuxarıdakı "öz mənbələri dəyişmədən" və
"faylda yox" artıq doğru deyil. `tools/build/lockup.py` sətir 33
`CAP_H = 10 * C / 3`-ü faylın özündə saxlayır (bölmə 11.9, maddə 1);
`lockup.py`, `raster.py` və `vector.py` rəng yolu üçün redaktə olunub (bölmə
11.1); `platform.py`-nin qoruyucusu düzəldilib (bölmə 11.7, qüsur 5);
`sitegen.py` `tools/build/`-da yoxdur, `archive/v1-2026-08-24/tools-build-sitegen.py`-dadır.

---

## 2. Nə quruldu, hansı mühərriklə

| Qovluq | Fayl | v2 sürücüsü | v1 mühərriki | Nə dəyişdi |
|---|---|---|---|---|
| `01-master/` | 23 | `masters.py` | `lockup.py`, `wordmark.py` | nişan 5 lövhə, `CAP_H = 9.0u` |
| `02-raster/` | 71 | `masters.py` | `raster.py` | nişan nərdivanı `cut_for()`-dan keçir |
| `03-icons/` | 8 | `icons_pdf.py` | `lockup.tile()`, `raster.py` | `favicon.ico` kadrları kəsik-kəsik seçilir |
| `04-pdf/` | 6 | `icons_pdf.py` | `vector.py` | yatıq kilidlər `2/1` ilə yenidən çəkilib |
| `05-collateral/` | 16 | `collateralv2.py` | `collateral.py`, `templates.py` | blok en/hündürlük nisbəti `2/1`-ə görə yenidən |
| `06-fonts/` | 6 | surət | - | dəyişmir - eyni korpus, eyni OFL |
| `07-platform/` | 44 | `platformv2.py` | `platform.py` | maskable `0.56`, plitələrdə `cut_for()` |
| `08-site/` | 9 | `sitev2.py` | `sitegen.py` | yeni nişan, yeni kilid, yeni tokenlər |
| `09-banner/` | 11 | `render_banner.py`, `banner.py`, `lockupv2.py` | - | `b4-oversize-monogram` yenidən qurulub |
| kök | 6 | `manifest.py` + sənədlər | `package.py` | `cuts_shipped` iki dəyər, `tiles` və `lockup` blokları yeni |

v2-də `v2-ai/build/` altındakı modulların heç biri həndəsə və ya tipoqrafika
yenidən yazmırdı. Hamısı v1 mühərrikini çağırırdı; fərq yalnız hansı nişanın
və hansı `CAP_H`-ın bağlandığında idi.

Cədvəldəki qovluq adları v2-nindir. v3-də bu modullar `tools/build/`-dadır,
qovluqlar yenidən adlandırılıb (bölmə 11.4) və qovluq sayları bölmə 11.4-dədir.

---

## 3. Ölçülmüş həndəsə - beş lövhə

`python tools/build/mark.py` həndəsəni parametrlərdən **yenidən törədir** və
on yoxlama dəstindən keçirir. Nəticə `PASS`.

| Kəmiyyət | Ölçülən | Mənbə |
|---|---|---|
| Lövhə sayı | `5` (`stem, arm, tittle, leg, crossbar`) | `markv2.PLATES` |
| Təpə sayı | `25` (reduktiv: `16`) | `markv2.plates()` |
| İnk qutusu | `[3, 3] .. [21, 21]`, `18 x 18` | `markv2.ink_box("main")` |
| İnk örtüyü, əsas kəsik | `0.245534` | `markv2.coverage("main")` |
| İnk örtüyü, reduktiv | `0.226934` | `markv2.coverage("reduced")` |
| Örtük lövhələrinin payı | `0.018601` | fərq |
| Struktur aralığı | `4k = 2.700000` | `stem\|arm`, `stem\|leg`, `arm\|leg` |
| Örtük aralığı | `2k = 1.350000` | `arm\|tittle`, `stem\|crossbar`, `leg\|crossbar` |
| Örtüyün tək ölçüsü | `3k = 2.025000` | nöqtənin tərəfi = tirin qalınlığı = tirin üzmə payı |
| Daxili bucaqlar | `{90, 135}`, ən kiçik `90` | `markv2.interior_angles()` |
| Qolun geri çəkilməsi | `k = 0.675`, üç xarici təpə `(-k, +k)` | `markv2.RETRACT` |

**İnk qutusu v1 ilə təpə-təpə eynidir**, ona görə `lockup.py`-nin `M = 18.0`
sabiti, qoruq sahə rəqəmləri və `16 / 32 / 48 / 128 / 512 px` piksel şəbəkəsi
landing-ləri **toxunulmamış qalır**. Bu, iddia deyil: öz-sınağının 3-cü dəsti
hər ölçüdə `x0, x1, y0, y1`-in tam ədəd olduğunu yoxlayır.

**`stem` və `leg` v1-in göndərilmiş nişanı ilə təpə-təpə eynidir**, `arm` isə
yalnız üç xarici təpəsi ilə `(-0.675, +0.675)` sürüşüb. Öz-sınağının 2-ci dəsti
bunu `tools/build/mark.py`-nin öz çıxışına qarşı ölçür.

**v3 qeydi (2026-09-26).** Cədvəldəki `markv2.*` adları v2 modulunundur; v3-də
eyni adlar `tools/build/mark.py`-dadır (`mark.PLATES`, `mark.plates()`,
`mark.ink_box()`, `mark.coverage()`, `mark.interior_angles()`, `mark.RETRACT`).
Yuxarıdakı cümlədəki `tools/build/mark.py` v2 vaxtı v1 generatoru idi. v3-də o
fayl v2 generatorudur və miras yoxlaması v1-i
`archive/v1-2026-08-24/tools-build-mark.py`-dən yükləyir; arxiv tapılmasa
yoxlama keçmir, `SystemExit` ilə dayanır. Bu, bölmə 11.7-dəki qüsur 3 və 4-ün
düzəlişidir.

---

## 4. Kilid: `M/C = 2/1`

### 4.1 Elan edilən və faktiki çəkilən

`01-master/MEASUREMENTS.json` hər kilid üçün iki sütun saxlayır: `ideal` -
qərarın öz rəqəmi, `drawn` - göndərilmiş SVG-nin yol datasından ölçülmüş rəqəm.
`MANIFEST.json` -> `lockup` bloku hər ikisini yan-yana daşıyır.

| Kilid | ideal `M/C` | çəkilən `M/C` | ideal AR | çəkilən AR |
|---|---|---|---|---|
| yatıq, `KESTRIDGE AI` | `2.000000` | `1.999929` | `6.145335` | `6.145511` |
| yatıq, `KESTRIDGE` | `2.000000` | `1.999929` | `5.233746` | `5.233890` |
| stacked, `KESTRIDGE AI` | `2.666667` | `2.662690` | `1.722632` | `1.722693` |

Fərqin səbəbi **yuvarlaqlaşdırmadır, qərar deyil**: SVG transformu altı onluq
rəqəmlə yazılır, `686 x 0.013120 = 9.000320u`. `1024 px` enli renderdə bu
`0.03 piksel`dir. Ona görə sənəd "dəqiq `2.0`" yazmır, iki rəqəmi də saxlayır.

**v3 yoxlaması (2026-09-26).** `01-master/MEASUREMENTS.json` v3-də
`qa/01-logo-svg-measurements.json`-dadır. Nə orada, nə
`archive/v2-2026-09-21/delivery/01-master/MEASUREMENTS.json`-da `drawn` açarı
var; `qa/stage-manifest.json` -> `lockup` -> `instances` altında hər kilidin
`ink_box_u`, `aspect_ratio`, `ratio_M_over_C`, `wordmark_cap_u` sahələrində
`drawn: null`-dır. Yuxarıdakı
`1.999929` kimi çəkilmiş rəqəmlər yalnız sənədlərdə qalır, maşın oxunan qeyddə
yoxdur. Açıq maddə 17, bölmə 11.9.

Stacked kiliddəki `2.662690` **v1-dən mirasdır, v2 gətirməyib**: `wdth 70`
instansiyasında `K` qlifi `687` vahiddir, miqyas isə `686`-dan hesablanır.
v1-in öz `kestridge-lockup-stacked-default-positive.svg` faylı eyni transformu
daşıyır.

### 4.2 Deskriptor - yeni qərar deyil, məcburi nəticə

`CAP_H` `2/3`-ə düşdüyü üçün sözmarkanın içindəki hər uzunluq eyni əmsalla
gedir. Deskriptor da:

| Kəmiyyət | v1 | v2 | Qapanış |
|---|---|---|---|
| deskriptor cap | `1.5c = 4.05u` | `c = 2.70u` | `DESC_CAP / CAP_H = 0.30` saxlanır |
| deskriptor enişi | `3.5c = 9.45u` | `7c/3 = 6.30u` | `DESC_DROP / CAP_H = 0.70` saxlanır |

Miqyaslanmasaydı deskriptor `88.57u` enli sözmarkanın altında `132.96u` enində
çıxardı - yəni sətir sözmarkadan `1.5` dəfə uzun olardı.

### 4.3 Kiçik ölçü döşəməsi - bağlayıcı qapı dəyişdi

`M/C = 2/1`-də nişan sözmarkadan iki dəfə böyükdür, ona görə **bağlayan qapı
artıq nişan deyil, sözmarkanın cap döşəməsidir**.

| | v1 (`4/3`) | v2 (`2/1`) |
|---|---|---|
| ən kiçik icazəli kilid ink eni | `137.69 px` | **`122.91 px`** |
| orada nişan | `16.0 px` | **`20.0 px`** |
| orada cap | `12.0 px` | `10.0 px` |
| render hündürlüyü döşəməsi | `20.8 px` | **`26 px`** |

Ayaq izi `11` faiz **kiçilir** və həmin nöqtədə nişan `16` yox, `20 px` olur.
Rəqəmlər `08-site/kestridge-tokens.css`-ə də düşür (`min-h 20`, `min-w 123`),
yəni sayt tərəfi eyni döşəməni oxuyur. v3-də bu fayl
`delivery/08-website/kestridge-tokens.css`-dir və iki token orada qalır
(`--k-lockup-min-h: 20px`, `--k-lockup-min-w: 123px`).

**Bir istisna ölçüldü və düzəldildi.** `lockup.py`-nin `floor_ok()` funksiyası
hər yatıq kilid üçün blok hündürlüyünü `2c + max(M, CAP_H) = 23.4u` götürür və
deskriptorun `9 px` cap qapısını **heç yoxlamır**. Deskriptor variantının
kanvası isə `25.24723u`-dur. Düz hesablandıqda onun döşəməsi `26 px` yox,
**`85 px`**-dir və `64 px`-də keçmir. `01-master/MEASUREMENTS.json` bu sətri
düzəldilmiş saxlayır (`min_floor_height_px: 85`, `floor_ok.64: false`) və
manifest onu həmin faylın sətrinə daşıyır. `tools/` oxunur-yazılmır, ona görə
funksiyanın özü dəyişmir - düzəliş çağırış tərəfindədir (bölmə 10).

**v3 qeydi (2026-09-26).** Düzəliş əl ilə deyil, göndərilmiş v2-də də deyildi:
v2 commit-i `66f7460`-dən bəri `masters.py` (o vaxt `v2-ai/build/`, indi `tools/build/`)
deskriptor kilidinin döşəməsini öz `descriptor_floor_ok()` funksiyası ilə
hesablayır; bölmə 8 və 10-dakı "əl ilə" ondan əvvəlki `95c1e1d` vəziyyətidir. `qa/01-logo-svg-measurements.json`-da deskriptor kilidi
`min_floor_height_px: 85` və `floor_ok` -> `"64": false` daşıyır, və fayl hər
build-də yenidən yaranır. `lockup.floor_ok()`-un özü yenə deskriptoru bilmir;
düzəliş yenə çağırış tərəfindədir.

---

## 5. Reduktiv kəsik - nə yenidən quruldu

### 5.1 Həd seçilmədi, ölçüldü

`reduced.py` (v2-də `v2-ai/build/reduced.py`, v3-də `tools/build/reduced.py`)
`16..64 px` arasında hər tam ölçüdə hər iki konsepti
render edir və `50%` / `25%` hədlərində, `4-` və `8-qonşuluqda` əlaqəli
komponentləri sayır. `REDUCED_FLOOR_PX` - yuxarı doğru **bir istisna olmadan**
tam kəsiyin ən azı reduktiv qədər komponent verdiyi və `50%` həddində beş
lövhəni ayrı-ayrı həll etdiyi ən kiçik ölçüdür.

| Ölçü | Tam kəsik `50/8` | Tam kəsik `25/4` | Nəticə |
|---|---|---|---|
| `16 px` | `5` | **`2`** | ən pis hal - bütöv nişan çılpaq `K`-nın üçündən az komponentə axır |
| `23 px` | **`4`** | `5` | son uğursuzluq - ayaq və tir künc-künc toxunur |
| **`24 px`** | `5` | `5` | **həd** |

`24 px` həm də bələdçinin lövhə aralığı sübutu ilə onsuz da "təmiz döşəmə"
dediyi rəqəmdir - iki ölçmə bir-birinə uyğunlaşdırılmadan üst-üstə düşür.
Tam cədvəl v2-də `REDUCED-CUT.md`-də, xam data `REDUCED-CUT.json` və
`RASTER-MERGE-GROUND-TRUTH.txt`-də idi. v3-də: cədvəl bu qovluqdakı
`06-REDUCED-CUT.md` (generasiya olunur), xam data `qa/reduced-cut.json` və
`qa/raster-merge-ground-truth.txt`. v3 build-inin `qa/reduced-cut.json`-u
eyni həddi və eyni sətirləri verir: `threshold_px: 24`; `16 px`-də tam kəsik
`50/8` = `5`, `25/4` = `2`; `23 px`-də `50/8` = `4` (2026-09-26 tutuşdurulub).

### 5.2 Hansı fayl reduktiv kəsiklə getdi

Qayda **hər** `24 px`-dən aşağı lövhə ölçüsünə tətbiq olunur, ona görə tarama
bütün paketi gəzdi:

| Fayl (v2 adı) | Lövhə `px` | Kəsik | v3-də yeri |
|---|---|---|---|
| `02-raster/kestridge-mark-main-16.png` | `16` | reduktiv | `delivery/02-logo-png/mono/kestridge-mark-positive-16.png` |
| `07-platform/veb-standartlari-favicon-klassik-olchu-16x16.png` | `16` | reduktiv | `delivery/07-platforms/veb-standartlari-favicon-klassik-olchu-16x16.png` |
| `07-platform/veb-standartlari-favicon-chox-olchulu-deste-html-standard-numunesi-16x16.png` | `16` | reduktiv | `delivery/07-platforms/veb-standartlari-favicon-chox-olchulu-deste-html-standard-numunesi-16x16.png` |
| `03-icons/favicon.ico` -> `16 px` kadrı | `16` | reduktiv | `delivery/03-app-icons/favicon.ico` |
| `08-site/favicon.ico` -> `16 px` kadrı | `16` | reduktiv | `delivery/08-website/favicon.ico` |
| - (v3-də yeni) | `16` | reduktiv | `delivery/02-logo-png/color/kestridge-mark-color-16.png` |
| - (v3-də yeni) | `16` | reduktiv | `delivery/02-logo-png/color/kestridge-mark-color-knockout-16.png` |

v3-də platforma plitələri və favicon rənglidir; `reduced.py` döşəmədən aşağı
platforma plitələrini platformanın rəng yolu ilə çəkir və `platformv2`-nin
faylını bayt-bayt təkrarlamasa dayanır (bölmə 11.7, qüsur 6). Son iki sətir
(rəngli `16 px` nişanlar) `qa/reduced-cut.json`-da yoxdur - açıq maddə 20, 11.9.

Qalan **68 aktiv** taranıb və əsas kəsikdə qalıb: `02-raster` `30` + `1`,
`07-platform` `33` + `2`, `09-banner` `2`.

**Bu sətirlər müstəqil yoxlandı, qeydə inanılmadı.** `raster.mark_png()` təzədən
işlədilib və nəticə diskdəki fayl ilə tutuşdurulub:

| Yoxlama | Nəticə |
|---|---|
| `02-raster/kestridge-mark-main-16.png` sha256 | reduktiv render ilə **eyni**, əsas render ilə fərqli |
| `02-raster/kestridge-mark-main-24.png` sha256 | əsas render ilə **eyni** |
| `03-icons/favicon.ico` `16 px` kadrı | reduktiv render ilə kanal fərqi **`0`**; əsas kəsiklə fərq `255` |

Yəni həd faylda işləyir, təkcə sənəddə yox.

### 5.3 Reduktiv kəsik v1-in nişanı deyil

Asan səhv budur: "reduktiv kəsik = köhnə üç lövhəli nişan". **Belə deyil.**
Reduktiv kəsik `f2`-nin geri çəkilmiş qol ucunu **saxlayır**, ona görə siluet
həddin hər iki tərəfində kəsilməz qalır. Ölçülmüş fərq:

| | ink örtüyü |
|---|---|
| v1 nişanı (v2 vaxtı `tools/build/mark.py`; v3-də `archive/v1-2026-08-24/tools-build-mark.py`) | `0.232793` |
| v2 reduktiv kəsik | `0.226934` |

`0.005859` fərq məhz geri çəkilmiş qol ucudur.

### 5.4 Parlaqlıq

`REDUCED-CUT.json` -> `luminance` (`proof.py draw_plates`, `fit 0.75`, `SS 8`,
LANCZOS; v3-də `qa/reduced-cut.json`, altı rəqəm eynidir, 2026-09-26
tutuşdurulub):

| Ölçü | əsas | reduktiv | v1 | göndərilən |
|---|---|---|---|---|
| `16 px` | `0.7399` | `0.7597` | `0.7535` | reduktiv |
| `128 px` | `0.7547` | `0.7720` | `0.7666` | əsas |

Yəni `16 px`-də göndərilən fayl v1-dən `0.006` açıqdır, tam kəsikdən isə
`0.020` açıq - örtük getdiyi üçün. Rəqəm gizlədilmir.

---

## 6. Plitə nisbətləri - `maskable 0.56` yenidir

Nisbətlər fərz edilmir, **törədilir**. `platformv2.py` nişanın öz lövhələrini
gəzib ink qutusunun mərkəzindən ən uzaq təpəni tapır:

| Kəmiyyət | Ölçülən |
|---|---|
| ən pis ink radiusu | `12.727922u` |
| ink qutusunun kənarı | `18.0u` |
| `r_max` / ink kənarı | `0.70710678` = `sqrt(2)/2` |

Bu rəqəm plitə ölçüsündən asılı deyil, ona görə hər kəsim radiusu üçün bir
tavan verir:

| Kəsim | Radius payı | Tavan | Göndərilən | Tavanın payı |
|---|---|---|---|---|
| dairəvi (daxili dairə) | `0.50` | `0.707107` | `0.70` | `0.98995` |
| PWA maskable (safe zone) | `0.40` | `0.565685` | **`0.56`** | `0.98995` |

`0.56 = 0.70 x 0.80` - dairəvi nisbət zəmanətli diametrə vurulur və maskable
ikon **avatarların öz ehtiyat payını miras alır**, yeni bir pay mübahisə
edilmir. Kvadrat nisbət `0.75` yerində qalır: onu heç kim kəsmir.

**Düzəldilən qüsur.** `tools/build/platform.py`-nin `CIRCULAR` regex-i
`maskable` sətrini tutmur, ona görə maskable ikon **kvadrat nisbəti ilə**
çəkilirdi:

| | əvvəl (`0.75`) | indi (`0.56`) |
|---|---|---|
| ink yarımdiaqonalı | `271.53 px` | `202.59 px` |
| safe radiusa nisbət (`204.8 px`) | `1.3258` | `0.9892` |
| safe zone xaricindəki ink | `13774.06 px2` = **`21.37%`** | **`0.00 px2` = `0.00%`** |
| safe zone xaricinə toxunan piksel | - | **`0`** |
| ehtiyat | - | `2.21 px` (mərkəzdən `1.86`, küncdən `1.15`) |

Düzəliş `tools/`-a toxunmadan edilib: `lockup.TILE_SQUARE` **bir çağırışın
müddətinə** `0.56`-ya bağlanır və eyni kompozisiya yolu yenidən işlədilir.
İkinci render yoxdur.

**v1 eyni qüsurdadır** - onun göndərilmiş maskable faylında ink-in `21.02`
faizi safe zone xaricindədir. `delivery/` bu paketdə yalnız oxunur, ona görə
düzəliş orada edilmir; açıq maddə kimi qalır (bölmə 10). (v2 vaxtı `delivery/`
v1 paketi idi; o paket indi `archive/v1-2026-08-24/delivery/`-də dondurulub.
v3 statusu: bölmə 11.9, maddə 4.)

---

## 7. Nə silindi və niyə

İkisi də **ölçmə ilə** silindi, zövqlə yox. Rəqəmlər
`09-banner/measurements.json`-da qalır: sətir getdi, sətri aparan ədədlər qaldı.
(v3-də bu fayl `qa/09-linkedin-banner/measurements.json`-dır.)

### 7.1 Banner deskriptor sətri - yazılmır

Əvvəlki paket hər üç banneri `descriptor_ok: false` ilə göndərmişdi. Səbəb
qapının **yanlış yerdə** yoxlanması idi: döşəmə fayl ölçüsündə ölçülürdü,
halbuki banner heç vaxt fayl ölçüsündə görünmür.

`C_d = 2.7u` olduğuna görə `9 px` cap döşəməsi kilidin ink eni haqqında
ifadədir: `9 / 2.7 = 3.3333 px/u`, yəni **`368.7 px`**.

| | faylda | telefon `390 px` | `1/3` miqyas | döşəmə |
|---|---|---|---|---|
| örtük, kilid ink eni | `516.2 px` | **`245.8 px`** | **`172.1 px`** | `368.7 px` |
| profil, kilid ink eni | `614.5 px` | **`216.2 px`** | **`204.8 px`** | `368.7 px` |

Dörd haldan dördü aşağıdır, ona görə qayda sətrin yazılmamasını tələb edir.

Sətri döşəməyə qaldırmaq yolu (`a`) **ölçmə ilə** rədd edildi:

| | ən yumşaq hal | ən pis hal |
|---|---|---|
| lazım olan böyümə | `x1.500` | `x2.143` |
| tikişi keçir | `159.6 px` | `425.4 px` |
| deskriptor cap / sözmarka cap | `0.450` | `0.643` (göndərilmiş nisbət `0.300`) |

`1/3` miqyasda döşəməni keçmək üçün kilidin ink eni faylda `1106.2 px`
olmalıdır, örtüyün ən dar kəsim pəncərəsi isə `819.2 px`-dir - **sadəcə
sığmır**.

Sətir blokun bir hissəsi idi, ona görə getməsi bloku qısaltdı və şaquli yer
yenidən ölçüldü: örtükdə blok `92.6 -> 84.0 px`, profildə `110.3 -> 100.0 px`;
profildə `block_cy` `168 -> 198 = H/2`, hava `118 / 178 -> 148 / 148`.

**Nə itdi:** banner artıq `AUTOMATION SECURITY ANALYTICS` yazmır və
`petrol-400 #2FA8C7` bannerdə heç yerdə qalmır. Kompensasiya üçün başqa
elementin rəngi **dəyişdirilmədi**.

**v3-də əvəz olunub (2026-09-26):** "`petrol-400` bannerdə heç yerdə qalmır"
cümləsi artıq doğru deyil. v3-də banner kilidi `color-knockout`-dur, `petrol-400`
kilidin Ai lövhələrindədir. Kilidin ən zəif inki ink sahəsi üzərində
`petrol-400`-dür: `6.70:1` (v2-də yalnız paper ilə `17.84:1` idi), `4.5:1`
qapısı keçir. Deskriptor sətri v3-də də yazılmır. Bax bölmə 11.1 və
`05-BANNER-NOTES.md`.

Eyni qayda sayt tərəfində də işləyir və eyni nəticəni verir:
`08-site/opengraph-image.png`-də (v3-də `delivery/08-website/opengraph-image.png`;
v3-də də yazılmır, `qa/08-website-measurements.json` -> `descriptor.set: false`)
deskriptor **yazılmır**, çünki faylda cap
`10.8 px` olsa da, xidmət olunan ölçüdə `4.50 px` (X `summary-large-image`) və
`4.23 px` (Facebook feed) çıxır - hər ikisi `9 px` döşəməsindən aşağı.

### 7.2 `alt-texture` faylı - silindi

`kestridge-banner-linkedin-cover-1512x256-alt-texture.png`, eyni örtük,
monoqram `2.0x` kadrlaması ilə. Ölçmə onu belə yazmışdı:

| Lövhə | `visible_area_share` |
|---|---|
| `arm` | **`0.000`** |
| `tittle` | **`0.000`** |
| `stem` | `0.500` |
| `leg` | `0.857` |
| `crossbar` | `1.000` |

Arifmetika bağlıdır: `2.0x`-də kadrda `18 / 2.0 = 9.0u` var, iki örtük
lövhəsinin arası isə `18.975 - 3.000 = 15.975u`-dur. **Heç bir kadrlama
ikisini birdən tuta bilmir**; tutan hər kadrlama `1.127x` və aşağısındadır,
orada isə render əsas örtüyün özüdür. Yəni "yenidən kadrla" yolu fayla heç nə
qazandırmır.

Üstəlik fayl `_` prefiksi olmadan, iki əsl göndəriş faylının yanında dayanırdı -
üçüncü göndəriş kimi oxunurdu. Fayl və onun üç sübutu silindi.

Eyni dərs `08-site/opengraph-image.png`-də **qabaqcadan** tətbiq olunub:
monoqram orada kətan hündürlüyünün `0.5`-i qədərdir, beş lövhədən dördü tam
kadrdadır və yalnız `leg` sağ kənarla kəsilir - yəni `2.0x` səhvi təkrarlanmır.

---

## 8. Tapılan qüsurlar

| Qüsur | Nə idi | Düzəliş | Harada |
|---|---|---|---|
| **PWA maskable ikon kəsirdi** | `platform.py`-nin `CIRCULAR` regex-i `maskable` sətrini tutmur, ikon kvadrat nisbəti ilə çəkilirdi; ink-in `21.37` faizi safe zone xaricində | `TILE_MASKABLE = 0.56`, çağırış müddətinə bağlanır | `platformv2.py`, bölmə 6 |
| **Deskriptor banneri qüsurlu göndərmişdi** | döşəmə fayl ölçüsündə yoxlanırdı, göndərilən ölçüdə yox | `descriptor_gate()`, sətir yazılmır | `banner.py`, bölmə 7.1 |
| **`16 px`-də örtük silueti bağlayırdı** | tir gövdəni ayağa bağlayır, `25%` həddində bütöv nişan `2` komponentə axır - çılpaq `K`-nın `3`-ündən az | `cut_for()`, `REDUCED_FLOOR_PX = 24` | `markv2.py`, bölmə 5 |
| **`floor_ok()` deskriptoru yoxlamır** | blok hündürlüyünü `23.4u` götürür, deskriptorun `9 px` qapısını heç yoxlamır; nəticə `26 px` çıxırdı, düzü `85 px`-dir | `MEASUREMENTS.json`-da əl ilə düzəldilib, generator açıqdır (v2 commit-i `66f7460`-də artıq əvəz olunmuşdu, göndərilmiş v2 nüsxəsi daxil: `masters.descriptor_floor_ok()` hesablayır, bölmə 4.3 v3 qeydi və 11.9 maddə 3) | bölmə 4.3, 10 |
| **`raster` idxalı səssizcə uğursuz olurdu** | `platformv2` idxal olunanda `tools/build`-i `sys.path`-dan çıxarır; sonrakı `import raster` `except`-ə düşür və parlaqlıq sütunu boş qalırdı | `raster` `platformv2`-dən əvvəl idxal edilir, `assert` ilə | `manifest.py`, bölmə 1 |
| **`alt-texture` üçüncü göndəriş kimi oxunurdu** | `_` prefiksi yox idi, iki əsl faylın yanında dayanırdı | silindi | bölmə 7.2 |

Miras qalan və **düzəldilməyən** bir rəqəm: stacked kiliddə çəkilən
`M/C = 2.662690` (ideal `2.666667`). Səbəb `wdth 70` instansiyasında `K`
qlifinin `687` vahid olması, miqyasın isə `686`-dan hesablanmasıdır. v1-in öz
faylı eyni transformu daşıyır, ona görə bu v2-nin gətirdiyi qüsur deyil və
burada düzəldilmir - düzəlişi sözmarka mühərrikinə toxunmağı tələb edir.

v3 build-ində tapılan səkkiz qüsur bölmə 11.7-dədir.

---

## 9. Paket: fayl-fayl tutuşdurma və manifest

### 9.1 v1-də olub v2-də olmayan fayl yoxdur

Hər qovluq ad-ad tutuşduruldu:

| Qovluq | v1 | v2 | v1-də olub v2-də olmayan |
|---|---|---|---|
| `01-master` | 22 | 23 | yoxdur (`+ MEASUREMENTS.json`) |
| `02-raster` | 71 | 71 | yoxdur |
| `03-icons` | 7 | 8 | yoxdur (`+ MEASUREMENTS.json`) |
| `04-pdf` | 5 | 6 | yoxdur (`+ MEASUREMENTS.json`) |
| `05-collateral` | 16 | 16 | yoxdur |
| `06-fonts` | 6 | 6 | yoxdur |
| `07-platform` | 32 | 37 | yoxdur (`+ NOTES.md`, `_check/`, favicon dəsti `16/32/48`) |
| `08-site` | 9 | 9 | yoxdur |

`09-banner` v1-də yoxdur - tamamilə yeni qovluqdur.

### 9.2 Ölçülmüş: beş yeni qovluq

**`03-icons`.** `favicon.ico` yeddi kadr daşıyır (`16, 24, 32, 48, 64, 128,
256`); hər kadr öz ölçüsündə çəkilib və fayldan geri deşifrə edilib
(`max_channel_diff: 0`, o biri kəsiklə fərq `255`). Altı plitə SVG-si: kvadrat
`0.75` üç ölçüdə, dairəvi `0.70` üç ölçüdə; hər birində ən pis ink radiusu
kvadratda `12.727922u`, dairədə `11.879395u`, daxili dairə `12.0u` - yəni
dairəvi plitə **kəsmir** (`clips: false`).

**`04-pdf`.** Beş fayl, hamısı `pypdf` ilə geri oxunub:

| Fayl | `w x h` pt | v1 | nişan alt yolu / təpə | şəkil | mətn op. |
|---|---|---|---|---|---|
| `kestridge-mark-positive.pdf` | `144.00 x 144.00` | eyni | `5 / 25` | `0` | `0` |
| `...-horizontal-default.pdf` | `356.97 x 72.00` | `493.23` | `5 / 25` | `0` | `0` |
| `...-horizontal-reserve.pdf` | `306.48 x 72.00` | `417.50` | `5 / 25` | `0` | `0` |
| `...-stacked-default.pdf` | `115.41 x 72.00` | `115.41` | `5 / 25` | `0` | `0` |
| `...-stacked-reserve.pdf` | `94.77 x 72.00` | `94.77` | `5 / 25` | `0` | `0` |

Yatıq kilidlər `27.6` faiz daraldı (`356.97 / 493.23 = 0.7237`), stacked
kilidlər isə **eninə qədər dəyişmədi** - çünki `CAP_S` toxunulmayıb. Bu, `2/1`
qərarının yalnız yatıq kilidə aid olmasının birbaşa sübutudur.

**`05-collateral`.** On altı fayl: səkkiz artefakt, səkkiz bələdçi qatı.
Kanvas ölçüləri `US-COLLATERAL-SPEC.md` (v3-də `tools/data/US-COLLATERAL-SPEC.md`)
bölmə 10 cədvəlinin öz düsturu ilə
(`canvas_px = ceil((dim + 2 * bleed) * dpi)`) yenidən hesablanıb və artefakt
fayllarının hamısı uyğundur (`matches_spec_canvas: true`). Bələdçi qatları
qəsdən daha hündürdür (`letterhead-p1` `2550 x 3300`, bələdçisi `2550 x 3392`) -
legend zolağına görə; v1 də eyni iki ölçünü göndərir.

E-poçt imzası döşəmə baxımından ayrıca yoxlandı: `email-sig-logo-1x.png`-də
nişanın ink hündürlüyü `26 px`, yəni lövhə `34.67 px` - `24 px` həddindən
yuxarı, ona görə **əsas kəsik** gedir və nöqtə ilə tir imzada görünür.

**`06-fonts`.** Üç dəyişkən TTF və üç OFL mətni, mənbədən bayt-bayt surət
(`identical: true`). Versiyalar fayl `name` cədvəlindən oxunub: Archivo
`2.001`, Inter `4.001`, JetBrains Mono `2.211`.

**`08-site`.** Doqquz fayl. `icon.svg` və `icon-light.svg` beş yol daşıyır
(`viewBox 0 0 24 24`, `frac 0.75`, künc radiusu `0`); `header-lockup.svg` və
`header-lockup-dark.svg` `01-master`-in yatıq kilidləri ilə **bayt-bayt
eynidir** (manifest bunu `identical_to` ilə yazır), `favicon.ico`-nun `16 px`
kadrı reduktivdir. OG şəkli `1200 x 630`: nişan `72 px`, sözmarka cap `36 px`
(`M/C = 2/1`), kilidin fonla kontrastı **`17.84 : 1`** (tünd variantda
`20.08 : 1`), monoqram kətan hündürlüyünün yarısı, `leg` istisna hər lövhə tam
kadrda. `kestridge-tokens.css` `16` neytral, `10` petrol, `12` semantik tonu və
kilid tokenlərini daşıyır (`ar 6.1453`, `min-h 20`, `min-w 123`,
`reduced-floor 24`).

**v3 qeydi (2026-09-26).** v3-də sayt faylları `delivery/08-website/`-dadır və
rənglidir: başlıq kilidi açıq fonda `color`, tünd fonda `color-knockout`; OG
şəkilləri `color-knockout`. Ona görə yuxarıdakı `17.84 : 1` v2-nin rəqəmidir.
v3-də kilidin ən zəif inki `petrol-400`-dür: `qa/08-website-measurements.json`
-> `opengraph-image.png` `min_contrast: 6.7`, `opengraph-image-dark.png`
`min_contrast: 7.55`.

### 9.3 Manifest

`MANIFEST.json` `v2-ai/build/manifest.py` ilə yazılır və **bu sənəddən sonra**
qurulur, ona görə bu faylın `sha256`-sı da indeksdədir.

**v3-də əvəz olunub (2026-09-26):** v3-də iki manifest var. `delivery/MANIFEST.json`-u
`tools/build/pack.py` düzülmüş paketdən yazır (hər faylın `bytes`, `sha256`,
şəkillər üçün `px`; üstəlik `colourways`, `folders`, `file_count`). Aşağıdakı
dörd bloklu manifest isə `tools/build/manifest.py`-nin stage-ə yazdığı
manifestdir və `qa/stage-manifest.json`-a düşür. `00-docs/`-dakı sənədlərin
`sha256`-sı `delivery/MANIFEST.json`-dadır, ona görə sənəd redaktəsindən sonra
manifest yenidən yazılmalıdır (bölmə 11.10). Bax bölmə 11.4.

v1-dən fərqli olaraq manifest dörd blok daşıyır:

| Blok | Nə var |
|---|---|
| `decisions` | palitra (petrol pillələri daxil), sətirlər, deskriptor, döşəmələr, **`cuts_shipped: ["main", "reduced"]`** |
| `geometry` | beş lövhənin təpələri, iki aralıq pilləsi, örtük modulu, hər iki kəsiyin ink qutusu və örtüyü, `reduced_floor_px` |
| `tiles` | `0.75 / 0.70 / 0.56` və hər üçünün törədilməsi (ölçülmüş yarımdiaqonal, tavan, tavanın payı) |
| `lockup` | `M / CAP_H = 2/1`, hər kilid üçün `ideal` və `drawn` cütü, `floor_ok` sətirləri |

`cuts_shipped` v1-də tək elementli siyahı idi (`["main"]`). v2-də iki elementdir,
çünki `reduced` **göndərilir**. `dense` kəsik v1-dəki kimi geri götürülmüş
qalır: `cut_for()` onu heç vaxt qaytarmır.

Manifest həm də **tamlıq yoxlayıcısıdır**. `completeness` bloku gözlənilən
qovluq siyahısını, mövcud olanları, çatışmayanları və hər qovluqdakı fayl
sayını v1-in öz sayı ilə yan-yana saxlayır. Bu paket üçün `complete: true`.

---

## 10. Açıq qalan iş

Bu cədvəl v2-nin 2026-09-21 vəziyyətidir. v3-də statusu dəyişən maddələr və
yeni maddələr bölmə 11.9-dadır.

| # | Nə | Status |
|---|---|---|
| 1 | `tools/build/lockup.py` -> `CAP_H = 10 * C / 3` | **açıq** - v1 əvəz olunanda faylda bir sətir |
| 2 | `tools/build/mark.py`-ın `markv2.py` ilə əvəz olunması | **açıq** - sahib təsdiqindən sonra |
| 3 | `masters.py` deskriptor döşəməsini hələ `floor_ok()` ilə hesablayır və `drawn` blokunu yazmır | **açıq** - `01-master/MEASUREMENTS.json`-dakı iki düzəliş **əl ilə** yazılıb; generator düzəlmədən `masters.py` yenidən işlədilsə silinirlər (v3: bölmə 11.9) |
| 4 | v1 paketindəki maskable ikon (`delivery/07-platform`, v1 indi `archive/v1-2026-08-24/delivery/07-platform/`) hələ `21.02` faiz kəsir | **açıq** - `delivery/` burada yalnız oxunur (v3: bölmə 11.9) |
| 5 | Stacked kiliddə çəkilən `M/C = 2.662690` | **açıq** - v1-dən miras, sözmarka mühərrikinə aiddir |
| 6 | `tools/build/platform.py`-nin `CIRCULAR` regex-inə `maskable` əlavəsi | **açıq** - v2-də çağırış yerində həll olunub |
| 7 | **`05-collateral` maşın oxunan ölçmə faylı göndərmir** | **açıq** - `collateralv2.py` yalnız çap edir; qalan beş qovluğun hər birində `MEASUREMENTS.json` var. Manifest boşluğu spesifikasiyanın öz cədvəlindən doldurur, amma bu, generatorun öz ölçməsi deyil |
| 8 | **`08-site` ölçmə faylı `v2-ai/build/_sitev2-measurements.json`-dadır**, paketin içində yox | **açıq** - rəqəmlər var, paketlə səyahət etmir (v3: `qa/08-website-measurements.json`, bölmə 11.9) |
| 9 | `04-pdf/MEASUREMENTS.json` -> `mark_vertex_err_pt` | **bağlandı** - narahatlıq köhnəlmiş sətirdən gəlirdi. Göndərilmiş faylda beş dəyər var və hamısı `4.00e-06` ilə `5.06e-06 pt` arasındadır, yəni reportlab-ın altı onluq rəqəm formatlaması. Müstəqil təkrar ölçmə eyni həddi verir (`6e-06 pt`) |
| 10 | Chromium ilə `16 / 32 px` parlaqlıq təkrar ölçməsi | **edilməyib** |
| 11 | Raster birləşmə taramasının `proof.py`-a sərt qapı kimi əlavəsi | **tövsiyə**, edilməyib |
| 12 | Dairəvi avatarda yeni nişanın gözlə təsdiqi | **edilməyib** |
| 13 | Fiziki istehsal sınağı (tikmə, trafaret, oyma) beş lövhə ilə | ilk partiya |
| 14 | Profildə `seam_x = 1300` artıq deskriptora bağlı deyil | **açıq** - yenidən seçilə bilər, bu paketdə dəyişdirilməyib |
| 15 | Kolleteral kontakt sətirləri | **boş** - şirkət qeydiyyatdan keçməyib, ünvan və telefon uydurulmayıb |

**Ad verdikti AMBER-dir, şərtlidir** və v2 işi onu dəyişmir. Bu sənəd hüquqi
məsləhət deyil.

---

## 11. v3 - iki rəngli nişan, tək giriş nöqtəsi, yeni düzüm (2026-09-26)

Bölmə 0-10 v2-nin qeydidir və yerində qalır. v3 onun üzərinə üç şey gətirir:
nişan iki rəngdə göndərilir, paket bir əmrlə qurulur, `delivery/` yalnız
işlədilən faylları saxlayır. **Həndəsə v2 ilə eynidir:** beş lövhə
(`f2-strict-system`), ink qutusu `18 x 18`, `[3,21]`-də, aralıqlar `2.7`
struktur / `1.35` örtük, reduktiv kəsik `24` lövhə pikselindən aşağı. **Kilid
dəyişmir:** `M / CAP_H = 2 / 1`.

### 11.0 Bir baxışda

| Nə | Nəticə |
|---|---|
| Nişanın rəngi | **iki rəng**: gövdə ink `#0F1317`, `i` və `A`-nı quran dörd lövhə petrol-600 `#0E6A82`; tünd fonda paper `#FAFAF7` və petrol-400 `#2FA8C7` (11.1) |
| Tək rəngli fayllar | master, raster və PDF-lər v2 arxivi ilə **bayt-bayt eynidir** (11.8) |
| Giriş nöqtəsi | `python tools/build/pack.py` - 9 builder, hamısı kod `0` (11.2) |
| Stage | repodan kənarda, `%TEMP%\kestridge-ai-brand-stage` (11.3) |
| Marşrut | hər stage faylı açıq qayda ilə düzülür; qaydasız fayl işi dayandırır (11.4) |
| Paket | `delivery/` - 272 indekslənmiş fayl + `README.md` + `MANIFEST.json` (11.4) |
| Sübut | ölçmə faylları və yoxlama şəkilləri `qa/`-dadır, 31 fayl (11.5) |
| Platforma kilidi | yatıq kompozisiyalarda kadrın `23.1 %`-indən `32.8 %`-inə; `72 %` en tavanının bağladığı dar bannerlərdə (məs. `1200 x 628`) daha az (11.6) |
| Tapılan qüsurlar | **8**, hamısı düzəldildi, heç biri göndərilmiş fayla çatmamışdı (11.7) |
| Yoxlama | `icons_pdf` 147 yoxlama OK, `verify_site` 64 keçdi / 0 uğursuz, `manifest` `complete True`, üç öz-sınaq sıfır FAIL (11.8) |
| Yeni açıq maddələr | 6 (16-21): 16, 17, 19, 20, 21 ölçülüb, 18 kod oxunmasından, sınanmayıb (11.9) |

### 11.1 Rəng mühərriki: `ink` lüğət ola bilər və yalnız yarpaqda açılır

v2-də `ink` hər yerdə bir rəng idi - hex sətri, raster yollarında RGB tuple.
v3-də o, həm də **rəng yolu** (colourway) ola bilər: hər lövhə üçün bir dolğu,
üstəlik nişanın yanındakı sözmarka və deskriptor. İki rəng yolu
`tools/build/mark.py`-dadır:

| lövhə / hissə | `color` (açıq fon), `mark.COLOUR` | `color-knockout` (tünd fon), `mark.COLOUR_KNOCKOUT` |
|---|---|---|
| `stem` | `#0F1317` ink | `#FAFAF7` paper |
| `arm`, `tittle`, `leg`, `crossbar` | `#0E6A82` petrol-600 | `#2FA8C7` petrol-400 |
| sözmarka | `#0F1317` | `#FAFAF7` |
| deskriptor | `#0E6A82` | `#2FA8C7` |

Lüğət hər çağırışdan **toxunulmadan** keçir və yalnız lövhənin və ya mətn
sətrinin həqiqətən doldurulduğu yerdə açılır, üç funksiya ilə:
`mark.plate_ink()`, `mark.text_ink()`, `mark.desc_ink()`. Builder ilə yarpaq
arasındakı funksiyalar lüğəti olduğu kimi ötürür. Bir istisna var:
`raster.mark_png()` (`raster.py` sətir 40) lüğəti `_rgb()`-dən yan keçirmək
üçün bir sətirlik növ yoxlaması aldı (`ink if isinstance(ink, dict) else _rgb(ink)`).
Bu üç funksiyanı çağıran fayllar (`grep` ilə sayılıb):
`mark.py`, `lockup.py`, `lockupv2.py`, `raster.py`, `vector.py`,
`icons_pdf.py`, `platformv2.py`, `verify_site.py`.

Bir rəng verildikdə üç funksiya o rəngi qaytarır, yəni tək rəngli yol əvvəlki
kimi işləyir. Bu iddia deyil, ölçüdür: tək rəngli master, raster və PDF-lər
`archive/v2-2026-09-21/delivery/` ilə bayt-bayt eyni çıxdı (11.8).

Yarpaqda üç yer ayrıca iş tələb etdi:

- **Şəffaf fonda raster** (`mark.raster(..., alpha=True)`). Tək rəngdə RGB
  müstəvisi bir ink ilə doldurulur və yalnız alfa kiçildilir. İki inklə kənar
  pikselin rəngi yalnız örtdüyü lövhənin rəngidir; düz alfanın RGB-sini
  ortalamaq şəffaf qaranı içəri çəkər və tünd haşiyə qoyardı. Ona görə rəng
  yolu premultiplied həll olunur (`mark.py`-dakı şərh).
- **Göndərilmiş pikseldən örtüyün oxunması** - `mark.coverage_axis()`, qüsur 7.
- **PDF yoxlayıcısı** - rəng yolu hər rəng üçün bir yol çəkir, qüsur 8.

**Niyə məhz bu bölgü.** Gövdə yalnız `K`-ya aid olan tək lövhədir. Qol həm də
`i`-nin gövdəsidir, nöqtə `i`-nin nöqtəsi, ayaq `A`-nın diaqonalı, tir `A`-nın
tiridir. Rəng bölgüsü **K + Ai** oxunuşunun özüdür. petrol-600 `#0E6A82`
saytın öz vurğu rəngidir (kestridge.com-da `--accent`). Reduktiv kəsikdə nöqtə
və tir düşür, gövdə (ink) və qol / ayaq (petrol) qalır, ona görə `16 px`
favicon da iki rənglidir. Seçimin gedişi `01-DECISION-RECORD.md`-də, istifadə
qaydaları `02-BRAND-GUIDELINES.md`-dədir.

Ölçülmüş kontrast (WCAG 2 nisbi parlaqlığı):

| cüt | nisbət |
|---|---|
| petrol-600 paper `#FAFAF7` üzərində | `5.90:1` |
| petrol-600 ağ `#FFFFFF` üzərində | `6.17:1` |
| petrol-400 ink `#0F1317` üzərində | `6.70:1` |
| petrol-400 səth `#171D23` üzərində | `6.10:1` |
| petrol-400 qara `#000000` üzərində | `7.55:1` |
| paper ink üzərində | `17.84:1` |
| petrol-600 ink-ə qarşı (bölgünün özü: gövdə ilə qol) | `3.02:1` |

Qrafik döşəmə hər yerdə `3.0:1` (WCAG 1.4.11), mətn döşəməsi `4.5:1`. Son
sətir ona görə vacibdir ki, iki ink təkcə çalarla yox, açıqlıqla da fərqlənir:
bölgü boz çapda da qalır.

**Tək generator.** v2-də `v2-ai/build/markv2.py` nişan generatorunun ikinci
surəti idi. v3-də o silinib; bir generator var - `tools/build/mark.py`, v2
yüksəldiləndən bəri v2 həndəsəsini daşıyan. Bölmə 1-dəki enjeksiya qalır, amma
indi `tools/build/mark.py`-nin özünü bağlayır: məsələn `collateralv2.py` və
`icons_pdf.py` `import mark as markv2` yazıb onu `sys.modules["mark"]`-a qoyur
və bağlanmanı `assert` ilə yoxlayır. v1 generatoru
`archive/v1-2026-08-24/tools-build-mark.py`-dir.

### 11.2 Tək giriş nöqtəsi: `tools/build/pack.py` və `tools/build/paths.py`

v2-də hər builder ayrıca işlədilirdi və öz qovluqlarını özü hesablayırdı.
v3-də bir əmr var (layihə kökündən):

```
python tools/build/pack.py              hər şeyi qur, sonra delivery/ və qa/-ni düz
python tools/build/pack.py --no-build   qurma, yalnız son stage-dən delivery/ və qa/-ni düz
```

`pack.py` builder-ləri asılılıq sırası ilə, hər birini ayrı prosesdə işlədir:

`masters` -> `icons_pdf` -> `collateralv2` -> `platformv2` -> `sitev2` ->
`render_banner` -> `reduced` -> `manifest` -> `verify_site`

Sıranın səbəbi `pack.py`-da yazılıb: `reduced` `platformv2`-nin yaratdığı
döşəmədən aşağı plitələri yenidən yazır, `manifest` hazır stage-i indeksləyir,
`verify_site` sayt fayllarını yoxlayır. **Hər hansı builder sıfırdan fərqli
kodla çıxsa, iş dayanır və `delivery/` ilə `qa/`-yə toxunulmur.**

`tools/build/paths.py` build-in oxuduğu və yazdığı qovluqların **tək
cədvəlidir**:

| Ad | Yer |
|---|---|
| `DELIVERY` | `delivery/` - yalnız `pack.py` yazır |
| `QA` | `qa/` |
| `ARCHIVE_V1`, `V1_DELIVERY`, `V1_MARK` | `archive/v1-2026-08-24/`, onun `delivery/`-si, `archive/v1-2026-08-24/tools-build-mark.py` |
| `ARCHIVE_V2` | `archive/v2-2026-09-21/` |
| `STAGE` | repodan kənar, 11.3 |
| `MARK_SPEC` | `tools/data/mark-plates-f2-strict-system.json` - dondurulmuş nişan spesifikasiyası |
| `PLATFORM_DIMENSIONS` | `tools/data/platform-dimensions.json` |

Səbəb qüsur 3 və 4-dür (11.7): yazıldığı gün bir şey, paket köçəndən sonra
başqa şey demək olan yol. `delivery/` v2 olandan sonra da "v1" kimi oxunurdu.
Bir cədvəl - səhv olmaq üçün bir yer, baxmaq üçün bir yer.

Hazırda builder-lər bu cədvəldən yalnız `STAGE` və `DELIVERY`-ni oxuyur
(`pack.py` həm də `QA`-nı). `ARCHIVE_V1`, `ARCHIVE_V2`, `V1_DELIVERY`,
`V1_MARK`, `MARK_SPEC` və `PLATFORM_DIMENSIONS` heç bir modulda işlədilmir:
`mark.py` sətir 169 nişan spesifikasiyasının, `platform.py` sətir 27
`platform-dimensions.json`-un, `collateral.py` sətir 24
`US-COLLATERAL-SPEC.md`-nin yolunu özü yığır (sonuncu `paths.py`-da yoxdur).
Açıq maddə 19.

Səkkiz builder çıxışını `paths.STAGE`-ə yazır və `paths`-ı özü idxal edir.
`verify_site` heç bir fayl yazmır: `sitev2`-nin stage-dəki sayt fayllarını
(`S.OUT_SITE`) açıb yoxlayır. v1 arxivinin yolu isə hələ beş modulda yerində
yığılır - açıq maddə 19, 11.9.

### 11.3 Stage repodan kənardadır

Builder-lər `delivery/`-ə yox, stage-ə yazır: `%TEMP%\kestridge-ai-brand-stage`
(`paths.STAGE`, `tempfile.gettempdir()` üzərindən). `KESTRIDGE_STAGE` mühit
dəyişəni onu başqa yerə köçürür. Stage build məhsuludur: `pack.py` onu hər tam
icrada silib yenidən qurur, repoya düşmür və `git status`-da görünmür.

`KESTRIDGE_STAGE` yalnız stage-i köçürür. `pack.py` düzməni yenə də repodakı
`delivery/` və `qa/`-yə edir (`paths.DELIVERY`, `paths.QA` sabitdir). Paketə
toxunmadan bir builder-i yoxlamaq üçün onu ayrıca işlət, məsələn
`python tools/build/masters.py`: o yalnız stage-ə yazır.

Stage daxilində builder-lər hələ v2 qovluq adları ilə yazır; yeni adlar yalnız
düzmə zamanı, 11.4-dəki qaydalarla verilir.

### 11.4 Açıq marşrut: qaydasız fayl işi dayandırır

Stage `delivery/` və `qa/`-yə `pack.py`-nın `route()` funksiyası ilə düzülür.
Hər stage faylı üçün cavab üçdən biridir: `delivery/`-də yer, `qa/`-də yer, ya
da səbəbi yazılmış `skip`. **Heç bir qaydanın tutmadığı fayl işi dayandırır**
və siyahısı çıxır: builder yeni bir şey yazmağa başlasa, ona `pack.py`-da yer
verilməlidir - təsadüfən atıla və ya göndərilə bilməz. İki stage faylı eyni
yerə düşsə də iş dayanır.

`skip` qaydası yalnız stage-in platforma və banner qovluqlarındakı `NOTES.md`
üçün var: stage-də belə fayl görünsə, göndərilmir, çünki kanonik nüsxə bu
qovluqdakı əl ilə yazılmış `04-PLATFORM-NOTES.md` və `05-BANNER-NOTES.md`-dir.
Son stage-də (2026-09-26) belə fayl yoxdur.

Düzmədən əvvəl `delivery/` təmizlənir; `00-docs/`-dakı əl ilə yazılmış
sənədlərə toxunulmur, orada yalnız generasiya olunan `06-REDUCED-CUT.md`
yenidən yazılır. `qa/` də təmizlənir, `qa/raster-merge-ground-truth.txt`
istisna (11.5). Sonra `delivery/README.md` və `delivery/MANIFEST.json` düzülmüş
paketdən yenidən yazılır, ona görə paketdən ayrı düşə bilmirlər.

Mətn qeydləri (`.md`, `.json`, `.txt`) surətlənəndə `pack.py`-nın `copy_out()`
funksiyası onları `translate()`-dən keçirir: içlərindəki stage yolları eyni
`route()` qaydası ilə göndərilən yollara çevrilir (məs.
`02-raster/kestridge-mark-main-16.png` ->
`02-logo-png/mono/kestridge-mark-positive-16.png`), JSON çevrilmədən sonra
yenidən parse olunur. Bu addım 9a9dabc commit-ində əlavə olunub. Diskdəki paket
ondan sonra `pack.py --no-build` ilə yenidən düzülüb (commit 2e61d97): `06-REDUCED-CUT.md`, `qa/reduced-cut.json` və
`qa/stage-manifest.json`-dakı fayl yolları artıq göndərilən adları daşıyır.
İkinci yoxlama turu üç boşluq tapdı və üçü də `translate()`-də bağlandı
(2026-09-26): JSON-da dırnaq içindəki çılpaq qovluq adı (məs.
`"where": "02-raster"`) indi çevrilir; sonunda durğu işarəsi olan yol
(`01-master/MEASUREMENTS.json.`) nöqtə ayrılaraq tanınır; `route()`-un `skip`
etdiyi iki əl yazısı qeyd (`07-platform/NOTES.md`, `09-banner/NOTES.md`)
kanonik yerinə, `00-docs/04-PLATFORM-NOTES.md` və `00-docs/05-BANNER-NOTES.md`-yə
yönəldilir. Yenidən düzülmədən sonra `qa/`-nın bütün mətn qeydlərində,
`06-REDUCED-CUT.md`-də və iki README-də stage adı sayı `0`-dır.

**Qovluq xəritəsi (v2 -> v3):**

| v2-də | v3-də |
|---|---|
| `01-master/` | `delivery/01-logo-svg/color/` və `delivery/01-logo-svg/mono/` |
| `02-raster/` | `delivery/02-logo-png/color/` və `delivery/02-logo-png/mono/` |
| `03-icons/` | `delivery/03-app-icons/` |
| `04-pdf/` | `delivery/04-logo-pdf/color/` və `delivery/04-logo-pdf/mono/` |
| `05-collateral/` | `delivery/05-stationery/`; `-guides.png` sübutları `qa/05-stationery-guides/` |
| `06-fonts/` | `delivery/06-fonts/` |
| `07-platform/` | `delivery/07-platforms/`; `_check/` sübutları `qa/07-platforms/` |
| `08-site/` | `delivery/08-website/` |
| `09-banner/` | `delivery/09-linkedin-banner/`; sübut və ölçülər `qa/09-linkedin-banner/` |
| kökdəki `00-BUILD-RECORD.md` və digər sənədlər | `delivery/00-docs/` |
| `07-platform/NOTES.md` | `delivery/00-docs/04-PLATFORM-NOTES.md` |
| `09-banner/NOTES.md` | `delivery/00-docs/05-BANNER-NOTES.md` |
| `REDUCED-CUT.md` | `delivery/00-docs/06-REDUCED-CUT.md` (generasiya olunur) |
| `REDUCED-CUT.json` | `qa/reduced-cut.json` |
| `RASTER-MERGE-GROUND-TRUTH.txt` | `qa/raster-merge-ground-truth.txt` |
| `01-master/MEASUREMENTS.json` | `qa/01-logo-svg-measurements.json` |
| `03-icons/MEASUREMENTS.json` | `qa/03-app-icons-measurements.json` |
| `04-pdf/MEASUREMENTS.json` | `qa/04-logo-pdf-measurements.json` |
| `09-banner/measurements.json` | `qa/09-linkedin-banner/measurements.json` |
| `v2-ai/build/_sitev2-measurements.json` | `qa/08-website-measurements.json` |
| kökdəki `MANIFEST.json` (`manifest.py`) | `qa/stage-manifest.json`; `delivery/MANIFEST.json`-u indi `pack.py` yazır |
| `v2-ai/build/*.py` | `tools/build/*.py` (`markv2.py` silinib) |

**Fayl adları:**

- Nişan adlarından `-main` düşdü (`main` kəsik adıdır):
  `kestridge-mark-main-positive.svg` -> `01-logo-svg/mono/kestridge-mark-positive.svg`;
  rəngli: `01-logo-svg/color/kestridge-mark-color.svg`.
- Tək rəngli rasterlərə `positive` əlavə olundu: `kestridge-mark-main-128.png`
  -> `02-logo-png/mono/kestridge-mark-positive-128.png`;
  `kestridge-lockup-horizontal-default-512.png` ->
  `02-logo-png/mono/kestridge-lockup-horizontal-default-positive-512.png`.
- Tək rəngli PDF-lər də: `kestridge-lockup-horizontal-default.pdf` ->
  `04-logo-pdf/mono/kestridge-lockup-horizontal-default-positive.pdf`.
- Rəngli rasterlər: `02-logo-png/color/kestridge-mark-color-<px>.png`,
  `kestridge-mark-color-knockout-<px>.png`,
  `kestridge-lockup-<kind>-<tag>-color-<h>.{png,jpg,webp}`.
- Kilid SVG-ləri: `kestridge-lockup-<kind>-<tag>-<way>.svg`, `<way>` bunlardan
  biridir: `color`, `color-knockout`, `positive`, `knockout`, `mono-black`,
  `mono-white`.

**Rəngli kompozisiyalar.** Tətbiq ikonları, favicon, kolleteral (vizit kartı,
blank, zərf, slaydlar, e-poçt imzası; tünd bölmə slaydı `color-knockout`),
bütün platforma şəkilləri, sayt başlıq kilidləri (açıq: `color`, tünd:
`color-knockout`), sayt ikonları (`icon.svg` tünd plitə: `color-knockout`;
`icon-light.svg`: `color`; `apple-icon.png`: səth üzərində `color-knockout`),
OpenGraph şəkilləri (`color-knockout`) və LinkedIn bannerləri
(`color-knockout`). Tək rəngli variantlar bir rəngli işlər (çap, oyma, faks)
üçün `mono/` qovluqlarında qalır.

**Qovluq sayları** (`delivery/MANIFEST.json` -> `folders`):

| Qovluq | Fayl |
|---|---|
| `00-docs` | 7 |
| `01-logo-svg` | 34 (`color` 12, `mono` 22) |
| `02-logo-png` | 153 (`color` 82, `mono` 71) |
| `03-app-icons` | 7 |
| `04-logo-pdf` | 10 (`color` 5, `mono` 5) |
| `05-stationery` | 9 |
| `06-fonts` | 6 |
| `07-platforms` | 35 |
| `08-website` | 9 |
| `09-linkedin-banner` | 2 |
| cəmi | 272 indekslənmiş fayl, üstəlik `README.md` və `MANIFEST.json` |

### 11.5 `qa/` qovluğu

v2-də ölçmə faylları və sübut şəkilləri göndərilən faylların yanında dayanırdı:
`01-master`, `03-icons`, `04-pdf`-də `MEASUREMENTS.json`, `09-banner`-də `measurements.json`, `07-platform/_check/`, `_` prefiksli banner sübutları,
kökdə `REDUCED-CUT.json`. Bölmə 7.2-dəki `alt-texture` dərsi məhz bu idi:
sübut faylı göndəriş kimi oxunurdu. v3-də onlar `delivery/`-dən çıxıb `qa/`-yə
keçdi - istifadə üçün yox, yoxlamaq üçün. `qa/` 31 fayldır (2026-09-26 sayılıb):

| Fayl / qovluq | Nəyi sübut edir |
|---|---|
| `qa/01-logo-svg-measurements.json` | kilidlərin nisbəti, ink qutuları, minimum ölçülər (`min_floor_height_px`, `floor_ok`) |
| `qa/03-app-icons-measurements.json` | ikon plitələrinin miqyası və kəsiyi |
| `qa/04-logo-pdf-measurements.json` | PDF-lərin təmiz vektor olduğu, təpə dəqiqliyi |
| `qa/05-stationery-guides/` | kolleteralın kəsim, təhlükəsiz zona və loqo sahəsi xətləri, 7 şəkil |
| `qa/07-platforms/` | dairəvi kəsim, maskable ikon və favicon yoxlama şəkilləri, onların `MANIFEST.json`-u |
| `qa/08-website-measurements.json` | sayt faylları, OpenGraph kontrastı, deskriptor qapısı |
| `qa/09-linkedin-banner/` | bannerin kəsim, kiçik ekran və loqo üst-üstə yoxlamaları (6 şəkil), bannerlərin işlətdiyi kilid `lockup-horizontal-color-knockout.svg`, `measurements.json` |
| `qa/reduced-cut.json` | `24 px`-dən aşağı nöqtə və tirin niyə düşdüyü |
| `qa/raster-merge-ground-truth.txt` | kiçik ölçülərdə lövhə birləşməsinin əsl ölçüsü |
| `qa/stage-manifest.json` | `manifest.py`-nin tam indeksi: `decisions`, `geometry`, `tiles`, `lockup`, `completeness`, `reduced_cut` blokları |
| `qa/README.md` | bu cədvəlin qısa variantı, `pack.py` yazır (`write_qa_readme()`) |

Hamısı build ilə yenidən yaranır və əl ilə redaktə olunmur. Bir istisna:
`qa/raster-merge-ground-truth.txt`-i heç bir builder yazmır; `pack.py` `qa/`-ni
təmizləyəndə onu saxlayır.

### 11.6 Platforma şəkillərində kilidin ölçüsü

`tools/build/platform.py` yatıq kilidin PNG **qutusunu** kətan hündürlüyünün
`0.30`-u götürürdü. Qutu hər tərəfdə `c` (`2.7u`) qoruq sahəsi daşıyır, ona
görə nişanın öz inki kadrın `0.30 x 18 / 23.4 = 0.2308`-inə (`23.1 %`)
düşürdü. Təsdiqlənmiş LinkedIn banneri nişanı `84 / 256 = 0.328` (`32.8 %`)
ilə qoyur. Sahib platforma örtüyünü görüb kilidi "köhnə nisbət" kimi oxudu;
nisbət isə hər ikisində `2.0` idi (hərf-hərf ölçülüb: düz hərflər
`K E T R I D A` cap `42 px`, nişan `84 px`; `S` və `G` yalnız dairəvi hərfin
aşması üzündən `44 px` oxunur). Fərq kadrlamada idi: `1.42x`.

Düzəliş: `HORIZONTAL_BOX = 0.328125 x (18 + 2 x 2.7) / 18 = 0.4266`,
`tools/build/platformv2.py`-da `PLAT.HORIZONTAL_BOX` kimi. En/hündürlük
nisbəti `1.6` və yuxarı olan platforma şəkillərinə (yatıq kilid
kompozisiyaları) tətbiq olunur. Stacked kompozisiya (`STACKED_BOX 0.34`) və
`72 %` en tavanı dəyişmir; tavan `1200 x 628` kimi dar bannerlərdə bağlayır.
`tools/build/platform.py`-nin öz `HORIZONTAL_BOX = 0.30` sətri yerində qalır -
dəyər çağırış yerində, `platformv2.py`-da əvəz olunur.

### 11.7 v3 build-ində tapılan və düzəldilən qüsurlar

Səkkizinin heç biri göndərilmiş fayla çatmamışdı.

| # | Qüsur | Nə idi | Düzəliş |
|---|---|---|---|
| 1 | **`lockupv2.py` deskriptoru iki dəfə miqyaslayırdı** | `DESC_CAP = 2/3 x lockup.DESC_CAP`, halbuki `lockup.py` yüksəldiləndən bəri artıq miqyaslanmış `2.7u`-nu saxlayır; nəticə `1.8u`. Faylın öz öz-sınağı 6 yerdə uğursuz idi - yüksəltmədən sonra işlədilməmişdi | `c`-dən birbaşa yazılır: `DESC_CAP = c = 2.7u`, `DESC_DROP = 7c/3 = 6.3u` |
| 2 | **v1 cap-ı `lockup.CAP_H`-dan oxunurdu** | `masters.py`, `collateralv2.py`, `platformv2.py`, `icons_pdf.py`, `manifest.py`; `lockup.CAP_H` yüksəltmədən bəri v2 dəyərini saxlayır. `masters` və `collateralv2` yalnız təsadüfən düz idi: miqyas əmsalları `1.0`-a düşürdü | açıq yazılır: `CAP_H_V1 = 5c = 13.5u` |
| 3 | **"v1-dən fərqlidir" yoxlamaları v2-ni özü ilə tutuşdururdu** | `platformv2.py`, `sitev2.py`, `manifest.py`, `reduced.py` `delivery/`-i və ya `tools/build/mark.py`-ı "v1" kimi oxuyurdu; ikisi də yüksəltmədən bəri v2-dir. `platformv2` 35 faylın 35-ni eyni tapıb uğursuz olurdu | hamısı `archive/v1-2026-08-24/`-dən oxuyur |
| 4 | **ikinci generator** | `v2-ai/build/markv2.py` generatorun ikinci surəti idi; onun miras yoxlaması `tools/build/mark.py`-ı v1 kimi idxal edirdi - v2-ni özü ilə tutuşdururdu | silindi; tək generator `tools/build/mark.py`-dir |
| 5 | **`platform.py` və `sitegen.py` kompilyasiya olunmurdu** | qoruyucu sətirlərində `\n` əvəzinə hərfi sətir sonu vardı (shell heredoc onları çökdürmüşdü). `platform.py`-dakı qoruyucu icazəli v2 çağırıcısını da bloklayırdı | düzəldildi; qoruyucu yalnız `OUT` `delivery/`-ə yönələndə rədd edir. `sitegen.py` v3 build-ində işlədilmir, `archive/v1-2026-08-24/tools-build-sitegen.py`-dadır |
| 6 | **`reduced.py` rəngli plitələrin üstündən tək rənglə çəkirdi** | döşəmədən aşağı platforma plitələri (`16/32/48` favicon sırası, `16 px` klassik) `platformv2`-nin təzə yaratdığı rəngli plitələrin üzərinə bir rənglə yenidən çəkilirdi | platformanın rəng yolu ilə çəkir və yenidən yazısı `platformv2`-nin faylını bayt-bayt təkrarlamasa **xəta ilə dayanır** |
| 7 | **örtük boz orta ilə oxunurdu** | ink ilə paper arasındakı boz orta tam petrol-600 pikselini `72 %` ink kimi oxuyur | rəng yolu örtüyü inklərinin paperdən eyni məsafədə durduğu tək kanalda oxuyur (`mark.coverage_axis`; `COLOUR` üçün qırmızı: ink `15`, petrol-600 `14`, paper `250`). YouTube su nişanı kiçiltmə probu v2 midton rəqəmlərini dəqiq təkrarlayır (`14 px`: `12.76 %`, `24 px`: `7.12 %`); boz orta ilə `14 px` `19.90 %` oxunurdu |
| 8 | **PDF yoxlayıcısı nişanı bir yol sayırdı** | `icons_pdf.py` nişanın bir doldurulmuş yol olduğunu fərz edirdi; rəng yolu PDF-i hər rəng üçün bir yol çəkir, lövhə sırası ilə qruplaşdırılmış | yoxlayıcı indi o qədər dolğu oxuyur |

Qüsur 1-in əkizi `manifest.py`-da qalıb və düzəldilməyib - açıq maddə 16, 11.9.

### 11.8 Yoxlama (son tam build, 2026-09-26)

| Yoxlama | Nəticə |
|---|---|
| `python tools/build/pack.py` | 9 builder, hamısı kod `0` |
| tək rəngli master, raster, PDF | v2 arxivi ilə **bayt-bayt eyni** - mühərrik dəyişikliyi bir ink üçün çıxışa təsir etmir |
| `icons_pdf` | 147 yoxlama OK, PASS |
| `collateralv2` | hamısı keçdi |
| `platformv2` | hamısı keçdi; ortaq adlı 32 faylın 0-ı arxivdəki v1 ilə eynidir |
| `verify_site` | 64 yoxlama keçdi, 0 uğursuz |
| `manifest` | `complete True` |
| öz-sınaqlar | `tools/build/mark.py`, `lockup.py`, `lockupv2.py` - sıfır FAIL |
| təmizləmədən sonra yenidən build | paket bayt-bayt təkrarlandı; yalnız `MANIFEST.json` dəyişdi, çünki sənəd hash-ları dəyişmişdi (9a9dabc-dən əvvəlki `pack.py` ilə) |
| reduktiv kəsik | `qa/reduced-cut.json` bölmə 5-in rəqəmlərini verir: hədd `24 px`, parlaqlıq `16 px`-də `0.7399 / 0.7597 / 0.7535`, `128 px`-də `0.7547 / 0.772 / 0.7666` (əsas / reduktiv / v1) |

### 11.9 Açıq qalan iş (v3)

v2-nin bölmə 10-dakı maddələrindən v3-də yenidən yoxlananlar:

| v2 # | Nə | v3 statusu |
|---|---|---|
| 1 | `lockup.py` -> `CAP_H = 10 * C / 3` | **bağlandı** - `tools/build/lockup.py` sətir 33 |
| 2 | `mark.py`-ın `markv2.py` ilə əvəzi | **bağlandı** - `markv2.py` silinib, tək generator `tools/build/mark.py` (qüsur 4) |
| 3 | `masters.py` deskriptor döşəməsi və `drawn` bloku | **yarıya qədər bağlandı** - döşəmə v2 commit-i `66f7460`-dən bəri generatordadır (`masters.descriptor_floor_ok()`; v2 mətnindəki "əl ilə" ondan əvvəlki `95c1e1d` vəziyyətidir, göndərilmiş v2 nüsxəsi artıq generatordan idi), `qa/01-logo-svg-measurements.json` deskriptor kilidi üçün `min_floor_height_px: 85` və `"64": false` yazır. `drawn` bloku yazılmır - maddə 17 |
| 4 | v1 maskable ikonu `21.02` faiz kəsir | **v3-ə aid deyil** - v1 paketi `archive/v1-2026-08-24/delivery/`-də dondurulub. v3 paketində maskable nisbət `0.56`-dır (`qa/stage-manifest.json` -> `tiles`) |
| 6 | `platform.py`-nin `CIRCULAR` regex-i | **açıq** - `tools/build/platform.py` sətir 45 hələ `maskable` tutmur; `platformv2.py` çağırış yerində həll edir |
| 7 | kolleteral ölçmə faylı yoxdur | **açıq** - `qa/`-də kolleteral üçün yalnız `qa/05-stationery-guides/` şəkilləri var, ölçmə faylı yoxdur |
| 8 | sayt ölçmə faylı paketdən kənarda | **bağlandı** - `qa/08-website-measurements.json`, digər qovluqların ölçmə faylları ilə yanaşı, hər build-də yenidən yaranır |

Maddə 5 (stacked kiliddə `M/C = 2.662690`) açıq qalır: `tools/build/wordmark.py`
v3 commit-ində (b6f3328) yalnız şərhlərdəki sənəd yollarında dəyişib. Maddə 9
v2-də bağlanmışdı. Maddə 10-15 v3-də yenidən yoxlanmayıb; bölmə 10-dakı kimi
oxunmalıdır.

v3 yoxlaması zamanı tapılan yeni maddələr:

| # | Nə | Status |
|---|---|---|
| 16 | **`manifest.py` deskriptoru hələ iki dəfə miqyaslayır** - qüsur 1-in əkizi. `DESC_CAP_V2 = LK.DESC_CAP * SCALE` (sətir 53), `LK.DESC_CAP` isə artıq `2.7u`-dur; `DESC_DROP_V2` də eyni (sətir 54). Nəticə `qa/stage-manifest.json`-da: `decisions` və `lockup` bloklarında `descriptor_cap_u: 1.8` (düzü `2.7`), `descriptor_drop_u: 4.2` (düzü `6.3`), `decisions` blokunda `descriptor_cap_u_v1: 2.7` (düzü `4.05`, bölmə 4.2; `archive/v1-2026-08-24/delivery/MANIFEST.json` `4.050000000000001` yazır). `files` blokunda hər deskriptor kilidi faylının öz sətrində `descriptor_cap_u` `2.7`-dir | **açıq** - yalnız qeyd faylıdır: `DESC_CAP_V2` və `DESC_DROP_V2` `manifest.py`-da yalnız manifest sahələrinə yazılır. Düzəliş qüsur 1-dəki kimidir: `c`-dən birbaşa |
| 17 | **Kilidlərin çəkilmiş (`drawn`) rəqəmləri heç bir qeyddə yoxdur.** `qa/stage-manifest.json` -> `lockup` -> `instances` altında hər kilidin `ink_box_u`, `aspect_ratio`, `ratio_M_over_C`, `wordmark_cap_u` sahələrində `drawn: null`, `files` blokunda `ratio_M_over_C_drawn: null`; `qa/01-logo-svg-measurements.json`-da `drawn` açarı yoxdur. v2 arxivi də eynidir. Bölmə 4.1-dəki `1.999929` kimi rəqəmlər yalnız sənədlərdədir | **açıq** - `masters.py` `drawn` blokunu yazmalıdır |
| 18 | **`--no-build` stage-in varlığını yoxlamır.** `plan()` stage-i `os.walk` ilə gəzir və stage yoxdursa boş siyahı qaytarır; `lay_out()` isə `delivery/`-i yenə təmizləyir. Kod oxunmasından nəticə (sınanmayıb): `00-docs/`-dakı əl ilə yazılmış sənədlərdən və `README.md`-dən başqa hər şey silinər (`06-REDUCED-CUT.md` də), `qa/`-də yalnız `raster-merge-ground-truth.txt` və yenidən yazılan `README.md` qalar, və `MANIFEST.json` yalnız sənədləri indeksləyər. Git tarixindən bərpa olunur | **açıq** - `--no-build`-ı yalnız stage mövcud olanda işlət (11.10) |
| 19 | **v1 arxivinin yolu `paths.py`-dan oxunmur** - yerində yığılır: `manifest.py` sətir 23, `mark.py` sətir 462, `platformv2.py` sətir 99 və 970, `reduced.py` sətir 85, `sitev2.py` sətir 64 və 65. Hamısı düzgün qovluğa göstərir, `mark.py` arxiv tapılmasa dayanır. Giriş dataları da belədir: `mark.py` sətir 169 (`mark-plates-f2-strict-system.json`), `platform.py` sətir 27 (`platform-dimensions.json`) və `collateral.py` sətir 24 (`US-COLLATERAL-SPEC.md`, `paths.py`-da yoxdur) yolu özü yığır; `paths.py`-dakı `ARCHIVE_V1`, `ARCHIVE_V2`, `V1_DELIVERY`, `V1_MARK`, `MARK_SPEC`, `PLATFORM_DIMENSIONS` heç bir modulda işlədilmir (11.2) | **açıq** - arxiv köçsə bir yox, yeddi yer dəyişməlidir |
| 20 | **`reduced.py` rəngli rasterləri taramır.** `regenerate()` yalnız `kestridge-mark-main-<px>.png`-i, `untouched_scan()` `02-raster`-də yalnız tək rəngli adları gəzir; ona görə `qa/reduced-cut.json` v3-də də `regenerated` 3, `untouched` 68 sətir verir və `delivery/02-logo-png/color/`-dakı 82 fayl taranmır. `kestridge-mark-color-16.png` və `kestridge-mark-color-knockout-16.png` reduktiv kəsiklə göndərilir (2026-09-26 təzə render ilə tutuşdurulub), amma qeyddə yoxdur | **açıq** |
| 21 | **`manifest.py` v2 rəng qaydasını yazır.** `qa/stage-manifest.json` -> `decisions` -> `accent_rule`: `"petrol yeganne xromatik aksentdir; nishan hech vaxt petrol chekilmir"` (`manifest.py` sətir 755). v3-də nişanın Ai lövhələri petroldur (bölmə 11.1) | **açıq** - yalnız qeyd faylıdır |

LinkedIn banner deskriptor sətri v3-də də yazılmır; tövsiyə və ölçmə
`05-BANNER-NOTES.md`-dədir.

### 11.10 Paketi yenidən qurmaq

Layihə kökündən (`projects/kestridge-ai-brand/`):

1. **Tam build:** `python tools/build/pack.py`. Doqquz builder stage-ə yazır,
   biri uğursuz olsa `delivery/` və `qa/`-yə toxunulmur. Uğurlu icranın
   sonunda `delivery/` və `qa/` stage-dən düzülür, `delivery/README.md`,
   `delivery/MANIFEST.json` və `qa/README.md` yenidən yazılır.
2. **Yalnız düzmək:** `python tools/build/pack.py --no-build` - son stage-dən.
   Yalnız stage (`%TEMP%\kestridge-ai-brand-stage` və ya `KESTRIDGE_STAGE`)
   mövcud olanda işlət (maddə 18).
3. **`00-docs/`-da sənəd redaktə edəndən sonra:** sənədlərin `sha256`-sı
   `delivery/MANIFEST.json`-dadır. Manifesti tam build ilə (1) və ya stage
   mövcuddursa `--no-build` ilə (2) yenidən yaz. `06-REDUCED-CUT.md`-ni əl ilə
   redaktə etmə - onu `reduced.py` generasiya edir.
4. **Öz-sınaqlar** (`pack.py` onları işlətmir): `python tools/build/mark.py`,
   `python tools/build/lockup.py`, `python tools/build/lockupv2.py`.
5. **Yeni builder çıxışı:** stage-də qaydasız fayl işi dayandırır. Yeni faylın
   yeri `pack.py`-nın `route()` funksiyasına yazılmalıdır.
6. **Bir builder-i ayrıca yoxlamaq:** `python tools/build/<builder>.py` - yalnız
   stage-ə yazır. `KESTRIDGE_STAGE` stage-i köçürür, `delivery/` və `qa/`-ni yox.

v1 və v2 build-ləri yenidən qurulmur: onların paketləri
`archive/v1-2026-08-24/` və `archive/v2-2026-09-21/` altında dondurulub və
git-də tam izlənir (commit 79b9987). v3 işi ad verdiktini dəyişmir.
