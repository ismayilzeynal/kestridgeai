# Kestridge AI - build fazası qeydi

| | |
|---|---|
| Layihə | `kestridge-ai-brand` |
| Sənəd | `build/00-BUILD-RECORD.md` |
| Tarix | 2026-08-24 |
| Rol | icra və müstəqil ölçmə |
| Giriş | `concept/00-CONCEPT-BRIEF.md` bölmə 10 (build siyahısı) |
| Çıxış | `delivery/` (132 fayl), `tools/build/` (7 modul), bu qeyd |

Bu sənəd nə çəkildiyini yox, **nəyin ölçülüb təsdiqləndiyini** yazır. Briefin
hər rəqəmi burada müstəqil hesablanıb. Uyğun gəlməyən yerlər bölmə 4-dədir və
gizlədilmir.

---

## 0. Bir baxışda

| Nə | Nəticə |
|---|---|
| Briefin bölmə 4.5 həndəsə iddiaları | **hamısı təsdiqləndi** |
| Briefin bölmə 5.7 **en** sütunu (`en/cap`) | **dördüncü onluğa qədər bərpa olundu** |
| Briefin bölmə 5.7 **hündürlük** sütunu | **səhvdir** - brief öz 5.3-ü ilə ziddiyyətdədir, bax 3.4 |
| Briefin bölmə 6 kilid hesabları | **təsdiqləndi** - yatıq `154.899u` (brief `154.89u`) |
| Briefin bölmə 7 parlaqlıq rəqəmləri | təsdiqləndi, 32 px-də fərq 0.001 |
| `US-COLLATERAL-SPEC.md` bölmə 10 cədvəli | 38 sətrin **hamısı** öz düsturu ilə uyğundur |
| Briefdə tapılan qüsur | **4** - bölmə 4 və 3.4 |
| **Mənim kodumda tapılan qüsur** | **1, qərar reyestri tapdı** - bölmə 3.5 |
| Təhvil paketi | 132 fayl, 4.39 MB, sha256 manifesti ilə |

**Bu sənəd bir dəfə düzəlib.** İlk yazılışı `risk 13`-ü bağlanmış elan edirdi və
kilidin əsas sətrini `KESTRIDGE` yazırdı. `build/01-DECISION-RECORD.md` hər
ikisini geri qaytardı və mənim `wordmark.py`-da bir aralıq tracking qüsuru
tapdı. Düzəlişlər bölmə 3.4 və 3.5-dədir.

---

## 1. Alət mühiti - nə var, nə yoxdur

Bu maşında **heç bir SVG rasterləşdiricisi yoxdur**: `cairosvg`, `inkscape`,
`rsvg-convert`, `resvg` - dördü də quraşdırılmayıb.

**`convert` PATH-də var, amma ImageMagick deyil.** O, `C:\Windows\system32\convert.exe`,
yəni FAT-dan NTFS-ə çevirən Windows utilitidir. Rasterləşdirmə üçün çağırılsa
yanlış proqram işə düşür. **Çağırılmır.**

Bu itki deyil, çünki nişan üç düzxətli poliqondur:

| Nə | Necə çəkilir |
|---|---|
| Nişan | `PIL` poliqon doldurması, superseçmə ilə (`tools/build/mark.py`) |
| Sözmarka, raster | FreeType, qlif-qlif, shaper-in verdiyi addımda (`tools/build/raster.py`) |
| Sözmarka, vektor | `fontTools` konturları birbaşa `reportlab` yoluna (`tools/build/vector.py`) |
| PDF | `reportlab`, sonra şrift istinadları təmizlənir |

### 1.1 Şriftlər müvəqqəti qovluqdan asılı idi

Mövcud alətlər (`tools/measure.py`, `tools/render.py`) şrift korpusunu
`%TEMP%\kestridge-fonts`-dan oxuyur. Windows o qovluğu istənilən vaxt təmizləyə
bilər. Repodakı nüsxə - `system/scripts/brand/fonts/` - **bayt-bayt eynidir**
(`sha256` yoxlanıldı), ona görə build zənciri ona bağlandı.

