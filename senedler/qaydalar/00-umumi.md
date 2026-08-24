# Ümumi İş Qaydaları (bütün sahələr üçün)

Bu sənəd hər layihədə keçərlidir və işi icra edən mühəndis üçün əməli
təlimatdır - "hansı halda nə etməli, hansı tool ilə, hansı qayda ilə".
Sahəyə xas əlavələr ayrıca fayllardadır. Buradakılar kickoff-da müştəri
ilə razılaşdırılır; dəyişiklik yalnız yazılı təsdiqlə olur.

> **Qızıl qaydalar (heç vaxt pozulmur):** ① Least privilege - yalnız
> lazım olana, minimal səviyyədə giriş. ② Staging-first - production-a
> birbaşa əl vurulmur. ③ Hər qərar email ilə yazılı təsdiqlənir. ④
> Müştəri parolunu heç vaxt istəmirik. ⑤ Tam işləyənə əmin olmadan
> təhvil verib getmirik.

---

## 1. Rollar və məsuliyyət

| Rol | Kim | Nəyə cavabdehdir |
| --- | --- | --- |
| Layihə rəhbəri (PM/POC) | bizdən 1 nəfər | Ümumi gedişat, kommunikasiya, hesabatlar |
| Aparıcı mühəndis | sahə üzrə | Texniki icra, kod, deploy |
| Təhlükəsizlik məsulu | 1 nəfər (paylaşıla bilər) | Girişlər, secrets, offboarding auditi |
| Müştəri POC | müştəridən 1 nəfər | Qərar, giriş təsdiqi, sign-off |

Hər tərəfdən **bir POC** olur - bütün rəsmi qərarlar onun üzərindən keçir.

---

## 2. Müştərini işə salma (onboarding checklist)

Kickoff-dan sonra ilk 3 gündə bunlar bağlanır:

- [ ] NDA imzalanıb (lazımdırsa)
- [ ] POC-lar təyin olunub (hər iki tərəf), əlaqə kanalı qurulub
- [ ] Giriş üsulu razılaşdırılıb (bax. Bölmə 4) və verilib
- [ ] İş mühiti (VM / cloud project) hazırdır (bax. Bölmə 6)
- [ ] Giriş reyestri (access register) faylı açılıb
- [ ] Secrets üçün paylaşılan vault qovluğu yaradılıb (bax. Bölmə 5)
- [ ] Həftəlik status günü/saatı təyin olunub
- [ ] Data ötürülməsi lazımdırsa kanal razılaşdırılıb (bax. Bölmə 7)

---

## 3. Bizim iş cihazlarımız (endpoint təhlükəsizliyi)

Müştəri sisteminə yalnız bu şərtləri ödəyən cihazdan qoşuluruq:

- **Tam disk şifrələməsi** aktiv (BitLocker / FileVault / LUKS).
- Ekran kilidi ≤ 5 dəq, güclü login parolu + cihazda MFA.
- Antivirus / EDR aktiv, OS və brauzer yenilənmiş.
- **Müştəri datası şəxsi / idarə olunmayan cihazda saxlanmır** - yalnız
  layihə mühitində. Yerli kopya lazımdırsa şifrəli qovluqda, iş bitəndə
  silinir.
- Şəxsi USB, şəxsi bulud (Google Drive/iCloud) ilə müştəri datası
  daşınmır.
- İş bitəndə cihazdan müştəriyə aid nə varsa (config, açar, data) silinir.

---

## 4. Girişlər və qoşulma (ƏN VACİB BÖLMƏ)

### 4.1 Ümumi prinsiplər

- **Müştərinin şəxsi və ya admin hesabının parolunu heç vaxt istəmirik.**
  Bizim üçün **adlı hesab** (`ad.soyad@musteri` formasında) və ya bir
  **service account** yaradılır.
- Bütün hesablarda **MFA məcburi**; mümkünsə hardware açar (YubiKey) və ya
  authenticator app (SMS yox).
