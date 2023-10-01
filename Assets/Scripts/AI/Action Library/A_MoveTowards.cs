using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KieranCoppins.DecisionTrees;

/// <summary>
/// Uses mob's MoveAround and AvoidTarget functions to move around the given target
/// </summary>
public class A_MoveTowards : CustomAction
{
    private Vector2 _desiredPosition;

    public override IEnumerator Execute()
    {
        while (Mob.Target != null && Mob.HasLineOfSight(Mob.Target.position))
        {
            _desiredPosition = Mob.Target.position;
            Vector2 dir = Mob.GetMovementVector(_desiredPosition);
            if ((Mob.DebugFlags & DebugFlags.Pathfinding) == DebugFlags.Pathfinding)
            {
                Debug.DrawRay((Vector2)Mob.transform.position, dir);
                Debug.DrawRay((Vector2)Mob.transform.position, _desiredPosition - (Vector2)Mob.transform.position, Color.blue);
            }
            Mob.RigidBody.velocity = dir.normalized * Mob.MovementSpeed;
            yield return null;
        }
        yield return null;
    }

    public override string GetDescription(BaseNodeView nodeView)
    {
        return "Move towards the mob's target using the mobs custom movement vectors.";
    }
}
