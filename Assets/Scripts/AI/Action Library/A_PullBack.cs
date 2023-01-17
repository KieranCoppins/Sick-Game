using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KieranCoppins.DecisionTrees;

public class A_PullBack : CustomAction
{
    private Vector2 _desiredPosition;
    [SerializeField] private float _distance;

    public override IEnumerator Execute()
    {
        while (Mob.Target != null && Vector2.Distance(Mob.transform.position, Mob.Target.position) < _distance)
        {
            _desiredPosition = Mob.transform.position + (Mob.transform.position - Mob.Target.position).normalized * 5f;
            Vector2 dir = Mob.GetMovementVector(_desiredPosition);
            if ((Mob.DebugFlags & DebugFlags.Pathfinding) == DebugFlags.Pathfinding)
            {
                Debug.DrawRay((Vector2)Mob.transform.position + (dir * 0.45f), dir);
                Debug.DrawRay((Vector2)Mob.transform.position, _desiredPosition - (Vector2)Mob.transform.position, Color.blue);
            }
            Mob.RigidBody.velocity = dir.normalized * Mob.MovementSpeed;
            yield return null;
        }
        yield return null;
    }

    public override string GetDescription(BaseNodeView nodeView)
    {
        return $"The mob will retreat away from the target until they are {_distance} units away.";
    }
}
