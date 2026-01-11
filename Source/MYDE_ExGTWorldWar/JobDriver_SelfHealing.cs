using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace MYDE_ExGTWorldWar;

public class JobDriver_SelfHealing : JobDriver
{
    public readonly int FleckTickMax = 60;
    public readonly float HealNum = 5f;
    public int FleckTick;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDespawnedOrNull(TargetIndex.A);
        var WaitToHeal = ToilMaker.MakeToil("WaitToHeal");
        WaitToHeal.initAction = delegate { WaitToHeal.actor.pather.StopDead(); };
        WaitToHeal.defaultCompleteMode = ToilCompleteMode.Delay;
        WaitToHeal.defaultDuration = 600;
        WaitToHeal.tickAction = delegate
        {
            FleckTick++;
            if (FleckTick < FleckTickMax)
            {
                return;
            }

            FleckTick = 0;
            var map = pawn.Map;
            var dataStatic = FleckMaker.GetDataStatic(pawn.DrawPos, map, FleckDefOf.HealingCross);
            var velocityAngle = Rand.Range(-30f, 30f);
            dataStatic.velocityAngle = velocityAngle;
            dataStatic.velocitySpeed = Rand.Range(0.5f, 1f);
            map.flecks.CreateFleck(dataStatic);
        };
        WaitToHeal.WithProgressBarToilDelay(TargetIndex.A);
        yield return WaitToHeal;
        yield return new Toil
        {
            initAction = delegate
            {
                Hediff hediff = null;
                foreach (var hediff2 in pawn.health.hediffSet.hediffs)
                {
                    if (hediff2 is not Hediff_Injury || hediff2.IsPermanent())
                    {
                        continue;
                    }

                    hediff = hediff2;
                    break;
                }

                if (hediff != null)
                {
                    hediff.Heal(HealNum);
                    var drawPos = pawn.DrawPos;
                    var map = pawn.Map;
                    FleckMaker.ThrowSmoke(drawPos, map, 1.5f);
                    FleckMaker.ThrowMicroSparks(drawPos, map);
                    FleckMaker.ThrowLightningGlow(drawPos, map, 1.5f);
                    pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
                    var makeJob = JobMaker.MakeJob(MYDE_ThingDefOf.MYDE_ExGTWorldWar_Job_SelfHealing, pawn);
                    makeJob.count = 1;
                    pawn.jobs.TryTakeOrderedJob(makeJob, JobTag.Misc);
                }
                else
                {
                    pawn.records.Increment(RecordDefOf.ThingsRepaired);
                    pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
                    pawn.stances.SetStance(new Stance_Cooldown(30, pawn, null));
                }
            },
            defaultCompleteMode = ToilCompleteMode.Instant
        };
    }
}