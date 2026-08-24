# Kestridge - brend qaydaları

| | |
|---|---|
| Versiya | 1.0 |
| Tarix | 2026-08-24 |
| Bazar | Amerika Birləşmiş Ştatları, brend dili İngilis |
| Mənbələr | `concept/00-CONCEPT-BRIEF.md`, `build/00-BUILD-RECORD.md`, `build/01-DECISION-RECORD.md`, `brand-identity-2026/DESIGN-LANGUAGE.md` |
| Fayllar | `delivery/` |

Bu sənəddəki hər rəqəm ölçülüb. Ölçmə skriptləri `tools/build/` altındadır və
hər biri birbaşa işə salınanda öz sınağını verir. Rəqəm dəyişsə sınaq düşür.

**Bu sənədin mənbəyi `build/02-BRAND-GUIDELINES.md`-dir.** `delivery/` altındakı
nüsxə generasiya edilir; orada redaktə etmək itir.

---

## 1. Marka nədir

**Hazırkı kilid `KESTRIDGE AI`-dır.** Tam böyük hərf, heç vaxt qısaldılmır.
`KESTRIDGE` tək **ehtiyat markadır**, heç bir yenidən çəkmə tələb etmir və
qabaritləri ölçülüb (bölmə 5).

### 1.1 Qərar bağlanıb - sahib 2026-08-24-də təsdiqlədi

Bu qərar bir müddət şərti qaldı. Artıq deyil.

**Sahibin cavabı: `AI` hüquqi şəxs adında qalır.** Yəni şirkətin adı
`Kestridge AI`-dır. Canlı sayt da bunu təsdiqləyir: səhifə başlığı
`Kestridge AI | AI, Automation and IT Security in Illinois`.

Bu, hər iki qapını eyni anda bağlayır:

| Qapı | Nə idi | Nəticə |
|---|---|---|
| **U1** | Hüquqi şəxs adından `AI` düşürmü | **Düşmür.** Ad `Kestridge AI` |
| **U2** | `02-collision-scan` şərt 5 düzəliş tələb edirmi | **Etmir.** Şərt onsuz da ödənir |

Şərt 5 kilidin həmişə tam `Kestridge AI` olmasını tələb edirdi; ad dəyişmədiyi
üçün pozuntu yoxdur və düzəliş sətrinə ehtiyac qalmır. `03-wordmark-typography.md`
sətir 439 - *"`AI` taqlayn deyil, şirkətin hüquqi adının hissəsidir"* - indi
hüquqi faktla üst-üstə düşür.

**Nəticə: `KESTRIDGE AI` qəti əsas kiliddir.** `KESTRIDGE` tək ehtiyat marka
olaraq qalır və paketdə `reserve` etiketi ilə gəlir - ad gələcəkdə dəyişsə
yenidən çəkmə lazım deyil, `tools/build/package.py`-dakı `lockup_gates_open`
bayrağı çevrilir.

`US-TRADEMARK-SCAN.md` bölmə 8 bənd 8 hələ də qüvvədədir və **ziddiyyət deyil**:
ticarət nişanı ərizəsi `KESTRIDGE` sözmarkası üçün verilir, çünki `AI` təsviri
sözdür və qoruma vermir. **Ərizə adı ilə görünüş adı ayrı şeylərdir.**
Ərizə bu paketin əhatəsində deyil (sahibin göstərişi).

Tam əsaslandırma və təkzib zənciri: `build/01-DECISION-RECORD.md` bölmə 2.

---

## 2. Nişan

### 2.1 Konsept

Üç ayrılmış lövhə: bir şaquli gövdə, iki əks istiqamətli diaqonal. Onları
`K` edən yeganə şey **bir sabitdir**: hər aralıq dəqiq `c = 2.7u`.

**Qovşaq yoxdur.** Üç kontur heç bir yerdə bir-birinə toxunmur. Şrift qlifi
belə ola bilməz, çünki qlif bağlı olmalıdır - müdafiə burada siluetdə deyil,
**topologiyadadır**.

### 2.2 Ölçülmüş həndəsə

