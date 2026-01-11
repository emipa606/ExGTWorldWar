using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace MYDE_ExGTWorldWar;

public class PawnDasher : Thing, IThingHolder
{
    private Thing carriedThing;

    protected IntVec3 destCell;

    private Vector3 effectivePos;

    protected float flightDistance;

    private Effecter flightEffecter;

    protected EffecterDef flightEffecterDef;

    private Vector3 groundPos;
    private ThingOwner<Thing> innerContainer;

    private JobQueue jobQueue;

    private bool pawnCanFireAtWill = true;

    private bool pawnWasDrafted;

    private int positionLastComputedTick = -1;

    protected SoundDef soundLanding;

    protected Vector3 startVec;

    private LocalTargetInfo target;

    protected int ticksFlightTime = 120;

    protected int ticksFlying;

    public Dictionary<FleckDef, int> trails = new Dictionary<FleckDef, int>();

    private AbilityDef triggeringAbility;

    public PawnDasher()
    {
        innerContainer = new ThingOwner<Thing>(this);
    }

    public virtual Thing FlyingThing =>
        innerContainer.InnerListForReading.Count <= 0 ? null : innerContainer.InnerListForReading[0];

    public virtual Pawn FlyingPawn => FlyingThing as Pawn;

    public virtual Thing CarriedThing => carriedThing;

    public override Vector3 DrawPos
    {
        get
        {
            RecomputePosition();
            return effectivePos;
        }
    }

    public Vector3 DestinationPos
    {
        get
        {
            var flyingThing = FlyingThing;
            return GenThing.TrueCenter(destCell, flyingThing.Rotation, flyingThing.def.size, flyingThing.def.Altitude);
        }
    }

    public void GetChildHolders(List<IThingHolder> outChildren)
    {
        ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
    }

    public ThingOwner GetDirectlyHeldThings()
    {
        return innerContainer;
    }

    private void RecomputePosition()
    {
        if (positionLastComputedTick == ticksFlying)
        {
            return;
        }

        positionLastComputedTick = ticksFlying;
        var t = ticksFlying / (float)ticksFlightTime;
        var t2 = def.pawnFlyer.Worker.AdjustedProgress(t);
        groundPos = Vector3.Lerp(startVec, DestinationPos, t2);
        effectivePos = groundPos;
        Position = groundPos.ToIntVec3();
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref startVec, "startVec");
        Scribe_Values.Look(ref destCell, "destCell");
        Scribe_Values.Look(ref flightDistance, "flightDistance");
        Scribe_Values.Look(ref pawnWasDrafted, "pawnWasDrafted");
        Scribe_Values.Look(ref pawnCanFireAtWill, "pawnCanFireAtWill", true);
        Scribe_Values.Look(ref ticksFlightTime, "ticksFlightTime");
        Scribe_Values.Look(ref ticksFlying, "ticksFlying");
        Scribe_Defs.Look(ref flightEffecterDef, "flightEffecterDef");
        Scribe_Defs.Look(ref soundLanding, "soundLanding");
        Scribe_Defs.Look(ref triggeringAbility, "triggeringAbility");
        Scribe_References.Look(ref carriedThing, "carriedThing");
        Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
        Scribe_Deep.Look(ref jobQueue, "jobQueue");
        Scribe_TargetInfo.Look(ref target, "target");
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        if (respawningAfterLoad)
        {
            return;
        }

