using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PathfindingComponent))]
[RequireComponent(typeof(Rigidbody2D))]
[DisallowMultipleComponent]
public class BaseMob : MonoBehaviour
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
    [SerializeField] protected int maxHealth = 10;
    [SerializeField] protected int damage = 1;
    [SerializeField] protected float movementSpeed = 2;
    [SerializeField] public string mobName { get; protected set; }

    protected PathfindingComponent PathfindingComponent;
    protected Rigidbody2D rb;

    protected Queue<Vector2> path;

    private int _health;

    Vector2 movementDirection;

    Vector2 desiredPosition;

    bool hasPath;

    [Header("DEBUG VALUES")]
    [SerializeField] Vector2 debugPosition;


    public void TakeDamage(int dmg)
    {
        Health -= dmg;

        // Invoke our take damage event when we take damage
        onTakeDamage?.Invoke();
    }

    public void Heal(int amount)
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
        MoveTo();
    }

    // Sets the desired position to the next item in the queue
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
        path = new Queue<Vector2>();

        movementDirection = Vector2.zero;

    }

    // Implement basic movement following the path as default movement
    void Update()
    {
        if (Vector2.Distance(transform.position, desiredPosition) <= 0.1f)
        {
            MoveTo();
        }
        if (hasPath)
        {
            movementDirection = desiredPosition - (Vector2)transform.position;
        }
        rb.velocity = movementDirection.normalized * movementSpeed;
    }

    public void DEBUG_SetPosition()
    {
        CalculatePath(debugPosition);
        Vector2[] pathArray = path.ToArray();
        Debug.DrawLine(desiredPosition, pathArray[0], Color.red, 50000.0f);
        for (int i = 1; i < pathArray.Length; i++)
        {
            Debug.DrawLine(pathArray[i-1], pathArray[i], Color.red, 50000.0f);
        }
    }
}
