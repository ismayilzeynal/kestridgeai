# Automation - Layihə yol xəritəsi

Müraciətdən təhvilə qədər bütün addımlar. Fərqləndirici cəhət: köhnə proses
yeni avtomatlaşdırma **sübut olunana qədər dayandırılmır**.

| # | Mərhələ | Müddət | Nəticə |
| --- | --- | --- | --- |
| 0 | Müraciət və cavab | 1 to 2 iş günü | Görüş vaxtı təyin olunub |
| 1 | İlk görüş (discovery) | 30 to 45 dəq | Hansı proseslər, nə qazanc |
| 2 | Proses xəritələnməsi | 3 to 7 iş günü | As-is proses yazılı təsdiqlənib |
| 3 | Təklif və müqavilə | 2 to 3 iş günü | To-be dizayn + qiymət imzalanıb |
| 4 | Kickoff | ~1 həftə | Test hesabları, mühit hazır |
| 5 | Qurulma (build) | sprintlərlə | Workflow-lar test mühitində işləyir |
| 6 | Paralel dövr | min. 2 həftə | Nəticələr köhnə proseslə üst-üstə düşür |
| 7 | Cutover (keçid) | 1 to 2 gün | Proses avtomat işləyir |
| 8 | Stabilizasiya | 2 to 4 həftə | Real yük altında stabil |
| 9 | Təhvil-təslim | 2 to 3 gün | Runbook, təlim, girişlərin ləğvi |

## Addımlar

**0. Müraciət və cavab.** 1 to 2 iş günü içində cavab, görüş vaxtı təyin edilir.

**1. İlk görüş.** Hansı proseslər vaxt yeyir, harada səhvlər çıxır, nəyi
avtomatlaşdırmaq istəyirlər - dinləyirik. Ən çox qazanc verəcək 1 to 3 proses
seçilir.

**2. Proses xəritələnməsi.** Prosesi bu gün icra edən adamlarla birlikdə
addım-addım sənədləşdiririk (ekran yazısı da olar). **As-is proses müştəri
tərəfindən yazılı təsdiqlənir** - avtomatlaşdırma yalnız düzgün başa
düşülmüş prosesin üstündə qurulur.

**3. Təklif və müqavilə.** To-be (avtomatlaşdırılmış) prosesin dizaynı,
istisna halların davranışı, scope, müddət və qiymət. Qəbul kriteriyası:
prosesin hansı faizi avtomat gedir, xəta halında nə baş verir.

**4. Kickoff.** İnteqrasiya olunacaq sistemlərə test hesabları alınır, test
mühiti qurulur, kommunikasiya kanalı razılaşdırılır.
Qaydalar: [qaydalar/automation.md](../qaydalar/automation.md).

**5. Qurulma.** Workflow-lar qurulur: API varsa API ilə, yoxdursa RPA.
Hər addımda error handling + bildiriş. Hər həftə status + işlək demo.

**6. Paralel dövr.** Yeni avtomatlaşdırma **minimum 2 həftə** köhnə
prosesle yanaşı işləyir, nəticələr gündəlik tutuşdurulur. Fərq çıxarsa
səbəb tapılıb düzəldilir və dövr uzadılır. Köhnə proses bu mərhələdə
toxunulmaz qalır.

**7. Cutover.** Nəticələr üst-üstə düşəndən sonra köhnə prosesdən çıxış
razılaşdırılmış gündə olur. Geri dönüş (köhnə prosesə qayıtma) yolu açıq
saxlanılır.

**8. Stabilizasiya.** Real yük altında izləmə, xətaların dərhal həlli,
lazım gələrsə tənzimləmə. **Prosesin tam işlədiyinə əmin olmadan təhvil
verib getmirik.**

**9. Təhvil-təslim.** Runbook (necə dayandırmalı, necə yenidən başlatmalı,
xəta çıxanda nə etməli), komanda təlimi, girişlərin ləğvi. Sonrası - razılaşdırılmış dəstək rejimi.
