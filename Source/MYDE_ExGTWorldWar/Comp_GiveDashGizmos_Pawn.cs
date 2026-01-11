using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace MYDE_ExGTWorldWar;

public class Comp_GiveDashGizmos_Pawn : ThingComp
{
    public readonly int ColdDownTickMax = 15000;

    public readonly float Range = 26f;
    public int ColdDownTick;

    public string Icon;

    public string Label;

    public CompProperties_GiveDashGizmos_Pawn Props => props as CompProperties_GiveDashGizmos_Pawn;

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref ColdDownTick, "ColdDownTick");
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        foreach (var item in base.CompGetGizmosExtra())
        {
            yield return item;
        }

        if (parent is not Pawn { IsPlayerControlled: true } pawn)
        {
            yield break;
        }

        var thingWithComps2 = pawn.equipment.Primary;
        if (thingWithComps2 == null)
        {
            yield break;
        }

        foreach (var comp in thingWithComps2.GetComps<Comp_GiveDashGizmos_Equipment>())
        {
            _ = comp;
            yield return new Command_Action
            {
                Disabled = ColdDownTick < ColdDownTickMax,
                defaultLabel = Label,
                defaultDesc = "MYDE_ExGTWorldWar_Dash_Desc".Translate(),
                icon = ContentFinder<Texture2D>.Get(Icon),
                action = doSomething
            };
            if (DebugSettings.ShowDevGizmos)
            {
                yield return new Command_Action
                {
                    defaultLabel = "Max",
                    action = delegate { ColdDownTick = ColdDownTickMax; }
                };
            }
        }
    }

    private void doSomething()
    {
        if (ColdDownTick < ColdDownTickMax)
        {
            return;
        }

        var Pawn = parent as Pawn;
        var Map = Pawn?.Map;
        var targetParams = new TargetingParameters
        {
            canTargetLocations = true,
            validator = target => target.IsValid && !target.Cell.Fogged(Map) && target.Cell.InBounds(Map)
        };

        Find.Targeter.BeginTargeting(targetParams, action, highlightAction, targetValidator);
        return;

        void highlightAction(LocalTargetInfo Target)
        {
            if (Pawn == null)
            {
                return;
            }

            var position = Pawn.Position;
            var cell = Target.Cell;
            var list = new List<IntVec3>();
            foreach (var item in MYDE_ModFront.GetLine(position, cell))
            {
                if (!position.InHorDistOf(item, Range) || !item.InBounds(Pawn.Map) || !item.Walkable(Map) ||
                    !GenSight.LineOfSight(position, item, Map, false))
                {
                    break;
                }

                list.Add(item);
            }

            GenDraw.DrawFieldEdges(list, Color.white);
            GenDraw.DrawRadiusRing(position, Range);
        }

        bool targetValidator(LocalTargetInfo Target)
        {
            if (Pawn == null)
            {
                return true;
            }

            var position = Pawn.Position;
            var cell = Target.Cell;
            if (position == cell)
            {
                return false;
            }

            if (!parent.Position.InHorDistOf(cell, Range))
            {
                return false;
            }

            if (!GenSight.LineOfSight(position, cell, Map, false))
            {
                return false;
            }

            foreach (var item2 in MYDE_ModFront.GetLine(position, cell))
            {
                if (!position.InHorDistOf(item2, Range))
                {
                    return false;
                }

                if (!item2.InBounds(Pawn.Map))
                {
                    return false;
                }

                if (!item2.Walkable(Map))
                {
                    return false;
                }
            }

            return true;
        }

        void action(LocalTargetInfo tar)
        {
            var cell = tar.Cell;
            ColdDownTick = 0;
            var isSelected = Find.Selector.IsSelected(Pawn);
            var pawnDasher = PawnDasher.MakeDasher(MYDE_ThingDefOf.PawnJumper_Dash, Pawn, cell, null, null);
            if (pawnDasher == null)
            {
                return;
            }

            GenSpawn.Spawn(pawnDasher, cell, Map);
            if (isSelected)
            {
                Find.Selector.Select(pawnDasher, false, false);
            }
        }
    }

    public override void CompTick()
    {
        base.CompTick();
        if (ColdDownTick < ColdDownTickMax)
        {
            ColdDownTick++;
            Label = (ColdDownTickMax - ColdDownTick).ToStringSecondsFromTicks();
            Icon = "UI/Charge_B";
        }
        else
        {
            Label = "MYDE_ExGTWorldWar_Dash_Label".Translate();
            Icon = "UI/Charge_A";
        }
    }
}