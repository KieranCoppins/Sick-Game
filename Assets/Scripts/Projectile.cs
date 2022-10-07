using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(SpriteRenderer))]
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
    

    Rigidbody2D rb;
    float aliveTime = 0;
    float velocity = 0;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
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
            Destroy(this.gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            // Deal damage to player here
        }
        Destroy(this.gameObject);
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
