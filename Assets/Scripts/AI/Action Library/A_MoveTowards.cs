using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Uses mob's MoveAround and AvoidTarget functions to move around the given target
/// </summary>
public class A_MoveTowards : Action
{
    Transform target;
    Vector2 desiredPosition;

    public A_MoveTowards()
    {

    }

    public override void Initialise()
    {
        base.Initialise();
        target = GameObject.FindGameObjectWithTag("Player").transform; // TODO make the target a parameter so we can define different targets
    }

    public override IEnumerator Execute()
    {
        while (mob.HasLineOfSight(target.position))
        {
            desiredPosition = target.position;
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