| Parametr | Dəyər |
|---|---|
| Çərçivə | `viewBox="0 0 24 24"`, `u = 1/24` |
| **`c`** | **`2.7u`** - nişanın **yeganə** parametri, bütün aralıqlar |
| `k` (faslə) | `c/4 = 0.675u` |
| `w` (gövdə eni) | `4.0u` əsas kəsik, `4.4u` sıx kəsik |
| `a` (qol qalınlığı) | `5.0u` əsas kəsik, `5.5u` sıx kəsik |
| Bucaq ailəsi | **0 / 45 / 90**. Dördüncü istiqamət yoxdur |
| Künc radiusu | **0**, hər yerdə |
| Kontur | yalnız `fill`. `stroke` yoxdur, əyri seqment yoxdur |
| Təpə sayı | **16** (4 + 6 + 6) |

Ölçmə ilə təsdiqlənən nəticələr (əsas kəsik):

| Nə | Ölçülən |
|---|---|
| Mürəkkəb qutusu | **`18.000 x 18.000`** - dəqiq kvadrat |
| Kənar boşluq | `3.0u` hər dörd tərəfdən |
| Ən kiçik daxili bucaq | **`90.0`** dərəcə, hər üç konturda |
| Üç aralıq | **`2.7000`** - üzdən üzə |
| Mürəkkəb payı | gövdə `53.7%`, ayaq `28.2%`, qol `18.1%` |
| Qol/gövdə çəki nisbəti | `0.8839` |
| Bağlanma eyniliyi | `w + a - 2k = 7.65` |
| Asimmetriya | `2.7` **iki ortoqonal oxda** |

**Bağlanma eyniliyi pozulsa mürəkkəb qutusu kvadrat olmur.** Bu, nişanı
dəyişməzdən əvvəl yoxlanan bir sətirlik testdir.

### 2.3 Niyə şevron oxunmur

İki ayrılmış diaqonal `<` kimi oxuna bilərdi. Üç ölçülmüş əlamət bunu dayandırır:

1. **Güzgü simmetriyası yoxdur** - ayağın ucu `21`, qolun ucu `18.3`.
2. **Qollar sərbəst dayanmır** - aralıqda `4u` gövdə durur və mürəkkəbin
   `53.7` faizini daşıyır.
3. **Qollar bərabər deyil** - ox uzunluqları `10.82u` və `7.00u`.

Şevron bərabər qollar və simmetriya tələb edir. Heç biri yoxdur.

### 2.4 Nişan güzgüyə salınmır - heç vaxt

Güzgü `K`-nı öldürür **və** eyni anda simmetriyanı bərpa edib şevron
oxunuşunu geri gətirir. Ərəb qrafikalı materialda nişan söz işarəsinin
**sağına** qoyulur, forması dəyişmir.

---

## 3. Rəng

Palitra `brand-identity-2026/DESIGN-LANGUAGE.md` bölmə 2-dən bütöv götürülür.
Burada yalnız **nişana aid** hissə var.

### 3.1 Nişanın rəngləri

| Rol | Token | HEX | Nə vaxt |
|---|---|---|---|
| **Müsbət** | `neutral-950` | **`#0F1317`** | açıq fonda, default |
| **Knockout** | `neutral-025` | **`#FAFAF7`** | tünd fonda |
| Mono, tünd | - | `#000000` | tək rəngli çap, faks, oyma |
| Mono, açıq | - | `#FFFFFF` | tünd tək rəngli səth |
| Kağız | `neutral-025` | `#FAFAF7` | |
| Tünd səth | `neutral-900` | `#171D23` | |

**Nişan petrol rəngdə çəkilmir.** `petrol-500` `#1187A5` aksentdir - ikon,
sərhəd, fokus halqası, qrafik ştrix. Marka neytral mürəkkəbdə qalır.

`#1B2430` **silinib**. O, briefin öz sözü ilə "brend rəngi deyil" idi
(brief 4.1) və plaseholder kimi qalması təhlükəli idi.

### 3.2 Kontrast - ölçülmüş

Palitranın öz qaydası (`DESIGN-LANGUAGE.md` 2.11 bənd 6): **marka istənilən
fonda `>= 3:1`**. Loqotip WCAG-dən azaddır, bu hədd özümüzün qoyduğumuzdur.

