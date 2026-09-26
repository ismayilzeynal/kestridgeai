# Reduktiv kəsik - ölçülmüş həd və yenidən qurulan fayllar

> Bu sənəd **`tools/build/reduced.py`** tərəfindən ölçmənin özündən
> yaradılır. Əl ilə redaktə edilmir: rəqəmi dəyişmək üçün ölçmə yenidən
> işə salınır (`python tools/build/reduced.py`). Xam data:
> `qa/reduced-cut.json`.

---

## 1. Qüsur - auditin ölçdüyü, burada təkrar istehsal olunan

`16 px`-də v2 nişanı v1-dən **pis** deqradasiya edirdi. İki ayrı hadisə:

1. **Nöqtə (`tittle`) kir kimi oxunur.** `2.025u` kvadrat `16 px`
   lövhədə `1.35` cihaz pikselidir: bir tam mürəkkəb piksel, ətrafında
   boz halə. Forma qalmır, ləkə qalır.
2. **Tir (`crossbar`) körpü qurur.** Tirin sətri counter-i keçərək
   gövdəni ayağa bağlayan orta-boz zolağa çevrilir. `25%` həddində
   bütöv nişan **iki** komponentə axır, halbuki çılpaq `K` hər ölçüdə və
   hər həddə **üç** saxlayır.

| Ölçü | v2 tam kəsik | v1 göndərilmiş | Fərq | Qiymət |
|---|---|---|---|---|
| Midton piksel payı, `16x16` | **`12.11%`** | `8.59%` | **`+40.9%`** | **QÜSUR** - boz halə |
| Komponent sayı, `50%` hədd, `4-qonşu`, `16 px` | **`5`** | `3` | `+2` | qüsur deyil - v2-də beş lövhə var, ayrılması gözləniləndir |
| Komponent sayı, `50%` hədd, `8-qonşu`, `16 px` | **`5`** | `3` | `+2` | qüsur deyil - v2-də beş lövhə var, ayrılması gözləniləndir |
| Komponent sayı, `25%` hədd, `4-qonşu`, `16 px` | **`2`** | `3` | `-1` | **QÜSUR** - çılpaq `K`-nın üçündən az |
| Komponent sayı, `25%` hədd, `8-qonşu`, `16 px` | **`2`** | `3` | `-1` | **QÜSUR** - çılpaq `K`-nın üçündən az |

**Midton tərifi:** mürəkkəb örtüyü `0.20` ilə `0.80`
arasında qalan piksel - nə mürəkkəb, nə kağız. Auditin öz tərifidir və
burada dördüncü onluğa qədər təkrar istehsal olunur.

Göndərilən `16 px` faylın piksel xəritəsi, əvvəl və sonra:

```
    ƏVVƏL (tam kəsik)          SONRA (reduktiv kəsik)
    |                |   |                |
    |                |   |                |
    |  @@*     .@:   |   |  @@*           |
    |  @@*      -    |   |  @@*           |
    |  @@*   -##+    |   |  @@*   -##+    |
    |  @@*  =@@%.    |   |  @@*  =@@%.    |
    |  @@* -@@%.     |   |  @@* -@@%.     |
    |  @@*  ..       |   |  @@*  ..       |
    |  @@*  ..       |   |  @@*  ..       |
    |  @@* -@@%.     |   |  @@* -@@%.     |
    |  @@*  =@@%.    |   |  @@*  =@@%.    |
    |  @@*-#-=@@%.   |   |  @@*   =@@%.   |
    |  @@*:** =@@%.  |   |  @@*    =@@%.  |
    |  @@*     =@@%  |   |  @@*     =@@%  |
    |                |   |                |
    |                |   |                |
```

Sol tərəfdə `2.` sətrindəki `.@:` nöqtənin ləkəsidir, `11.`-`12.`
sətirlərdəki `-#-` / `:**` isə tirin qurduğu körpüdür. Sağ tərəfdə
ikisi də yoxdur və gövdə ilə ayaq arasındakı kanal açıq qalır.

---

## 2. Həll - reduktiv kəsik, ikinci master deyil

