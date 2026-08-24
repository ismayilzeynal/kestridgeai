# Kestridge AI - qərar reyestri

| | |
|---|---|
| Layihə | `kestridge-ai-brand` |
| Sənəd | `DECISION-REGISTRY.md` |
| Tarix | 2026-08-24 |
| Rol | barışdırma hakimi. Altı ziddiyyət, altı hökm, bir icra siyahısı |
| Girişlər | altı barışdırma verdikti, doqquz adversarial təkzib, `concept/00-CONCEPT-BRIEF.md`, `concept/01-name-semantics.md`, `concept/02-collision-scan.md`, `concept/03-wordmark-typography.md`, `narrative/04-FACT-GATE.md`, `domain/01..03`, `US-TRADEMARK-SCAN.md`, `US-TYPEFACE-PERCEPTION.md`, `US-COLLATERAL-SPEC.md`, `FONT-COVERAGE.md`, `projects/brand-identity-2026/DESIGN-LANGUAGE.md` + `data/` |
| Statusun mənbəyi | hər hökm oxunmuş sətirlə bağlıdır. Oxunmayan yer `unknown` yazılır |

Bu sənəd qərar verir, müzakirə açmır. Bölmə 0 nəticəni, bölmə 1-6 əsası, `DƏYİŞİKLİK SİYAHISI` icranı, `HƏLL OLUNMAYAN` isə kimin nəyi qərar verməli olduğunu saxlayır.

**Ölçülmüş vəziyyət qeydi.** Reyestr yazılan anda `delivery/MANIFEST.json` (generated 2026-08-24T07:39:04Z, 116 fayl) artıq mövcuddur və `decisions` bloku belədir: `ink #0F1317`, `knockout #FAFAF7`, `paper #FAFAF7`, `accent #1187A5`, `primary_text KESTRIDGE`, `secondary_text KESTRIDGE AI`. Yəni build C2 və C3 hökmlərini reyestrdən **əvvəl** tətbiq edib. Bu, hökmü təsdiqləmir - hökm sənəd əsasında verilir; amma icra siyahısında "artıq edilib" sətirləri buna görə ayrıca işarələnir.

---

## 0. Bir baxışda

| # | Ziddiyyət | Hökm | Güvən | Build-i bloklayır? |
|---|---|---|---|---|
| **C1** | Bazar: brief "Azərbaycan" vs qalan dəst "ABŞ" | **ABŞ qalib.** Brief 2.1 düzəlir; Azərbaycan ölçmələri silinmir, "gələcək dil genişlənməsi" faktına enir. Archivo qalır, esaslandırması dəyişir | yüksək (təkzib yoxdur) | **Bəli** - 2.1-in iki xanası |
| **C2** | Kilid: `KESTRIDGE AI` vs `KESTRIDGE` + deskriptor | **ŞƏRTİ.** Kilid `KESTRIDGE`-ə keçir yalnız iki qapı ödənəndə: hüquqi şəxs adından "AI" düşməsi və `02-collision-scan` şərt 5-in rəsmi düzəlişi. Ödənməyincə default `KESTRIDGE AI` | **orta** (ölümcül təkzib endirdi) | **Bəli** |
| **C3** | Rəng: `#1B2430` müvəqqəti vs `DESIGN-LANGUAGE` palitrası | **Palitra tam götürülür.** Nişan `neutral-950 #0F1317` / knockout `neutral-025 #FAFAF7`. `petrol-500 #1187A5` aksent. `#1B2430` silinir | yüksək (nüvə), esaslandırma və siyahı düzəlişi ilə | **Bəli** |
| **C4** | Tipoqrafiya: üç sənəd, üç parametr dəsti | **Ailə yığını təsdiq** (Archivo 2.001 + Inter 4.001 + JetBrains Mono 2.211, hamısı OFL, RFN yoxdur). **wdth/tracking arbitrajı ŞƏRTİ** - "ziddiyyət" iddiası kateqoriya səhvi kimi rədd edildi | ailə: yüksək; parametr: **orta** | Qismən |
| **C5** | Söz işarəsi AR: 9.70 / 10.26 / 9.553 | **Təsnifat qəbul edilir** (üç fərqli kəmiyyət, bir zəncir). **Risk 13 BAĞLANMIR** - diskdə beş rəqəm var, ikisi işləyən koddur | təsnifat: yüksək; bağlanma: **rədd** | **Bəli** |
| **C6** | Ad verdikti: AMBER, risk 9/11/12 | **AMBER qalır.** Risk 9 ağırlaşır, risk 11 daralır (bağlanmır), risk 12 `.com`/`.ai`/`.io` üçün bağlanır. **Yaşıl işıq daralıb**: "identiklik" mənbədə vəkil qapısının arxasındadır | yüksək (tapıntı); **orta** (yaşıl işıq) | Qismən |

---

## 1. C1-BAZAR

### Nə idi
`concept/00-CONCEPT-BRIEF.md` icra spesifikasiyasıdır və sətir 84-də hələ "Kestridge AI **Azərbaycanda** dörd sahədə" yazır; ölçü meyarı isə (sətir 89-91) alıcını "SOCAR, banklar, nazirliklər, operatorlar, beynəlxalq inteqrator tenderi" kimi təyin edir. Bütün yeni ABŞ dəsti bunun əksini deyir.

### Hökm
**ABŞ tərəfi qalib gəlir.** Brief 2.1-in mövqe cümləsi və ölçü meyarı ABŞ dəyərləri ilə əvəz olunur. Archivo qərar kimi sağ qalır, amma 5.2-nin birinci arqumenti (Azərbaycan örtüyü qapısı) ölür və esaslandırma `US-TYPEFACE-PERCEPTION.md` 5.1-ə köçür. 5.3 registr qərarı (tam BÖYÜK HƏRF) toxunulmazdır - bazardan asılı deyil.

### Niyə
| Sitat | Yer |
|---|---|
| "Sirket ABS bazarina xidmet veren ingilisdilli B2B texnologiya sirketidir." | `project.yaml`, `notes` |
| "İstifadəçi dəqiqləşdirdi: **Kestridge AI ABŞ bazarı üçün ingilisdilli şirkətdir.** Azərbaycan diakritikaları artıq diskvalifikasiya səbəbi deyil." | `FONT-COVERAGE.md` |
| "Ölçmə silinmir - o, doğrudur və gələcək dil genişlənməsi üçün faktdır. Yalnız **nəticə** dəyişir." | `FONT-COVERAGE.md` |
| "Səkkiz namizədin hamısı ingilisdilli ABŞ brendi üçün **texniki cəhətdən uyğundur**. Heç biri glif örtüyünə görə düşmür." | `FONT-COVERAGE.md` |
| "**Azərbaycan örtüyü qapıdır və o, iyirmi ailədən səkkizini kəsir.**" | brief:442 - artıq heç nə kəsmir |
| "Alıcı - ABŞ korporativ qərar vericisi." | `US-TYPEFACE-PERCEPTION.md`:5 |

Üç müstəqil mənbə ABŞ deyir, bir sənəd (brief 2.1) Azərbaycan deyir. Layihə reyestrinin özü ABŞ yazır, ona görə brief düzəlir, əksi yox.

### Nə dəyişir
Brief 2.1 (bloklayıcı), 5.2 birinci abzas, risk 9/11/12/16, `01-name-semantics.md` sətir 11, `02-collision-scan.md` sətir 9 və bölmə 11 şərt 7-8, `03-wordmark-typography.md` sətir 9 və bölmə 2 başlığı, `narrative/03-story-craft.md` bölmə 8 bənd 1 (bağlanır).

**Təkzib:** yoxdur. Hökm tam qüvvədədir.

---

## 2. C2-KİLİD

### Nə idi
`domain/02-ai-suffix-longevity.md` bölmə 0: "**Loqo kilidi (lockup)** | Əsas markada \"AI\" **olmasın**. AI ayrıca **deskriptor sətrində** yaşasın". `US-TRADEMARK-SCAN.md` bənd 8: "`KESTRIDGE AI` yox, **`KESTRIDGE`** sözmarkasını qeyd etdir." Buna qarşı `domain/03-operational-test.md` 5c cədvəli "| Loqo | Bəli |" yazır və `02-collision-scan.md` şərt 5 (sətir 614-616) "Lockup həmişə tam `Kestridge AI`" tələb edir.

