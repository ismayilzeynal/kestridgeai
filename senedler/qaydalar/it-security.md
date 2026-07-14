# IT Security — iş qaydaları

[Ümumi qaydalar](00-umumi.md) burada da keçərlidir. Aşağıdakılar
təhlükəsizlik işlərinə xas əlavələrdir. Bu sahədə qaydalar ən sərtdir —
istisna yoxdur.

## Başlamazdan əvvəl — məcburi şərtlər

- **Yazılı icazə (authorization letter):** müştəri rəhbərliyindən,
  imzalı. Scope-dakı sistemlərin siyahısı (IP, domen, tətbiq) əlavə
  olunur. **Bu sənəd olmadan heç bir test, hətta port skanı belə
  başlamır.**
- Sistemlər 3-cü tərəfdədirsə (hosting, SaaS) — müştəri həmin tərəfin
  test icazəsini də təsdiqlədiyini yazılı bildirir (məs. cloud
  provayderin pentest qaydaları).
- **Fövqəladə əlaqə:** hər iki tərəfdən 24/7 telefon nömrəsi. Test zamanı
  gözlənilməz vəziyyət yaranarsa dərhal zəng + testin dayandırılması.

## Test qaydaları

- Testlər yalnız **razılaşdırılmış pəncərələrdə** (adətən iş saatlarından
  kənar). Pəncərədən kənar heç nə işlədilmir.
- **Scope-dan kənar sistemə toxunulmur** — test zamanı maraqlı görünsə
  belə. Scope genişlənməsi yalnız yeni yazılı icazə ilə.
- Xidmət kəsintisinə səbəb ola biləcək testlər (DoS xarakterli, brute
  force və s.) yalnız ayrıca açıq razılıqla aparılır.
- Tapılan zəiflikdən yalnız sübut üçün minimum istifadə olunur —
  data çıxarılmır, sistem dəyişdirilmir, arxa qapı qoyulmur.
- Test zamanı əldə olunan hər şey (screenshot, log, data nümunəsi)
  layihə mühitində saxlanılır və hesabatdan sonra silinir.

## Tapıntı və hesabat qaydaları

- **Kritik zəiflik → 24 saat içində birbaşa bildiriş.** Hesabatın
  hazır olması gözlənilmir; müştəri dərhal bağlamaq imkanı qazanır.
- Hesabat formatı: hər tapıntı üçün severity (Critical/High/Medium/Low),
  təsvir, sübut, biznes riski, konkret remediasiya addımı. Üstündə
  rəhbərlik üçün texniki olmayan xülasə.
- Hesabatlar və tapıntılar **yalnız şifrəli kanalla** ötürülür — adi email
  əlavəsi kimi göndərilmir.
- Hesabat yalnız müştərinin təyin etdiyi şəxslərə verilir; bizim tərəfdə
  yalnız layihə komandası görür.

## Remediasiya və retest

- Remediasiya müştəridə də qala bilər, bizdə də — müqavilədə yazılır.
  Hər halda suallara dəstək veririk.
- Plana daxil **bir retest ödənişsizdir**; nəticəsi yekun hesabatda
  "bağlandı / qismən / açıqdır" statusu ilə göstərilir.

## Davamlı monitorinq xidməti üçün

- Qoşulan log mənbələrinin siyahısı və saxlama müddəti sənədləşir.
- **Eskalasiya matrisi:** hansı severity-də kim, hansı kanalla, hansı
  müddətdə məlumatlandırılır (gecə saatları daxil) — kickoff-da təsdiqlənir.
- İlk 2–4 həftə tuning dövrüdür: yalançı alarmlar təmizlənir; bu dövrdə
  alert axını normaldan çox ola bilər.
- Aylıq hesabat: insidentlər, trendlər, tövsiyələr.

## Təhvildə əlavə olaraq

- Yekun hesabat + retest nəticələri.
- Bizim bütün girişlərin ləğvinin təsdiqi (bu sahədə xüsusilə vacibdir).
- Tövsiyə olunan növbəti addımlar (illik retest, monitorinq və s.).
