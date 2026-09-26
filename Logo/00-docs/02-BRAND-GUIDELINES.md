# Kestridge - brend qaydaları

| | |
|---|---|
| Versiya | **v3** (v2-ni əvəz edir; v2 sənədi **2.0** idi) |
| Tarix | 2026-09-26 (v2: 2026-09-21) |
| Bazar | Amerika Birləşmiş Ştatları, brend dili İngilis |
| Nişan | **`KAi`**, namizəd `f2-strict-system`, **iki rəngdə** (bölmə 3.0). Generator `tools/build/mark.py`, paketi `tools/build/pack.py` qurur |
| Mənbələr | `archive/v2-2026-09-21/BRIEF.md`, `delivery/00-docs/01-DECISION-RECORD.md`, `tools/data/mark-plates-f2-strict-system.json` (dondurulmuş lövhə spesifikasiyası), `tools/build/mark.py` (`COLOUR`, `COLOUR_KNOCKOUT`), `qa/01-logo-svg-measurements.json`, `delivery/00-docs/05-BANNER-NOTES.md`, `qa/09-linkedin-banner/measurements.json`, `docs/concept/renders/_build/browsercheck.json` (v1 Chromium) |
| Fayllar | `delivery/` (v2-də `v2-ai/delivery/` idi) |

> **v3-də əsas yenilik (2026-09-26): nişan iki rənglidir** - mürəkkəb `K`,
> petrol `Ai` (bölmə 3.0). Ağ fonda
> `01-logo-svg/color/kestridge-lockup-horizontal-default-color.svg`, tünd
> fonda `01-logo-svg/color/kestridge-lockup-horizontal-default-color-knockout.svg`;
> tək rəngli iş üçün `01-logo-svg/mono/`. Paketin bütün qovluqları və fayl
> adları - bölmə 8.

**Yollar necə oxunur.** Paketin öz faylları `delivery/`-ə nisbətən yazılır
(`01-logo-svg/...`, `00-docs/...`); qalan hər şey layihə kökünə -
`projects/kestridge-ai-brand/` - nisbətəndir (`tools/...`, `qa/...`,
`archive/...`, `docs/...`, `delivery/...`). Bir istisna: palitranın mənbəyi
`brand-identity-2026/DESIGN-LANGUAGE.md` qonşu layihədədir, HQ-da
`projects/brand-identity-2026/DESIGN-LANGUAGE.md`.

**v2-nin iş qovluğu artıq diskdə yoxdur.** v2 yazılanda mənbə sətrində
`v2-ai/candidates/f2-strict-system/{plates,qa}.json` + `NOTES.md` və
`v2-ai/lockup-study/{RECOMMENDATION.md,ladder-measurements.json}` də vardı.
v3 təmizliyində `v2-ai/` silindi; həmin fayllar git tarixçəsindədir (`b6f3328`-dən
əvvəl, məsələn `79b9987` commit-ində). Seçilmiş namizədin `plates.json`-u
`tools/data/mark-plates-f2-strict-system.json` kimi qalır - JSON məzmunu eynidir
(2026-09-26-da müqayisə edildi). Aşağıda `NOTES.md`, `qa.json` və ya
`markv2.py` adı keçirsə, o, v2 tarixçəsinə istinaddır; `markv2.py`-ın bu gün
yeganə qarşılığı `tools/build/mark.py`-dır.

Bu sənəddəki hər rəqəm ölçülüb. `python tools/build/mark.py` nişan
həndəsəsini parametrlərdən yenidən törədir və `PASS` verir; kilid rəqəmləri
`qa/01-logo-svg-measurements.json`-dadır (v2-də `01-master/MEASUREMENTS.json`
idi). Rəqəm dəyişsə sınaq düşür.

**İki istisna açıq işarələnir, gizlədilmir:** (1) ölçmə olmayan tipoqrafik
konvensiyalar - məsələn `A` tirinin `30-40` faizlik zolağı (bölmə 2.5); (2)
elan edilən **ideal** dəyərlə göndərilmiş vektorun **çəkdiyi** dəyər arasındakı
yuvarlaqlaşdırma fərqi - hər iki rəqəm verilir (bölmə 5.1, 5.2). v2 mətni
burada `MEASUREMENTS.json`-un `drawn` blokuna da istinad edirdi; 2026-09-26-da
yoxlandı: nə `qa/01-logo-svg-measurements.json`-da, nə
`archive/v2-2026-09-21/delivery/01-master/MEASUREMENTS.json`-da belə blok
yoxdur. Çəkilən rəqəmlərin yeri bölmə 5.1 və 5.2-nin cədvəlləridir.

**Köhnə paketlər silinməyib.** v1 `archive/v1-2026-08-24/`-dədir, v2
`archive/v2-2026-09-21/`-dədir (202 fayl, 2026-09-21-də göndərilən
`delivery/`-in bayt surəti). v2 yazılanda v1 hələ `delivery/`-də idi və bu
sənəd v1-in brend qaydalarının (indi
`archive/v1-2026-08-24/delivery/02-BRAND-GUIDELINES.md`) **əvəzedicisi** idi,
düzəlişi yox (`archive/v2-2026-09-21/BRIEF.md` bölmə 8).

v2-də dəyişən bölmələr: **2** (nişan artıq beş lövhədir), **5** (kilid nisbəti
`M/C = 2/1`), **6** (reduktiv davranış və döşəmələr), **9** (aralıq qadağası
iki pilləli ifadəyə keçir). Qalan bölmələr v1-dəki qərarları saxlayır.

**v3-də (2026-09-26) dəyişən bölmələr:** **3** (nişan iki rəngdə göndərilir -
yeni bölmə **3.0**; 3.1-dəki "nişan petrol rəngdə çəkilmir" qaydası və bölmə 9
bənd 12-nin rəng cümləsi əvəz olunub), **8** (paketin yeni qovluq düzümü və
fayl adları; platforma şəkillərində kilidin ölçüsü). Qalan bölmələrdə fayl
yolları yenilənib və qısa v3 qeydləri əlavə olunub (5.3 - rəngli fayllarda
dairəvi ölçmə; 6 - reduktiv kəsikdə iki rəng; 9 - bənd 13; 10 - maddələr
9, 10, 23-25). Köhnə mətnin ölçmə faylına aid iki istinadı (`drawn` bloku,
`floor_basis` açarı) faylda tapılmadı və bu, yerində açıq yazılıb. Həndəsə,
təmiz sahə, minimum ölçülər, kilid qaydaları və tipoqrafika **dəyişməyib**.

---

## 1. Marka nədir

**Hazırkı kilid `KESTRIDGE AI`-dır.** Tam böyük hərf, heç vaxt qısaldılmır.
`KESTRIDGE` tək **ehtiyat markadır**, heç bir yenidən çəkmə tələb etmir və
qabaritləri ölçülüb (bölmə 5).

### 1.1 Qərar bağlanıb - sahib 2026-08-24-də təsdiqlədi

**Sahibin cavabı: `AI` hüquqi şəxs adında qalır.** Yəni şirkətin adı
`Kestridge AI`-dır. Canlı sayt da bunu təsdiqləyir: səhifə başlığı
`Kestridge AI | AI, Automation and IT Security in Illinois`.

Bu, hər iki qapını eyni anda bağlayır:

| Qapı | Nə idi | Nəticə |
|---|---|---|
| **U1** | Hüquqi şəxs adından `AI` düşürmü | **Düşmür.** Ad `Kestridge AI` |
| **U2** | `02-collision-scan` şərt 5 düzəliş tələb edirmi | **Etmir.** Şərt onsuz da ödənir |

**Nəticə: `KESTRIDGE AI` qəti əsas kiliddir.** `KESTRIDGE` tək ehtiyat marka
olaraq qalır və paketdə `reserve` etiketi ilə gəlir.

`docs/research/US-TRADEMARK-SCAN.md` bölmə 8 bənd 8 hələ də qüvvədədir və **ziddiyyət
deyil**: ticarət nişanı ərizəsi `KESTRIDGE` sözmarkası üçün verilir, çünki
`AI` təsviri sözdür və qoruma vermir. **Ərizə adı ilə görünüş adı ayrı
şeylərdir.**

### 1.2 v2-də adla bağlı heç nə dəyişmir

Yeni nişan `Ai`-ı **şəkil kimi** daşıyır, mətn kimi yox. Bu, adın yazılışına,
kilid sətrinə və ticarət nişanı strategiyasına heç nə əlavə etmir. Bölmə 9
bənd 3 qüvvədədir: yalnız-nişan variantı heç vaxt `K`, `KAI`, `Kestr` mətn
forması ilə yan-yana görünmür - **`KAi` də daxil olmaqla**.

---

## 2. Nişan

### 2.1 Konsept

**Beş ayrılmış lövhə, iki qat:**

| Qat | Lövhələr | Nə oxunur |
|---|---|---|
| struktur | `stem`, `arm`, `leg` | `K` - monoqram, Kestridge |
| örtük | `tittle`, `crossbar` | `i` (qol + nöqtə) və `A` (gövdə + ayaq + tir) |

Struktur qatı **göndərilmiş nişandır**: `stem` və `leg` v1 ilə təpə-təpə
eynidir, `arm`-ın altı təpəsindən üçü yerindədir, qalan üçü dəqiq `(-k, +k)`
sürüşüb. Sürüşmə qolun öz anti-diaqonalı üzrədir, ona görə terminalın forması
dəyişmir, qol yalnız bir faslə geri çəkilir.

**Qovşaq yoxdur.** Beş kontur heç bir yerdə bir-birinə toxunmur. Şrift qlifi
belə ola bilməz, çünki qlif bağlı olmalıdır - müdafiə burada siluetdə deyil,
**topologiyadadır**.

Nişanı `K` edən şey artıq tək sabit deyil, **iki pilləli bir şəbəkədir**:
hər ədəd `k = c/4`-ün mislidir (bölmə 9 bənd 6).

### 2.2 Ölçülmüş həndəsə

#### 2.2.1 Parametrlər

| Parametr | Dəyər |
|---|---|
| Çərçivə | `viewBox="0 0 24 24"`, `u = 1/24` |
| **`c`** (struktur aralığı) | **`2.7u`** - K-nın öz üç kanalı |
| `k` (faslə) | `c/4 = 0.675u` |
| **`o`** (örtük aralığı) | **`2k = 1.35u`** - örtük lövhəsi ilə onun bağlandığı lövhə |
| **`m`** (örtük modulu) | **`3k = 2.025u`** - nöqtənin tərəfi, tirin qalınlığı, tirin baseline-dan hündürlüyü |
| `w` (gövdə eni) | `4.0u` əsas kəsik |
| `a` (qol qalınlığı) | `5.0u` əsas kəsik |
| `r` (qolun geri çəkilməsi) | `c + k = 5k = 3.375u` |
| Bucaq ailəsi | **0 / 45 / 90**. Dördüncü istiqamət yoxdur |
| Künc radiusu | **0**, hər yerdə |
| Kontur | yalnız `fill`. `stroke` yoxdur, əyri seqment yoxdur |
| Təpə sayı | **25** (4 + 6 + 4 + 6 + 5) |

Sıx kəsik (`dense`) **arxivdədir**: `cut_for()` onu heç vaxt qaytarmır və
paketdə çəkilmir (bölmə 6.1).

#### 2.2.2 Lövhə-lövhə

| Lövhə | Təpə | Mürəkkəb qutusu | Gövdə ölçüsü | Sahə `u^2` | Ink payı |
|---|---|---|---|---|---|
| `stem` | 4 | `[3, 3] .. [7, 21]` | `4.000` en | `72.000` | `0.5091` |
| `arm` | 6 | `[9.7, 6.375] .. [17.625, 10.65]` | `3.536` perpendikulyar | `20.919` | `0.1479` |
| `tittle` | 4 | `[16.275, 3.0] .. [18.3, 5.025]` | `2.025 x 2.025` kvadrat | `4.101` | `0.0290` |
| `leg` | 6 | `[9.7, 13.35] .. [21, 21]` | `3.536` perpendikulyar | `37.794` | `0.2672` |
| `crossbar` | 5 | `[8.35, 16.95] .. [12.065812, 18.975]` | `t = 2.025`, `W_top = 2.365812`, `W_bot = 3.715812` | `6.613` | `0.0468` |

v2 namizədinin `qa.json`-undakı (git tarixçəsində, yuxarıya bax) `thickness_proxy` (`4A/P`) **qalınlıq deyil**: o, uzun
düzbucaqlıdan başqa hər formada həqiqi gövdə enini şişirdir. Yuxarıdakı
"gövdə ölçüsü" sütunu həqiqi minimum endir.

#### 2.2.3 Ölçmə ilə təsdiqlənən nəticələr

| Nə | Ölçülən |
|---|---|
| Mürəkkəb qutusu | **`18.000 x 18.000`** - dəqiq kvadrat, `[3,21] x [3,21]` |
| Kənar boşluq | `3.0u` hər dörd tərəfdən |
| Piksel şəbəkəsi | 16 / 32 / 48 / 128 / 512 px-də **dörd kənarın hamısı tam piksel** |
| Ən kiçik daxili bucaq | **`90.0`** dərəcə, hər beş konturda. Bucaq çoxluğu `{90, 135}`, iti künc yoxdur. Ölçən: `tools/build/mark.py` `interior_angles()`, öz-sınağın 4-cü bölməsi (v2-də `markv2.py`) |
| **Struktur aralıqları** | **`2.7000`** - `stem\|arm`, `stem\|leg`, `arm\|leg` |
| **Örtük aralıqları** | **`1.3500`** - `arm\|tittle`, `stem\|crossbar`, `leg\|crossbar` |
| Elan edilmiş **lövhə-lövhə** aralıqların dəyər sayı | **dəqiq iki**: `[1.35, 2.7]` (tirin baseline-dan `3k` hündürlüyü aralıq deyil, moduldur - bölmə 9 bənd 6) |
| `min_gap` | `1.35` |
| Ink örtüyü | `0.245534` (v1 `0.232793`, **`+5.47%`**) |
| Qol/gövdə çəki nisbəti | `0.883883` = `a / sqrt(2) / w` - **v1 ilə eyni** |
| Bağlanma eyniliyi | `w + a - 2k = 7.65`, yəni `w + a = 9.0` - **v1 ilə eyni** |
| Asimmetriya | `3.375 = c + k` **iki ortoqonal oxda** (v1-də `2.7 = c` idi) |
| Yarımdiaqonal | `12.727922u` - v1 ilə eyni, çünki qutu dəyişməyib |

