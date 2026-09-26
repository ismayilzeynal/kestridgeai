# 09-linkedin-banner - b4-oversize-monogram, yeni nişanla yenidən qurulub

Tarix: 2026-09-21 (v2), 2026-09-26 (v3 yeniləməsi - bölmə 0)
Vahid: L2-DES / L3-DES-GRPH
Göndəriş faylları: `delivery/09-linkedin-banner/`
Sübut və ölçmə: `qa/09-linkedin-banner/`
Generator: `tools/build/banner.py`, `tools/build/render_banner.py`

v2-də hamısı bir qovluqda idi: əvvəl `v2-ai/delivery/09-banner/`, göndərişdə
`delivery/09-banner/` (arxiv surəti: `archive/v2-2026-09-21/delivery/09-banner/`).

Bu, sahibin seçdiyi `b4-oversize-monogram` konsepsiyasının **yeni beş lövhəli
nişanla** və **yeni `M/C = 2/1` kilidi ilə** yenidən qurulmuş halıdır.
Kompozisiya dəyişməyib: solda mürəkkəb sahə, üstündə kilid, sağdan 45 dərəcəlik
tikişlə girən petrol paz, sağ kənarla kəsilən nəhəng monoqram.

v2 qurğusu zamanı `v2-ai/banner/b4-oversize-monogram/` (köhnə, üç lövhəli
render), `delivery/` və `tools/` **oxunub, dəyişdirilməyib**. `v2-ai/` qovluğu
v3-də silinib; mətn, kod, JSON və SVG faylları `b6f3328`-dən əvvəlki commit-dən (`79b9987`) bərpa olunur, PNG renderlər (bu köhnə render daxil) `.gitignore`-dakı `*.png` qaydasına görə git-də heç vaxt olmayıb və bərpa olunmur.

> **2026-09-21 auditdən sonrakı düzəliş.** İki qüsur bağlandı:
> deskriptor sətri artıq bannerdə **yazılmır** (bölmə 7.1 - ölçülmüş səbəb),
> `alt-texture` faylı isə **silindi** (bölmə 4.2 - həndəsi səbəb). Hər iki
> qərarın rəqəmləri `measurements.json`-dadır və aşağıda təkrarlanır: heç bir
> iddia silinməyib, səhv rəqəmlər düzəldilib.

---

## 0. v3 yeniləməsi (2026-09-26)

**Kompozisiya dəyişməyib.** Sahə, paz, tikiş, monoqram, aksent tiri, kilidin
yeri və ölçüsü v2-dəki kimidir. Dəyişən iki şeydir: kilidin rəngi və
faylların yeri.

### 0.1 Kilid indi `color-knockout`-dur

v3-də nişan iki rəngdə göndərilir: `K` mürəkkəb, `Ai` petrol
(`02-BRAND-GUIDELINES.md`, rəng bölməsi). Tünd sahədə yalnız `color-knockout`
işlənir. Mənbə: `tools/build/mark.py` -> `COLOUR_KNOCKOUT`, bannerdə
`tools/build/render_banner.py` -> `lk_tone=MARK.COLOUR_KNOCKOUT`.

| Hissə | v2 | v3 |
|---|---|---|
| `stem` | `#FAFAF7` | `#FAFAF7` kağız |
| `arm`, `tittle`, `leg`, `crossbar` | `#FAFAF7` | **`#2FA8C7` petrol-400** |
| sözmarka | `#FAFAF7` | `#FAFAF7` |
| deskriptor | yazılmır | yazılmır (bölmə 0.3) |

Kontrast (ilk iki sətir `qa/09-linkedin-banner/measurements.json` ->
`renders[].lockup`; üçüncü sətir `tools/build/mark.py`-da `COLOUR_KNOCKOUT`-un
yanındakı ölçü: kağız / mürəkkəb `17.84 : 1`):

| Ölçmə | örtük | profil | Qapı 4.5 |
|---|---|---|---|
| kilidin arxasındakı ton | `#0F1317`, `100%` | eyni | təmiz |
| **ən zəif ink: petrol-400 `#2FA8C7` / `#0F1317`** | **`6.70 : 1`** | **`6.70 : 1`** | keçildi |
| `stem` və sözmarka: `#FAFAF7` / `#0F1317` | `17.84 : 1` | `17.84 : 1` | keçildi |

v2-də kilidin tək inki kağız idi, ona görə `min_contrast` `17.84 : 1` idi.
İndi kilid ən zəif inki qədər oxunur: `tools/build/banner.py` -> `contrast()`
rəng yolunun hər inkini ayrıca ölçür (deskriptoru çıxmaqla) və ən kiçiyini
götürür. Nəticə `6.70 : 1`-dir, qapı `4.5 : 1`, keçir.

**Renderdə yoxlanıldı** (2026-09-26, PIL `ImageChops.difference`, v3 faylı
ilə `archive/v2-2026-09-21/delivery/09-banner/`-dakı eyni adlı fayl arasında).
Fərqli piksellər yalnız bir düzbucaqlıdadır:

| | fərq qutusu (`getbbox`) | kilidin ink qutusu |
|---|---|---|
| örtük | `436, 86 .. 496, 170` | `412, 86 .. 928, 170` |
| profil | `449, 148 .. 521, 248` | `420, 148 .. 1035, 248` |

