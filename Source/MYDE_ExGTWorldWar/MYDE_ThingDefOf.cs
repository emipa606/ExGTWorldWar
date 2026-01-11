using RimWorld;
using Verse;

namespace MYDE_ExGTWorldWar;

[DefOf]
public static class MYDE_ThingDefOf
{
    public static ThingDef PawnJumper_Dash;

    public static HediffDef BurntTentacle;

    public static HediffDef MYDE_Hediff_WhitePhosphorus;

    public static ThingDef MYDE_WW_TheDead_A;

    public static ThingDef MYDE_WW_TheDead_B;

    public static ThingDef MYDE_WW_TheDead_C;

    public static ThingDef MYDE_WW_TheDead_D;

    public static ThingDef MYDE_WW_TheDead_E;

    public static PawnKindDef WW_TheDead;

    public static PawnKindDef WW_TheDeadFemale;

    public static PawnKindDef WW_TheDeadHulk;

    public static PawnKindDef WW_TheDeadMale;

    public static PawnKindDef WW_TheDeadThin;

    public static JobDef MYDE_ExGTWorldWar_Job_SelfHealing;

    static MYDE_ThingDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(MYDE_ThingDefOf));
    }
}