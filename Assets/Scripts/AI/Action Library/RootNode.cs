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

    public override DecisionTreeEditorNode Clone()
    {
        RootNode node = Instantiate(this);
        node.child = (DecisionTreeNode)child.Clone();
        return node;
    }

    public override string GetTitle()
    {
        return GenericHelpers.SplitCamelCase(name);
    }

    public override string GetDescription(BaseNodeView nodeView)
    {
        return "This is the root node of the decision tree. This is your starting point.";
    }
}