Bu qutu nişanın `stem`-dən sağdakı hissəsidir, yəni dörd petrol lövhəsi.
Sözmarka, `stem`, sahə, paz, tikiş, monoqram və aksent tiri piksel-piksel
eynidir - kompozisiyanın dəyişmədiyinin ölçülmüş sübutu. Tam `#2FA8C7`
tonunda piksel sayı: örtükdə `1360`, profildə `1965`.

`measurements.json` da eyni şeyi deyir. v2 arxivindəki fayl ilə müqayisədə
(`archive/v2-2026-09-21/delivery/09-banner/measurements.json`) yalnız
`lockup.generator` yolu, `renders[].lockup` altında `tones_behind[].contrast`
ilə `min_contrast` (`17.84` -> `6.7`) və `lockup.file`, `renders[].proofs`,
`removed[].proofs_removed` adlarındakı `_` prefiksi dəyişib; prefiksi
`pack.py` -> `translate()` atır (bölmə 0.2). Monoqram, kəsimlər, qoruq,
kiçik ölçü döşəmələri, deskriptor və sahə pillələri açar-açar eynidir.

Monoqram fakturası dəyişməyib: lövhədə petrol-700 `#0B5568`, sahədə
petrol-900 `#002C38`. Faktura istisnası v3-də də qüvvədədir.

### 0.2 Faylların yeri

| v2 | v3 |
|---|---|
| `09-banner/kestridge-banner-linkedin-*.png` | `delivery/09-linkedin-banner/` (adlar eyni) |
| `09-banner/_check-*.png` | `qa/09-linkedin-banner/check-*.png` (`_` prefiksi atılıb) |
| `09-banner/_lockup-horizontal-knockout-v2.svg` | `qa/09-linkedin-banner/lockup-horizontal-color-knockout.svg` (v3-də adı rəng yolunu göstərir) |
| `09-banner/measurements.json` | `qa/09-linkedin-banner/measurements.json` |
| `09-banner/NOTES.md` | `delivery/00-docs/05-BANNER-NOTES.md` (bu sənəd) |
| `v2-ai/build/banner.py`, `render_banner.py` | `tools/build/banner.py`, `tools/build/render_banner.py` |
| `v2-ai/build/lockupv2.py` | `tools/build/lockupv2.py` |
| `v2-ai/build/markv2.py` | silinib; generator birdir, `tools/build/mark.py` |

`measurements.json`-un içindəki fayl adları (`lockup.file`, `renders[].proofs`,
`removed[].proofs_removed`) mərhələdə `_` prefiksi ilə yazılır.
`tools/build/pack.py` faylları `route()` ilə prefikssiz adla `qa/`-ya köçürür,
mətn qeydlərinin içindəki adları isə `copy_out()` -> `translate()` ilə eyni
qaydada düzəldir (commit `9a9dabc`). Diskdəki
`qa/09-linkedin-banner/measurements.json` artıq o commit-dən sonrakı düzülüşdəndir:
içindəki adlar prefikssizdir, `lockup.file` və `renders[].proofs` adları
`qa/09-linkedin-banner/`-dakı fayllarla üst-üstə düşür.

### 0.3 Deskriptor qərarı dəyişmir

Sətir yenə **yazılmır**. Rəqəmlər bölmə 7.1-dədir və v3 renderində eynidir:
faylda cap `12.6 px`-dir, amma LinkedIn örtüyü `3.2 : 1` kəsimində verir və
ekrana kiçildir; `390 px` telefonda cap `6.0 px` olur, brendin döşəməsi isə
`9 px`-dir (`02-BRAND-GUIDELINES.md` 5.4). Döşəməyə qaldırmaq `x1.500`
böyümə istəyir və sətri `159.6 px` 45 dərəcəlik tikişin üstünə aparır.

Rəng dəyişməsi bu qərara toxunmur: sətir həndəsə səbəbi ilə yazılmır, rəng
səbəbi ilə yox. Yazılsaydı rəngi `tools/build/render_banner.py` ->
`desc_tone=T["petrol-400"]` = `#2FA8C7` olardı (v2-dəki kimi). Banner
deskriptoru kilidlə yox, `tools/build/banner.py` -> `descriptor_png()` ilə
ayrıca çəkir; `COLOUR_KNOCKOUT` -> `descriptor` da eyni tondur.

**Tövsiyə:** strapline LinkedIn səhifəsinin öz tagline sahəsinə yazılsın. O,
canlı mətndir və hər ekranda oxunur.

---

## 1. Fayllar

Göndəriş faylları `delivery/09-linkedin-banner/`-dadır, sübut və ölçmə
`qa/09-linkedin-banner/`-dadır. Bu sənəddə `measurements.json` deyiləndə
`qa/09-linkedin-banner/measurements.json` nəzərdə tutulur.

| Fayl | Yer | Nədir |
|---|---|---|
| `kestridge-banner-linkedin-cover-1512x256.png` | `delivery/09-linkedin-banner/` | LinkedIn şirkət səhifəsi örtüyü. Ölçü mənbədə təsdiqlidir (`tools/data/platform-dimensions.json`) |
| `kestridge-banner-linkedin-profile-background-1584x396.png` | `delivery/09-linkedin-banner/` | LinkedIn şəxsi profil fonu. Bu ölçü təsdiqli cədvəldə **yoxdur**, `unverified` işarəlidir |
| `check-logo-overlay-*.png` | `qa/09-linkedin-banner/` | LinkedIn dairəvi loqo maketi + qoruq düzbucaqlısı |
| `check-crop-*.png` | `qa/09-linkedin-banner/` | Dar ekran kəsimləri işarələnmiş nüsxə |
| `check-small-*.png` | `qa/09-linkedin-banner/` | 1/3 miqyas, mobil oxunaqlılıq üçün |
| `lockup-horizontal-color-knockout.svg` | `qa/09-linkedin-banner/` | Bannerlərin işlətdiyi kilid, olduğu kimi. 5 nişan lövhəsi + 1 sözmarka yolu. v3-də `color-knockout`: `stem` və sözmarka `#FAFAF7`, dörd lövhə `#2FA8C7` |
| `measurements.json` | `qa/09-linkedin-banner/` | Aşağıdakı bütün rəqəmlərin maşın oxunan mənbəyi |

