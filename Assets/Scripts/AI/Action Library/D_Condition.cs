using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class D_Condition : Decision
{
    public F_Condition Condition;

    public D_Condition()
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
