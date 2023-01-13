using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_HasIdlePath : F_Condition
{
    public override void Initialise(BaseMob mob)
    {
        base.Initialise(mob);
    }

    public override bool Invoke()
    {
        return mob.IdlePathNodes.Length > 0;
    }

    public override string GetSummary(BaseNodeView nodeView)
    {
        return "the mob has an idle path.";
    }
}