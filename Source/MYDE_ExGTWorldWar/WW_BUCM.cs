using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace MYDE_ExGTWorldWar;

public class WW_BUCM : Apparel
{
    private readonly List<ThingDef> DefsOfTheDeads =
    [
        MYDE_ThingDefOf.MYDE_WW_TheDead_A,
        MYDE_ThingDefOf.MYDE_WW_TheDead_B,
        MYDE_ThingDefOf.MYDE_WW_TheDead_C,
        MYDE_ThingDefOf.MYDE_WW_TheDead_D,
        MYDE_ThingDefOf.MYDE_WW_TheDead_E
    ];

    public readonly int GiveHediffTickMax = 3600;

    public readonly int Spawn_CDTickMax = 300000;

    public readonly int Tend_CDTickMax = 60000;
    public int GiveHediffTick;

    public List<Pawn> ListFivePawn = [];

    public int Spawn_CDTick;

    public string Spawn_Icon;

    public string Spawn_Label;

    public int Tend_CDTick;

    public string Tend_Icon;

    public string Tend_Label;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref Tend_CDTick, "Tend_CDTick");
        Scribe_Values.Look(ref Spawn_CDTick, "Spawn_CDTick");
        Scribe_Collections.Look(ref ListFivePawn, "ListFivePawn", LookMode.Reference);
    }

    public override IEnumerable<Gizmo> GetWornGizmos()
    {
        foreach (var comp in AllComps)
        {
            foreach (var item in comp.CompGetWornGizmosExtra())
            {
                yield return item;
            }
        }

        if (Wearer != null)
        {
            yield return new Command_Action
            {
                Disabled = Tend_CDTick < Tend_CDTickMax,
                defaultLabel = Tend_Label,
                defaultDesc = "MYDE_ExGTWorldWar_Tend_Desc".Translate(),
                icon = ContentFinder<Texture2D>.Get(Tend_Icon),
                action = DoSomething_Tend
            };
            yield return new Command_Action
            {
                Disabled = Spawn_CDTick < Spawn_CDTickMax,
                defaultLabel = Spawn_Label,
                defaultDesc = "MYDE_ExGTWorldWar_Spawn_Desc".Translate(),
                icon = ContentFinder<Texture2D>.Get(Spawn_Icon),
                action = DoSomething_Spawn
            };
        }

        if (DebugSettings.ShowDevGizmos)
        {
            yield return new Command_Action
            {
                defaultLabel = "Max",
                action = delegate
                {
                    Tend_CDTick = Tend_CDTickMax;
                    Spawn_CDTick = Spawn_CDTickMax;
                }
            };
        }
    }

    private void DoSomething_Tend()
    {
        if (Tend_CDTick < Tend_CDTickMax)
        {
            return;
        }

        Tend_CDTick = 0;
        var hediffs = Wearer.health.hediffSet.hediffs;
        var list = new List<Hediff>();
        foreach (var hediff in hediffs)
        {
            if (hediff.TendableNow() && (hediff is Hediff_Injury || hediff is Hediff_MissingPart))
            {
                list.Add(hediff);
            }
        }

        if (list.Count <= 0)
        {
            return;
        }

        // ReSharper disable once ForCanBeConvertedToForeach
        for (var j = 0; j < list.Count; j++)
        {
            var hediff = list[j];
            hediff.Tended(0.05f, 0.05f);
        }
    }

    private void DoSomething_Spawn()
    {
        var dead = true;
        for (var i = 0; i < ListFivePawn.Count; i++)
        {
            if (!ListFivePawn[i].Spawned)
            {
                ListFivePawn.Remove(ListFivePawn[i]);
            }

            if (!ListFivePawn[i].Dead)
            {
                dead = false;
            }
        }

        if (!dead)
        {
            return;
        }

        ListFivePawn.Clear();
        if (Spawn_CDTick < Spawn_CDTickMax)
        {
            return;
        }

        Spawn_CDTick = 0;
        var map = Wearer.Map;
        var position = Wearer.Position;
        var list = new List<IntVec3>();
        var num = 3;
        for (var j = -num; j <= num; j++)
        {
            for (var k = -num; k <= num; k++)
            {
                var item = position + new IntVec3(j, 0, k);
                list.Add(item);
            }
        }

        if (Wearer == null)
        {
            return;
        }

        foreach (var defsOfTheDead in DefsOfTheDeads)
        {
            var loc = list.RandomElement();
            var theDead = ThingMaker.MakeThing(defsOfTheDead) as TheDead;
            theDead?.CasterPawn = Wearer;
            GenSpawn.Spawn(theDead, loc, map);
        }
    }

    protected override void Tick()
    {
        base.Tick();
        if (Wearer != null)
        {
            GiveHediffTick++;
            if (GiveHediffTick >= GiveHediffTickMax)
            {
                GiveHediffTick = 0;
                HealthUtility.AdjustSeverity(Wearer, MYDE_ThingDefOf.BurntTentacle, 1f);
            }
        }

        if (Tend_CDTick < Tend_CDTickMax)
        {
            Tend_CDTick++;
            Tend_Label = (Tend_CDTickMax - Tend_CDTick).ToStringSecondsFromTicks();
            Tend_Icon = "UI/Tend_B";
        }
        else
        {
            Tend_Label = "MYDE_ExGTWorldWar_Tend_Label".Translate();
            Tend_Icon = "UI/Tend_A";
        }

        if (Spawn_CDTick < Spawn_CDTickMax)
        {
            Spawn_CDTick++;
            Spawn_Label = (Spawn_CDTickMax - Spawn_CDTick).ToStringSecondsFromTicks();
            Spawn_Icon = "UI/Spawn_B";
        }
        else
        {
            Spawn_Label = "MYDE_ExGTWorldWar_Spawn_Label".Translate();
            Spawn_Icon = "UI/Spawn_A";
        }
    }
}