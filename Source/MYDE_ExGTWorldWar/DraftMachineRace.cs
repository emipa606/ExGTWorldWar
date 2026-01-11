using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace MYDE_ExGTWorldWar;

public class DraftMachineRace : Pawn
{
    public readonly int Check_DownTickMax = 1200;

    public readonly int CheckLordTickMax = 18000;
    public int Check_DownTick;

    public int CheckLordTick;
    public Pawn FollowPawn;

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        drafter ??= new Pawn_DraftController(this);
    }

    public override void PostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
    {
        base.PostApplyDamage(dinfo, totalDamageDealt);
        if (Dead || CurJob.def != MYDE_ThingDefOf.MYDE_ExGTWorldWar_Job_SelfHealing)
        {
            return;
        }

        records.Increment(RecordDefOf.ThingsRepaired);
        jobs.EndCurrentJob(JobCondition.Succeeded);
    }

    public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
    {
        var wornApparel = FollowPawn?.apparel.WornApparel;
        // ReSharper disable once ForCanBeConvertedToForeach
        if (wornApparel != null)
        {
            for (var i = 0; i < wornApparel.Count; i++)
            {
                if (wornApparel[i] is not WW_BUCM bucm)
                {
                    continue;
                }

                bucm.ListFivePawn?.Remove(this);
            }
        }

        base.Destroy(mode);
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        if (!Spawned)
        {
            yield break;
        }

        foreach (var gizmo in base.GetGizmos())
        {
            yield return gizmo;
        }

        foreach (var thingComp in AllComps)
        {
            foreach (var item in thingComp.CompGetGizmosExtra())
            {
                yield return item;
            }
        }

        if (!IsPlayerControlled)
        {
            yield break;
        }

        var command_Toggle = new Command_Toggle
        {
            hotKey = KeyBindingDefOf.Command_ColonistDraft,
            isActive = () => Drafted,
            toggleAction = delegate
            {
                drafter.Drafted = !drafter.Drafted;
                PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.Drafting,
                    KnowledgeAmount.SpecificInteraction);
                if (Drafted)
                {
                    LessonAutoActivator.TeachOpportunity(ConceptDefOf.QueueOrders, OpportunityType.GoodToKnow);
                }
            },
            defaultDesc = "CommandToggleDraftDesc".Translate(),
            icon = TexCommand.Draft,
            turnOnSound = SoundDefOf.DraftOn,
            turnOffSound = SoundDefOf.DraftOff,
            groupKey = 81729172,
            defaultLabel = (Drafted ? "CommandUndraftLabel" : "CommandDraftLabel").Translate()
        };
        if (Downed)
        {
            command_Toggle.Disable("IsIncapped".Translate(LabelShort, this));
        }

        command_Toggle.tutorTag = !Drafted ? "Draft" : "Undraft";

        yield return command_Toggle;
        if (Drafted && equipment.Primary != null && equipment.Primary.def.IsRangedWeapon)
        {
            yield return new Command_Toggle
            {
                hotKey = KeyBindingDefOf.Misc6,
                isActive = () => drafter.FireAtWill,
                toggleAction = delegate { drafter.FireAtWill = !drafter.FireAtWill; },
                icon = TexCommand.FireAtWill,
                defaultLabel = "CommandFireAtWillLabel".Translate(),
                defaultDesc = "CommandFireAtWillDesc".Translate(),
                tutorTag = "FireAtWillToggle"
            };
        }

        if (Spawned)
        {
            yield return new Command_Action
            {
                action = DoSomething,
                defaultLabel = "MYDE_ExGTWorldWar_Heal_Label".Translate(),
                defaultDesc = "MYDE_ExGTWorldWar_Heal_Desc".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Tend_A")
            };
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref FollowPawn, "FollowPawn");
    }

    protected override void Tick()
    {
        base.Tick();
        CheckLordTick++;
        if (CheckLordTick >= CheckLordTickMax)
        {
            CheckLordTick = 0;
            if (FollowPawn != null && this.GetLord() == null)
            {
                var lordJob = new LordJob_FollowPawn(FollowPawn);
                LordMaker.MakeNewLord(Faction.OfPlayer, lordJob, Map).AddPawn(this);
            }
        }

        Check_DownTick++;
        if (Check_DownTick < Check_DownTickMax)
        {
            return;
        }

        Check_DownTick = 0;
        if (!Downed)
        {
            return;
        }

        for (var i = 0; i < 10; i++)
        {
            if (Dead)
            {
                continue;
            }

            var brain = health.hediffSet.GetBrain();
            var dinfo = new DamageInfo(DamageDefOf.Cut, 100f, 100f, -1f, null, brain);
            TakeDamage(dinfo);
        }
    }

    private void DoSomething()
    {
        var job = JobMaker.MakeJob(MYDE_ThingDefOf.MYDE_ExGTWorldWar_Job_SelfHealing, this);
        job.count = 1;
        jobs.TryTakeOrderedJob(job, JobTag.Misc);
    }
}