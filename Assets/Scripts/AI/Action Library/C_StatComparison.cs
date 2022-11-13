using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_StatComparison : F_Condition
{
    [SerializeField] PlayerStat stat;
    [SerializeField] Operators operation;
    [SerializeField] float value;
    public override bool Invoke()
    {
        int statValue = (int)typeof(BaseCharacter).GetField(stat, GenericHelpers.getFieldFlags).GetValue(mob);
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
}
