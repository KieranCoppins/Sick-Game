using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

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
    private BaseMob mob;

    public RootNode root;

    private Vector2 playerPrevPos;

    /// Editor Values
    // A list of nodes for our editor, they don't have to be linked to the tree
    [HideInInspector] public List<DecisionTreeEditorNode> nodes = new List<DecisionTreeEditorNode>();
    [HideInInspector] public List<InputOutputPorts> inputs = new List<InputOutputPorts>();

    public DecisionTree()
    {
    }

    public DecisionTree(BaseMob mob)
    {
        this.mob = mob;
    }

    public void Initialise(BaseMob mob)
    {
        this.mob = mob;
        root.Initialise(mob);
    }
    public Action Run()
    {
        try
        {
            return root.MakeDecision() as Action;
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

    public DecisionTreeEditorNode CreateNode(System.Type type, Vector2 creationPos)
    {
        DecisionTreeEditorNode node = ScriptableObject.CreateInstance(type) as DecisionTreeEditorNode;
        node.name = type.Name;
        node.guid = GUID.Generate().ToString();
        node.positionalData.xMin = creationPos.x;
        node.positionalData.yMin = creationPos.y;
        nodes.Add(node);

        AssetDatabase.AddObjectToAsset(node, this);
        AssetDatabase.SaveAssets();
        return node;
    }

    public DecisionTreeEditorNode CreateNode(ScriptableObject scriptableObject, Vector2 creationPos)
    {
        DecisionTreeEditorNode node = ScriptableObject.Instantiate(scriptableObject) as DecisionTreeEditorNode;
        node.name = scriptableObject.name;
        node.guid = GUID.Generate().ToString();
        node.positionalData.xMin = creationPos.x;
        node.positionalData.yMin = creationPos.y;
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

    [HideInInspector] public BaseMob mob;

    public virtual void Initialise(BaseMob mob)
    {
        this.mob = mob;
    }
}

public abstract class DecisionTreeNode : DecisionTreeEditorNode
{

    public DecisionTreeNode()
    {

    }
    public abstract DecisionTreeNode MakeDecision();

    public virtual DecisionTreeNode Clone()
    {
        return Instantiate(this);
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

/// <summary>
/// An abstract attack action that all attack actions should inherit
/// </summary>
public abstract class A_Attack : Action
{
    public bool CanCast { get; protected set; }

    protected float cooldown;
    protected Transform target;

    public A_Attack()
    {
        Flags |= ActionFlags.Interruptor;       // This action is an interruptor
        Flags &= ~ActionFlags.Interruptable;    // This action is not interruptable
        CanCast = true;
    }

    protected virtual IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        CanCast = true;
    }

    public override void Initialise(BaseMob mob)
    {
        base.Initialise(mob);
        target = GameObject.FindGameObjectWithTag("Player").transform; // TODO make the target a parameter so we can define different targets
    }
}


public delegate bool Condition();
public abstract class Decision : DecisionTreeNode
{
    public DecisionTreeNode trueNode;
    public DecisionTreeNode falseNode;

    readonly Condition Condition;

    public Decision()
    {

    }

    public abstract DecisionTreeNode GetBranch();

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

    public override void Initialise(BaseMob mob)
    {
        base.Initialise(mob);
        trueNode.Initialise(mob);
        falseNode.Initialise(mob);
    }
}

/// Custom Yield Instructions

// Allows for a function to be called every frame whilst we are waiting for the seconds passed
public class DoTaskWhilstWaitingForSeconds : CustomYieldInstruction
{
    readonly UnityAction task;
    float timer;
    public DoTaskWhilstWaitingForSeconds(UnityAction task, float seconds)
    {
        this.task = task;
        this.timer = seconds;
    }

    public override bool keepWaiting
    {
        get
        {
            task.Invoke();
            timer -= Time.deltaTime;
            return timer > 0;
        }
    }
}

///  Editor classes that are also referenced in engine

[System.Serializable]
public class InputOutputPorts
{
    public string inputGUID;
    public string inputPortName;
    public string outputGUID;
    public string outputPortName;

    public InputOutputPorts(string inputGUID, string inputPortName, string outputGUID, string outputPortName)
    {
        this.inputGUID = inputGUID;
        this.inputPortName = inputPortName;
        this.outputGUID = outputGUID;
        this.outputPortName = outputPortName;
    }

    public override string ToString()
    {
        return $"In GUID: {inputGUID} | In Port: {inputPortName} | Out GUID: {outputGUID} | Out Port: {outputPortName}";
    }

    public static bool operator !=(InputOutputPorts input, Edge edge)
    {
        return !(input == edge);
    }

    public static bool operator ==(InputOutputPorts input, Edge edge)
    {
        BaseNodeView inputNodeView = edge.input.node as BaseNodeView;
        BaseNodeView outputNodeView = edge.output.node as BaseNodeView;
        return input.inputGUID == inputNodeView.node.guid &&
            input.inputPortName == edge.input.name &&
            input.outputGUID == outputNodeView.node.guid &&
            input.outputPortName == edge.output.name;
    }

    public bool Equals(InputOutputPorts input)
    {
        return this.inputGUID == input.inputGUID && 
            this.inputPortName == input.inputPortName && 
            this.outputGUID == input.outputGUID && 
            this.outputPortName == input.outputPortName;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as InputOutputPorts);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

}

public abstract class BaseNodeView : UnityEditor.Experimental.GraphView.Node
{
    public System.Action<BaseNodeView> OnNodeSelected;

    public DecisionTreeEditorNode node;

    public Dictionary<string, Port> inputPorts;
    public Dictionary<string, Port> outputPorts;

    public BaseNodeView(DecisionTreeEditorNode node)
    {
        this.node = node;
        this.title = node.name;

        style.left = node.positionalData.xMin;
        style.top = node.positionalData.yMin;
        this.viewDataKey = node.guid;
        inputPorts = new Dictionary<string, Port>();
        outputPorts = new Dictionary<string, Port>();
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        node.positionalData = newPos;
    }

    public override void OnSelected()
    {
        base.OnSelected();
        if (OnNodeSelected != null)
            OnNodeSelected.Invoke(this);
    }

}

