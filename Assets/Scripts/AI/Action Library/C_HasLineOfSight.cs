using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KieranCoppins.DecisionTrees;

public class C_HasLineOfSight : CustomFunction<bool>
{
    public override bool Invoke() => Mob.HasLineOfSight(Mob.Target.position);

    public override string GetSummary(BaseNodeView nodeView)
    {
        return "the mob has line of sight to the player";
    }

    public override string GetDescription(BaseNodeView nodeView)
    {
        return $"Returns true if {GetSummary(nodeView)}.";
    }
}
