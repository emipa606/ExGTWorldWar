using HarmonyLib;
using Verse;

namespace MYDE_ExGTWorldWar;

[HarmonyPatch(typeof(DamageWorker_Extinguish), "Apply")]
internal static class MYDE_Harmony_ExGTWorldWar_DamageWorker_Extinguish
{
    public static void Postfix(Thing victim)
    {
        if (victim is not Pawn pawn)
        {
            return;
        }

        if (pawn.health.hediffSet.GetFirstHediffOfDef(MYDE_ThingDefOf.MYDE_Hediff_WhitePhosphorus) != null)
        {
            pawn.health.hediffSet.GetFirstHediffOfDef(MYDE_ThingDefOf.MYDE_Hediff_WhitePhosphorus).Severity -= 1f;
        }
    }
}