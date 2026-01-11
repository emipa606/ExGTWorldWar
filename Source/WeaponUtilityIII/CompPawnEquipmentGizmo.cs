using System.Collections.Generic;
using Verse;

namespace WeaponUtilityIII;

internal class CompPawnEquipmentGizmo : ThingComp
{
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
        if (thingWithComps?.AllComps.NullOrEmpty() ?? true)
        {
            yield break;
        }

        foreach (var allComp in thingWithComps.AllComps)
        {
            if (allComp is not CompSecondaryVerb compSecondaryVerb)
            {
                continue;
            }

            foreach (var item in compSecondaryVerb.CompGetGizmosExtra())
            {
                yield return item;
            }

            yield break;
        }
    }
}