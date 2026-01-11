using RimWorld;
using Verse;

namespace MYDE_ExGTWorldWar;

public class Comp_GiveDashGizmos_Equipment : CompEquippable
{
    private Ability ability;

    public CompProperties_GiveDashGizmos_Equipment Props => (CompProperties_GiveDashGizmos_Equipment)props;

    public Ability AbilityForReading => ability;

    public override void Initialize(CompProperties props)
    {
        base.Initialize(props);
        if (Holder == null)
        {
            return;
        }

        AbilityForReading?.pawn = Holder;
        AbilityForReading?.verb?.caster = Holder;
    }

    public override void Notify_Equipped(Pawn pawn)
    {
        if (pawn == null)
        {
            return;
        }

        AbilityForReading?.pawn = pawn;
        AbilityForReading?.verb?.caster = pawn;
        pawn.abilities?.Notify_TemporaryAbilitiesChanged();
    }

    public override void Notify_Unequipped(Pawn pawn)
    {
        pawn?.abilities?.Notify_TemporaryAbilitiesChanged();
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Deep.Look(ref ability, "ability");
        if (Scribe.mode != LoadSaveMode.PostLoadInit || Holder == null)
        {
            return;
        }

        AbilityForReading.pawn = Holder;
        AbilityForReading.verb.caster = Holder;
    }
}