**Daxili bucaq sətri indi həqiqətən ölçülür.** Əvvəl bu cədvəldə dururdu, amma
v2-nin `markv2.py`-ı (indi `tools/build/mark.py`) yalnız **kənar istiqamətlərini** (0 / 45 / 90 ailəsi) ölçürdü -
kənar ailəsi isə küncün özünü sübut etmir. Ona görə generatora
`interior_angles()` əlavə olundu: hər lövhə öz dolanma istiqamətində gəzilir,
xarici dönmə həmin istiqamətin işarəsi ilə götürülür, daxili bucaq `180 -
dönmə` olur, və yoxlama hər lövhə üçün cəmin `(n - 2) x 180` olmasıdır
(`360 / 720 / 360 / 720 / 540`). Nəticə: bütün 25 küncdə bucaq ya `90`, ya
`135`-dir, ən kiçiyi `90.000000`. Eyni rəqəm `f2-strict-system/plates.json`
(indi `tools/data/mark-plates-f2-strict-system.json`)
üzərində müstəqil hesablanmaqla da təsdiqləndi.

**`leg|crossbar` aralığı `1.35`-dir və perpendikulyar ölçülür**, üfüqi yox.
Üfüqi ofset `2k * sqrt(2) = 1.909188`-dir. Onu üfüqi `1.35` kimi qurmaq
perpendikulyarda `0.9546` verir - elan edilməmiş üçüncü dəyər və 16 px-də
qaynaq. Kanal başdan-başa sabit enlidir.

#### 2.2.4 Qapanış eynilikləri

Bunlar nişanı dəyişməzdən əvvəl işlədilən bir sətirlik testlərdir. Hamısı
`tools/build/mark.py` öz-sınağında yoxlanılır (v2-də `markv2.py`):

```
cap zolağının bölgüsü   h + o = 3k + 2k = 5k = c + k = r = 3.375
gövdə qapanışı          w + a = h + o + Sa + 2k = 9.0
counter baseline-da     18 - (w + a) + k = 9.675
qolun sağ kəsimi        21 - (c + k) = 17.625
nöqtənin sağ kənarı     21 - c = arm_right + k = 18.3
tirin üzləri            BASE - 6k = 16.95,  BASE - 3k = 18.975
tirin eni               W_bot = counter(18.975) - 2k - 2k*sqrt(2) - k = 3.715812
örtüyün tək ölçüsü      nöqtənin tərəfi = tirin qalınlığı = tirin hündürlüyü = 3k
```

**Bağlanma eyniliyi pozulsa mürəkkəb qutusu kvadrat olmur.**

### 2.3 Niyə şevron oxunmur

İki ayrılmış diaqonal `<` kimi oxuna bilərdi. Dörd ölçülmüş əlamət bunu
dayandırır:

1. **Güzgü simmetriyası yoxdur** - ayağın ucu `x = 21`, qolun ucu `x = 17.625`.
   Fərq `3.375 = c + k`; v1-də `2.7` idi, yəni asimmetriya **artıb**.
2. **Qollar sərbəst dayanmır** - aralıqda `4u` gövdə durur və mürəkkəbin
   `50.9` faizini daşıyır.
3. **Qollar bərabər deyil** - sahələri `37.794 u^2` (ayaq) və `20.919 u^2`
   (qol), nisbət `0.554`.
4. **Tir şevronu pozur** - şevronun köndələn tiri olmur.

Şevron bərabər qollar və simmetriya tələb edir. Heç biri yoxdur.

v1-dəki "ox uzunluqları `10.82u` və `7.00u`" sətri götürülmüşdü
`docs/concept/00-CONCEPT-BRIEF.md`-dən; həmin ölçmənin tərifi göndərilmiş ölçmə
kodunda yoxdur, ona görə v2-də onun yerinə **lövhə sahələri** yazılır -
`tools/build/mark.py` (v2-də `markv2.py`) onları birbaşa verir.

### 2.4 Nişan güzgüyə salınmır - heç vaxt

Güzgü `K`-nı öldürür **və** eyni anda simmetriyanı bərpa edib şevron
oxunuşunu geri gətirir. Üstəlik güzgüdə `A` tiri gövdənin yox, ayağın
tərəfinə düşür və `A` oxunuşu da ölür. Ərəb qrafikalı materialda nişan söz
işarəsinin **sağına** qoyulur, forması dəyişmir.

### 2.5 Örtüyün oxunuşu - elan edilmiş hədd

Bu bölmə marketinq mətninin nə deyə biləcəyini müəyyən edir.

**`A` tapılan oxunuşdur, göz önündə olan deyil.** Tir öz səviyyəsində
counter-in `48.57` faizini kəsir və mürəkkəbin `4.68` faizini daşıyır. Tavan
da təxminən yarıdır: `A`-nın sol ştrixi şaquli gövdədir, ona görə counter
**düzbucaqlı üçbucaqdır**, `A`-nın açılan splayı deyil, və apeksi yoxdur.
Daha uzun tir yalnız 16 px-də əriyən aralıqlarla alınır.

Tirin yeri üçbucağın hündürlüyünün **`31.4`** faizindədir. Bu rəqəm
ölçülmüşdür (v2 namizədinin `f2-strict-system/NOTES.md` bölmə 5-i, indi git
tarixçəsində: tirin mərkəzi baseline-dan `3.0375`, üçbucağın hündürlüyü `9.675`).

**`30-40` faiz isə ölçmə deyil, konvensiyadır.** `A` hərfinin köndələn tiri
üçün tipoqrafik ədəbiyyatda işlənən adi zolaqdır; bu paketdə onu ölçən sınaq
**yoxdur** və ona istinad edən heç bir qərar da yoxdur - tirin yeri `3k`
modulundan çıxır, zolaqdan yox. Sənədin "hər rəqəm ölçülüb" vədi bu cümləyə
şamil edilməsin deyə burada açıq işarələnir: **ölçülən `31.4`-dür, `30-40`
istinad çərçivəsidir.**

**`i`-nin gövdəsi 45 diaqonaldır**, klassik şaquli gövdə deyil. Bu, nişanın
konsepsiyasının içindədir: qol `i`-nin gövdəsidir. Nöqtə kompakt kvadratdır,
qolun düz üst kəsiyinin üstündə oturur və `1.35` üst-üstə düşmə ilə paralel
ağ kanal saxlayır - künc öpüşü deyil.

**İcazə verilən cümlə:** "nişanın içində `Ai` var". **İcazə verilməyən
cümlə:** "nişan `A` və `i` hərflərindən ibarətdir". Nişan `K`-dır; `Ai`
ikinci qatdır.

---

## 3. Rəng

Palitra `brand-identity-2026/DESIGN-LANGUAGE.md` bölmə 2-dən bütöv götürülür.
Burada yalnız **nişana aid** hissə var. **v2-də rəng qərarları dəyişmir.**
**v3-də dəyişir:** nişan iki rəngdə göndərilir - bölmə 3.0. Palitranın özü
dəyişmir; yeni rəng əlavə olunmur, iki mövcud petrol pilləsi nişana daxil olur.

### 3.0 Nişanın iki rəngi - v3 (2026-09-26)

**Əsas nişan artıq iki rənglidir: mürəkkəb `K`, petrol `Ai`.** Həndəsə
dəyişmir - eyni beş lövhə (`f2-strict-system`), eyni `18 x 18` ink qutusu
(`[3, 21]`), eyni `2.7` struktur / `1.35` örtük aralığı, `24` board px-dən
aşağıda eyni reduktiv kəsik, eyni kilid (`M / C = 2/1`). Dəyişən yalnız
lövhələrin mürəkkəbidir. Rənglər bir yerdədir: `tools/build/mark.py`,
`COLOUR` və `COLOUR_KNOCKOUT`.

#### 3.0.1 İki rəng yolu

| Lövhə / hissə | **`color`** (açıq fon) | **`color-knockout`** (tünd fon) |
|---|---|---|
| `stem` (gövdə) | `#0F1317` ink | `#FAFAF7` paper |
| `arm`, `tittle`, `leg`, `crossbar` | `#0E6A82` petrol-600 | `#2FA8C7` petrol-400 |
| sözmarka | `#0F1317` | `#FAFAF7` |
| deskriptor sətri | `#0E6A82` | `#2FA8C7` |

`petrol-600` `#0E6A82` saytın öz aksentidir: `kestridge.com`-da `--accent`
dəyəri budur (ölçülüb). Yəni nişan saytla eyni petrolu daşıyır.

#### 3.0.2 Niyə bölgü `K` / `Ai`-dır

Gövdə (`stem`) **yalnız `K`-ya aid** olan yeganə lövhədir. Qalan dördü iki
hərfdə işləyir: qol həm də `i`-nin gövdəsidir, nöqtə `i`-nin nöqtəsidir, ayaq
həm də `A`-nın diaqonalıdır, tir `A`-nın tiridir (bölmə 2.1, 2.5). Ona görə
rəng bölgüsü oxunuşun özüdür: **mürəkkəb `K` + petrol `Ai`**. Rəng yeni detal
yaratmır, həndəsənin onsuz da daşıdığı ikinci qatı göstərir.

**Bölgü reduktiv kəsikdə də qalır.** `24` board px-dən aşağıda nöqtə və tir
çəkilmir (bölmə 6); gövdə (mürəkkəb) və qol ilə ayaq (petrol) qalır. Ona görə
`16 px` favicon da iki rənglidir.

#### 3.0.3 Kontrast - ölçülmüş

WCAG 2 nisbi parlaqlıq düsturu, 3.2-dəki cədvəllə eyni hesab:

| Cüt | Nisbət |
|---|---|
| petrol-600 paper `#FAFAF7` üzərində | **`5.90:1`** |
| petrol-600 white `#FFFFFF` üzərində | `6.17:1` |
| petrol-400 ink `#0F1317` üzərində | **`6.70:1`** |
| petrol-400 surface `#171D23` üzərində | `6.10:1` |
| petrol-400 black `#000000` üzərində | `7.55:1` |
| paper ink üzərində | `17.84:1` |
| **petrol-600 ink-ə qarşı** (gövdə ilə qol, yəni bölgünün özü) | **`3.02:1`** |

Döşəmələr hər yerdə eynidir: qrafika **`3.0:1`** (WCAG 1.4.11), mətn
**`4.5:1`**. Hər lövhə öz fonunda qrafik döşəməni keçir. Deskriptor mətndir
və onu da keçir: açıq fonda petrol-600 `5.90:1`, tünd fonda petrol-400
`6.70:1` (ink) və `6.10:1` (surface).

**Boz çap - son sətir buna görə var.** İki mürəkkəb yalnız çalarda deyil,
**parlaqlıqda** da fərqlənir: petrol-600 ilə ink arasında `3.02:1`. Ona görə
bölgü ağ-qara çapda və rəng görmə pozuntularının çoxunda da oxunur - rəngi
götürəndə `K` ilə `Ai` yenə iki ayrı tondur.

#### 3.0.4 Hansı variant nə vaxt

| Variant | Harada | Nümunə fayl |
|---|---|---|
| **`color`** - default | açıq neytral fon: paper `#FAFAF7`, white `#FFFFFF` | `01-logo-svg/color/kestridge-lockup-horizontal-default-color.svg` |
| **`color-knockout`** | tünd neytral fon: ink `#0F1317`, surface `#171D23`, black `#000000` | `01-logo-svg/color/kestridge-lockup-horizontal-default-color-knockout.svg` |
| `positive` `#0F1317` | tək rəngli iş, açıq fon | `01-logo-svg/mono/kestridge-lockup-horizontal-default-positive.svg` |
| `knockout` `#FAFAF7` | tək rəngli iş, tünd fon | `01-logo-svg/mono/kestridge-lockup-horizontal-default-knockout.svg` |
| `mono-black` `#000000` | tək rəngli çap, faks, oyma | `01-logo-svg/mono/kestridge-lockup-horizontal-default-mono-black.svg` |
| `mono-white` `#FFFFFF` | tünd tək rəngli səth | `01-logo-svg/mono/kestridge-lockup-horizontal-default-mono-white.svg` |

Qayda üç bənddir:

1. **Rəng mümkündürsə:** açıq fonda `color`, tünd fonda **yalnız**
   `color-knockout`.
2. **Proses tək rənglidirsə** (çap, basma / embossing, faks, tikmə, oyma):
   dörd mono variantdan biri - bölmə 3.1 və 7.
3. **Fon yuxarıdakı cədvəldə yoxdursa** (məsələn petrol sahəsi və ya foto):
   rəng yolunun həmin fondakı kontrastı ölçülməyib (`unknown`); işlətməzdən
   əvvəl hər lövhə öz fonunda `3.0:1`-ə qarşı ölçülür.

