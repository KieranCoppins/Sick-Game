using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class DecisionTree
{
    protected BaseMob mob;
    public DecisionTree(BaseMob mob)
    {
        this.mob = mob;
    }

    protected DecisionTreeNode root;

    public abstract void Initialise();
}

public abstract class DecisionTreeNode
{
    public abstract DecisionTreeNode MakeDecision();
}

public abstract class Action : DecisionTreeNode
{
    public override DecisionTreeNode MakeDecision()
    {
        return this;
    }

    /// <summary>
    /// Do this action
    /// </summary>
    public abstract void Execute();
}

public class Decision : DecisionTreeNode
{
    protected readonly DecisionTreeNode trueNode;
    protected readonly DecisionTreeNode falseNode;

    Condition condition;

    public DecisionTreeNode GetBranch()
    {
        
        return condition() ? trueNode : falseNode;
    }

    public override DecisionTreeNode MakeDecision()
    {
        return GetBranch().MakeDecision();
    }

    public Decision(DecisionTreeNode trueNode, DecisionTreeNode falseNode, Condition condDelegate)
    {
        this.trueNode = trueNode;
        this.falseNode = falseNode;
        condition = condDelegate;
    }
}

public abstract class Decision<T> : DecisionTreeNode
{
    protected readonly DecisionTreeNode trueNode;
    protected readonly DecisionTreeNode falseNode;

    public abstract T TestData();

    public abstract DecisionTreeNode GetBranch();

    public override DecisionTreeNode MakeDecision()
    {
        return GetBranch().MakeDecision();
    }

    public Decision(DecisionTreeNode trueNode, DecisionTreeNode falseNode)
    {
        this.trueNode=trueNode;
        this.falseNode=falseNode;
    }
}

public delegate bool Condition();