- Giriş **müddətlidir - standart 30 gün.** Layihə davam edirsə hər ayın
  sonunda uzadılma email ilə istənilir (kim, hansı giriş, nə üçün, nə
  qədər). Bu email hər iki tərəf üçün qeyddir.
- Bütün girişlər **giriş reyestrində** saxlanılır: kim / hansı sistem /
  səviyyə / verilmə tarixi / bitmə tarixi.

### 4.2 Qoşulma üsulu - üstünlük sırası

Aşağıdan yuxarı deyil, **yuxarıdan aşağı** seçirik: birinci mümkün olan
variant götürülür.

| Sıra | Üsul | Nə vaxt | Şərtlər |
| --- | --- | --- | --- |
| 1 | Müştərinin öz VPN-i | Müştəridə VPN varsa | WireGuard / OpenVPN / IPsec; bizə adlı profil |
| 2 | Zero-trust mesh | VPN yoxdursa, sürətli lazımdırsa | Tailscale / Cloudflare Access / Twingate |
| 3 | Bastion / jump host | Şəbəkə daxilinə keçid | Tək giriş nöqtəsi, loglanır, MFA |
| 4 | Birbaşa (allowlist) | Yalnız izolə tək server | Yalnız bizim statik IP açıq, geri qalan bağlı |

**Heç vaxt:** RDP/SSH/DB portunu internetə açıq qoymaq, paylaşılan parol,
paylaşılan hesab, VPN-siz birbaşa 0.0.0.0/0 giriş.

### 4.3 SSH (Linux serverlər)

- **Yalnız açar ilə** giriş (parol authentication söndürülür).
  Açar tipi: `ed25519`. Açar parolla qorunur, vault-da saxlanılır.
- Bastion varsa `ProxyJump` istifadə olunur, birbaşa yox:

  ```
  # ~/.ssh/config
  Host musteri-bastion
      HostName bastion.musteri.com
      User kestridge.name
      IdentityFile ~/.ssh/musteri_ed25519
  Host musteri-app
      HostName 10.0.1.20
      User kestridge.name
      ProxyJump musteri-bastion
      IdentityFile ~/.ssh/musteri_ed25519
  ```

- Agent forwarding yalnız zərurət olduqda; sudo əməliyyatları loglanır.
- Server açarları (host keys) ilk qoşulmada təsdiqlənir və qeyd olunur.

### 4.4 Windows serverlər (RDP)

- RDP **birbaşa internetdən yox** - yalnız VPN və ya bastion arxasından.
- **NLA (Network Level Authentication)** aktiv, adlı hesab + MFA.
- Uzun sessiyalar loglanır; iş bitəndə sessiya bağlanır (logout).

### 4.5 Cloud konsolları (AWS / Azure / GCP)

- **SSO ilə giriş** (Google/Microsoft), root/owner hesabı istifadə
  edilmir.
- Bizə **məhdud IAM rolu** verilir - yalnız layihəyə lazım servislərə.
- **Uzunömürlü access key yaratmırıq** - müvəqqəti kredensial:
  `aws sso login` / STS, `az login`, `gcloud auth login`.
- Bütün əməliyyatlar cloud audit log-da qalır (CloudTrail / Activity Log).

### 4.6 Verilənlər bazası

- Bizə **read-only istifadəçi** (yazma lazımdırsa ayrıca, məhdud).
- Baza **internetə açıq deyil** - SSH tunnel və ya bastion üzərindən:

  ```
  ssh -L 5432:db.internal:5432 musteri-bastion
  # sonra lokal 127.0.0.1:5432-ə qoşul
  ```

- Prod bazada ağır sorğu yalnız iş saatından kənar / replica üzərində.

### 4.7 SaaS admin panelləri və API

- Adlı hesab + MFA; paylaşılan login yox.
- API açarları vault-da; kodda / Git-də açıq saxlanmır (bax. Bölmə 5).
- API-yə minimal scope (yalnız lazım icazələr).

