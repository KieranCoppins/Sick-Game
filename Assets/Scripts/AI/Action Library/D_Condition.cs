using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class D_Condition : Decision
{
    [HideInInspector] public F_Condition Condition;

    public D_Condition(F_Condition Condition)
    {
        this.Condition = Condition;
    }


    public override DecisionTreeNode GetBranch()
    {
        return Condition.Invoke() ? trueNode : falseNode;
    }

    public override void Initialise(BaseMob mob)
    {
        base.Initialise(mob);
        Condition.Initialise(mob);
    }

    public override DecisionTreeEditorNode Clone()
    {
        D_Condition node = Instantiate(this);
        node.trueNode = (DecisionTreeNode)trueNode.Clone();
        node.falseNode = (DecisionTreeNode)falseNode.Clone();
        node.Condition = (F_Condition)Condition.Clone();
        return node;
    }

    public override string GetDescription()
    {
        try
        {
            return $"The mob will {trueNode.GetTitle().ToLower()} if {Condition.GetSummary().ToLower()}. Otherwise the mob will {falseNode.GetTitle().ToLower()}.";
        }
        catch
        {
            return "There was an issue with this description";
        }
    }
}