| Fon | Müsbət `#0F1317` | Knockout `#FAFAF7` |
|---|---|---|
| Paper `#FAFAF7` | **17.84** | 1.00 |
| White `#FFFFFF` | **18.65** | 1.05 |
| `neutral-150` `#DDE2E6` | **14.30** | 1.25 |
| Surface `#171D23` | 1.10 | **16.24** |
| Ink `#0F1317` | 1.00 | **17.84** |
| `petrol-500` `#1187A5` | **4.47** | **3.99** |
| `petrol-700` `#0B5568` | 2.23 | **7.99** |

Hər fonda ən azı bir variant qaydanı keçir. `petrol-500` üzərində **ikisi də**
keçir - orada seçim estetikdir. `neutral-150` üzərində yalnız müsbət variant
işləyir.

### 3.3 Qadağalar

- **Qradiyent yoxdur** - nə markada, nə səthdə. Marka tək fill ilə düzlənəndə
  yaşamalıdır və yaşayır.
- **İkinci brend çaları yoxdur.** `petrol-*` yeganə xromatik aksentdir.
- **Semantik rənglər** (`success` / `warning` / `danger` / `info`) interfeys
  mülkiyyətidir, marketinq materialında dekorativ işlədilmir.
- Qırmızı + təhlükəsizlik + bucaqlı işarə üçlüyündən uzaq durulur (CrowdStrike,
  Fortinet, Databricks doymuş sahədir).

---

## 4. Tipoqrafika

### 4.1 Yığın

| Rol | Ailə | Versiya | Lisenziya |
|---|---|---|---|
| Sözmarka / display | **Archivo** | 2.001 | SIL OFL 1.1 |
| İnterfeys / mətn | **Inter Variable** | 4.001 | SIL OFL 1.1 |
| Kod / data | **JetBrains Mono** | 2.211 | SIL OFL 1.1 |

Versiyalar TTF-lərin `name` cədvəlindən oxunub. Lisenziya faylları paketdədir
(`06-fonts/`) - OFL 1.1 şrift paylananda lisenziyanın da getməsini tələb edir.

### 4.2 Sözmarka spesifikasiyası

| Parametr | Dəyər |
|---|---|
| Çəki | `wght 600` (stem `130 pm em`, cap-ın `19.0` faizi) |
| En | `wdth 100` yatıq kiliddə, `wdth 70` stacked və dar konteynerdə |
| Registr | **tam BÖYÜK HƏRF** |
| Söz boşluğu | `0.45 x cap` = **`309 pm em`**, tracking-dən **asılı deyil** |

**Vahid qaydası:** bu sənəddə `pm` **em-in mində biridir**. Archivo-nun `cap`-ı
`686` vahiddir, ona görə `pm cap` ilə qarışdırılsa nəticə `1.458` dəfə səhv
çıxar. CSS-də `pm em` birbaşa `em`-dir: tracking `-14` -> `letter-spacing: -0.014em`.

### 4.3 Cüt düzəlişləri - məcburi

Heç bir şrift `T-R` cütünü kernləmir, Archivo `S-T`-ni də kernləmir. Bu
düzəlişlər olmadan sözmarka ucuz görünür.

| Cüt | K-E | E-S | S-T | T-R | R-I | I-D | D-G | G-E | A-I |
|---|---|---|---|---|---|---|---|---|---|
| **pm em** | -19 | -58 | -39 | -39 | +18 | +25 | +5 | +4 | 0 |

Statik loqo fayllarında marka **kontura çevrilir** və bu problem qalxmır.
Canlı mətndə düzəlişlər hərf-hərf `<span>` ilə verilir.

### 4.4 Tracking

```
tracking(pm em) = -24.2 * log2(cap_px / 48),   [-30, +30] araligina kesilmish
```

| cap hündürlüyü | tracking | CSS |
|---|---|---|
| <= 20 px | +30 | `+0.030em` |
| 32 px | +14 | `+0.014em` |
| **48 px (etalon)** | **0** | `0` |
| **72 px (əsas yatıq kilid)** | **-14** | `-0.014em` |
| >= 128 px | -30 | `-0.030em` |

