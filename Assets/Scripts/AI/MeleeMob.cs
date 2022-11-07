using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeMob : BaseMob
{
    public float MeleeRange { get { return _meleeRange; } protected set { _meleeRange = value; } }
    public float MeleeDamage { get { return _meleeDamage; } protected set { _meleeDamage = value; } }
    public float MeleeSpeed { get { return _meleeSpeed; } protected set { _meleeSpeed = value; } }

    [Header("Melee Mob Attributes")]
    [SerializeField] float _meleeRange;
    [SerializeField] float _meleeDamage;
    [SerializeField] float _meleeSpeed;

    protected DecisionTreeGeneric<MeleeMob> decisionTree;

    protected override void Start()
    {
        base.Start();
        decisionTree = new DT_MeleeMob(this);
        decisionTree.Initialise(this);
        StartCoroutine(Think());
    }

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

    protected override void Update()
    {
        base.Update();
    }

    protected override float MoveAround(Vector2 targetDir, Vector2 dir, Vector2 target, bool moveStraight)
    {
        float dist = Vector2.Distance(target, transform.position);

        if (moveStraight)
            return Vector2.Dot(targetDir, dir);

        if (dist < 2f)
            return Vector2.Dot(targetDir, dir) + Vector2.Dot(rb.velocity.normalized, dir);

        return 1.0f - Mathf.Abs(Vector2.Dot(targetDir, dir) - 0.9f) + Vector2.Dot(rb.velocity.normalized, dir);

    }
}

public class DT_MeleeMob : DecisionTreeGeneric<MeleeMob>
{
    public DT_MeleeMob(MeleeMob mob) : base(mob)
    {

    }

    /*
    public override void Initialise()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;

        /// Actions
        A_PathTo MoveToPlayer = new(mob, FindTileNearPlayer, CancelPathfinding);
        A_Melee AttackPlayer = new(mob, player, mob.MeleeSpeed);
        A_MoveTowards moveTowardsPlayer = new(mob, player);
        A_PullBack moveAwayFromPlayer = new(mob, player, 3f);

        /// Decisions
        Decision PullbackDecision = new(moveAwayFromPlayer, moveTowardsPlayer, ShouldPullback, mob);
        AttackDecision attackDecision = new(AttackPlayer, PullbackDecision, mob, player, mob.MeleeRange);

        root = new Decision(MoveToPlayer, attackDecision, ShouldMoveToPlayer, mob);
    }
    */

    bool ShouldPullback()
    {
        Vector2 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        return Vector2.Distance(mob.transform.position, playerPos) < mob.MeleeRange;
    }

    Vector2 FindTileNearPlayer()
    {
        return GameObject.FindGameObjectWithTag("EQSManager").GetComponent<EQSManager>().RunEQSystem(EQSystem.MeleeMobMoveToPlayer, mob.gameObject);
    }
}
