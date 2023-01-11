using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_HasLineOfSight : F_Condition
{
    public override bool Invoke() => mob.HasLineOfSight(mob.Target.position);

    public override string GetSummary(BaseNodeView nodeView)
    {
        return "the mob has line of sight to the player";
    }
}
