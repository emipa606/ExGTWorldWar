using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace MYDE_ExGTWorldWar;

public class TheDead : ThingWithComps
{
    public readonly List<ThingDef> FilthTypes = [];

    public readonly int FleckTickMax = 20;

    public readonly List<Thing> SpawnedPawns = [];
    public Pawn CasterPawn;

    public int FleckTick;
    public int SpawnTick;

    public int SpawnTickMax = 1200;

    public Sustainer Sustainer;

    public PawnKindDef PawnToSpawn => def.GetModExtension<TheDead_DefModExtension>().pawnKindToSpawn;

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        FilthTypes.Add(ThingDefOf.Filth_Dirt);
        FilthTypes.Add(ThingDefOf.Filth_Dirt);
        FilthTypes.Add(ThingDefOf.Filth_Dirt);
        FilthTypes.Add(ThingDefOf.Filth_RubbleRock);
        CreateSustainer();
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref SpawnTick, "SpawnTick");
        Scribe_Values.Look(ref SpawnTickMax, "SpawnTickMax");
        Scribe_References.Look(ref CasterPawn, "CasterPawn");
    }

    public override void PostMake()
    {
        base.PostMake();
        SpawnTickMax += Rand.Range(360, 600);
    }

    protected override void Tick()
    {
        if (!Spawned)
        {
            return;
        }

        Sustainer.Maintain();
        var position = Position;
        var loc = position.ToVector3Shifted();
        var map = Map;
        FleckTick++;
        if (FleckTick >= FleckTickMax)
        {
            FleckTick = 0;
            FilthMaker.TryMakeFilth(position, map, FilthTypes.RandomElement());
            FleckMaker.ThrowDustPuffThick(loc, Map, Rand.Range(1.5f, 3f), new Color(1f, 1f, 1f, 2.5f));
        }

        SpawnTick++;
        if (SpawnTick < SpawnTickMax)
        {
            return;
        }

        SpawnTick = 0;
        Sustainer.End();
        DoSomething();
        Destroy();
    }

    public void DoSomething()
    {
        var map = Map;
        var position = Position;
        var ofPlayer = Faction.OfPlayer;
        var draftMachineRace =
            PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnToSpawn, ofPlayer,
                PawnGenerationContext.PlayerStarter, -1)) as DraftMachineRace;
        SpawnedPawns.Add(draftMachineRace);
        draftMachineRace?.FollowPawn = CasterPawn;
        GenSpawn.Spawn(draftMachineRace, position, map).SetFaction(ofPlayer);
        var lordJob = new LordJob_FollowPawn(CasterPawn);
        LordMaker.MakeNewLord(Faction.OfPlayer, lordJob, map).AddPawn(draftMachineRace);
        var wornApparel = CasterPawn.apparel.WornApparel;
        foreach (var apparel in wornApparel)
        {
            if (apparel is not WW_BUCM wWBucm)
            {
                continue;
            }

            wWBucm.ListFivePawn.Add(draftMachineRace);
        }
    }

    protected override void DrawAt(Vector3 drawLoc, bool flip = false)
    {
        base.DrawAt(drawLoc, flip);
        Rand.PushState();
        Rand.Seed = thingIDNumber;
        for (var i = 0; i < 6; i++)
        {
            DrawDustPart(Rand.Range(0f, 360f), Rand.Range(0.9f, 1.1f) * Rand.Sign * 4f, Rand.Range(1f, 1.5f));
        }

        Rand.PopState();
    }

    private void DrawDustPart(float initialAngle, float speedMultiplier, float scale)
    {
        var num = (SpawnTickMax - SpawnTick).TicksToSeconds();
        var pos = Position.ToVector3ShiftedWithAltitude(AltitudeLayer.FloorCoverings);
        pos.y += 3f / 74f * Rand.Range(0f, 1f);
        var matrix = Matrix4x4.TRS(pos, Quaternion.Euler(0f, initialAngle + (speedMultiplier * num), 0f),
            Vector3.one * scale);
        var material = MaterialPool.MatFrom(color: new Color(0.47058824f, 0.38431373f, 0.3254902f, 0.7f),
            texPath: "Things/Mote/DustPuff", shader: ShaderDatabase.Transparent);
        Graphics.DrawMesh(MeshPool.plane10, matrix, material, 0);
    }

    private void CreateSustainer()
    {
        LongEventHandler.ExecuteWhenFinished(delegate
        {
            var tunnel = SoundDefOf.Tunnel;
            Sustainer = tunnel.TrySpawnSustainer(SoundInfo.InMap(this, MaintenanceType.PerTick));
        });
    }
}