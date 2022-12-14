using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A generic attack decision that checks if an attack action can be performed
/// </summary>
public class D_AttackDecision : Decision
{
    A_Attack action;
    [SerializeField] float attackRange = 0;

    public override void Initialise(BaseMob mob)
    {
        base.Initialise(mob);
        action = trueNode as A_Attack;
        if (action == null)
            Debug.LogError("True node of D_AttackDecision doesn't point to an A_Attack action");
    }

    float TestData()
    {
        return Vector2.Distance(mob.transform.position, mob.Target.position);
    }

    public override DecisionTreeNode GetBranch()
    {
        // If we are in range & have line of sight
        if (TestData() <= attackRange && mob.HasLineOfSight(mob.Target.position) && mob.CanAttack)
        {
            return trueNode;
        }
        return falseNode;
    }
}
