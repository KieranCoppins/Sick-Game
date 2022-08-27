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
            Debug.DrawRay(lowerStart, position - lowerStart, Color.magenta, 0.5f);
            Debug.DrawRay(upperStart, position - upperStart, Color.magenta, 0.5f);
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
}
