using System.Reflection;
using HarmonyLib;
using Verse;

namespace MYDE_ExGTWorldWar;

[StaticConstructorOnStartup]
public class MYDE_ExGTWorldWar_Patch
{
    static MYDE_ExGTWorldWar_Patch()
    {
        new Harmony("MYDE.ExGTWorldWar").PatchAll(Assembly.GetExecutingAssembly());
    }
}