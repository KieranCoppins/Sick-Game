using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DebugHelper : MonoBehaviour
{
    [Header("Press SPACE to Invoke event")]
    [SerializeField] UnityEvent DebugCalls;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            DebugCalls.Invoke();
    }
}

/// 
///     BELOW CONTAINS HOW TO IMPLEMENT A DECISION TREE WITH DECISION, DECISION<T> & ACTION NODES.
///     This decision tree system is very script heavy. It uses plenty of generic classes,
///     but it doesn't allow for a non programatic workflow. However, it does mean that
///     it is highly customisable where each action or decision can be unique and made just the way
///     the programmer wants (An issue with Unreal Engine's Decision trees).
///

public class TestDecisionTree : DecisionTree
{
    public TestDecisionTree(BaseMob mob) : base(mob)
    {

    }

    public override void Initialise()
    {
        Attack attackAction = new Attack();
        Dodge dodge = new Dodge();
        Idle idle = new Idle();
        MoveToPlayer moveToPlayer = new MoveToPlayer();
        Decision shouldAttack = new Decision(attackAction, idle, ShouldAttack);
        FloatDecision shouldMoveToPlayer = new FloatDecision(moveToPlayer, shouldAttack, 2.0f, 10.0f);
        root = new Decision(dodge, shouldMoveToPlayer, ShouldDodge);

        ((Action)root.MakeDecision()).Execute();
    }

    bool ShouldAttack()
    {
        Debug.Log("Should attack");
        return true;
    }

    bool ShouldDodge()
    {
        Debug.Log("Should Dodge");
        return false;
    }
}

public class Attack : Action
{
    public override void Execute()
    {
        Debug.Log("Attacking");
    }
}

public class Dodge : Action
{
    public override void Execute()
    {
        Debug.Log("Dodging");
    }
}

public class Idle : Action
{
    public override void Execute()
    {
        Debug.Log("Idleing");
    }
}

public class MoveToPlayer : Action
{
    public override void Execute()
    {
        Debug.Log("Moving to player");
    }
}

// I cannot think of a scenario where we'd want to use a typed decision like this and not use a function delegate
// However, it is good to have I suppose
public class FloatDecision : Decision<float>
{
    float minValue;
    float maxValue;

    public override DecisionTreeNode GetBranch()
    {
        Debug.Log("Should move towards player");
        if (TestData() >= minValue && TestData() <= maxValue)
        {
            return trueNode;
        }
        return falseNode;
    }

    public override float TestData()
    {
        return 5.0f;
    }

    public FloatDecision(DecisionTreeNode trueNode, DecisionTreeNode falseNode, float minValue, float maxValue) :
        base(trueNode, falseNode)
    {
        this.minValue = minValue;
        this.maxValue = maxValue;
    }
}
