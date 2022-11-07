using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_PullBack : Action
{
    readonly Transform target;
    Vector2 desiredPosition;
    readonly float distance;

    public A_PullBack()
    {

    }

    public A_PullBack(BaseMob mob, Transform target, float distance) : base(mob)
    {
        this.target = target;
        this.distance = distance;
    }

    public override IEnumerator Execute()
    {
        while (Vector2.Distance(mob.transform.position, target.position) < distance)
        {
            desiredPosition = mob.transform.position + (mob.transform.position - target.position).normalized * 5f;
            Vector2 dir = mob.GetMovementVector(desiredPosition);
            if ((mob.debugFlags & DebugFlags.Pathfinding) == DebugFlags.Pathfinding)
            {
                Debug.DrawRay((Vector2)mob.transform.position + (dir * 0.45f), dir);
                Debug.DrawRay((Vector2)mob.transform.position, desiredPosition - (Vector2)mob.transform.position, Color.blue);
            }
            mob.rb.velocity = dir.normalized * mob.MovementSpeed;
            yield return null;
        }
        yield return null;
    }

}
