using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MYDE_ExGTWorldWar;

public class PawnJumper_Dash : PawnDasher
{
    public bool IF_DoDamageFinish;

    public List<Pawn> potentialTargets;

    protected override void Tick()
    {
        if (!IF_DoDamageFinish)
        {
            Check_DashAndStop();
        }

        base.Tick();
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        var list = new List<Pawn>();
        foreach (var allPawn in map.mapPawns.AllPawns)
        {
            if (allPawn.Spawned && allPawn.Faction != FlyingPawn.Faction &&
                (DrawPos - allPawn.DrawPos).magnitude < 1.5f * flightDistance)
            {
                list.Add(allPawn);
            }
        }

        potentialTargets = list;
    }

    public void Check_DashAndStop()
    {
        foreach (var potentialTarget in potentialTargets)
        {
            if (!((potentialTarget.DrawPos - DrawPos).magnitude < 0.7f))
            {
                continue;
            }

            DoDamage(potentialTarget);
            IF_DoDamageFinish = true;
            break;
        }
    }

    public void DoDamage(Pawn pawn)
    {
        var damageInfo = new DamageInfo(DamageDefOf.Stab, 40f, 999999f);
        if (pawn.RaceProps.Humanlike)
        {
            pawn.Kill(damageInfo);
            pawn.Faction?.Notify_MemberDied(pawn, damageInfo, false, true, Map);
        }
        else
        {
            pawn.TakeDamage(damageInfo);
            pawn.Faction?.Notify_MemberTookDamage(pawn, damageInfo);
        }

        destCell = pawn.Position;
        ticksFlying = ticksFlightTime;
    }
}