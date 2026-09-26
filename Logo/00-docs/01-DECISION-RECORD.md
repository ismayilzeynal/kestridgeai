# Kestridge AI - "KAi" nişanı, qərar reyestri (v2, v3)

| | |
|---|---|
| Layihə | `kestridge-ai-brand` |
| Sənəd | `delivery/00-docs/01-DECISION-RECORD.md`. v2 paketində `delivery/01-DECISION-RECORD.md` idi (iş nüsxəsi `v2-ai/delivery/00-DECISION-RECORD-V2.md`); göndərilmiş v2 nüsxəsi `archive/v2-2026-09-21/delivery/01-DECISION-RECORD.md`-dədir |
| Tarix | 2026-09-21 (v2, bölmə 0-11); 2026-09-26 (v3 əlavələri, D7-D10, bölmə 12-15) |
| Vahid | L2-DES / L3-DES-GRPH |
| Status | **v3 paketi, `delivery/`.** v2 paketi `archive/v2-2026-09-21/`-də, v1 paketi `archive/v1-2026-08-24/`-dədir. 2026-09-21 statusu ("v2.0 namizəd paketi; `delivery/` altındakı v1 paketi yerində qalır və əvəz olunmayıb") v3-də əvəz olunub (2026-09-26) - bölmə 15 |
| Girişlər (v3, 2026-09-26) | `tools/build/mark.py` (`COLOUR`, `COLOUR_KNOCKOUT`), `tools/build/platform.py` və `tools/build/platformv2.py` (`PLAT.HORIZONTAL_BOX`), `qa/09-linkedin-banner/measurements.json`, `qa/01-logo-svg-measurements.json`, `delivery/MANIFEST.json`, `tools/build/pack.py`, `tools/build/paths.py`, rəng turlarının datası (mənbəsi bölmə 12.1-də), git commit-ləri `79b9987` və `b6f3328` |
| Girişlər (v2, 2026-09-21) | Aşağıdakı `v2-ai/...` yolları o vaxtın yeridir, tarixçə kimi saxlanılır; hazırkı yerləri bölmə 15.4-dədir. `v2-ai/BRIEF.md`, on bir namizədin `plates.json` + `qa.json` + `NOTES.md` faylı, `v2-ai/lockup-study/RECOMMENDATION.md` + `ladder-measurements.json`, `tools/build/mark.py` öz-sınağı, `v2-ai/delivery/01-master/MEASUREMENTS.json`, `v2-ai/delivery/09-banner/measurements.json`, `docs/concept/renders/_build/browsercheck.json` (v1 Chromium ölçmələri) |
| Rəqəmlərin mənbəyi | hər ədəd yuxarıdakı fayllardan birindədir və ya diskdən / git-dən sayılıb (yeri mətndə yazılır). Ölçülməyən yer `açıqdır` yazılır, təxmin yazılmır. v2 girişlərinin `v2-ai/...` faylları v3-də ağacdan çıxıb: `BRIEF.md` və `f2`-nin `plates.json`-u köçüb (bölmə 15.4), qalanı silinib və yalnız git tarixçəsindədir (`79b9987`, bölmə 15.3); 6.1-in v3 qeydi `95c1e1d` və `66f7460` commit-lərindən oxunub. Ağacdan kənar mənbələr ayrıca yazılır: v2-nin hakim raundu (3.1), ikinci rəng turunun sayları və 12.4-dəki qısa siyahı cədvəli (12.1-in mənbə qeydi), kestridge.com-un canlı CSS ölçməsi (12.5) |

Bu sənəd nə qərar verildiyini və **niyə** verildiyini saxlayır. Qaydaların özü
`02-BRAND-GUIDELINES.md`-dədir; burada təkrarlanmır.

---

## 0. Bir baxışda

| # | Qərar | Nəticə | Kim verdi |
|---|---|---|---|
| **D1** | Nişan | **`f2-strict-system`**. Beş lövhə: `stem`, `arm`, `tittle`, `leg`, `crossbar`. Kanonik generator `tools/build/mark.py` | sahib |
| **D2** | Kilid nisbəti | **`M / C = 2/1`**, yəni yatıq kiliddə `CAP_H = 10c/3 = 9.0u`. Stacked `CAP_S = 2.5c = 6.75u` toxunulmur | sahib |
| **D3** | Banner | **`b4-oversize-monogram`**, yeni nişanla yenidən **qurulub**: indi `delivery/09-linkedin-banner/` (v2-də `v2-ai/delivery/09-banner/` idi). LinkedIn `1512 x 256` örtüyünün **kanonik** faylı budur (tünd b4), `07-platforms/`-dakı açıq variant deyil (v2-də `07-platform`) - `02-BRAND-GUIDELINES.md` 8 | sahib |
| **D4** | Aralıq sistemi | **iki pillə**: struktur `c = 4k = 2.7`, örtük `2k = 1.35`. Üçüncü **lövhə-lövhə** dəyər yoxdur; tirin baseline-dan hündürlüyü (`3k = 2.025`) aralıq deyil, **modul**dur və 5.1-də açıq yazılır | D1-dən çıxır |
| **D5** | Deskriptor | cap `0.30 C` invariantı saxlanılır, yəni `C_d = 2.7u = c` (əvvəl `4.05u`) | D2-dən çıxır |
| **D6** | Reduktiv kəsik | **`24 px`**-dən aşağıda nişan `tittle` və `crossbar` lövhələrini çəkmir, üç struktur lövhəsi ilə gedir. İkinci master deyil: həndəsə əsas kəsiyin özüdür. Həd **ölçülüb**, seçilməyib. Qayda `markv2.cut_for()` (indi `tools/build/mark.py`), sənəd `00-docs/06-REDUCED-CUT.md` (v2-də paketin kökündə `REDUCED-CUT.md` idi) | ölçmədən çıxır (7.1) |
| **D7** | Rəng (v3) | Nişan **iki rəngdə** göndərilir: yalnız `K`-ya aid olan `stem` mürəkkəbdə (`#0F1317`), `Ai`-ni quran dörd lövhə (`arm`, `tittle`, `leg`, `crossbar`) `petrol-600` `#0E6A82`-də; tünd fonda `#FAFAF7` + `petrol-400` `#2FA8C7`. Qısa siyahının **#9**-u, `struktur-govde-ink-qollar-petrol`. Həndəsə dəyişmir | sahibin həvaləsi ilə (2026-09-26), bölmə 12 |
| **D8** | Platforma şəkillərində kilidin miqyası (v3) | Yatıq kilidin qutusu bannerin ink payından törədilir: `HORIZONTAL_BOX = 0.4266` (əvvəl `0.30`). `M / C` dəyişmir | sahibin müşahidəsi, ölçmədən çıxır - bölmə 13 |
| **D9** | Banner deskriptor sətri (v3) | Banner `color-knockout` kilidlə yenidən quruldu; deskriptor sətri **yenə yazılmır**, strapline LinkedIn-in öz tagline sahəsinə tövsiyə olunur | ölçmədən çıxır - bölmə 14 |
| **D10** | Paketin qovluqlanması (v3) | `delivery/` adla tapılan qovluqlara bölündü (`color/` və `mono/` ayrı), sübut faylları `qa/`-ya, sənədlər `00-docs/`-a köçdü; iş artefaktları silindi; mətn, kod, JSON və SVG git-dən bərpa olunur, `v2-ai/`-dakı PNG / JPG renderlər yox | sahib (tapşırıq) - bölmə 15 |

D1, D2 və D3 **bağlıdır**, yenidən açılmır. D4 və D5 onların həndəsi
nəticəsidir. D6 auditin ölçdüyü qüsurun cavabıdır və 7-nin öz "örtük itə bilər,
siluet korlana bilməz" qərarını icraya çevirir.

D7-D10 v3-dür (2026-09-26). D1-D6-nın heç birini açmır: lövhələr, aralıqlar,
kilid nisbəti və reduktiv kəsik olduğu kimi qalır. D7 isə `02-BRAND-GUIDELINES.md`
bölmə 3-dəki "nişan petrol rəngdə çəkilmir" və "nöqtə və tir aksent rəngə
boyanmır" qaydalarını əvəz edir - bölmə 12.6.

---

## 1. Sahibin istəyi

### 1.1 Nişan - sahibin öz sözləri

Sahib göndərilmiş nişanın üzərinə əl ilə iki yaşıl ləkə çəkdi və yazdı:

> Logonun içində aşağı sağ hissədə elə bil A hərfi, yuxarı sağ tərəfə doğru
> gedən hissə isə kiçik i hərfi kimi olacaq və "Ai" anlamın verəcək. K hərfi də
> təbii olaraq Kestridge-i təmsil edir.

Eskizin şablon olmadığını da o özü dedi:

> Mənim çəkdiyim təqribi əl ilə çəkilmiş formadı, onun eynisini et demədim axı
> sənə, mən sənə onu anlamaq üçün verdim.

> Bənzəməsi problem deyil, sadəcə mən ölçülərdə sürüşmə etmiş ola bilərəm
> və ya yarımçıq çəkmiş ola bilərəm. Ona görə dedim ki, mənimki "must copy"
> olmalı deyil. Yəni limitləmək istəmirəm heç nəyi.

Bağlayıcı olan üç şərt (`BRIEF.md` 2.1):

1. qolun ucunun üstündə **nöqtə** var, qol + nöqtə `i` oxunur;
2. gövdə + ayaq + yeni **köndələn tir** birlikdə `A` oxunur;
3. bütöv forma hələ də `K`-dır.

Koordinatlar, ölçülər, proporsiyalar bağlayıcı **deyil**.

### 1.2 İkinci və üçüncü istək

Bu ikisinin hərfi sitatı arxivdə deyil, ona görə burada **yazılı tələb kimi**
verilir, sitat kimi yox:

| # | Tələb | Harada qeyd olunub |
|---|---|---|
| 2 | Kiliddə **K sözdən böyük olsun** | `v2-ai/banner/b3-deep-field/NOTES.md` sətir 175, `b4-oversize-monogram/NOTES.md` bölmə 7 |
| 3 | LinkedIn üçün **tünd, bahalı görünən** banner; göstərdiyi rəng `#0E6A82` | `b4-oversize-monogram/NOTES.md` bölmə 2, `b1`, `b3`, `b5` NOTES-larında eyni rəng qeydi |

**v3 qeydi (2026-09-26).** Bu cədvəldəki `v2-ai/banner/...` `NOTES.md` faylları
v3-də iş artefaktı kimi silinib (bölmə 15.3). Git tarixçəsində qalırlar və
`b6f3328`-dən əvvəlki commit-dən (`79b9987`) bərpa olunurlar.

İkinci tələb nişan işi deyil, **kilid** işidir: nişanın öz ink qutusu
`18 x 18` qalır, dəyişən onun sözmarkaya nisbətidir (bölmə 6).

Üçüncü tələbin rəngi paletdədir: `#0E6A82` = `petrol-600`. Yəni sorğu brend
sisteminin içindədir, kənar rəng gətirmir. (v3-də nişanın `Ai` qrupu məhz bu
rəngdə çəkilir - D7, bölmə 12.)

---

## 2. Ölçülmüş mane: `18 x 18` qutuda tək aralıqla `A` tiri qurulmur

