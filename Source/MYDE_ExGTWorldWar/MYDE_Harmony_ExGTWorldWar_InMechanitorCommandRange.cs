using HarmonyLib;
using Verse;

namespace MYDE_ExGTWorldWar;

[HarmonyPatch(typeof(MechanitorUtility), "InMechanitorCommandRange")]
internal class MYDE_Harmony_ExGTWorldWar_InMechanitorCommandRange
{
    [HarmonyPriority(700)]
    private static void Postfix(ref bool __result, Pawn mech)
    {
        if (!__result && mech is DraftMachineRace)
        {
            __result = true;
        }
    }
}