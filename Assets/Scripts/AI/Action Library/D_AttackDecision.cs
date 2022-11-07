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
    Transform target;



    public D_AttackDecision()
    {

    }

    public override void Initialise()
    {
        base.Initialise();
        target = GameObject.FindGameObjectWithTag("Player").transform; // TODO make the target a parameter so we can define different targets
        action = trueNode as A_Attack;
        if (action == null)
            Debug.LogError("True node of D_AttackDecision doesn't point to an A_Attack action");
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