### Hökm - ŞƏRTİ
İlkin verdikt "`KESTRIDGE` əsas kilid, `AI` deskriptora keçir, `KESTRIDGE AI` ağ siyahılı ikinci dərəcəli variant" idi, güvəni **yüksək**. **Təkzibçi ölümcül qüsur tapdı, ona görə hökm şərti oldu.**

Kilid `KESTRIDGE`-ə keçir **yalnız hər iki qapı ödənəndə**:
1. **Hüquqi şəxs adı qapısı.** Qeydiyyat adından "AI" düşməlidir (`domain/02` bölmə 0, güvən "Yüksək"). Ödənməyincə `03-wordmark-typography.md` sətir 439-440 qüvvədədir.
2. **Şərt 5 qapısı.** `02-collision-scan.md` bölmə 11 rəsmi düzəliş sətri ilə yenilənməlidir. Düzəlişsiz keçid pozuntudur.

Qapılar ödənməyincə default kilid `KESTRIDGE AI`, ehtiyat marka `KESTRIDGE`-dir (`03-wordmark` sətir 623).

### Niyə - hökmün lehinə
| Sitat | Yer |
|---|---|
| "Variant A-da `AI`-ı silmək **pulsuzdur**: `KESTRIDGE` tək halda tam balanslı, tamamlanmış markadır (ölçülmüş ink AR 7.786)." | brief:575-577 |
| "- **Kestrel AI (YC F25).** İki hərf fərqi, eyni ` AI` suffiksi, dörd sahədən üçündə üst-üstə düşmə" | `02-collision-scan.md` 10.2 |
| "8. `KESTRIDGE AI` yox, **`KESTRIDGE`** sözmarkasını qeyd etdir. \"AI\" təsviri sözdür" | `US-TRADEMARK-SCAN.md`:453-454 |
| "**Amma xeyr, yalnız loqo da kifayət etmir.** Loqo şəkildir." | `domain/03`:376-378 - arqument maşınoxunan mətn haqqındadır, kilid haqqında yox |

Rəqəmlər iki müstəqil təkzibçi tərəfindən yenidən ölçülüb və üst-üstə düşüb: `KESTRIDGE` wdth 100 ink AR 7.746 / en 8.017 cap; wdth 70 5.711 / 5.917; deskriptor `AI AUTOMATION SECURITY DATA` 108.10-108.12u. **Uydurma rəqəm yoxdur.**

### Niyə hökm zəiflədi - ölümcül təkzib
Təkzibçi üç daşıyıcı sitatın şərtləndirici bənddən qoparıldığını və korpusun ən birbaşa əks mətninin heç göstərilmədiyini tapdı:

| Tapıntı | Sətir |
|---|---|
| "pulsuzdur" sitatının mövzu cümləsi atılıb: "2. *Brend **gələcəkdə** bu təyindən kənara çıxa bilsin və lockup qırılmasın.*" - gələcək müddəasıdır, indiki defolt icazəsi deyil | brief:574 |
| "`Kestridge` sətir olaraq praktiki şəkildə boşdur" cümləsi ortadan kəsilib; davamı əksini deyir: "Problem sətirdə deyil, **sətrin düşdüyü fonetik məhəllədədir**" | `02-collision-scan.md`:66-68 |
| Bölmə 11-in sanksiyası gizlədilib: "**Bu şərtlər pozulsa verdikt avtomatik qırmızıya keçir.**" | `02-collision-scan.md`:602 |
| Korpusun ən birbaşa əks hökmü sitat edilməyib: "`AI` taqlayn deyil, **şirkətin hüquqi adının hissəsidir**. Ona görə əsas markadan çıxarıla bilməz." | `03-wordmark-typography.md`:439-440 |
| "Yəni **A seçmək gələcəkdə E-yə keçidi pulsuz edir.**" - E gələcək opsiondur | `03-wordmark-typography.md`:447 |
| Variant E-nin öz hökmü: "**Ehtiyat marka kimi saxlanılır**" | `03-wordmark-typography.md`:425 |

İkinci təkzib (ciddi) əlavə etdi: `03-wordmark-typography.md` downstream siyahısında ümumiyyətlə yoxdur, halbuki brief:422 "Bütün rəqəmlər `03-wordmark-typography.md`-dən gəlir" yazır; must_become mətni briefdə **tərif edilməmiş** "variant E" adına istinad edir (brief 5.6 yalnız A, B, C, D tərif edir); 6.1/6.2 patch-ləri sətir 633 və 651-ə toxunmadığı üçün cədvəli öz-özünə zidd hala salır; bölmə 10 patch-i sətir 874-ü ("Söz işarəsi kontura çevrilir; cüt düzəlişləri konturda bişirilir.") silir; bölmə 7-yə çarpaz istinad yoxdur və ölçü bazası fərqlidir (bölmə 7 24u plitə, deskriptor döşəməsi 18u ink).

### Nə dəyişir
Bax `DƏYİŞİKLİK SİYAHISI`. Bütün C2 patch-ləri **iki qapı ödənənə qədər tətbiq edilmir**, istisna: `03-wordmark-typography.md` və `01-name-semantics.md`-in siyahıya əlavəsi, bölmə 6.1/6.2 patch-lərinin genişləndirilməsi, sətir 874-ün qorunması - bunlar hər halda düzəlməlidir.

---

## 3. C3-RƏNG

### Nə idi
Brief sətir 306: "| Müvəqqəti rəng | `#1B2430` (brend rəngi deyil, bax risk 11) |" (istinad da səhvdir - rəng riski 8-dir). Qonşu layihə `DESIGN-LANGUAGE.md` isə tam ölçülmüş palitra verir və `#1B2430` orada yoxdur.

### Hökm
`brand-identity-2026/DESIGN-LANGUAGE.md` bölmə 2 palitrası **tam və dəyişdirilmədən** götürülür. Nişan neytral mürəkkəbdə çəkilir: müsbət `neutral-950 #0F1317`, knockout `neutral-025 #FAFAF7`. `petrol-500 #1187A5` aksentdir. `#1B2430` repodan silinir.

### Niyə
| Sitat / ölçmə | Yer |
|---|---|
| Şərt 9 literal olaraq yalnız qırmızını hədəf alır: "Qırmızı + təhlükəsizlik + bucaqlı işarə üçlüyü artıq doymuş sahədir." | `02-collision-scan.md`:624-628 |
| dE2000: CrowdStrike `#FC0000` 61.09, Databricks `#FF3621` 60.30, Splunk `#FF6600` 50.26 | `DESIGN-LANGUAGE.md`:106-108 + `data/palette-report.txt` |
| "| **`petrol-500`** | **`#1187A5`** | ... 0.5786 0.1031 221.5 | 0.20164 | anchor, **BRAND** |" | `DESIGN-LANGUAGE.md`:178 |
| "6. **Kiçik ölçüdə solğunluq tam həll olunmayıb.** 16 px orta parlaqlıq: əsas kəsik 0.801, sıx kəsik 0.781." | brief risk 6 |
| "4. Başqa heç nə. Boz variant, kontur variant, konteyner içində variant yoxdur." | `DESIGN-LANGUAGE.md`:812 |

`petrol-500`-ün nisbi işıqlılığı `neutral-950`-dən 32 dəfə yüksəkdir (0.20164 vs 0.00629) - petrol nişan risk 6-nı ən kritik ölçüdə pisləşdirir. `#1B2430` OKLCh H 255.8 / C 0.0262 ilə pilləkənin soyuq onurğasından kənara düşür; `neutral-950`-dən 7.021 dE2000 uzaqdır.

Toqquşma yoxlaması (üç xarici fayl, bayt ölçüləri iki müstəqil yükləmə ilə üst-üstə düşdü): Kestra bənövşəyi-magenta (`#A950FF`, `#F62E76`), petroldan 30.24-62.26; Kestrel AI-ın `logo-cropped.svg`-də sıfır `fill` atributu var, monoxromdur; onların `--accent-cyan:#6bb0c0` petroldan 14.86 uzaqdır - Snowflake-in 15.57-sindən yaxın, amma brend rəngi deyil və işıqlılığı fərqlidir.

### Nə dəyişdi - iki ciddi təkzib
Hökmün nüvəsi sınmadı: 17 sitatın hamısı, 12 rəqəmin hamısı, üç xarici yükləmə bayt-dəqiq təsdiqləndi. **İki düzəliş məcburidir:**