Örtüyün oxunduğu ölçüdən aşağıda nişan **nöqtəni və tiri atır** və üç
struktur lövhəsi ilə gedir. Bu, **reduksiyadır**:

- `stem`, `arm`, `leg` əsas kəsiyin təpələrini **hərfi mənada** saxlayır
  (`f2`-nin geri çəkilmiş qol ucu daxil) - `markv2._raw()` beş lövhəni
  həmişə qurur və mərkəzləmə sürüşməsini həmişə beşinin üzərindən alır,
  ona görə örtüyü atmaq `K`-nı **yerindən tərpədə bilmir**;
- mürəkkəb qutusu eyni qalır: `3u .. 21u`, yəni `18 x 18`, ona görə
  `lockup.py`-nin `M = 18.0` sabiti, clear-space rəqəmləri və piksel
  şəbəkəsi düşmələri dəyişmir;
- siluet həddin hər iki tərəfində **kəsilməz** qalır - yalnız iki kiçik
  lövhə solğunlaşır.

Kod tərəfi (`tools/build/mark.py`):

| Ad | Nədir |
|---|---|
| `CONCEPTS` | konsept -> (həndəsəni aldığı `CUTS` sətri, çəkdiyi lövhələr) |
| `REDUCED_PLATES` | `('stem', 'arm', 'leg')` |
| `OVERLAY_PLATES` | `('tittle', 'crossbar')` - reduksiyanın atdıqları |
| `plate_names(cut)` | adlandırılmış aksessor: konseptin çəkdiyi lövhələr |
| `plates(cut)` / `contours(cut)` | `reduced` üçün eyni dəyərlər, iki açar əskik |
| `REDUCED_FLOOR_PX` | **`24`** - ölçülmüş həd |
| `cut_for(px)` | bütöv qayda: `px >= REDUCED_FLOOR_PX` -> `main`, yoxsa `reduced` |

`cut_for(px)` v1-in `tools/build/mark.py` faylındakı eyniadlı köməkçinin
naxışıdır, ona görə çağıran tərəf sadə qalır: `raster(px)` özü seçir.

`python tools/build/mark.py` öz-sınağının **9-cu və 10-cu bölmələri**
bunu yoxlayır: lövhə-lövhə təpə eyniliyi, eyni ink qutusu, atılan
örtüyün sahəsi, reduktiv kəsikdə tək aralıq pilləsi (`2.7`), və
`cut_for`-un `1..4096` aralığında monoton olması.

---

## 3. Ölçmə protokolu

| | |
|---|---|
| Rasteriser | **`markv2.raster()`** - paketin öz rasteriseri, ikinci mühərrik yoxdur |
| Supersample | `markv2.ss_for(px)`, `px * ss >= 512` |
| Ölçülər | `16`-dan `64`-ə hər **tam** ölçü (lövhə piksel) |
| Hədlər | `50%` və `25%` alfa |
| Əlaqəlilik | `4`-qonşu və `8`-qonşu |
| Midton | örtük `0.20` .. `0.80` |
| Müqayisə | tam kəsik, reduktiv kəsik və göndərilmiş v1 - eyni boru xətti |

**Həddin tərifi.** Həd elə ən kiçik ölçüdür ki, **ondan yuxarı bir dənə
də istisna olmadan** tam kəsik reduktiv kəsikdən pis deyil:

- **R1** - `comp_full >= comp_reduced`, hər dörd (hədd, qonşuluq) cütündə.
  Örtük siluetə bir kanala başa gələ bilməz. Körpü testi budur.
- **R2** - `comp_full == 5`, `50%` həddində, hər iki qonşuluqda. Örtük
  örtük kimi oxunmalıdır, ləkə kimi yox. Oxunuş testi budur.

Midton payı **hesabat rəqəmidir, qapı deyil**: örtük oxunduğu yerdə də
bir qədər midton yaradır, ona görə onu qapıya çevirmək həddi heç vaxt
açılmayan yerə itələyərdi. Oxunuşun dürüst testi R2-dir.

