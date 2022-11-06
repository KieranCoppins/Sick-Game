using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

public class DecisionTreeGeneric<T> : DecisionTree where T : BaseMob
{
    protected T mob;
    public DecisionTreeGeneric()
    {

    }
    public DecisionTreeGeneric(T mob) : base(mob)
    {
        this.mob = mob;
    }
}

[CreateAssetMenu(menuName = "Decision Tree/Decision Tree")]
public class DecisionTree : ScriptableObject
{
    [SerializeField] private BaseMob mob;
    public DecisionTree()
    {
    }

    public DecisionTree(BaseMob mob)
    {
        this.mob = mob;
    }

    public RootNode root;

    private Vector2 playerPrevPos;

    /// Editor Values
    // A list of nodes for our editor, they don't have to be linked to the tree
    [HideInInspector] public List<DecisionTreeEditorNode> nodes;


    public void Initialise()
    {

    }
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

    public DecisionTree Clone()
    {
        DecisionTree tree = Instantiate(this);
        tree.root = root.Clone() as RootNode;
        return tree;
    }


    /// Some protected functions that maybe useful for all decision making

    protected virtual bool ShouldMoveToPlayer()
    {
        Vector2 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        if (!mob.HasLineOfSight(playerPos))
        {
            playerPrevPos = playerPos;
            return true;
        }
        return false;
    }
    protected virtual bool CancelPathfinding()
    {
        Vector2 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        return mob.HasLineOfSight(playerPos) || Vector2.Distance(playerPrevPos, playerPos) > 0.5f;
    }

    /// Editor Functions

    public DecisionTreeEditorNode CreateNode(System.Type type)
    {
        DecisionTreeEditorNode node = ScriptableObject.CreateInstance(type) as DecisionTreeEditorNode;
        node.name = type.Name;
        node.guid = GUID.Generate().ToString();
        nodes.Add(node);

        AssetDatabase.AddObjectToAsset(node, this);
        AssetDatabase.SaveAssets();
        return node;
    }

    public void DeleteNode(DecisionTreeEditorNode node)
    {
        nodes.Remove(node);
        AssetDatabase.RemoveObjectFromAsset(node);
        AssetDatabase.SaveAssets();
    }
}

public abstract class DecisionTreeEditorNode : ScriptableObject
{
    /// Editor Values
    [HideInInspector] public string guid;
    [HideInInspector] public Rect positionalData;


}

public abstract class DecisionTreeNode : DecisionTreeEditorNode
{
    [SerializeField] protected BaseMob mob;

    public DecisionTreeNode()
    {

    }

    public DecisionTreeNode(BaseMob mob)
    {
        this.mob = mob;
    }
    public abstract DecisionTreeNode MakeDecision();

    public virtual DecisionTreeNode Clone()
    {
        return Instantiate(this);
    }


}

public class RootNode : DecisionTreeNode
{
    public DecisionTreeNode child;

    public override DecisionTreeNode MakeDecision()
    {
        return child.MakeDecision();
    }

    public override DecisionTreeNode Clone()
    {
        RootNode node = Instantiate(this);
        node.child = child.Clone();
        return node;
    }
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

    public Action()
    {

    }

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
    [HideInInspector] public DecisionTreeNode trueNode;
    [HideInInspector] public DecisionTreeNode falseNode;

    readonly Condition Condition;

    public Decision()
    {

    }

    public Decision(DecisionTreeNode trueNode, DecisionTreeNode falseNode, Condition condDelegate, BaseMob mob) : base(mob)
    {
        this.trueNode = trueNode;
        this.falseNode = falseNode;
        Condition = condDelegate;
    }

    public virtual DecisionTreeNode GetBranch()
    {
        
        return Condition() ? trueNode : falseNode;
    }

    public override DecisionTreeNode MakeDecision()
    {
        return GetBranch().MakeDecision();
    }

    public override DecisionTreeNode Clone()
    {
        Decision node = Instantiate(this);
        node.trueNode = trueNode.Clone();
        node.falseNode = falseNode.Clone();
        return node;
    }
}

public abstract class Decision<T> : Decision
{
    public Decision()
    {

    }
    public Decision(DecisionTreeNode trueNode, DecisionTreeNode falseNode, BaseMob mob)
    {
        this.trueNode = trueNode;
        this.falseNode = falseNode;
    }

    public abstract T TestData();

    public override abstract DecisionTreeNode GetBranch();

    public override DecisionTreeNode Clone()
    {
        Decision<T> node = Instantiate(this);
        node.trueNode = trueNode.Clone();
        node.falseNode = falseNode.Clone();
        return node;
    }

}

public delegate bool Condition();
