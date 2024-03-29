using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI Abilities/Projectile Ability")]
public class ProjectileAbility : AbilityBase
{
    [Tooltip("Our projectile base game object")]
    [SerializeField] private GameObject _projectile;

    [Tooltip("The angle of the cone infront of the caster in which to cast the number of projectiles")]
    [SerializeField] private float _angle;

    [Tooltip("The number of projectiles to spawn around the caster")]
    [SerializeField] private int _numberProjectiles;

    [Tooltip("The velocity at which the projectile travels")]
    [SerializeField] private float _projectileVelocity;

    [Tooltip("The amount of damage our projectile should do when it hits something")]
    [SerializeField] private int _damage;

    public override void Cast(Vector2 position, Vector2 direction, Transform target, BaseCharacter caster, float directionalOffset = 1.0f)
    {
        Vector2 minDirection = Quaternion.AngleAxis(-(_angle / 2), new Vector3(0, 0, 1)) * direction;
        float angleStep = _angle / _numberProjectiles;
        for (int i = 0; i < _numberProjectiles; i++)
        {
            Vector2 projectileDirection = Quaternion.AngleAxis(angleStep * i, new Vector3(0, 0, 1)) * minDirection;
            GameObject go = Instantiate(_projectile, position + projectileDirection.normalized * directionalOffset, Quaternion.identity);
            Physics2D.IgnoreCollision(go.GetComponent<Collider2D>(), caster.GetComponent<Collider2D>(), true);
            go.transform.up = projectileDirection.normalized;
            Projectile projectileComp = go.GetComponent<Projectile>();
            projectileComp.Target = target;
            projectileComp.SetVelocity(_projectileVelocity);
            projectileComp.Damage = _damage;
            projectileComp.Caster = caster;
        }
    }

    public override string GetDescription()
    {
        string desc = $"Fires {_numberProjectiles} projectile(s) within a {_angle} cone. Each projectile dealing {_damage} points of damage.";
        desc += _projectile.GetComponent<Projectile>().Type == ProjectileType.Guided ? " This projectile is guided to target." : "";
        return desc;
    }

    public float GetProjectileVelocity() { return _projectileVelocity; }
}