1. **Mənbəyə zidd yazılış.** Verdiktin brief risk 8-ə yazacağı cümlə - "Aksent `petrol-500` yalnız **link**, fokus halqası, hairline, ikon və qrafik ştrixidir" - iki yerdə mənbəni pozur. `DESIGN-LANGUAGE.md`:327 deyir: "O, yalnız **marka**, ikon, sərhəd, fokus halqası və qrafik ştrixi kimi işlədilir"; :328 isə linki `petrol-600`/`petrol-400`-ə bağlayır, `petrol-500`-ə yox. **Düzəliş:** cümlə mənbənin öz siyahısına qaytarılır ("marka" qalır, "link" çıxır), və nişanın neytral mürəkkəbdə çəkilməsi mənbə qadağası kimi deyil, **layihə səviyyəsində seçim** kimi yazılır (`DESIGN-LANGUAGE.md`:809 "Tam `petrol-500` neytral fonda" icazəli tək rəngli vəziyyətlərin birincisidir).
2. **Siyahı natamam idi.** `1B2430` repoda **14 faylda 37 dəfə** keçir; ilkin siyahı 4 fayl / 10 keçid adlandırırdı. Ən ağırı: canlı generator kodunda hardcoded default var - `tools/build/mark.py:82`, `tools/build/lockup.py:48,78,107`, `tools/build/raster.py:37,58`, `tools/fix01.py`. Heç bir çağırış yeri `ink`-i açıq ötürmür. Əlavə unudulanlar: bölmə 7-nin bütün ölçmə bazası (`brief:690` "ağ fonda"), bölmə 11 fayl istinadları, `renders/13`, `renders/14`, `renders/_build/` (12 artefakt, 07:29), `domain/03`:171-172-dəki `geai` azaldıcısı (iki rəngə söykənir), `US-COLLATERAL-SPEC.md`:273, :280, :504-505.

**Nəticə:** hökm qüvvədədir, esaslandırma cümləsi və dəyişiklik siyahısı yenidən yazılıb. `delivery/MANIFEST.json` bunu artıq tətbiq edib (`ink #0F1317`), amma `tools/build/*` və eskiz SVG-lər hələ köhnə dəyəri daşıyır.

---

## 4. C4-TİPO

### Nə idi
Üç sənəd üç parametr dəsti verir: brief 5.1 `wdth 100` / `wdth 70`, `DESIGN-LANGUAGE.md`:527 `wdth 92 (söz marka), 100 (başlıq)`, `US-TYPEFACE-PERCEPTION.md`:336 "`wdth` optik olaraq 96-100 arası kökləmək". Tracking-də brief `-24.2 * log2(cap_px/48)`, DL:577 "+2.5%".

### Hökm
**Ailə yığını mübahisəsizdir və təsdiqlənir:** Archivo 2.001 (sözmarka/display) + Inter Variable 4.001 (UI/body) + JetBrains Mono 2.211 (kod/data), hamısı SIL OFL 1.1. `DESIGN-LANGUAGE.md`:397 və `US-TYPEFACE-PERCEPTION.md`:15-17 eyni üç ailəni eyni üç rolda verir. Diskdəki üç TTF-in name cədvəlində **Reserved Font Name yoxdur** - yenidən çəkmə, aralıq dəyişmə və öz adı ilə paylama OFL daxilində qanunidir.

**Düzəlişlər (bloklayıcı):**
- `US-TYPEFACE-PERCEPTION.md`:19-20 Inter üçün **yanlışdır**. Inter `rsms/inter` `InterVariable.ttf`-dən gəlir (879708 bayt, "Version 4.001;git-9221beed3"), Google Fonts-dan yox. Sənəddəki bütün Inter rəqəmi həmin fayldandır.
- `text2path.py` FONTS registrində 10 slug var, `jetbrains-mono` yoxdur. Fayl diskdədir (187208 bayt). **Bloklayıcı deyil** - kontura çevrilən yeganə mətn Archivo sözmarkasıdır.
- `US-COLLATERAL-SPEC.md` 752 sətirdə şrift adını bir dəfə də çəkmir. Vizit kartı, letterhead, banner build-i mətni hansı şriftlə yığacağını bilmir. **Bloklayıcı.**

### Hökm - ŞƏRTİ hissə
İlkin verdikt "tracking-də ziddiyyət realdır, fərq 3.92 punkt və işarə tərsdir" deyirdi və DL:577-ni Kestridge briefinə bağlayan bloklayıcı patch təklif edirdi. **Təkzib bunu rədd etdi və mən rəddi qəbul edirəm:**

- DL:577-nin tam mətni "+2.5%"-i **toxum** elan edir: "Bu **yalnız başlanğıcdır**; söz marka mütləq optik olaraq əl ilə düzəldilməlidir və nəticə konturlanmış path-a \"bişirilir\"." Briefin `-14`-ü isə **düzəlişdən sonrakı** nəticədir (brief:588 "Cüt düzəlişləri **tətbiq olunmuş** və tracking `-14` verilmiş halda"). İki sənəd eyni boru xəttinin iki mərhələsini yazır. Toxumdan nəticəni çıxmaq kateqoriya səhvidir.
- Təklif olunan patch verdiktin öz arbitraj qaydasını pozurdu: `DESIGN-LANGUAGE.md`:5-6 "Bu sənəd **markanı** təyin etmir, markanın yaşayacağı **sistemi** təyin edir", :10 "Aşağıdakı hər qayda ada baxmayaraq işləyir". Ad-neytral sənədin içinə `projects/kestridge-ai-brand/...` yolu yazmaq onu bir brendə bağlayır və HQ üç qat qaydasını (sistem qatı layihə qatından asılı olmur) pozur. **Doğru düzəliş:** DL-in öz mexanizmi ilə - "| `unknown` | Məlumat yoxdur. Uydurulmayıb |" (:21).
- "wdth 92 arxasında heç bir ölçmə yoxdur" iddiası **yanlışdır**: `03-wordmark-typography.md`:583-585 ölçülmüş əyri verir (wdth 90 -> AR 9.39, wdth 100 -> 10.26). 92 interpolyasiya ilə ~9.56.
- "Kök səbəb" kəşf deyil, artıq sənəddədir: `03-wordmark`:652-654 "-24.2 pm Inter-in `opsz` oxundan gəlir ... Display ölçülərinə (cap > 100 px) **ekstrapolyasiyadır; ona görə -30-da kəsilir**."
- `US-TYPEFACE-PERCEPTION.md`:336-337 üçün təklif olunan mətn öz-özünə ziddir: eyni abzasda həm "stacked üçün `wdth` 70", həm "kiçik ölçüdə `wdth` 100-dən aşağı düşməsin" saxlayır, halbuki brief 6.2-yə görə stacked konstruksiyaca kiçik/dar haldır (`C = 2.5c = 6.75u`).
- `DESIGN-LANGUAGE.md`:556-558-dəki mexaniki qayda ("Archivo 20 px-dən aşağı **heç vaxt** işlədilmir") həll olunmamış qalır, halbuki brief 5.5 cədvəlinin "<= 20 px" sətri aşağıya açıqdır.

**Nəticə:** DL 3.8 patch-i **rədd olunur**; onun yerinə DL 3.7/3.8-ə əhatə sətri yazılır ("bu şkala və tracking funksiyası cari mətnə şamil olunur, söz markaya yox") və DL 3.6-nın `wdth 92` xanası `unknown`-a çevrilir. wdth 70 döşəməsi ölçülməmiş qalır.

---

## 5. C5-AR

### Nə idi
`03-wordmark-typography.md` bölmə 4 (sətir 372) `9.70` verir, bölmə 8 (sətir 585) `10.26`, brief 5.7 isə `9.553`. Brief risk 13 bunu "uyğunsuzluq" adlandırır və barışdırma tələb edir.

### Hökm - qismən
**Təsnifat qəbul edilir və qiymətlidir.** Üç rəqəm üç fərqli kəmiyyətdir, üç fərqli ölçmə deyil:

| Rəqəm | Nədir | Şərt | Yer |
|---|---|---|---|
| `9.70` | ink bbox eni / ink bbox hünd. | düzəlişsiz, tracking 0, default söz boşluğu 200u | `03-wordmark`:372 |
| `10.26` | advance cəmi / cap | eyni sətir, eyni `wdth 100`, sadəcə advance və məxrəci cap (686u) | `03-wordmark`:585 |
| `9.553` | ink AR, düzəlişlər + tracking -14 + söz boşluğu `0.45 x cap` | Chromium/HarfBuzz raster, cap 200 px | brief:593 |

