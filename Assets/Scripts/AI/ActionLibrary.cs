using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_MoveInRangeOfPlayer : Action
{
    PathfindingComponent pathfinding;
    public A_MoveInRangeOfPlayer(BaseMob mob) : base(mob)
    {
        pathfinding = mob.PathfindingComponent;
    }

    public override IEnumerator Execute()
    {

        // First find a position to move to
        // ^this can be done with some kind of environment query system or bitmap to highlight suitable tiles
        Vector2 position = new(-3, 12);

        // Calculate a path to the position
        Vector2[] p = pathfinding.CalculateAStarPath(mob.transform.position, position);

        if (p == null)
            yield break;

        // Put our path in a queue for easier access
        Queue<Vector2> path = new Queue<Vector2>(p);

        yield return null;

        // Run for as long as we have items in our queue
        while (path.Count > 0)
        {
            // Get our next position to move to from the queue
            Vector2 pos = path.Dequeue();

            // Keep moving towards the position until we're at least 0.1 units close to it
            while (Vector2.Distance(mob.transform.position, pos) > 0.1f)
            {
                Vector2 dir = pos - (Vector2)mob.transform.position;
                mob.rb.velocity = dir.normalized * mob.MovementSpeed;
                Debug.DrawLine(mob.transform.position, pos, Color.red, 500.0f);
                yield return null;
            }
        }

        // Set our velocity to zero once we've arrived
        mob.rb.velocity = Vector2.zero;
        yield return null;
    }
}