Bu, zövq mübahisəsi deyil. Dörd eyniliklə bağlıdır və hər dördü yoxlanılıb.

### 2.1 Counter-in eni

Gövdənin sağ üzü `x = 7`-də, ayağın alt üzü `x = y - 4.325` xəttindədir:

```
counter(y)  = (y - 4.325) - 7 = y - 11.325
counter(21) = 9.675 = M - (w + a) + k = 18 - 9.0 + 0.675
```

`w + a = 9.0` **kvadratlıq eyniliyi** ilə kilidlidir (`2(w + a - 2k) + c = 18`).
Ona görə `w`/`a` bölgüsünü dəyişmək counter-i dəyişmir - yoxlanılıb, iddia deyil.

### 2.2 Tək pilləli sistemin arifmetikası

Tiri hər tərəfdən `c` aralıqla yerləşdirsək, tirin alt eni:

```
W_bot = counter(21)
        - c              baseline aralığı (tirin alt üzü y = 21 - c)
        - c              gövdəyə aralıq
        - c * sqrt(2)    ayağın 45 üzünə c perpendikulyar, üfüqi qarşılığı 3.818
        - k              ucun fasələsi
      = 9.675 - 2.7 - 2.7 - 3.818 - 0.675
      = -0.218
```

**Mənfi.** Yəni tir üçün yer yoxdur, qalınlığı sıfır olsa belə.

`BRIEF.md` bölmə 4 eyni nəticəni digər tərəfdən verir: tələb olunan `W` eni
üçün lazım olan qutu `M = 2W + 2t + 14.85`-dir, yəni `W = 3.5`, `t = 1.8`
üçün `M = 25.45` lazımdır - indiki `18`-in `1.41` misli. Bu isə `c / M`
nisbətini `0.15`-dən `0.106`-ya endirir, yəni nişanın xarakterini dəyişir.

### 2.3 Üç çıxış yolu, biri qadağan

| Yol | Qiyməti | Vəziyyət |
|---|---|---|
| ink qutusunu böyütmək | `c / M` dəyişir; `s1`-in `w + a = M / 2` miqyas kilidi sübut edir ki, bu, `c`-ni kiçiltməklə eynidir. `s6` və `s7` ölçdü: K kvadrat plitədə `13%` kiçilir | rədd |
| ikinci aralıq pilləsi | "hər aralıq dəqiq `c`" cümləsi itir | **seçilən** |
| tirin gövdəyə və ya ayağa toxunması | topologiya müdafiəsi ölür | **qadağan** (`BRIEF.md` 5.4) |

### 2.4 Seçilən yolun arifmetikası

İki pillə ilə eyni hesab (`o = 2k = 1.35`, tir `y = 18.975`-də bitir):

```
W_bot = counter(18.975) - 2k - 2k*sqrt(2) - k
      = 7.650 - 1.350 - 1.909188 - 0.675
      = 3.715812
```

Müsbət və işlək. `W_top = 2.365812`. Bu iki ədəd `markv2.py` öz-sınağının
8-ci bölməsində iki müstəqil törəmə ilə bağlanır.

---

## 3. Sahə: on bir namizəd, səkkiz strategiya, üç linza

### 3.1 Kəşfiyyat raundu - səkkiz namizəd

Hər biri `v2-ai/tools/proof.py` (dəyişdirilməmiş harness) ilə ölçüldü və
**üç müstəqil hakim** tərəfindən oxundu: **hərf** (letterform), **sənətkarlıq**
(craft), **sistem** (system).

> **Mənbə qeydi - hakim raundu ağacdan kənardır.** Üç linzalı oxunuş **baş
> verdi**, amma o, ayrıca workflow-da işlədi və hər agentin nəticəsi bu
> reponun xaricində qaldı: `v2-ai/` altında nə transkript, nə bal cədvəli, nə
> də hakim çıxışının hər hansı faylı var. Ona görə burada **hakim balı və ya
> hökmü sitat kimi verilmir**; hakimlərə istinad yalnız namizəd `NOTES.md`
> fayllarının öz "keep / drop" maddələri vasitəsilə qalır (məsələn
> `f2-strict-system/NOTES.md` 7.2, `f1-best-of-field/NOTES.md` 2), və hər
> belə istinad **ağacdan kənar ölçmə** sayılır. Aşağıdakı cədvəlin hər sütunu
> isə ağacdakı fayldan gəlir - bu bölmənin qərara dayaq olan hissəsi odur.
> Finalist raundu üçün eyni dürüstlük 3.3-dədir.

| Namizəd | Strategiya | ink qutusu | `min_gap` | örtük | təpə |
|---|---|---|---|---|---|
| `00-sketch-literal` | eskizin hərfi transkripsiyası, **kontrol** | `18 x 19.31` | `0.6541` | `0.279681` | 24 |
| `s1-grow-box` | tək aralıq dəyəri, böyük nişan | `18 x 18` | `1.5` | `0.264091` | 25 |
| `s2-k-gap` | göndərilmiş K toxunulmur, örtük `k = c/4` pilləsində | `18 x 18` | `0.675` | `0.260287` | 26 |
| `s3-half-gap` | yeni lövhələr K-nın öz zolaqlarından `c/2` ilə kəsilir | `18 x 18` | `0.9546` | `0.264155` | 25 |
| `s5-redraw` | beş lövhə sıfırdan, `c` şəbəkəsində | `18 x 18` | `2.0` | `0.253310` | 24 |
| `s6-tall-box` | kvadratdan imtina, hər aralıq `c` | `18 x 20.7` | `1.35` | `0.252576` | 24 |
| `s7-axis-i` | nöqtə qolun oxunu davam etdirir, qutu böyüyür | `20.7 x 20.7` | `1.35` | `0.198607` | 24 |
| `s8-module` | bir paralelqram iki dəfə, güzgüdə: nöqtə və tir eyni fiqur | `18 x 18` | `0.9` | `0.253204` | 24 |

Namizəd nömrələnməsində `s4` yoxdur - ağacda belə bir qovluq mövcud deyil.
Göndərilmiş nişanın örtüyü müqayisə bazasıdır: `0.232793`.

Harness xəbərdarlığını **iki** namizəd aldı, biri yox: `s6-tall-box` **və**
`00-sketch-literal`. Harness-in öz sətri (`qa.json` -> `warns`) belədir:

> ink box does not land on whole pixels at every ladder size

Yəni harness "**hər** nərdivan ölçüsündə düşmür" deyir, "**heç bir** ölçüdə
düşmür" yox - sonuncu daha güclü cümlədir və harness onu demir. Bu iki
namizəd üçün daha güclü hal ayrıca ölçülüb və `qa.json` ->
`measured.pixel_grid`-dədir: `on_grid` beş ölçünün (16 / 32 / 48 / 128 /
512) **hamısında** `false`-dur. Xəbərdarlıq qapı deyil - səkkiz namizədin
hamısında `pass: true`, `fails: []`.

### 3.2 Finalist raundu - üç namizəd

Kəşfiyyatın "saxla" maddələri üç fərqli qurluşda birləşdirildi:

| Namizəd | Bir cümlə ilə |
|---|---|
| `f1-best-of-field` | sahənin yekun variantı; ziddiyyətdə **hərf hakimi** üstün tutulur |
| `f2-strict-system` | auditor üçün: sistem qaydası nə qədər azdırsa, o qədər yaxşı |
| `f3-max-legibility` | ölçülmüş 16 px döşəmələri hər şeydən üstündür |

Hər üçü eyni bazanı saxladı: `stem` və `leg` göndərilmiş nişanla **təpə-təpə
eyni**, qolun ucu bir `k` geri, ink qutusu `18 x 18`, təpə sayı 25.

### 3.3 Niyə `f2`

Seçimi sahib verdi. Ağacda finalist hakim transkripti **yoxdur**, ona görə
burada hökm yazılmır; aşağıdakı cədvəl seçimin **ölçülmüş dayağıdır**.

| Göstərici | `f1` | **`f2`** | `f3` | Mənbə |
|---|---|---|---|---|
| elan edilmiş lövhə-lövhə aralıq **dəyərləri** | 3 (`2.7`, `1.35`, `1.4319`) | **2 (`2.7`, `1.35`)** | 3 (`2.7`, `1.35`, `1.9092`) | `qa.json` `pair_distances` |
| `leg\|crossbar` (45 kanal) | `1.4319` | **`1.35`** | `1.9092` | eyni |
| gözlə seçilmiş ədəd sayı | 0 | **0** | 1 (`bar_t = 1.8`) | `NOTES.md` 7.5 |
| tirin öz səviyyəsində counter-i kəsmə payı | `46.4 / 47.1%` | **`48.57%`** | `43.24%` | `NOTES.md`, `markv2.py` |
| ink örtüyü | `0.242095` (`+4.00%`) | `0.245534` (**`+5.47%`**) | `0.244204` (`+4.90%`) | `qa.json` |
| tirin ink payı | `0.0332` | `0.0468` | `0.0416` | `qa.json` `share` |
| 50% həddində 4-qonşu birləşmə, 16..64 px | yoxdur | yoxdur | yoxdur | hər namizədin öz ölçməsi |
| 50% həddində 8-qonşu künc toxunuşu | **ölçülməyib** | **`leg\|crossbar` 17, 18, 23 px** (tək cüt) | yoxdur (`16..96`) | `f2`: `qa/raster-merge-ground-truth.txt` (v2-də paketin kökündə `RASTER-MERGE-GROUND-TRUTH.txt`), LANCZOS; `f3`: `NOTES.md` 8.1 |

**8-qonşu sətrində `f1` xanası niyə boş deyil, "ölçülməyib"dir.** `f1`-in öz
`NOTES.md`-i ("Raster həqiqəti" bölməsi) yalnız **50% həddində komponent
sayır** - qonşuluq növü nə seçilib, nə də yazılıb, və 8-qonşu tarama heç vaxt
işlədilməyib. Ona görə orada "təmiz" yazmaq ölçülməmiş nəticəni ölçülmüş kimi
göstərmək olardı. `f3`-də belə deyil: onun `NOTES.md` 8.1 bölməsi 8-qonşu
taramanı açıq verir (`0.50` hədd, 8 qonşuluq, `16..96`, on cütün hamısı
təmiz). `f2` xanası isə **yenidən ölçüldü** (aşağıdakı bənd).

Oxunuşu belədir:

- **`f2` sistem üzrə qalibdir.** Lövhə-lövhə elan edilmiş məsafələr **dəqiq
  iki dəyər** alır. `f1` və `f3`-də örtük pilləsinin iki fərqli qiyməti var,
  çünki hər ikisi 45 kanalı `sqrt(2)` faktoru ilə ayrıca elan edir. `f2` 45
  kanalı da `2k`-ya sabitləyir və fərqi koordinata ötürür (`10.715812`,
  `12.065812`), qaydaya yox.
- **`f2` `A`-da ən uzun tiri verir** (`48.57%`), üstəlik tirin qalınlığı,
  nöqtənin tərəfi və tirin baseline-dan hündürlüyü **eyni ədəddir**: `3k`.
