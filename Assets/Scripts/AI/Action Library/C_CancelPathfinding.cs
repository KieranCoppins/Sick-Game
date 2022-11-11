using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_CancelPathfinding : F_Condition
{
    Vector2 prevPlayerPos;
    Transform target;

    public override void Initialise(BaseMob mob)
    {
        base.Initialise(mob);
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public override bool Invoke()
    {
        Vector2 playerPos = target.position;
        bool shouldCancel = mob.HasLineOfSight(playerPos) || Vector2.Distance(prevPlayerPos, playerPos) > 3f;
        if (shouldCancel)
            prevPlayerPos = playerPos;
        Debug.Log(shouldCancel);
        return shouldCancel;
    }
}