**Tracking `KESTRIDGE AI` sətrində 10 aralığa verilir, 9-a yox.** Boşluqdan
**əvvəlki** aralıq da tracking alır; boşluğun özünün eni isə sabit qalır
(`0.45 x cap`). Bir aralıq atlansa sətir dəqiq `14/686 = 0.0204` cap vahidi
enliyir - bu, build fazasında tapılan və düzəldilən qüsurdur.

### 4.5 Ölçülmüş qabaritlər

Cüt düzəlişləri tətbiq olunmuş, tracking `-14`:

| Sətir | `wdth` | `en/cap` | `hünd/cap` | ink AR |
|---|---|---|---|---|
| **`KESTRIDGE AI`** | 100 | **9.8407** | 1.0350 | **9.508** |
| **`KESTRIDGE AI`** | 70 | **7.3800** | 1.0364 | **7.121** |
| `KESTRIDGE` | 100 | 8.0175 | 1.0350 | 7.746 |
| `KESTRIDGE` | 70 | 5.9169 | 1.0364 | 5.709 |

**`hünd/cap` və `AR` sütunları briefin 5.7 cədvəlindən fərqlidir və burada
düzgün olanlardır.** Brief 5.7 `1.030` yazır, amma brief 5.3-ün özü `G`
overshoot-unu `12 pm` verir: `686 + 2 x 12 = 710`, yəni `1.0350`. Şrift
faylından ölçüldü və `1.0350` təsdiqləndi. `AR = (en/cap) / (hünd/cap)`, ona
görə briefin AR sütunu səhv hündürlüklə hesablanıb.

Kiçik düzəliş: brief 5.3 "**yalnız** `G`-nin overshoot-u" deyir. Ölçmə iki
qlif tapır - `G` **və** `S`, hər ikisi eyni `12` vahid.

---

## 5. Kilidlər

`M` = nişanın mürəkkəb hündürlüyü = `18u`. `C` = sözmarkanın cap hündürlüyü.
Hər ölçü `c`-nin qatıdır.

### 5.1 Yatıq kilid

| Parametr | Dəyər |
|---|---|
| Cap hündürlüyü | **`C = 5c = 13.5u`** |
| Ölçü nisbəti | **`M / C = 4/3`** |
| Aralıq | **`1.5c = 4.05u`** - nişanın ink sağ kənarından sözmarkanın ink sol kənarına |
| Şaquli düzülmə | nişanın ink mərkəzi = sözmarkanın **cap zolağının** mərkəzi |
| Sözmarka eni | `wdth 100`, tracking `-14` |

| Sətir | Bütöv ink | AR |
|---|---|---|
| **`KESTRIDGE AI`** (default) | **`154.899u x 18u`** | **8.6055** |
| `KESTRIDGE` (ehtiyat) | `130.286u x 18u` | 7.2381 |

Overshoot optikdir və düzülməyə **girmir**.

### 5.2 Stacked kilid

| Parametr | Dəyər |
|---|---|
| Cap hündürlüyü | **`C = 2.5c = 6.75u`** |
| Ölçü nisbəti | **`M / C = 8/3`** |
| Sistem qaydası | stacked-də nişan sözmarkaya görə **dəqiq iki dəfə** böyükdür |
| Şaquli aralıq | `1.5c = 4.05u`, nişanın ink altından sözmarkanın **cap xəttinə** |
| Sözmarka eni | `wdth 70`, tracking `-14` |

| Sətir | Bütöv blok | AR |
|---|---|---|
| **`KESTRIDGE AI`** (default) | **`49.82u x 29.05u`** | **1.715** |
| `KESTRIDGE` (ehtiyat) | `39.94u x 29.05u` | 1.375 |

### 5.3 Yalnız nişan

| Kontekst | Qayda |
|---|---|
| Kvadrat plitə (app icon, favicon, avatar) | **ink = `0.75` x plitə kənarı** |
| **Dairəvi kəsim** (LinkedIn, WhatsApp Business, Teams) | **ink = `0.70` x plitə kənarı** |
| Minimum təmiz sahə | **`c = 2.7u = 0.15 M`** hər tərəfdən |