Tərkibli aktivlər artıq rənglidir: app ikonları və favicon, ofis materialları
(kartvizit, blank, zərf, slaydlar, e-imza; tünd bölücü slayd
`color-knockout`-dur), bütün platforma şəkilləri, sayt başlıq kilidləri
(açıq: `color`, tünd: `color-knockout`), sayt ikonları (`08-website/icon.svg`
tünd plitə: `color-knockout`; `08-website/icon-light.svg`: `color`;
`08-website/apple-icon.png`: surface üzərində `color-knockout`), OpenGraph
şəkilləri (`color-knockout`), LinkedIn bannerləri (`color-knockout`).

#### 3.0.5 Yeni qadağa: bölgü yenidən rənglənmir

- **Dəqiq iki rəng, hərf qrupu üzrə** - `K` (gövdə) və `Ai` (qol, nöqtə,
  ayaq, tir). Lövhə-lövhə rəng yoxdur; nöqtə və ya tir **təkbaşına** heç vaxt
  rənglənmir, yalnız bütöv `Ai` qrupunun hissəsi kimi petroldur.
- Açıq fonda petrol-600-dən başqa **petrol pilləsi yoxdur**.
- Petroldan başqa **çalar yoxdur**.
- Tünd fonda **yalnız** `color-knockout`.
- Rəng yolunda (`color`, `color-knockout`) bölgünün yeri dəyişmir: gövdə həmişə neytral, qalan dörd lövhə həmişə petrol; tək rəngli işdə beş lövhə də bir mürəkkəbdədir (3.0.4).

#### 3.0.6 Rəng necə seçildi

- **Raund 1:** altı rəng linzası, `34` təklif, adversarial süzgəc `11`-ini
  saxladı. Sahibin rəyi: nişan **ağ fonda** qalmalıdır; "rəng" `K`-nın
  hissələrini ayrı rəngləmək deməkdir (bəyəndiyi: raund 1 `#7`, `#8`, `#11`).
- **Raund 2:** altı linza (spark, duotone, bold, letter, structure, tonal),
  `45` təklif. Süzgəc rəy deyil, ölçmə idi: palitra üzvlüyü, hər lövhə öz
  fonunda `>= 3.0:1`, ən çox `3` rəng, bir variantdakı istənilən iki rəng
  arasında `>= 1.35:1` (ikisi düşdü: petrol-500 / neutral-500 `1.02:1`,
  petrol-700 / neutral-700 `1.03:1`), təkrarlar çıxarıldı (`3`). `40` təklif
  keçdi, sahibə `12`-lik qısa siyahı göstərildi.
- **Seçim:** sahib 2026-09-26-da seçimi həvalə etdi. Seçilən qısa siyahının
  `#9`-udur: `struktur-govde-ink-qollar-petrol`.
- **Niyə `#9`, sahibin bəyəndiyi `#7` (spark) və `#8` (duotone) yox:** o ikisi
  rəngi yalnız nöqtə və tirə qoyur, reduktiv kəsik isə məhz o iki lövhəni
  çıxarır - favicon ölçüsündə nişan tamamilə mürəkkəbə dönür. `#9` rəngini
  hər ölçüdə saxlayır.

Birinci raundun datası (`v2-ai/COLOUR-VARIANTS.json`) və ikinci raundun yalnız ilk üç linzası (`v2-ai/COLOUR-ROUND2-PARTIAL.json`, `21` təklif) v2 iş qovluğunda commit olunmuşdu; v3 təmizliyində silinib, `79b9987` commit-indən bərpa olunur. İkinci raundun tam `45` təklifi, süzgəcin çıxışı və `12`-lik qısa siyahı repoya düşməyib (`delivery/00-docs/01-DECISION-RECORD.md` 12.1). Qərar zənciri: `delivery/00-docs/01-DECISION-RECORD.md`.

### 3.1 Nişanın rəngləri

**v3 qeydi:** bu bölmə v2-də yazılıb və nişanı tək rəngli təsvir edir. v3-də
aşağıdakı cədvəl **tək rəngli (mono) variantların** cədvəlidir; "default"
sözü v2-yə aiddir - v3-də açıq fonda default `color`-dur (3.0.4).

| Rol | Token | HEX | Nə vaxt |
|---|---|---|---|
| **Müsbət** | `neutral-950` | **`#0F1317`** | açıq fonda, default |
| **Knockout** | `neutral-025` | **`#FAFAF7`** | tünd fonda |
| Mono, tünd | - | `#000000` | tək rəngli çap, faks, oyma |
| Mono, açıq | - | `#FFFFFF` | tünd tək rəngli səth |
| Kağız | `neutral-025` | `#FAFAF7` | |
| Tünd səth | `neutral-900` | `#171D23` | |

> **R1 - v3-də əvəz olunub (2026-09-26), bax bölmə 3.0.** Aşağıdakı ilk
> cümlə v2 qaydasıdır və o vaxtkı qərar kimi saxlanılır. İndi əsas nişan iki
> rənglidir: gövdə neytral mürəkkəb, `Ai` qrupu petrol (`#0E6A82` açıq fonda,
> `#2FA8C7` tünd fonda). `petrol-500`-ün aksent rolu dəyişmir.

**Nişan marka kimi petrol rəngdə çəkilmir.** `petrol-500` `#1187A5`
aksentdir - ikon, sərhəd, fokus halqası, qrafik ştrix. Marka neytral
mürəkkəbdə qalır.
Petrol pilləsi: `petrol-400 #2FA8C7`, `petrol-500 #1187A5`,
`petrol-600 #0E6A82`, `petrol-700 #0B5568`, `petrol-800 #004051`,
`petrol-900 #002C38`. **Petrol yeganə xromatik aksentdir.**

**Bir istisna var və o, adlandırılıb: fon teksturası.** Nişan səhifə
hündürlüyündən böyük çəkilib kənarla kəsiləndə və mətnin arxasında qalanda
o, marka deyil - qrafik sahədir. Orada `petrol-700` və `petrol-900` işlədilir
(`09-linkedin-banner`, v2-də `09-banner`; sahibin seçdiyi `b4-oversize-monogram`). Şərt üçdür: həmin
kadrda **oxunan marka ayrıca durmalıdır**, tekstura ondan ən azı iki ton
aşağı olmalıdır, və tekstura heç vaxt yeganə nişan kimi qalmamalıdır.
Bu istisna yalnız fon üçündür - kiçik ölçüdə, kilidin içində, ikon və ya
avatar kimi nişan həmişə neytral mürəkkəbdədir.

> **Tekstura istisnası v3-də dəyişmir.** Bannerdəki nəhəng monoqram yenə
> `petrol-700` (petrol-800 lövhədə) və `petrol-900` (sahədə) teksturadır, üç
> şərt qüvvədədir. Yuxarıdakı son cümlənin "həmişə neytral mürəkkəbdədir"
> hissəsi isə **v3-də əvəz olunub (2026-09-26)**: kiçik ölçüdə, kilidin içində,
> ikon və avatar kimi nişan indi 3.0-dakı iki rəng yolundan birindədir, tək
> rəngli işdə isə mono variantdadır (3.0.4). Bannerin oxunan kilidi də artıq
> `color-knockout`-dur.

**Beş lövhə beş rəng demək deyil.** Örtük lövhələri struktur lövhələri ilə
**eyni mürəkkəbdə** çəkilir. Nöqtəni və ya tiri aksent rəngə boyamaq
qadağandır (bölmə 9 bənd 12): nişan tək rəngə yastılananda sağ qalmalıdır və
rəng fərqi ilə detal yaradılmır.