---

## 5. Secrets (parol, açar, token) idarəetməsi

- Mərkəz: **parol meneceri / vault** - 1Password və ya Bitwarden; hər
  müştəri üçün ayrıca paylaşılan qovluq (vault).
- **Heç vaxt Git-ə düşmür.** `.env` faylları `.gitignore`-da; nümunə üçün
  `.env.example` (dəyərsiz).
- Commit-dən əvvəl **secret scanning:** `gitleaks` (mümkünsə pre-commit
  hook). Təsadüfən düşərsə - dərhal rotasiya (dəyişdirmə), tarixdən
  təmizləmə.
- Secrets **şifrəli kanalla** ötürülür (vault paylaşımı / şifrəli link) - email, mesaj, Slack ilə açıq mətnlə yox.
- Rotasiya: layihə sonunda və işçi dəyişəndə bütün paylaşılan secrets
  yenilənir.

---

## 6. İş mühiti və infrastruktur

- **Standart tələb - 1 ayrılmış VM:** min. 4 vCPU / 16 GB RAM / 100 GB
  SSD, Ubuntu 22.04 LTS (və ya müştərinin OS standartı). Sahəyə xas
  tələblər (məs. AI üçün GPU) sahə sənədindədir.
- Mühit qatları ayrı: **dev / staging / production**. Test staging-də;
  prod-a yalnız təsdiqli deploy.
- **Adlandırma:** `kestridge-<musteri>-<mühit>-<rol>` (məs.
  `kestridge-acme-staging-app`).
- Mümkün olan yerdə **Infrastructure as Code:** Terraform (resurslar),
  Ansible (server konfiqurasiyası) - əl ilə "click-ops" minimuma endirilir
  ki, sonradan təkrar qurmaq mümkün olsun.
- Konteynerləşdirmə: **Docker** - mühit fərqlərini aradan qaldırır.
- **Cloud xərci müştərinin hesabındadır** - görünürlük tam onlarda qalır.

---

## 7. Data ilə iş

- **Təsnifat:** hər data "public / internal / confidential / PII" kimi
  qiymətləndirilir; PII və confidential ən sərt rejimdə.
- **Şifrələmə:** ötürülmədə (TLS/SSH) və saxlanmada (disk şifrələmə).
- **Ötürülmə üsulları (yalnız bunlar):** SFTP, müştərinin bulud storage-i
  (bizə açılmış bucket/qovluq), müddətli şifrəli link. **Email ilə data
  göndərilmir.**
- **Anonimləşdirmə / maskalama:** mümkün olan yerdə maskalanmış data ilə
  işləyirik; prod data yalnız zərurət + yazılı razılıqla.
- **Saxlama və silinmə:** data yalnız layihə mühitində; iş bitəndən sonra
  **30 gün içində silinir**, silinmə email ilə təsdiqlənir (müqavilədə
  fərqli yoxdursa).
- **Data residency:** ABŞ müştəriləri üçün data ABŞ regionunda saxlanılır
  (müştəri fərqli tələb etməyibsə).

---

## 8. Versiya nəzarəti və dəyişiklik idarəetməsi

- Bütün kod / IaC / konfiq **Git-də** (GitHub / GitLab, privat repo).
- İş branch-larda; `main`-ə **məcburi PR review** (ən azı 1 nəfər baxır).
- **Production-a birbaşa push yoxdur** - yalnız təsdiqli deploy pəncərəsində.
- Hər deploy-dan əvvəl **snapshot / backup** (bax. Bölmə 9), rollback
  planı hazır.
- Mümkünsə **CI/CD:** avtomatik test + deploy pipeline (GitHub Actions).

---

## 9. Backup, rollback və fəlakət bərpası

- **Dəyişiklikdən əvvəl:** VM snapshot / DB dump / config kopyası.
- Rollback planı hər deploy-dan əvvəl yazılır: nəyi necə geri qaytarmalı,
  neçə dəqiqədə.
