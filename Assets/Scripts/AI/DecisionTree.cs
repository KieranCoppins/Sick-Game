using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using Unity.VisualScripting;
using System.Reflection.Emit;
using UnityEngine.UIElements;

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

    [HideInInspector] public RootNode root;

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
        return root.MakeDecision() as Action;
    }

    public DecisionTree Clone(string name = null)
    {
        DecisionTree tree = Instantiate(this);
        if (name != null)
            tree.name = name;
        tree.root = root.Clone() as RootNode;
        return tree;
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

public delegate void DecisionTreeEditorNodeOnValidate();

public abstract class DecisionTreeEditorNode : ScriptableObject
{
    /// Editor Values
    [HideInInspector] public string guid;
    [HideInInspector] public Rect positionalData;

    [HideInInspector] public BaseMob mob;

    public DecisionTreeEditorNodeOnValidate OnValidateCallback { get; set; }

    public virtual void Initialise(BaseMob mob)
    {
        this.mob = mob;
    }
    public virtual DecisionTreeEditorNode Clone()
    {
        return Instantiate(this);
    }

    public virtual string GetTitle()
    {
        return GenericHelpers.SplitCamelCase(name.Substring(2));
    }

    public virtual string GetDescription(BaseNodeView nodeView)
    {
        return "This is the default description of a DecisionTreeEditorNode";
    }

    private void OnValidate()
    { 
        OnValidateCallback?.Invoke();
    }
}

public abstract class DecisionTreeNode : DecisionTreeEditorNode
{

    public DecisionTreeNode()
    {

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

    public ActionFlags Flags { get { return _flags; } protected set { _flags = value; } }
    [EnumFlags]
    [SerializeField]
    ActionFlags _flags;

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
    protected float cooldown;

    public A_Attack()
    {
        Flags |= ActionFlags.Interruptor;       // This action is an interruptor
        Flags &= ~ActionFlags.Interruptable;    // This action is not interruptable
    }

    protected virtual IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        mob.CanAttack = true;
    }

    public override void Initialise(BaseMob mob)
    {
        base.Initialise(mob);
    }

    public override string GetDescription(BaseNodeView nodeView)
    {
        return $"Attack the mob's target. The attack has a {cooldown} second cooldown";
    }
}

public abstract class Decision : DecisionTreeNode
{
    [HideInInspector] public DecisionTreeNode trueNode;
    [HideInInspector] public DecisionTreeNode falseNode;

    public Decision()
    {

    }

    public abstract DecisionTreeNode GetBranch();

    public override DecisionTreeNode MakeDecision()
    {
        return GetBranch().MakeDecision();
    }

    public override DecisionTreeEditorNode Clone()
    {
        Decision node = Instantiate(this);
        node.trueNode = (DecisionTreeNode)trueNode.Clone();
        node.falseNode = (DecisionTreeNode)falseNode.Clone();
        return node;
    }

    public override void Initialise(BaseMob mob)
    {
        base.Initialise(mob);
        trueNode.Initialise(mob);
        falseNode.Initialise(mob);
    }
}

// A base function node that can return a given value
public abstract class Function<T> : DecisionTreeEditorNode
{
    public Function()
    {

    }

    // The condition to invoke
    public abstract T Invoke();
}

public abstract class F_Condition : Function<bool>
{
    public override string GetDescription(BaseNodeView nodeView)
    {
        return $"Returns true if {GetSummary(nodeView)}.";
    }

    /// <summary>
    /// This should return a quick summary of what the function does. NOT THE DESCRIPTION. This will be used within the description of other nodes.
    /// </summary>
    /// <returns></returns>
    public abstract string GetSummary(BaseNodeView nodeView);
}


public abstract class F_LogicGate : F_Condition
{
    public F_Condition A;
    public F_Condition B;

    public F_LogicGate(F_Condition A, F_Condition B)
    {
        this.A = A;
        this.B = B;
    }

    public override void Initialise(BaseMob mob)
    {
        base.Initialise(mob);
        A.Initialise(mob);
        B.Initialise(mob);
    }

    public override DecisionTreeEditorNode Clone()
    {
        F_LogicGate node = Instantiate(this);
        node.A = (F_Condition)A.Clone();
        node.B = (F_Condition)B.Clone();
        return node;
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

// Does task every frame until condition is true
public class DoTaskWhilstWaitingUntil : CustomYieldInstruction
{
    readonly UnityAction task;
    Func<bool> condition;
    public DoTaskWhilstWaitingUntil(UnityAction task, Func<bool> condition)
    {
        this.task = task;
        this.condition = condition;
    }

    public override bool keepWaiting
    {
        get
        {
            task.Invoke();
            return !condition.Invoke();
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

    public static bool operator !=(InputOutputPorts input, UnityEditor.Experimental.GraphView.Edge edge)
    {
        return !(input == edge);
    }

    public static bool operator ==(InputOutputPorts input, UnityEditor.Experimental.GraphView.Edge edge)
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

    public List<BaseNodeView> connectedNodes;

    readonly UnityEngine.UIElements.Label descriptionLabel;
    readonly UnityEngine.UIElements.Label errorLabel;
    readonly UnityEngine.UIElements.VisualElement errorContainer;

    public string description {  
        get { return descriptionLabel.text; } 
        
        set
        {
            if (value != description)
            {
                descriptionLabel.text = value;

                // Update all nodes connected to this node until theres no change to be made
                foreach (var node in connectedNodes)
                {
                    node.description = node.node.GetDescription(node);
                }
            }
        } 
    }

    public string error
    {
        get { return errorLabel.text; }
        set
        {
            if (value == null || value == "")
            {
                errorLabel.style.display = DisplayStyle.None;
                errorContainer.style.display = DisplayStyle.None;
                errorLabel.text = "";
            }
            else
            {
                errorLabel.style.display = DisplayStyle.Flex;
                errorContainer.style.display = DisplayStyle.Flex;
                errorLabel.text = value;
            }
        }
    }


    public BaseNodeView(DecisionTreeEditorNode node) : base("Assets/Editor/AI/DecisionTreeNodeView.uxml")
    {
        inputPorts = new Dictionary<string, Port>();
        outputPorts = new Dictionary<string, Port>();
        connectedNodes = new List<BaseNodeView>();

        this.node = node;
        node.OnValidateCallback += OnValidate;
        this.title = node.GetTitle();

        // Default our error to be hidden
        errorLabel = this.Q<UnityEngine.UIElements.Label>("error-label");
        errorContainer = this.Q<UnityEngine.UIElements.VisualElement>("error");
        errorLabel.style.display = DisplayStyle.None;
        errorContainer.style.display = DisplayStyle.None;

        descriptionLabel = this.Q<UnityEngine.UIElements.Label>("description");
        this.description = node.GetDescription(this);

        style.left = node.positionalData.xMin;
        style.top = node.positionalData.yMin;
        this.viewDataKey = node.guid;
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

    void OnValidate()
    {
        title = node.GetTitle();
        description = node.GetDescription(this);
    }

}