> **v3 (2026-09-26):** qaydanın başlığı - **beş lövhə beş rəng demək deyil**
> (R4) - **qüvvədədir və sərtləşib:** dəqiq **iki** rəng, **hərf qrupu üzrə**
> (`K` / `Ai`), heç vaxt lövhə-lövhə (3.0.5). Ortadakı iki cümlə
> ("eyni mürəkkəbdə çəkilir", "nöqtəni və ya tiri aksent rəngə boyamaq
> qadağandır", R3) **v3-də əvəz olunub (2026-09-26)**: nöqtə və tir indi
> petroldur, amma yalnız bütöv `Ai` qrupunun hissəsi kimi, heç vaxt
> təkbaşına. Son hissə qüvvədədir: nişan tək rəngə yastılananda sağ qalır
> (mono variantlar eyni həndəsədir) və detalı rəng yox, aralıqlar yaradır -
> rəng yalnız həndəsənin onsuz da ayırdığı iki qrupu göstərir.

### 3.2 Kontrast - ölçülmüş

| Fon | Müsbət `#0F1317` | Knockout `#FAFAF7` |
|---|---|---|
| Paper `#FAFAF7` | **17.84** | 1.00 |
| White `#FFFFFF` | **18.65** | 1.05 |
| `neutral-150` `#DDE2E6` | **14.30** | 1.25 |
| Surface `#171D23` | 1.10 | **16.24** |
| Ink `#0F1317` | 1.00 | **17.84** |
| `petrol-400` `#2FA8C7` | **6.70** | 2.66 |
| `petrol-500` `#1187A5` | **4.47** | **3.99** |
| `petrol-600` `#0E6A82` | **3.02** | **5.90** |
| `petrol-700` `#0B5568` | 2.23 | **7.99** |

Marka istənilən fonda `>= 3:1` olmalıdır. Hər fonda ən azı bir variant
qaydanı keçir.

**`petrol-400` və `petrol-600` bu cədvələ sonradan əlavə olundu, çünki v2
paketi hər ikisini masaya gətirir, cədvəl isə onları buraxmışdı.**
`petrol-600` `#0E6A82` sahibin banner üçün göstərdiyi rəngdir
(`00-docs/01-DECISION-RECORD.md` 1.2) və LinkedIn banneri (`09-linkedin-banner`,
v2-də `09-banner`) onu faskada və aksent tirində işlədir
(`00-docs/05-BANNER-NOTES.md` bölmə 5, sahə pillələri). `petrol-400` `#2FA8C7` banner
mühərrikinin paletindədir (`tools/build/banner.py`, v2-də `v2-ai/build/banner.py`) və v2 bannerinin ilk
yığımında deskriptor sətrinin rəngi idi; həmin sətir sonradan `9 px` cap
döşəməsinə görə çıxarıldı, amma rəng paletdə qalır, ona görə nisbəti burada
durmalıdır. İkisi də WCAG 2.x nisbətidir, paletdəki digər sətirlərlə eyni
düsturla hesablanıb.

**v3-də hər iki pillə nişanın özündədir** (3.0): `petrol-600` `color`-da,
`petrol-400` `color-knockout`-da `Ai` qrupunun rəngidir. `petrol-400` beləliklə
bannerə də qayıdıb - kilid `color-knockout`-dur və onun ən zəif mürəkkəbi ink
sahəsi üzərində `petrol-400`-dür: `6.70:1` (v2-də yalnız paper ilə `17.84:1`
idi); `4.5` qapısı keçir. Yuxarıdakı cədvəl isə **tək rəngli** variantları
neytral və petrol **fonlar** üzərində ölçür və v3-də dəyişmir.

**Diqqət - `petrol-400` fonunda yalnız müsbət variant işləyir.** Orada
knockout `2.66`-dır, yəni `3:1` həddindən aşağı. `petrol-600` fonunda hər iki
variant keçir (`3.02` və `5.90`), amma müsbət variant həddin **dibindədir** -
kiçik ölçüdə knockout seçilir.

### 3.3 Qadağalar

- **Qradiyent yoxdur** - nə markada, nə səthdə.
- **İkinci brend çaları yoxdur.** `petrol-*` yeganə xromatik aksentdir.
- **Nişanın rəng bölgüsü yenidən rənglənmir** (v3, 2026-09-26): dəqiq iki
  rəng, `K` / `Ai` üzrə; açıq fonda petrol-600-dən başqa pillə yoxdur,
  petroldan başqa çalar yoxdur, tünd fonda yalnız `color-knockout` - bölmə 3.0.5.
- **Semantik rənglər** (`success` / `warning` / `danger` / `info`) interfeys
  mülkiyyətidir, marketinq materialında dekorativ işlədilmir.
- Qırmızı + təhlükəsizlik + bucaqlı işarə üçlüyündən uzaq durulur.

---

## 4. Tipoqrafika

**v2-də tipoqrafika qərarları dəyişmir.** Dəyişən yalnız sözmarkanın kiliddəki
**ölçüsüdür** (bölmə 5).

### 4.1 Yığın

| Rol | Ailə | Versiya | Lisenziya |
|---|---|---|---|
| Sözmarka / display | **Archivo** | 2.001 | SIL OFL 1.1 |
| İnterfeys / mətn | **Inter Variable** | 4.001 | SIL OFL 1.1 |
| Kod / data | **JetBrains Mono** | 2.211 | SIL OFL 1.1 |

### 4.2 Sözmarka spesifikasiyası

| Parametr | Dəyər |
|---|---|
| Çəki | `wght 600` (stem `130 pm em`, cap-ın `19.0` faizi) |
| En | `wdth 100` yatıq kiliddə, `wdth 70` stacked və dar konteynerdə |
| Registr | **tam BÖYÜK HƏRF** |
| Söz boşluğu | `0.45 x cap` = **`309 pm em`**, tracking-dən **asılı deyil** |

**Vahid qaydası:** bu sənəddə `pm` **em-in mində biridir**. Archivo-nun `cap`-ı
`686` vahiddir. CSS-də `pm em` birbaşa `em`-dir: tracking `-14` ->
`letter-spacing: -0.014em`.

### 4.3 Cüt düzəlişləri - məcburi

| Cüt | K-E | E-S | S-T | T-R | R-I | I-D | D-G | G-E | A-I |
|---|---|---|---|---|---|---|---|---|---|
| **pm em** | -19 | -58 | -39 | -39 | +18 | +25 | +5 | +4 | 0 |

Statik loqo fayllarında marka **kontura çevrilir** və bu problem qalxmır.

### 4.4 Tracking

```
tracking(pm em) = -24.2 * log2(cap_px / 48),   [-30, +30] araligina kesilmish
```

| cap hündürlüyü | tracking | CSS |
|---|---|---|
| <= 20 px | +30 | `+0.030em` |
| 32 px | +14 | `+0.014em` |
| **48 px (etalon)** | **0** | `0` |
| **72 px** | **-14** | `-0.014em` |
| >= 128 px | -30 | `-0.030em` |

**Kilid mühərriki tracking-i `-14`-də sabitləyir** (`lockup.py` `TRACK`),
ölçüdən asılı olmayaraq - bu, v1-dən dəyişməyib. Dəyişən odur ki, `cap = 72 px`
nöqtəsi indi daha böyük kilidə düşür: `M/C = 2/1`-də cap `72 px` olanda nişan
`144 px`, kilid eni isə `885 px`-dir (v1-də müvafiq olaraq `96 px` və `826 px`
idi).

**Tracking `KESTRIDGE AI` sətrində 10 aralığa verilir, 9-a yox.** Boşluqdan
**əvvəlki** aralıq da tracking alır; boşluğun özünün eni isə sabit qalır
(`0.45 x cap`).

### 4.5 Ölçülmüş qabaritlər

Cüt düzəlişləri tətbiq olunmuş, tracking `-14`:

| Sətir | `wdth` | `en/cap` | `hünd/cap` | ink AR |
|---|---|---|---|---|
| **`KESTRIDGE AI`** | 100 | **9.8407** | 1.0350 | **9.508** |
| **`KESTRIDGE AI`** | 70 | **7.3800** | 1.0364 | **7.121** |
| `KESTRIDGE` | 100 | **8.0175** | 1.0350 | 7.746 |
| `KESTRIDGE` | 70 | 5.9169 | 1.0364 | 5.709 |

Bu sütunlar `en/cap` nisbətidir, ona görə **kilid nisbəti dəyişəndə də
dəyişmirlər**. Bölmə 5-dəki mütləq enlər bunlardan çıxır.

Overshoot iki qlifdədir - `G` **və** `S`, hər ikisi `12` vahid.

---

## 5. Kilidlər

`M` = nişanın mürəkkəb hündürlüyü = `18u`. `C` = sözmarkanın cap hündürlüyü.
Hər ölçü `c`-nin qatıdır.

**`M` nişanın cap zolağıdır** (`y = 3 .. 21`), bütöv ink qutusu deyil. `f2`-də
nöqtə qutunun **içindədir**, ona görə bu iki kəmiyyət eynidir və qayda
sadələşir: `MARK.ink_box()` hündürlüyü `18u`-dur və `lockup.py`-dakı
`M = 18.0` sabiti toxunulmur.

### 5.1 Yatıq kilid

| Parametr | Dəyər | v1-də nə idi |
|---|---|---|
| Cap hündürlüyü | **`C = 10c/3 = 9.0u`** | `5c = 13.5u` |
| **Ölçü nisbəti** | **`M / C = 2/1`** (ideal; göndərilmiş vektor `1.999929` çəkir - aşağıya bax) | `4/3` |
| Aralıq | `1.5c = 4.05u` - nişanın ink sağ kənarından sözmarkanın ink sol kənarına | dəyişmir |
| **`GAP / cap`** | **`0.450`** | `0.300` |
| Şaquli düzülmə | nişanın ink mərkəzi = sözmarkanın **cap zolağının** mərkəzi | dəyişmir |
| Sözmarka eni | `wdth 100`, tracking `-14` | dəyişmir |

| Sətir | Bütöv ink | AR | Kanvas (`c` pad ilə) |
|---|---|---|---|
| **`KESTRIDGE AI`** (default) | **`110.616035u x 18u`** | **`6.1453`** | `116.016035 x 23.4` |
| `KESTRIDGE` (ehtiyat) | `94.207434u x 18u` | `5.2337` | `99.607434 x 23.4` |

Overshoot optikdir və düzülməyə **girmir**.

**İdeal rəqəm ilə çəkilən rəqəm eyni deyil - fərq burada açıq yazılır.**
`lockup.py` sözmarka transformunu altı onluq rəqəmlə yazır
(`scale(0.013120 -0.013120)`), dəqiq miqyas isə `9.0 / 686 = 0.0131195335`-dir.
Nəticə göndərilmiş fayllarda ölçülüb:

| Kəmiyyət | Elan edilən (ideal) | **Göndərilmiş vektorda çəkilən** |
|---|---|---|
| `M / C` | `2.000000` | **`1.999929`** |
| Sözmarka cap | `9.000000u` | `9.000320u` |
| `KESTRIDGE AI` ink eni | `110.616035u` | `110.619204u` |
| `KESTRIDGE AI` AR | `6.145335` | **`6.145511`** |
| `KESTRIDGE` ink eni | `94.207434u` | `94.210020u` |
| `KESTRIDGE` AR | `5.233746` | `5.233890` |

Ölçmə `01-logo-svg/mono/kestridge-lockup-horizontal-{default,reserve}-positive.svg`
(v2-də `01-master/`) fayllarının öz yol datasından götürülüb; v3-ün tək
rəngli masterləri v2 arxivi ilə bayt-bayt eynidir. Fərq `2e-4`-dən kiçikdir, yəni
`1024 px` enli renderdə `0.03 px`; qərar dəyişmir, amma **"dəqiq `2.0`"
demək olmaz**. v2 mətni burada "`01-master/MEASUREMENTS.json` hər iki rəqəmi
saxlayır" deyirdi; 2026-09-26-da yoxlandı: `qa/01-logo-svg-measurements.json`
yalnız ideal `ratio_M_over_C: 2.0`-ı saxlayır, çəkilən `1.999929` isə heç bir
maşın oxunan faylda yoxdur, yalnız sənədlərdədir: yuxarıdakı cədvəl,
`00-docs/01-DECISION-RECORD.md` 6.1 və `00-docs/00-BUILD-RECORD.md` 4.1.

**`GAP / cap = 0.45` təsadüf deyil.** Sözmarkanın öz söz boşluğu `0.45 x cap`-dır
(bölmə 4.2), yəni `2/1`-də nişanla söz arasındakı məsafə sözün `KESTRIDGE` ilə
`AI` arasında işlətdiyi boşluqla **dəqiq eyni** olur. Nişan sətirdəki üçüncü söz
kimi, bərabər ritmdə oturur. `2/1`-dən yuxarı pillələrdə (`7/3`, `8/3`) aralıq
sözün daxili boşluğundan geniş olur və söz nişandan qopur; ona görə `2/1`
mövcud aralıq qaydasının pozulmadan yaşadığı **son pillədir**.

### 5.2 Stacked kilid

| Parametr | Dəyər |
|---|---|
| Cap hündürlüyü | **`C = 2.5c = 6.75u`** - **dəyişmir** |
| Ölçü nisbəti | **`M / C = 8/3 = 2.666667`** - **dəyişmir**; göndərilmiş vektor `2.662690` çəkir (aşağıya bax) |
| Yatıq cap / stacked cap | **`9.0 / 6.75 = 4/3`** (v1-də `13.5 / 6.75 = 2`) |
| Şaquli aralıq | `1.5c = 4.05u`, nişanın ink altından sözmarkanın **cap xəttinə** |
| Sözmarka eni | `wdth 70`, tracking `-14` |

| Sətir | Bütöv ink | AR | Mühərrik bloku | Mühərrik AR |
|---|---|---|---|---|
| **`KESTRIDGE AI`** (default) | **`49.815197u x 28.918076u`** | **`1.7226`** | `49.815197 x 29.045991` | `1.7150` |
| `KESTRIDGE` (ehtiyat) | `39.93914u x 28.918076u` | `1.3811` | `39.93914 x 29.045991` | `1.3750` |

**İki hündürlük var və ikisi də verilir.** `lockup.py`-ın `block_h`-i cap
xəttindən yuxarı çıxan `G` overshoot-unu ikinci dəfə sayır; fərq `0.127915u`.
`ink_box_u` həqiqi mürəkkəb qutusudur, kanvas isə mühərrik bloku ilə qurulur,
yəni fayl ölçüsü v1 ilə eynidir. v1 sənədi yalnız mühərrik rəqəmini verirdi
(`29.05u`, AR `1.715`); o rəqəm səhv deyil, sadəcə tam deyil.

**Stacked-də sözmarka yatıqdakından kiçikdir, nişan isə eynidir.** v1-dəki
"stacked-də nişan sözmarkaya görə dəqiq iki dəfə böyükdür" cümləsi yatıq cap
ilə stacked cap arasındakı `2.000` nisbətinə söykənirdi; `M/C = 2/1` ilə həmin
nisbət `4/3`-dür və cümlə qüvvədən düşür. Qalan qayda budur: **stacked-də
`M / C = 8/3` və bu, tavandır** - `64 px` blokda cap `12.54 px`-dir, döşəmə isə
`12 px`, yəni ehtiyat cəmi `4.5` faizdir.

**Stacked-də elan edilən `2.666667` çəkilmir: fayl `2.662690` çəkir, və bu
v1-dən mirasdır.** `wdth 70` instansiyasında `K` qlifi **`687`** vahid
hündürlükdədir, miqyas isə `686` vahidlik cap dəyərindən hesablanır
(`6.75 / 686 -> scale(0.009840)`), ona görə çəkilən cap `6.750000u` yox,
`6.760080u` olur. Çəkilən ink qutusu `49.816968 x 28.918080u`, AR `1.722693`
(elan edilən `1.722632`). **v2 bunu gətirməyib:** v1-in öz
`archive/v1-2026-08-24/delivery/01-master/kestridge-lockup-stacked-default-positive.svg` faylındakı
transform hərf-hərf eynidir (`translate(2.1391 31.5000) scale(0.009840
-0.009840)`). `wdth 100`-də `K` tam `686` vahiddir, ona görə yatıq kiliddə
yeganə mənbə yuvarlaqlaşdırmadır (5.1).

### 5.3 Yalnız nişan

| Kontekst | Qayda | Kəsim radiusu |
|---|---|---|
| Kvadrat plitə (app icon, favicon) | **ink = `0.75` x plitə kənarı** | kəsim yoxdur |
| **Dairəvi kəsim** (LinkedIn, WhatsApp Business, Teams, avatar) | **ink = `0.70` x plitə kənarı** | `0.50` x plitə eni |
| **PWA maskable ikon** | **ink = `0.56` x plitə kənarı** | `0.40` x plitə eni |
| Minimum təmiz sahə | **`c = 2.7u = 0.15 M`** hər tərəfdən | - |

Üç sətir bir hesabın üç oxunuşudur: nişan nə qədər böyük ola bilər ki, platforma
onu öz kəsim radiusu ilə kəsəndə mürəkkəb itməsin.

**Nisbət necə çıxır.** Nişanın ink qutusu mərkəzindən ən uzaq təpəsi gövdənin
xarici küncüdür (`3, 3`); məsafə `12.727922u`-dur. Bu rəqəm fərz edilmir,
lövhələrin təpələri gəzilib ölçülür (`platformv2.half_diagonal_u`). Plitə
kompozisiyası `18u` ink qutusunu `frac x W` piksellik kvadrata miqyaslayır, ona
görə ən pis ink radiusu belədir:

    r_max = frac x W x (12.727922 / 18) = frac x W x 0.70710678

Kəsim radiusu `radius_share x W`-dirsə, `r_max <= radius_share x W` şərti
nisbətə tavan qoyur:

    frac_tavan = radius_share / 0.70710678

| Kəsim | `radius_share` | Tavan | Qəbul edilən | Tavanın payı |
|---|---|---|---|---|
| Daxili dairə (dairəvi kəsim) | `0.50` | `0.707107` | `0.70` | `0.98995` |
| **Maskable safe zone** | `0.40` | **`0.565685`** | **`0.56`** | `0.98995` |

`0.70 x 0.80 = 0.56`. Maskable spesifikasiyası yalnız mərkəzi `0.80` diametri
zəmanət verir, yəni radius payı `0.50`-dən `0.40`-a düşür; dairəvi nisbəti həmin
əmsala vurmaq tavanın eyni payını saxlayır. Maskable ikon avatarlar qədər
təhlükəsizdir və yeni bir təhlükəsizlik arqumenti qurulmur.

**Kvadrat sətir dəyişmir.** Heç kim kəsmədiyi plitədə `0.75` doğru cavabdır;
səhv o nisbətin özündə deyil, maskable ikonun da həmin sətrə düşməsində idi.

**Ölçülmüş nəticə, `512 x 512` maskable ikonu:**

| Göstərici | `0.75` (əvvəl) | `0.56` (indi) |
|---|---|---|
| ink yarımdiaqonalı | `271.53 px` | `202.5861 px` |
| safe zone radiusu | `204.80 px` | `204.80 px` |
| nisbət | `1.3258` | `0.9892` |
| ehtiyat | `-66.73 px` | `2.2139 px` |
| safe zone xaricində mürəkkəb | `13774.06 px2` = **`21.37%`** | `0.0000 px2` = **`0.00%`** |
| daxili dairədən də kənarda | `726.64 px2` = `1.13%` | `0.0000 px2` |
| göndərilən faylda safe dairəni kəsən piksel | `14335` | `0` |
| board / ofset | `512 px` / `0 px` | `382 px` / `65 px` |

Spesifikasiyanın **"kənar `10` faiz kəsilə bilər"** ifadəsi plitənin kənar
zolağını təsvir edir, itəcək mürəkkəbin payını yox. Burada pay `21.37` faiz idi,
yəni zolağın iki qatı. Köhnə MANIFEST həmin ifadəni hökm kimi yazmışdı; indi
hökm ölçülən paydır.

v1 eyni qüsurdadır: onun göndərilmiş eyni faylında alfa kəsim `12823.1 px2` (v2 ölçməsində `12850.6 px2`),
mürəkkəbin `21.02` faizini aparır. (Audit qeydi v1 üçün `20.87`, v2 üçün `21.25`
faiz yazmışdı; bu paketin öz ölçməsi `21.02` və `21.37` faiz verir. Qüsurun
varlığı dəyişmir, rəqəm dəqiqləşir.) v1-in səhv olması onu yenidən göndərmək
üçün əsas deyil.

**Sübut:** `qa/07-platforms/check-maskable-safezone-veb-standartlari-pwa-maskable-ikon-minimum-olchu-512x512.png`
göndərilən faylın üzərinə safe dairəni çəkir,
`qa/07-platforms/check-maskable-evvel-indi-veb-standartlari-pwa-maskable-ikon-minimum-olchu-512x512.png`
isə solda `0.75`, sağda `0.56` variantını yan-yana qoyur. (v2-də bu şəkillər
paketin içində, `07-platform/_check/` altında `_check-` önəki ilə idi.)

**`0.56` qaydasının döşəməsi.** Nominal ehtiyat `(0.40 - 0.56 x sqrt(2)/2) x W
= 0.004020 x W px`-dir, `512 px`-də `2.06 px`. Tam ədədli `inner` və mərkəzləmə
bu ehtiyatı kiçik plitədə yeyib qurtarır: `16..1024` aralığında `87` ölçü kəsir,
ən pis aşma `0.7924 px`, sonuncu kəsən ölçü `219 px`. **`220 px`-dən yuxarı hər
ölçü təhlükəsizdir**; göndərilən yeganə maskable ölçü `512`-dir və siyahıda
yoxdur. `512`-dən kiçik maskable ikon istənərsə ölçü əvvəlcə taramadan
keçirilməlidir (`platformv2.headroom_scan`).

**Dairəvi qayda məcburidir və v2-də dəyişmir**, çünki ink qutusu dəyişməyib:
yarımdiaqonal `12.727922u`-dur, öz `24u` plitəsinin daxili dairəsindən (`12.0u`)
böyükdür. `0.70`-də yarımdiaqonal `11.879394u` olur və içəridə qalır -
ölçüldü, kəsmir.

**Dairəvi slotlar, göndərilən piksellərdə.** Əvvəl bu sətirlər yalnız vektor
küncündən yazılırdı; aşağıdakı rəqəmlər v2-nin `07-platform`-undakı faylların
(indi `archive/v2-2026-09-21/delivery/07-platform/`) öz piksellərindən oxunub.

| Fayl | Plitə | Daxili dairə `r` | ink `r` (vektor) | Mərkəz ehtiyatı | Künc ehtiyatı | Alfa kəsimin apardığı |
|---|---|---|---|---|---|---|
| `youtube-profile-picture-kanal-shekli-avatar` | `98` | `49.00 px` | `48.9671 px` | `0.2096 px` | **`-0.4975 px`** | `0.1005 px2` = `0.0049%` |
| `github-personal-account-profile-picture` | `500` | `250.00 px` | `248.3713 px` | `1.8055 px` | `1.0984 px` | `0.0000 px2` |
| `linkedin-shirket-sehifesi-company-page-logo-profile` | `400` | `200.00 px` | `198.5202 px` | `1.3030 px` | `0.5959 px` | `0.0000 px2` |
| `x-profile-photo-avatar` | `400` | `200.00 px` | `198.5202 px` | `1.3030 px` | `0.5959 px` | `0.0000 px2` |

İki ehtiyat iki fərqli sual verir. **Mərkəz ehtiyatı** dairənin ən uzaq mürəkkəb
pikselinin mərkəzinə dəyməsinə nə qədər qaldığını, **künc ehtiyatı** isə həmin
pikselin ən uzaq küncünə dəyməsinə nə qədər qaldığını deyir. Sərhəd pikseli
dairəni kəsdikdə künc ehtiyatı mənfi olur, mürəkkəb itməsə də.

**`98 px` avatar sərhəddədir və rəqəmi belədir.** Bir sərhəd pikseli - `(14, 14)`,
örtük dəyəri `0.3899` - dairəni kəsir: küncü `49.4975 px`-dədir, dairə isə
`49.00 px`. Hökmü vektor verir: bütün mürəkkəb `48.9671 px` radiusunun
içindədir, yəni **kəsilən mürəkkəb sıfırdır**. Alfa ilə kəsən platforma həmin
bir küncü `0.1005 px2` yumşaldır, bu da mürəkkəbin `0.0049` faizidir. Örtük
dəyəri mürəkkəbin piksel içində harada durduğunu demir, ona görə piksel küncü
ölçü kimi yazılır, hökm kimi yox. Qalan üç slotda dairəni kəsən sərhəd pikseli
ümumiyyətlə yoxdur.

**v3 qeydi - rəngli fayllarda eyni ölçmə.** v3-ün dairəvi slotları rənglidir
və eyni ölçmə `qa/07-platforms/MANIFEST.json`-da `checks.circular` altındadır.
Dörd slotun hamısında daxili dairə, vektor ink radiusu, mərkəz və künc
ehtiyatı yuxarıdakı cədvəllə eynidir (`98 px`: `0.2096 px` və `-0.4975 px`).
Fərq yalnız `98 px` slotun sərhəd pikselindədir: örtük dəyəri `0.3907`, alfa
kəsimin apardığı `0.1007 px2`, payı `4.896e-05`. Hökm dəyişmir: kəsilən
mürəkkəb vektorda sıfırdır.

`98 px` dairəvi taramada kəsən ölçülər siyahısında deyil (`0.70` qaydası üçün
sonuncu kəsən ölçü `172 px`), göndərilən dairəvi ölçülərin hamısı - `98`, `400`,
`400`, `500` - təhlükəsizdir.

**Yeni qeyd:** nöqtə ink qutusunun **içindədir** (`[16.275, 3.0] .. [18.3, 5.025]`),
ona görə dairəvi kəsimə əlavə risk gətirmir. Ink qutusunun yuxarı sağ küncü
`(21, 3)` **boş qalır** - yumru və ya dairəvi kəsim ilk olaraq məhz o küncü
yeyir və orada mürəkkəb yoxdur. Bu, qəsdən belədir.

### 5.4 Deskriptor sətri

**`AUTOMATION SECURITY ANALYTICS`**

| Parametr | Dəyər | v1-də nə idi |
|---|---|---|
| Ailə, çəki, en | Archivo 2.001, **`wght 500`**, `wdth 100` | dəyişmir |
| Registr | tam BÖYÜK HƏRF | dəyişmir |
| **Cap hündürlüyü** | **`C_d = 0.30 C = 2.7u = c`** | `0.30 C = 1.5c = 4.05u` |
| **Tracking** | **`+180` pm em** (`0.180em`) | dəyişmir |
| Söz boşluğu | **şriftin öz boşluğu** (`205u`) üstəgəl tracking | dəyişmir |
| Üfüqi düzülmə | sözmarkanın **ink sol kənarı** ilə flush | dəyişmir |
| **Şaquli yer** | baza xətti sözmarka baza xəttindən **`0.70 C = 6.3u`** aşağı | `0.70 C = 3.5c = 9.45u` |
| Kilid + deskriptor ink | **`110.689504u x 19.84723u`**, AR **`5.5771`** | `155.009u x 25.271u`, AR `6.134` |
| Deskriptor ink eni | `88.639504u`, sözmarka `88.566035u` - artıq `+0.083%` | eyni `+0.083%` |

**İnvariant `0.30 C`-dir, `1.5c` deyil.** v1-də bu ikisi eyni ədədi verirdi,
çünki `C = 5c` idi. `M/C = 2/1` ilə ayrılırlar və doğrusu nisbətdir: cap
`4.05u` saxlansaydı deskriptor sözmarkadan **`50.1` faiz uzun** olardı
(`132.959 / 88.566 = 1.5012`) və 5.4-ün flush qaydası ölərdi. `0.30 C` ilə
artıq eni yenə `+0.083` faizdir - dəqiq flush `+179.336` tracking olardı,
`+180` tam ədəd üçün seçilib. Eyni məntiqlə enmə də nisbətlə gedir:
`0.70 C = 6.3u`.

**`AI` deskriptorda yazılmır.** O, kilidin özündədir. **`DATA` də yazılmır**:
söz Kestra-nın `7544165` nömrəli qeydiyyatının mətnindədir.

**Deskriptor kilidin hissəsi deyil.** Kilid faylı yalnız nişan + sözmarkadır.
Deskriptor ayrıca variant faylıdır: **mətni versiyalana bilər, həndəsəsi yox.**

**İşlənmir:**

- deskriptor cap `9 px`-dən kiçik olanda. `C_d = 2.7u` olduğu üçün bu, kilid
  ink eni **`368.7 px`**-dən kiçik olanda baş verir (v1-də `344.2 px` idi) -
  yəni deskriptor variantı `24.5 px` daha gec açılır. Döşəmə qaydası
  dəyişməyib, yalnız onun düşdüyü ölçü dəyişib. **Eyni qapı render
  hündürlüyü ilə: `85 px`** - deskriptor faylının kanvası `25.24723u`-dur,
  `9 px` cap üçün miqyas `3.333333 px/u` lazımdır, yəni kanvas hündürlüyü
  `84.16 px`, tam pikselə yuvarlaqlanmış `85 px`. İki ifadə eyni qapıdır,
  sadəcə biri eni, digəri hündürlüyü ölçür; `qa/01-logo-svg-measurements.json`
  `horizontal-descriptor` sətri `min_floor_height_px: 85` və `floor_ok.64:
  false` verir, `floor_note` isə üç döşəməni (nişan `16 px`, sözmarka cap
  `10 px`, deskriptor cap `9 px`) adlandırır. (v2 mətni burada `floor_basis`
  açarını və üç qapını - nişan `23 px`, sözmarka cap `29 px`, deskriptor cap
  `85 px` - göstərirdi; 2026-09-26-da yoxlandı, faylda `floor_basis` açarı
  yoxdur);
- **stacked kiliddə** - eyni nisbətdə flush üçün tracking mənfi çıxır;
- yalnız-nişan halında (app icon, favicon, avatar).

---

## 6. Reduktiv davranış

| Ölçü | Kəsik | Nə olur |
|---|---|---|
| 512 px | əsas | tam spesifikasiya, fasələr və örtük qərar kimi oxunur |
| 128 px | əsas | fasələr görünür, `Ai` tam oxunur |
| 48 px | əsas | fasələr optik yox olur - zərərsiz, onlar istehsal xüsusiyyətidir; örtük hələ aydındır |
| 32 px | əsas | `i` oxunuşunun etibarlı alt həddi |
| **24 px** | əsas | **təmiz döşəmə** - beş lövhə hər həddə ayrı qalır; **reduktiv kəsiyə keçid həddi** |
| 16 px | **reduktiv** | nöqtə və tir çəkilmir, `K` təmiz oxunur |

**Nişanın döşəməsi 16 px-dir.** Bundan aşağı işlədilmir.

**`24 px`-dən aşağıda nişan reduktiv kəsiklə gedir.** Nöqtə (`tittle`) və tir
(`crossbar`) çəkilmir; `stem`, `arm`, `leg` əsas kəsiyin təpələrini hərfi
mənada saxlayır, ona görə siluet həddin hər iki tərəfində kəsilməz qalır.
Qayda `tools/build/mark.py` `cut_for(px)`-dədir, həd `REDUCED_FLOOR_PX = 24`
(v2-də `markv2.cut_for`, `markv2.REDUCED_FLOOR_PX`).

**v3: reduktiv kəsik də iki rənglidir.** Qalan üç lövhədən gövdə mürəkkəbdə,
qol və ayaq petroldadır, ona görə `16 px`-də də bölgü görünür (bölmə 3.0.2).

**Əvvəlki sətir səhv idi və düzəldilir, silinmir.** Bu cədvəl `16 px` üçün
"əsas kəsik, örtük boz zolağa çevrilir" deyirdi. Ölçmə göstərdi ki, həmin boz
zolaq zərərsiz deyil: tir gövdəni ayağa bağlayır və `25%` həddində bütöv nişan
**iki** komponentə axır - çılpaq `K`-nın **üçündən az**. Tam cədvəl, protokol və
yenidən qurulan fayllar: **`00-docs/06-REDUCED-CUT.md`** (v2-də paketin kökündə `REDUCED-CUT.md` idi).

### 6.0 Örtüyün solğunlaşması - ölçülmüş

Bu, v2-nin **yeni** davranışıdır və gizlədilmir: `Ai` örtüyü **24 px və yuxarı
xüsusiyyətdir**.

**Düzəliş.** Bu bölmə əvvəl "16 px-də örtük boz zolağa dönür, amma `K` silueti
korlanmır" deyirdi. Birinci hissə doğrudur, ikincisi yox: boz zolaq tirin
gövdə ilə ayaq arasında qurduğu körpüdür və `25%` həddində siluetin öz kanalını
bağlayır (aşağıdakı cədvəlin "16 px, 25% hədd" sətri bunu onsuz da göstərirdi -
nəticə çıxarılmamışdı). İndi örtük `24 px`-dən aşağı **çəkilmir**, ona görə
cümlə qüvvədən düşür: 16 px-də boz zolaq yoxdur, çünki tir yoxdur.
Bax **`00-docs/06-REDUCED-CUT.md`**.

Ölçmə protokolu: `tools/build/proof.py`-ın öz `draw_plates` yolu (`fit = 0.75`, miqyas
nişanın ink qutusundan, `SS = 8` supersempl, **LANCZOS** resolve), 16-dan
64 px-ə qədər hər tam ölçü, hər lövhə cütü ayrıca, 50% və 25% hədləri ilə,
4-qonşu və 8-qonşu əlaqəliliklə (`scipy.ndimage.label`).

| Nəticə | Ölçülən |
|---|---|
| 50% hədd, 4-qonşu, 16..64 px | **hər ölçüdə beş ayrı komponent** - heç bir lövhə birləşmir |
| 50% hədd, 8-qonşu | `leg\|crossbar` 17, 18, 23 px - **yeganə künc toxunuşu budur**. `arm\|tittle` heç bir ölçüdə birləşmir |
| 50% hədd, 8-qonşu, bütöv nişan | 17, 18 və 23 px-də **dörd komponent**, qalan hər ölçüdə beş |
| 25% (boz) hədd, 4-qonşu | `arm\|tittle` 16, 17, 18, 21 px; `stem\|crossbar` 16, 18, 19, 22, 25 px; `leg\|crossbar` 16 px |
| 16 px, 25% hədd | bütöv nişan iki komponentə axır |
| **K-nın öz üç kanalı** | **heç bir ölçüdə, heç bir həddə birləşmir** (`stem\|arm`, `stem\|leg`, `arm\|leg` - dörd ölçmənin hamısında təmiz) |
| Nöqtənin eni 16 px-də | `1.35` cihaz pikseli |
| Tirin qalınlığı 16 px-də | `1.35` cihaz pikseli |

**Bu cədvəl bir dəfə səhv düzəldilmişdi və geri qaytarılıb.** Aralıq
auditdə `arm|tittle` sətri "17 px-də birləşir" kimi yazılmış, bütöv nişanın
17 px sətri də ona uyğun "üç komponent"ə endirilmişdi. Hər iki rəqəm
**BOX** filtrindən gəlir, halbuki bu bölmənin elan etdiyi protokol
**LANCZOS**-dur. Namizədin ilkin yazdığı "heç vaxt" düzgün imiş.

Müstəqil təkrar ölçmə diskdədir: **`qa/raster-merge-ground-truth.txt`**
(v2-də paketin kökündə `RASTER-MERGE-GROUND-TRUTH.txt` idi) - eyni
skript hər iki filtri yan-yana işlədir, 16..64 px, hər cüt, hər dörd
hədd/qonşuluq kombinasiyası.

| Nə | LANCZOS (elan olunan protokol) | BOX (`mark.raster` yolu, `tools/build/mark.py`; v2-də `markv2.raster`) |
|---|---|---|
| `arm\|tittle`, 50% / 8-qonşu | **heç vaxt** | 17, 21 px |
| `leg\|crossbar`, 50% / 8-qonşu | **17, 18, 23 px** | 17, 18, 19, 23 px |
| `stem\|crossbar`, 50% / 4-qonşu | **heç vaxt** | 19 px |
| bütöv nişan, 50% / 8-qonşu | **17, 18, 23 px-də dörd** | 17 px-də üç, 19 px-də üç |
| `arm\|tittle`, 25% / 4-qonşu | **16, 17, 18, 21 px** | 16, 17, 18, 20, 21, 22, 25 px |

İki filtr iki fərqli cədvəl verir və fərq kiçik deyil - BOX hər ölçüdə daha
erkən birləşdirir. Bu sənədin cədvəli **LANCZOS** cədvəlidir, çünki elan
olunan ölçmə protokolu `tools/build/proof.py`-ındır. Rəqəmi təkrar yoxlayan kəs eyni
filtri işlətməlidir; başqa filtr işlədirsə, aldığı fərq qüsur deyil.

**`00-docs/06-REDUCED-CUT.md` BOX rəqəmləri ilə işləyir** və bunu öz içində yazır -
orada `mark.raster` yolu (v2-də `markv2.raster`) ölçülür, çünki kəsik həddi məhz həmin yolla
render olunan aktivlərə tətbiq olunur. İki sənədin rəqəmləri fərqlidir və
bu, ziddiyyət deyil: fərqli filtrlərin ölçüləridir. Hər iki nəticə `24 px`
həddini eyni verir.

Qayda bir cümlə ilə: **örtük itə bilər, siluet korlana bilməz.**

**Bu ölçmələr harness-dən kənardır.** `tools/build/proof.py` (v2-də
`v2-ai/tools/proof.py`) yalnız həndəsə
ölçür; onun `pass: true` sətri raster haqqında heç nə demir. Yuxarıdakılar
namizədin öz taramasıdır və v2 namizədinin `f2-strict-system/NOTES.md` bölmə
6-sındadır (git tarixçəsində, bu sənədin əvvəlinə bax).

### 6.1 Sıx kəsik geri götürüldü - bir master var

**Bu qərar v2-də də qüvvədədir.** Nişanın 16 px döşəməsinə qədər yeganə
**həndəsəsi** əsas kəsikdir - `w = 4.0`, `a = 5.0`, eyni təpələr.

**Reduktiv kəsik bu qərarı pozmur.** O, ikinci master deyil: `CUTS`-da yeni
sətir açmır, `w` və `a` dəyişmir, təpələri yenidən çəkmir. `tools/build/mark.py` `CONCEPTS`-də (v2-də `markv2.CONCEPTS`)
`reduced` həndəsəsini birbaşa `main`-dən alır və yalnız iki örtük lövhəsini
çəkmir. Sıx kəsiyi öldürən hər iki səbəb (piksel şəbəkəsi və parlaqlıq)
reduktiv kəsiyə aid deyil, çünki ink qutusu eyni `3u ... 21u` qalır.

**Birinci səbəb: sıx kəsik piksel şəbəkəsi düzülüşünü pozurdu.** Əsas kəsiyin
qutusu `3u ... 21u`, yəni lövhənin `1/8` və `7/8`-idir və 16 / 32 / 48 / 128 /
512 px-də **tam piksel sərhədinə** düşür. Sıx kəsiyin qutusu `18.9u`-dur və
heç bir ölçüdə düşmür.

**İkinci səbəb: rəng qərarı solğunluq problemini onsuz da həll etdi.**

Ölçülmüş parlaqlıq (eyni PIL yolu, eyni fit):

| px | v2 tam kəsik | v2 **göndərilən** kəsik | v1 göndərilmiş |
|---|---|---|---|
| 16 | `0.7399` | **`0.7597`** (reduktiv) | `0.7535` |
| 128 | `0.7547` | `0.7547` (əsas) | `0.7666` |

**Bu sətir düzəldilib.** Cədvəl əvvəl yalnız iki sütun idi və "yeni nişan kiçik
ölçüdə v1-dən **tündür**" deyirdi. `128 px`-də bu hələ də doğrudur. `16 px`-də
isə artıq reduktiv kəsik göndərilir, onun parlaqlığı `0.7597`-dir, yəni v1-dən
`0.0062` **açıqdır** - iddia orada qüvvədən düşür və saxlanmır.

**Sıx kəsiyin geri götürülməsi buna baxmayaraq qüvvədədir:** sıx kəsiyin
mövcudluq səbəbi `0.781`-ə qədər açılan parlaqlıq idi; `0.7597` ondan hələ də
`0.0213` tünddür. Yəni ikinci master üçün səbəb yenə yoxdur.

**Chromium ilə təkrar ölçmə hələ edilməyib.** Yuxarıdakı iki sütun PIL
rəqəmidir. v1 sənədindəki `0.7703` / `0.7497` cütü isə **iki mühərrik deyil,
iki kəsikdir**: eyni Chromium ölçməsində (`device_scale_factor = 1`, 16 px)
v1-in **əsas** kəsiyi `0.7703`, **sıx** kəsiyi `0.7497` verir. Sıx kəsik geri
götürülüb, ona görə `0.7497` yalnız arxiv rəqəmidir. Mənbə:
`docs/concept/renders/_build/browsercheck.json`.

Həmin faylda hər ölçünün PIL qarşılığı da var, yəni **Chromium ilə PIL
arasındakı fərq v1-də ölçülüb**: səkkiz cütdə `0.0015` ilə `0.0070`
arasındadır (16 px əsas `0.0059`, 16 px sıx `0.0070`, 32 px əsas `0.0015`,
48 px əsas `0.0050`). Ona görə "fərq `0.007`-dən kiçik idi" ifadəsi v1-i
olduğundan yaxşı göstərir: doğru ifadə **intervaldır**, və ən böyük fərq məhz
`16 px`-dədir - qərarın asıldığı ölçüdə. Qərarın dəyişmə ehtimalı yenə
aşağıdır, amma bu, v2 üçün hələ də **ölçülməmiş** sətirdir.

**Sınanmamış qalan:** Windows tapşırıq paneli və macOS dock.

### 6.2 Kiçik ölçü döşəmələri - ölçülmüş

| Nə | Döşəmə |
|---|---|
| Nişan (bütün hallarda) | **`16 px`** ink hündürlüyü |
| Sözmarka `wdth 100` | cap **`>= 10 px`** |
| Sözmarka `wdth 70` | cap **`>= 12 px`** |
| **Stacked kilid** | **`64 px`** bütöv blok |
| Deskriptor | cap **`>= 9 px`** |

Döşəmələrin özü **dəyişməyib**. Dəyişən odur ki, yatıq kiliddə hansının
bağlayıcı olduğudur.

**Yatıq kiliddə bağlayıcı şərt indi sözmarkanın cap döşəməsidir, nişanın öz
döşəməsi yox.** `M / C = 2/1`-də nişan sözmarkadan iki dəfə böyükdür, ona görə
cap `10 px`-ə çatanda nişan artıq `20 px`-dir - öz `16 px` döşəməsindən yuxarı.
Tərs istiqamətdə: nişan `16 px` olanda cap cəmi `8.0 px` olur, yəni döşəmədən
aşağı. Bağlayan qapı cap-dır.

| | v1 (`4/3`) | **v2 (`2/1`)** |
|---|---|---|
| ən kiçik icazəli kilid ink eni | `137.69 px` | **`122.91 px`** |
| orada nişan | `16.0 px` | **`20.0 px`** |
| orada cap | `12.0 px` | `10.0 px` |
| **bağlayıcı döşəmə** | **nişan** | **sözmarka cap** |
| render hündürlüyü döşəməsi (pad daxil) | `20.8 px` | **`26 px`** |

**Kilidin ən kiçik ayaq izi böyümür, kiçilir** (`11` faiz), üstəlik həmin
nöqtədə nişan `16` yox, `20 px` olur. Yəni kiçik ölçüdə vəziyyət yaxşılaşır.

Hər iki qapı `tools/build/lockup.py` `floor_ok()`-də yoxlanılır və funksiya
onsuz da **ikisini də** yoxlayırdı, ona görə məntiqdə düzəliş lazım deyil -
yalnız `CAP_H` sabiti dəyişir. `qa/01-logo-svg-measurements.json` (v2-də
`01-master/MEASUREMENTS.json`) hər kilid üçün
`floor_ok` sətrini 64 / 128 / 256 / 512 / 1024 px-də verir.

**Bir istisna var və o, `floor_ok()`-in öz məhdudiyyətidir: deskriptor
variantı.** Funksiya hər yatıq kilid üçün blok hündürlüyünü
`2c + max(M, CAP_H) = 23.4u` götürür və deskriptorun `9 px` cap qapısını
**heç yoxlamır**. Deskriptor faylının kanvası isə `25.24723u`-dur, üstəlik
onun öz cap döşəməsi var. Düz hesablandıqda həmin variantın döşəməsi
`26 px` yox, **`85 px`**-dir və `64 px`-də keçmir. `qa/01-logo-svg-measurements.json`
bu sətri düzəldilmiş halda saxlayır (`floor_note`, `min_floor_height_px:
85`, `floor_ok.64: false`; v2 mətnindəki `floor_basis` açarı faylda yoxdur);
qalan dörd kilid `64 px`-dən yuxarı hər ölçüdə
keçir. v2-də `tools/` oxunur-yazılmırdı, ona görə funksiyanın özü dəyişmədi -
düzəliş çağırış tərəfindədir (`tools/build/masters.py`, bölmə 10 bənd 18); v3-də də `tools/build/lockup.py` `floor_ok()` deskriptor qapısını yoxlamır.

---

## 7. Fiziki istehsal

| Proses | Qayda |
|---|---|
| Tikmə, trafaret, oyma möhür | **yalnız tərs (knockout) kəsik** |
| Qravür, folqa, vinil kəsim | istənilən kəsik - ən kiçik künc `90` dərəcədir |

Ayrılmış lövhələr bərk sahədə **deşik** kimi çəkiləndə fiziki dayaq problemi
qalxmır və körpü lazım gəlmir. Körpü əlavə etmək konsepti pozur.

**Knockout avtomatik invertdir.** Eyni **beş** kontur, eyni koordinatlar,
yalnız mürəkkəb rəngi `#0F1317` -> `#FAFAF7`. Buna baxmayaraq **həmişə ayrıca
master fayl kimi yazılır** - istehsal tərəfi rəngi özü çevirməməlidir.

`DESIGN-LANGUAGE.md` 4.5 tərs marka üçün `6.25` faizlik ştrix nazilməsi tələb
edir. **Bu nişana adlı istisna ilə tətbiq olunmur.** Səbəb həndəsidir: nazilmə
mürəkkəb qutusunu `17.4375 x 18.0000` edir (kvadrat deyil), bağlanma eyniliyini
`7.65`-dən `7.0875`-ə endirir, və qutu kənarını 16 / 32 / 48 / 128 px-də tam
pikseldən çıxarır. Üstəlik `k = c/4` qanunu saxlanılsa `w + a = 9` çıxır, yəni
nazilmə üçün üçüncü şəbəkə-qanuni yol yoxdur.

**v2-də bir səbəb də əlavə olunur:** örtük lövhələri onsuz da strukturdan
nazikdir (`2.025` qarşı `4.000`), ona görə bərabər faizli nazilmə onlara ən
ağır dəyir və `16 px` döşəmə iddiası **yenidən ölçülməli** olardı. Ölçülməyib,
çünki istisna qüvvədədir.

Fiqur/zəmin çevrilməsi də mümkün deyil: üç struktur boşluğu xarici sahənin bir
bağlantılı hissəsidir; nöqtə və tirin ətrafındakı `1.35` kanallar da həmin
sahəyə açılır.

**İlk istehsal partiyasında yoxlanılmalıdır:** tərs kəsiyin tikmədə davranışı,
kiçik ölçüdə çapda şevron oxunuşunun dayandığı, **və örtük lövhələrinin
tikmədə sağ qalıb-qalmadığı** - `2.025u` tirin fiziki ölçüsü tikmə iynəsinin
həddinə yaxın ola bilər.

---

## 8. Fayl indeksi

v2 mətni burada v2 paketini belə təsvir edirdi (`v2-ai/delivery/`, 2026-09-21):
doqquz qovluq, səkkiz kök faylı, cəmi `202` fayl; v1-in `173` faylının
hamısının qarşılığı var idi - dördü adı dəyişmişdi (`-V2` şəkilçisi). Həmin
paketin göndərilən surəti `archive/v2-2026-09-21/delivery/`-dədir
(`MANIFEST.json` ilə; orada dörd sənəd `-V2` şəkilçisizdir).

**v3 paketi (2026-09-26):** on qovluq, `delivery/MANIFEST.json`-da `272`
fayl, üstəgəl `README.md` və `MANIFEST.json`-un özü. `delivery/`-i yalnız
`tools/build/pack.py` yazır - `00-docs/` istisnadır, orada sənədlər əl ilə
yazılır (`06-REDUCED-CUT.md` generasiya olunur). Kanonik siyahı və hər faylın
sha256-sı `delivery/MANIFEST.json`-dadır; aşağıdakı ağac onunla eynidir.
Qovluqlar istifadə sırası ilə nömrələnib, rəngli və tək rəngli fayllar
`color/` və `mono/` alt qovluqlarına ayrılıb.

```
delivery/                                 272 fayl + README.md + MANIFEST.json
  README.md                  paketə giriş, "tez tapmaq üçün" cədvəli
  MANIFEST.json              kanonik fayl indeksi, hər fayl üçün sha256

  00-docs/              7    sənədlər
                             00-BUILD-RECORD.md        build qeydi
                             01-DECISION-RECORD.md     qərar reyestri
                             02-BRAND-GUIDELINES.md    bu sənəd
                             03-SITE-NOTES.md          sayt tətbiqi üçün qeydlər
                             04-PLATFORM-NOTES.md      platforma dəsti qeydləri
                             05-BANNER-NOTES.md        LinkedIn banneri qeydləri
                             06-REDUCED-CUT.md         kiçik ölçü kəsiyi (generasiya olunur)

  01-logo-svg/         34    master SVG
    color/             12    kestridge-mark-{color,color-knockout}.svg
                             kestridge-lockup-{horizontal,stacked}-{default,reserve}-{color,color-knockout}.svg
                             kestridge-lockup-horizontal-descriptor-{color,color-knockout}.svg
    mono/              22    kestridge-mark-{positive,knockout,mono-black,mono-white}.svg
                             kestridge-lockup-{horizontal,stacked}-{default,reserve}-{positive,knockout,mono-black,mono-white}.svg
                             kestridge-lockup-horizontal-descriptor-{positive,knockout}.svg

  02-logo-png/        153    nişan PNG (alfa kanallı), kilid PNG/JPG/WEBP (açıq fonda)
    color/             82    kestridge-mark-color-<px>.png
                             kestridge-mark-color-knockout-<px>.png
                               px: 16, 24, 32, 48, 64, 128, 180, 192, 256, 512, 1024
                             kestridge-lockup-{horizontal,stacked}-{default,reserve}-color-<h>.{png,jpg,webp}
                               h: 64, 128, 256, 512, 1024
    mono/              71    kestridge-mark-positive-<px>.png          (eyni 11 ölçü)
                             kestridge-lockup-{horizontal,stacked}-{default,reserve}-positive-<h>.{png,jpg,webp}

  03-app-icons/         7    favicon.ico (7 freym: 16, 24, 32, 48, 64, 128, 256)
                             kestridge-tile-{square,circle}-{192,512,1024}.svg

  04-logo-pdf/         10    vektor PDF
    color/              5    kestridge-mark-color.pdf
                             kestridge-lockup-{horizontal,stacked}-{default,reserve}-color.pdf
    mono/               5    kestridge-mark-positive.pdf
                             kestridge-lockup-{horizontal,stacked}-{default,reserve}-positive.pdf

  05-stationery/        9    business-card-{front,back}.png, letterhead-p1.png,
                             envelope-10-front.png, slide-{title,content,divider}-hd.png,
                             email-sig-logo-{1x,2x}.png

  06-fonts/             6    {Archivo,Inter,JetBrainsMono}-Variable.ttf
                             OFL-{Archivo,Inter,JetBrainsMono}.txt

  07-platforms/        35    github-*, linkedin-shirket-sehifesi-*, x-*, youtube-*,
                             veb-standartlari-* (OG, favicon, PWA, apple-touch)

  08-website/           9    header-lockup.svg, header-lockup-dark.svg, icon.svg,
                             icon-light.svg, apple-icon.png, favicon.ico,
                             opengraph-image.png, opengraph-image-dark.png,
                             kestridge-tokens.css

  09-linkedin-banner/   2    kestridge-banner-linkedin-cover-1512x256.png
                             kestridge-banner-linkedin-profile-background-1584x396.png
```

**Paketdə olmayanlar:**

| Nə | Harada |
|---|---|
| Ölçmə faylları və yoxlama şəkilləri (paketin iddialarının sübutu) | `qa/` - bax `qa/README.md` |
| v1 paketi və generatorları | `archive/v1-2026-08-24/` |
| v2 paketi (202 fayl, 2026-09-21-də göndərilənin bayt surəti) və v2 `BRIEF.md` | `archive/v2-2026-09-21/` |
| Araşdırma sənədləri | `docs/concept/`, `docs/domain/`, `docs/narrative/`, `docs/research/` |
| Generator | `tools/build/` (giriş nöqtəsi `pack.py`), girişləri `tools/data/` |

**Yoxlama şəkilləri artıq paketin içində deyil.** v2-də `_check` ilə başlayan
şəkillər qovluqların içində idi və manifestdə göndəriş olmadıqları
işarələnirdi; müştəriyə gedən dəstə yığılanda çıxarılmalı idilər. v3-də onlar
`qa/`-dadır (`check-` önəki ilə): `qa/07-platforms/`,
`qa/09-linkedin-banner/`, kolleteralın `-guides.png` vərəqləri isə
`qa/05-stationery-guides/`. Ona görə `delivery/`-dəki hər fayl göndərişdir.

**v2 -> v3 qovluq xəritəsi** (köhnə yolu olan qeyd tapılsa, buradan tapılır):

| v2 | v3 |
|---|---|
| `01-master/` | `01-logo-svg/color/` və `01-logo-svg/mono/` |
| `02-raster/` | `02-logo-png/color/` və `02-logo-png/mono/` |
| `03-icons/` | `03-app-icons/` |
| `04-pdf/` | `04-logo-pdf/color/` və `04-logo-pdf/mono/` |
| `05-collateral/` | `05-stationery/` (`-guides.png` vərəqləri `qa/05-stationery-guides/`-a) |
| `06-fonts/` | `06-fonts/` |
| `07-platform/` | `07-platforms/` (`_check/` vərəqləri `qa/07-platforms/`-a) |
| `08-site/` | `08-website/` |
| `09-banner/` | `09-linkedin-banner/` (yoxlama və ölçmələr `qa/09-linkedin-banner/`-a) |
| kökdəki `00-BUILD-RECORD.md` və s. | `00-docs/` |
| `07-platform/NOTES.md` | `00-docs/04-PLATFORM-NOTES.md` |
| `09-banner/NOTES.md` | `00-docs/05-BANNER-NOTES.md` |
| `REDUCED-CUT.md` | `00-docs/06-REDUCED-CUT.md` (generasiya olunur) |
| `REDUCED-CUT.json`, `RASTER-MERGE-GROUND-TRUTH.txt`, bütün `MEASUREMENTS.json` | `qa/` |

**Fayl adları.** Nişan adlarından `-main` düşüb:
`kestridge-mark-main-positive.svg` -> `01-logo-svg/mono/kestridge-mark-positive.svg`;
rəngli: `01-logo-svg/color/kestridge-mark-color.svg`. Mono rasterlər `positive` alıb:
`kestridge-mark-main-128.png` -> `02-logo-png/mono/kestridge-mark-positive-128.png`,
`kestridge-lockup-horizontal-default-512.png` ->
`02-logo-png/mono/kestridge-lockup-horizontal-default-positive-512.png`. Mono
PDF-lər də: `kestridge-lockup-horizontal-default.pdf` ->
`04-logo-pdf/mono/kestridge-lockup-horizontal-default-positive.pdf`. Kilid
SVG-lərinin adı `kestridge-lockup-<kind>-<tag>-<way>.svg`-dir, `<way>` -
`color`, `color-knockout`, `positive`, `knockout`, `mono-black`, `mono-white`.

**Platforma şəkillərində kilidin ölçüsü v3-də düzəldilib.** v2-də
`tools/build/platform.py` yatıq kilidin PNG **qutusunu** kadr hündürlüyünün
`0.30`-u götürürdü. Qutu hər tərəfdən `c` (`2.7u`) təmiz sahə daşıyır, ona
görə nişanın öz ink-i kadrın `0.30 x 18 / 23.4 = 0.2308`-inə (`23.1%`)
düşürdü. Təsdiqlənmiş LinkedIn banneri isə nişanı `84 / 256 = 0.328`-də
(`32.8%`) qoyur. Sahib platforma örtüyünə baxıb kilidi "köhnə nisbət" kimi
oxudu; nisbət əslində hər ikisində `2.0` idi (hərf-hərf ölçülüb: düz hərflər
`K E T R I D A` cap `42 px`, nişan `84 px`; `S` və `G` yalnız dəyirmi hərf
overshoot-una görə `44 px` oxunur). Fərq çərçivədə idi: `1.42x`. Düzəliş:
`HORIZONTAL_BOX = 0.328125 x (18 + 2 x 2.7) / 18 = 0.4266`,
`tools/build/platformv2.py`-da `PLAT.HORIZONTAL_BOX`. Qayda en/hündürlük
nisbəti `>= 1.6` olan platforma şəkillərinə (yatıq kilid kompozisiyaları)
tətbiq olunur. Stacked kompozisiya (`STACKED_BOX 0.34`) və `72%` en tavanı
dəyişmir; tavan daha dar bannerlərdə, məsələn `1200 x 628`-də bağlayır.

### 8.1 LinkedIn `1512 x 256` örtüyü - kanonik fayl birdir

Paketdə bu slotu **iki** fayl doldurur və ikisi ayrı dizayndır. Sahibin
seçimi **tünd b4**-dür, ona görə qayda budur:

| Fayl | Status |
|---|---|
| `09-linkedin-banner/kestridge-banner-linkedin-cover-1512x256.png` | **KANONİK.** LinkedIn şirkət səhifəsinə gedən örtük budur. Tünd `b4-oversize-monogram`: mürəkkəb sahə, sağ kənarla kəsilən nəhəng monoqram, `petrol` paz. v3-də kilid `color-knockout`-dur (nişan `84 px`, cap `42 px`) |
| `07-platforms/linkedin-shirket-sehifesi-company-page-cover-banner-1512x256.png` | **KANONİK DEYİL.** Açıq kağız fonunda mərkəzlənmiş kilidin generik render-idir (v3-də `color`); platforma dəstinin ölçü/kompozisiya nümunəsi kimi qalır, LinkedIn-ə yüklənmir |

Səbəb qarışıqlığın özüdür: eyni ölçü, eyni ad sahəsi, iki fərqli dizayn -
kim birinci tapsa onu yükləyir. Qərar `00-docs/01-DECISION-RECORD.md` D3-dədir
(`b4-oversize-monogram`, sahib). `07-platforms`-dakı fayl silinmir, çünki
platforma dəsti öz daxilində bütövdür; sadəcə **işarələnir**
(`00-docs/04-PLATFORM-NOTES.md`; v2-də `07-platform/NOTES.md` idi). v3-də
qovluq adı özü də bunu deyir: kanonik bannerin qovluğu `09-linkedin-banner/`-dir.

**Bannerin deskriptor sətri v3-də də yazılmır.** Faylda sətrin cap-ı
`12.6 px`-dir, amma LinkedIn örtüyü `3.2:1` kəsimdə ekrana miqyaslayır:
`390 px` telefonda cap `6.0 px` olur, bölmə 5.4-ün `9 px` döşəməsindən aşağı.
Döşəməyə qaldırmaq `1.5x` tələb edir və sətri `45` dərəcəlik tikişin üstünə
`160 px` aparır. Tövsiyə: şüar LinkedIn səhifəsinin öz tagline sahəsinə
yazılsın - o, canlı mətndir və həmişə oxunur. Təfərrüat:
`00-docs/05-BANNER-NOTES.md`.

**v1 və v2 aktivləri eyni materialda yan-yana işlədilmir.** v2 paketi
bütövdür və köhnə nişanı daşıyan heç bir faylı yoxdur; v1 arxivə köçür və
yalnız tarixçə üçün saxlanılır. **v3-də:** v1 `archive/v1-2026-08-24/`-də,
v2 `archive/v2-2026-09-21/`-dədir. v3-ün tək rəngli masterləri, rasterləri və
PDF-ləri v2 arxivi ilə bayt-bayt eynidir, rəngli fayllar isə yenidir.

v2 mətni burada "kanonik siyahı paketin öz manifestində olacaq" deyirdi -
artıq var: `delivery/MANIFEST.json`. `qa/01-logo-svg-measurements.json` (v2-də
`01-master/MEASUREMENTS.json`) hər kilid üçün ölçüləri və `floor_ok`
nəticələrini saxlayır.

**Sıx kəsik paketdə yoxdur** (bölmə 6.1). Bir master var.

**`default`** = hazırda işlədilən sətir = `KESTRIDGE AI`.
**`reserve`** = `KESTRIDGE`.

Kolleteral şablonları (`05-stationery/`) **doldurulmamışdır**: şirkət hələ qeydiyyatdan keçməyib,
ünvan və telefon yoxdur, ona görə kontakt sətirləri uydurulmayıb.

---

## 9. Qadağalar - güzəştsiz

1. **Nişan güzgüyə salınmır.**
2. **Ad qısaldılmır** - `Kestr`, `KAI`, `Kest`, `K.` yoxdur.
3. **Yalnız nişan variantı** heç vaxt `K`, `KAI`, `KAi`, `Kestr` mətn forması
   ilə yan-yana görünmür.
4. **Qradiyent, kölgə, parıltı, ulduzcuq yoxdur.** AI ulduzcuğu qadağandır -
   ölçmə göstərir ki, onu `17` faiz AI kimi, `73` faiz "sevimlilərə əlavə et"
   kimi oxuyur. **Nişanın içində `Ai` olması bu qadağanı yumşaltmır.**
5. **Yırtıcı quş silueti işlədilmir.** CrowdStrike "the falcon logo" iddia edir.
6. **Aralıq sistemi iki pillədir və pillələr dəyişmir.**

   | Pillə | Dəyər | Üzvləri |
   |---|---|---|
   | struktur | `4k = c = 2.7` | `stem\|arm`, `stem\|leg`, `arm\|leg` |
   | örtük | `2k = 1.35` | `arm\|tittle`, `stem\|crossbar`, `leg\|crossbar` |

   Üçüncü **lövhə-lövhə** dəyər yoxdur və yaradılmır: hər pillənin əhalisi
   **üçdür**, bir nəfərlik pillə qadağandır. `c` nişanın kök parametridir -
   onu dəyişmək markanı dəyişməkdir; `k = c/4`-ü dəyişmək isə hər iki pilləni
   eyni anda dəyişir. `leg|crossbar` aralığı **perpendikulyar** ölçülür (üfüqi
   ofset `2k * sqrt(2) = 1.909188`); onu üfüqi `1.35` kimi qurmaq elan
   edilməmiş üçüncü dəyər (`0.9546`) yaradır və 16 px-də qaynaq verir.

   **İstisna - tirin baseline-dan hündürlüyü `3k = 2.025`.** Nişanın içində
   iki pillənin heç birinə düşməyən bir ağ məsafə var: tirin alt üzü
   (`y = 18.975`) ilə baseline (`y = 21`) arasındakı zolaq. Bu, iki lövhə
   arasındakı aralıq deyil - tirin altında lövhə yoxdur - ona görə yuxarıdakı
   qadağanı pozmur; amma gözlə baxanda o da ağ məsafədir və nə `2.7`-dir, nə
   `1.35`. Sistemdə onun adı **örtük modulu**dur (`m = 3k`, bölmə 2.2.1) və
   `tools/build/mark.py` (v2-də `markv2.py`) onu `ortuyun tek olchusu: tir b` sətri ilə ölçür. Yəni qayda
   belə oxunur: **aralıq iki dəyər alır, modul isə üçüncü bir dəyərdir və
   aralıq deyil.** Spesifikasiya bunu əvvəldən elan edirdi, qadağa bəndi isə
   susurdu; indi susmur.

   v1-dəki "`c` dəyişmir. Aralıq nişanın yeganə parametridir" cümləsi bu
   bəndlə **əvəz olunur**, silinmir: `c` yenə dəyişmir, sadəcə yeganə aralıq
   deyil.
7. **Künc radiusu `0` qalır.**
8. **"Bizim bucağımız" formulası işlədilmir.** Bizimki sabit **aralıq**dır -
   indi iki pilləli, amma yenə aralıq.
9. **Etimologiya danışılmır.** Heç bir materialda "adımız ... deməkdir"
   cümləsi yazılmır.
10. **"The kestrel ... edir" tipli cümlə qurulmur.** Ya cins səviyyəsində
    danışılır (`kestrels hover`), ya növ **adlandırılır**.
11. **Silsilə xətti (dağ, zirvə) vizual mənbə kimi işlədilmir.**
12. **Nişan beş lövhədir və lövhə silinmir.** Kiçik ölçüdə nöqtə və tir optik
    olaraq solğunlaşa bilər (bölmə 6.0), amma **fayl həmişə beş konturu
    daşıyır**. "Kiçik ölçü üçün sadələşdirilmiş üç lövhəli variant"
    hazırlanmır: bu, ikinci master deməkdir və 6.1 onu bağlayıb. Örtük
    lövhələri ayrıca rəngə də boyanmır (bölmə 3.1).

    **Son cümlə v3-də əvəz olunub (2026-09-26), bax bölmə 3.0.** Nöqtə və tir
    indi petroldur, amma yalnız bütöv `Ai` qrupunun (qol, nöqtə, ayaq, tir)
    hissəsi kimi - heç vaxt təkbaşına, heç vaxt qrupun qalan lövhələrindən
    ayrı rəngdə. Bəndin qalanı dəyişmir, bir dəqiqləşdirmə ilə: "fayl həmişə
    beş konturu daşıyır" cümləsi `24 px` və yuxarı ölçülərə aiddir - ondan
    aşağıda fayl reduktiv kəsiklə üç lövhə (`stem`, `arm`, `leg`) daşıyır
    (bölmə 6, `01-DECISION-RECORD.md` D6); bu, ikinci master deyil (6.1).
13. **Nişanın rəng bölgüsü yenidən rənglənmir** (v3, 2026-09-26). Dəqiq iki
    rəng, hərf qrupu üzrə: `K` (gövdə) və `Ai` (qol, nöqtə, ayaq, tir), heç
    vaxt lövhə-lövhə. Açıq fonda petrol-600-dən başqa petrol pilləsi yoxdur,
    petroldan başqa çalar yoxdur; tünd fonda yalnız `color-knockout`. Bölmə 3.0.5.

---

## 10. Açıq maddələr

| # | Nə | Status |
|---|---|---|
| 1 | Kilid qapıları U1, U2 | **bağlandı** 2026-08-24 - bölmə 1.1 |
| 2 | Sıx kəsiyin statusu | **bağlandı** - geri götürüldü, bölmə 6.1 |
| 3 | Deskriptor sətrinin sözləri | **bağlandı** - bölmə 5.4 |
| 4 | Knockout master ziddiyyəti | **bağlandı** - bölmə 7 |
| 5 | `wdth 70` kiçik ölçü döşəməsi | **bağlandı** - bölmə 6.2 |
| 6 | Nişanın `Ai` qatı | **bağlandı** - `f2-strict-system`, `01-DECISION-RECORD.md` D1 |
| 7 | Kilid nisbəti | **bağlandı** - `M/C = 2/1`, D2 |
| 8 | Deskriptorun `0.30 C` invariantı | **bağlandı** - D5, bölmə 5.4 |
| 9 | `tools/build/lockup.py` `CAP_H` sabiti | **bağlandı** (v3-də yoxlanıb) - faylda indi `CAP_H = 10 * C / 3` (`9.0u`, v2 dəyəri); v2-də "açıq - v2 paketində çağırış yerində dəyişdirilir; v1 əvəz olunanda faylda bir sətir dəyişməlidir" idi. v1 cap-ı lazım olan builder-lər onu ayrıca `CAP_H_V1 = 5c = 13.5u` kimi saxlayır (`masters.py`, `collateralv2.py`, `platformv2.py`, `icons_pdf.py`, `manifest.py` - hamısı `tools/build/`-da) |
| 10 | `tools/build/mark.py`-ın `markv2.py` ilə əvəz olunması | **bağlandı** - v2 təşviqindən sonra `tools/build/mark.py` v2 generatorudur; `v2-ai/build/markv2.py` ikinci surət idi və v3-də silindi. Bir generator var. v2-də "açıq - sahib təsdiqindən sonra" idi |
| 11 | Chromium ilə 16 / 32 px parlaqlıq təkrar ölçməsi | **edilməyib** - bölmə 6.1 |
| 12 | Raster birləşmə taramasının `tools/build/proof.py`-a sərt qapı kimi əlavəsi | **tövsiyə**, edilməyib |
| 13 | Dairəvi avatarda yeni nişanın gözlə təsdiqi | **edilməyib** - bölmə 5.3 |
| 14 | `03-icons`, `04-pdf`, `05-collateral`, `06-fonts`, `08-site`, `MANIFEST.json` (v2 adları; v3-də `03-app-icons`, `04-logo-pdf`, `05-stationery`, `06-fonts`, `08-website`, `delivery/MANIFEST.json`) | **bağlandı** - hamısı qurulub, bölmə 8 |
| 15 | PWA maskable ikonunun kəsməsi | **bağlandı** - `0.56` nisbəti, safe zone xaricində `0.00%`, bölmə 5.3 |
| 16 | v1 paketindəki (v1-də `delivery/07-platform`, indi `archive/v1-2026-08-24/delivery/07-platform`) maskable ikon | **bağlandı** - v1 arxivə köçür, v2 həmin slotu düzgün nisbətlə doldurur |
| 17 | LinkedIn `1512 x 256` örtüyünün kanonik faylı | **bağlandı** - `09-linkedin-banner` (v2-də `09-banner`; tünd b4), bölmə 8.1 |
| 18 | `masters.py` deskriptor döşəməsini `floor_ok()` ilə hesablayırdı (düz kilidin `23.4u` bloku, deskriptor qapısı yoxdur) | **bağlandı** - generator düzəldilib, ölçmə faylı (`qa/01-logo-svg-measurements.json`) artıq əl ilə redaktə olunmur |
| 19 | 6.0 raster cədvəlinin filtr asılılığı | **bağlandı** - `qa/raster-merge-ground-truth.txt` (v2-də `RASTER-MERGE-GROUND-TRUTH.txt`), LANCZOS və BOX yan-yana ölçülüb |
| 20 | Stacked kilidin `M/C = 2.6629` çəkməsi (nominal `8/3 = 2.6667`) | **açıq, v1-dən miras** - səbəb `wdth 70` instansiyasının `687` vahidlik düz təpəsidir, `sCapHeight` isə `686`; v1 eyni transformu daşıyır. Optik təsiri yoxdur, `0.15` faizdir |
| 21 | Fiziki sınaq: beş lövhə ilə tikmə, trafaret, oyma | ilk istehsal - bölmə 7 |
| 22 | Domen, sosial handle-lar, şifahi identika | dəyişməyib, sahib |
| 23 | Nişanın rəngi | **bağlandı** 2026-09-26 - sahib seçimi həvalə etdi, qısa siyahının `#9`-u (`struktur-govde-ink-qollar-petrol`), bölmə 3.0 |
| 24 | Platforma şəkillərində yatıq kilidin ölçüsü (nişan kadrın `0.2308`-i idi, banner `0.328`) | **bağlandı** - `PLAT.HORIZONTAL_BOX = 0.4266`, `tools/build/platformv2.py`, bölmə 8 |
| 25 | LinkedIn bannerinin deskriptor sətri | **yazılmır** - telefonda cap `6.0 px`, döşəmə `9 px`; tövsiyə: şüar LinkedIn səhifəsinin tagline sahəsinə, bölmə 8.1 |

Tam qərar zənciri: `delivery/00-docs/01-DECISION-RECORD.md` (v2 iş qovluğunda
`v2-ai/delivery/00-DECISION-RECORD-V2.md` idi - v2 mətni onu `v2-ai/delivery/01-DECISION-RECORD.md` kimi göstərirdi, belə fayl git tarixçəsində yoxdur; v2-nin göndərilən surəti
`archive/v2-2026-09-21/delivery/01-DECISION-RECORD.md`), v1 üçün
`archive/v1-2026-08-24/delivery/01-DECISION-RECORD.md` (v1-də
`delivery/01-DECISION-RECORD.md` idi).

**Ad verdikti AMBER-dir, şərtlidir** və v2 işi onu dəyişmir. İki tərəfdən
sıxılma real və daimidir: `Kestrel AI` (YC F25) fonetik olaraq, `Kestra`
(`kestra.io`) funksional olaraq.

**Brend kitabı, sayt lansmanı, müştəriyə gedən çap və USPTO ərizəsi ABŞ ticarət
nişanı vəkilinin yaşıl işığından asılıdır.** Bu sənəd hüquqi məsləhət deyil.
