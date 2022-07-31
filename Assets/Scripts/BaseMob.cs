using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PathfindingComponent))]
[RequireComponent(typeof(Rigidbody2D))]
public class BaseMob : MonoBehaviour
{
    [Header("Events")]
    [Tooltip("This event calls when the mob takes damage")]
    public UnityEvent onTakeDamage;
    [Tooltip("This event calls when the mob hals")]
    public UnityEvent onHeal;
    public int Health {
        get
        {
            return Health;
        }
        protected set
        {
            Health = value;
            if (Health > maxHealth)
                Health = maxHealth;
            if (Health < 0)
                Health = 0;
        }
    }
    [Header("Mob Stats")]
    [SerializeField] protected int maxHealth = 10;
    [SerializeField] protected int damage = 1;
    [SerializeField] protected float movementSpeed = 2;
    [SerializeField] public string mobName { get; protected set; }

    protected PathfindingComponent PathfindingComponent;
    protected Rigidbody2D rb;

    protected Queue<Vector2> path;

    Vector2 desiredPosition;


    protected void TakeDamage(int dmg)
    {
        Health -= dmg;

        // Invoke our take damage event when we take damage
        onTakeDamage?.Invoke();
    }

    protected void Heal(int amount)
    {
        Health += amount;
        onHeal?.Invoke();
    }

    // Calculates the path to position and adds them to a queue
    protected void CalculatePath(Vector2 position)
    {
        Vector2[] p = PathfindingComponent.CalculateAStarPath(transform.position, position);
        path = new Queue<Vector2>();
        foreach(Vector2 pos in p)
        {
            path.Enqueue(pos);
        }
    }

    // Sets the desired position to the next item in the queue
    void MoveTo()
    {
        desiredPosition = path.Dequeue();
    }

    protected void Awake()
    {
        Health = maxHealth;
        PathfindingComponent = GetComponent<PathfindingComponent>();
        rb = GetComponent<Rigidbody2D>();

    }

    // Implement basic movement following the path as default movement
    void Update()
    {
        if (Vector2.Distance(transform.position, desiredPosition) < 0.1f)
        {
            MoveTo();
        }
        Vector2 direction = desiredPosition - (Vector2)transform.position;
        rb.velocity = direction.normalized * movementSpeed;
    }
}
