# 07-platforms - qeydlər

Tarix: 2026-09-21 (v2), yenilənib 2026-09-26 (v3)
Vahid: L2-DES / L3-DES-GRPH
Generator: `tools/build/platformv2.py` (render mühərriki `tools/build/platform.py`)
Qovluq: `delivery/07-platforms/`

v2-də qovluq `07-platform/`, generator `v2-ai/build/platformv2.py`, ölçü
cədvəli `build/platform-dimensions.json`, yoxlama şəkilləri
`07-platform/_check/` idi. v2 halı olduğu kimi
`archive/v2-2026-09-21/delivery/07-platform/`-dadır.

`delivery/07-platforms/` platforma ölçüləri dəstidir: hər fayl təsdiqlənmiş
ölçü cədvəlindən (`tools/data/platform-dimensions.json`) gələn bir slotu
doldurur. Kompozisiya qaydası sadədir - kvadrat və dairəvi slotlarda nişan,
aspekt nisbəti >= 1.6 olan düzbucaqlı slotlarda mərkəzlənmiş yatıq kilid,
qalan düzbucaqlı slotlarda (720 x 900, 900 x 600, 2160 x 3840) mərkəzlənmiş
stacked kilid, hamısı kağız fonunda. Yoxlama
şəkilləri `qa/07-platforms/` altındadır (`check-*.png`), maşın oxunan siyahı
`qa/07-platforms/MANIFEST.json`-dadır.

## v3: bütün platforma şəkilləri rənglidir (2026-09-26)

`delivery/07-platforms/`-dakı 35 faylın hamısı `color` rəng yolundadır:
gövdə və sözmarka `#0F1317`, qol, nöqtə, ayaq və tir petrol-600 `#0E6A82`,
fon kağız `#FAFAF7`. Rəng yolu `tools/build/platformv2.py`-da
`PLAT.INK = markv2.COLOUR` ilə qurulur (`markv2` burada `tools/build/mark.py`-ın
import adıdır; `COLOUR` rəng yolu orada təyin olunub). Piksellərdən ölçüldü (2026-09-26):
35 PNG-nin 35-ində petrol-600 piksel var;
`archive/v2-2026-09-21/delivery/07-platform/`-dakı 35 PNG-nin heç birində
yoxdur - v2 tək mürəkkəb idi.

24 board px-dən aşağı reduktiv kəsikdə nöqtə və tir düşür, gövdə (ink) və
qol/ayaq (petrol) qalır, ona görə 16 px favicon da iki rənglidir.

## v3: yatıq kilidin ölçüsü düzəldildi (2026-09-26)

`tools/build/platform.py` yatıq kilidin PNG qutusunu kətan hündürlüyünün 0.30-u
götürürdü. Qutu hər tərəfdə `c` (2.7u) boşluq payı daşıyır, ona görə nişanın öz
mürəkkəbi kadrın 0.30 x 18 / 23.4 = 0.2308-inə (23.1 %) düşürdü. Təsdiqlənmiş
LinkedIn banneri nişanı 84 / 256 = 0.328-də (32.8 %) qoyur. Sahib platforma
örtüyünə baxıb kilidi "köhnə nisbət" kimi oxudu; halbuki M / C hər ikisində 2.0
idi (bannerdə hərf-hərf ölçüldü: düz hərflər K E T R I D A cap 42 px, nişan
84 px verir; S və G yalnız dairəvi hərf aşmasına görə 44 px oxunur). Fərq
çərçivələmədə idi: 1.42x.

Düzəliş: `HORIZONTAL_BOX = 0.328125 x (18 + 2 x 2.7) / 18 = 0.4266`,
`tools/build/platformv2.py`-da `PLAT.HORIZONTAL_BOX` kimi qurulur - qutu
bannerin nişan payından çıxarılır, əksinə yox. Aspekt nisbəti >= 1.6 olan
şəkillərə, yəni yatıq kilid kompozisiyalarına aiddir: `delivery/07-platforms/`-da
14 fayl (`qa/07-platforms/MANIFEST.json`, `composition` =
`yatiq kilid, merkezlenmish`).
Stacked kompozisiya (`STACKED_BOX` 0.34) dəyişməyib. 72 % en tavanı da
dəyişməyib və bu 14 faylın 10-unda bağlayır (aspekt nisbəti 1.778 ilə 2.0
arası, məs. 1200 x 628). Tavan v2-də də bağlayırdı, ona görə bu fayllarda
kilidin mürəkkəb eni v2 ilə 2 px daxilində eynidir. Kilid yalnız 4 faylda
böyüdü: 1512 x 256, 1235 x 338, 1128 x 376, 1500 x 500. Onlarda kilidin
mürəkkəb hündürlüyü kadrın 0.230 ilə 0.234 arası payından 0.330 ilə 0.332
arası payına qalxdı (2026-09-26, `archive/v2-2026-09-21/delivery/07-platform/`
ilə piksel müqayisəsi).

Qeyd: `qa/07-platforms/MANIFEST.json`-dakı `checks.lockup_slots` hələ köhnə
0.30 qutusu ilə hesablanır (`tools/build/platformv2.py`:
`th = int(round(h * (0.34 if stack else 0.30)))`). Tavanın bağlamadığı 4
faylda göstərilən `mark_px` göndərilmiş şəkildən kiçikdir; məs.
1512 x 256 üçün `59.23`, faylda isə kilidin mürəkkəb hündürlüyü 85 px-dir.

## LinkedIn `1512 x 256` örtüyü `07-platforms/`-dan getmir

| Fayl | Status |
|---|---|
| `delivery/07-platforms/linkedin-shirket-sehifesi-company-page-cover-banner-1512x256.png` | **kanonik deyil** - generik, açıq fonda mərkəzlənmiş kilid render-i. Ölçü və kompozisiya nümunəsi kimi qalır, LinkedIn şirkət səhifəsinə yüklənmir |
| `delivery/09-linkedin-banner/kestridge-banner-linkedin-cover-1512x256.png` | **kanonik** - sahibin seçdiyi tünd `b4-oversize-monogram` dizaynı (v3-də kilid `color-knockout`) |

Səbəb: eyni ölçünü iki fərqli dizayn doldurur və heç bir fayl adı bunu
demirdi. Qərar `01-DECISION-RECORD.md` D3-dədir, qayda
`02-BRAND-GUIDELINES.md` 8.1-dədir. `delivery/07-platforms/`-dakı digər
LinkedIn faylları (loqo, feed, Life tab, post şəkli) kanonik qalır - onlar
üçün ikinci dizayn yoxdur. v2-də kanonik fayl `09-banner/` altında idi.
