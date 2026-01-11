using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace MYDE_ExGTWorldWar;

[HarmonyPatch]
internal class FloatMenuOptionProvider_GetOptions
{
    private static readonly PropertyInfo MechanoidCanDoInfo =
        AccessTools.Property(typeof(FloatMenuOptionProvider), "MechanoidCanDo");

    public static IEnumerable<MethodBase> TargetMethods()
    {
        // Get all classes that derive from FloatMenuOptionProvider
        var allSubClasses = typeof(FloatMenuOptionProvider).AllSubclassesNonAbstract();
        foreach (var subClass in allSubClasses)
        {
            // Parse all public methods
            var methods = subClass.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.Name is "GetOptionsFor" or "GetSingleOptionFor");
            var methodInfos = methods.ToList();
            if (!methodInfos.Any())
            {
                continue;
            }

            foreach (var method in methodInfos)
            {
                yield return method;
            }
        }
    }

    public static void Prefix(ref FloatMenuOptionProvider __instance, out bool __state, FloatMenuContext context)
    {
        __state = MechanoidCanDoInfo.GetValue(__instance) as bool? ?? false;
        if (__state || context.FirstSelectedPawn is not DraftMachineRace { IsPlayerControlled: true })
        {
            return;
        }

        MechanoidCanDoInfo.SetValue(__instance, true);
    }

    public static void Postfix(ref FloatMenuOptionProvider __instance, bool __state, FloatMenuContext context)
    {
        if (__state || context.FirstSelectedPawn is not DraftMachineRace { IsPlayerControlled: true })
        {
            return;
        }

        MechanoidCanDoInfo.SetValue(__instance, false);
    }
}