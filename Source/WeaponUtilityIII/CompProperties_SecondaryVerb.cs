using Verse;

namespace WeaponUtilityIII;

internal class CompProperties_SecondaryVerb : CompProperties
{
    public readonly string description = "";

    public readonly string mainCommandIcon = "";

    public readonly string mainWeaponLabel = "";

    public readonly string secondaryCommandIcon = "";

    public readonly string secondaryWeaponLabel = "";
    public readonly VerbProperties verbProps = new VerbProperties();

    public CompProperties_SecondaryVerb()
    {
        compClass = typeof(CompSecondaryVerb);
    }
}