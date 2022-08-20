using System;
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
    public Action Run()
    {
        try
        {
            return (Action)root.MakeDecision();
        }
        catch (InvalidCastException e)
        {
            Debug.LogError("Decision Tree did not reach an action node (InvalidCastException)");
        }
        catch (NullReferenceException e)
        {
            Debug.LogError("Decision tree returned null. Has the tree been initialised?");
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        return null;
    }
}

public abstract class DecisionTreeNode
{
    protected BaseMob mob;
    public DecisionTreeNode(BaseMob mob)
    {
        this.mob = mob;
    }
    public abstract DecisionTreeNode MakeDecision();
}

public abstract class Action : DecisionTreeNode
{
    public Action(BaseMob mob, bool ASyncAction = false, bool Interruptor = false) : base(mob)
    {
        _asyncAction = ASyncAction;
        _interruptor = Interruptor;
    }

    public bool ASyncAction
    {
        get
        {
            return _asyncAction;
        }
    }
    public bool Interruptor
    {
        get
        {
            return _interruptor;
        }
    }

    protected readonly bool _asyncAction = false;
    protected readonly bool _interruptor = false;
    public override DecisionTreeNode MakeDecision()
    {
        return this;
    }

    /// <summary>
    /// Do this action
    /// </summary>
    public abstract IEnumerator Execute();
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

    public Decision(DecisionTreeNode trueNode, DecisionTreeNode falseNode, Condition condDelegate, BaseMob mob) : base(mob)
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

    public Decision(DecisionTreeNode trueNode, DecisionTreeNode falseNode, BaseMob mob) : base(mob)
    {
        this.trueNode=trueNode;
        this.falseNode=falseNode;
    }
}

public delegate bool Condition();
