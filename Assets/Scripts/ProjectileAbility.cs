using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI Abilities/Projectile Ability")]
public class ProjectileAbility : AbilityBase
{
    [Tooltip("Our projectile base game object")]
    [SerializeField] GameObject projectile;

    [Tooltip("The angle of the cone infront of the caster in which to cast the number of projectiles")]
    [SerializeField] float angle;

    [Tooltip("The number of projectiles to spawn around the caster")]
    [SerializeField] int numberProjectiles;

    [Tooltip("The velocity at which the projectile travels")]
    [SerializeField] float projectileVelocity;

    [Tooltip("The amount of damage our projectile should do when it hits something")]
    [SerializeField] int damage;

    public override void Cast(Vector2 position, Vector2 direction, Transform target)
    { 
        Vector2 minDirection = Quaternion.AngleAxis(-(angle/2), new Vector3(0, 0, 1)) * direction;
        float angleStep = angle / numberProjectiles;
        for (int i = 0; i < numberProjectiles; i++)
        {
            Vector2 projectileDirection = Quaternion.AngleAxis(angleStep * i, new Vector3(0, 0, 1)) * minDirection;
            GameObject go = Instantiate(projectile, position + projectileDirection.normalized * 1.0f, Quaternion.identity);
            go.transform.up = projectileDirection.normalized;
            Projectile projectileComp = go.GetComponent<Projectile>();
            projectileComp.target = target;
            projectileComp.SetVelocity(projectileVelocity);
            projectileComp.damage = damage;
        }
    }

    public override string GetDescription()
    {
        string desc = $"Fires {numberProjectiles} projectile(s) within a {angle} cone. Each projectile dealing {damage} points of damage.";
        desc += projectile.GetComponent<Projectile>().type == ProjectileType.Guided ? " This projectile is guided to target." : "";
        return desc;
    }

    public float GetProjectileVelocity() { return projectileVelocity; }
}
