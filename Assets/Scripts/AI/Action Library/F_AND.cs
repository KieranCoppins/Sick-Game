using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_AND : F_LogicGate
{
    public F_AND(F_Condition A, F_Condition B) : base(A, B)
    {
    }

    public override bool Invoke()
    {
        return A.Invoke() && B.Invoke();
    }

    public override string GetSummary()
    {
        //if (A != null && B != null)
        return $"{A?.GetSummary()} AND {B?.GetSummary()}";
    }
}
