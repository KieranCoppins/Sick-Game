using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class D_ShouldPullBack : Decision
{
    [SerializeField] float pullbackDistance;

    public override void Initialise(BaseMob mob)
    {
        base.Initialise(mob);
    }

    public override DecisionTreeNode GetBranch()
    {
        return TestData() ? trueNode : falseNode;
    }

    bool TestData()
    {
        return Vector2.Distance(mob.transform.position, mob.Target.position) < pullbackDistance;
    }
}
