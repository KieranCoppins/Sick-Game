using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KieranCoppins.DecisionTrees;

public class C_HasTarget : CustomFunction<bool>
{
    public override bool Invoke()
    {
        return Mob.Target != null;
    }

    public override string GetSummary(BaseNodeView nodeView)
    {
        return "the mob has a target";
    }

    public override string GetDescription(BaseNodeView nodeView)
    {
        return $"Returns true if {GetSummary(nodeView)}.";
    }
}