- Kritik sistemlərdə backup avtomatlaşdırılır və **bərpa sınaqdan
  keçirilir** (yoxlanmamış backup = backup deyil).

---

## 10. Loglama, monitorinq və audit

- Bastion və serverlərdə **sessiya logları** - kim, nə vaxt, nə etdi.
- Cloud audit izləri aktiv (CloudTrail / Activity Log / Cloud Audit Logs).
- Kritik sistemlərdə monitorinq + alert (bax. sahə sənədləri).
- Loglar müştəriyə aiddir və konfidensialdır; bizim tərəfdə yalnız layihə
  komandası baxır.

---

## 11. Kommunikasiya və status

- Kanallar: **email** (rəsmi qərarlar), **video** (Google Meet / Zoom),
  müştəri istəsə **Slack / Teams** (gündəlik operativ).
- **Həftəlik status:** sabit gün. Format - nə edildi / nə edilir / nə mane
  olur. 15-30 dəqiqə.
- Cavab müddətimiz: adi sual - 1 iş günü; bloklayan məsələ - 4 iş saatı.
- İş idarəetməsi: **issue tracker** (Jira / Linear / GitHub Issues) - hər tapşırıq izlənir.

---

## 12. Scope və dəyişikliklər

- Yeni istək = **change request:** email ilə yazılır, effort və müddətə
  təsiri qiymətləndirilir, təsdiqdən sonra plana salınır.
- "Balaca şeydir, elə indi əlavə edək" - yoxdur. Kiçik dəyişiklik də qeyd
  olunur; sadəcə qiymətləndirməsi sürətli olur.

---

## 13. İnsident qaydası

| Severity | Nümunə | Reaksiya |
| --- | --- | --- |
| Kritik | Sistem dayanıb, data sızması | 4 saat içində, dərhal xəbərdarlıq |
| Yüksək | Əsas funksiya işləmir | 1 iş günü |
| Orta / aşağı | Kiçik nasazlıq | Növbəti status / plana görə |

- Kritik insidentdə **əvvəl sistem geri qaytarılır (rollback), sonra səbəb
  araşdırılır.**
- İnsidentdən sonra qısa **post-mortem:** nə oldu, niyə oldu,
  təkrarlanmaması üçün nə edildi.

---

## 14. Hüquqi / uyğunluq - mühəndisin bilməli olduğu

- **NDA** istənilən mərhələdə imzalana bilər; müştəri məlumatı 3-cü tərəflə
  paylaşılmır.
- **Təhlükəsizlik işlərində yazılı icazə (authorization letter) olmadan
  heç nə başlamır** (bax. IT Security qaydaları).
- **Scope intizamı:** icazə verilməyən sistemə toxunulmur - maraqlı
  görünsə belə.

---

## 15. Offboarding (layihə bitəndə)

- [ ] Giriş reyestrindəki **bütün girişlər ləğv olunur**
- [ ] Paylaşılan secrets rotasiya olunur (müştəri tərəfdə dəyişilir)
- [ ] Bizim cihazlardan müştəri datası silinir (30 gün qaydası)
- [ ] Ləğv və silinmə **email ilə təsdiqlənir**
- [ ] Təhvil paketi verilir (bax. Bölmə 16)

---

## 16. Təhvil paketi (hər layihədə eyni)

1. Arxitektura sxemi + konfiqurasiya sənədi.
2. **Runbook** - sistemi necə işə salmalı / dayandırmalı, tipik problemlər
   və həlləri, kimlə əlaqə.
3. Secrets / parolların təhlükəsiz ötürülməsi (vault və ya şifrəli kanal).
4. Təlim sessiyası (1-2 saat, video yazıya alınır).
5. Girişlərin ləğvi + data silinməsi təsdiqi.
6. Dəstək şərtləri: nə daxildir, necə müraciət, cavab müddətləri.
