using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KieranCoppins.DecisionTrees;

public class D_ShouldPullBack : CustomDecision
{
    [SerializeField] private float _pullbackDistance;

    public override DecisionTreeNode GetBranch()
    {
        return TestData() ? TrueNode : FalseNode;
    }

    private bool TestData()
    {
        return Vector2.Distance(Mob.transform.position, Mob.Target.position) < _pullbackDistance && !Mob.CanAttack;
    }

    public override string GetDescription(BaseNodeView nodeView)
    {
        return $"Returns true if the mob is within {_pullbackDistance} units of its target and if the mob can attack.";
    }
}