**Müstəqil boru xətti ilə üst-üstə düşmə.** `02-BRAND-GUIDELINES.md`
bölmə `6.0` eyni sayları tamam başqa yolla ölçüb: `proof.py`-ın
`draw_plates` yolu, ink qutusundan miqyas, **LANCZOS** resolve. O sətirlər
`50%` / `8`-qonşu üçün `17 px`-də üç, `18` və `23 px`-də dörd komponent,
`25%` həddində isə `16 px`-də iki komponent verir. Yuxarıdakı cədvəl
(`markv2.raster`, **BOX** resolve) **eyni rəqəmləri** verir. İki ayrı
rasterizasiya yolu eyni nəticəyə gəlir - nəticə resolve filtrinin
artefaktı deyil.

**Parlaqlıq.** Reduksiya nişanı açıqlaşdırır, çünki iki lövhə az
mürəkkəb qalır. `proof.py` yolu ilə ölçülmüş (bölmə `6.1` ilə eyni):

| px | tam kəsik | reduktiv | v1 göndərilmiş | göndərilən |
|---|---|---|---|---|
| `16` | `0.7399` | `0.7597` | `0.7535` | `reduced` |
| `128` | `0.7547` | `0.7720` | `0.7666` | `main` |

`16 px`-də göndərilən kəsik v1-dən `0.0062` açıqdır.
Bu, `6.1`-dəki "v2 kiçik ölçüdə v1-dən tündür" iddiasını `16 px`
üçün qüvvədən salır və orada düzəldilib. Sıx kəsiyin geri götürülməsi
isə qüvvədə qalır: sıx kəsiyin hədəflədiyi `0.781`-dən hələ də
`0.0213` tünddür.

---

## 4. Tam cədvəl

Komponent sütunları: `50/4` = `50%` hədd, `4`-qonşu əlaqəlilik.
Reduktiv kəsik və v1 bütün `49` ölçüdə hər dörd sütunda `3` verir,
ona görə onlar bir sütunda yığılıb; fərqli dəyər olsaydı görünərdi.