v2-də bu yeddi növün hamısı bir qovluqda idi (`09-banner/`), sübutlar `_`
prefiksi ilə - bölmə 0.2.

**İki göndəriş faylı var, üç yox.** `kestridge-banner-linkedin-cover-1512x256-alt-texture.png`
və onun üç sübutu silindi. Səbəb və arifmetika bölmə 4.2-də, həmçinin
`measurements.json` -> `removed` açarında.

Generatorlar `tools/build/` altındadır (v2-də `v2-ai/build/` idi). Paket bir
giriş nöqtəsindən qurulur:

```powershell
python C:\Users\Asus\HQ\projects\kestridge-ai-brand\tools\build\pack.py
```

`render_banner.py` doqquz qurucudan altıncısıdır. O, yalnız mərhələyə yazır
(`%TEMP%\kestridge-ai-brand-stage`, `KESTRIDGE_STAGE` ilə dəyişdirilir);
`delivery/09-linkedin-banner/` və `qa/09-linkedin-banner/`-ya faylları yalnız
`pack.py` köçürür. `render_banner.py` tək işlədilibsə, `pack.py --no-build`
son mərhələdən düzür.

| Generator | Nədir |
|---|---|
| `mark.py` | Nişan və v3 rəng yolları (`COLOUR`, `COLOUR_KNOCKOUT`). v2-də bannerin nişanı ayrıca `markv2.py`-dan gəlirdi; v3-də o silindi, generator birdir |
| `lockupv2.py` | `tools/build/lockup.py`-i idxal edir, `MARK` və `CAP_H`-i yenidən bağlayır. Mühərrik yenidən yazılmayıb |
| `banner.py` | Kompozisiya mühərriki. **Yeni:** `descriptor_gate()` və `texture_gate()` |
| `render_banner.py` | Sürücü: hər şeyi yenidən qurur və `measurements.json` yazır |

---

## 2. Nə dəyişdi

Bu cədvəl v1 -> v2 keçididir. v2 -> v3 keçidi bölmə 0-dadır: nişan
generatoru birləşdi və kilid `color-knockout` oldu, kompozisiya dəyişmədi.

| Qat | Əvvəl (v1 render) | İndi (v2) |
|---|---|---|
| Nişan | `tools/build/mark.py`, 3 lövhə | `markv2.py`, 5 lövhə (`stem, arm, tittle, leg, crossbar`). v3-də `markv2.py` silinib, 5 lövhəli nişan `tools/build/mark.py`-dadır |
| Kilidin rəngi | `#FAFAF7`, bir ink | `#FAFAF7`, bir ink. v3-də əvəz olunub (2026-09-26): `color-knockout` - bölmə 0.1 |
| Kilid | göndərilmiş SVG, `M/C = 4/3` | `lockupv2.py`, `M/C = 2/1`, canlı çəkilir |
| Deskriptor | `AI · AUTOMATION · SECURITY · DATA`, Inter, PIL ilə əl izləməsi | **bannerdə yazılmır** - bölmə 7.1 |
| Monoqram kadrlaması | `mono_cx` / `mono_cy` kətan payları | nişanın öz koordinatları (`mono_right_u`, `mono_top_u`) |

Deskriptor mühərriki yerindədir (`descriptor=True` bir açardır) və
`lockupv2.descriptor_svg` ilə hər an çəkilə bilər. Bannerdə açılmır, çünki
brendin öz qaydası bu ölçüdə onu bağlayır - bölmə 7.1.

---

## 3. Kilid: `M/C = 2/1` və mətn sütununun `2/3`-ə kiçilməsi

`CAP_H` `13.5u`-dan `9.0u`-ya düşür, yəni **tam `2/3`**. Sözmarkanın içindəki
hər uzunluq cap-a mütənasib olduğu üçün deskriptor da eyni əmsalla gedir:

| Kəmiyyət | v1 | v2 | Qapanış |
|---|---|---|---|
| sözmarka cap | `5c = 13.5u` | `10c/3 = 9.0u` | `M/C = 2/1` |
| aralıq | `1.5c = 4.05u` | dəyişmir | `GAP / cap = 0.45` = sözmarkanın öz söz boşluğu |
| deskriptor cap | `1.5c = 4.05u` | `c = 2.70u` | `1.5c x 2/3 = c` |
| deskriptor enişi | `3.5c = 9.45u` | `7c/3 = 6.30u` | |
| kilid eni | `154.90u` | `110.616u` | AR `8.61` -> `6.145` |

`python tools/build/lockupv2.py` bu qapanışların hamısını ölçür və `PASS` verir.

