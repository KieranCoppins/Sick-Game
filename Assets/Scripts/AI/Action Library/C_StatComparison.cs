using UnityEngine;
using KieranCoppins.DecisionTrees;
using KieranCoppins.GenericHelpers;

public class C_StatComparison : CustomFunction<bool>
{
    [SerializeField] PlayerStat stat;
    [SerializeField] Operators operation;
    [SerializeField] float value;
    public override bool Invoke()
    {
        int statValue = (int)typeof(BaseCharacter).GetProperty(stat, GenericHelpers.GetFieldFlags).GetValue(Mob);
        switch (operation)
        {
            case (Operators.LessThan):
                return statValue < value;
            case (Operators.GreaterThan):
                return statValue > value;
            case (Operators.EqualTo):
                return statValue == value;
            default:
                Debug.LogError("Unsupported operator");
                return false;
        }
    }

    public override string GetSummary(BaseNodeView nodeView)
    {
        try
        {
            nodeView.Error = "";
            return $"{GenericHelpers.SplitCamelCase(stat)} is {GenericHelpers.SplitCamelCase(operation.ToString()).ToLower()} {value}";
        }
        catch (System.Exception e)
        {
            nodeView.Error = e.Message;
            return "";
        }
    }

    public override string GetDescription(BaseNodeView nodeView)
    {
        return $"Returns true if {GetSummary(nodeView)}.";
    }
}
