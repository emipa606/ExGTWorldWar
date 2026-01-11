using System.Collections.Generic;
using Verse;

namespace MYDE_ExGTWorldWar;

public class Comp_GiveExtraVerbGizmos_Pawn : ThingComp
{
    public CompProperties_GiveExtraVerbGizmos_Pawn Props => props as CompProperties_GiveExtraVerbGizmos_Pawn;

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        foreach (var gizmo in base.CompGetGizmosExtra())
        {
            yield return gizmo;
        }

        if (parent is not Pawn { IsPlayerControlled: true } pawn)
        {
            yield break;
        }

        var thingWithComps = pawn.equipment.Primary;
        if (thingWithComps == null || thingWithComps.AllComps.NullOrEmpty())
        {
            yield break;
        }

        foreach (var thingComp in thingWithComps.AllComps)
        {
            if (thingComp is not Comp_GiveExtraVerbGizmos_Equipment Comp_GiveExtraVerbGizmos_Equipment)
            {
                continue;
            }

            Comp_GiveExtraVerbGizmos_Equipment.Pawn = pawn;
            foreach (var item in Comp_GiveExtraVerbGizmos_Equipment.CompGetGizmosExtra())
            {
                yield return item;
            }

            yield break;
        }
    }
}