v3 qeydi: v2 promosiyasından sonra bu selftest 6 yerdə uğursuz idi.
`lockupv2.py` deskriptoru iki dəfə kiçildirdi (`DESC_CAP = 2/3 x
lockup.DESC_CAP`, halbuki `lockup.py` artıq kiçilmiş `2.7u`-nu saxlayırdı, nəticə
`1.8u`). İndi `DESC_CAP = c = 2.7u` və `DESC_DROP = 7c/3 = 6.3u` birbaşa `c`-dən
yazılır, selftest sıfır FAIL verir. Bannerə təsiri olmayıb: sətir bannerdə
yazılmır və heç bir göndərilmiş fayl bu xətanı daşımayıb.

Bannerdə görünən nəticə:

| | örtük `1512 x 256` | profil `1584 x 396` |
|---|---|---|
| nişan ink hündürlüyü | `84 px` | `100 px` |
| sözmarka cap | `42 px` | `50 px` |
| kilid ink eni | `516.2 px` | `614.5 px` |
| kilid ink qutusu | `412, 86 .. 928, 170` | `420, 148 .. 1035, 248` |
| deskriptor | **yazılmır** (saxlansaydı cap `12.6 px`) | **yazılmır** (saxlansaydı cap `15.0 px`) |

Köhnə renderdə nişan örtükdə `64 px`, sözmarka cap `48 px` idi. Yəni nişan
**31 faiz böyüyüb**, sözmarka `12` faiz kiçilib - sahibin istədiyi istiqamət.

Aksent tiri əl ilə qoyulmur: kilidin öz aralığı (`1.5c`) qədər sola,
eni bir faska (`k`), hündürlüyü nişanın cap zolağı qədər.

### 3.1 Blokun şaquli yeri - sətir gedəndən sonra

Deskriptor blokun bir hissəsi idi, ona görə onun getməsi bloku qısaldır.
Ölçüldü və düzəldildi:

| | örtük | profil |
|---|---|---|
| blok hündürlüyü, əvvəl | `92.6 px` | `110.3 px` |
| blok hündürlüyü, indi | `84.0 px` | `100.0 px` |
| `block_cy`, əvvəl | `128` = `H/2` | `168` |
| `block_cy`, indi | `128` - **dəyişmir** | **`198` = `H/2`** |
| yuxarıdan / aşağıdan hava | `86 / 86` | `118 / 178` -> **`148 / 148`** |
| sözmarkanın sağ ucundan tikişə | `47.4 px` -> **`72.8 px`** | `42.2 px` -> **`42.5 px`** |

Örtükdə `block_cy` onsuz da `H/2` idi, ona görə sətir gedəndə kilid öz-özünə
mərkəzə oturdu və tikişə qədər `25 px` daha çox hava qazandı. Profildə `168`
**uzun blokun** mərkəzi idi; tək kilid orada `118 / 178` ilə yuxarıda qalırdı.
`198`-ə keçid havanı simmetrik edir və tikişə qədər olan aralığı göndərilmiş
faylın onsuz da daşıdığı rəqəmə qaytarır (`42.5` yerinə `42.2`). Bu, bir
sabitdir və geri qaytarıla bilər.

### 3.2 Kilidi böyütmək olardımı? Yox - eni bağlayır

Sətir gedəndən sonra bloku daha ağır etmək üçün kilidi böyütmək təbii fikirdir.
Ölçüldü: mümkün deyil. İki şərt eyni anda saxlanmalıdır - aksent tiri ən dar
kəsim pəncərəsindən indiki qədər aralı qalsın, sözmarkanın sağ ucu tikişdən
indiki qədər aralı qalsın. Kilid `f` dəfə böyüyəndə hər iki ehtiyat eyni anda
yeyilir:

```
örtük:   580.25 f <= 585.1   ->  f <= 1.008
profil:  690.75 f <= 695.8   ->  f <= 1.007
```

Yəni yer `1 faizdən azdır`. Kilid göndərilmiş ölçüsündə qalır.

---

## 4. Monoqram

### 4.1 `2.0x` niyə `1.0x` oldu

**Bu, sənədləşdirilmiş bir dəyişiklikdir və səbəbi həndəsidir.**

Yeni nişanın iki kiçik lövhəsi ink qutusunun **diaqonal əks künclərindədir**:

| Lövhə | Mövqe (nişan vahidləri) |
|---|---|
| `tittle` (nöqtə) | `x 16.275 .. 18.300`, `y 3.000 .. 5.025` - yuxarı sağ |
| `crossbar` (tir) | `x 8.350 .. 12.066`, `y 16.950 .. 18.975` - aşağı sol |

Şaquli olaraq onların arası `18.975 - 3.000 = 15.975u`-dur, yəni `18u`-luq ink
qutusunun `89` faizi. Hər ikisini kadrda saxlayan istənilən pəncərə ən azı
`15.975u` hündür olmalıdır. Kətanın hündürlüyü `H` sabit olduğuna görə bu,
birbaşa miqyasa tavan qoyur:

```
mono_h <= H * 18 / 15.975  =  1.127 * H
```

Seçim:

> Monoqramın ink qutusunun hündürlüyü **dəqiq kətanın hündürlüyüdür**
> (`mono_h = 1.0`), ona görə banner bir vahid daşıyır: `u = H / 18`.
> Fiqur bir vahid **aşağı sürüşdürülür**, beləcə nöqtə yuxarı kənardan `1u`
> aralı, tam kvadrat kimi oxunur; ayaqlar aşağıdan eyni `1u` qədər çıxır.
> Sağdan kəsim `2k = 1.35u`-dur və bu, nöqtənin sağındakı hava ilə eynidir -
> yəni nöqtəni qoldan ayıran həmin örtük aralığı.

