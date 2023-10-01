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
    public ProjectileType Type;

    [Tooltip("The target to guide to")]
    public Transform Target;

    [Tooltip("The rotation speed at which the projectile rotates")]
    [SerializeField] private float RotationSpeed;

    [Tooltip("How long in seconds before the projectile destroys itself")]
    [SerializeField] private float Lifespan;

    [SerializeField] private UnityEvent _onAwake;
    [SerializeField] private UnityEvent _onDeath;

    private Rigidbody2D _rigidBody;
    private float _aliveTime = 0;
    private float _velocity = 0;

    public int Damage;

    public BaseCharacter Caster;


    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _rigidBody.gravityScale = 0;
        _onAwake?.Invoke();
    }

    void Update()
    {
        // If we are a guided type, we want to rotate the projectile towards the target
        if (Type == ProjectileType.Guided)
        {
            Vector2 directionToTarget = (Target.position - transform.position).normalized;
            Vector2 rot = Vector2.Lerp(transform.up, directionToTarget, Time.deltaTime * RotationSpeed);
            transform.up = rot;
        }
        Vector2 desiredVelocity = transform.up * _velocity;
        _rigidBody.velocity = desiredVelocity;

        _aliveTime += Time.deltaTime;
        if (_aliveTime >= Lifespan)
        {
            _onDeath?.Invoke();
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        BaseCharacter character = collision.collider.GetComponent<BaseCharacter>();
        if (character == Caster) return;
        if (character)
        {
            character.TakeDamage(Caster, Damage);
        }
        _onDeath?.Invoke();
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        // Put projectile destroy animations here
    }

    public void SetVelocity(float value) { _velocity = value; }
}

public enum ProjectileType
{
    Guided,
    Direct
}
