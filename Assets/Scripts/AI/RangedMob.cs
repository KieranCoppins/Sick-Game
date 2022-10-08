using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedMob : BaseMob
{
    [Header("Ranged Mob Attributes")]
    [SerializeField] public AbilityBase ability;

    protected DecisionTree<RangedMob> decisionTree;

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

    // Instead of trying to schedule an action every frame, lets do it every 100ms
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

    protected override float MoveAround(Vector2 targetDir, Vector2 dir, Vector2 target, bool moveStraight)
    {
        float dist = Vector2.Distance(target, transform.position);

        if (moveStraight)
            return Vector2.Dot(targetDir, dir);

        // Move away from the target if too close
        if (dist < 2.0f)
            return ((Vector2.Dot(targetDir, dir) * -1) - 0.4f) + Vector2.Dot(rb.velocity.normalized, dir);  // We add the dot product of our current velocity so that we try and favor where we are currently going - prevents random switches in direction

        // Circle the target if in range
        else if (dist < ability.Range - 1.0f)
            return 1.0f - Mathf.Abs(Vector2.Dot(targetDir, dir)) + Vector2.Dot(rb.velocity.normalized, dir);

        // Otherwise move towards the target
        return Vector2.Dot(targetDir, dir) + Vector2.Dot(rb.velocity.normalized, dir);
    }

    protected override float AvoidObsticle(Vector2 targetDir, Vector2 dir)
    {
        return 1.0f - Mathf.Abs(Vector2.Dot(targetDir, dir) - 0.65f) + Vector2.Dot(rb.velocity.normalized, dir);
    }
}

public class DT_RangedMob : DecisionTree<RangedMob>
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
        A_PathTo MoveToPlayer = new (mob, FindTileNearPlayer, CancelPathfinding);
        A_CastAbility castComet = new(mob, player, mob.ability);
        A_MoveTowards moveTowardsPlayer = new(mob, player, 0); // Always move towards the player (since moving towards for a ranged mob is actually circling them)


        /// DECISIONS
        AttackDecision shouldCastComet = new(castComet, moveTowardsPlayer, mob, player, mob.ability.Range);

        // Initialise our root
        root = new Decision(MoveToPlayer, shouldCastComet, ShouldMoveToPlayer, mob);
    }

    bool CancelPathfinding()
    {
        Vector2 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        return Vector2.Distance(mob.transform.position, playerPos) <= mob.ability.Range - 1.0f && mob.HasLineOfSight(playerPos);
    }

    bool ShouldMoveToPlayer()
    {
        Vector2 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        // We want to move to a distance slightly less than our abilities range so we're not *just* in range
        if (Vector2.Distance(mob.transform.position, playerPos) > mob.ability.Range - 1.0f || !mob.HasLineOfSight(playerPos))
        {
            if (playerPrevPos == null || Vector2.Distance(playerPrevPos, playerPos) > 0.5f)
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
        float range = mob.ability.Range;
        Vector2 position = range == 0 ? target.position : GameObject.FindGameObjectWithTag("EQSManager").GetComponent<EQSManager>().RunEQSystem(EQSystem.RangedMobMoveToPlayer, range, target.position, mob.gameObject);
        return position;
    }
}