### 1.2 Lisenziya faylları ümumiyyətlə yox idi

OFL 1.1 şrift paylananda lisenziyanın da getməsini tələb edir.
`US-TYPEFACE-PERCEPTION.md` "hər üçünün `OFL.txt`-i yoxlanıb" yazır, amma fayllar
saxlanılmamışdı. Üçü də çəkildi (HTTP 200) və şrift anbarına yazıldı:

```
system/scripts/brand/fonts/OFL-archivo.txt          4388 bayt
system/scripts/brand/fonts/OFL-inter.txt            4377 bayt
system/scripts/brand/fonts/OFL-jetbrains-mono.txt   4399 bayt
```

### 1.3 Şrift versiyaları - `name` cədvəlindən oxundu, yaddaşdan yox

| Fayl | Ad | Versiya | upm | Oxlar |
|---|---|---|---|---|
| `archivo.ttf` | Archivo | **2.001** | 1000 | `wght 100..900`, **`wdth 62..125`** |
| `inter.ttf` | Inter Variable | **4.001** | 2048 | `opsz 14..32`, `wght 100..900` |
| `jetbrains-mono.ttf` | JetBrains Mono | **2.211** | 1000 | `wght 100..800` |

Üçü də `US-TYPEFACE-PERCEPTION.md`-in iddia etdiyi versiyadır. Archivo-nun
`wdth` oxu 62-dən 125-ə qədərdir, yəni briefin tələb etdiyi `wdth 70` və
`wdth 100` **hər ikisi faylın içindədir** - ikinci lisenziya lazım deyil.

---

## 2. Həndəsə: brief, generator və eskiz üç yerdə eynidir

`tools/build/mark.py` konturları eskiz SVG-dən **köçürmür** - onları bölmə 4.2-nin
parametrlərindən (`c = 2.7`, `k = c/4`, `w`, `a`) yenidən törədir və sonra
eskizlə **təpə-təpə** müqayisə edir. Uyğunsuzluq olsa öz-sınağı düşür.

İki səhv törəmə cəhdi tutuldu və düzəldildi: 45 gedişi `y = 21`-də yox
`y = 21 - k`-də bitir, və sıx kəsik mürəkkəb qutusunun mərkəzinə görə
sürüşdürülür (`x0 = 2.55`, `3 - 0.45`, çünki qutu 18.9u-dur).

### 2.1 Ölçülmüş invariantlar

| İddia (brief 4.5) | Ölçülən | Nəticə |
|---|---|---|
| Mürəkkəb qutusu `18.000 x 18.000` | `18.000 x 18.000` | uyğun |
| Ən kiçik daxili bucaq **90.0** (hər üç kontur) | `90.0 / 90.0 / 90.0` | uyğun |
| Aralıq gövdə...qol | `2.7000` | uyğun |
| Aralıq gövdə...ayaq | `2.7000` | uyğun |
| Aralıq qol...ayaq | `2.7000` | uyğun |
| Qol/ayaq üst-üstə düşmə `4.325u` | `4.325u` | uyğun |
| Ümumi mürəkkəb `134.089 u²` | `134.089 u²` | uyğun |
| Paylar `53.7 / 28.2 / 18.1` faiz | `53.7 / 28.2 / 18.1` | uyğun |
| Qol/gövdə çəki nisbəti `0.8839` | `0.883883` | uyğun |
| Bağlanma eyniliyi `w + a - 2k = 7.65` | `7.65` | uyğun |
| Asimmetriya `2.7` iki oxda | `2.700 / 2.700` | uyğun |
| Yarımdiaqonal `12.728u` > daxili dairə `12.0u` | `12.728u` | uyğun |
| Dairəvi qayda `0.70` -> `11.879u` | `11.879u` | uyğun |

