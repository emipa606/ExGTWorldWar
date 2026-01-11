using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace WeaponUtilityIII;

internal class CompSecondaryVerb : ThingComp
{
    private bool isSecondaryVerbSelected;

    public CompProperties_SecondaryVerb Props => (CompProperties_SecondaryVerb)props;

    public bool IsSecondaryVerbSelected => isSecondaryVerbSelected;

    private CompEquippable EquipmentSource
    {
        get
        {
            if (field != null)
            {
                return field;
            }

            field = parent.TryGetComp<CompEquippable>();
            if (field == null)
            {
                Log.ErrorOnce($"{parent.LabelCap} has CompSecondaryVerb but no CompEquippable", 50020);
            }

            return field;
        }
    }

    public Pawn CasterPawn => Verb.caster as Pawn;

    private Verb Verb
    {
        get
        {
            field ??= EquipmentSource.PrimaryVerb;

            return field;
        }
    }

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

        if (CasterPawn != null && !CasterPawn.Faction.Equals(Faction.OfPlayer))
        {
            yield break;
        }

        var text = IsSecondaryVerbSelected ? Props.secondaryCommandIcon : Props.mainCommandIcon;
        if (text == "")
        {
            text = "UI/Buttons/Reload";
        }

        yield return new Command_Action
        {
            action = SwitchVerb,
            defaultLabel = IsSecondaryVerbSelected ? Props.secondaryWeaponLabel : Props.mainWeaponLabel,
            defaultDesc = Props.description,
            icon = ContentFinder<Texture2D>.Get(text, false)
        };
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref isSecondaryVerbSelected, "PLA_useSecondaryVerb");
        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            PostAmmoDataLoaded();
        }
    }

    private void SwitchVerb()
    {
        if (!IsSecondaryVerbSelected)
        {
            EquipmentSource.PrimaryVerb.verbProps = Props.verbProps;
            isSecondaryVerbSelected = true;
        }
        else
        {
            EquipmentSource.PrimaryVerb.verbProps = parent.def.Verbs[0];
            isSecondaryVerbSelected = false;
        }
    }

    private void PostAmmoDataLoaded()
    {
        if (isSecondaryVerbSelected)
        {
            EquipmentSource.PrimaryVerb.verbProps = Props.verbProps;
        }
    }
}