- **Qiyməti var və gizlədilmir.** Örtüyü üç finalistin ən ağırıdır
  (`+5.47%`), tirin ink payı ən böyüyüdür, və 8-qonşuluqda **bir** cüt künc
  toxunuşu verir: `leg|crossbar`, 17, 18 və 23 px-də. Bütöv nişanın
  komponent sayı həmin üç ölçüdə beşdən **dördə** düşür. 4-qonşu körpü heç
  bir ölçüdə yoxdur, yəni siluet dəyişmir, amma bu, real ölçülmüş
  kontaktdır. (`arm|tittle` cütü LANCZOS protokolunda heç bir ölçüdə
  birləşmir; aralıq auditin "17 px" rəqəmi BOX filtrindən gəlmişdi və geri
  qaytarılıb - `qa/raster-merge-ground-truth.txt`,
  `02-BRAND-GUIDELINES.md` 6.0.)
- `f1` ən yüngül nişandır, `f3` ən təmiz rasterdir. İkisi də arxivdə qalır.

---

## 4. Dürüst məhdudiyyət: `A` tapılan oxunuşdur, göz önündə olan yox

Bunu yumşaltmaq olmaz, ona görə düz yazılır.

**Tir counter-in təxminən yarısını kəsir və tavan da təxminən yarıdır.**
`f2`-də tir öz səviyyəsində counter-in `48.57` faizini tutur. Daha uzun tir
almaq mümkündür, amma yalnız 16 px-də əriyən aralıqlarla: `s3` `67.6%` aldı və
kanalı 16 px-də yapışdı, `s2` `67%` aldı və 48 px-dən aşağı yapışdı.

**Səbəb strukturdadır, tirdə deyil.** `A`-nın sol ştrixi şaquli gövdədir, ona
görə counter **düzbucaqlı üçbucaqdır**, `A`-nın açılan splayı deyil. Apeks
yoxdur: gövdənin sağ üzü ilə ayağın alt üz xətti `(7.0, 11.325)` nöqtəsində
kəsişir və oradan yuxarı heç nə açılmır.

**Bu qüsuru on bir namizədin hamısı daşıyır.** Səkkiz kəşfiyyat namizədinin
heç birində hər üç hakim eyni anda `A = true` verməyib - bu cümlə **ağacdan
kənar** hakim raundundan gəlir (3.1-dəki mənbə qeydi), ağacdakı fayldan yox,
və burada məhz bu şərtlə saxlanılır. Ağacda yoxlana bilən hissəsi budur: üç
finalistin hər birinin öz `NOTES.md` risk siyahısında eyni bənd var.

Nəticə praktiki cümlə ilə: **"burada `A` var" deyilsə, oxucu onu tapır;
deyilməsə, kiçik üfüqi element görür.** Nişan `K`-dır və `K` qalır; `Ai`
tapılan ikinci qatdır. Marketinq mətni bunun əksini iddia etməməlidir.

`i` oxunuşu üçün eyni dürüstlük: `i`-nin gövdəsi **45 diaqonaldır**, klassik
şaquli gövdə deyil. Bu, sahibin öz konsepsiyasının içindədir (qol `i`-nin
gövdəsidir), ona görə həll ediləsi problem deyil. Mümkün olan ən yaxşısı
edilib: kompakt kvadrat nöqtə, qolun düz ucunun üstündə, `1.35` üst-üstə
düşmə ilə.

---

## 5. Aralıq sistemi iki pillə oldu

### 5.1 Nə dəyişdi

v1 qaydası (v1 paketinin `02-BRAND-GUIDELINES.md` 2.1 və 9.6; v2 vaxtı
`delivery/`-də idi, indi `archive/v1-2026-08-24/delivery/02-BRAND-GUIDELINES.md`):

> Onları `K` edən yeganə şey bir sabitdir: hər aralıq dəqiq `c = 2.7u`.
> ... `c` dəyişmir. Aralıq nişanın yeganə parametridir.

Bu cümlə **artıq doğru deyil** və əvəz olunur:

| Pillə | Dəyər | Üzvləri | Nədir |
|---|---|---|---|
| **struktur** | `4k = c = 2.7` | `stem\|arm`, `stem\|leg`, `arm\|leg` | K-nın öz üç kanalı |
| **örtük** | `2k = 1.35` | `arm\|tittle`, `stem\|crossbar`, `leg\|crossbar` | örtük lövhəsi ilə onun bağlandığı lövhə |

Hər pillənin əhalisi **üçdür**. Bir nəfərlik pillə yoxdur, üçüncü **lövhə-lövhə**
dəyər yoxdur. `c` dəyişmir; `k = c/4` də dəyişmir - onu dəyişmək hər iki
pilləni eyni anda dəyişir.

**Bir istisna var və o, gizlədilmir: tirin baseline-dan hündürlüyü.** Nişanın
içində bu iki pillənin heç birinə düşməyən bir məsafə var - tirin alt üzü ilə
baseline arasındakı ağ zolaq, `BASE - by1 = 3k = 2.025`. Bu, iki lövhə
arasındakı aralıq deyil (tirin altında lövhə yoxdur), ona görə iki pilləli
**aralıq** qaydası pozulmur; amma gözlə baxanda o da ağ məsafədir və nə
`2.7`-dir, nə `1.35`. Spesifikasiya onu **örtük modulu** kimi elan edir
(`markv2.py` bölmə 8: `ortuyun tek olchusu: tir b = 2.025`, yəni nöqtənin
tərəfi = tirin qalınlığı = tirin baseline-dan hündürlüyü = `3k`). Qadağa
cümləsi (`02-BRAND-GUIDELINES.md` 9 bənd 6) bunu demirdi; indi orada da
yazılıb - eyni faktı bir sənəd elan edib o birinin gizlətməsi qalmır.

### 5.2 Niyə məhz `2k`

`markv2.py` öz-sınağının 5-ci bölməsi ölçür və eyni nəticəni verir:

```
struktur  stem|arm = stem|leg = arm|leg     = 2.700000
örtük     arm|tittle = stem|crossbar = leg|crossbar = 1.350000
elan edilmiş aralıqlar tam iki dəyər alır: [1.35, 2.7]
```

`leg|crossbar` `1.35`-i **perpendikulyar**dır, üfüqi deyil. Üfüqi ofset
`2k * sqrt(2) = 1.909188`-dir. `s3-half-gap` burada səhv etmişdi: `1.35`-i
üfüqi ölçmüş və perpendikulyarda `0.9546` almışdı, yəni elan etmədiyi
üçüncü dəyər. `f2`-də kanal başdan-başa sabit enlidir.

### 5.3 Örtüyün tək ölçüsü

Örtük `3k = 2.025`-in üç dəfə işlənməsidir: nöqtənin tərəfi, tirin qalınlığı,
tirin baseline-dan hündürlüyü. Buna görə cap zolağının bölgüsü bağlanır:

```
h + o = 3k + 2k = 5k = c + k = r = 3.375
```

burada `r` qolun geri çəkilməsidir. Yəni nöqtə **mövcud zolaqdan kəsilir**,
nişanın üstünə əlavə edilmir.

---

## 6. Kilid nisbəti `4/3` -> `2/1`

### 6.1 Qərar

`ladder-measurements.json`-dakı altı pillə eyni banner enində (`1512 px`)
render edildi. Sahibin "K sözdən böyük olsun" tələbi rəqəmlə budur:

| | `4/3` (v1) | **`2/1` (v2)** | fərq |
|---|---|---|---|
| Nişan `1512 px` bannerdə | `175.70 px` | **`246.04 px`** | **`+40.0%`** |
| Sözmarka cap | `131.78 px` | `123.02 px` | `-6.6%` |
| Kilid AR | `8.6055 : 1` | **`6.1453 : 1`** | `-28.6%` |
| `GAP / cap` | `0.300` | **`0.450`** | - |
| Sözmarka cap `u` | `13.5` | `9.0` | `-33.3%` |

**`2/1` ideal dəyərdir; göndərilmiş vektor `1.999929` çəkir.** Səbəb
yuvarlaqlaşdırmadır, qərar deyil: `lockup.py` sözmarka transformunu altı onluq
rəqəmlə yazır (`scale(0.013120 -0.013120)`), dəqiq miqyas isə
`9.0 / 686 = 0.0131195335`-dir. Ona görə faylda cap `9.000320u` çıxır və
`M / C = 18 / 9.000320 = 1.999929` olur. Eyni səbəbdən kilid AR-ı elan edilən
`6.145335` deyil, **`6.145511`** çəkilir (göndərilmiş
`kestridge-lockup-horizontal-default-positive.svg`-dən ölçüldü: ink qutusu
`110.619204 x 18.000000`). Fərq `1.8e-4`, yəni `1024 px` enli renderdə
`0.03 px`-dən azdır - praktiki nəticəsi yoxdur, amma "dəqiq `2.0`" demək
səhvdir və `01-master/MEASUREMENTS.json` indi hər iki rəqəmi verir.

**v3 qeydi (2026-09-26): yuxarıdakı son cümlə artıq doğru deyil.** Çəkilən
dəyərləri daşıyan `drawn` bloku `01-master/MEASUREMENTS.json`-a əl ilə
yazılmışdı: commit `95c1e1d`-də var, v2 commit-i `66f7460`-də yoxdur, və
göndərilmiş v2 nüsxəsində (`archive/v2-2026-09-21/delivery/01-master/MEASUREMENTS.json`)
də yoxdur. Bu, bölmə 10 bənd 11-in xəbərdar etdiyi haldır:
`tools/build/masters.py` `drawn` blokunu yazmır (faylda bu söz yoxdur), ona
görə hər yığım onu silir. Hazırkı ölçü faylı `qa/01-logo-svg-measurements.json`
yalnız elan edilən dəyərləri verir (`ratio_M_over_C 2.0`, `aspect_ratio 6.145335`).
Çəkilən `1.999929` və `6.145511` heç bir maşın oxunan faylda yoxdur (`qa/01-logo-svg-measurements.json`, `qa/stage-manifest.json`, `delivery/MANIFEST.json`); yalnız sənədlərdə yazılıdır: bu bölmə, `00-BUILD-RECORD.md` və `02-BRAND-GUIDELINES.md` 5.1.

En sabit olanda nisbəti qaldırmaq demək olar **tamamilə nişanı böyüdür**:
sözmarka kilidin eninin `86` faizini tutur, ona görə cap-ı azaltmaq bütün
blokun enini yığır və miqyas nişana qayıdır.

### 6.2 Niyə `2/1`, niyə daha yuxarı deyil

`GAP = 1.5c = 4.05u` nişana bağlıdır, sözmarkaya yox. Sözmarkanın öz söz
boşluğu isə `0.45 x cap`-dır. `2/1`-də bu ikisi **dəqiq bərabərləşir**:

| `M/C` | cap `u` | `GAP / cap` |
|---|---|---|
| 4/3 | 13.5 | 0.300 |
| 3/2 | 12.0 | 0.338 |
| 5/3 | 10.8 | 0.375 |
| **2/1** | **9.0** | **0.450** |
| 7/3 | 7.71 | 0.525 |
| 8/3 | 6.75 | 0.600 |

