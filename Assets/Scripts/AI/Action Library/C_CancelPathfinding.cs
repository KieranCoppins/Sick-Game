using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_CancelPathfinding : F_Condition
{
    Vector2 prevPlayerPos;

    public override void Initialise(BaseMob mob)
    {
        base.Initialise(mob);
    }

    public override bool Invoke()
    {
        if (mob.Target != null)
        {
            Vector2 playerPos = mob.Target.position;
            bool shouldCancel = mob.HasLineOfSight(playerPos) || Vector2.Distance(prevPlayerPos, playerPos) > 3f;
            if (shouldCancel)
                prevPlayerPos = playerPos;
            return shouldCancel;
        }
        return true;
    }
}
