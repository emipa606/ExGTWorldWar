using System.Reflection;
using HarmonyLib;
using Verse;

namespace DwarfGekko_PathForCE;

[StaticConstructorOnStartup]
public class PatchMain
{
    static PatchMain()
    {
        new Harmony("DwarfGekko_PathForCE_By3H").PatchAll(Assembly.GetExecutingAssembly());
    }
}