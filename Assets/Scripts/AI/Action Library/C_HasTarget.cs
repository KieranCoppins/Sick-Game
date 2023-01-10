using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_HasTarget : F_Condition
{
    public override bool Invoke()
    {
        return mob.Target != null;
    }

    public override string GetSummary()
    {
        return "the mob has a target.";
    }
}
