# Ümumi iş qaydaları (bütün sahələr üçün)

Bu sənəd hər layihədə keçərlidir. Sahəyə xas qaydalar ayrıca fayllardadır.
Burada yazılanlar kickoff görüşündə müştəri ilə razılaşdırılır və
dəyişiklik yalnız yazılı təsdiqlə olur.

## 1. Prinsiplər

- **Least privilege** — yalnız lazım olan sistemə, minimal səviyyədə giriş.
- **Staging-first** — production-a birbaşa dəyişiklik yoxdur; əvvəl test mühiti.
- **Yazılı iz** — bütün qərarlar və razılaşmalar email ilə təsdiqlənir.
  Şifahi razılaşma = razılaşma deyil.
- **Konfidensiallıq** — müştəri məlumatı 3-cü tərəflə paylaşılmır; NDA
  istənilən mərhələdə imzalanır.
- **Tam işləyənə qədər ayrılmırıq** — təhvil yalnız stabilizasiyadan sonra.

## 2. Kommunikasiya

- Hər iki tərəfdən **bir məsul şəxs (POC)** təyin olunur — bütün suallar
  onun üzərindən gedir.
- Kanallar: email (rəsmi qərarlar üçün), video görüş (Google Meet / Zoom),
  müştəri istəsə Slack / Teams (gündəlik operativ üçün).
- **Həftəlik status:** sabit gün/saat təyin olunur. Format: nə edildi,
  nə edilir, nə mane olur. 15–30 dəqiqədən uzun çəkmir.
- Cavab müddətimiz: adi sual — 1 iş günü; bloklayan məsələ — 4 iş saatı.

## 3. Girişlər (access)

- Müştərinin **şəxsi və ya admin hesabının parolunu heç vaxt istəmirik**.
  Bizim üçün ayrıca adlı hesablar (`ad.soyad@ / -aivanta` formasında) və ya
  bir service account yaradılır.
- Bütün hesablarda **MFA aktiv** olmalıdır; mümkün olan yerdə giriş VPN
  arxasından.
- Girişlər **müddətli verilir — standart 30 gün.** Layihə davam edirsə hər
  ayın sonunda uzadılma email ilə istənilir (kimə hansı giriş, nə üçün,
  nə qədər müddətə). Bu email həm bizim, həm müştərinin qeydiyyatıdır.
- Girişlərin siyahısı layihə boyu bir sənəddə saxlanılır: kim, hansı
  sistem, hansı səviyyə, verilmə/bitmə tarixi.
- Layihə bitəndə **offboarding:** siyahıdakı bütün girişlər ləğv olunur,
  ləğv təhvil sənədində təsdiqlənir.

## 4. İş mühiti

- Standart istək — müştəridən bizim işlər üçün **1 ayrılmış VM:**
  minimum 4 vCPU / 16 GB RAM / 100 GB SSD, Ubuntu 22.04 LTS (və ya
  müştərinin öz OS standartı). Sahəyə görə fərqli tələblər sahə
  sənədindədir (məs. AI üçün GPU).
- VM-ə giriş **yalnız SSH açarı ilə** (parolla yox), IP məhdudiyyəti və ya
  VPN arxasından.
- Müştəri cloud-dadırsa (AWS / Azure / GCP): VM əvəzinə ayrıca
  project / resource group + bizim üçün məhdud IAM rollar. Xərc müştərinin
  hesabında qalır — görünürlük tam onlarda olur.
- Production-da iş yalnız planlaşdırılmış deploy pəncərəsində, rollback
  planı hazır halda.

## 5. Data

- Ötürülmə **yalnız şifrəli kanalla:** SFTP, müştərinin cloud storage-i
  (bizə açılmış bucket/qovluq) və ya müddətli şifrəli link.
  **Email ilə data göndərilmir.**
- Mümkün olan yerdə **anonimləşdirilmiş / maskalanmış data** ilə işləyirik.
  Production data yalnız zərurət olduqda və yazılı razılıqla.
- Data yalnız layihə mühitində saxlanılır — şəxsi kompüterlərə kopyalanmır.
- Layihə bitdikdən sonra **30 gün içində data silinir** və silinmə təsdiqi
  email ilə göndərilir (müqavilədə fərqli saxlama nəzərdə tutulmayıbsa).

## 6. Scope və dəyişikliklər

- Yeni istək = **change request:** email ilə yazılır, effort və müddətə
  təsiri qiymətləndirilir, təsdiqdən sonra plana salınır.
- "Balaca şeydir, elə indi əlavə edək" — yoxdur. Kiçik görünən dəyişiklik
  də qeyd olunur; sadəcə qiymətləndirməsi sürətli olur.

## 7. İnsident qaydası

- Bizim qurduğumuz sistemdə problem çıxarsa: **kritik (sistem dayanıb) —
  4 saat içində reaksiya; digərləri — 1 iş günü.**
- Hər deploy-dan əvvəl rollback planı hazırdır; kritik insidentdə əvvəl
  sistem geri qaytarılır, sonra səbəb araşdırılır.
- İnsidentdən sonra qısa post-mortem: nə oldu, niyə oldu, təkrarlanmaması
  üçün nə edildi.

## 8. Təhvil paketi (hər layihədə eyni)

1. Arxitektura sxemi və konfiqurasiya sənədi.
2. **Runbook** — sistemi necə işə salmalı / dayandırmalı, tipik problemlər
   və həlləri, kimlə əlaqə saxlamalı.
3. Secrets / parolların təhlükəsiz ötürülməsi (password manager və ya
   şifrəli kanal ilə — açıq mətnlə yox).
4. Təlim sessiyası (1–2 saat, video yazıya alınır).
5. Girişlərin ləğvi + data silinməsi təsdiqi.
6. Dəstək şərtləri: nə daxildir, necə müraciət olunur, cavab müddətləri.
