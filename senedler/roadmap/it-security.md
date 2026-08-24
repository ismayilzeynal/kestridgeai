# IT Security - Layihə yol xəritəsi

Müraciətdən təhvilə qədər bütün addımlar. Dəmir qayda: **yazılı icazə
olmadan heç bir test başlamır** - adi skan da daxil.

| # | Mərhələ | Müddət | Nəticə |
| --- | --- | --- | --- |
| 0 | Müraciət və cavab | 1-2 iş günü | Görüş vaxtı təyin olunub |
| 1 | İlk görüş (discovery) | 30-45 dəq | Ehtiyac aydındır (audit / pentest / monitorinq) |
| 2 | Scope və yazılı icazə | 2-4 iş günü | İmzalanmış authorization + scope siyahısı |
| 3 | Təklif və müqavilə | 2-3 iş günü | Metodologiya, müddət, qiymət imzalanıb |
| 4 | Kickoff | 2-3 gün | Test pəncərələri, fövqəladə əlaqə razılaşdırılıb |
| 5 | İcra (qiymətləndirmə) | 1-3 həftə | Testlər scope daxilində tamamlanıb |
| 6 | Hesabat | 3-5 iş günü | Severity üzrə tapıntılar + remediasiya addımları |
| 7 | Remediasiya dəstəyi | plana görə | Zəifliklər bağlanır |
| 8 | Retest | 2-5 iş günü | Bağlanma təsdiqlənib |
| 9 | Yekun hesabat və təhvil | 1-2 gün | Təmiz status, girişlərin ləğvi |

## Addımlar

**0. Müraciət və cavab.** 1-2 iş günü içində cavab, görüş vaxtı təyin edilir.

**1. İlk görüş.** Nə lazımdır: təhlükəsizlik auditi, penetration test,
compliance hazırlığı, yoxsa davamlı monitorinq? Mühitin ümumi mənzərəsi
alınır. NDA adətən elə bu mərhələdə imzalanır.

**2. Scope və yazılı icazə.** Hansı sistemlər test olunacaq (IP-lər,
domenlər, tətbiqlər) - dəqiq siyahı. Müştəri rəhbərliyindən **yazılı
icazə (authorization letter)** alınır. Scope-da olmayan heç nəyə
toxunulmur.

**3. Təklif və müqavilə.** Metodologiya, test növləri, müddət, qiymət,
hesabat formatı. Xidmət kəsintisinə səbəb ola biləcək testlər yalnız
ayrıca açıq razılıqla scope-a düşür.

**4. Kickoff.** Test pəncərələri (adətən iş saatlarından kənar), hər iki
tərəfdən 24/7 fövqəladə əlaqə nömrələri, dayandırma proseduru
razılaşdırılır. Qaydalar: [qaydalar/it-security.md](../qaydalar/it-security.md).

**5. İcra.** Testlər scope və pəncərələr daxilində aparılır.
**Kritik zəiflik tapılan kimi hesabat gözlənilmir - 24 saat içində
birbaşa bildirilir.**

**6. Hesabat.** Tapıntılar Critical / High / Medium / Low üzrə, hər biri
üçün: təsvir, sübut, biznes riski, konkret remediasiya addımı. Rəhbərlik
üçün ayrıca qısa xülasə. Hesabat yalnız şifrəli kanalla ötürülür.

**7. Remediasiya dəstəyi.** Müştəri özü bağlayır və ya biz bağlayırıq - plana görə. Suallara cavab bu mərhələdə dayanmır.

**8. Retest.** Bağlanmış zəifliklər yenidən yoxlanılır. Plana daxil olan
bir retest əlavə ödənişsizdir.

**9. Yekun hesabat və təhvil.** Təmiz status sənədləşir, bizim bütün
girişlər ləğv olunur və bu, təhvil sənədində təsdiqlənir. **Zəifliklərin
həqiqətən bağlandığına əmin olmadan işi bağlamırıq.**

## Davamlı monitorinq xidməti üçün əlavə

Monitorinq sifariş olunubsa, 5-9 əvəzinə: log mənbələrinin qoşulması →
2-4 həftə tuning (yalançı alarmların təmizlənməsi) → eskalasiya matrisinin
təsdiqi → razılaşdırılmış monitorinq saatlarına keçid → aylıq hesabatlar.