Bölmə 4-ün metodu bunu özü elan edir: "Aspekt nisbəti mürəkkəb qutusundandır, **advance-dan yox**" (`03-wordmark`:366-368). Barışdırma zənciri bağlanır: 10.036 + 0.1585 (boşluq 0.292 -> 0.45 cap) - 0.1501 (cüt düzəlişləri -103 pm em) - 0.2041 (tracking -14 x 10 aralıq) = **9.8406**, ölçülən 9.840. Eyni resept `wdth 70` sətrini də hərfi bərpa edir (7.3799 vs 7.380). İki müstəqil təkzibçi bölmə 8-in səkkiz sütununun səkkizini də advance/686 kimi yenidən istehsal etdi.

### Niyə hökm bağlanmır - ciddi təkzib
Operativ nəticə - "risk 13 bağlanır, 9.553 yeganə qabaritdir" - **rədd olunur**. Diskdə `KESTRIDGE AI` wdth 100 üçün **beş** rəqəm var və ikisi işləyən koddur:

| Dəyər (en/cap) | Mənbə |
|---|---|
| 10.036 | `03-wordmark` bölmə 4 (ink/ink, düzəlişsiz) |
| 10.258 | `03-wordmark` bölmə 8 (advance/cap) |
| 9.840 | brief 5.7 (10 tracking aralığı) |
| **9.861** | `tools/build/wordmark.py` - kod tracking-i **9 aralığa** verir: `elif nxt == " ": pass  # gap owned by the space itself` |
| **10.022** | `concept/renders/12-browser-css-proof.png` - `tools/browserproof.py` hər hərfi öz `letter-spacing` span-ına salır, bu konteynerin `-0.014em`-ini əvəz edir, ona görə tracking yalnız `AI` qoşasına düşür |

Kritik nöqtə: 9 aralıq **yazılı qaydanın nəticəsidir**, səhv deyil. Brief:555-556 və `03-wordmark`:433 ("Boşluq tracking-ə tabe deyil, sabitdir") məhz bunu deyir. Briefin 9.840 rəqəmi isə 10 aralıq tələb edir. Yəni bu etiket boşluğu deyil, **məzmun ziddiyyətidir** - və `wordmark.py`-ın öz-sınaq toleransı (`d2 < 0.06`) fərqi səssizcə udur.

Əlavə: `tools/build/lockup.py` `wm_ink_w`-dən `18 + 4.05 + 9.861 x 13.5 = 155.17u` çıxarır, brief 6.1-in `154.89u`-su deyil (AR 8.621 vs 8.61).

**Nəticə:** risk 13 **açıq qalır və genişlənir**. Bağlanması üçün bir qərar lazımdır: tracking neçə aralığa verilir - 9 (yazılı qayda, kod) yoxsa 10 (briefin rəqəmi)? Qərar veriləndən sonra 5.7-nin dörd rəqəmi, bölmə 6-nın bütün sabitləri, `wordmark.py`, `lockup.py`, `browserproof.py` və `12-browser-css-proof.png` eyni anda yenilənir.

Kiçik qeydlər: verdiktin `unresolved #4`-ü ("qarışıq registr sütununu təkrar istehsal edə bilmədim, 797 vs 803") **səhvdir** - sənədin öz metodu ilə (Pillow/FreeType em=2000, hədd 128) ölçmə 797.1 / 131.9 / 6.042 verir, hərfi uyğundur. Təklif olunan ink/ink əvəz sırasında iki rəqəm səhvdir (8.04, 8.88 olmalıdır). AR ikinci onluq rəqəmlə verilməlidir - üçüncü rəqəm `ink hünd. / cap` məxrəcinin bir piksel kvantlanmasına (206/200, 207/200) oturur.

---

## 6. C6-AD

### Nə idi
Brief risk 9 AMBER verdikti, risk 11 "klirens aparılmayıb", risk 12 "domenlər `unverified`". Yeni `US-TRADEMARK-SCAN.md` və `domain/03-operational-test.md` real sorğu apardı.

### Hökm
| Risk | Status | Əsas |
|---|---|---|
| **9 (AMBER)** | **Bağlanmır, ağırlaşır** | Sənədin öz yekunu şərhsiz "# AMBER" və "ad hüquqi baxımdan boşdur, amma kommersiya baxımından boş deyil. Risk reyestrdən yox, bazardan gəlir." Kestra rəqəmi köhnəlib: 8 mln USD yox, 25 mln USD Series A (2026-03-31) + cəmi 36 mln USD, ABŞ reg. **7544165** canlı, IC 009 + IC 042, təsvirində "orchestrate, automate, schedule". Kestrel AI qanadı yeni skanla **örtülməyib** |
| **11** | **Daralır, bağlanmır** | "USPTO bazasında `KESTRIDGE` sözmarkası ümumiyyətlə yoxdur. Nə canlı, nə ölü." Nəzarət sorğusu ilə müdafiə olunub: `WM:kestrel` 129, `WM:kestra` 15, `WM:kestridge` **0**. Bölmə 9 açıq saxlayır: ştat reyestrləri, common law, sosial handle, EUIPO/WIPO/Madrid, TTAB |
| **12** | **`.com`/`.ai`/`.io`/`.net`/`.org`/`.co` üçün bağlanır** | RDAP reyestr obyektini sorğulayır, ona görə `02-collision-scan` 8.4-dəki NXDOMAIN ikililiyi ("qeydiyyatdan keçmiş, lakin nameserver təyin edilməmiş domen də eyni cavabı verir") aradan qalxır. Müsbət nəzarət: `google.com` 200, `perplexity.ai` 200, `kestra.com` 200. `kestridge.az` yalnız DNS ilə baxılıb - **açıq**. Domenlər hələ **alınmayıb** |

### Yaşıl işıq - daralmış
İlkin verdikt "vəkil şərti **yalnız** ərizə və geri qaytarılmayan xarici xərc üçün qapıdır, build ilə vəkil **paralel** gedir" deyirdi, güvəni yüksək. **İki ciddi təkzib bunu daraltdı:**

- Qapının başlığı hərfi olaraq belədir: "**Pul xərcləməzdən əvvəl (identiklik, çap, veb sayt):**" (`US-TRADEMARK-SCAN.md`:433). Sənəd "identiklik"i çap və veb saytla eyni səbətə qoyur. Verdikt həmin sətri öz sübutunda gətirir, sonra "identiklik" sözünü təhlildən çıxarır. Sənəddə "yalnız" sözü yoxdur.
- Sənədin verdiyi yeganə açıq gözləməmə istisnası **dardır və domenə aiddir**: "Ticarət nişanı araşdırması bitənə qədər gözləmək lazım deyil - **domen ucuzdur, itirilmiş domen bahalıdır**" (:385-387).
- `02-collision-scan.md`:19-22 heç yerdə cavablandırılmır: "Ad üzrə yekun qərar verilməzdən əvvəl ... səlahiyyətli ticarət nişanı vəkili ilə ... rəsmi nişan axtarışı aparılmalıdır."

**Düzəldilmiş yaşıl işıq:** dərhal icazəli olan - **domen alışı (187.56 USD birinci il) və sosial handle tutulması**. Rəqəmsal master build-i **daxili artefakt kimi** gedir, amma "identiklik"in kommersiya təhvili (brend kitabı, sayt lansmanı, müştəriyə gedən çap, USPTO ərizəsi) vəkil yaşıl işığından asılıdır.

### Silsilə xətti tələsi - yeni tapıntı, bloklayıcı
`US-TRADEMARK-SCAN.md` iki yerdə (:351-353 və :467) vizual mənbə kimi **silsilə xəttini** tövsiyə edir: "`-ridge` hissəsi burada daha təhlükəsiz vizual mənbədir: silsilə, təbəqə, yüksəlmə. O sahə boşdur."
`02-collision-scan.md`:611 eyni şeyi adı ilə **qadağan edir**: "4. **Dağ / zirvə / silsilə xətti qadağandır.**" Və :602: "Bu şərtlər pozulsa verdikt avtomatik **qırmızıya** keçir."
Seçilmiş nişan mücərrəd K lövhəsidir, ona görə bu gün pozuntu yoxdur. Amma build masasında bir-birini inkar edən iki yazılı göstəriş qalır. **Barışdırılmalıdır.**