İlk qurğuda fiqur yuxarı kənarla **flush** idi (`mono_top_u = 3.0`). O halda
nöqtənin yuxarı üzü kənarın üstündə oturur və nöqtə kimi yox, kənardan sallanan
**dil** kimi oxunurdu. Bir vahid sürüşmə bunu həll etdi.

"Oversize" iddiası itmir, yalnız hansı şeyə nisbətən ölçüldüyü dəyişir:

| | örtük | profil |
|---|---|---|
| monoqram ink hündürlüyü | `256.0 px` | `396.0 px` |
| kətanın hündürlüyünə nisbət | `1.00x` | `1.00x` |
| **kilidin nişanına nisbət** | **`3.05x`** | **`3.96x`** |
| sol kənarı | `x = 1275.2` | `x = 1217.7` |
| sağdan kəsilir | `19.2 px` | `29.7 px` |
| aşağıdan çıxır | `14.2 px` | `22.0 px` |
| kiliddən aralığı | `347.2 px` | `182.7 px` |

Lövhə-lövhə nəticə (hər ikisində eyni):

| Lövhə | Görünən sahə |
|---|---|
| `tittle` | `1.000` - tam içəridə |
| `crossbar` | `1.000` - tam içəridə |
| `arm` | `1.000` |
| `stem` | `0.944` - aşağıdan bir vahid çıxır |
| `leg` | `0.860` - sağ ayağı kəsilir |

### 4.2 `alt-texture` faylı niyə silindi

Fayl: `kestridge-banner-linkedin-cover-1512x256-alt-texture.png`.
Eyni örtük, monoqram `2.0x` kadrlaması ilə.

Ölçmə onu belə yazmışdı:

| Lövhə | `visible_area_share` |
|---|---|
| `arm` | **`0.000`** - tamamilə kadrdan kənarda |
| `tittle` | **`0.000`** - tamamilə kadrdan kənarda |
| `stem` | `0.500` |
| `leg` | `0.857` |
| `crossbar` | `1.000` |

Fayla baxıldı: nişan kimi oxunmur. Bir mücərrəd çəp xətt və yetim bir trapesiya
görünür. `2.0x`-də kadrda cəmi `18 / 2.0 = 9.0u` var, iki örtük lövhəsinin
arası isə `15.975u`-dur - **heç bir kadrlama ikisini birdən tuta bilmir.**
Tutan hər kadrlama `1.127x` və aşağısındadır, orada isə render əsas örtüyün
özüdür. Yəni "yenidən kadrla" yolu fayla heç nə qazandırmır.

Üstəlik fayl `_` prefiksi olmadan, iki əsl göndəriş faylının yanında, üst
səviyyədə dayanırdı - yəni üçüncü göndəriş kimi oxunurdu.

Ona görə fayl və onun üç sübutu (`_check-logo-overlay-`, `_check-crop-`,
`_check-small-linkedin-cover-alt-texture-1512x256.png`) silindi. Silən
arifmetika `measurements.json` -> `removed` açarında qalır.

### 4.3 Faktura: nöqtə və tir qəsdən oxunur, ləkə deyil

Sual budur: monoqram faktura ölçüsündə işlədiləndə iki kiçik lövhə **qəsdən**
qoyulmuş kimi oxunur, yoxsa ləkə kimi? Döşəmə brendin özünündür: nişan `16 px`
ink hündürlüyünə qədər göndərilir, orada isə nöqtənin tərəfi və tirin qalınlığı
`MODULE / 18 x 16 = 1.8 px`-dir. Brendin oxunaqlı saydığı ən kiçik rəqəm budur.

| | örtük | profil |
|---|---|---|
| nöqtənin tərəfi | `28.8 px` | `44.6 px` |
| tirin qalınlığı | `28.8 px` | `44.6 px` |
| kətanın hündürlüyünün payı | `11.25%` | `11.25%` |
| `16 px` döşəməsindəki ekvivalent | `1.8 px` | `1.8 px` |
| **neçə dəfə yuxarı** | **`16.0x`** | **`24.8x`** |
| telefon `390 px`-də | `13.7 px` | `15.7 px` |
| `1/3` miqyasda | `9.6 px` | `14.9 px` |

Fayllar açılıb baxıldı (`2x` böyüdülmüş kəsimlər): hər iki renderdə beş lövhənin
hamısı ayrı-ayrılıqda oxunur, nöqtə kvadrat kimi, tir çəp uclu tir kimi.
Miqyas və ya kadr dəyişdirilməsinə ehtiyac olmadı.

Bir dürüst qeyd: **örtükdə faktura yalnız masaüstündədir.** Monoqramın ink
sahəsi `x 1275.2 .. 1531.2`-dədir, dar kəsim pəncərələri isə `244 .. 1268` və
`346 .. 1165`. Yəni telefonda örtükdən monoqram **sıfır piksel** görünür.
Profildə belə deyil: `3.4:1`-də `247.5 px`, `2.8:1`-də `128.7 px` görünür.

---

## 5. Ölçülmüş kontrast

Rəqəmlər göz ilə yox, **renderin üzərindən** çıxarılıb: eyni banner kilidsiz
ikinci dəfə render olunur, sonra ink qutusunun altındakı **hər fərqli ton**
sayılır və ən pisi götürülür.

