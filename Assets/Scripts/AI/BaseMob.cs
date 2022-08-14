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

    Vector2 movementDirection;
    Vector2 desiredPosition;
    Vector2 targetPosition;

    bool hasPath;
    bool stopMoving = false;

    protected ActionManager actionManager = new ActionManager();
    protected DecisionTree decisionTree;

    public delegate void OnFinishDelegate(IEnumerator coroutine);
    public event OnFinishDelegate onFinish;

    bool executingActions = false;

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

    protected virtual void Start()
    {
        Health = MaxHealth;
        PathfindingComponent = GetComponent<PathfindingComponent>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        onFinish += delegate (IEnumerator coroutine)
        {
            // Remove this action from current actions
            actionManager.currentActions.Remove(coroutine);

            // Check if we have any more actions in the current actions
            if (actionManager.currentActions.Count == 0)
                executingActions = false;
        };
    }

    // Implement basic movement following the path as default movement
    protected virtual void Update()
    {

        // If we aren't executing any actions currently we should pull from the queue and execute them
        if (!executingActions)
        {
            actionManager.Execute();
            ExecuteActions();
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

    public void StopMoving()
    {
        stopMoving = true;
    }

    public void ResumeMoving()
    {
        stopMoving = false;
    }

    IEnumerator ActionWrapper(IEnumerator coroutine)
    {
        bool running = true;
        IEnumerator e = coroutine;
        while (running)
        {
            if (e != null && e.MoveNext())
                yield return e.Current;
            else
                running = false;
        }
        OnFinishDelegate handler = onFinish;
        if (handler != null)
            handler(coroutine);
    }

    void ExecuteActions()
    {
        executingActions = true;
        // Execute all actions in current actions
        foreach (IEnumerator action in actionManager.currentActions)
        {
            StartCoroutine(ActionWrapper(action));
        }
    }
}
