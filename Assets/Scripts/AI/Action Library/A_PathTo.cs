using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate Vector2 GetDestination();
public delegate bool CancelPathfinding();

/// <summary>
/// A generic path to action node
/// </summary>
public class A_PathTo : Action
{
    readonly PathfindingComponent pathfinding;

    public GetDestination destinationDelegate;

    public CancelPathfinding cancelPathfindingDelegate;

    Vector2 desiredPosition;

    public A_PathTo()
    {

    }

    // Make this action take a target and a range. Also we always want our path to to be an interruptor
    public A_PathTo(GetDestination destinationDelegate, CancelPathfinding cancelPathfindingDelegate)
    {
        pathfinding = mob.PathfindingComponent;
        this.destinationDelegate = destinationDelegate;
        this.cancelPathfindingDelegate = cancelPathfindingDelegate;
    }

    public override IEnumerator Execute()
    {
        // Call our get destination delegate to get the tile we want to pathfind to
        Vector2 position = destinationDelegate();


        // Calculate a path to the position
        Vector2[] p = pathfinding.CalculateAStarPath(mob.transform.position, position);

        // Make sure we have a path
        if (p == null)
            yield break;

        // Put our path in a queue for easier access
        Queue<Vector2> path = new Queue<Vector2>(p);

        if ((mob.debugFlags & DebugFlags.Pathfinding) == DebugFlags.Pathfinding)
        {
            for (int i = 1; i < p.Length; i++)
                Debug.DrawLine(p[i - 1], p[i], Color.red, 1.0f);
        }

        // Run for as long as we have items in our queue
        while (path.Count > 0)
        {
            // Check if we should stop pathfinding
            if (cancelPathfindingDelegate())
                break;

            // Get our next position to move to from the queue
            desiredPosition = path.Dequeue();

            // Keep moving towards the position until we're at least 0.1 units close to it
            while (Vector2.Distance(mob.transform.position, desiredPosition) > 0.5f)
            {
                // Check if we should stop pathfinding
                if (cancelPathfindingDelegate())
                    break;

                // Add velocity of move to target
                Vector2 dir = mob.GetMovementVector(desiredPosition, true);
                if ((mob.debugFlags & DebugFlags.Pathfinding) == DebugFlags.Pathfinding)
                {
                    Debug.DrawRay((Vector2)mob.transform.position + (dir * 0.45f), dir);
                    Debug.DrawRay((Vector2)mob.transform.position, dir, Color.blue);

                }
                mob.rb.velocity = dir.normalized * mob.MovementSpeed;
                yield return null;
            }
        }

        // Set our velocity to zero once we've arrived
        mob.rb.velocity = Vector2.zero;
        yield return null;
    }
}