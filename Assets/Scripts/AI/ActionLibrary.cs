using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// A generic move to action node
/// </summary>
public class A_MoveTo : Action
{
    readonly PathfindingComponent pathfinding;

    public delegate Vector2 GetDestination();

    GetDestination destinationDelegate;

    // Make this action take a target and a range. Also we always want our move to to be an interruptor
    public A_MoveTo(BaseMob mob, GetDestination destinationDelegate) : base(mob, Interruptor: true)
    {
        pathfinding = mob.PathfindingComponent;
        this.destinationDelegate = destinationDelegate;
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

        // Run for as long as we have items in our queue
        while (path.Count > 0)
        {
            // Get our next position to move to from the queue
            Vector2 pos = path.Dequeue();

            // Keep moving towards the position until we're at least 0.1 units close to it
            while (Vector2.Distance(mob.transform.position, pos) > 0.2f)
            {
                Vector2 dir = pos - (Vector2)mob.transform.position;
                mob.rb.velocity = dir.normalized * mob.MovementSpeed;
                yield return null;
            }
        }

        // Set our velocity to zero once we've arrived
        mob.rb.velocity = Vector2.zero;
        yield return null;
    }
}

public class A_Idle : Action
{
    public A_Idle(BaseMob mob) : base(mob)
    {
        
    }

    public override IEnumerator Execute()
    {
        yield return new WaitForSeconds(1.0f);  // Wait for seconds so we dont spam whilst we debug
    }
}

public class A_Attack : Action
{
    Transform target;
    AIAbility ability;

    public A_Attack(BaseMob mob, Transform target, AIAbility ability) : base(mob)
    {
        this.target = target;
        this.ability = ability;
    }

    public override IEnumerator Execute()
    {
        // Wait for our casting time
        yield return new WaitForSeconds(ability.ability.CastTime);

        ability.Cast(mob, target);

        yield return null;
    }
}


/// DECISIONS
public class AttackDecision : Decision<float>
{
    AIAbility ability;
    public AttackDecision(DecisionTreeNode tNode, DecisionTreeNode fNode, BaseMob mob, AIAbility ability) : base(tNode, fNode, mob)
    {
        this.ability = ability;
    }

    public override float TestData()
    {
        return Vector2.Distance(mob.transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);
    }

    public override DecisionTreeNode GetBranch()
    {
        // If we are in range & have line of sight
        if (TestData() <= ability.ability.Range && mob.HasLineOfSight(GameObject.FindGameObjectWithTag("Player").transform.position) && ability.canCast)
        {
            return trueNode;
        }
        return falseNode;
    }
}

