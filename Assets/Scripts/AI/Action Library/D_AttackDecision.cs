using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KieranCoppins.DecisionTrees;

/// <summary>
/// A generic attack decision that checks if an attack action can be performed
/// </summary>
public class D_AttackDecision : CustomDecision
{
    private A_Attack _action;
    [SerializeField] private float _attackRange = 0;

    public override void Initialise<T>(T metaData)
    {
        base.Initialise(metaData);
        _action = TrueNode as A_Attack;
        if (_action == null)
            Debug.LogError("True node of D_AttackDecision doesn't point to an A_Attack action");
    }

    float TestData()
    {
        return Vector2.Distance(Mob.transform.position, Mob.Target.position);
    }

    public override DecisionTreeNode GetBranch()
    {
        // If we are in range & have line of sight
        if (TestData() <= _attackRange && Mob.HasLineOfSight(Mob.Target.position) && Mob.CanAttack)
        {
            return TrueNode;
        }
        return FalseNode;
    }

    public override string GetDescription(BaseNodeView nodeView)
    {
        try
        {
            nodeView.Error = "";
            return $"If the mob has line of sight to the target and is within {_attackRange} units, it will {TrueNode.GetTitle().ToLower()}.";
        }
        catch(System.Exception e)
        {
            nodeView.Error = e.Message;
            return "";
        }
    }
}
