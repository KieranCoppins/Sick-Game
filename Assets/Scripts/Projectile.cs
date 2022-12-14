using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [Tooltip("Guided missiles rotate to face the target")]
    public ProjectileType type;

    [Tooltip("The target to guide to")]
    public Transform target;

    [Tooltip("The rotation speed at which the projectile rotates")]
    [SerializeField] float rotationSpeed;

    [Tooltip("How long in seconds before the projectile destroys itself")]
    [SerializeField] float lifespan;

    [SerializeField] UnityEvent OnAwake;
    [SerializeField] UnityEvent OnDeath;
    
    Rigidbody2D rb;
    float aliveTime = 0;
    float velocity = 0;

    public int damage;

    public BaseCharacter caster;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        OnAwake?.Invoke();
    }

    void Update()
    {
        // If we are a guided type, we want to rotate the projectile towards the target
        if (type == ProjectileType.Guided)
        {
            Vector2 directionToTarget = (target.position - transform.position).normalized;
            Vector2 rot = Vector2.Lerp(transform.up, directionToTarget, Time.deltaTime * rotationSpeed);
            transform.up = rot;
        }
        Vector2 desiredVelocity = transform.up * velocity;
        rb.velocity = desiredVelocity;

        aliveTime += Time.deltaTime;
        if (aliveTime >= lifespan)
        {
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        BaseCharacter character = collision.collider.GetComponent<BaseCharacter>();
        if (character)
        {
            character.TakeDamage(caster, damage);
        }
        OnDeath?.Invoke();
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        // Put projectile destroy animations here
    }

    public void SetVelocity(float value) { velocity = value; }
}

public enum ProjectileType
{
    Guided,
    Direct
}
