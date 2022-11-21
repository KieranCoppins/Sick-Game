using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_Wander : Action
{
    [SerializeField] float wanderRange;
    [SerializeField] float wanderSpeed;
    public override IEnumerator Execute()
    {
        Vector2 originPoint = mob.transform.position;
        while (true) 
        {
            if (Random.Range(0f, 1f) <= 0.1f)
            {
                yield return new DoTaskWhilstWaitingForSeconds(() => mob.rb.velocity = Vector2.zero, Random.Range(0.3f, 2.0f));
            }
            else
            {
                Vector2 dir = mob.WanderVector(originPoint, wanderRange);
                yield return new DoTaskWhilstWaitingForSeconds(() =>
                {
                    if ((mob.debugFlags & DebugFlags.Pathfinding) == DebugFlags.Pathfinding)
                    {
                        Debug.DrawRay((Vector2)mob.transform.position + (dir * 0.45f), dir);
                    }
                    mob.rb.velocity = dir.normalized * wanderSpeed;
                }, Random.Range(0.3f, 2.0f));
            }
        }
    }
}
