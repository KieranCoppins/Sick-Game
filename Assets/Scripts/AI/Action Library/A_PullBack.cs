using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_PullBack : Action
{
    Vector2 desiredPosition;
    [SerializeField] float distance;

    public A_PullBack() : base()
    {

    }
    public override void Initialise(BaseMob mob)
    {
        base.Initialise(mob);
    }

    public override IEnumerator Execute()
    {
        while (mob.Target != null && Vector2.Distance(mob.transform.position, mob.Target.position) < distance)
        {
            desiredPosition = mob.transform.position + (mob.transform.position - mob.Target.position).normalized * 5f;
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
