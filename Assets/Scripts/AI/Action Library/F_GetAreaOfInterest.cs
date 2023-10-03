using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KieranCoppins.DecisionTrees;

// Replace function template type with the return type of your function
public class F_GetAreaOfInterest : CustomFunction<Vector2?>
{
    // Change type of this function to what you need it to be
    public override Vector2? Invoke()
    {
        return Mob.AreaOfInterest;
    }

    public override string GetDescription(BaseNodeView nodeView)
    {
        return $"Returns {GetSummary(nodeView)}.";
    }

    public override string GetSummary(BaseNodeView nodeView)
    {
        return "the mob's area of interest";
    }
}