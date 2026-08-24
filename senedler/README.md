# Kestridge AI - Daxili sənədlər

Bu qovluq xidmət sahələri üzrə iş sənədlərini saxlayır.

**Dillər və formatlar:**
- Azərbaycanca mənbələr: `roadmap/` və `qaydalar/` (markdown)
- İngiliscə mənbələr: `en/roadmap/` və `en/qaydalar/` (markdown)
- **Word versiyaları (paylaşmaq üçün): `word/az/` və `word/en/`** - brendlənmiş .docx, hər dildə 9 sənəd. Mənbə md dəyişəndə Word-ü
  yenidən generasiya edin:

```bash
pip install python-docx
python senedler/tools/make-docx.py
```

  Skript md-ni oxuyub 18 sənədi yenidən yazır. Versiya sətri skriptin
  içindədir (`Versiya 1.1 · Avqust 2026`) - məzmun ciddi dəyişəndə qaldırın.

## roadmap/ - Layihə yol xəritələri
Hər sahə üzrə layihənin 0-dan təhvilə qədər keçdiyi addımlar.
Müştəri ilə söhbətdə və daxili planlaşdırmada istifadə üçün.

| Fayl | Sahə |
| --- | --- |
| [roadmap/ai-solutions.md](roadmap/ai-solutions.md) | AI Solutions |
| [roadmap/automation.md](roadmap/automation.md) | Automation |
| [roadmap/it-security.md](roadmap/it-security.md) | IT Security |
| [roadmap/analytics.md](roadmap/analytics.md) | Analytics |

## qaydalar/ - İş qaydaları (dokumentasiya)
İşi icra edən mütəxəssis üçün əməli playbook - "hansı halda nə et, hansı
tool ilə, hansı qayda ilə". Girişlər (VPN/bastion/SSH/RDP/cloud IAM/DB),
secrets, mühitlər (VM/IaC), data, backup/rollback, audit, insident,
offboarding, təhvil + hər sahə üçün konkret tool stack.

| Fayl | Məzmun |
| --- | --- |
| [qaydalar/00-umumi.md](qaydalar/00-umumi.md) | Bütün sahələr üçün ortaq qaydalar - əvvəlcə bunu oxu |
| [qaydalar/ai-solutions.md](qaydalar/ai-solutions.md) | AI layihələrinə xas qaydalar |
| [qaydalar/automation.md](qaydalar/automation.md) | Avtomatlaşdırma layihələrinə xas qaydalar |
| [qaydalar/it-security.md](qaydalar/it-security.md) | Təhlükəsizlik işlərinə xas qaydalar |
| [qaydalar/analytics.md](qaydalar/analytics.md) | Analitika layihələrinə xas qaydalar |

**Əsas prinsip (bütün sahələrdə):** layihənin tam işlədiyinə əmin olmadan
işi təhvil verib getmirik. Təhvil yalnız stabilizasiya mərhələsi uğurla
bitdikdən sonra olur.
