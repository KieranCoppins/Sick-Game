using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootNode : DecisionTreeNode
{
    public DecisionTreeNode child;

    public override DecisionTreeNode MakeDecision()
    {
        return child.MakeDecision();
    }

    public override void Initialise(BaseMob mob)
    {
        base.Initialise(mob);
        child.Initialise(mob);
    }

    public override DecisionTreeNode Clone()
    {
        RootNode node = Instantiate(this);
        node.child = child.Clone();
        return node;
    }
}