Yəni `2/1`-də nişanla söz arasındakı məsafə sözmarkanın `KESTRIDGE` ilə `AI`
arasında işlətdiyi boşluqla eyni olur: nişan sətirdəki üçüncü söz kimi, bərabər
ritmdə oturur. `7/3` və `8/3`-də aralıq sözün öz daxili boşluğundan geniş olur,
söz nişandan qopur və sözmarka altyazıya çevrilir.

`2/1` mövcud aralıq qaydasının pozulmadan yaşadığı **son pillədir**.

### 6.3 Kiçik ölçüdə vəziyyət yaxşılaşır

| | `4/3` (v1) | **`2/1` (v2)** |
|---|---|---|
| ən kiçik icazəli kilid eni | `137.69 px` | **`122.91 px`** |
| orada nişan | `16.0 px` | **`20.0 px`** |
| orada cap | `12.0 px` | `10.0 px` |
| **bağlayıcı döşəmə** | **nişan** | **sözmarka cap** |
| render hündürlüyü döşəməsi (pad daxil) | `20.8 px` | `26 px` |

Bağlayıcı şərt **tərsinə çevrilir** və bu, `02-BRAND-GUIDELINES.md` 6.2-də
yazılıb. `lockup.py` `floor_ok()` onsuz da hər iki qapını yoxlayır, ona görə
məntiqdə düzəliş lazım deyil - yalnız `CAP_H` sabiti və izah sətri dəyişir.

### 6.4 Stacked kilid dəyişmir

`CAP_S = 2.5c = 6.75u`, `M / C = 8/3`. Səbəb: `64 px` blokda cap `12.54 px`-dir
və döşəmə `12 px`-dir, yəni ehtiyat cəmi `4.5` faizdir. Növbəti pillə (`3/1`)
cap-ı `11.4 px`-ə endirir - döşəmədən aşağı. Stacked-də sahibin istədiyi
hökmranlıq onsuz da tavandadır.

**Stacked-də elan edilən `2.666667` çəkilmir - `2.662690` çəkilir, və bu
v1-dən mirasdır.** `wdth 70` instansiyasında `K` qlifi **`687`** vahid
hündürlükdədir, miqyas isə `686` vahidlik cap dəyərindən hesablanır
(`6.75 / 686 -> scale(0.009840)`), ona görə faylda cap `6.760080u` olur və
`18 / 6.760080 = 2.662690` çıxır. **v2 bunu gətirməyib:** v1-in öz
`archive/v1-2026-08-24/delivery/01-master/kestridge-lockup-stacked-default-positive.svg`
faylındakı (v2 vaxtı `delivery/01-master/`-də idi)
transform hərf-hərf eynidir (`translate(2.1391 31.5000) scale(0.009840
-0.009840)`). `wdth 100`-də belə fərq yoxdur - orada `K` tam `686` vahiddir,
fərq yalnız altı onluqlu yuvarlaqlaşdırmadan gəlir (6.1).

**Yan nəticə, qeyd edilməlidir:** v1-də yatıq cap stacked cap-ın **dəqiq iki
misli** idi (`13.5 / 6.75 = 2.000`). İndi bu nisbət `9.0 / 6.75 = 1.3333`-dür.
v1 qaydalarının 5.2 bəndindəki "stacked-də nişan sözmarkaya görə dəqiq iki dəfə
böyükdür" cümləsi bu səbəbdən yenidən yazıldı.

### 6.5 Deskriptor sətri - D2-nin məcburi nəticəsi (D5)

Deskriptorun eni `u` vahidində sabitdir və `cap`-dan xətti asılıdır.
`C_d = 4.05u` olaraq qalsaydı:

```
deskriptor eni / sözmarka eni = 132.9593 / 88.5660 = 1.5012
```

Yəni deskriptor sözmarkadan **50 faiz uzun** olardı və 5.4-ün flush qaydası
ölərdi. Doğru invariant `1.5c` deyil, **`0.30 C`**-dir:

```
C_d = 0.30 * 9.0 = 2.7u = c
deskriptor eni = 88.639504u,  sözmarka 88.566035u,  fərq +0.083 faiz
```

`+0.083` faiz v1-dəki **eyni** artıq eniddir (tracking `+180` seçilib, dəqiq
flush `+179.336` olardı). Yəni qayda dəyişmir, yalnız onun `c` ilə ifadəsi
dəyişir. Eyni məntiqlə enmə (drop) `0.70 C = 6.3u` olur (`3.5c = 9.45u` idi).

Ödənilən qiymət: deskriptorun `9 px` cap döşəməsi indi **daha erkən** bağlayır.
Deskriptor cap `2.7u` olduğu üçün `9 px`-ə çatması üçün kilid eni
`368.7 px` olmalıdır (v1-də `344.2 px`). Yəni deskriptor variantı `24.5 px`
daha gec açılır.

Eyni qapının hündürlük ifadəsi: deskriptor faylının kanvası `25.24723u`-dur,
ona görə render hündürlüyü **`85 px`**-dən aşağı düşəndə cap `9 px`-dən
kiçilir. `01-master/MEASUREMENTS.json`-da bu sətir əvvəl `26 px` yazırdı -
həmin rəqəm düz kilidin `23.4u` bloku üzərində hesablanmışdı və deskriptorun
öz qapısını heç saymırdı. Düzəldildi: `min_floor_height_px = 85`,
`floor_ok.64 = false`, üstəlik üç qapı ayrıca verilir (nişan `23 px`,
sözmarka cap `29 px`, deskriptor cap `85 px`).

**v3 qeydi (2026-09-26).** Hazırkı yer `qa/01-logo-svg-measurements.json`,
`horizontal-descriptor` sətridir. Orada `min_floor_height_px = 85` və
`floor_ok.64 = false` var və onları indi generator özü hesablayır
(`tools/build/masters.py`: `descriptor_floor_ok`, `min_descriptor_floor_height`).
Üç qapının ayrıca hündürlükləri isə (`floor_basis.gates`: `23`, `29`, `85 px`)
faylda yoxdur - `floor_note` yalnız üç döşəməni sadalayır. Onlar `drawn` bloku
ilə birlikdə əl yazısı idi və eyni yolla itib (6.1-in v3 qeydi).

---

## 7. Kiçik ölçü dürüstlüyü: `Ai` 24 px və yuxarı xüsusiyyətdir

Bu ölçmələr namizədin öz raster taramasındandır. **Harness-də raster testi
yoxdur** (`proof.py` yalnız həndəsə ölçür), ona görə `qa.json`-dakı
`pass: true` bu bölmənin arxasında durmur.

| Ölçü | Nə olur |
|---|---|
| 512 / 128 px | tam spesifikasiya; nöqtə və tir qərar kimi oxunur |
| 48 px | örtük hələ aydın; fasələr optik yox olur |
| **24 px** | **təmiz döşəmə**: beş lövhənin hamısı hər həddə ayrıdır |
| 16 px | tam kəsikdə nöqtə `1.35 px`, tir `1.35 px` boz zolağa çevrilirdi - **buna görə orada artıq reduktiv kəsik göndərilir (D6, 7.1)** |

**`0.9 px` düzəlişi.** Bu sətirdə əvvəl tir `0.9 px` yazılmışdı; bu, `f1`-in
rəqəmidir, `f2`-nin yox. `f2`-nin tiri `3k = 2.025u` qalınlığındadır, yəni
16 px-də `2.025 / 24 x 16 = 1.35 px` (eyni nəticə harness protokolu ilə də
çıxır: `fit = 0.75`, `2.025 / 18 x 12 = 1.35`). `0.9 px` `f1`-in `1.35u`
tirinə aiddir və `f1-best-of-field/NOTES.md` sonunda məhz belə yazılıb.
`02-BRAND-GUIDELINES.md` 6.0 onsuz da `1.35`-i düzgün verirdi - iki
göndərilmiş sənəd bu sətirdə bir-birinə zidd idi, indi deyil.

Ölçülmüş faktlar:

- 16 px-dən 64 px-ə qədər **hər tam ölçüdə**, 50% həddində, 4-qonşuluqda
  **beş ayrı komponent**. Yəni heç bir lövhə birləşmir.
- 8-qonşuluqda üç ölçüdə (17, 18, 23 px) komponent sayı beşdən **dördə**
  düşür. Tək künc toxunuşu `leg|crossbar` cütündədir. Mürəkkəb əlavə etmir,
  siluet dəyişmir, amma real kontaktdır
  (`02-BRAND-GUIDELINES.md` 6.0, `qa/raster-merge-ground-truth.txt`).
- 25% boz həddində örtük kanalları boyanır: `arm|tittle` 16, 17, 18, 21 px-də;
  `stem|crossbar` 16, 18, 19, 22, 25 px-də; `leg|crossbar` 16 px-də. 16 px-də
  bütöv nişan həmin həddə iki komponentə axır.
- **K-nın öz üç kanalı heç bir ölçüdə, heç bir həddə birləşmir.** Yəni
  göndərilmiş nişanın oxunuşu tam qorunur; risk yalnız örtükdədir.

Qərar cümləsi: **örtük itə bilər, siluet korlana bilməz.** `i` oxunuşu 32
px-dən aşağıda faktiki olaraq yoxdur və bunu gizlətmək olmaz.

### 7.1 Düzəliş: yuxarıdakı son bənd səhv nəticə çıxarırdı (D6)

**"Risk yalnız örtükdədir" cümləsi yanlışdır və saxlanmır - düzəldilir.**
Bəndlərin özü doğrudur: `stem|leg` cütü doğrudan da heç vaxt birbaşa
birləşmir. Buradan çıxarılan nəticə yanlış idi. `25%` həddində tir **gövdə ilə
ayaq arasında körpü** qurur - ikisi bir-birinə yox, **tirə** yapışır - və
nəticədə `16 px`-də bütöv nişan **iki** komponentə axır, halbuki çılpaq `K` hər
ölçüdə və hər həddə **üç** saxlayır. Cüt-cüt ölçmə bunu görmürdü, çünki qüsur
cütdə yox, **bütövdə**dir. Nöqtə eyni ölçüdə bir tam mürəkkəb pikselə + boz
haləyə düşür və forma kimi yox, ləkə kimi oxunur. `16x16`-da midton piksel payı
`12.11%`-dir, v1-in `8.59%`-nə qarşı: `+40.9%`.

**D6 - reduktiv kəsik.** Ölçülmüş həddən (`24 px`) aşağıda nişan nöqtəni və
tiri **çəkmir** və üç struktur lövhəsi ilə gedir. Bu, ikinci master deyil:
`stem`, `arm`, `leg` əsas kəsiyin təpələrini hərfi mənada saxlayır (`f2`-nin
geri çəkilmiş qol ucu daxil) və ink qutusu eyni `18 x 18` qalır, ona görə
siluet həddin hər iki tərəfində kəsilməzdir. Yəni "örtük itə bilər, siluet
korlana bilməz" qərarı indi **kodda icra olunur**, yalnız nəsrdə vəd edilmir.

