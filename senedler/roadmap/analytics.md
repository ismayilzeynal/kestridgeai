# Analytics - Layihə yol xəritəsi

Müraciətdən təhvilə qədər bütün addımlar. Fərqləndirici cəhət: heç bir
dashboard **rəqəmləri mənbə ilə üzləşdirilmədən** müştəriyə açılmır.

| # | Mərhələ | Müddət | Nəticə |
| --- | --- | --- | --- |
| 0 | Müraciət və cavab | 1 to 2 iş günü | Görüş vaxtı təyin olunub |
| 1 | İlk görüş (discovery) | 30 to 45 dəq | Hansı qərarlar üçün nə lazımdır |
| 2 | Data mənbələri auditi | 3 to 5 iş günü | Mənbələrin siyahısı və vəziyyəti |
| 3 | Təklif və müqavilə | 2 to 3 iş günü | KPI siyahısı + scope imzalanıb |
| 4 | Kickoff | ~1 həftə | Read-only girişlər, mühit hazır |
| 5 | Data pipeline | 1 to 3 həftə | Data avtomat yığılır və təmizlənir |
| 6 | Dashboard qurulması | sprintlərlə | Hər həftə baxış, iterativ düzəliş |
| 7 | Validasiya (sign-off) | 3 to 5 iş günü | Rəqəmlər mənbə ilə üzləşdirilib, təsdiq alınıb |
| 8 | Canlı + təlim | 2 to 3 gün | Komanda özü istifadə edə bilir |
| 9 | Stabilizasiya | 2 to 4 həftə | Refresh-lər stabil, alertlər işləyir |
| 10 | Təhvil-təslim | 1 to 2 gün | Sənədlər, girişlərin ləğvi |

## Addımlar

**0. Müraciət və cavab.** 1 to 2 iş günü içində cavab, görüş vaxtı təyin edilir.

**1. İlk görüş.** Rəqəmlərə baxıb hansı qərarları vermək istəyirlər?
Bu sualdan başlayırıq - dashboard bəzək deyil, qərar alətidir. İlkin KPI
siyahısı çıxır.

**2. Data mənbələri auditi.** Data haradadır (CRM, ERP, DB, Excel...),
hansı keyfiyyətdədir, KPI-ları hesablamağa yetərlidirmi. Çatışmayan data
varsa - açıq deyilir və plan buna görə qurulur.

**3. Təklif və müqavilə.** KPI-ların dəqiq tərifi (**metric definitions - hər metrik necə hesablanır, yazılı**), dashboard siyahısı, refresh tezliyi,
müddət, qiymət. Təriflər əvvəldən təsdiqlənir ki, sonda "bu rəqəm niyə
belədir" mübahisəsi olmasın.

**4. Kickoff.** Mənbələrə **read-only** girişlər alınır, iş mühiti qurulur,
status günü razılaşdırılır.
Qaydalar: [qaydalar/analytics.md](../qaydalar/analytics.md).

**5. Data pipeline.** Data mənbələrdən avtomat yığılır, təmizlənir və bir
yerə (warehouse) toplanır. Pipeline xətaları üçün alert qurulur.

**6. Dashboard qurulması.** İterativ: hər həftə hazır hissə göstərilir,
rəy alınır, düzəliş edilir. İstifadəçilər prosesə əvvəldən qoşulur.

**7. Validasiya.** Hər dashboard rəqəmi mənbə sistemlə üzləşdirilir.
Müştəri **"rəqəmlər düzdür" təsdiqini (sign-off)** verir - canlıya yalnız
bundan sonra çıxırıq.

**8. Canlı + təlim.** İstifadəçilərə giriş açılır, təlim sessiyası
keçirilir (yazıya alınır), qısa istifadə təlimatı verilir.

**9. Stabilizasiya.** Refresh-lərin stabil işləməsi, data gecikməsi və
alertlər izlənilir; çıxan problem dərhal həll olunur. **Sistemin tam
işlədiyinə əmin olmadan təhvil verib getmirik.**

**10. Təhvil-təslim.** Pipeline sənədləri, metric definitions, runbook,
girişlərin ləğvi. Sonrası - razılaşdırılmış dəstək rejimi.