| Ölçmə | örtük | profil | Qapı 4.5 |
|---|---|---|---|
| kilid `#FAFAF7` / arxadakı ton (v2, tək ink) | `17.84 : 1` | `17.84 : 1` | keçildi. v3-də əvəz olunub (2026-09-26) - növbəti sətir, bölmə 0.1 |
| **kilid, v3 `color-knockout`: ən zəif ink petrol-400 `#2FA8C7` / arxadakı ton** | **`6.70 : 1`** | **`6.70 : 1`** | keçildi |
| kilidin arxasında neçə ton var | 1 ton: `#0F1317`, `100%` | eyni | təmiz |
| deskriptor | yazılmır | yazılmır | tətbiq olunmur |

v3-də `stem` və sözmarka yenə `#FAFAF7`-dir və sahə üzərində `17.84 : 1`
verir; kilidin qiyməti isə ən zəif inkidir, yəni `6.70 : 1`
(`measurements.json` -> `renders[].lockup.min_contrast`).

> **v3-də əvəz olunub (2026-09-26) - bölmə 0.1.** Aşağıdakı abzas v2 halını
> təsvir edir. v3-də petrol-400 `#2FA8C7` bannerə kilidin `Ai` lövhələri ilə
> qayıdıb (`arm`, `tittle`, `leg`, `crossbar`), ona görə aksent tirini
> petrol-500-ə qaldırmaq təklifinə ehtiyac qalmayıb. Aksent tiri petrol-600,
> tikiş saç xətti petrol-500 olaraq qalır.

Deskriptor getdiyi üçün **petrol-400 `#2FA8C7` artıq bannerdə heç yerdə
yoxdur**. Bu, sətrin getməsinin birbaşa nəticəsidir. Kompensasiya üçün başqa
elementin rəngi **dəyişdirilmədi** - aksent tiri petrol-600, tikiş saç xətti
petrol-500 olaraq qalır, çünki rəng dəyişməsi sahibin təsdiqlədiyi
kompozisiyaya aid deyil. Sahib parlaq tonun qayıtmasını istəsə, ən ucuz yol
aksent tirini petrol-500-ə qaldırmaqdır (sahə üzərində `4.47 : 1`).

### Sahə pillələri (mətn deyil, `4.5` qapısı tətbiq olunmur)

| Cüt | Nisbət |
|---|---|
| lövhə `#004051` / sahə `#0F1317` | `1.645` |
| faska `#0E6A82` / lövhə | `1.837` |
| faska / sahə | `3.022` |
| tikiş xətti `#1187A5` / faska | `1.479` |
| tikiş xətti / sahə | `4.470` |
| monoqram `#0B5568` / lövhə | **`1.357`** |
| monoqram `#0B5568` / faska | `1.354` |
| monoqram `#002C38` / sahə | **`1.259`** |
| aksent tiri `#0E6A82` / sahə | `3.022` |

Monoqramın iki nisbəti (`1.357` və `1.259`) qəsdən yaxındır: fiqur tikişi
keçəndə ağırlığını dəyişmir. Qeyd: örtükdə monoqram tikişi **keçmir** (sol
kənarı `1275.2`, tikiş orada `1150`-dən soldadır), ona görə `#002C38` sətri
örtükdə mövcud olmayan sahəni təsvir edir; profildə keçir və orada real ölçüdür.

---

## 6. Sənətkarlıq qapıları

**Aşağı sol qoruq.** Örtükdə `300 x 256`, profil fonunda `348 x 396` qutusu
ölçüldü: hər ikisində **cəmi bir ton var**, `#0F1317`, sahənin `100%`-i. İlk
essensial element (aksent tiri) örtükdə `x = 390`-da, profil fonunda
`x = 394`-də başlayır, yəni qoruqdan `90` və `46` px sağda. Sübut:
`qa/09-linkedin-banner/check-logo-overlay-*.png`.

**Dar ekran kəsimi.** Essensiallar indi ikidir: kilid və aksent tiri.
Sübut: `qa/09-linkedin-banner/check-crop-*.png`.

| Ölçü | Kəsim | Görünən en | Pəncərə | Hər şey içəridə? | Ən pis kənar |
|---|---|---|---|---|---|
| `1512 x 256` | `4.0 : 1` | `1024 px` | `244 .. 1268` | bəli | `146.0 px` (aksent tiri) |
| `1512 x 256` | `3.2 : 1` | `819 px` | `346 .. 1165` | bəli | **`43.6 px`** (aksent tiri) |
| `1584 x 396` | `3.4 : 1` | `1346 px` | `118 .. 1465` | bəli | `275.2 px` |
| `1584 x 396` | `2.8 : 1` | `1108 px` | `237 .. 1346` | bəli | `156.4 px` |

Kəsim nisbətləri **təxmindir**, mənbədə təsdiqli deyil. Ona görə hər ölçüdə iki
fərqli nisbət yoxlanılır.

**Kiçik ölçü döşəmələri - indi hamısı keçir.**

| Vəziyyət | Miqyas | nişan (döşəmə `16`) | sözmarka cap (döşəmə `10`) | deskriptor (döşəmə `9`) |
|---|---|---|---|---|
| örtük, telefon `390 px` | `0.476` | `40.0 px` keçir | `20.0 px` keçir | yazılmır |
| örtük, `1/3` miqyas | `0.333` | `28.0 px` keçir | `14.0 px` keçir | yazılmır |
| profil, telefon `390 px` | `0.352` | `35.2 px` keçir | `17.6 px` keçir | yazılmır |
| profil, `1/3` miqyas | `0.333` | `33.3 px` keçir | `16.7 px` keçir | yazılmır |

