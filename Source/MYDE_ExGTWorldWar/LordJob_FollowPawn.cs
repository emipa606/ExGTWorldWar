using RimWorld;
using Verse;
using Verse.AI.Group;

namespace MYDE_ExGTWorldWar;

public class LordJob_FollowPawn : LordJob
{
    public Pawn escortee;

    private Faction escorteeFaction;

    public string leavingDangerMessage;

    public Thing shuttle;

    public LordJob_FollowPawn()
    {
    }

    public LordJob_FollowPawn(Pawn escortee, Thing shuttle = null)
    {
        this.escortee = escortee;
        this.shuttle = shuttle;
        escorteeFaction = escortee.Faction;
    }

    public override bool AlwaysShowWeapon => true;

    public override StateGraph CreateGraph()
    {
        var stateGraph = new StateGraph();
        _ = shuttle.TryGetComp<CompShuttle>();
        var lordToil_EscortPawn = new LordToil_EscortPawn(escortee);
        stateGraph.AddToil(lordToil_EscortPawn);
        var lordToil = shuttle == null ? new LordToil_ExitMap() : (LordToil)new LordToil_EnterShuttleOrLeave(shuttle);
        stateGraph.AddToil(lordToil);
        var transition = new Transition(lordToil_EscortPawn, lordToil);
        var trigger = new Trigger_Custom(signal => signal.type == TriggerSignalType.Tick && escortee.Dead);
        transition.AddTrigger(trigger);
        stateGraph.AddTransition(transition);
        var transition2 = new Transition(lordToil_EscortPawn, lordToil);
        var trigger2 = new Trigger_Custom(signal =>
            signal.type == TriggerSignalType.Tick && (escortee.MapHeld != lord.Map || shuttle != null &&
                escortee.ParentHolder == shuttle.TryGetComp<CompTransporter>() &&
                shuttle.TryGetComp<CompShuttle>().shipParent.Waiting));
        transition2.AddTrigger(trigger2);
        stateGraph.AddTransition(transition2);
        var transition3 = new Transition(lordToil_EscortPawn, lordToil);
        transition3.AddTrigger(new Trigger_BecamePlayerEnemy());
        stateGraph.AddTransition(transition3);
        var transition4 = new Transition(lordToil_EscortPawn, lordToil);
        transition4.AddTrigger(new Trigger_Custom(signal =>
            signal.type == TriggerSignalType.Tick && escortee.Faction != escorteeFaction));
        stateGraph.AddTransition(transition4);
        return stateGraph;
    }

    public override void ExposeData()
    {
        Scribe_References.Look(ref escortee, "escortee");
        Scribe_References.Look(ref shuttle, "shuttle");
        Scribe_References.Look(ref escorteeFaction, "escorteeFaction");
        Scribe_Values.Look(ref leavingDangerMessage, "leavingDangerMessage");
    }
}