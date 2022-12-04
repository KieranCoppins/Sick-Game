using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[Flags]
public enum DebugFlags
{
    None = 0,
    Pathfinding = 1 << 0,
    EQS = 1 << 1,
    Combat = 1 << 2,
    DecisionTree = 1 << 3,
}

public enum CombatState
{
    Idle,
    Investigate,
    Combat
}

public enum AggressionSystems
{
    Timebased,
    Damagebased
}

[RequireComponent(typeof(PathfindingComponent))]
[RequireComponent(typeof(ActionManager))]
[DisallowMultipleComponent]

public abstract class BaseMob : BaseCharacter
{
    [Header("Events")]
    protected UnityEvent onTakeDamage;
    protected UnityEvent onHeal;
    protected UnityEvent onDeath;

    [Header("Mob Stats")]
    [SerializeField] public string mobName;

    [Header("AI")]
    [Tooltip("The decision tree for this mob to run")]
    [SerializeField] protected DecisionTree decisionTree;
    [Tooltip("The aggression system this mob uses")]
    [SerializeField] protected AggressionSystems AggressionSystem;
    [Tooltip("The threshold for our aggression system")]
    [SerializeField] protected float AggressionThreshold;
    [Tooltip("Nodes of a path that the AI will follow if they are out of combat")]
    public Transform[] IdlePathNodes;

    public PathfindingComponent PathfindingComponent { get; protected set; }
    protected float attackTimer;
    protected ActionManager actionManager;
    protected List<Vector2> movementDirections;
    public Vector2 AreaOfInterest { get; protected set; }
    [HideInInspector] public CombatState State { get; set; }

    // Values for our aggression system
    protected Dictionary<BaseCharacter, float> aggressionWeights = new Dictionary<BaseCharacter, float>();

    [Header("DEBUG VALUES")]
    [EnumFlags]
    [SerializeField]
    public DebugFlags debugFlags;


    public string GetCurrentActionText()
    {
        string text = "";
        foreach (IEnumerator action in actionManager.currentActions)
        {
            text += action.ToString() + ", ";
        }

        return text;
    }

    public string GetActionQueueText()
    {
        string text = "";
        foreach (ActionPacket action in actionManager.actionQueue)
        {
            text += action.action.ToString() + " | " + (Time.time - action.time) + "\n";
        }

        return text;
    }

    /// <summary>
    /// Takes dmg away from health and invokes the onTakeDamage event
    /// </summary>
    /// <param name="character"></param>
    /// <param name="dmg"></param>
    public void TakeDamage(BaseCharacter character, int dmg)
    {
        Health -= dmg;
        onTakeDamage?.Invoke();
        switch (AggressionSystem)
        {
            case AggressionSystems.Timebased:
                aggressionWeights[character] = Time.time;
                break;
            case AggressionSystems.Damagebased:
                if (aggressionWeights.ContainsKey(character))
                    aggressionWeights[character] += dmg * character.Aggression;
                else
                    aggressionWeights[character] = dmg * character.Aggression;
                break;
        }
        // Check if we have a target, if not assign the target to the character that attacked us
        if (Target)
        {
            BaseCharacter targetBaseCharacter = Target.GetComponent<BaseCharacter>();
            // Check if our target has ever attacked us, theres a chance our target is just the first character we saw. If they're not, then target the character that is actually attacking us
            if (aggressionWeights.ContainsKey(targetBaseCharacter))
            {
                // check if the character is of an enemy faction AND if the character's aggression weights is higher than our target's aggression weights plus our threshold
                if (character.Faction != Faction && (aggressionWeights[character] >= aggressionWeights[targetBaseCharacter] + AggressionThreshold))
                    Target = character.transform;
            }
            else
                if (character.Faction != Faction)
                    Target = character.transform;

        }
        else
            if (character.Faction != Faction)
                Target = character.transform;

        Debug.Log($"----{name}'s aggression table----");
        foreach (var item in aggressionWeights)
        {
            Debug.Log($"    {item.Key.name}: {item.Value}");
        }
        Debug.Log($"----{name}'s target: {Target.name}----");
        StartCoroutine(Stun(.5f));
    }

    /// <summary>
    /// Adds amount to health and invokes the onHeal event
    /// </summary>
    /// <param name="amount"></param>
    public void Heal(int amount)
    {
        Health += amount;
        onHeal?.Invoke();
    }

    protected override void Start()
    {
        base.Start();
        PathfindingComponent = GetComponent<PathfindingComponent>();
        actionManager = GetComponent<ActionManager>();

        // Set up movement directions
        movementDirections = new List<Vector2>();
        float angle = 18f;
        Vector2 dir = Vector2.up;
        for (int i = 0; i <= 360 / angle; i++)
        {
            movementDirections.Add((Quaternion.AngleAxis(angle * i, Vector3.back) * dir).normalized);
        }

        State = CombatState.Idle;

        // Initialise our decision tree
        decisionTree = decisionTree.Clone(this.name);
        decisionTree.Initialise(this);
        StartCoroutine(Think());
    }

    protected override void Update()
    {
        base.Update();
    }

    protected virtual void LateUpdate()
    {
        if (Stunned)
            rb.velocity = Vector2.zero;
    }

