using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EmitAlert
{
    public static void Emit(Vector2 origin, float radius)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(origin, radius);
        foreach(var collider in colliders)
        {
            BaseMob mob = collider.GetComponent<BaseMob>();
            if (mob != null && mob.Target == null && mob.HasLineOfSight(origin))
            {
                mob.AlertListener(origin);
            }
        }
    }
}
