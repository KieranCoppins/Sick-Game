using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PathfindingComponent))]
[RequireComponent(typeof(Rigidbody2D))]
[DisallowMultipleComponent]
public abstract class BaseMob : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent onTakeDamage;
    public UnityEvent onHeal;
    public int Health {
        get
        {
            return _health;
        }
        protected set
        {
            _health = value;
            if (_health > maxHealth)
                _health = maxHealth;
            if (_health < 0)
                _health = 0;
        }
    }
    [Header("Mob Stats")]

    [Tooltip("The maximum health of the mob")]
    [SerializeField] protected int maxHealth = 10;
    [Tooltip("How much damage on attack does this mob deal?")]
    [SerializeField] protected int damage = 1;
    [SerializeField] protected float movementSpeed = 2;
    [Tooltip("How often in seconds can this mob attack?")]
    [SerializeField] protected float attackRate = 2;
    [SerializeField] public string mobName { get; protected set; }

    protected PathfindingComponent PathfindingComponent;
    protected Rigidbody2D rb;

    protected Queue<Vector2> path;

    protected float attackTimer;

    private int _health;

    Vector2 movementDirection;
    Vector2 desiredPosition;
    Vector2 targetPosition;

    bool hasPath;
    bool stopMoving = false;

    [Header("DEBUG VALUES")]
    [SerializeField] bool DebugMode;
    [SerializeField] Transform debugTarget;

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

    /// <summary>
    /// Calculates the path to position and adds all waypoints in path to a queue
    /// </summary>
    /// <param name="position"></param>
    protected void CalculatePath(Vector2 position)
    {
        Vector2[] p = PathfindingComponent.CalculateAStarPath(transform.position, position);
        if (p == null)
        {
            Debug.LogError("Cannot produce a path to " + position.ToString());
            return;
        }
        path = new Queue<Vector2>();
        foreach(Vector2 pos in p)
        {
            path.Enqueue(pos);
        }
        MoveTo();
    }

    void MoveTo()
    {
        if (path.Count == 0)
        {
            movementDirection = Vector2.zero;
            hasPath = false;
        }
        else
        {
            hasPath = true;
            desiredPosition = path.Dequeue();
        }
    }

    protected void Awake()
    {
        Health = maxHealth;
        PathfindingComponent = GetComponent<PathfindingComponent>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        path = new Queue<Vector2>();

    }

    // Implement basic movement following the path as default movement
    protected virtual void Update()
    {
        if (Vector2.Distance(transform.position, desiredPosition) <= 0.1f)
        {
            MoveTo();
        }
        if (hasPath)
        {
            movementDirection = desiredPosition - (Vector2)transform.position;
        }
        rb.velocity = stopMoving ? Vector2.zero : movementDirection.normalized * movementSpeed;
        CalculateNextPosition();
        attackTimer += Time.deltaTime;
    }

    public void DEBUG_SetPosition()
    {
        CalculatePath(debugTarget.position);
        DEBUG_DrawPath();
    }

    void DEBUG_DrawPath()
    {

        Vector2[] pathArray = path.ToArray();
        Debug.DrawLine(desiredPosition, pathArray[0], Color.red, 20.0f);
        for (int i = 1; i < pathArray.Length; i++)
        {
            Debug.DrawLine(pathArray[i - 1], pathArray[i], Color.red, 20.0f);
        }
    }

    /// <summary>
    /// Attack the target - should be overwritten for child classes
    /// </summary>
    public abstract void Attack(GameObject target);

    /// <summary>
    /// Checks if the mob has a straight line of sight to position
    /// </summary>
    /// <param name="position"></param>
    /// <returns>True if the mob has line of sight</returns>
    protected bool HasLineOfSight(Vector2 position)
    {
        RaycastHit2D hit;
        Vector3 direction = position - (Vector2)transform.position;
        hit = Physics2D.Raycast(transform.position, direction);
        if (DebugMode)
            Debug.DrawRay(transform.position, direction);
        if (hit.collider.CompareTag("Player"))
            return true;

        return false;
    }

    /// <summary>
    /// Calculate our next position. This function contains the logic on where our next position should be and calculates a path to this position
    /// </summary>
    public virtual void CalculateNextPosition()
    {
        // By default we'll just pathfind to the player
        Vector2 newTargetPosition = GameObject.FindGameObjectWithTag("Player").transform.position;

        // Check if our target position changed
        if (Vector2.Distance(newTargetPosition, targetPosition) > 2.0f)
        {
            CalculatePath(newTargetPosition);
            targetPosition = newTargetPosition;
            if (DebugMode)
                DEBUG_DrawPath();
        }
    }

    public void StopMoving()
    {
        stopMoving = true;
    }

    public void ResumeMoving()
    {
        stopMoving = false;
    }
}
