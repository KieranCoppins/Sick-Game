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
        StartCoroutine(Think());
    }

    protected override void Update()
    {
        base.Update();
    }

    // Instead of trying to schedule an action every frame, lets do it every second or something
    IEnumerator Think()
    {
        while (true)
        {
            // Constantly try to determine what we should be doing
            Action actionToBeScheduled = decisionTree.Run();
            actionManager.ScheduleAction(actionToBeScheduled);
            actionManager.Execute();
            yield return new WaitForSeconds(0.1f);
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
        A_MoveTo MoveToPlayer = new (mob, FindTileNearPlayer);   // We want to move in slightly more than what our ability allows
        A_Attack castComet = new(mob, player, ((RangedMob)mob).ability);
        A_Idle idle = new(mob);


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

    Vector2 FindTileNearPlayer()
    {
        Transform target = GameObject.FindGameObjectWithTag("Player").transform;
        float range = ((RangedMob)mob).ability.ability.Range;
        Vector2 position = range == 0 ? target.position : GameObject.FindGameObjectWithTag("EQSManager").GetComponent<EQSManager>().RunEQSystem(EQSystem.RangedMobMoveToPlayer, range, target.position, mob.gameObject);
        return position;
    }
}