### FACT-GATE məhdudiyyətləri
Build fazasında yazılan hər mətn artefaktı `narrative/04-FACT-GATE.md` bölmə 10 (qara siyahı, B1-B33) və bölmə 11 (sarı siyahı, A1-A14) üzrə yoxlanır. Qara siyahı bəndi olan mətn çıxmır. Prioritet sırası sənədin özündədir:
1. "**Növ qarışıqlığı həll edilməlidir** (bölmə 2). Ya cins səviyyəsində danış, ya növü adlandır. Bu, düzəldilməmiş ən böyük riskdir."
2. "**B21 düzəldilməlidir.** Yanlış cümlə hazır brend mətninin içindədir."

İlkin verdikt yalnız 2-cini icra edirdi. **D2-D13 də siyahıya girir**, xüsusən D8: "`01-kestrel-verified.md`, bütün sənəd | Növ qarışıqlığı qeyd edilməyib | **Bölmə 2** əlavə edilməlidir. **Ən vacib düzəliş.**" Əlavə: bölmə 13 nişan seçildikdən sonra `03-story-craft.md` R-2 inkar testini tələb edir - nişan artıq seçilib (brief 3.1), yəni bu indi ödənməlidir.

---

## DƏYİŞİKLİK SİYAHISI

Build agentinin iş siyahısı. `B` = bloklayıcı, `N` = bloklamayan. `ŞƏRTİ` = C2 qapıları ödənənə qədər tətbiq edilmir.

### A. `concept/00-CONCEPT-BRIEF.md`

| # | Yer | Nə olur | Mənbə | Status |
|---|---|---|---|---|
| A1 | 2.1, sətir 84-87 | "Azərbaycanda" -> "ABŞ bazarında", "İT" -> "IT", "ingilisdilli B2B xidmət şirkəti". Qalan iki cümlə dəyişmir | C1 | **B** |
| A2 | 2.1, sətir 89-91 | Ölçü meyarı: alıcı "ABŞ müəssisəsinin IT və təhlükəsizlik qərar vericisi"; müqayisə dəsti Palo Alto Networks, CrowdStrike, Datadog, Splunk, Cloudflare, Okta, Zscaler, Fortinet, Elastic, HashiCorp, Databricks, Snowflake; səthlər 3.5 x 2 in kart, 1920 x 1080 slayd (safe inset 120 px), dairəvi avatar | C1 | **B** |
| A3 | 4.1, sətir 306 | `#1B2430` -> `neutral-950 #0F1317`; tərsdə `neutral-025 #FAFAF7`; "bax risk 11" -> "bax risk 8" | C3 | **B** |
| A4 | Risk 8, sətir 802-804 | Dörd mürəkkəb vəziyyəti yazılır (müsbət, knockout, monoxrom, qravür). Aksent cümləsi mənbənin öz siyahısı ilə: "yalnız **marka**, ikon, sərhəd, fokus halqası və qrafik ştrixi" (`DESIGN-LANGUAGE.md`:327). "link" YAZILMIR. Neytral mürəkkəb seçimi layihə qərarı kimi işarələnir, mənbə qadağası kimi yox | C3 | **B** |
| A5 | Bölmə 7, sətir 690 + cədvəl | Ölçmə bazası köhnəlir: cədvəlin hər rəqəmi `#1B2430` / `#FFFFFF` rejimindədir. Ya şərt etiketi yazılır, ya `#0F1317` / `#FAFAF7` ilə yenidən ölçülür. Risk 6-nın 0.801 / 0.781 rəqəmləri bu bazadandır | C3 | **B** |
| A6 | Bölmə 11, fayl istinadları | `renders/13-selected-lockups.png` və `renders/14-selected-size-ladder.png` köhnə rənglə render olunub; kanonik siyahı yenilənir | C3 | **B** |
| A7 | 5.4, cədvəldən əvvəl | Tracking aralıq qaydası açıq yazılır. **Bu bənd C5 qərarından asılıdır** - 9 və ya 10 aralıq. Hazırda kod 9, brief rəqəmi 10 tələb edir | C5 | **B** |
| A8 | 5.4, düzəlişlər cədvəli | Qeyd: cüt düzəlişləri `wdth 70`-də **miqyaslanmır**, eyni pm em dəyərləri ilə tətbiq olunur (miqyaslansa 7.380 yerinə 7.402 çıxır) | C5 | **B** |
| A9 | Risk 13 | "Həll olundu" YAZILMIR. Yenidən yazılır: təsnifat qəbul edilir (9.70 = ink/ink, 10.26 = advance/cap, 9.553 = düzəlişli ink/ink), **amma risk açıq qalır** - diskdə beş rəqəm var: 10.036, 10.258, 9.840, `wordmark.py` 9.861, `12-browser-css-proof.png` 10.022 | C5 | **B** |
| A10 | 5.7, sətir 598-600 | Mötərizədəki "risk **12**-də bayraqlanır" -> "risk **13**" | C5 | N |
| A11 | Bölmə 10 | İki qapı: (i) bənd 1-6 daxili artefakt kimi gedir; (ii) bənd 7 və hər xarici/kommersiya istifadəsi `US-TRADEMARK-SCAN.md`:433-435 vəkil yaşıl işığından asılıdır. Yeni bənd: hər mətn artefaktı FACT-GATE bölmə 10 + 11 üzrə yoxlanır. Yeni bənd: rəng CI testi (icazəli token dəsti + kontrast >= 3:1 + `#1B2430` / `#006A7A` qrep testi). **Sətir 874 saxlanılır** | C6, C3, C2 | **B** |
| A12 | 5.2, sətir 442-447 | Azərbaycan örtüyü abzası birinci arqument mövqeyindən çıxır, bölmə sonuna "gələcək dil genişlənməsi qeydi" kimi köçür | C1 | N |
| A13 | 5.2, yeni birinci arqument | `US-TYPEFACE-PERCEPTION.md` 5.1: ABŞ grotesque registri, x/cap 0.767, kontrast 1.26, `o` 0.945, apertura 192.4 pm, mahmızlı `G` (notch 108.3), düz və 177.8 pm açılan `R` ayağı, `tnum` + kəsik `zero`, `wdth` 62-125 | C1, C4 | N |
| A14 | 5.1, spesifikasiya cədvəli | İkinci və üçüncü rol əlavə olunur: UI/body = Inter Variable 4.001, mono/data = JetBrains Mono 2.211. Hər üç TTF diskdədir | C4 | N |
| A15 | 5.5, düsturdan sonra | Əhatə: yalnız söz marka. Qəbul edilmiş qalıq: `-24.2` Inter `opsz` oxundan, cap 128 px-ə ekstrapolyasiya; `-30` kəsimi bunu hüdudlayır (`03-wordmark`:652-654); DL 3.8-in caps üçün +6.0%/+4.0% düzəlişi tətbiq edilmir | C4 | N |
| A16 | 5.3, registr cədvəli | "aspekt nisbəti" sətrinə şərt etiketi: "düzəlişsiz, ink/ink; icra rəqəmi 5.7-dədir" | C5 | N |
| A17 | Risk 9, sətir 808-815 | Kestra rəqəmləri yenilənir (25 mln Series A 2026-03-31, cəmi 36 mln, Nyu-York, reg. 7544165 canlı IC 009+042). Kestrel AI qanadının yeni skanla **örtülmədiyi** açıq yazılır | C6 | N |
| A18 | Risk 11, sətir 823-827 | Daraldılır: USPTO federal sözmarka qatı təmiz (`WM:kestridge` 0, `PM:kestridge` 0, `WM:kestridg*` 0, `WM:kestr*` 167 siyahısı endirilib). Açıq: vəkil klirensi, 50 ştat, common law, EUIPO/WIPO/Madrid, TTAB, fonetik qonşular. Azərbaycan Əqli Mülkiyyət Agentliyi siyahıdan çıxır | C6 | N |
| A19 | Risk 12, sətir 828-833 | RDAP nəticələri yazılır; `.az` və handle-lar açıq qalır; **"hələ alınmayıb"** açıq bənd kimi qeyd olunur; `kestridj` çıxır (azərbaycandilli yazı artefaktı, `01-name-semantics`:359); `kestredge.com` artıq tutulub | C6, C1 | N |
| A20 | Risk 16, sətir 859-863 | Azərbaycandilli oxucu, `[kestric]` qeydi və transliterasiya cümləsi silinir. Şərt 6 qalır, səbəbi dəyişir: ölçülmüş ingilis səhv rejimləri (`kestrige`, `kestbridge`, `kestredge` yüksək) + `KES-trij` vs `KES-truh` | C1 | N |
| A21 | 5.1 sətir 432/434/438, 5.6, 6.1 (633 + 639-640), 6.2 (651 + 657-658), yeni 6.6 deskriptor, yeni 6.7 ağ siyahı, risk 1 (769-773), bölmə 10 bənd 3-4 | Bütün C2 patch dəsti. **Şərtlərə diqqət:** (a) 6.1/6.2 patch-i sətir 633 və 651-i də əhatə etməlidir, yoxsa cədvəl öz-özünə zidd olur; (b) "variant E" termini briefdə tərif edilməyib - ya tərif əlavə olunur, ya `03-wordmark`:425-ə istinad verilir; (c) bənd 3-ün ikinci sətri (874) saxlanılır; (d) 6.6 döşəməsi (40.6 px, 18u ink) bölmə 7-nin 24u plitə oxuna çevrilib yazılmalıdır (54.1 px plitə) | C2 | **B, ŞƏRTİ** |

