# AI Solutions - Layihə yol xəritəsi

Müraciətdən təhvilə qədər bütün addımlar. Müddətlər tipik layihə üçündür,
konkret rəqəmlər müqavilədə dəqiqləşir.

| # | Mərhələ | Müddət | Nəticə |
| --- | --- | --- | --- |
| 0 | Müraciət və cavab | 1-2 iş günü | Görüş vaxtı təyin olunub |
| 1 | İlk görüş (discovery) | 30-45 dəq | Problem və hədəf aydındır |
| 2 | Texniki qiymətləndirmə | 3-5 iş günü | Data auditi + həll variantı |
| 3 | Təklif və müqavilə | 2-3 iş günü | İmzalanmış scope + qiymət |
| 4 | Kickoff | ~1 həftə | Girişlər, mühit, data hazır |
| 5 | POC / pilot | 2-4 həftə | İşlək prototip, go/no-go |
| 6 | Tam icra | sprintlərlə | Model + inteqrasiya hazır |
| 7 | Test və validasiya | 1-2 həftə | Hədəflər real datada təsdiqlənib |
| 8 | Canlıya keçid | 1-3 gün | Production-da işləyir |
| 9 | Stabilizasiya | 2-4 həftə | Sistem sübut olunmuş şəkildə stabil |
| 10 | Təhvil-təslim | 2-3 gün | Sənədlər, təlim, girişlərin ləğvi |

## Addımlar

**0. Müraciət və cavab.** Form / email ilə müraciət gəlir. 1-2 iş günü
içində cavab yazılır, ilk görüş üçün vaxt təyin edilir.

**1. İlk görüş.** Müştərinin problemi, mövcud sistemləri, datanın vəziyyəti
və gözlənilən nəticə dinlənilir. İstəsə elə bu mərhələdə NDA imzalanır.
Satış yox, anlamaq görüşüdür.

**2. Texniki qiymətləndirmə.** Data nümunəsinə baxılır (keyfiyyət, həcm,
əlçatanlıq), həll variantları və təxmini effort müəyyənləşir. Data AI üçün
yararlı deyilsə - bunu açıq deyirik və əvvəlcə nəyin düzəldilməli olduğunu
göstəririk.

**3. Təklif və müqavilə.** Scope, mərhələlər, müddət, qiymət və **rəqəmlə
yazılmış qəbul kriteriyaları** (məs. minimum dəqiqlik faizi, cavab vaxtı)
təqdim olunur. Təsdiqdən sonra müqavilə imzalanır.

**4. Kickoff.** Girişlər alınır, iş mühiti qurulur (VM / cloud project),
data ötürülür, kommunikasiya kanalı və həftəlik status günü razılaşdırılır.
Qaydalar: [qaydalar/ai-solutions.md](../qaydalar/ai-solutions.md).

**5. POC / pilot.** Məhdud data üzərində işlək prototip qurulur və hədəf
metriklərlə ölçülür. Nəticə müştəri ilə birlikdə qiymətləndirilir - davam / düzəliş / dayanma qərarı burada verilir. Böyük xərcdən əvvəl
yanaşmanın işlədiyi sübut olunur.

**6. Tam icra.** Sprintlərlə: model təkmilləşir, mövcud sistemlərlə
inteqrasiya və (lazımdırsa) interfeys qurulur. Hər həftə status + demo.

**7. Test və validasiya.** Real ssenarilərdə, müştəri komandasının
iştirakı ilə (UAT). Müqavilədəki qəbul kriteriyaları bir-bir yoxlanılır.

**8. Canlıya keçid.** Production deploy + monitorinq (performans, drift,
xətalar) qurulur. Rollback planı əvvəlcədən hazırdır.

**9. Stabilizasiya.** Canlıdan sonra sistem real yük altında izlənilir,
çıxan hər problem dərhal həll olunur. **Sistemin tam işlədiyinə əmin
olmadan layihəni təhvil verib getmirik - bu mərhələ bağlanmadan təhvil
yoxdur.**

**10. Təhvil-təslim.** Sənədlər (arxitektura, konfiqurasiya, runbook),
komanda üçün təlim sessiyası, secrets-lərin təhlükəsiz ötürülməsi və bizim
girişlərin ləğvi. Sonrası - razılaşdırılmış dəstək rejimi.
