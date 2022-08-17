using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// A generic move to action node
/// </summary>
public class A_MoveTo : Action
{
    PathfindingComponent pathfinding;
    Transform target;
    float range;

    // Make this action take a target and a range. Also we always want our move to to be an interruptor
    public A_MoveTo(BaseMob mob, Transform target, float range = 0) : base(mob, Interruptor: true)
    {
        pathfinding = mob.PathfindingComponent;
        this.target = target;
        this.range = range;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public override IEnumerator Execute()
    {
        if (target == null)
            throw new MissingReferenceException("No target assigned to action");

        // First we should find a suitable position in the passed range to the target
        // ^this can be done with some kind of environment query system or bitmap to highlight suitable tiles
        Vector2 position;
        if (range == 0)
            position = target.position;
        else
        {
            // Get our controller and eqs Manager
            TilemapController controller = GameObject.FindGameObjectWithTag("Tilemap").GetComponent<TilemapController>();
            EQSManager eqsManager = GameObject.FindGameObjectWithTag("EQSManager").GetComponent<EQSManager>();
            // Convert our float vector3 to a int vector 3 by flooring out ints to get the right tile coord
            Vector3Int pos = new Vector3Int(Mathf.FloorToInt(target.transform.position.x), Mathf.FloorToInt(target.transform.position.y));

            // Use the tilemap controller to get all tiles within range
            Vector2Int[] tiles = controller.GetTilesInRange(pos, Mathf.CeilToInt(range));

            //Initialise our EQS system with these parameters
            eqsManager.GetEQS(EQSSystem.RangedMobMoveToPlayer).Initialise(controller, tiles, mob.gameObject);
            // Run the EQS system to get our tile and get the postiion of the tile
            position = eqsManager.GetEQS(EQSSystem.RangedMobMoveToPlayer).Run();
        }

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
        // Play some kind of idle animation?
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

