using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class DecisionTree<T> where T : BaseMob
{
    protected T mob;
    public DecisionTree(T mob)
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
            Debug.LogError("Decision Tree did not reach an action node (InvalidCastException) " + e.Message);
        }
        catch (NullReferenceException e)
        {
            Debug.LogError("Decision tree returned null. Has the tree been initialised? " + e.Message);
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
    [Flags]
    public enum ActionFlags
    {
        SyncAction = 1 << 0,
        Interruptor = 1 << 1,
        Interruptable = 1 << 2,
    }

    public ActionFlags Flags { get; protected set; }
    public Action(BaseMob mob) : base(mob)
    {
        Flags = 0;

        // Default interruptable to true
        Flags |= ActionFlags.Interruptable;
    }

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

    readonly Condition Condition;

    public Decision(DecisionTreeNode trueNode, DecisionTreeNode falseNode, Condition condDelegate, BaseMob mob) : base(mob)
    {
        this.trueNode = trueNode;
        this.falseNode = falseNode;
        Condition = condDelegate;
    }

    public DecisionTreeNode GetBranch()
    {
        
        return Condition() ? trueNode : falseNode;
    }

    public override DecisionTreeNode MakeDecision()
    {
        return GetBranch().MakeDecision();
    }
}

public abstract class Decision<T> : DecisionTreeNode
{
    protected readonly DecisionTreeNode trueNode;
    protected readonly DecisionTreeNode falseNode;
    public Decision(DecisionTreeNode trueNode, DecisionTreeNode falseNode, BaseMob mob) : base(mob)
    {
        this.trueNode = trueNode;
        this.falseNode = falseNode;
    }

    public abstract T TestData();

    public abstract DecisionTreeNode GetBranch();

    public override DecisionTreeNode MakeDecision()
    {
        return GetBranch().MakeDecision();
    }
}

public delegate bool Condition();