    /// <summary>
    /// Checks if the mob has a straight line of sight to position
    /// </summary>
    /// <param name="position"></param>
    /// <returns>True if the mob has line of sight</returns>
    public bool HasLineOfSight(Vector2 position)
    {
        Vector3 direction = position - (Vector2)transform.position;

        RaycastHit2D lowerHit;
        RaycastHit2D upperHit;
        float angle = Vector2.Angle(Vector2.up, (Vector2)direction);
        angle = direction.x > 0 ? angle : -angle;
        Vector2 lowerStart = (Vector2)transform.position + (Vector2)(Quaternion.AngleAxis(angle, Vector3.back) * new Vector2(-0.42f, 0));
        Vector2 upperStart = (Vector2)transform.position + (Vector2)(Quaternion.AngleAxis(angle, Vector3.back) * new Vector2(0.42f, 0));
        lowerHit = Physics2D.Raycast(lowerStart, position - lowerStart);
        upperHit = Physics2D.Raycast(upperStart, position - upperStart);
        if ((debugFlags & DebugFlags.Combat) == DebugFlags.Combat)
        {
            Debug.DrawRay(lowerStart, position - lowerStart, Color.magenta);
            Debug.DrawRay(upperStart, position - upperStart, Color.magenta);
        }
        if ((lowerHit.collider == null && lowerHit.collider == null) || 
            ((Vector2)lowerHit.collider.transform.position == position && (Vector2)upperHit.collider.transform.position == position))
            return true;

        return false;


    }

    public Vector2 GetMovementVector(Vector2 target, bool moveStraight = false)
    {
        Vector2 targetDir = (target - (Vector2)transform.position).normalized;
        
        List<KeyValuePair<Vector2, float>> directionWeights = new List<KeyValuePair<Vector2, float>>();

        // Calculate dot products
        foreach (Vector2 dir in movementDirections)
        {
            KeyValuePair<Vector2, float> pair = new KeyValuePair<Vector2, float>(dir, MoveAround(targetDir, dir, target, moveStraight));
            directionWeights.Add(pair);
        }

        // Sort our weights so the first direction has the best score
        directionWeights.Sort(new KeyValuePairComparer<Vector2, float>());

        foreach (KeyValuePair<Vector2, float> pair in directionWeights)
        {
            // Check to see if moving in this direction will cause us to hit an obstruction - we dont want this
            RaycastHit2D hit = Physics2D.CircleCast((Vector2)transform.position, .5f, pair.Key, 1f);
            if (!hit) return pair.Key;
            if ((debugFlags & DebugFlags.Pathfinding) == DebugFlags.Pathfinding)
                Debug.DrawRay((Vector2)transform.position, pair.Key * 2f, Color.magenta);
        }

        return Vector2.zero;
    }

    /// <summary>
    /// Is called when moving around the target
    /// </summary>
    /// <param name="targetDir"></param>
    /// <param name="dir"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    protected abstract float MoveAround(Vector2 targetDir, Vector2 dir, Vector2 target, bool moveStraight);

    public Vector2 WanderVector(Vector2 originPoint, float range)
    {
        Vector2 returnVector = (originPoint - (Vector2)transform.position).normalized;
        List<KeyValuePair<Vector2, float>> directionWeights = new List<KeyValuePair<Vector2, float>>();

        float distanceFromOrigin = Vector2.Distance(originPoint, transform.position);

        foreach(Vector2 dir in movementDirections)
        {
            // Our shaping function for wandering around an origin point
            float weight = (Vector2.Dot(dir, returnVector) * (distanceFromOrigin / range)) + (Vector2.Dot(dir, rb.velocity.normalized) + (1.0f - Mathf.PerlinNoise(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f))));
            KeyValuePair<Vector2, float> pair = new KeyValuePair<Vector2, float>(dir, weight);
            directionWeights.Add(pair);
        }

        directionWeights.Sort(new KeyValuePairComparer<Vector2, float>());

        foreach (KeyValuePair<Vector2, float> pair in directionWeights)
        {
            // Check to see if moving in this direction will cause us to hit an obstruction - we dont want this
            RaycastHit2D hit = Physics2D.CircleCast((Vector2)transform.position, .5f, pair.Key, 1f);
            if (!hit) return pair.Key;
            if ((debugFlags & DebugFlags.Pathfinding) == DebugFlags.Pathfinding)
                Debug.DrawRay((Vector2)transform.position, pair.Key * 2f, Color.magenta);
        }

        return Vector2.zero;
    }

    /// <summary>
    /// Finds a new action in the decision tree and adds it to the action manager every 100ms. Automatically starts running at runtime
    /// </summary>
    /// <returns></returns>
    protected IEnumerator Think()
    {
        while (true)
        {
            // Constantly try to determine what we should be doing
            Action actionToBeScheduled = decisionTree.Run();
            actionManager.ScheduleAction(actionToBeScheduled);
            actionManager.Execute();
            yield return new WaitForSeconds(0.1f);
        }

    }

    protected override void Die()
    {
        onDeath?.Invoke();
        Destroy(this.gameObject);
    }

    public void AlertListener(Vector2 origin)
    {
        if (State == CombatState.Idle)
        {
            AreaOfInterest = origin;
            State = CombatState.Investigate;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.isTrigger || State == CombatState.Combat)
            return;

        BaseCharacter character = collision.GetComponent<BaseCharacter>();
        if (character != null && character.Faction != Faction && HasLineOfSight(character.transform.position))
        {
            State = CombatState.Combat;
            Target = collision.transform;
            EmitAlert.Emit(transform.position, 10f);
        }
    }
}


public delegate float DirectionWeightFunction(Vector2 targetDir, Vector2 dir);

public class EnumFlagsAttribute : PropertyAttribute
{

}