### B. `concept/02-collision-scan.md`

| # | Yer | Nə olur | Mənbə | Status |
|---|---|---|---|---|
| B1 | Bölmə 11, şərt 4 (sətir 611) | Silsilə xətti qadağası ilə `US-TRADEMARK-SCAN.md`:351-353 və :467 tövsiyəsi barışdırılır. Barışdırılmasa build masasında avtomatik qırmızı tələsi qalır (:602) | C6 | **B** |
| B2 | Bölmə 11, şərt 5 (sətir 614-616) | Şərt 5 rəsmi düzəliş sətri ilə yenilənir: `AI` adın morfemi deyil, kateqoriya deskriptorudur. **C2 qapısıdır** - bu düzəliş yazılmadan kilid dəyişməz | C2 | **B, ŞƏRTİ** |
| B3 | Başlıq, sətir 9 | "Coğrafiya | Azərbaycan" -> "ABŞ, əməliyyat dili: İngilis". Toqquşma tapıntıları dəyişmir | C1 | N |
| B4 | Bölmə 11, şərt 7 | Qeydiyyat statusu artıq məlumdur (RDAP altı TLD üçün boş); `.com` kanonik, `.ai` müdafiə; `.az` opsional və hələ naməlum | C6, C1 | N |
| B5 | Bölmə 11, şərt 8 | Qismən icra olunub (federal USPTO). EUIPO, WIPO, Madrid qalır. Azərbaycan agentliyi çıxır. Şərt bağlı sayılmır | C6, C1 | N |

### C. `concept/03-wordmark-typography.md`

| # | Yer | Nə olur | Mənbə | Status |
|---|---|---|---|---|
| C1a | Sətir 7, 425, 427, 617, 621, 622, 623, 628-637 (CSS) | **İlkin C2 və C4 siyahılarında bu fayl ümumiyyətlə yox idi.** Brief:422 "Bütün rəqəmlər `03-wordmark-typography.md`-dən gəlir" - kilid və ya parametr dəyişirsə mənbə sənəd eyni anda dəyişməlidir, yoxsa iki kanonik sənəd bir-birinə zidd qalır | C2, C4, C5 | **B, ŞƏRTİ** |
| C2a | Bölmə 9, sətir 661-663 | Placeholder `#006A7A` ləğv edilir: işıqlı rejimdə `petrol-600 #0E6A82`, qaranlıq rejimdə `petrol-400 #2FA8C7`. Variant D əsas lockup-da işlədilmir | C3 | N |
| C3a | Başlıq sətir 9, bölmə 2 başlığı, 2.2 hökmü | "Dillər | Azərbaycan və İngilis" -> "İngilis (ABŞ); Azərbaycan gələcək genişlənmə". Bölmə 2 "qapı" -> "gələcək genişlənmə üçün ölçmə". 2.3 (`İ` nöqtəsi, +30 faiz) fakt kimi tam saxlanılır | C1 | N |
| C4a | Bölmə 4, bənd 5 (sətir 400-404) | Nömrələnmiş arqumentdən qeyd sətrinə enir. Registr qərarı 1-4 ilə ayaqda qalır | C1 | N |
| C5a | Bölmə 4 cədvəl başlığı, bölmə 8 bənd 4 | Şərt etiketləri: bölmə 4 = ink bbox / ink bbox, düzəlişsiz, tracking 0, default boşluq; bölmə 8 = advance cəmi / cap. İnk/ink qarşılığı: 6.57 / 7.23 / **8.04** / 8.63 / **8.88** / 9.70 / 10.83 / 12.51 | C5 | N |

### D. Digər sənədlər

| # | Fayl və yer | Nə olur | Mənbə | Status |
|---|---|---|---|---|
| D1 | `US-TYPEFACE-PERCEPTION.md`:19-20 | Inter mənbəsi düzəlir: `rsms/inter` `InterVariable.ttf`. Archivo və JetBrains Mono Google Fonts-dadır | C4 | **B** |
| D2 | `US-TYPEFACE-PERCEPTION.md`:336-337 | `wdth` bəndi yenidən yazılır. **Diqqət:** "stacked = wdth 70" və "kiçik ölçüdə 100-dən aşağı düşməsin" eyni abzasda ziddir - biri seçilməlidir və ya döşəmə cap px ölçülməlidir | C4 | **B** |
| D3 | `US-COLLATERAL-SPEC.md`, yeni tipoqrafiya bölməsi | 752 sətirdə bir dəfə də şrift adı keçmir. Yekun stack və rol bölgüsü yazılır: söz marka Archivo 2.001 `wght 600`; başlıq/cari mətn DL 3.7 şkalası; rəqəm cədvəlləri `tnum` 1 + `zero` 1; kod/log JetBrains Mono 2.211 | C4 | **B** |
| D4 | `US-COLLATERAL-SPEC.md` bölmə 4, sətir 270-273 | Title slide baza xətləri yenidən törədilir. 80 px fərqi `C = 114.29 px` deməkdir, o halda deskriptorun cap xətti `y = 605.7`-dədir - `y = 600..604` aksent xəttindən 1.7 px. Rəqəm uydurulmur, `C` seçiləndən sonra düsturla yazılır | C2 | **B, ŞƏRTİ** |
| D5 | `US-COLLATERAL-SPEC.md` bölmə 10 girişi | "Bu cədvəl birbaşa build skriptinə keçir" - vəkil qapısı bura yazılmalıdır; geri qaytarılmayan çap xərcini doğuran əsl sənəd budur | C6 | **B** |
| D6 | `US-COLLATERAL-SPEC.md` 9e (557-558), :273, :280, :504-505 | Accent `petrol-500 #1187A5` yazılır və 9e proseduru işə düşür. `accent_rule` və full-bleed sahə üçün rəng dəyəri; üstündəki mətnin kontrastı; Outlook dark mode üçün bərk fon tokeni | C3 | N |
| D7 | `narrative/03-story-craft.md` 5.3, nümunə 6 | B21 düzəlişi. Yanlış cümlə hazır brend mətnindədir - build ondan kopyalamamalıdır. Doğru variant FACT-GATE S17-də hazırdır | C6 | **B** |
| D8 | `narrative/01-kestrel-verified.md` | FACT-GATE D8: **bölmə 2 (növ qarışıqlığı) əlavə edilməlidir - "Ən vacib düzəliş"**. Həmçinin D2, D3, D4, D5, D6, D7, D9, D13 | C6 | **B** |
| D9 | `narrative/02-ridge-semantics.md` | FACT-GATE D10, D11, D12 | C6 | N |
| D10 | `narrative/03-story-craft.md` R-2 | Nişan seçildiyi üçün FACT-GATE bölmə 13-ün tələb etdiyi ikinci yoxlama (inkar testi) icra olunur | C6 | **B** |
| D11 | `narrative/03-story-craft.md` bölmə 8, bənd 1 | Bazar uyğunsuzluğu bəndi bağlanır: "həll olundu, bazar ABŞ (C1)" | C1 | N |
| D12 | `US-TRADEMARK-SCAN.md` bölmə 9 | Siyahıya əlavə: Kestrel AI (Kestrel Systems, Inc., YC F25) bu sənəddə kommersiya toqquşması kimi qiymətləndirilməyib; mənbə `02-collision-scan.md` 3.1, orada "ƏN YÜKSƏK RİSK" | C6 | N |
| D13 | `domain/03-operational-test.md` 5c cədvəli və bölmə 8 | "| Loqo | Bəli |" -> "| Loqo (kilid) | Xeyr - AI deskriptor sətrindədir |". Sənədin arqumenti dəyişmir | C2 | **B, ŞƏRTİ** |
| D14 | `domain/03-operational-test.md` :319-320 vs `US-TRADEMARK-SCAN.md`:428 | `.io` ziddiyyəti: biri "al", digəri "alma". Biri seçilməlidir; 187.56 USD büdcəsi `.io`-nu içinə almır | C6 | N |
| D15 | `domain/02-ai-suffix-longevity.md` bölmə 0, sətir 28 | Güvən "Orta-yüksək" -> "Yüksək" **yalnız C2 qapıları ödənəndən sonra** | C2 | N, ŞƏRTİ |
| D16 | `concept/01-name-semantics.md` sətir 11, :359, 4.3, :641 | Bazar sətri ABŞ olur; `KESTRIDJ` və azərbaycandilli oxucu bölməsi "gələcək dil genişlənməsi"nə köçür. Sətir 641 ("`AI` söz işarəsində tabe element kimi qurulur") C2 qəbul olunarsa yenilənir və ya arxivlənir | C1, C2 | N |
| D17 | `concept/04-monogram-geometry.md` :576, :682-683 | `#1B2430` -> `#0F1317`; rəng bəndi C3 istinadı ilə əvəzlənir. Arxiv sənəddir | C3 | N |
| D18 | `project.yaml` + yeni `profiles/kestridge-ai/brand.yaml` | DL 7.1 profil qatı: `color.brand #1187A5`, `color.paper #FAFAF7`, `color.ink #0F1317`, `type.display Archivo`, `type.ui Inter`, `type.mono JetBrains Mono`. Sonra `profiles: [kestridge-ai]`, `default_profile: kestridge-ai`. Rəngin kanonik yeri bundan sonra profil faylıdır | C3 | N |

