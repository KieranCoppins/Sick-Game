using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KieranCoppins.DecisionTrees;

public class C_CancelPathfinding : CustomFunction<bool>
{
    private Vector2 _prevPlayerPos;

    public override bool Invoke()
    {
        if (Mob.Target != null)
        {
            Vector2 playerPos = Mob.Target.position;
            bool shouldCancel = Mob.HasLineOfSight(playerPos) || Vector2.Distance(_prevPlayerPos, playerPos) > 3f;
            if (shouldCancel)
                _prevPlayerPos = playerPos;
            return shouldCancel;
        }
        return true;
    }

    public override string GetSummary(BaseNodeView nodeView)
    {
        return "the player no longer has line of sight to the target or if the player has moved more than 3 units";
    }

    public override string GetDescription(BaseNodeView nodeView)
    {
        return $"Returns true if {GetSummary(nodeView)}";
    }
}
