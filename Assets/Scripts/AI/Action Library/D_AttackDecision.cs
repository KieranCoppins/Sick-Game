using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A generic attack decision that checks if an attack action can be performed
/// </summary>
public class D_AttackDecision : Decision
{
    readonly A_Attack action;
    readonly float attackRange = 0;
    readonly Transform target;



    public D_AttackDecision()
    {

    }

    public D_AttackDecision(A_Attack attackNode, DecisionTreeNode fNode, Condition cond, BaseMob mob, Transform target, float attackRange) : base(attackNode, fNode, cond, mob)
    {
        this.action = attackNode;
        this.attackRange = attackRange;
        this.target = target;
    }

    float TestData()
    {
        return Vector2.Distance(mob.transform.position, target.position);
    }

    public override DecisionTreeNode GetBranch()
    {
        // If we are in range & have line of sight
        if (TestData() <= attackRange && mob.HasLineOfSight(target.position) && action.CanCast)
        {
            return trueNode;
        }
        return falseNode;
    }
}