Sıx kəsik üçün eyni yoxlama: qutu `18.900 x 18.000`, bucaqlar yenə `90.0`,
üç aralıq yenə `2.7000`, bağlanma eyniliyi qəsdən `8.55`.

**Briefin bölmə 4.5-dəki heç bir rəqəm ölçmədə düşmədi.**

### 2.2 Dairəvi qayda sıx kəsik üçün də işləyir

Brief `0.70` qaydasını yalnız əsas kəsik üçün çıxarmışdı. Sıx kəsik üçün
hesablandı: yarımdiaqonal `0.70` miqyasında `11.600u`, daxili dairədən (`12.0u`)
kiçikdir. **Qayda hər iki kəsik üçün etibarlıdır** və bu, briefdə yazılmayıb.

---

## 3. Sözmarka və kilidlər

### 3.1 Vahid tələsi - briefin `pm`-i em-in mində biridir

`00-CONCEPT-BRIEF.md` bölmə 5 hər yerdə `pm em` yazır. `US-TYPEFACE-PERCEPTION.md`
isə öz `pm`-ini **cap hündürlüyünün** mində biri kimi təyin edir. İkisi
qarışdırılsa sözmarka `1000/686 = 1.458` dəfə səhv çıxır.

Briefin öz üç rəqəmi məsələni birmənalı həll edir:

| Briefin öz sətri | Yoxlama | Nəticə |
|---|---|---|
| 5.1: "söz boşluğu `0.45 x cap` = **309 pm em**" | `0.45 x 686 = 308.7` | em |
| 5.2: "stem **130 pm**", "stem/cap **19.0%**" | `130 / 686 = 18.95%` | em |
| 5.5: tracking `-14` -> CSS `letter-spacing: -0.014em` | birbaşa | em |

**Brief daxilən uyğundur.** Ziddiyyət iki sənədin konvensiyası arasındadır və
brend kitabında bir dəfə yazılmalıdır.

### 3.2 Bölmə 5.7 - en sütunu dəqiq bərpa olundu

Cüt düzəlişləri (bölmə 5.4) və tracking `-14` tətbiq olunmuş halda, konturların
əsl bezier ekstremumlarından ölçüldü:

| Sətir | `wdth` | `en/cap` ölçülən | brief | fərq |
|---|---|---|---|---|
| `KESTRIDGE AI` | 100 | **9.8407** | 9.840 | 0.0007 |
| `KESTRIDGE` | 100 | **8.0175** | 8.020 | 0.0025 |
| `KESTRIDGE AI` | 70 | **7.3800** | 7.380 | 0.0000 |
| `KESTRIDGE` | 70 | **5.9169** | 5.915 | 0.0019 |

Bu, briefin **birbaşa ölçdüyü** kəmiyyətdir və bərpa olunandır.

### 3.3 Kilid hesabları

| Kilid | Sətir | Ölçülən | brief |
|---|---|---|---|
| Yatıq | `KESTRIDGE AI` | ink **`154.899u x 18u`**, AR **8.6055** | `154.89u`, 8.61 |
| Yatıq | `KESTRIDGE` | ink `130.286u x 18u`, AR 7.2381 | (yoxdur) |
| Stacked | `KESTRIDGE AI` | blok `49.82u x 29.05u`, AR 1.715 | `49.82 x 29.03`, 1.72 |
| Stacked | `KESTRIDGE` | blok `39.94u x 29.05u`, AR 1.375 | (yoxdur) |

`M/C` nisbətləri dəqiqdir: yatıqda `4/3`, stacked-də `8/3`. Aralıq hər ikisində
`1.5c = 4.05u`.

`KESTRIDGE` tək sətirlər **yeni**dir - heç yerdə ölçülməmişdi. Onlar ehtiyat
markanın qabaritləridir (bax 3.5).