| px | tam 50/4 | tam 50/8 | tam 25/4 | tam 25/8 | red. (4 sütun) | v1 (4 sütun) | midton tam | midton red. | midton v1 | R1 | R2 |
|---|---|---|---|---|---|---|---|---|---|---|---|
| `16` | `5` | `5` | `2` | `2` | `3` | `3` | `12.11%` | `8.98%` | `8.59%` | **X** | OK |
| `17` | `5` | `3` | `4` | `3` | `3` | `3` | `8.30%` | `6.23%` | `5.19%` | OK | **X** |
| `18` | `5` | `4` | `4` | `3` | `3` | `3` | `17.59%` | `14.51%` | `14.81%` | OK | **X** |
| `19` | `5` | `5` | `4` | `3` | `3` | `3` | `14.40%` | `12.19%` | `13.30%` | OK | OK |
| `20` | `5` | `5` | `5` | `4` | `3` | `3` | `9.75%` | `8.50%` | `8.50%` | OK | OK |
| `21` | `5` | `5` | `4` | `3` | `3` | `3` | `12.47%` | `9.98%` | `9.30%` | OK | OK |
| `22` | `5` | `5` | `4` | `3` | `3` | `3` | `16.53%` | `14.05%` | `15.29%` | OK | OK |
| `23` | `5` | `4` | `5` | `4` | `3` | `3` | `10.21%` | `8.13%` | `9.26%` | OK | **X** |
| `24` **<- həd** | `5` | `5` | `5` | `4` | `3` | `3` | `6.77%` | `5.56%` | `5.56%` | OK | OK |
| `25` | `5` | `5` | `4` | `4` | `3` | `3` | `6.56%` | `5.44%` | `4.80%` | OK | OK |
| `26` | `5` | `5` | `5` | `5` | `3` | `3` | `14.64%` | `12.57%` | `13.02%` | OK | OK |
| `27` | `5` | `5` | `5` | `4` | `3` | `3` | `10.43%` | `8.09%` | `8.92%` | OK | OK |
| `28` | `5` | `5` | `5` | `4` | `3` | `3` | `13.65%` | `12.50%` | `12.63%` | OK | OK |
| `29` | `5` | `5` | `5` | `5` | `3` | `3` | `11.42%` | `10.46%` | `9.99%` | OK | OK |
| `30` | `5` | `5` | `5` | `5` | `3` | `3` | `11.33%` | `9.56%` | `9.89%` | OK | OK |
| `31` | `5` | `5` | `5` | `5` | `3` | `3` | `4.27%` | `2.81%` | `3.43%` | OK | OK |
| `32` | `5` | `5` | `5` | `4` | `3` | `3` | `8.59%` | `6.84%` | `6.93%` | OK | OK |
| `33` | `5` | `5` | `5` | `5` | `3` | `3` | `7.81%` | `6.89%` | `6.43%` | OK | OK |
| `34` | `5` | `5` | `5` | `5` | `3` | `3` | `6.06%` | `5.62%` | `5.80%` | OK | OK |
| `35` | `5` | `5` | `5` | `5` | `3` | `3` | `11.59%` | `9.80%` | `9.96%` | OK | OK |
| `36` | `5` | `5` | `5` | `5` | `3` | `3` | `9.65%` | `7.64%` | `7.72%` | OK | OK |
| `37` | `5` | `5` | `5` | `5` | `3` | `3` | `8.18%` | `7.16%` | `7.30%` | OK | OK |
| `38` | `5` | `5` | `5` | `5` | `3` | `3` | `6.79%` | `5.82%` | `5.96%` | OK | OK |
| `39` | `5` | `5` | `5` | `5` | `3` | `3` | `6.90%` | `5.65%` | `5.79%` | OK | OK |
| `40` | `5` | `5` | `5` | `5` | `3` | `3` | `6.69%` | `5.50%` | `5.69%` | OK | OK |
| `41` | `5` | `5` | `5` | `5` | `3` | `3` | `3.39%` | `2.08%` | `2.62%` | OK | OK |
| `42` | `5` | `5` | `5` | `5` | `3` | `3` | `8.50%` | `7.26%` | `7.48%` | OK | OK |
| `43` | `5` | `5` | `5` | `5` | `3` | `3` | `8.00%` | `7.36%` | `7.08%` | OK | OK |
| `44` | `5` | `5` | `5` | `5` | `3` | `3` | `6.97%` | `6.25%` | `6.40%` | OK | OK |
| `45` | `5` | `5` | `5` | `5` | `3` | `3` | `6.22%` | `4.74%` | `5.28%` | OK | OK |
| `46` | `5` | `5` | `5` | `5` | `3` | `3` | `7.33%` | `6.29%` | `6.57%` | OK | OK |
| `47` | `5` | `5` | `5` | `5` | `3` | `3` | `2.63%` | `2.26%` | `1.72%` | OK | OK |
| `48` | `5` | `5` | `5` | `5` | `3` | `3` | `2.69%` | `2.17%` | `2.30%` | OK | OK |
| `49` | `5` | `5` | `5` | `5` | `3` | `3` | `3.46%` | `2.71%` | `3.17%` | OK | OK |
| `50` | `5` | `5` | `5` | `5` | `3` | `3` | `4.64%` | `3.72%` | `3.36%` | OK | OK |
| `51` | `5` | `5` | `5` | `5` | `3` | `3` | `5.73%` | `4.61%` | `4.27%` | OK | OK |
| `52` | `5` | `5` | `5` | `5` | `3` | `3` | `5.84%` | `4.84%` | `4.77%` | OK | OK |
| `53` | `5` | `5` | `5` | `5` | `3` | `3` | `5.52%` | `4.70%` | `5.13%` | OK | OK |
| `54` | `5` | `5` | `5` | `5` | `3` | `3` | `6.04%` | `4.94%` | `4.94%` | OK | OK |
| `55` | `5` | `5` | `5` | `5` | `3` | `3` | `5.82%` | `4.83%` | `4.73%` | OK | OK |
| `56` | `5` | `5` | `5` | `5` | `3` | `3` | `4.24%` | `3.35%` | `3.48%` | OK | OK |
| `57` | `5` | `5` | `5` | `5` | `3` | `3` | `3.60%` | `2.86%` | `3.26%` | OK | OK |
| `58` | `5` | `5` | `5` | `5` | `3` | `3` | `4.49%` | `3.81%` | `3.81%` | OK | OK |
| `59` | `5` | `5` | `5` | `5` | `3` | `3` | `6.49%` | `5.54%` | `5.43%` | OK | OK |
| `60` | `5` | `5` | `5` | `5` | `3` | `3` | `6.64%` | `5.56%` | `6.00%` | OK | OK |
| `61` | `5` | `5` | `5` | `5` | `3` | `3` | `4.70%` | `3.63%` | `4.03%` | OK | OK |
| `62` | `5` | `5` | `5` | `5` | `3` | `3` | `5.02%` | `4.40%` | `4.47%` | OK | OK |
| `63` | `5` | `5` | `5` | `5` | `3` | `3` | `4.96%` | `4.28%` | `4.01%` | OK | OK |
| `64` | `5` | `5` | `5` | `5` | `3` | `3` | `3.69%` | `3.00%` | `3.10%` | OK | OK |

