using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_FollowPath : Action
{
    [SerializeField] float walkSpeed;
    [SerializeField] float waitTime;
    int currentWaypoint = 0;
    public A_FollowPath() : base()
    {
        Flags |= ActionFlags.Interruptable;
    }

    public override void Initialise(BaseMob mob)
    {
        base.Initialise(mob);
    }

    public override IEnumerator Execute()
    {
        while (true)
        {
            float distance = Vector2.Distance(mob.transform.position, mob.IdlePathNodes[currentWaypoint].position);
            while (distance > 0.3f)
            {
                Vector2 dir = mob.GetMovementVector(mob.IdlePathNodes[currentWaypoint].position, true);
                mob.rb.velocity = dir * walkSpeed;
                distance = Vector2.Distance(mob.transform.position, mob.IdlePathNodes[currentWaypoint].position);
                yield return null;
            }
            currentWaypoint++;
            if (currentWaypoint > mob.IdlePathNodes.Length - 1)
                currentWaypoint = 0;
            yield return new WaitForSeconds(waitTime);
        }
    }
}