Həd seçilməyib, **ölçülüb**: hər iki kəsik `16`-dan `64 px`-ə qədər hər tam
ölçüdə `markv2.raster()` ilə çəkilib, `50%` və `25%` hədlərində, `4`- və
`8`-qonşuluqda komponentlər sayılıb. Tam cədvəl, qayda və yenidən qurulan üç
fayl: **`00-docs/06-REDUCED-CUT.md`** (v2-də paketin kökündə `REDUCED-CUT.md`
idi; xam data indi `qa/reduced-cut.json`). Kod: `markv2.cut_for()`,
`markv2.REDUCED_FLOOR_PX = 24` (v3-də `tools/build/mark.py`, qurucular onu
`markv2` adı ilə idxal edir), öz-sınaq bölmə `9`-`10`.

Orta parlaqlıq müqayisəsi (eyni PIL yolu, eyni fit):

| px | v2 tam kəsik | v2 **göndərilən** kəsik | v1 göndərilmiş |
|---|---|---|---|
| 16 | `0.7399` | **`0.7597`** (reduktiv, D6) | `0.7535` |
| 128 | `0.7547` | `0.7547` (əsas) | `0.7666` |

`128 px`-də yeni nişan v1-dən **tündür**. `16 px`-də isə D6-dan sonra reduktiv
kəsik göndərilir və o, v1-dən `0.0062` **açıqdır** - "v2 kiçik ölçüdə v1-dən
tündür" cümləsi orada qüvvədən düşür və saxlanmır. `6.1`-in "ikinci master
lazım deyil" qərarı buna baxmayaraq qüvvədədir: sıx kəsiyin hədəflədiyi
`0.781`-dən `0.7597` hələ də `0.0213` tünddür. Chromium ilə
təkrar ölçmə hələ **edilməyib**; yuxarıdakı iki sütun PIL rəqəmidir və v1-in
Chromium rəqəmləri ilə qarışdırılmamalıdır. Həmin Chromium cütü budur:

| v1 Chromium, `16 px`, `device_scale_factor = 1` | Orta parlaqlıq | Mənbə |
|---|---|---|
| **əsas kəsik** (göndərilən) | **`0.7703`** | `docs/concept/renders/_build/browsercheck.json` |
| **sıx kəsik** (geri götürülüb, 6.1) | **`0.7497`** | eyni |

Yəni `0.7703` / `0.7497` iki mühərrik deyil, **iki kəsikdir**: eyni Chromium
ölçməsində v1-in əsas və sıx kəsiyi. Sıx kəsik paketdə yoxdur, ona görə
`0.7497` yalnız arxiv rəqəmidir.

Eyni faylda hər ölçü üçün PIL qarşılığı da var, yəni Chromium ilə PIL
arasındakı fərq **ölçülüb**: səkkiz cütdə `0.0015` ilə `0.0070` arasındadır
(16 px əsas `0.0059`, 16 px sıx `0.0070`, 32 px əsas `0.0015`). Ona görə
"fərq `0.007`-dən kiçikdir" cümləsi v1-i olduğundan yaxşı göstərir - doğru
ifadə **interval**dır və ən böyük fərq məhz `16 px`-dədir, yəni qərarın
asıldığı ölçüdə.

---

## 8. Mirasın sübutu - iddia deyil

`markv2.py` öz-sınağı (`python tools/build/mark.py` -> `PASS`, exit 0):

| Yoxlama | Nəticə |
|---|---|
| `f2-strict-system/plates.json` ilə təpə-təpə eynilik | 25 təpə, 5 lövhə, artıq lövhə yoxdur |
| `stem` və `leg` = `mark.py contours("main")` | **təpə-təpə eyni** |
| `arm` sürüşməsi | `[(0,0), (0,0), (-0.675, 0.675), (-0.675, 0.675), (-0.675, 0.675), (0,0)]` |
| ink qutusu | `(3.0, 3.0, 21.0, 21.0)`, `18 x 18`, mərkəzdə |
| piksel şəbəkəsi 16 / 32 / 48 / 128 / 512 | dörd kənarın hamısı tam piksel |
| bucaq ailəsi | ən pis kənar sapması `1.78e-15` |
| aralıq pillələri | `[1.35, 2.7]`, başqa dəyər yoxdur |
| topologiya | minimum aralıq `1.35`; toxunuş yox, örtüşmə yox, öz-kəsişmə yox |
| `qa.json`-dakı 10 cüt məsafə | 4 onluq dəqiqliklə üst-üstə düşür (v2 qaçışı; v3-də `f2`-nin `qa.json`-u silinib və `tools/build/mark.py` öz-sınağı bu yoxlamanı - bölmə 7 - atlayır) |
| **24** qapanış eyniliyi | hamısı bağlanır |

Yəni "göndərilmiş K qorunub" cümləsi **maşınla yoxlanan iddiadır**, redaktor
sözü deyil.

**Qapanış eyniliklərinin sayı artefaktdan götürülür.** Bu sətirdə əvvəl `23`
yazılmışdı; `python tools/build/mark.py` öz-sınağının 8-ci bölməsi
**`24`** sətir çap edir (`cap zolagi` sətrindən `ink ortuyu` sətrinə qədər
sayılıb). Sənəd generatorun çıxışını təkrarlayır, əksi yox.

---

## 9. Bağlanmayan iki eynilik

Dürüstlük üçün, ikisi də ölçülüb:

1. **Namizədin `mark.svg`-i öz `plates.json`-u ilə bit-dəqiq deyil.** Bu,
   `markv2`-nin qüsuru deyil, əvvəldən var idi: `proof.py`-ın SVG yazıcısı
   `%g` (6 əhəmiyyətli rəqəm) işlədir, ona görə `10.715812` fayla `10.7158`
   kimi düşür. Ən böyük koordinat sapması `1.2e-5`, yəni namizədin SVG-sində
   `leg|crossbar` kanalı `1.35 +/- 1.7e-5`-dir. `NOTES.md` 8.3 bunu
   "`1e-6` səviyyəsi" adlandırır; **ölçülmüş dəyər bir tərtib böyükdür.**
   `markv2.py` `_fmt` ilə tam 6 onluq yazır və `plates.json`-a (kanonik spek)
   dəqiq uyğundur, ona görə onun SVG-si namizədinkindən **daha sıxdır**.
2. **`lockup.py`-dakı `CAP_H = 5 * C = 13.5` sabiti D2 ilə ziddiyyətdədir.**
   `tools/` oxunur-yazılmır, ona görə v2 paketində sabit **çağırış yerində**
   dəyişdirilir (`CAP_H = 10 * C / 3`) və `MEASUREMENTS.json` bunu açıq yazır.
   v1 paketi əvəz olunanda `lockup.py`-da bir sətir dəyişməlidir; o vaxta qədər
   `tools/build/lockup.py` öz-sınağı hələ `M/C = 4/3` gözləyir və bu **normaldır**.

   **v3 qeydi (2026-09-26): bağlanıb.** `tools/build/lockup.py` indi
   `CAP_H = 10 * C / 3` (`9.0u`) daşıyır. v1 dəyəri qurucularda ayrıca ad
   ilə saxlanılır, `CAP_H_V1 = 5c = 13.5u` (`masters.py`, `collateralv2.py`,
   `platformv2.py`, `icons_pdf.py`, `manifest.py`), çünki onlar v1-i
   `lockup.CAP_H`-dan oxuyurdu və sətir dəyişəndən sonra v2-ni v1 sanırdılar.
   `lockup.py` öz-sınağı sıfır `FAIL` ilə keçir.

---

## 10. Nə dəyişir - icra siyahısı

| # | İş | Vəziyyət |
|---|---|---|
| 1 | Nişan generatoru `tools/build/mark.py` | **hazır**, öz-sınağı `PASS` |
| 2 | Master SVG-lər, v2-də `v2-ai/delivery/01-master/`; indi `delivery/01-logo-svg/mono/` | **yazılıb** (22 fayl + `MEASUREMENTS.json`; ölçü faylı indi `qa/01-logo-svg-measurements.json`) |
| 3 | Bu qərar reyestri və `02-BRAND-GUIDELINES.md` | **bu sənəd**; ikisi də indi `delivery/00-docs/`-dadır |
| 3a | Rasterlər, v2-də `v2-ai/delivery/02-raster/`; indi `delivery/02-logo-png/mono/` | **yazılıb** - 71 fayl (31 PNG, 20 JPG, 20 WEBP): nişan `16..1024 px`, dörd kilid variantı `64..1024 px` |
| 3b | Platforma dəsti, v2-də `v2-ai/delivery/07-platform/`; indi `delivery/07-platforms/` | **yazılıb** - 35 aktiv + `_check/` (7 yoxlama şəkli + `MANIFEST.json`; indi `qa/07-platforms/`): LinkedIn, X, YouTube, GitHub, OG / favicon / PWA |
| 4 | `b4-oversize-monogram` bannerinin yeni nişanla yenidən qurulması | **yazılıb** - v2-də `v2-ai/delivery/09-banner/`; indi `delivery/09-linkedin-banner/`, kanonik LinkedIn örtüyü `kestridge-banner-linkedin-cover-1512x256.png`, ölçmələr `qa/09-linkedin-banner/measurements.json`, izah `00-docs/05-BANNER-NOTES.md` |
| 5 | `tools/build/lockup.py` `CAP_H` sabiti | **açıqdır** - v1 əvəz olunanda |
| 6 | `tools/build/mark.py`-ın `markv2.py` ilə əvəz olunması | **açıqdır** - sahib təsdiqindən sonra |
| 7 | Chromium ilə 16 / 32 px parlaqlıq təkrar ölçməsi | **edilməyib** |
| 8 | Raster birləşmə taramasının `proof.py`-a sərt qapı kimi əlavəsi | **tövsiyə**, edilməyib |
| 9 | Dairəvi avatarda yeni nişanın gözlə təsdiqi | **edilməyib** |
| 10 | Fiziki istehsal sınağı (tikmə, trafaret, oyma) beş lövhə ilə | ilk partiya |
| 11 | `masters.py`-da deskriptor döşəməsinin hesabı və `drawn` bloku | **açıqdır, təsviri dəqiqləşdirilib (2026-09-26)** - v2 mətni burada `01-master/MEASUREMENTS.json`-a əl ilə yazılmış düzəlişlərdən və `regeneration_warning` sətrindən danışırdı. Yoxlandı: nə göndərilmiş v2 faylında (`archive/v2-2026-09-21/delivery/01-master/MEASUREMENTS.json`), nə v3 faylında (`qa/01-logo-svg-measurements.json`) `drawn` bloku və ya belə sətir yoxdur; v2 manifestində də `drawn` 20 yerdə `null` idi. Yəni v3 qurulması heç nə silməyib. Deskriptor döşəməsi v2-dən bəri generatordan gəlir (`masters.py`, `descriptor_floor_ok`); **açıq qalan** `drawn` rəqəmlərinin generatordan çıxmamasıdır - çəkilən dəyərlər hələ yalnız sənədlərdədir: bu sənədin 6.1 və 6.4 bölmələri, `00-BUILD-RECORD.md` 4.1 və `02-BRAND-GUIDELINES.md` 5.1, 5.2 |

**v3 vəziyyəti (2026-09-26).** Cədvəl v2-nin icra siyahısıdır və olduğu kimi
saxlanılır (2-4-cü bəndlərə hazırkı yollar əlavə olunub, bənd 11-in təsviri 2026-09-26-da dəqiqləşdirilib); iki bəndin vəziyyəti dəyişib, 11-inki isə v3-də dəyişməyib:

| # | v3-də |
|---|---|
| 5 | **bağlanıb** - bölmə 9 bənd 2-nin v3 qeydi |
| 6 | **bağlanıb** - bir generator var, `tools/build/mark.py`. `v2-ai/build/markv2.py` ikinci nüsxə idi və öz miras yoxlamasında `tools/build/mark.py`-ı v1 kimi idxal edirdi, yəni v2-ni özü ilə müqayisə edirdi; silinib. Qurucular modulu `markv2` adı ilə idxal edir |
| 11 | **v3-də dəyişməyib, təsviri dəqiqləşdirilib** - deskriptor döşəməsini `masters.py` v2 commit-i `66f7460`-dən bəri özü hesablayır (`85 px`); `drawn` bloku isə yazılmır və faylda yoxdur, ona görə bənd açıq qalır (6.1 və 6.5-in v3 qeydləri) |

Bənd 7-10 v3 işinin əhatəsinə düşmədi və bu yeniləmədə yenidən yoxlanılmayıb;
cədvəldəki vəziyyətləri 2026-09-21 tarixinə aiddir.

---

## 11. Bu sənədin iddia etmədikləri

- **"QA təmizdir" dəlil deyil.** `proof.py`-ın minimum aralıq həddi yoxdur,
  raster testi yoxdur, `0.9` nazik lövhə xəbərdarlığı bu ölçülərdə heç vaxt
  işləmir və dairə testi hər kvadrat ink qutusu üçün `0.494975` verdiyinə görə
  **tavtologiyadır**.
- **Piksel şəbəkəsi yalnız ink qutusuna aiddir.** Daxili kənarlar tam piksel
  sərhədinə düşmür - göndərilmiş nişanda da düşmürdü.
- **Eskizə bənzəmək arqument deyil.** `00-sketch-literal` kontrol nümunəsidir,
  hədəf deyil.
- **Bu sənəd hüquqi məsləhət deyil.** Ad verdikti v1-dəki kimi **AMBER**-dir və
  bu iş onu dəyişmir.

---

## 12. v3 - rəng bölgüsü: mürəkkəb `K`, petrol `Ai` (D7)

### 12.1 Sahibin istəyi və mənbə qeydi

v2 nişanı tək rəngdə göndərmişdi. v3-də sahib rəngli nişan istədi və birinci
turun cavabında iki şərt qoydu. Hərfi sitatı arxivdə deyil, ona görə burada
**yazılı tələb kimi** verilir:

1. nişan **ağ fonda** qalır - rəng nişanın altındakı sahəyə köçmür;
2. "rəng" `K`-nın **hissələrini fərqli rəngləmək** deməkdir.

Birinci turdan `#7`, `#8` və `#11`-i bəyəndi (12.2). Seçimi isə 2026-09-26-da
həvalə etdi:

> Ən yaxşısın sən müəyyən et və hər şeyi hazırla.

> **Mənbə qeydi - ikinci turun tam datası ağacdan kənardır.** Birinci turun
> datası `v2-ai/COLOUR-VARIANTS.json`-da commit olunmuşdu (`11` saxlanılan +
> `23` atılan = `34` təklif, fayldan sayılıb). İkinci turdan yalnız ilk üç
> linzanın (`cesur`, `duotone`, `qigilcim`) `21` təklifi
> `v2-ai/COLOUR-ROUND2-PARTIAL.json`-da commit olunmuşdu. İki fayl da v3-də
> silinib (bölmə 15.3) və `79b9987` commit-indən bərpa olunur. İkinci turun tam
> `45` təklifi, süzgəcin çıxışı və `12`-lik qısa siyahı sessiyanın iş
> fayllarında qaldı və repoya düşmədi: 12.3-dəki saylar həmin faylların
> 2026-09-26 ölçməsidir, 12.4-dəki cədvəl oradan köçürülüb. Seçimin nəticəsi
> isə ağacdadır: `tools/build/mark.py`, `COLOUR` və `COLOUR_KNOCKOUT`.

### 12.2 Birinci tur - altı linza, `34` təklif, `11` saxlanıldı

Altı rəng linzası (`sistem`, `sahe`, `aksent`, `gradient`, `duotone`, `tonal`)
`34` təklif verdi; süzgəc və hakim paneli `11`-ni saxladı, `23`-ü atdı. Turun
öz tövsiyəsi (`sistem-knockout-petrol800-sahe`) rəngi nişanın altındakı
**sahəyə** verirdi və nişanı kağız mürəkkəbində saxlayırdı. Sahibin birinci
şərti bu istiqaməti bağladı.

Sahibin bəyəndiyi üç variant:

| Tur 1 # | id | Rəng harada |
|---|---|---|
| 7 | `gradient-ink-spark` | yalnız `tittle`, `petrol-400` `#2FA8C7`; qalan dörd lövhə mürəkkəb |
| 8 | `duotone-ink-petrol500` | `tittle` və `crossbar`, `petrol-500` `#1187A5`; üç struktur lövhəsi mürəkkəb |
| 11 | `tonal-letter-groups` | xromsuz: `stem` mürəkkəb, `arm` + `tittle` `#434D57`, `leg` + `crossbar` `#1F262E` - hərf qrupları üzrə neytral pillələr |

### 12.3 İkinci tur - altı linza, `45` təklif, ölçü ilə süzgəc

Linzalar: `qigilcim` (qığılcım), `duotone`, `cesur` (cəsur), `herf` (hərf),
`struktur`, `tonal`. `45` təklif gəldi. Süzgəc rəy yox, **ölçü** idi - hər
qayda arifmetikadır:

| Qayda | Nə yoxlanır |
|---|---|
| palitra üzvlüyü | hər lövhənin rəngi brend palitrasındandır |
| ağ fon | sahibin birinci şərti (12.1) |
| lövhə kontrastı | hər lövhə öz fonunda `>= 3.0:1` (WCAG 1.4.11, qrafik döşəmə) |
| rəng sayı | ən azı `2`, ən çoxu `3` |
| rənglər seçilir | bir variantdakı istənilən iki rəng arasında `>= 1.35:1` |
| təkrar | eyni bölgü iki ad altında gəlibsə biri atılır |

İki təklif "rənglər seçilir" qaydasından keçmədi: `petrol-500` ilə
`neutral-500` `1.02:1`, `petrol-700` ilə `neutral-700` `1.03:1` - göz onları
bir rəng kimi oxuyur. Üçü təkrar idi. `40` təklif keçdi. Sahibə `12`-lik qısa
siyahı göstərildi.

### 12.4 Qısa siyahı - `12` variant

Nömrə sahibə göstərilən sıradır. Lövhə rəngləri palitra adı ilə verilir
(`ink` = `#0F1317`, `paper` = `#FAFAF7`, `white` = `#FFFFFF`). Son sütun
reduktiv kəsikdən (D6) sonra neçə rəngin qaldığını göstərir: `24` board px-dən
aşağı `tittle` və `crossbar` çəkilmir, rəng yalnız `stem`, `arm`, `leg`-də
yaşaya bilər.

| # | id | linza | `stem` | `arm` | `leg` | `tittle` | `crossbar` | fon | `24 px`-dən aşağı |
|---|---|---|---|---|---|---|---|---|---|
| 1 | `qigilcim-tek-noqte-petrol500` | qigilcim | ink | ink | ink | petrol-500 | ink | paper | tək rəng |
| 2 | `herf-yalniz-i-qalxir` | herf | ink | petrol-600 | ink | petrol-600 | ink | paper | iki rəng |
| 3 | `herf-k-butov-ortaq-imza` | herf | ink | ink | ink | petrol-600 | petrol-600 | paper | tək rəng |
| 4 | `qigilcim-a-herfi-petrol600` | qigilcim | ink | ink | petrol-600 | ink | petrol-600 | neutral-050 | iki rəng |
| 5 | `cesur-i-tund-a-parlaq` | cesur | ink | petrol-800 | petrol-500 | petrol-800 | petrol-500 | paper | üç rəng |
| 6 | `herf-uc-herf-uc-reng` | herf | ink | petrol-500 | neutral-600 | petrol-500 | neutral-600 | paper | üç rəng |
| 7 | `duotone-uch-herf-uch-reng` | duotone | ink | petrol-700 | neutral-600 | petrol-700 | neutral-600 | white | üç rəng |
| 8 | `tonal-neytral-yuxari-isiq` | tonal | neutral-700 | neutral-500 | ink | neutral-500 | ink | paper | üç rəng |
| **9** | **`struktur-govde-ink-qollar-petrol`** | struktur | **ink** | **petrol-600** | **petrol-600** | **petrol-600** | **petrol-600** | paper | **iki rəng** |
| 10 | `struktur-uc-lovhe-uc-reng` | struktur | ink | petrol-500 | petrol-700 | petrol-500 | petrol-700 | paper | üç rəng |
| 11 | `struktur-petrol-govde-ink-qollar` | struktur | petrol-800 | ink | ink | petrol-800 | petrol-800 | white | iki rəng |
| 12 | `cesur-iki-petrol-qutbu` | cesur | petrol-800 | petrol-500 | petrol-500 | petrol-800 | petrol-800 | white | iki rəng |

`#1` tur 1-in `#7`-sinin düzəldilmiş formasıdır (`petrol-400` əvəzinə
`petrol-500`), `#3` tur 1-in `#8`-inin saytın rəngindəki (`petrol-600`)
formasıdır; `#6`, `#7` və `#8` tur 1-in `#11`-inin hərf-qrupu məntiqini
davam etdirir.

### 12.5 Seçim: `#9`, sahibin həvaləsi ilə

Seçilən **`#9`**, `struktur-govde-ink-qollar-petrol`. Kodda
`tools/build/mark.py` -> `COLOUR` və `COLOUR_KNOCKOUT`.

**Bölgü oxunuşun özüdür.** `stem` beş lövhənin içində yalnız `K`-ya aid olan
yeganə lövhədir. Qalan dördü iki hərfə birdən xidmət edir: `arm` həm də `i`-nin
gövdəsidir, `tittle` `i`-nin nöqtəsidir, `leg` həm də `A`-nın diaqonalıdır,
`crossbar` `A`-nın tiridir. Yəni rəng bölgüsü **`K` + `Ai`** oxunuşunun
özüdür: mürəkkəb `K`, petrol `Ai`. Qısa siyahıda bölgüsü dəqiq bu sərhəddən
keçən iki rəngli yeganə variant `#9`-dur: `#5`, `#6`, `#7`, `#10` `i` ilə `A`-nı
ayrı rəngə salır (üç rəng), `#2` yalnız `i`-ni, `#4` yalnız `A`-nı rəngləyir,
`#11` rəngi gövdəyə və örtüklərə verir, `#12`-də mürəkkəb yoxdur, `#8`
xromsuzdur, `#1` və `#3` yalnız örtükləri rəngləyir.

Bölmə 4-ün məhdudiyyəti rənglə aradan qalxmır: həndəsə eynidir, ona görə
marketinq mətni yenə `A`-nın göz önündə olduğunu iddia etməməlidir.

