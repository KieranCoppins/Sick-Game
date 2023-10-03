using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using KieranCoppins.DecisionTrees;
using System;

/// <summary>
/// A generic path to action node
/// </summary>
public class A_PathTo : CustomAction
{
    private PathfindingComponent _pathfinding;

    [HideInInspector] public Function<Vector2?> DestinationQuery;
    [HideInInspector] public Function<bool> CancelPathfindingCondition;

    public A_PathTo() : base()
    {
    }

    // Make this action take a target and a range. Also we always want our path to to be an interruptor
    public A_PathTo(Function<Vector2?> DestinationQuery, Function<bool> CancelPathfindingCondition) : base()
    {
        _pathfinding = Mob.PathfindingComponent;
        this.DestinationQuery = DestinationQuery;
        this.CancelPathfindingCondition = CancelPathfindingCondition;
    }

    public override IEnumerator Execute()
    {
        // Call our get destination delegate to get the tile we want to pathfind to
        Vector2? position = DestinationQuery.Invoke();

        if (position != null)
        {
            // If our CancelPathFindingCondition is true then we will set this var and break out of the path
            bool breakOut = false;

            // Calculate a path to the position
            Vector2[] p = _pathfinding.CalculateAStarPath(Mob.transform.position, position.Value);

            // Make sure we have a path
            if (p == null)
                yield break;

            // Put our path in a queue for easier access
            Queue<Vector2> path = new Queue<Vector2>(p);

            if ((Mob.DebugFlags & DebugFlags.Pathfinding) == DebugFlags.Pathfinding)
            {
                for (int i = 1; i < p.Length; i++)
                    Debug.DrawLine(p[i - 1], p[i], Color.red, 2f);
            }


            // Get our next position to move to from the queue
            Vector2 desiredPosition = path.Dequeue();

            // Run for as long as we have items in our queue and we can see our next node, if something has gone wrong, re calculate the path
            while (path.Count > 0 && Mob.HasLineOfSight(desiredPosition))
            {
                if (breakOut)
                    break;

                // Keep moving towards the position until we're at least 0.1 units close to it
                while (Vector2.Distance(Mob.transform.position, desiredPosition) > 0.05f)
                {
                    // Check if we should stop pathfinding
                    if (CancelPathfindingCondition && CancelPathfindingCondition.Invoke())
                    {
                        breakOut = true;
                        break;
                    }

                    // Add velocity of move to target
                    Vector2 dir = Mob.GetMovementVector(desiredPosition, true);
                    if ((Mob.DebugFlags & DebugFlags.Pathfinding) == DebugFlags.Pathfinding)
                    {
                        Debug.DrawRay((Vector2)Mob.transform.position + (dir * 0.45f), dir);
                        Debug.DrawRay((Vector2)Mob.transform.position, dir, Color.blue);

                    }
                    Mob.RigidBody.velocity = dir.normalized * Mob.MovementSpeed;

                    yield return null;
                }

                // Get our next position to move to from the queue
                desiredPosition = path.Dequeue();
            }

            // Set our velocity to zero once we've arrived
            Mob.RigidBody.velocity = Vector2.zero;
        }

        yield return null;
    }

    public override void Initialise<T>(T metaData)
    {
        base.Initialise(metaData);
        DestinationQuery.Initialise(metaData);
        CancelPathfindingCondition?.Initialise(metaData);
        _pathfinding = Mob.PathfindingComponent;
    }

    public override DecisionTreeEditorNodeBase Clone()
    {
        A_PathTo clone = Instantiate(this);
        clone.DestinationQuery = (Function<Vector2?>)DestinationQuery.Clone();
        clone.CancelPathfindingCondition = (Function<bool>)CancelPathfindingCondition?.Clone();
        return clone;
    }

    public override string GetDescription(BaseNodeView nodeView)
    {
        if (DestinationQuery == null)
            nodeView.Error = "A destination query is needed to pathfind to!";
        else
            nodeView.Error = "";

        return "Use A* to pathfind to the position given by our EQS query.";
    }

    public override List<DecisionTreeEditorNodeBase> GetChildren()
    {
        return new() { DestinationQuery, CancelPathfindingCondition };
    }
}
