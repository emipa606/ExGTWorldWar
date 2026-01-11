using RimWorld;
using Verse;

namespace MYDE_ExGTWorldWar;

public class HediffComp_WhitePhosphorus : HediffComp
{
    public readonly int TickMax = 60;
    public int Tick;

    public CompProperties_WhitePhosphorus Props => (CompProperties_WhitePhosphorus)props;

    public override void CompPostPostAdd(DamageInfo? dinfo)
    {
        base.CompPostPostAdd(dinfo);
        if (Pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.CoveredInFirefoam) != null)
        {
            parent.Severity -= 1f;
        }
    }

    public override void CompPostTick(ref float severityAdjustment)
    {
        base.CompPostTick(ref severityAdjustment);
        Tick++;
        if (Tick < TickMax)
        {
            return;
        }

        Tick = 0;
        var pawn = Pawn;
        GenSpawn.Spawn(ThingDefOf.Fire, pawn.Position, pawn.Map);
        pawn.TryAttachFire(0.3f, parent.pawn);
    }
}