### 3.4 Briefin 5.7 hündürlük sütunu səhvdir - risk 13 belə bağlanır

Brief `risk 13`-ü özü qaldırıb: `9.70`, `10.26` və `9.553` üç fərqli rəqəmdir.
Qərar reyestri bunu "üç fərqli kəmiyyət, bir zəncir" kimi təsnif etdi, amma
riski **bağlamadı**. Ölçmə onu bağlayır və cavab briefin gözlədiyi deyil.

Şrift faylından oxundu (Archivo `wght 600`):

| `wdth` | overshoot daşıyan qliflər | ink hündürlüyü | `h/cap` | brief 5.7 |
|---|---|---|---|---|
| 100 | `G` (+12 / -12), `S` (+12 / -12) | `698 - (-12) = 710` | **1.0350** | 1.030 |
| 70 | `G` (+13 / -12), `S` (+13 / -12) | `699 - (-12) = 711` | **1.0364** | 1.035 |

**`1.0350` briefin öz 5.3 sətrindən çıxır** - "`G`-nin 12 pm overshoot-u" -
`686 + 2 x 12 = 710`. Yəni brief öz iki bölməsi arasında ziddiyyətdədir və
5.7 cədvəlindəki `1.030` yanlışdır.

Bunun nəticəsi AR sütunudur, çünki `AR = (en/cap) / (h/cap)`:

| Sətir | `wdth` | brief AR | düzəlmiş AR |
|---|---|---|---|
| `KESTRIDGE AI` | 100 | 9.553 (`9.840 / 1.030`) | **9.508** (`9.8407 / 1.0350`) |
| `KESTRIDGE` | 100 | 7.786 | **7.746** |
| `KESTRIDGE AI` | 70 | 7.130 | **7.121** |
| `KESTRIDGE` | 70 | 5.715 | **5.709** |

Əlavə qüsur: brief 5.3 "**yalnız** `G`-nin overshoot-u" yazır. Ölçmə iki qlif
tapır - `G` **və** `S`, hər ikisi eyni 12 vahid. Hündürlüyə təsiri yoxdur,
amma "yalnız" sözü yanlışdır.

### 3.5 Mənim kodumda bir aralıq tracking qüsuru vardı

`build/01-DECISION-RECORD.md` bənd F5 `wordmark.py`-da qüsur tapdı: tracking
9 aralığa verilirdi, brief isə 10 tələb edir. Boşluqdan **əvvəlki** aralıq
atlanırdı.

Yoxlandı və təsdiqləndi:

| Sətir | `wdth` | 9 aralıq | 10 aralıq | brief |
|---|---|---|---|---|
| `KESTRIDGE AI` | 100 | 9.8611 | **9.8407** | 9.840 |
| `KESTRIDGE AI` | 70 | 7.4004 | **7.3800** | 7.380 |

Fərq dəqiq `14 / 686 = 0.0204` cap vahididir - bir tracking addımı.
`KESTRIDGE` tək sözdür, boşluğu yoxdur, ona görə orada iki rejim eynidir və
qüsur yalnız iki sözlü sətirdə görünürdü.

**Qüsuru mənim öz sınağım gizlədirdi:** tolerans `0.06` idi, xəta `0.02`.
Tolerans `0.004`-ə daraldıldı. Düzəlişdən sonra yatıq kilid `154.899u` verir -
briefin `154.89u`-su ilə üst-üstə düşür, yəni qərar reyestrinin F6 bəndi də
bağlandı.

Söz boşluğunun **özü** hələ də tracking almır (`0.45 x cap`, sabit) - o,
briefin ayrıca qaydasıdır və dəyişmir.

---

## 4. Briefdə tapılan üç qüsur

### 4.1 Sıx kəsik piksel şəbəkəsi üstünlüyünü pozur - qeydə alınmayıb

Brief bölmə 3.3 (etiraz F cavabı) yeni çərçivəni bu arqumentlə müdafiə edir:

> "`3u` və `21u` artboard-un `1/8` və `7/8`-idir, ona görə mürəkkəb qutusunun
> kənarları 16, 32, 48, 128 və 512 px-də **tam piksel sərhədinə** düşür"

Bu, **əsas kəsik üçün doğrudur**. Sıx kəsik üçün yanlışdır və brief bunu heç
yerdə demir:

| px | Əsas kəsik, sol/sağ kənar | Sıx kəsik, sol/sağ kənar |
|---|---|---|
| 16 | `2.000 / 14.000` | `1.700 / 14.300` |
| 32 | `4.000 / 28.000` | `3.400 / 28.600` |
| 48 | `6.000 / 42.000` | `5.100 / 42.900` |
| 128 | `16.000 / 112.000` | `13.600 / 114.400` |
| 512 | `64.000 / 448.000` | `54.400 / 457.600` |

Səbəb struktura bağlıdır: sıx kəsiyin qutusu `18.9u`-dur, `24u` lövhənin
`1/8`-inin qatı deyil. Sürüşmə heç bir ölçüdə itmir.

Yəni brief `<= 32 px` üçün məcburi etdiyi kəsik, məhz həmin ölçüdə öz iddia
etdiyi üstünlüyü ləğv edir.

**Ölçülmüş qiymət** (mürəkkəb `neutral-950`, kağız `neutral-025`):

| px | Kəsik | Orta parlaqlıq | Saf piksel | Şəbəkə |
|---|---|---|---|---|
| 16 | əsas | 0.764 | **63.7%** | düzülür |
| 16 | sıx | **0.743** | 58.6% | sürüşür |
| 32 | əsas | 0.764 | **70.2%** | düzülür |
| 32 | sıx | **0.743** | 65.9% | sürüşür |

Sıx kəsik `0.021` parlaqlıq qazanır, `5.1` faiz bənd saf piksel itirir.

### 4.2 Rəng qərarı solğunluq problemini onsuz da həll edir

Briefin risk 6-sı belədir: "Kiçik ölçüdə solğunluq tam həll olunmayıb. 16 px orta
parlaqlıq: əsas kəsik 0.801, sıx kəsik 0.781. Sıx kəsik **məcburidir**."

Həmin rəqəmlər müvəqqəti mürəkkəb `#1B2430` üçündür. Barışdırma qərarı mürəkkəbi
`neutral-950 #0F1317`-yə keçirdi. Ölçmə:

| Variant | 16 px orta parlaqlıq | Saf piksel | Şəbəkə |
|---|---|---|---|
| Briefin seçimi: sıx + `#1B2430` | 0.774 | 60.9% | sürüşür |
| Əsas kəsik + `#0F1317` | 0.764 | 63.7% | düzülür |
| Əsas kəsik + `#000000` | **0.763** | **81.6%** | düzülür |

**Əsas kəsik yeni mürəkkəblə onsuz da briefin sıx kəsik üçün hədəflədiyi
`0.781`-dən tünddür.** Yəni ikinci masterin əsas əsaslandırması qalmır.

Bu, briefin risk 5-inə birbaşa toxunur: "İki master lazımdır (əsas + sıx)...
iki master real xərcdir".

**Tövsiyə:** sıx kəsik təhvil paketində saxlanılır (fayllar var), amma
`<= 32 px` üçün **məcburi** statusu yenidən baxılmalıdır. Bu, geri qaytarıla
bilən qərardır və rəqəmlər yuxarıdadır.

**Real mühərrikdə yoxlandı.** İlk yazılış bu müqayisənin yalnız mənim
rasterləşdiricimdə (PIL + BOX) aparıldığını qeyd edirdi və real hədəfdə
təkrarlanmasını tələb edirdi. Təkrarlandı: `tools/build/browsercheck.py`
nişanı **Chromium**-da `device_scale_factor=1` ilə render edir - briefin özünün
işlətdiyi mühərrik.

