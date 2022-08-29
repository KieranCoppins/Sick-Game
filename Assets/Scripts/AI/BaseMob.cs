using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PathfindingComponent))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(ActionManager))]
[DisallowMultipleComponent]
public abstract class BaseMob : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent onTakeDamage;
    public UnityEvent onHeal;
    public UnityEvent onDeath;
    public int Health {
        get
        {
            return _health;
        }
        protected set
        {
            _health = value;
            if (_health > _maxHealth)
                _health = _maxHealth;
            if (_health <= 0)
            {
                onDeath?.Invoke();
                Destroy(this.gameObject);
            }
        }
    }
    public int MaxHealth
    {
        get
        {
            return _maxHealth;
        }
        protected set
        {
            if (value > 0)
                _maxHealth = value;
        }
    }

    public float MovementSpeed
    {
        get { return movementSpeed; }
    }

    [Header("Mob Stats")]
    [Tooltip("The maximum health of the mob")]
    [SerializeField] int _maxHealth = 10;
    [Tooltip("How much damage on attack does this mob deal?")]
    [SerializeField] protected int damage = 1;
    [SerializeField] protected float movementSpeed = 2;
    [Tooltip("How often in seconds can this mob attack?")]
    [SerializeField] protected float attackRate = 2;
    [SerializeField] public string mobName { get; protected set; }

    [HideInInspector] public PathfindingComponent PathfindingComponent;
    [HideInInspector] public Rigidbody2D rb;

    protected float attackTimer;

    private int _health = 10;

    bool stopMoving = false;

    protected ActionManager actionManager;
    protected DecisionTree decisionTree;

    [Header("DEBUG VALUES")]
    [SerializeField] public bool DebugMode;

    protected List<Vector2> movementDirections;

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
    /// <param name="dmg"></param>
    public void TakeDamage(int dmg)
    {
        Health -= dmg;


        // Invoke our take damage event when we take damage
        onTakeDamage?.Invoke();
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

    protected virtual void Start()
    {
        Health = MaxHealth;
        PathfindingComponent = GetComponent<PathfindingComponent>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        actionManager = GetComponent<ActionManager>();

        // Set up movement directions
        movementDirections = new List<Vector2>();
        float angle = 18f;
        Vector2 dir = Vector2.up;
        for (int i = 0; i <= 360 / angle; i++)
        {
            movementDirections.Add((Quaternion.AngleAxis(angle * i, Vector3.back) * dir).normalized);
        }

        //GetMovementVector(new Vector2(5, 5));
    }

    // Implement basic movement following the path as default movement
    protected virtual void Update()
    {
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
        if (DebugMode)
        {
            Debug.DrawRay(lowerStart, position - lowerStart, Color.magenta);
            Debug.DrawRay(upperStart, position - upperStart, Color.magenta);
        }
        if (lowerHit.collider.CompareTag("Player") && upperHit.collider.CompareTag("Player"))
            return true;

        return false;


    }

    public void StopMoving()
    {
        stopMoving = true;
    }

    public void ResumeMoving()
    {
        stopMoving = false;
    }

    public Vector2 GetMovementVector(Vector2 target, bool moveStraight = false)
    {
        Vector2 targetDir = (target - (Vector2)transform.position).normalized;
        bool avoid = false;
        // We should check for all obstructions around us first
        
        for (int i = 0; i < movementDirections.Count; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position, movementDirections[i], 1f);
            Debug.DrawRay((Vector2)transform.position, movementDirections[i], Color.cyan);
            if (hit)
            {
                Debug.DrawRay((Vector2)transform.position, movementDirections[i], Color.red);
                targetDir = movementDirections[i] * -1;
                avoid = true;
                break;
            }
        }
        List<KeyValuePair<Vector2, float>> directionWeights = new List<KeyValuePair<Vector2, float>>();

        // Calculate dot products
        foreach (Vector2 dir in movementDirections)
        {
            KeyValuePair<Vector2, float> pair = new KeyValuePair<Vector2, float>(dir, avoid ? AvoidObsticle(targetDir, dir) : MoveAround(targetDir, dir, target, moveStraight));
            directionWeights.Add(pair);
        }

        // Sort our weights so the first direction has the best score
        directionWeights.Sort(new KeyValuePairComparer());

        foreach (KeyValuePair<Vector2, float> pair in directionWeights)
        {
            // Check to see if moving in this direction will cause us to hit an obstruction - we dont want this
            RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position, pair.Key, 1.5f);
            if (!hit) return pair.Key;

            Debug.DrawRay((Vector2)transform.position, pair.Key, Color.magenta);
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

    /// <summary>
    /// Is called when we are next to an obsticle and want to avoid it
    /// </summary>
    /// <param name="targetDir"></param>
    /// <param name="dir"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    protected abstract float AvoidObsticle(Vector2 targetDir, Vector2 dir);
}

// A custom comparer class that ensures the largest value of the key value pair appears first in the array
public class KeyValuePairComparer : IComparer<KeyValuePair<Vector2, float>>
{
    int IComparer<KeyValuePair<Vector2, float>>.Compare(KeyValuePair<Vector2, float> x, KeyValuePair<Vector2, float> y)
    {
        if (x.Value > y.Value)
            return -1;
        else if (x.Value == y.Value)
            return 0;
        else
            return 1;
    }
}


public delegate float DirectionWeightFunction(Vector2 targetDir, Vector2 dir);
