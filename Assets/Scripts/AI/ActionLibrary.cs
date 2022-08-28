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

    Vector2 desiredPosition;

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

        if (mob.DebugMode)
        {
            for (int i = 1; i < p.Length; i++)
                Debug.DrawLine(p[i - 1], p[i], Color.red, 1.0f);
        }

        // Run for as long as we have items in our queue
        while (path.Count > 0)
        {
            // Get our next position to move to from the queue
            desiredPosition = path.Dequeue();

            // Keep moving towards the position until we're at least 0.1 units close to it
            while (Vector2.Distance(mob.transform.position, desiredPosition) > 0.5f)
            {
                // Add velocity of move to target
                Vector2 dir = mob.GetMovementVector(desiredPosition, MoveTowards, AvoidTarget);
                if(mob.DebugMode)
                {
                    Debug.DrawRay((Vector2)mob.transform.position + (dir * 0.45f), dir);
                    Debug.DrawRay((Vector2)mob.transform.position, desiredPosition - (Vector2)mob.transform.position, Color.blue);

                }
                mob.rb.velocity = dir.normalized * mob.MovementSpeed;
                yield return null;
            }
        }

        // Set our velocity to zero once we've arrived
        mob.rb.velocity = Vector2.zero;
        yield return null;
    }

    float MoveTowards(Vector2 targetDir, Vector2 dir)
    {
        return Vector2.Dot(targetDir, dir) + Vector2.Dot(mob.rb.velocity.normalized, dir);
    }

    float AvoidTarget(Vector2 targetDir, Vector2 dir)
    {
        return 1.0f - Mathf.Abs(Vector2.Dot(targetDir, dir) - 0.65f) + Vector2.Dot(mob.rb.velocity.normalized, dir); ;
    }
}

public class A_StrafeAround : Action
{
    readonly Transform target;
    readonly float distance;
    readonly float spiralModifier;

    Vector2 desiredPosition;

    /// <summary>
    /// Make the mob strafe around the target at distance. Spiral modifier should be between 0 and 1 inclusive,
    /// larger the number, the more the mob will move towards the target as they spiral
    /// </summary>
    /// <param name="mob"></param>
    /// <param name="target"></param>
    /// <param name="distance"></param>
    /// <param name="spiralModifier"></param>
    public A_StrafeAround(BaseMob mob, Transform target, float distance, float spiralModifier = 0) : base(mob)
    {
        this.target = target;
        this.distance = distance;
        this.spiralModifier = Mathf.Clamp(spiralModifier, 0f, 1f);
    }

    public override IEnumerator Execute()
    {
        while (true)
        {
            desiredPosition = target.position;
            Vector2 dir = mob.GetMovementVector(desiredPosition, MoveTowards, AvoidTarget);
            if (mob.DebugMode)
            {
                Debug.DrawRay((Vector2)mob.transform.position + (dir * 0.45f), dir);
                Debug.DrawRay((Vector2)mob.transform.position, desiredPosition - (Vector2)mob.transform.position, Color.blue);
            }
            mob.rb.velocity = dir.normalized * mob.MovementSpeed;
            yield return null;
        }
    }

    float MoveTowards(Vector2 targetDir, Vector2 dir)
    {
        if (Vector2.Distance(desiredPosition, mob.transform.position) < distance)
            return 1.0f - Mathf.Abs(Vector2.Dot(targetDir, dir) - spiralModifier) + Vector2.Dot(mob.rb.velocity.normalized, dir);
        return Vector2.Dot(targetDir, dir) + Vector2.Dot(mob.rb.velocity.normalized, dir);
    }

    float AvoidTarget(Vector2 targetDir, Vector2 dir)
    {
        return 1.0f - Mathf.Abs(Vector2.Dot(targetDir, dir) - 0.65f) + Vector2.Dot(mob.rb.velocity.normalized, dir); ;
    }
}

public class A_Attack : Action
{
    public bool CanCast
    {
        get { return _canCast; }
    }

    public AbilityBase Ability
    {
        get { return _ability; }
    }

    readonly Transform target;
    readonly AbilityBase _ability;

    bool _canCast = true;

    public A_Attack(BaseMob mob, Transform target, AbilityBase ability) : base(mob, Interruptor: true, Interruptable: false)
    {
        this.target = target;
        this._ability = ability;
    }

    public override IEnumerator Execute()
    {
        // Stop our velocity
        mob.rb.velocity = Vector2.zero;
        _canCast = false;

        // Wait for our casting time
        yield return new WaitForSeconds(_ability.CastTime);

        Vector2 direction = (target.position - mob.transform.position).normalized;
        _ability.Cast(mob.transform.position, direction, target);
        mob.StartCoroutine(Cooldown());

        yield return null;
    }

    public IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(_ability.AbilityCooldown);
        _canCast = true;
    }
}


/// DECISIONS
public class AttackDecision : Decision<float>
{
    A_Attack action;
    public AttackDecision(DecisionTreeNode tNode, DecisionTreeNode fNode, BaseMob mob) : base(tNode, fNode, mob)
    {
        this.action = (A_Attack)tNode;
    }

    public override float TestData()
    {
        return Vector2.Distance(mob.transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);
    }

    public override DecisionTreeNode GetBranch()
    {
        // If we are in range & have line of sight
        if (TestData() <= action.Ability.Range && mob.HasLineOfSight(GameObject.FindGameObjectWithTag("Player").transform.position) && action.CanCast)
        {
            return trueNode;
        }
        return falseNode;
    }
}

