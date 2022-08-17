using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedMob : BaseMob
{
    [Header("Ranged Mob Attributes")]
    [SerializeField] public AIAbility ability;

    protected override void Start()
    {
        base.Start();
        decisionTree = new DT_RangedMob(this);
        decisionTree.Initialise();
    }

    protected override void Update()
    {
        base.Update();

        // Constantly try to determine what we should be doing
        Action actionToBeScheduled = decisionTree.Run();

        // If our action is an interruptor or we're not doing anything, queue our action
        if (actionToBeScheduled.Interruptor || !actionManager.executingActions)
        {
            actionManager.ScheduleAction(actionToBeScheduled);
            actionManager.Execute();
        }
    }
}

public class DT_RangedMob : DecisionTree
{
    Vector2 playerPrevPos;
    public DT_RangedMob(RangedMob mob) : base(mob)
    {

    }
    public override void Initialise()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        // Initialise all our Nodes

        /// ACTIONS
        A_MoveTo MoveToPlayer = new (mob, player, ((RangedMob)mob).ability.ability.Range - 2.0f);   // We want to move in slightly more than what our ability allows
        A_Idle idle = new (mob);
        A_Attack castComet = new(mob, player, ((RangedMob)mob).ability);


        /// DECISIONS
        AttackDecision shouldCastComet = new(castComet, idle, mob, ((RangedMob)mob).ability);

        // Initialise our root
        root = new Decision(MoveToPlayer, shouldCastComet, ShouldMoveToPlayer, mob);
    }

    bool ShouldMoveToPlayer()
    {
        Vector2 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        if (Vector2.Distance(mob.transform.position, playerPos) > ((RangedMob)mob).ability.ability.Range || !mob.HasLineOfSight(playerPos))
        {
            if (playerPrevPos == null || Vector2.Distance(playerPrevPos, playerPos) > 2.0f)
            {
                playerPrevPos = playerPos;
                return true;
            }
        }
        return false;
    }
}
