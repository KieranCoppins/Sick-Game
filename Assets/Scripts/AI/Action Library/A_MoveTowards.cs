using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Uses mob's MoveAround and AvoidTarget functions to move around the given target
/// </summary>
public class A_MoveTowards : Action
{
    Vector2 desiredPosition;

    public A_MoveTowards() : base()
    {

    }

    public override void Initialise(BaseMob mob)
    {
        base.Initialise(mob);
    }

    public override IEnumerator Execute()
    {
        while (mob.Target != null && mob.HasLineOfSight(mob.Target.position))
        {
            desiredPosition = mob.Target.position;
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

    public override string GetDescription(BaseNodeView nodeView)
    {
        return "Move towards the mob's target using the mobs custom movement vectors.";
    }
}