### E. Qonşu layihə - `projects/brand-identity-2026/DESIGN-LANGUAGE.md`

| # | Yer | Nə olur | Mənbə | Status |
|---|---|---|---|---|
| E1 | 3.8, sətir 577 | **İlkin patch RƏDD OLUNUR.** Ad-neytral sənədin içinə layihə yolu yazmaq :5-6 və :10 ilə ziddir. Əvəzinə: 3.7/3.8-ə əhatə sətri - "bu şkala və tracking funksiyası cari mətnə şamil olunur, söz markaya yox" | C4 | **B** |
| E2 | 3.6, sətir 527 | `wdth 92` xanası `unknown`-a çevrilir (DL 7.2 bənd 2 onsuz da söz marka ölçülərini `unknown` sayır). "Ölçmə yoxdur" iddiası yazılmır - `03-wordmark`:583-585-də ölçülmüş əyri var | C4 | **B** |
| E3 | 3.7, sətir 556-558 | "Archivo 20 px-dən aşağı heç vaxt işlədilmir" qaydası ilə briefin "<= 20 px" tracking sətri barışdırılmalıdır | C4 | N |
| E4 | 7.2, bənd 5 | Ən yaxın qonşu dəyişdi: Kestrel AI `--accent-cyan #6bb0c0` dE2000 = 14.86, Snowflake 15.57-dən yaxın. Yan-yana test hər ikisi ilə. Kestrel AI-ın rəsmi guideline-ı `unknown`, dəyər canlı CSS-dən | C3 | N |
| E5 | 7.2, bənd 1 | `text2path.py` registrinə `jetbrains-mono` əlavə olunur (fayl diskdə, 187208 bayt). Logo build-i mono `text2path` işlətmir | C4 | N |

### F. Kod və artefaktlar - `tools/`, `renders/`, `delivery/`

| # | Fayl | Nə olur | Mənbə | Status |
|---|---|---|---|---|
| F1 | `tools/build/mark.py:82`, `tools/build/lockup.py:48,78,107`, `tools/build/raster.py:37,58`, `tools/fix01.py:6-7` | Hardcoded `ink="#1B2430"` -> `#0F1317`; `bg="#FFFFFF"` -> `#FAFAF7` (`neutral-025`). Heç bir çağırış yeri `ink`-i açıq ötürmür, ona görə SVG-ni düzəltmək kifayət etmir | C3 | **B** |
| F2 | `concept/sketches/04r-plate-selected.svg`, `04r-plate-dense.svg` | 3+3 `fill="#1B2430"` -> `#0F1317`. Brief 4.3, 4.6, 11-də referans master kimi adlanır | C3 | **B** |
| F3 | `concept/sketches/01-apex.svg` (2), `02-fissure.svg` (4), `03-crest.svg` (3), `04-counters.svg` (3), `05-displacement.svg` (2), `06-fold.svg` (3) | 17 keçid. Arxiv eskizlərdir, amma A11-in qrep testi onlarda sınır. Ya düzəldilir, ya test istisna siyahısı alır | C3 | N |
| F4 | `tools/build/__pycache__/*.pyc` | Binar fayllarda `#1B2430` var; qrep testi bunlarda da sınır. Təmizlənir və ya test `.pyc`-ni istisna edir | C3 | N |
| F5 | `tools/build/wordmark.py:90-91` | Tracking 9 aralığa verilir (`elif nxt == " ": pass`), brief rəqəmi 10 tələb edir. Öz-sınaq toleransı `d2 < 0.06` fərqi udur. **C5 qərarından sonra düzəlir və tolerans daraldılır** | C5 | **B** |
| F6 | `tools/build/lockup.py` | `18 + 4.05 + 9.861 x 13.5 = 155.17u` çıxır, brief 6.1-in `154.89u`-su yox (AR 8.621 vs 8.61). C5 qərarı ilə birlikdə uzlaşdırılır | C5 | **B** |
| F7 | `tools/browserproof.py` + `concept/renders/12-browser-css-proof.png` | Hər hərf öz `letter-spacing` span-ına salınır, bu konteynerin `-0.014em`-ini əvəz edir; ölçülən en/cap 10.022. Reseptin özü brief 5.8 və `03-wordmark` "Veb tətbiqi" bölmələrindədir - hər ikisi ilə birlikdə düzəlir | C5 | **B** |
| F8 | `concept/renders/13-selected-lockups.png`, `14-selected-size-ladder.png`, `renders/_build/` (12 artefakt, 07:29) | Köhnə rənglə və köhnə sətirlə render olunub. Yenidən çıxarılır. Bölmə 6 və 7-nin sübut renderləridir | C2, C3 | **B** |
| F9 | `delivery/` (116 fayl, MANIFEST 07:39:04Z) | **Artıq C3 tətbiq olunub** (`ink #0F1317`, `knockout #FAFAF7`, `accent #1187A5`) və **C2 primary=`KESTRIDGE`** kimi çıxarılıb. Deskriptor sətri (`AI AUTOMATION SECURITY DATA`) paketdə **yoxdur**. C2 qapıları ödənməyibsə paket `primary_text` üçün yenidən çıxarılmalıdır | C2, C3 | **B, ŞƏRTİ** |

---

## HƏLL OLUNMAYAN

