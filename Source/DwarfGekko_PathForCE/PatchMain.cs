using System.Reflection;
using HarmonyLib;
using Verse;

namespace DwarfGekko_PathForCE;

[StaticConstructorOnStartup]
public static class PatchMain
{
    static PatchMain()
    {
        new Harmony("DwarfGekko_PathForCE_By3H").PatchAll(Assembly.GetExecutingAssembly());
    }
}