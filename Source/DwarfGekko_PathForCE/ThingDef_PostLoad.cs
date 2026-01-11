using System.Linq;
using HarmonyLib;
using Verse;

namespace DwarfGekko_PathForCE;

[HarmonyPatch(typeof(ThingDef), "PostLoad")]
internal static class ThingDef_PostLoad
{
    public static void Prefix(ThingDef __instance)
    {
        var ext = __instance.GetModExtension<DefModExtension_RemoveComps>();
        if (__instance.comps != null && ext != null)
        {
            __instance.comps = __instance.comps.Where(c => !ext.compProps.Contains(c.GetType())).ToList();
        }
    }
}