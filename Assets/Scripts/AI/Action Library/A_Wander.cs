using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KieranCoppins.DecisionTrees;

public class A_Wander : CustomAction
{
    [SerializeField] private float _wanderRange;
    [SerializeField] private float _wanderSpeed;

    public override IEnumerator Execute()
    {
        Vector2 originPoint = Mob.transform.position;
        while (true)
        {
            if (Random.Range(0f, 1f) <= 0.2f)
            {
                yield return new DoTaskWhilstWaitingForSeconds(() => Mob.RigidBody.velocity = Vector2.zero, Random.Range(0.3f, 2.0f));
            }
            else
            {
                Vector2 dir = Mob.WanderVector(originPoint, _wanderRange);
                yield return new DoTaskWhilstWaitingForSeconds(() =>
                {
                    if ((Mob.DebugFlags & DebugFlags.Pathfinding) == DebugFlags.Pathfinding)
                    {
                        Debug.DrawRay((Vector2)Mob.transform.position, dir);
                    }
                    RaycastHit2D hit = Physics2D.CircleCast((Vector2)Mob.transform.position, .4f, dir, .5f);
                    if (!hit)
                        Mob.RigidBody.velocity = dir.normalized * _wanderSpeed;
                    else
                        dir = Mob.WanderVector(originPoint, _wanderRange);
                }, Random.Range(0.3f, 2.0f));
            }
        }
    }

    public override string GetDescription(BaseNodeView nodeView)
    {
        return $"Wander around the point at which wondering started within a range of {_wanderRange}. During wandering the mob will move at {_wanderSpeed}.";
    }
}