Həddən aşağı uğursuz ölçülər: `16, 17, 18, 23`.

---

## 5. Həd = `24 px`

Bu rəqəm seçilməyib, yuxarıdakı cədvəldən çıxıb.

- **Həddin altındakı son uğursuzluq `23 px`-dir**: orada `50%` həddində `8`-qonşu sayı `4`-ə düşür
  (`leg` ilə `crossbar` künc toxunuşu), yəni R2 pozulur.
- **Ən pisi `16 px`-dir**: `25%` həddində sayı `2`-yə
  düşür - çılpaq `K`-nın `3`-ündən **az**, yəni R1 pozulur. Bu, tirin
  qurduğu körpüdür.
- `24 px`-dən yuxarı `64` px-ə qədər **hər** ölçü hər iki qaydanı keçir.

**Müstəqil təsdiq.** `02-BRAND-GUIDELINES.md` bölmə `6` onsuz da
`24 px`-i "təmiz döşəmə" adlandırırdı - amma tamam başqa dəlilə,
lövhə aralıqlarının optik ölçülməsinə söykənərək. İki müstəqil ölçmə
eyni rəqəmə gəlir; heç biri o birinə uyğunlaşdırılmayıb.

---

## 6. Yenidən qurulan fayllar

Qayda **faylın öz piksel ölçüsünə** yox, **nişanın çəkildiyi lövhə
pikselinə** tətbiq olunur - plitə üçün bunlar fərqli rəqəmlərdir:

```
  02-logo-png/mono/kestridge-mark-positive-<px>.png     lövhə = px
  07-platforms/<plitə>                        lövhə = min(w, h) * frac / 0.75
```

| Fayl | Lövhə px | Kəsik | sha256 tam kəsik | sha256 göndərilən |
|---|---|---|---|---|
| `02-logo-png/mono/kestridge-mark-positive-16.png` | `16` | `reduced` | `cd50e19221b6` | `9a0c516ba0a4` |
| `07-platforms/veb-standartlari-favicon-klassik-olchu-16x16.png` | `16` | `reduced` | `06156a70e140` | `df43bc510c1f` |
| `07-platforms/veb-standartlari-favicon-chox-olchulu-deste-html-standard-numunesi-16x16.png` | `16` | `reduced` | `06156a70e140` | `df43bc510c1f` |

"Tam kəsik" sütunu **yadda saxlanmır, yenidən törədilir**: həmin
ölçüdə eyni boru xətti ilə çəkilmiş beş lövhəli variantın hash-ıdır.
Ona görə `reduced.py`-ni iki dəfə işə salmaq sütunu özünə çevirmir.

Fayl adındakı `main` qalır, çünki **həndəsə** əsas kəsikdir: dəyişən
lövhə sayıdır, çəkiliş deyil. Ad dəyişsəydi istinad edən hər HTML və
hər manifest sınardı, halbuki fayl eyni slotun eyni aktividir.