        var a = Mathf.Max(flightDistance, 1f) / def.pawnFlyer.flightSpeed;
        a = Mathf.Max(a, def.pawnFlyer.flightDurationMin);
        ticksFlightTime = a.SecondsToTicks();
        ticksFlying = 0;
    }

    public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
    {
        flightEffecter?.Cleanup();
        base.Destroy(mode);
    }

    protected override void Tick()
    {
        if (flightEffecter == null && flightEffecterDef != null)
        {
            flightEffecter = flightEffecterDef.Spawn();
            flightEffecter.Trigger(this, TargetInfo.Invalid);
        }
        else
        {
            flightEffecter?.EffectTick(this, TargetInfo.Invalid);
        }

        if (ticksFlying >= ticksFlightTime)
        {
            RespawnPawn();
            Destroy();
        }
        else
        {
            if (ticksFlying % 5 == 0)
            {
                CheckDestination();
            }
        }

        ticksFlying++;
    }

    public override void DynamicDrawPhaseAt(DrawPhase phase, Vector3 drawLoc, bool flip = false)
    {
        RecomputePosition();
        if (FlyingPawn != null)
        {
            FlyingPawn.DynamicDrawPhaseAt(phase, effectivePos);
        }
        else
        {
            FlyingThing?.DynamicDrawPhaseAt(phase, effectivePos);
        }

        base.DynamicDrawPhaseAt(phase, drawLoc, flip);
    }

    protected override void DrawAt(Vector3 drawLoc, bool flip = false)
    {
        if (CarriedThing != null && FlyingPawn != null)
        {
            PawnRenderUtility.DrawCarriedThing(FlyingPawn, effectivePos, CarriedThing);
        }
    }

    private void CheckDestination()
    {
        if (JumpUtility.ValidJumpTarget(this, Map, destCell))
        {
            return;
        }

        var num = GenRadial.NumCellsInRadius(3.9f);
        for (var i = 0; i < num; i++)
        {
            var intVec = destCell + GenRadial.RadialPattern[i];
            if (!JumpUtility.ValidJumpTarget(this, Map, intVec))
            {
                continue;
            }

            destCell = intVec;
            break;
        }
    }

    public static PawnDasher MakeDasher(ThingDef dashingDef, Pawn pawn, IntVec3 destCell, EffecterDef flightEffecterDef,
        SoundDef landingSound, bool flyWithCarriedThing = false, Vector3? overrideStartVec = null,
        Ability triggeringAbility = null, LocalTargetInfo target = default)
    {
        var pawnDasher = (PawnDasher)ThingMaker.MakeThing(dashingDef);
        pawnDasher.startVec = overrideStartVec ?? pawn.TrueCenter();
        pawnDasher.flightDistance = pawn.Position.DistanceTo(destCell);
        pawnDasher.destCell = destCell;
        pawn.rotationTracker.FaceCell(destCell);
        var rotation = pawn.Rotation = pawn.Rotation;
        pawnDasher.Rotation = rotation;
        pawnDasher.pawnWasDrafted = pawn.Drafted;
        pawnDasher.flightEffecterDef = flightEffecterDef;
        pawnDasher.soundLanding = landingSound;
        pawnDasher.triggeringAbility = triggeringAbility?.def;
        pawnDasher.target = target;
        if (pawn.drafter != null)
        {
            pawnDasher.pawnCanFireAtWill = pawn.drafter.FireAtWill;
        }

        if (pawn.CurJob != null)
        {
            if (pawn.CurJob.def == JobDefOf.CastJump)
            {
                pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
            }
            else
            {
                pawn.jobs.SuspendCurrentJob(JobCondition.InterruptForced);
            }
        }

        pawnDasher.jobQueue = pawn.jobs.CaptureAndClearJobQueue();
        if (flyWithCarriedThing && pawn.carryTracker.CarriedThing != null &&
            pawn.carryTracker.TryDropCarriedThing(pawn.Position, ThingPlaceMode.Direct, out pawnDasher.carriedThing))
        {
            pawnDasher.carriedThing.holdingOwner?.Remove(pawnDasher.carriedThing);

            pawnDasher.carriedThing.DeSpawn();
        }

        if (pawn.Spawned)
        {
            pawn.DeSpawn(DestroyMode.WillReplace);
        }

        if (!pawnDasher.innerContainer.TryAdd(pawn))
        {
            Log.Error($"Could not add {pawn.ToStringSafe()} to a flyer.");
            pawn.Destroy();
        }

        if (pawnDasher.carriedThing != null && !pawnDasher.innerContainer.TryAdd(pawnDasher.carriedThing))
        {
            Log.Error($"Could not add {pawnDasher.carriedThing.ToStringSafe()} to a flyer.");
        }

        return pawnDasher;
    }

    protected virtual void RespawnPawn()
    {
        var flyingThing = FlyingThing;
        LandingEffects();
        innerContainer.TryDrop(flyingThing, destCell, flyingThing.MapHeld, ThingPlaceMode.Direct,
            out _, null, null, false);
        var pawn = flyingThing as Pawn;
        if (pawn?.drafter != null)
        {
            pawn.drafter.Drafted = pawnWasDrafted;
            pawn.drafter.FireAtWill = pawnCanFireAtWill;
        }

        flyingThing.Rotation = Rotation;
        if (carriedThing != null && innerContainer.TryDrop(carriedThing, destCell, flyingThing.MapHeld,
                ThingPlaceMode.Direct, out _, null, null, false) && pawn != null)
        {
            carriedThing.DeSpawn();
            if (!pawn.carryTracker.TryStartCarry(carriedThing))
            {
                Log.Error($"Could not carry {carriedThing.ToStringSafe()} after respawning flyer pawn.");
            }
        }

        if (pawn == null)
        {
            return;
        }

        if (jobQueue != null)
        {
            pawn.jobs.RestoreCapturedJobs(jobQueue);
        }

        pawn.jobs.CheckForJobOverride();
        if (def.pawnFlyer.stunDurationTicksRange != IntRange.Zero)
        {
            pawn.stances.stunner.StunFor(def.pawnFlyer.stunDurationTicksRange.RandomInRange, null, false, false);
        }

        if (triggeringAbility == null)
        {
            return;
        }

        var ability = pawn.abilities.GetAbility(triggeringAbility);
        if (ability?.comps == null)
        {
            return;
        }

        foreach (var comp in ability.comps)
        {
            if (comp is ICompAbilityEffectOnJumpCompleted compAbilityEffectOnJumpCompleted)
            {
                compAbilityEffectOnJumpCompleted.OnJumpCompleted(startVec.ToIntVec3(), target);
            }
        }
    }

    private void LandingEffects()
    {
        soundLanding?.PlayOneShot(new TargetInfo(Position, Map));
        FleckMaker.ThrowDustPuff(DestinationPos + Gen.RandomHorizontalVector(0.5f), Map, 2f);
    }
}