**Dairəvi qayda məcburidir.** Ölçülmüş səbəb: nişanın ink qutusunun
yarımdiaqonalı `12.728u`-dur, öz `24u` plitəsinin daxili dairəsindən (`12.0u`)
böyükdür. `0.75` ilə dairəvi kəsim **ayağın ucunu kəsir**. `0.70`-də
yarımdiaqonal `11.879u` olur və içəridə qalır.

Sıx kəsik üçün də yoxlanılıb: `0.70`-də `11.600u`. Qayda hər iki kəsik üçün
etibarlıdır - bu, briefdə yazılmayıb.

### 5.4 Deskriptor sətri

**`AUTOMATION SECURITY ANALYTICS`**

| Parametr | Dəyər |
|---|---|
| Ailə, çəki, en | Archivo 2.001, **`wght 500`**, `wdth 100` |
| Registr | tam BÖYÜK HƏRF |
| **Cap hündürlüyü** | **`C_d = 1.5c = 4.05u`** (`= 0.30 C`) |
| **Tracking** | **`+180` pm em** (`0.180em`) |
| Söz boşluğu | **şriftin öz boşluğu** (`205u`) üstəgəl tracking. Sözmarkanın `0.45 x cap` qaydası burada **tətbiq olunmur** |
| Üfüqi düzülmə | sözmarkanın **ink sol kənarı** ilə flush |
| Şaquli yer | baza xətti sözmarka baza xəttindən **`3.5c = 9.45u`** aşağı |
| Kilid + deskriptor ink | **`155.009u x 25.271u`**, AR **`6.134`** |

Dəqiq flush `+179.336` olardı; `+180` seçilib və eni cəmi `+0.083` faiz artırır.
Bir onluq yerinə tam ədəd - sistemdə sabit sayı artmır.

**`AI` deskriptorda yazılmır.** O, kilidin özündədir; iki sətir aşağıda təkrarı
sətri markanın onsuz da sahib olduğu sözə xərcləyərdi. Kilid və deskriptor
birlikdə dörd xidmət sütununu **təkrarsız** verir.