`measurements.json` -> `small_viewport` -> `all_floors_ok` hər dörd sətirdə
`true`-dur.

**Nişanın üstündə effekt yoxdur.** Qradiyent yox, kölgə yox, parıltı yox,
AI ulduzcuğu yox, vinyet yox - sahə tam yastıdır
(`measurements.json` -> `"vignette": "none"`). Nişan güzgülənməyib.

---

## 7. Düz çıxmayan, açıq yazılır

### 7.1 Deskriptor sətri bannerdə yazılmır - qüsur belə bağlandı

**Əvvəlki paket qüsurlu idi və bunu öz ölçməsində yazırdı.** Hər üç banner
`descriptor_ok: false` ilə göndərilmişdi: örtükdə telefonda `6.0 px`, `1/3`
miqyasda `4.2 px`, profildə `5.3 px` cap - hamısı `9 px` döşəməsindən aşağı.
Rəqəm rastra bişirildiyi üçün miqyasla geri qaytarıla bilməzdi.

**Səbəb:** döşəmə **fayl ölçüsündə** yoxlanılırdı. Faylda cap `12.6` və
`15.0 px`-dir, yəni keçir. Amma banner heç vaxt fayl ölçüsündə görünmür -
LinkedIn onu kəsir, telefon qalanını kiçildir. Döşəmə **göndərilən ölçüdə**
yoxlanmalıdır. `banner.py` -> `descriptor_gate()` indi məhz bunu edir.

Brendin öz qaydası bunu onsuz da deyir. `02-BRAND-GUIDELINES.md` 5.4,
"İşlənmir" siyahısı: *deskriptor cap `9 px`-dən kiçik olanda*. `C_d = 2.7u`
olduğuna görə bu, kilidin ink eni haqqında ifadədir: `9 / 2.7 = 3.3333 px/u`,
yəni **`368.7 px` kilid ink eni**. Göndərilən ölçüdə ölçüldü:

| | kilid ink eni, faylda | telefon `390 px`-də | `1/3` miqyasda | döşəmə |
|---|---|---|---|---|
| örtük | `516.2 px` | **`245.8 px`** | **`172.1 px`** | `368.7 px` |
| profil | `614.5 px` | **`216.2 px`** | **`204.8 px`** | `368.7 px` |

Dörd haldan dördü döşəmədən aşağıdır. Qayda bu halda sətrin yazılmamasını
tələb edir, ona görə **sətir yazılmır**. Bu, (b) yoludur.

**(a) yolu niyə rədd edildi - ölçü ilə, zövqlə yox.** Sətri döşəməyə qaldırmaq
üçün nə qədər böyüməlidir və bunun qiyməti nədir:

| | örtük, telefon | örtük, `1/3` | profil, telefon | profil, `1/3` |
|---|---|---|---|---|
| lazım olan cap | `18.9 px` | **`27.0 px`** | `25.6 px` | **`27.0 px`** |
| böyümə | `x1.500` | **`x2.143`** | `x1.706` | `x1.800` |
| sətrin ink eni | `620.6 px` | `886.4 px` | `840.0 px` | `886.4 px` |
| sətrin sağ ucu | `x 1135.5` | `x 1401.3` | `x 1382.5` | `x 1428.9` |
| tikiş həmin baza xəttində | `x 975.9` | `x 975.9` | `x 1047.1` | `x 1047.1` |
| **tikişi neçə px keçir** | **`159.6`** | **`425.4`** | **`335.4`** | **`381.8`** |
| cap / sözmarka cap | `0.450` | **`0.643`** | `0.512` | `0.540` |

Göndərilmiş nisbət `0.300`-dür. Yəni ən yumşaq halda belə (`x1.500`) sətir
`159.6 px` tikişi keçib nəhəng monoqramın üstünə düşür - sahibin təsdiqlədiyi
kompozisiya (solda mürəkkəb sahə, sağda paz, sağ kənarla kəsilən monoqram)
məhz orada ölür. Ən pis halda sətrin cap-ı sözmarka cap-ının `0.643`-nə çatır,
yəni deskriptor **ikinci sözmarkaya** çevrilir.

Eyni şey kilid tərəfdən də baxıla bilər: `1/3` miqyasda döşəməni keçmək üçün
faylda kilidin ink eni `1106.2 px` olmalıdır, örtüyün ən dar kəsim pəncərəsi
isə `819.2 px`-dir. Yəni **sadəcə sığmır**.

Buna görə (a) rədd edildi, (b) seçildi. `measurements.json` -> `descriptor`
açarı bu rəqəmlərin hamısını saxlayır: sətir getdi, sətri aparan ədədlər qaldı.

**Nə itdi.** Banner artıq `AUTOMATION SECURITY ANALYTICS` yazmır. Dəyər vədi
bannerdən oxunmur; LinkedIn-in öz tagline sahəsi və "Haqqında" bölməsi onu
daşımalıdır. Bu, ödənilən qiymətdir və gizlədilmir.

**v3 (2026-09-26):** qərar dəyişmir. v3 renderində yuxarıdakı bütün rəqəmlər
eynidir (`qa/09-linkedin-banner/measurements.json` -> `renders[].descriptor`),
çünki kilidin həndəsəsi dəyişməyib, yalnız rəngi dəyişib - bölmə 0.3.