| px | Kəsik | Chromium lum | PIL lum | fərq | Chromium saf piksel |
|---|---|---|---|---|---|
| 16 | əsas | **0.7703** | 0.7644 | +0.0059 | **69.1%** |
| 16 | sıx | 0.7497 | 0.7427 | +0.0070 | 64.1% |
| 32 | əsas | **0.7659** | 0.7644 | +0.0015 | **71.5%** |
| 32 | sıx | 0.7445 | 0.7428 | +0.0017 | 67.5% |

İki nəticə:

1. **PIL və Chromium arasındakı fərq `0.002 ... 0.007`-dir** - üçüncü onluqdan
   aşağı. Yuxarıdakı bütün müqayisələr etibarlıdır.
2. **Münasibət eynidir:** sıx kəsik 16 px-də `0.021` tündlük qazanır və
   `5.1` faiz bənd saf piksel itirir; 32 px-də `0.021` qazanır, `4.0` itirir.

Əlavə təsdiq: brief bölmə 7 əsas kəsik üçün 16 px-də "təmiz ağ **69.1%**"
yazır. Chromium dəqiq `69.1%` verir - briefin öz rəqəmi bərpa olundu.

### 4.3 Rasterləşdirmə tələsi - PIL poliqonu artıq doldurur

`PIL.ImageDraw.polygon` konturu da çəkir, ona görə perimetr boyunca təxminən
yarım piksel artıq örtük verir. Xəta ümumi cihaz həllinə tərs mütənasibdir:

| `px * ss` | Örtük xətası |
|---|---|
| 128 | +13.83 f.b. |
| 256 | +6.60 f.b. |
| 512 | **+0.21 f.b.** |
| 4096 | +0.06 f.b. |

Bu görülmədən 16 px parlaqlıq rəqəmləri təxminən `0.02` yanlış çıxırdı.
`mark.py` indi superseçməni `px * ss >= 512` olacaq şəkildə seçir və
`BOX` (sahə ortalaması) süzgəci ilə həll edir - `LANCZOS` overshoot verir və
nəticəni süni tündləşdirir.

Analitik yoxlama: `1 - örtük x (1 - ink_lum)`. Əsas kəsik `0.799`, sıx kəsik
`0.779` - briefin `0.801` və `0.781` rəqəmləri məhz bunlardır.

---

## 5. Təhvil paketi

```
delivery/
  01-master/     24  master SVG (nişan 2 kəsik x 4 variant, kilid 2 tip x 2 sətir x 4 variant)
  02-raster/     74  PNG (alfa) + JPG + WEBP
  03-icons/       7  favicon.ico (7 kadr) + kvadrat/dairə plitələr
  04-pdf/         5  saf vektor PDF
  05-collateral/ 16  vizit kartı, letterhead, zərf, slaydlar, e-poçt imzası + bələdçi qatları
  06-fonts/       6  3 TTF + 3 OFL lisenziyası
  MANIFEST.json      hər fayl üçün sha256, ölçü, mənşə, ölçülmüş metadata
```

Kilid faylları `default` və `reserve` etiketi daşıyır, `primary` / `secondary`
yox. Səbəb: C2 **şərti** hökmdür (bax `01-DECISION-RECORD.md` bölmə 2), ona görə
mübahisəli qərar fayl adına bişirilmir. Hazırda `default = KESTRIDGE AI`,
`reserve = KESTRIDGE`. Qapılar açılanda `package.py`-dakı `lockup_gates_open`
bayrağı çevrilir və paket yenidən çıxarılır - başqa dəyişiklik lazım deyil.

### 5.1 PDF-lər saf vektordur - yoxlanıla bilər

Məzmun axını `pypdf` ilə açılıb sayıldı, xam bayt axtarışı ilə yox:

| Fayl | Bayt | `m` | `l` | `c` | Mətn op. | `/Font` | `/XObject` |
|---|---|---|---|---|---|---|---|
| `kestridge-mark-positive.pdf` | 809 | 3 | 13 | 0 | 0 | yox | yox |
| `...lockup-horizontal-primary.pdf` | 8094 | 14 | 83 | 99 | 0 | yox | yox |
| `...lockup-stacked-secondary.pdf` | 8800 | 17 | 96 | 106 | 0 | yox | yox |

Nişanın PDF-i **cəmi 809 baytdır** və dəqiq 16 təpə daşıyır - üç poliqon,
əyri seqment sıfır.

`reportlab` hər səhifəyə boş `BT /F1 12 Tf ... ET` bloku yazır. O blok heç nə
çəkmir (`Tj` yoxdur), amma faylda şrift istinadı buraxır və prepress preflight
istənilən şrift istinadını bayraqlayır. Blok və `/Font` resursu silinir.

### 5.2 Manifest yoxlandı

116 faylın **hamısının** `sha256`-sı və bayt ölçüsü manifestlə uyğundur.

### 5.3 Kolleteral spesifikasiyası müstəqil təsdiqləndi

`tools/build/collateral.py` `US-COLLATERAL-SPEC.md` bölmə 10 cədvəlini **oxuyur**
(nüsxə saxlamır) və hər `canvas_px` rəqəmini sənədin öz düsturu ilə yenidən
hesablayır:

```
canvas_px = ceil((dim + 2 * bleed) * dpi)
```

**38 sətrin hamısı uyğundur.** 23-ü `verified: yes`, 15-i `partial`.
İki sətir (`table-throw-8ft`, `backdrop-20x8`) 120 Mpx-dən böyükdür və
artefakt kimi çəkilmir - ölçüləri saxlanılır, çap faylı sifariş anında qurulur.

---

## 6. Tapılan və düzəldilən build qüsurları

| Qüsur | Nə idi | Düzəliş |
|---|---|---|
| **Tracking bir aralıq əskik** (F5) | boşluqdan əvvəlki aralıq atlanırdı; öz toleransım (`0.06`) `0.02` xətanı udurdu | `elif nxt:` şərti, tolerans `0.004`. Bax 3.5 |
| **Müvəqqəti rəng default olaraq qalmışdı** (F1, F2) | `mark.py`, `lockup.py`, `raster.py` default-ları və iki referans SVG hələ `#1B2430` daşıyırdı; `ink` ötürməyi unudan çağırış səssizcə plaseholder göndərərdi | `INK_DEFAULT = "#0F1317"`, `PAPER_DEFAULT = "#FAFAF7"`; referans SVG-lər yeniləndi |
| **Kilidin əsas sətri vaxtından əvvəl dəyişdirilmişdi** | paket `primary = KESTRIDGE` çıxarmışdı, halbuki C2 şərtidir | `lockup_gates_open` bayrağı, `default` / `reserve` adlandırması |
| `favicon.ico` bir kadrlı çıxırdı | Pillow baza şəkildən **böyük** ölçüləri səssizcə atır; baza 16 px verilmişdi | ən böyük kadr baza olur, sonra kadr sayı yoxlanılır və uyğun deyilsə build dayanır |
| Parlaqlıq `0.02` yanlış | PIL poliqonu artıq doldurur (4.3) | superseçmə `px * ss >= 512`, `BOX` süzgəc |
| Sıx kəsik törəməsi səhv yerdə idi | 45 gedişi `y=21`-də bitirdi, `y=21-k`-də bitməli idi | öz-sınağı eskizlə təpə-təpə müqayisə edir |
| Şrift korpusu `%TEMP%`-də | Windows silə bilər | repodakı bayt-eyni nüsxəyə bağlandı |
| OFL lisenziyaları yox idi | OFL 1.1 pozuntusu | üçü də çəkildi və paketə qoyuldu |