**`DATA` də yazılmır.** Söz Kestra-nın `7544165` nömrəli qeydiyyatının öz
mətnindədir; `US-TRADEMARK-SCAN.md` bölmə 8 bənd 7 həmin dilin təkrarından
çəkinməyi tələb edir və analitika tərəfini adı ilə tövsiyə edir.
`IT SECURITY` rədd edildi (Kestra mətni hərfən *"information technology
departments"* deyir), `CYBERSECURITY` rədd edildi (hüquqi fayda yox, sətir uzanır).

**Deskriptor kilidin hissəsi deyil.** Kilid faylı yalnız nişan + sözmarkadır.
Deskriptor ayrıca variant faylıdır: **mətni versiyalana bilər, həndəsəsi yox.**

**İşlənmir:**

- deskriptor cap `9 px`-dən kiçik olanda (kilid eni `344 px`);
- **stacked kiliddə** - eyni `0.30` nisbətdə flush üçün tracking `-21.62` pm em
  lazım gəlir, yəni mənfi; qayda prinsipcə ödənmir;
- yalnız-nişan halında (app icon, favicon, avatar).

---

## 6. Reduktiv davranış

| Ölçü | Kəsik | Nə olur |
|---|---|---|
| 512 px | əsas | tam spesifikasiya, faslələr qərar kimi oxunur |
| 128 px | əsas | faslələr hələ görünür, konsept tam oxunur |
| 48 px | əsas | faslələr optik yox olur - zərərsiz, onlar istehsal xüsusiyyətidir |
| 32 px | əsas | keçid nöqtəsi, bax aşağı |
| 16 px | əsas | `K` oxunur, konsept oxunmur |

**Nişanın döşəməsi 16 px-dir.** Bundan aşağı işlədilmir.

### 6.1 Sıx kəsik geri götürüldü - bir master var

Konsept briefi `<= 32 px` üçün ikinci masteri - sıx kəsiyi, `+10` faiz ştrix -
**məcburi** etmişdi. **Bu qərar geri götürülüb.** Nişanın 16 px döşəməsinə
qədər yeganə masteri əsas kəsikdir.

**Birinci səbəb: sıx kəsik piksel şəbəkəsi düzülüşünü pozurdu.** Əsas kəsiyin
qutusu `3u ... 21u`, yəni lövhənin `1/8` və `7/8`-idir və 16 / 32 / 48 / 128 /
512 px-də **tam piksel sərhədinə** düşür. Sıx kəsiyin qutusu `18.9u`-dur və
heç bir ölçüdə düşmür (16 px-də `1.700 ... 14.300`). Brief §3.3 bu düzülüşü
öz üstünlüyü kimi iddia edir - sıx kəsik məhz onu ləğv edirdi.

**İkinci səbəb: rəng qərarı solğunluq problemini onsuz da həll etdi.** Briefin
`0.781` hədəfi müvəqqəti mürəkkəb `#1B2430` üçün idi. `neutral-950` ilə əsas
kəsik 16 px-də `0.7644` verir - hədəfdən tünd.

**Chromium-da təsdiqləndi** (`device_scale_factor=1`, briefin öz mühərriki):

| px | Kəsik | Orta parlaqlıq | Saf piksel |
|---|---|---|---|
| 16 | **əsas** | **0.7703** | **69.1%** |
| 16 | sıx | 0.7497 | 64.1% |
| 32 | **əsas** | **0.7659** | **71.5%** |
| 32 | sıx | 0.7445 | 67.5% |

Sıx kəsik `0.021` tündlük qazanır və `5.1` faiz bənd saf piksel itirir.
Mübadilə qazanc deyil.

**Nəticə:** briefin `risk 5`-i (*"iki master real xərcdir"*) bağlanır - ikinci
master yoxdur. `04r-plate-dense.svg` eskizi arxivdə qalır və `mark.py`
öz-sınağı onu hələ də yoxlayır, amma paketdə çəkilmir və heç bir qaydada
adı çəkilmir.

**Sınanmamış qalan:** Windows tapşırıq paneli və macOS dock. Hər ikisi öz
miqyaslama boru xəttini işlədir. Qərarı dəyişməsi ehtimalı aşağıdır - Chromium
və PIL arasındakı fərq onsuz da `0.007`-dən kiçikdir - amma dürüstlük üçün
yazılır.

### 6.2 Kiçik ölçü döşəmələri - ölçülmüş

`US-TYPEFACE-PERCEPTION.md` kiçik ölçüdə `wdth 100`-dən aşağı düşməyi qadağan
edir, stacked kilid isə konstruksiyaca `wdth 70` işlədir. Ziddiyyət qadağanı
**en dəyəri üzərindən deyil, ölçülmüş cap piksel döşəməsi üzərindən** yenidən
yazmaqla həll olunur - counter-lər bağlandığı yer ölçülə bilir:

| Nə | Döşəmə |
|---|---|
| Nişan (bütün hallarda) | **`16 px`** ink hündürlüyü |
| Sözmarka `wdth 100` | cap **`>= 10 px`** |
| Sözmarka `wdth 70` | cap **`>= 12 px`** |
| **Stacked kilid** | **`64 px`** bütöv blok (nişan `32 px`, cap `12.5 px`) |
| Deskriptor | cap **`>= 9 px`** |

Yatıq kiliddə **bağlayıcı şərt nişanın öz döşəməsidir**, cap döşəməsi yox:
sözmarka cap `10 px`-ə çatanda nişan hələ `13.3 px`-dir. Hər iki qapı
`tools/build/lockup.py` `floor_ok()`-də yoxlanılır.

---

## 7. Fiziki istehsal

| Proses | Qayda |
|---|---|
| Tikmə, trafaret, oyma möhür | **yalnız tərs (knockout) kəsik** |
| Qravür, folqa, vinil kəsim | istənilən kəsik - ən kiçik künc `90` dərəcədir |

Ayrılmış lövhələr bərk sahədə **deşik** kimi çəkiləndə fiziki dayaq problemi
qalxmır və körpü lazım gəlmir. Körpü əlavə etmək konsepti pozur.

**Knockout avtomatik invertdir.** Eyni üç kontur, eyni koordinatlar, yalnız
mürəkkəb rəngi `#0F1317` -> `#FAFAF7`. Buna baxmayaraq **həmişə ayrıca master
fayl kimi yazılır** - istehsal tərəfi rəngi özü çevirməməlidir.

`DESIGN-LANGUAGE.md` 4.5 tərs marka üçün `6.25` faizlik ştrix nazilməsi tələb
edir. **Bu nişana adlı istisna ilə tətbiq olunmur.** Səbəb həndəsidir, zövq
deyil: nazilmə mürəkkəb qutusunu `17.4375 x 18.0000` edir (kvadrat deyil),
bağlanma eyniliyini `7.65`-dən `7.0875`-ə endirir, və qutu kənarını 16 / 32 /
48 / 128 px-də tam pikseldən çıxarır - yalnız 512 px-də sağ qalır. Üstəlik
`k = c/4` qanunu saxlanılsa `w + a = 9` çıxır, yəni nazilmə üçün üçüncü
şəbəkə-qanuni yol yoxdur.

Fiqur/zəmin çevrilməsi də mümkün deyil: şaquli kanal `x 7 -> 9.7` bütün
`y 3 -> 21` boyu kəsilmir, üfüqi kanal `y 10.65 -> 13.35` sağa açılır - üç
boşluq xarici sahənin bir bağlantılı hissəsidir.

**İlk istehsal partiyasında yoxlanılmalıdır:** tərs kəsiyin tikmədə davranışı,
və kiçik ölçüdə çapda şevron oxunuşunun dayandığı.

---

## 8. Fayl indeksi

```
delivery/
  01-master/      22  master SVG
                      kestridge-mark-main-{positive,knockout,mono-black,mono-white}.svg
                      kestridge-lockup-{horizontal,stacked}-{default,reserve}-<variant>.svg
                      kestridge-lockup-horizontal-descriptor-{positive,knockout}.svg
  02-raster/      71  PNG (alfa kanallı) + JPG + WEBP
  03-icons/        7  favicon.ico (16/24/32/48/64/128/256) + kvadrat/dairə plitələr
  04-pdf/          5  saf vektor PDF - şrift istinadı yoxdur, şəkil yoxdur
  05-collateral/  16  vizit kartı, letterhead, zərf, slaydlar, e-poçt imzası
                      hər biri üçün `-guides` variantı: bleed / trim / safe / zona
  06-fonts/        6  3 TTF + 3 OFL lisenziyası
  07-platform/    32  LinkedIn, X, YouTube, GitHub, OG / favicon / PWA
                      hər sətrin manifestdə rəsmi mənbə URL-i var
  08-site/         9  canlı sayta birbaşa düşən fayllar, Next.js adlandırması ilə
  *.md             4  build qeydi, qərar reyestri, bu sənəd, sayt qeydləri
  MANIFEST.json       hər fayl üçün sha256, ölçü, mənşə
```

**Sıx kəsik paketdə yoxdur** (bölmə 6.1). Bir master var.

**Platforma qovluğunda 14 slot çəkilmədi** - rəsmi mənbə ölçünü təsdiqləmədi.
Uydurulmuş ölçü çəkilmiş artefaktdan pisdir, çünki heç kim onu sual altına
almaz. Atlananların siyahısı `tools/build/platform.py` çıxışındadır.

**`default`** = hazırda işlədilən sətir = `KESTRIDGE AI`.
**`reserve`** = `KESTRIDGE`. Bölmə 1.1-dəki qapılar açılanda yerləri dəyişir.

Kolleteral şablonları **doldurulmamışdır**: şirkət hələ qeydiyyatdan keçməyib,
ünvan və telefon yoxdur, ona görə kontakt sətirləri uydurulmayıb. Hər şablonun
`-guides` faylı hansı qutuya nə düşəcəyini göstərir.

---

## 9. Qadağalar - güzəştsiz

1. **Nişan güzgüyə salınmır.**
2. **Ad qısaldılmır** - `Kestr`, `KAI`, `Kest`, `K.` yoxdur.
3. **Yalnız nişan variantı** heç vaxt `K`, `KAI`, `Kestr` mətn forması ilə
   yan-yana görünmür.
4. **Qradiyent, kölgə, parıltı, ulduzcuq yoxdur.** AI ulduzcuğu qadağandır -
   ölçmə göstərir ki, onu `17` faiz AI kimi, `73` faiz "sevimlilərə əlavə et"
   kimi oxuyur.
5. **Yırtıcı quş silueti işlədilmir.** CrowdStrike "the falcon logo" iddia edir.
6. **`c` dəyişmir.** Aralıq nişanın yeganə parametridir; onu dəyişmək markanı
   dəyişməkdir.
7. **Künc radiusu `0` qalır.**
8. **"Bizim bucağımız" formulası işlədilmir.** HP-nin sistemi sabit bucaq
   üzərindədir; bizimki sabit **aralıq**dır.
9. **Etimologiya danışılmır.** Heç bir materialda "adımız ... deməkdir"
   cümləsi yazılmır.
10. **"The kestrel ... edir" tipli cümlə qurulmur.** Mövcud tədqiqat **altı
    ayrı növ** üzərindədir; bir quşa yapışdırmaq skeptik oxucunun bir
    axtarışla tapacağı səhvdir. Ya cins səviyyəsində danışılır (`kestrels
    hover`), ya növ **adlandırılır**. ABŞ oxucusu `kestrel` deyəndə American
    kestrel (*Falco sparverius*) düşünür, briefdəki ölçmələr isə
    *Falco cenchroides* və *Falco tinnunculus* üzərindədir.
11. **Silsilə xətti (dağ, zirvə) vizual mənbə kimi işlədilmir** -
    `02-collision-scan.md` sətir 611 adı ilə qadağan edir. `US-TRADEMARK-SCAN.md`
    onu tövsiyə edir; ziddiyyət `01-DECISION-RECORD.md` U4-dədir və
    barışdırılmayıb. Seçilmiş nişan mücərrəd K lövhəsidir, ona görə bu gün
    pozuntu yoxdur.

Build fazasında yazılan hər mətn `narrative/04-FACT-GATE.md` bölmə 10 (qara
siyahı) və bölmə 11 (sarı siyahı) üzrə yoxlanır. Qara siyahı bəndi olan mətn
çıxmır.

---

## 10. Açıq maddələr

| # | Nə | Status |
|---|---|---|
| 1 | Kilid qapıları U1, U2 | **bağlandı** 2026-08-24 - bölmə 1.1 |
| 2 | Sıx kəsiyin statusu | **bağlandı** - geri götürüldü, bölmə 6.1 |
| 3 | Deskriptor sətrinin sözləri | **bağlandı** - bölmə 5.4 |
| 4 | Knockout master ziddiyyəti | **bağlandı** - bölmə 7 |
| 5 | `wdth 70` kiçik ölçü döşəməsi | **bağlandı** - bölmə 6.2 |
| 6 | Spot mürəkkəb (PMS) | **bağlandı** - elan olunmur, bölmə 3.4 |
| 7 | `brand-identity-2026` valideyn sistem | **bağlandı** - birtərəfli elan |
| 8 | Domen | **alınıb.** NS-lər hələ yerləşməyib |
| 9 | Ticarət nişanı ərizəsi, sənəd şablonu, sayt dizaynı | **əhatə xaricində** - sahibin göstərişi |
| 10 | Sosial handle-lar tutulmayıb | sahib |
| 11 | Şifahi identika - `-idge` sonluğu telefonda itir | ayrıca sənəd |
| 12 | Fiziki sınaq: tikmə, trafaret, oyma; Windows / macOS ikon boru xətti | ilk istehsal |

Tam qərar zənciri: `build/01-DECISION-RECORD.md` və
`build/open-items-decisions.json`.

**Ad verdikti AMBER-dir, şərtlidir.** İki tərəfdən sıxılma real və daimidir:
`Kestrel AI` (YC F25) fonetik olaraq, `Kestra` (`kestra.io`) funksional olaraq.
Federal USPTO sözmarka qatı təmizdir, qalan qatlar `unverified`.

**Brend kitabı, sayt lansmanı, müştəriyə gedən çap və USPTO ərizəsi ABŞ ticarət
nişanı vəkilinin yaşıl işığından asılıdır.** Bu sənəd hüquqi məsləhət deyil.
