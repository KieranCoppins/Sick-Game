using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class D_ShouldPathfindToPlayer : Decision
{

    public override void Initialise(BaseMob mob)
    {
        base.Initialise(mob);
    }
    public override DecisionTreeNode GetBranch()
    {
        if (!mob.HasLineOfSight(mob.Target.position))
        {
            return trueNode;
        }
        return falseNode;
    }

}