**`petrol-600` saytın öz aksentidir.** `#0E6A82` kestridge.com-un canlı
CSS-indəki `--accent` dəyəridir (2026-09-26-da ölçülüb) və sahibin v2-də
banner üçün göstərdiyi rəngdir (1.2). Qeyd: paketin öz token faylında
(`delivery/08-website/kestridge-tokens.css`) `--k-accent` `petrol-500`-dür,
`petrol-600` isə `--k-accent-link`-dir. Nişanın rəngi token faylından yox,
`tools/build/mark.py`-dan gəlir.

**Reduktiv kəsikdən sağ çıxır.** `24` board px-dən aşağı `tittle` və `crossbar`
çəkilmir (D6); `stem` (mürəkkəb) və `arm` / `leg` (petrol) qalır, ona görə
`16 px` favicon da iki rənglidir.

**Niyə sahibin bəyəndiyi örtük variantları deyil.** Tur 1-in `#7`-si
(qığılcım) və `#8`-i (duotone) rəngi yalnız `tittle` və `crossbar`-a qoyur;
qısa siyahıdakı düzəldilmiş formaları (`#1`, `#3`) də belədir. Reduktiv kəsik
məhz bu iki lövhəni çəkmir, ona görə favicon ölçüsündə həmin variantlar
bütünlüklə mürəkkəbə dönür və rəng ideyası itir. `#9` rəngini hər ölçüdə
saxlayır, çünki petrol struktur lövhələrindədir.

### 12.6 Nəyi əvəz edir

Qaydaların özü `02-BRAND-GUIDELINES.md` bölmə 3-dədir. Burada qərar qeydə
alınır:

| Qayda (v2) | Harada | v3 |
|---|---|---|
| **R1** - nişan marka kimi petrol rəngdə çəkilmir | `02-BRAND-GUIDELINES.md` 3 | **v3-də əvəz olunub (2026-09-26).** Əsas nişan iki rənglidir |
| **R3** - nöqtəni və ya tiri aksent rəngə boyamaq qadağandır | `02-BRAND-GUIDELINES.md` 3 və 9 bənd 12 | **v3-də əvəz olunub (2026-09-26).** Nöqtə və tir petroldur, amma yalnız bütöv `Ai` qrupunun üzvü kimi - heç vaxt tək başına |
| **R4** - beş lövhə beş rəng demək deyil | `02-BRAND-GUIDELINES.md` 3 | **qüvvədədir, sərtləşib:** dəqiq iki rəng, hərf qrupuna görə bölünür (`K` / `Ai`), heç vaxt lövhə-lövhə |
| **R2** - fon teksturası istisnası (böyük monoqram `petrol-700` / `petrol-900`-də, məsələn banner) | `02-BRAND-GUIDELINES.md` 3 | **tekstura istisnası dəyişmir.** Eyni abzasın son cümləsinin "kiçik ölçüdə, kilidin içində, ikon və ya avatar kimi nişan həmişə neytral mürəkkəbdədir" hissəsi isə **v3-də əvəz olunub (2026-09-26)**: orada nişan indi `color` / `color-knockout`-dur, tək rəngli işdə mono variantdadır |

Tək rəngli variantlar tək rəngli istifadə üçün qalır (çap, basma, faks):
positive `#0F1317`, knockout `#FAFAF7`, mono-black `#000000`, mono-white
`#FFFFFF`. Faylları `delivery/01-logo-svg/mono/`, `delivery/02-logo-png/mono/`
və `delivery/04-logo-pdf/mono/`-dadır.

**Yeni qayda:** bölgü yenidən rənglənmir - açıq fonda başqa petrol pilləsi
yoxdur, petroldan başqa çalar yoxdur. Tünd fonda yalnız `color-knockout`
işlədilir.

### 12.7 Rəng cütləri və kontrast

| Lövhə / hissə | `color` (açıq fon) | `color-knockout` (tünd fon) |
|---|---|---|
| `stem` | `#0F1317` ink | `#FAFAF7` paper |
| `arm`, `tittle`, `leg`, `crossbar` | `#0E6A82` petrol-600 | `#2FA8C7` petrol-400 |
| sözmarka | `#0F1317` | `#FAFAF7` |
| deskriptor sətri | `#0E6A82` | `#2FA8C7` |

WCAG 2 nisbi parlaqlıq ilə:

| Cüt | Nisbət |
|---|---|
| petrol-600, paper `#FAFAF7` üzərində | `5.90:1` |
| petrol-600, white `#FFFFFF` üzərində | `6.17:1` |
| petrol-400, ink `#0F1317` üzərində | `6.70:1` |
| petrol-400, surface `#171D23` üzərində | `6.10:1` |
| petrol-400, black `#000000` üzərində | `7.55:1` |
| paper, ink üzərində | `17.84:1` |
| petrol-600 ink-ə qarşı (gövdə ilə qol, yəni bölgünün özü) | `3.02:1` |

Döşəmələr: qrafik `3.0:1` (WCAG 1.4.11), mətn `4.5:1`. Son sətir vacibdir:
iki mürəkkəb yalnız çalarla yox, **parlaqlıqla** da fərqlənir, ona görə bölgü
boz çapda və rəng görmə pozuntularının çoxunda sağ qalır.

---

## 13. v3 - platforma şəkillərində kilidin miqyası (D8)

### 13.1 Sahibin müşahidəsi

Sahib platforma dəstinin LinkedIn örtüyünə
(`delivery/07-platforms/linkedin-shirket-sehifesi-company-page-cover-banner-1512x256.png`-in
köhnə `0.30` qutusu ilə yığılmış nüsxəsi) baxdı və kilidi köhnə proporsiya
kimi oxudu. Hərfi sitatı arxivdə deyil; müşahidə kimi verilir.

### 13.2 Ölçmə: nisbət düz idi, kadr yox

Kilid hərf-hərf ölçüldü və `M / C` həm bannerdə, həm platforma örtüyündə
`2.0` çıxdı: `84 px` nişanın yanında düz hərflər (`K E T R I D A`) cap `42 px`
verir, `S` və `G` isə `44 px` oxunur - yalnız dairəvi hərfin overshoot-u
səbəbindən. `84` / `42` cütü bannerin ölçmə faylındakı dəyərlərdir
(`qa/09-linkedin-banner/measurements.json`: `mark_ink_px 84`,
`wordmark_cap_px 42.0`). Yəni D2 düz tətbiq olunmuşdu; fərq **kadrda** idi:

- `tools/build/platform.py` yatıq kilidin PNG **qutusunu** kanvas
  hündürlüyünün `0.30`-u ölçüsündə qurur (`HORIZONTAL_BOX = 0.30`). Qutu hər
  tərəfdə `c` (`2.7u`) boş sahə daşıyır, ona görə nişanın öz ink-i kadrın
  `0.30 x 18 / 23.4 = 0.2308`-nə (`23.1 %`) düşürdü.
- Sahibin təsdiqlədiyi LinkedIn bannerində nişan kadrın
  `84 / 256 = 0.328`-ni (`32.8 %`) tutur.
- Fərq `1.42x`. Eyni `M / C = 2/1` kilidini platforma örtüyündə köhnə kimi
  oxutan məhz bu idi.

### 13.3 Düzəliş

```
HORIZONTAL_BOX = 0.328125 x (18 + 2 x 2.7) / 18 = 0.4266
```

`tools/build/platformv2.py`-da `PLAT.HORIZONTAL_BOX` kimi qurulur və
`LOCKUP_INK_SHARE = 84 / 256`-dan törədilir: qutu bannerin ink payından
hesablanır, əksinə yox. `tools/build/platform.py`-ın öz sabiti
(`HORIZONTAL_BOX = 0.30`) orada qalır; v3 dəyərini çağırış yeri bağlayır.

Əhatə:

- aspekt nisbəti `>= 1.6` olan platforma şəkilləri (yatıq kilid
  kompozisiyaları);
- stacked kompozisiya (`STACKED_BOX 0.34`) dəyişmir;
- `72 %` en tavanı (`WIDTH_CAP = 0.72`) dəyişmir - dar bannerlərdə (məsələn
  `1200 x 628`) bağlayan odur.

D2 toxunulmur: dəyişən kilid deyil, kadrdır.

---

## 14. v3 - LinkedIn banneri: rəngli knockout, deskriptor sətri yenə yazılmır (D9)

### 14.1 Nə dəyişdi

Sahibin sözləri:

> Banneri filan da ona uyğun yeni versiyasın hazırla ver. Köhnələri də silmə amma.

Banner D3 kompozisiyası ilə yenidən quruldu, kilidi indi `color-knockout`-dur:
gövdə və sözmarka `#FAFAF7`, `Ai` qrupu `petrol-400` `#2FA8C7`. Fayllar
`delivery/09-linkedin-banner/`-dadır. Köhnə v2 bannerləri silinməyib:
`archive/v2-2026-09-21/delivery/09-banner/`.

Ölçmə `qa/09-linkedin-banner/measurements.json`-dadır:

| | örtük `1512 x 256` | profil fonu `1584 x 396` |
|---|---|---|
| nişan | `84 px` | `100 px` |
| sözmarka cap | `42 px` | `50 px` |
| `M / C` | `2.0` | `2.0` |
| kilidin ən zəif mürəkkəbi | `petrol-400` ink sahəsində, `6.70:1` | `petrol-400` ink sahəsində, `6.70:1` |

Əvvəl kilid yalnız kağız mürəkkəbində idi və nisbət `17.84:1` idi. İndi ən
zəif mürəkkəb `petrol-400`-dür, `6.70:1`; mətn qapısı `4.5:1` keçir. Böyük
monoqram teksturası dəyişmir: `petrol-800` panelin üstündə `petrol-700`,
sahədə `petrol-900` (R2, 12.6).

### 14.2 Deskriptor sətri - niyə yenə yoxdur

Sətir v2-də çıxarılmışdı (`05-BANNER-NOTES.md` 7.1). v3-də yenidən ölçüldü,
nəticə dəyişmədi:

- Faylda sətrin cap-ı `12.6 px`-dir.
- LinkedIn örtüyü `3.2:1` kəsimlə verir və görüntü sahəsinə miqyaslayır:
  `390 px` telefonda cap `6.0 px` olur. Brendin döşəməsi `9 px`-dir
  (`02-BRAND-GUIDELINES.md` 5.4).
- Döşəməyə qaldırmaq `1.5x` böyümə tələb edir və sətri 45 dərəcəlik tikişin
  üstünə `159.6 px` aparır (`qa/09-linkedin-banner/measurements.json`,
  `route_a.overruns_seam_px`). Örtüyün `1/3` miqyasında hal daha ağırdır
  (`x2.143`, `425.4 px`); profil fonu daxil tam cədvəl `05-BANNER-NOTES.md`
  7.1-dədir.

Qərar: sətir yazılmır.

### 14.3 Tövsiyə

Strapline LinkedIn səhifəsinin öz **tagline** sahəsinə yazılsın: orada canlı
mətndir və həmişə oxunur. Bu, sahibin addımıdır; paket onu etmir.

---

## 15. v3 - paketin qovluqlanması və iş artefaktlarının silinməsi (D10)

Sahibin tapşırığı:

> Sonra ümumiyyətlə logo filan hamısın düzgün formada folderlə, gərəksiz, test
> üçün istifadə etdiyin, üzərində hesablama filan aparmaq üçün etdiklərini sil.
> Hər şey düzgün folderlənmiş olsun, hər şeyi rahat tapmaq olsun adı ilə.

### 15.1 Paket: `delivery/`

| v2 | v3 | Fayl |
|---|---|---|
| sənədlər paketin kökündə (`00-BUILD-RECORD.md` və s.) | `00-docs/` | 7 |
| `01-master/` | `01-logo-svg/color/` və `01-logo-svg/mono/` | 34 (12 + 22) |
| `02-raster/` | `02-logo-png/color/` və `02-logo-png/mono/` | 153 (82 + 71) |
| `03-icons/` | `03-app-icons/` | 7 |
| `04-pdf/` | `04-logo-pdf/color/` və `04-logo-pdf/mono/` | 10 (5 + 5) |
| `05-collateral/` | `05-stationery/`; `-guides.png` yoxlamaları `qa/05-stationery-guides/`-a | 9 |
| `06-fonts/` | `06-fonts/` | 6 |
| `07-platform/` | `07-platforms/`; `_check/` yoxlamaları `qa/07-platforms/`-a | 35 |
| `08-site/` | `08-website/` | 9 |
| `09-banner/` | `09-linkedin-banner/`; yoxlamalar və ölçmələr `qa/09-linkedin-banner/`-a | 2 |
| `07-platform/NOTES.md` | `00-docs/04-PLATFORM-NOTES.md` | |
| `09-banner/NOTES.md` | `00-docs/05-BANNER-NOTES.md` | |
| `REDUCED-CUT.md` | `00-docs/06-REDUCED-CUT.md` (generasiya olunur) | |
| `REDUCED-CUT.json`, `RASTER-MERGE-GROUND-TRUTH.txt`, `01-master/`, `03-icons/`, `04-pdf/` altındakı `MEASUREMENTS.json`-lar | `qa/reduced-cut.json`, `qa/raster-merge-ground-truth.txt`, `qa/01-logo-svg-measurements.json`, `qa/03-app-icons-measurements.json`, `qa/04-logo-pdf-measurements.json` (sayt ölçüləri `qa/08-website-measurements.json` v2 paketində yox idi, `v2-ai/build/_sitev2-measurements.json`-dan gəlir) | |

Saylar `delivery/MANIFEST.json`-dandır: `272` fayl indekslənib, üstəgəl
`README.md` və `MANIFEST.json`. `delivery/README.md` "Tez tapmaq üçün"
cədvəli ilə ən çox lazım olan faylı adı ilə göstərir.

**Fayl adları.** Nişan adlarından `-main` düşdü:
`kestridge-mark-main-positive.svg` -> `01-logo-svg/mono/kestridge-mark-positive.svg`;
rəngli: `01-logo-svg/color/kestridge-mark-color.svg`. Tək rəngli rasterlərə
`positive` əlavə olundu: `kestridge-mark-main-128.png` ->
`02-logo-png/mono/kestridge-mark-positive-128.png`,
`kestridge-lockup-horizontal-default-512.png` ->
`02-logo-png/mono/kestridge-lockup-horizontal-default-positive-512.png`. PDF-lər
də belə: `kestridge-lockup-horizontal-default.pdf` ->
`04-logo-pdf/mono/kestridge-lockup-horizontal-default-positive.pdf`. Rəngli
rasterlərə nümunə: `02-logo-png/color/kestridge-mark-color-128.png`,
`02-logo-png/color/kestridge-mark-color-knockout-128.png`,
`02-logo-png/color/kestridge-lockup-horizontal-default-color-512.png`. Kilid
SVG-lərinin son hissəsi rəng yoludur: `color`, `color-knockout`, `positive`,
`knockout`, `mono-black`, `mono-white`.

**Kompozisiyalar indi rənglidir:** app ikonları, favicon, ofis materialları
(vizit kartı, blank, zərf, slaydlar, e-poçt imzası; tünd bölücü slayd
`color-knockout`), bütün platforma şəkilləri, sayt başlığı kilidləri (açıq:
`color`, tünd: `color-knockout`), sayt ikonları (`icon.svg` tünd plitə:
`color-knockout`; `icon-light.svg`: `color`; `apple-icon.png`: surface
üzərində `color-knockout`), OpenGraph şəkilləri (`color-knockout`), LinkedIn
bannerləri (`color-knockout`).

### 15.2 Layihə

| Qovluq | Nədir |
|---|---|
| `delivery/` | paket - yalnız `tools/build/pack.py` yazır, `00-docs/` istisnadır |
| `delivery/00-docs/` | əl ilə yazılan sənədlərin kanonik yeri; `06-REDUCED-CUT.md` generasiya olunur |
| `qa/` | ölçmələr və yoxlama şəkilləri, yığım onları yenidən yaradır; istisna `qa/raster-merge-ground-truth.txt`-dir - onu heç bir qurucu yazmır, `pack.py` `qa/`-ni təmizləyəndə saxlayır |
| `docs/concept/`, `docs/domain/`, `docs/narrative/`, `docs/research/` | tədqiqat sənədləri. Əvvəl layihənin kökündə idilər: `concept/`, `domain/`, `narrative/`, həmçinin `FONT-COVERAGE.md`, `US-TRADEMARK-SCAN.md`, `US-TYPEFACE-PERCEPTION.md` |
| `tools/build/` | generator; giriş nöqtəsi `pack.py`, bütün qovluqlar `paths.py`-da |
| `tools/data/` | yığım girişləri: `mark-plates-f2-strict-system.json` (dondurulmuş nişan spesifikasiyası), `platform-dimensions.json`, `US-COLLATERAL-SPEC.md` (`collateral.py` oxuyur) |
| `archive/v1-2026-08-24/` | v1 paketi (`delivery/` altında `173` fayl, 2026-09-26-da sayılıb) və v1 generatorları, o cümlədən təqaüdə çıxmış `tools-build-package.py`, `tools-build-sitegen.py`, `tools-build-browsercheck.py` və `build-docs/open-items-decisions.json` |
| `archive/v2-2026-09-21/` | v2 paketi, `202` fayl, 2026-09-21-də göndərilən `delivery/`-nin bayt nüsxəsi, üstəgəl v2 dizayn `BRIEF.md` |

İki arxiv git-də tam izlənir: paketlər commit `79b9987`-dən, v1 generatorları (`tools-build-package.py`, `tools-build-sitegen.py`, `tools-build-browsercheck.py`, `tools-build-_measurements.json`), `build-docs/open-items-decisions.json` və v2 `BRIEF.md` isə `b6f3328`-dən.

Yığım: `python tools/build/pack.py` (yalnız son stage-dən düzmək üçün
`--no-build`). Stage reponun xaricindədir: `%TEMP%\kestridge-ai-brand-stage`,
`KESTRIDGE_STAGE` ilə dəyişdirilir. Qurucuların sırası: `masters`,
`icons_pdf`, `collateralv2`, `platformv2`, `sitev2`, `render_banner`,
`reduced`, `manifest`, `verify_site`. Sıfırdan fərqli çıxış yığımı
`delivery/`-yə toxunmadan dayandırır. Stage-in hər faylı açıq qayda ilə
xəritələnir; xəritələnməmiş fayl yığımı dayandırır.

### 15.3 Silinənlər - git-dən bərpa olunur

Commit `b6f3328` ilə silinib:

- `v2-ai/` iş qovluğu: 11 nişan namizədi (seçilən spesifikasiya istisna - o
  `tools/data/mark-plates-f2-strict-system.json`-a köçüb), kilid nisbəti
  tədqiqatı, banner kəşfiyyatları `b1`-`b5`, müqayisə vərəqləri, rəng
  turlarının datası (12.1);
- `tools/` altındakı v1 birdəfəlik analiz skriptləri: `17` skript və
  `tools/_crit/`;
- `build/`: dörd `.md` faylı çatdırılma sənədlərinin bayt nüsxəsi idi.

Mətn, kod, JSON və SVG faylları `b6f3328`-dən əvvəlki tarixçədədir və oradan bərpa olunur; `v2-ai/`-dakı PNG və JPG şəkillər (müqayisə vərəqləri, banner eskizlərinin renderləri, `v2-ai/delivery/`-nin rasterləri) `.gitignore`-dakı `*.png` / `*.jpg` qaydasına görə git-də heç vaxt olmayıb və bərpa olunmur:
`git show 79b9987:<yol>` və ya `git checkout 79b9987 -- <yol>`; yol HQ
reposunun kökündən yazılır (`projects/kestridge-ai-brand/...`).

**Silinməyənlər:** v1 və v2 paketləri (`archive/`), köhnə bannerlər
(`archive/v2-2026-09-21/delivery/09-banner/`) - sahib "Köhnələri də silmə
amma" dedi.

Təmizlikdən sonrakı yığım paketi bayt-bayt təkrar istehsal etdi; yalnız
`MANIFEST.json` dəyişdi, çünki sənədlərin hash-ləri dəyişmişdi.

### 15.4 Bölmə 1-11-dəki köhnə yollar - indi haradadır

Bölmə 1-11 v2 vaxtının yollarını tarixçə kimi saxlayır. Hazırkı yerlər:

| Bölmə 1-11-də | İndi |
|---|---|
| `v2-ai/BRIEF.md` | `archive/v2-2026-09-21/BRIEF.md` |
| `f2-strict-system/plates.json` | `tools/data/mark-plates-f2-strict-system.json` (git-də ad dəyişməsi, JSON məzmunu eyni) |
| `f2`-nin öz `qa.json`, `NOTES.md` və `mark.svg` faylları, digər namizədlərin `plates.json`, `qa.json`, `NOTES.md` faylları, `lockup-study/`, `v2-ai/banner/` | silinib, 15.3 |
| `markv2.py` | `tools/build/mark.py` (qurucular onu `markv2` adı ilə idxal edir) |
| `v2-ai/tools/proof.py` | `tools/build/proof.py` (köçürülüb və dəyişdirilib) |
| `01-master/MEASUREMENTS.json` | `qa/01-logo-svg-measurements.json`; göndərilmiş v2 nüsxəsi `archive/v2-2026-09-21/delivery/01-master/MEASUREMENTS.json` |
| `09-banner/measurements.json`, `09-banner/NOTES.md` | `qa/09-linkedin-banner/measurements.json`, `delivery/00-docs/05-BANNER-NOTES.md`; v2 nüsxələri `archive/v2-2026-09-21/delivery/09-banner/` |
| `REDUCED-CUT.md` | `delivery/00-docs/06-REDUCED-CUT.md` |
| `RASTER-MERGE-GROUND-TRUTH.txt` | `qa/raster-merge-ground-truth.txt` |
| v1 paketi (`delivery/01-master/`, `delivery/02-BRAND-GUIDELINES.md`) | `archive/v1-2026-08-24/delivery/` |
| `docs/concept/renders/_build/browsercheck.json` (başlıq və 7.1; v2 nüsxəsində `concept/renders/_build/browsercheck.json` idi, `b6f3328` bu sənəddə yolu artıq yeniləyib) | eyni yer, dəyişmir |