| # | Məsələ | Kim qərar verir |
|---|---|---|
| U1 | Hüquqi şəxs adı: `domain/02` bölmə 0 "Kestridge (Inc/LLC), AI olmasın" (yüksək), `domain/03` 5c "Hüquqi/ticarət adı - Bəli, Kestridge AI". **C2-nin birinci qapısıdır** | İstifadəçi (sahib) |
| U2 | `02-collision-scan` şərt 5-in rəsmi düzəlişi. **C2-nin ikinci qapısıdır**; düzəlişsiz keçid :602-yə görə verdikti qırmızıya çevirir | İstifadəçi + dizayn direktoru |
| U3 | Tracking neçə aralığa verilir - 9 (yazılı qayda + `wordmark.py`) yoxsa 10 (briefin 9.840 rəqəmi)? Bu qərar 5.7-nin dörd rəqəmini, bölmə 6-nın bütün sabitlərini, `wordmark.py`, `lockup.py`, `browserproof.py` və bir renderi eyni anda açır | Dizayn direktoru (ölçmə qərarı) |
| U4 | Silsilə xətti: `US-TRADEMARK-SCAN.md`:351,353,467 tövsiyə edir, `02-collision-scan.md`:611 qadağan edir. Hansı qalır? | Dizayn direktoru |
| U5 | Vəkil büdcəsi və namizədi. Sənəd "danışıqsız şərt" deyir, rəqəm və ad yoxdur. Ödənməyəcəksə ABŞ-da geniş kommersiya lansmanı və USPTO ərizəsi olmur | İstifadəçi (sahib) |
| U6 | Deskriptorun dəqiq ingilis sözləri. Ölçülmüş 108.10-108.12u `AI AUTOMATION SECURITY DATA` üçündür; brief:84-85 sütunları "IT təhlükəsizliyi, data analitikası" adlandırır. `IT SECURITY` və ya `CYBERSECURITY` seçilsə flush nəticəsi (0.14 faiz) yenidən ölçülür | İstifadəçi + dizayn direktoru |
| U7 | `wdth 70` üçün kiçik ölçü döşəməsi `unknown`. `US-TYPEFACE-PERCEPTION`:336 kiçik ölçüdə 100-dən aşağı düşməyi qadağan edir, stacked isə konstruksiyaca kiçik haldır. Dar kəsimdə counter bağlanması 16/32/48 px-də raster ilə yoxlanmalıdır | Dizayn direktoru (ölçmə) |
| U8 | Spot mürəkkəb (PMS) `unknown`. `petrol-500` üçün ən yaxın `7704 C`, dE2000 = 4.91 - "eyni rəng deyil". Ya PMS rəsmi elan olunub hex ona uyğunlaşdırılır, ya spot mürəkkəbdən imtina edilir. Fiziki Formula Guide və işıq kabineti lazımdır | İstifadəçi + çap tərəfi |
| U9 | Qravür, tikmə, trafaret üçün fiziki rəngləyici `unknown`: sap nömrəsi, folqa növü, anodlama tonu. `US-COLLATERAL-SPEC.md` 9e proseduru + ilk istehsal partiyası | Çap/istehsal tərəfi |
| U10 | Knockout master ziddiyyəti: DL 4.5 tərs markanı məcburi ayrıca çəkir və ştrixi 6.25 faiz nazildir (S = 8 U -> 7.5 U), invertasiyanı qadağan edir; brief 4.7 "avtomatik invert təhlükəsizdir" deyir. 24u şəbəkədə nazilmə bağlanma eyniliyini 7.65 -> 7.0875 edir | Dizayn direktoru + fiziki çap sınağı |
| U11 | Həndəsə sistemləri barışdırılmayıb: Kestridge nişanı 24u şəbəkədə, DL 96 U şəbəkədə (C2 qanunu, S = 8 U, 26:15 imza meyli). C3 yalnız palitranı həll edir | Dizayn direktoru |
| U12 | `kestridge.az` reyestr statusu `unknown` - heç bir sənəd Azərbaycan milli reyestrinə sorğu etməyib. Yalnız DNS NXDOMAIN var | İstifadəçi (AZ planı varsa) |
| U13 | Sosial handle-lar (LinkedIn, X, GitHub) `unverified`. Domenlə eyni gün tutulmalıdır | İstifadəçi (dərhal) |
| U14 | Domenlər hələ **alınmayıb**. Status "saatlarla ölçülür". Birinci il 187.56 USD | İstifadəçi (dərhal) |
| U15 | `.io` alınsınmı: `US-TRADEMARK-SCAN`:428 "al", `domain/03`:319-320 "alma, biznes əsası yoxdur" | İstifadəçi |
| U16 | Kestra Technologies-in etiraz (opposition) vermə ehtimalı - tərifinə görə vəkil sualıdır | Vəkil |
| U17 | `KESTRAIL` (ser. 99801933, pending, IC 042) izlənməsi: məsul təyin edilməyib | İstifadəçi |
| U18 | Ərizə adı vs görünüş adı ayrılması brend kitabında açıq yazılmalıdır (`US-TRADEMARK-SCAN` 8.8 `KESTRIDGE` qeyd etdirməyi, şərt 5 lockup-da tam adı tələb edir - ziddiyyət deyil, amma qarışa bilər) | Dizayn direktoru |
| U19 | `KESTRIDGE AI` ikinci dərəcəli kilidinin sönmə (sunset) tarixi və ya tetikləyicisi heç bir sənəddə yoxdur | İstifadəçi (biznes qərarı) |
| U20 | Deskriptorun nişan hüququna təsiri `unknown` (disclaimer məsələsi) | Vəkil |
| U21 | Növ qarışıqlığı ABŞ kontekstində kəskinləşir: ABŞ oxucusu "kestrel" deyəndə American kestrel (*Falco sparverius*) düşünür, brief 1.1 ölçmələri isə *Falco cenchroides* və *Falco tinnunculus* üzərindədir. FACT-GATE bölmə 14 bənd 1 | Dizayn direktoru + FACT-GATE |
| U22 | Chivo rədd rəqəmi iki sənəddə fərqlidir: brief 5.2 "maxdev 26.98 pm", `US-TYPEFACE-PERCEPTION` 4.2/6 "rmse 12.4". Hökm eynidir (rədd), rəqəm barışdırılmalıdır | Dizayn direktoru |
| U23 | `G` notch və `R` açılması iki sənəddə fərqlidir: `US-TYPEFACE-PERCEPTION` 108.3 / 177.8 pm, brief 5.2 74 / 95.5 pm. Ölçmə metodikası fərqli ola bilər, barışdırılmayıb | Dizayn direktoru |
| U24 | `renders/07-width-axis.png` sənəddə beş en kimi təsvir olunur, bölmə 8 cədvəlində səkkiz sütun var. Hansının köhnə olduğu yoxlanmayıb | Build agenti |
| U25 | Inter və JetBrains Mono üçün Kestridge tərəfində çəki dəsti yoxdur. DL 3.6 dəstinin olduğu kimi qəbul edilib-edilmədiyi təsdiqlənməyib | Dizayn direktoru |
| U26 | DL 3.3-ün icbari Archivo `liga`/`locl` qaydası ABŞ ingilisdilli mətndə lazımdırmı - `unknown` | Dizayn direktoru |
| U27 | DL 7.2 açıq iş 4: `petrol-800` və `petrol-900` sRGB gamut divarındadır (R = 0). Nişan onları işlətmir; böyük format və çapda işlənərsə yenidən yoxlanır | Çap tərəfi |
| U28 | Azərbaycan tərəfinin gələcək statusu: ikinci dilə keçidin planlaşdırılıb-planlaşdırılmadığı barədə sənədlərdə heç nə yoxdur - `unknown` | İstifadəçi |
| U29 | Kestridge sənədləri ilə `brand-identity-2026` arasında **sıfır istinad** var. DL profil qatıdır, amma Kestridge onu valideyn sistem elan etməyib. Bu elan yazılmasa C3 və C4 düzəlişləri yenidən dağılacaq | Dizayn direktoru |
| U30 | Kestra və Kestrel AI-ın rəsmi brend guideline hex-ləri `unknown`; dəyərlər canlı `logo.svg` və CSS bundle-ından oxunub və xəbərsiz dəyişə bilər. Fortinet-in rəsmi hex-i hər iki sənəddə `unknown` | Dizayn direktoru (ilk çapdan əvvəl təkrar yoxlama) |

---

## BUILD IŞIQ SİQNALI

**Build sənəd düzəlişləri ilə paralel davam edir, amma yalnız daxili artefakt kimi:** dərhal icazəli olan domen alışı (187.56 USD) və sosial handle tutulmasıdır; nişan və lockup masterləri A1-A11, B1, C1a, D1-D3, F1-F8 bloklayıcı bəndləri bağlanandan sonra çıxarıla bilər; `KESTRIDGE`-tək kilid U1 və U2 qapıları ödənməyincə tətbiq edilmir (default `KESTRIDGE AI` qalır); söz işarəsi qabaritləri U3 tracking aralığı qərarı verilməyincə brend kitabına düşmür; və brend kitabı, sayt lansmanı, müştəriyə gedən çap və USPTO ərizəsi `US-TRADEMARK-SCAN.md`:433-435-ə görə **ABŞ ticarət nişanı vəkilinin yaşıl işığından asılıdır** - bu, danışıqsız şərtdir.