---

## 7. Hələ edilməyən

| Nə | Niyə |
|---|---|
| Fiziki sınaq partiyası | Brief bölmə 10 bənd 7 - tikmə, trafaret, oyma. Yalnız tərs kəsik |
| **Kolleteral artefaktları** | Generator hazırdır və cədvəl təsdiqlənib; artefakt çəkilməsi tərtibat qərarı tələb edir |
| Brend qaydaları sənədi | Barışdırma qərar reyestri bağlanandan sonra |
| Fiziki sınaq partiyası | Brief bölmə 10 bənd 7 - tikmə, trafaret, oyma. Yalnız tərs kəsik |
| CMYK / Pantone press proof | `US-COLLATERAL-SPEC.md` 9c - ekrandan çıxarılan Pantone nömrəsi təxmindir |

---

## 7.1 Canlı sayt yoxlandı

`https://testlogo-site.vercel.app/` brauzerdə ölçüldü. Üç fərq tapıldı, biri
risk daşıyır. Tam sənəd: `build/03-SITE-NOTES.md`.

| Nə | Sayt | Sistem |
|---|---|---|
| Nişan | **lələk**, yuvarlaq plitə `rx=80` | üç lövhəli `K`, radius `0` |
| Aksent | `#0B7A67`, hue **170** | `#1187A5`, hue **192** |
| Registr | qarışıq | tam BÖYÜK |

**Lələk risk daşıyır:** `US-TRADEMARK-SCAN.md` bölmə 8 bənd 11 tanınan yırtıcı
quş formasını qadağan edir, şirkət isə məhz IT təhlükəsizliyi satır.

**Aksent toqquşması ölçüldü:** saytın yaşılı semantik `success-600`-dən (hue
149) cəmi `20` dərəcə uzaqdır; `petrol-500` `43` dərəcə uzaqdır. Dashboard
satan şirkətdə brend rəngi status rəngi ilə qarışmamalıdır
(`DESIGN-LANGUAGE.md` 2.11 bənd 2). **Petrol qalır.**

Uyğun gələn: **sayt onsuz da Archivo işlədir**, neytral pilləkən demək olar
eynidir. Hazır əvəz faylları `delivery/08-site/`-dədir və Next.js App Router
adlandırması ilə birbaşa yerinə düşür.

---

## 8. Alət modulları

| Modul | Nə edir | Öz-sınağı |
|---|---|---|
| `tools/build/mark.py` | həndəsəni parametrlərdən törədir, SVG və raster verir | eskizlə təpə-təpə |
| `tools/build/wordmark.py` | shaper, cüt düzəlişləri, tracking, konturlar | brief 5.7 cədvəli |
| `tools/build/lockup.py` | bölmə 6 kompozisiya qaydaları | brief 6.1/6.2/6.3 |
| `tools/build/raster.py` | nişan + sözmarka rasteri, parlaqlıq ölçüsü | brief bölmə 7 |
| `tools/build/vector.py` | saf vektor PDF | şrift/şəkil yoxluğu |
| `tools/build/package.py` | təhvil paketi + manifest | ICO kadr sayı |
| `tools/build/collateral.py` | spesifikasiya cədvəlini oxuyur və yoxlayır | 38 sətir |
| `tools/build/templates.py` | zona əsaslı kolleteral şablonları + bələdçi qatı | kanvas ölçüsü spesifikasiya ilə |
| `tools/verify_mark.py` | eskiz SVG-lərini briefə qarşı yoxlayır | bölmə 4.5 |

Hər modul birbaşa işə salınanda öz sınağını verir:

```powershell
python tools/build/mark.py
python tools/build/wordmark.py
python tools/build/lockup.py
python tools/build/raster.py
python tools/build/vector.py
python tools/build/collateral.py
python tools/build/templates.py
python tools/verify_mark.py
python tools/build/package.py --clean
```
