using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

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
    public A_PathTo(BaseMob mob, GetDestination destinationDelegate, CancelPathfinding cancelPathfindingDelegate) : base(mob)
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

/// <summary>
/// Uses mob's MoveAround and AvoidTarget functions to move around the given target
/// </summary>
public class A_MoveTowards : Action
{
    readonly Transform target;
    Vector2 desiredPosition;

    public A_MoveTowards()
    {

    }

    public A_MoveTowards(BaseMob mob, Transform target) : base(mob)
    {
        this.target = target;
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

public class A_PullBack : Action
{
    readonly Transform target;
    Vector2 desiredPosition;
    readonly float distance;

    public A_PullBack()
    {

    }

    public A_PullBack(BaseMob mob, Transform target, float distance) : base(mob)
    {
        this.target = target;
        this.distance = distance;
    }

    public override IEnumerator Execute()
    {
        while (Vector2.Distance(mob.transform.position, target.position) < distance)
        {
            desiredPosition = mob.transform.position + (mob.transform.position - target.position).normalized * 5f;
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

/// <summary>
/// An abstract attack action that all attack actions should inherit
/// </summary>
public abstract class A_Attack : Action
{
    public bool CanCast { get; protected set; }

    protected readonly float cooldown;
    protected readonly Transform target;

    public A_Attack()
    {

    }

    public A_Attack(BaseMob mob, Transform target, float cooldown) : base(mob)
    {
        this.cooldown = cooldown;
        this.target = target;
        CanCast = true;
        Flags |= ActionFlags.Interruptor;       // This action is an interruptor
        Flags &= ~ActionFlags.Interruptable;    // This action is not interruptable
    }

    protected virtual IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        CanCast = true;
    }
}

/// <summary>
/// An attack action that casts the mobs given ability
/// </summary>
public class A_CastAbility : A_Attack
{
    readonly AbilityBase _ability;

    Vector2 totalTargetVelocity;
    int totalVelocityEntries = 0;

    public A_CastAbility()
    {

    }

    public A_CastAbility(BaseMob mob, Transform target, AbilityBase ability) : base(mob, target, ability.AbilityCooldown)
    {
        this._ability = ability;
    }

    public override IEnumerator Execute()
    {
        totalTargetVelocity = target.GetComponent<Rigidbody2D>().velocity;
        totalVelocityEntries = 1;
        // Predict where the target will be
        Vector2 predictedLocation = PredictLocation();
        Vector2 predictedDirection = predictedLocation - (Vector2)mob.transform.position;

        // Do a ray cast to determine if we should start casting
        RaycastHit2D hit = Physics2D.Raycast((Vector2)mob.transform.position, predictedDirection.normalized, predictedDirection.magnitude);

        if ((mob.debugFlags & DebugFlags.Combat) == DebugFlags.Combat)
            Debug.DrawRay((Vector2)mob.transform.position, predictedDirection, Color.red, 0.5f);

        // We dont get a hit at all, or if we do hit something we hit our target
        if (!hit || (hit && hit.collider.CompareTag(target.tag)))
        {
            // Then we're good to start casting our ability

            // Stop our velocity
            mob.rb.velocity = Vector2.zero;
            CanCast = false;

            // Whilst we are casting, we want to get the average of our player's movement vector
            yield return new DoTaskWhilstWaitingForSeconds(() => { totalTargetVelocity += target.GetComponent<Rigidbody2D>().velocity; totalVelocityEntries += 1; }, _ability.CastTime);

            // Recalculate the predicted movement at the last second since the player may have changed direction during our casting time
            predictedLocation = PredictLocation();
            predictedDirection = predictedLocation - (Vector2)mob.transform.position;

            _ability.Cast(mob.transform.position, predictedDirection, target);
            mob.StartCoroutine(Cooldown());
        }

        yield return null;
    }

    Vector2 PredictLocation()
    {
        // Check if our ability is a projectile ability
        ProjectileAbility projectileAbility = _ability as ProjectileAbility;
        if (projectileAbility != null)
        {
            // Get the average velocity of our target
            Vector2 velocityVector = totalTargetVelocity / totalVelocityEntries;

            // Calculate how far away our target is
            float distanceFromAI = Vector2.Distance(mob.transform.position, target.position);

            // Using our targets position, the velocity they're moving and the velocity of our projectile, determine where they would be
            Vector2 predictedLocation = (Vector2)target.position + (velocityVector * (distanceFromAI / projectileAbility.GetProjectileVelocity()));

            return predictedLocation;
        }

        return target.position;
    }
}

/// <summary>
/// An attack action that melees the target
/// </summary>
public class A_Melee : A_Attack
{
    public A_Melee()
    {

    }
    public A_Melee(BaseMob mob, Transform target, float attackSpeed) : base(mob, target, attackSpeed)
    {
    }
    public override IEnumerator Execute()
    {
        CanCast = false;

        // We should play some kind of attack animation
        // We can do a sphere overlap cast to determine colliders where the melee weapon is.
        // We can then put an event in the animation to deal damage to all the colliders in the overlap check
        // This does require characters and animations to be included!

        mob.StartCoroutine(Cooldown());
        yield return null;
    }
}


/// DECISIONS

/// <summary>
/// A generic attack decision that checks if an attack action can be performed
/// </summary>
public class AttackDecision : Decision<float>
{
    readonly A_Attack action;
    readonly float attackRange = 0;
    readonly Transform target;



    public AttackDecision()
    {

    }

    public AttackDecision(A_Attack attackNode, DecisionTreeNode fNode, BaseMob mob, Transform target, float attackRange) : base(attackNode, fNode, mob)
    {
        this.action = attackNode;
        this.attackRange = attackRange;
        this.target = target;
    }

    public override float TestData()
    {
        return Vector2.Distance(mob.transform.position, target.position);
    }

    public override DecisionTreeNode GetBranch()
    {
        // If we are in range & have line of sight
        if (TestData() <= attackRange && mob.HasLineOfSight(target.position) && action.CanCast)
        {
            return trueNode;
        }
        return falseNode;
    }
}


/// Custom Yield Instructions

// Allows for a function to be called every frame whilst we are waiting for the seconds passed
public class DoTaskWhilstWaitingForSeconds : CustomYieldInstruction
{
    readonly UnityAction task;
    float timer;
    public DoTaskWhilstWaitingForSeconds(UnityAction task, float seconds)
    {
        this.task = task;
        this.timer = seconds;
    }

    public override bool keepWaiting
    {
        get
        {
            task.Invoke();
            timer -= Time.deltaTime;
            return timer > 0;
        }
    }
}