### 7.2 Profil fonunda monoqram tikişi yuxarıda kəsir

`seam_x = 1300`-də monoqramın sol kənarı `1217.7`-dədir, ona görə üst `82 px`-də
gövdə mürəkkəb sahənin üstünə düşür və orada `1.259 : 1` ilə demək olar
görünmür; faska zolağı isə `154 px`-ə qədər gövdəni kəsir və orada monoqram
fondan **açıq yox, tünd** olur - polyarlıq çevrilir. Bu, konsepsiyanın öz
şərtidir ("fiqur tikişi keçir, hər tərəfdə öz tonunu alır") və v1 renderində də
belə idi. Örtükdə bu problem yoxdur: monoqram bütünlüklə lövhənin üstündədir.

Qeyd: `seam_x = 1300` əvvəl deskriptorun sağ ucuna görə seçilmişdi. Sətir
getdiyi üçün bu bağ qırıldı - `seam_x` indi daha sərbəst seçilə bilər. Bu
paketdə **dəyişdirilmədi**, çünki tikişin yeri sahibin gördüyü kompozisiyanın
bir hissəsidir. Açıq iş kimi bölmə 8-ə yazılıb.

### 7.3 Sol yarı hələ də boşdur

LinkedIn loqonu ora qoyduğu üçün məcburidir. Köhnə sənəddə də eyni qeyd var.

---

## 8. Açıq qalan iş (bu tapşırığın hüdudundan kənar)

Siyahı v2 zamanı (2026-09-21) yazılıb; hər maddənin altındakı status
2026-09-26 v3 yeniləməsində yoxlanılıb.

`lockupv2.py` yalnız **yatıq** kilidi `2/1`-ə keçirir və bunu `lockup.py`-a
toxunmadan edir. Qərarın tam tətbiqi üçün hələ lazımdır:

1. `tools/build/lockup.py` -> `CAP_H = 10 * C / 3` (sahib v1 paketini əvəz
   etməyə icazə verəndə).
   **Bağlandı** (v2 promosiyası): `tools/build/lockup.py` indi məhz
   `CAP_H = 10 * C / 3` saxlayır.
2. `build/02-BRAND-GUIDELINES.md` 6.1 və 6.2: yatıq kiliddə bağlayıcı döşəmə
   artıq nişan deyil, **sözmarka cap-ıdır** (ən kiçik kiliddə nişan `20 px`
   olur, `16` yox).
   **Bağlandı:** `delivery/00-docs/02-BRAND-GUIDELINES.md` 6.2 bunu yazır.
   `build/` qovluğu v3-də silinib; onun dörd `.md` faylı `delivery/`
   sənədlərinin bayt surətləri idi.
3. `delivery/01-master/` altındakı yatıq SVG-lər və onlardan törəyən rastr,
   PDF, platforma aktivləri - yeni nişanla birlikdə.
   **Bağlandı:** yatıq kilidlər indi `delivery/01-logo-svg/color/` və
   `delivery/01-logo-svg/mono/` altındadır (v2-də `delivery/01-master/` idi);
   onlardan törəyən rastrlar `delivery/02-logo-png/color/` və
   `delivery/02-logo-png/mono/`, PDF-lər `delivery/04-logo-pdf/color/` və
   `delivery/04-logo-pdf/mono/`, platforma aktivləri `delivery/07-platforms/`
   altındadır.
4. **Yeni:** profildə `seam_x = 1300` artıq deskriptora bağlı deyil. Pazın
   monoqramı harada kəsməsi yenidən seçilə bilər (bölmə 7.2).
   **Açıqdır:** `tools/build/render_banner.py`-da `seam_x=1300` dəyişməyib və
   yanındakı şərh hələ onu deskriptorun sağ ucuna bağlayır.
5. **Yeni:** `02-BRAND-GUIDELINES.md` 5.4-ə bir cümlə: `9 px` döşəməsi
   **göndərilən** ölçüdə yoxlanılır, fayl ölçüsündə yox. Qayda dəyişmir,
   yalnız harada tətbiq edildiyi aydınlaşır.
   **Açıqdır:** 2026-09-26 oxunuşunda 5.4-də bu cümlə yoxdur.
6. **Bağlandı (v3, 2026-09-26):** banner artefaktlarının adları rəngi
   göstərmirdi - kilid faylı `lockup-horizontal-knockout-v2.svg` adlanırdı, içi
   isə `color-knockout` idi, `measurements.json` -> `version` isə hələ `v2`
   yazırdı. `tools/build/render_banner.py`-da düzəldi: fayl indi
   `qa/09-linkedin-banner/lockup-horizontal-color-knockout.svg`-dir,
   `version` `v3 - five-plate mark (f2-strict-system), colour split, lockup
   color-knockout, M/C = 2/1` yazır, `lockup.colourway` isə rəng yolunun özünü
   saxlayır. `lockup.file` və
   `renders[].proofs`-dakı `_` prefiksi isə artıq `tools/build/pack.py` ->
   `translate()`-də həll olunub (commit `9a9dabc`); diskdəki
   `qa/09-linkedin-banner/measurements.json` artıq prefikssiz adları saxlayır (bölmə 0.2).

Stacked kilid (`M/C = 8/3`), tile, ehtiyat marka dəyişmir.
