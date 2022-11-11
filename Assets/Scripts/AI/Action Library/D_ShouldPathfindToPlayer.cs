using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class D_ShouldPathfindToPlayer : Decision
{
    Transform target;

    public override void Initialise(BaseMob mob)
    {
        base.Initialise(mob);
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }
    public override DecisionTreeNode GetBranch()
    {
        if (!mob.HasLineOfSight(target.position))
        {
            return trueNode;
        }
        return falseNode;
    }

}
