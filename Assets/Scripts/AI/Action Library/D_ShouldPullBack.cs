using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class D_ShouldPullBack : Decision
{
    [SerializeField] float pullbackDistance;
    Transform target;

    public override void Initialise(BaseMob mob)
    {
        base.Initialise(mob);
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public override DecisionTreeNode GetBranch()
    {
        return TestData() ? trueNode : falseNode;
    }

    bool TestData()
    {
        return Vector2.Distance(mob.transform.position, target.position) < pullbackDistance;
    }
}