### 6.1 Toxunulmayanlar - ölçülmüş

"Həddən yuxarı hər şey toxunulmadı" iddiası yalnız ölçülərlə dəyər
daşıyır. Göndərilən hər səthdə nişanın düşdüyü lövhə pikseli:

| Yer | Növ | Səth | Həddən aşağı | Qalanların ən kiçik lövhə px-i |
|---|---|---|---|---|
| `02-logo-png` | kilid | `20` | `0` | `44.59` (kestridge-lockup-stacked-default-positive-64.png) |
| `02-logo-png` | nishan | `11` | `1` | `24.00` (kestridge-mark-positive-24.png) |
| `07-platforms` | kilid | `17` | `0` | `74.77` (linkedin-shirket-sehifesi-life-tab-custom-) |
| `07-platforms` | plite | `18` | `2` | `32.00` (veb-standartlari-favicon-chox-olchulu-dest) |
| `09-linkedin-banner` | kilid | `2` | `0` | `112.00` (kestridge-banner-linkedin-cover-1512x256.p) |

**Heç bir kilid səthi həddin altına düşmür.** Onların ən kiçiyi
`kestridge-lockup-stacked-default-positive-64.png` - `44.59` px lövhə, həddən
`20.59` px yuxarı. Banner kilidləri daha da
böyükdür. Qayda cəmi `3` faylı tutur - `02-logo-png`-dakı `16 px` nişan
və iki `16x16` favicon.

### 6.2 Vaterniki qayda tutmur - və niyə

`youtube-branding-watermark-video-uzerinde-abune-loqosu-150x150.png` **`150 px`** lövhədə çəkilir, yəni həddin çox
üzərindədir və **yenidən qurulmayıb**. Amma paket öz rasterinə
nəzarət edir, YouTube-un həmin faylı pleyerdə nə qədər kiçiltdiyinə
yox. Göndərilən baytlar üzərində ölçülmüş ikinci kiçiltmə:

| Ekran px | 50/4 | 50/8 | 25/4 | 25/8 | midton |
|---|---|---|---|---|---|
| `12` **körpülənir** | `3` | `3` | `1` | `1` | `31.25%` |
| `13` **körpülənir** | `5` | `3` | `2` | `2` | `20.71%` |
| `14` | `5` | `4` | `3` | `2` | `12.76%` |
| `15` | `5` | `5` | `3` | `3` | `12.44%` |
| `16` **körpülənir** | `5` | `5` | `2` | `2` | `10.94%` |
| `17` | `5` | `4` | `4` | `3` | `8.30%` |
| `18` | `5` | `4` | `4` | `3` | `16.67%` |
| `19` | `5` | `4` | `4` | `3` | `14.96%` |
| `20` | `5` | `5` | `5` | `4` | `11.00%` |
| `21` | `5` | `5` | `4` | `3` | `12.24%` |
| `22` | `5` | `4` | `4` | `3` | `14.26%` |
| `23` | `5` | `5` | `5` | `4` | `10.59%` |
| `24` | `5` | `5` | `5` | `4` | `7.12%` |
| `25` | `5` | `5` | `3` | `3` | `7.36%` |
| `26` | `5` | `5` | `5` | `5` | `14.35%` |
| `27` | `5` | `5` | `5` | `4` | `10.29%` |
| `28` | `5` | `5` | `5` | `5` | `10.97%` |
| `29` | `5` | `5` | `5` | `5` | `11.89%` |
| `30` | `5` | `5` | `5` | `5` | `11.11%` |
| `31` | `5` | `5` | `5` | `5` | `4.89%` |
| `32` | `5` | `5` | `5` | `4` | `7.32%` |

Bu, **paketin qüsuru deyil** - fayl tələb olunan ölçüdə göndərilir.
Amma rəqəmlər göstərir ki, platforma onu `16 px` ətrafına salsa,
eyni körpü qayıdır. Bunu bağlamağın yeganə dürüst yolu YouTube-un
faktiki görüntü ölçüsünü ölçməkdir; **ölçülməyib**, ona görə burada
qərar verilmir, yalnız yazılır.

---

