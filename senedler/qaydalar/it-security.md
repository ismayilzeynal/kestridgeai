# IT Security - İş Qaydaları

[Ümumi qaydalar](00-umumi.md) tam keçərlidir. Bu sahədə qaydalar ən
sərtdir - **istisna yoxdur.** Aşağıda metodologiya, tool stack və
prosedurlar.

## 1. Metodologiya

Tanınmış çərçivələrə əsaslanırıq - "öz bildiyimiz kimi" yox:

- **OWASP** (WSTG + Top 10) - veb tətbiqlər
- **PTES** - penetration test prosesi
- **NIST 800-115** - texniki test metodologiyası
- **MITRE ATT&CK** - hücum texnikaları istinadı

## 2. Tool stack (standart)

| Faza | Tool |
| --- | --- |
| Kəşf / skan | **Nmap**, nuclei, ffuf |
| Veb tətbiq | **Burp Suite Pro**, sqlmap |
| Zəiflik skanı | **Nessus** / **OpenVAS** |
| İstismar (exploit) | **Metasploit** |
| Hesabat / scoring | **CVSS 3.1/4.0** |

## 3. Başlamazdan əvvəl - MƏCBURİ şərtlər

- **Yazılı icazə (authorization letter):** müştəri rəhbərliyindən, imzalı.
  Scope-dakı sistemlər (IP, domen, tətbiq) əlavə olunur. **Bu sənəd
  olmadan heç bir test, hətta port skanı belə başlamır.**
  *(Yazılı icazəsiz test = ABŞ-da CFAA üzrə cinayət riski - heç vaxt.)*
- **Rules of Engagement (RoE):** scope daxili/xarici IP-lər, test pəncərəsi,
  icazəli texnikalar, **qadağan olunanlar** (məs. DoS), fövqəladə dayandırma
  əlaqəsi.
- Sistemlər 3-cü tərəfdədirsə (hosting, SaaS) - müştəri həmin tərəfin test
  icazəsini də təsdiqlədiyini yazılı bildirir.
- **Fövqəladə əlaqə:** hər iki tərəfdən 24/7 nömrə.

## 4. Test qaydaları

- Testlər yalnız **razılaşdırılmış pəncərələrdə** (adətən iş saatından
  kənar). Pəncərədən kənar heç nə.
- **Scope-dan kənar sistemə toxunulmur** - maraqlı görünsə belə. Genişlənmə
  yalnız yeni yazılı icazə ilə.
- Xidmət kəsintisi ehtimalı olan testlər (DoS, brute force) yalnız ayrıca
  açıq razılıqla.
- Tapılan zəiflikdən yalnız **sübut üçün minimum** istifadə - data
  çıxarılmır, sistem dəyişdirilmir, arxa qapı qoyulmur.

## 5. Sübut (evidence) idarəetməsi

- Screenshot, log, data nümunəsi - layihə mühitində, **şifrəli** saxlanılır.
- Chain of custody: hansı sübut, nə vaxt, kim tərəfindən.
- Hesabatdan sonra bütün sübutlar silinir (data destruction).

## 6. Hesabat qaydaları

- **Kritik zəiflik → 24 saat içində birbaşa bildiriş** (hesabat gözlənilmir).
- Format: hər tapıntı - severity (**CVSS**), təsvir, sübut, biznes riski,
  konkret remediasiya addımı. Üstündə rəhbərlik üçün qısa xülasə.
- **Hesabat yalnız şifrəli kanalla** - PGP / age / parolla qorunan + ayrı
  kanalla. Adi email əlavəsi kimi (canlı zəifliklərlə dolu) **heç vaxt**.
- Hesabat yalnız müştərinin təyin etdiyi şəxslərə; bizdə yalnız layihə
  komandası görür.

## 7. Remediasiya və retest

- Remediasiya müştəridə və ya bizdə - müqavilədə. Suallara dəstək davam edir.
- Plana daxil **bir retest ödənişsizdir**; yekun hesabatda "bağlandı /
  qismən / açıqdır" statusu.

## 8. Davamlı monitorinq xidməti

- Qoşulan log mənbələri + saxlama müddəti sənədləşir.
- **Eskalasiya matrisi:** hansı severity → kim, hansı kanal, hansı müddət
  (gecə saatları daxil) - kickoff-da təsdiqlənir.
- İlk 2-4 həftə tuning (yalançı alarm təmizlənməsi); bu dövrdə alert çox
  ola bilər.
- Aylıq hesabat: insidentlər, trendlər, tövsiyələr.

## 9. Təhvildə əlavə olaraq

- Yekun hesabat + retest nəticələri.
- Bütün girişlərin ləğvi təsdiqi (bu sahədə xüsusilə vacib) + sübut silinməsi.
- Növbəti addım tövsiyələri (illik retest, monitorinq).
