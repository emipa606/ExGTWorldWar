using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MYDE_ExGTWorldWar;

public class Comp_GiveExtraVerbGizmos_Equipment : ThingComp
{
    public Pawn Pawn;

    public CompProperties_GiveExtraVerbGizmos_Equipment Props => props as CompProperties_GiveExtraVerbGizmos_Equipment;

    public Verb AttackVerb => Pawn.verbTracker.PrimaryVerb;

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        foreach (var gizmo in base.CompGetGizmosExtra())
        {
            yield return gizmo;
        }

        if (!Pawn.IsPlayerControlled)
        {
            yield break;
        }

        yield return new Command_VerbTarget
        {
            defaultLabel = "Test",
            defaultDesc = "Test",
            icon = ContentFinder<Texture2D>.Get("UI/Commands/Attack"),
            verb = AttackVerb
        };
    }
}