## 7. İki eyni `16x16` favicon - qərar

Auditin tapıntısı: `07-platforms`-da iki `16x16` favicon bayt-bayt
eynidir. Doğrudur. Amma tapıntının ikinci şıqqı - "çoxölçülü sətir
`16/32/48` olmalıdır, bir ölçünün üç nüsxəsi yox" - **səhvdir**, və
silinmir, düzəldilir: çoxölçülü sətir **onsuz da** `16/32/48`-dir.

| Fayl | sha256 |
|---|---|
| `veb-standartlari-favicon-chox-olchulu-deste-html-standard-numunesi-16x16.png` | `df43bc510c1f2ba2` |
| `veb-standartlari-favicon-chox-olchulu-deste-html-standard-numunesi-32x32.png` | `65a388fe303cab06` |
| `veb-standartlari-favicon-chox-olchulu-deste-html-standard-numunesi-48x48.png` | `229ad24411be64b0` |
| `veb-standartlari-favicon-klassik-olchu-16x16.png` | `df43bc510c1f2ba2` |

Üç ayrı hash, üç ayrı ölçü. Dublikat çoxölçülü dəstin **daxilində**
deyil: o, çoxölçülü sətrin `16x16` üzvü ilə **ayrıca verified sətir**
olan `favicon - klassik olchu`-nun `16x16`-sı arasındadır. İki sətir,
eyni ölçü, eyni kompozisiya - deməli eyni pikselər.

**Qərar: hər ikisi qalır, eynilik açıqlanır.** Səbəblər:

1. **Paketin invariantı**: `build/platform-dimensions.json`-dakı hər
   `verified` sətir çəkilir, hər çəkilən fayl bir sətrə qayıdır. Birini
   silmək o sətri çatdırılmadan çıxarar və paketin öz "atlanan sətir"
   sayğacını yalançı edər.
2. **Naxış paketdə tək deyil.** Eyni ölçü + eyni kompozisiya başqa
   yerlərdə də bayt-bayt eyni fayl verir:

| sha256 | Fayllar |
|---|---|
| `22f35cecd27c` | `linkedin-shirket-sehifesi-company-page-logo-profile-400x400.png`<br>`x-profile-photo-avatar-400x400.png` |
| `d361d2ccc316` | `linkedin-shirket-sehifesi-feed-image-horizontal-single-image-ad-sponsored-content-1200x628.png`<br>`x-image-ad-promoted-shekli-1-91-1-x-business-ad-spec-1200x628.png` |
| `452b7d1f2916` | `linkedin-shirket-sehifesi-feed-image-square-single-image-ad-sponsored-content-1200x1200.png`<br>`x-image-ad-promoted-shekli-1-1-kvadrat-x-business-ad-spec-1200x1200.png` |
| `df43bc510c1f` | `veb-standartlari-favicon-chox-olchulu-deste-html-standard-numunesi-16x16.png`<br>`veb-standartlari-favicon-klassik-olchu-16x16.png` |

   Birini silmək bu cütlərin qalanı ilə ziddiyyət yaradardı; hamısını
   silmək isə platformaların gözlədiyi fayl adlarını aparardı.
3. **Dürüstlük gizlətmək deyil, açıqlamaqdır.** `platformv2.py` onsuz
   da `favicon_16_matches_classic` yoxlamasını aparırdı; çatışmayan
   şey onun çatdırılma sənədində görünməsi idi. İndi görünür.

Hər ikisi indi **reduktiv kəsiklə** gedir, ona görə eyni qalırlar - bu
gözləniləndir və yuxarıdakı cədvəldə təsdiqlənir.

---

## 8. Yenidən qurma

```
python tools/build/mark.py          # hendese + reduktiv kesik oz-sinagi
python tools/build/reduced.py         # olchme + hedden ashagi artefaktlar
python tools/build/reduced.py --table # yalniz olchme, yazmadan
```

`masters.py` və `platformv2.py` qaydanı **öz içində** tətbiq edir
(`markv2.cut_for`), ona görə paketin tam yenidən qurulması reduktiv
kəsiyi geri qaytarmır.
