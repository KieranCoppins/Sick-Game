using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class D_Condition : Decision
{
    [HideInInspector] public Function<bool> Condition;

    public D_Condition() : base()
    {

    }

    public D_Condition(F_Condition Condition)
    {
        this.Condition = Condition;
    }


    public override DecisionTreeNode GetBranch()
    {
        return Condition.Invoke() ? trueNode : falseNode;
    }
}
