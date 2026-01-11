using Verse;

namespace MYDE_ExGTWorldWar;

public class HediffComp_BurntTentacle : HediffComp
{
    public readonly int RemoveHediffTickMax = 3600;
    public int RemoveHediffTick;

    public HediffCompProperties_BurntTentacle Props => (HediffCompProperties_BurntTentacle)props;

    public override void CompPostTick(ref float severityAdjustment)
    {
        base.CompPostTick(ref severityAdjustment);
        RemoveHediffTick++;
        if (RemoveHediffTick < RemoveHediffTickMax)
        {
            return;
        }

        RemoveHediffTick = 0;
        if (Pawn.apparel.WornApparel.Count <= 0)
        {
            return;
        }

        var foundApparel = true;
        foreach (var apparel in Pawn.apparel.WornApparel)
        {
            if (apparel.def.defName != "WW_BUCM")
            {
                continue;
            }

            foundApparel = false;
            break;
        }

        if (foundApparel)
        {
            Pawn.health.RemoveHediff(parent);
        }
    }
}