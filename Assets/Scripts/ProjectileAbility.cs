using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI Abilities/Projectile Ability")]
public class ProjectileAbility : AbilityBase
{
    [SerializeField] GameObject projectile;
    [SerializeField] float angle;
    [SerializeField] int numberProjectiles;
    public override void Cast(Vector2 position, Vector2 direction, Transform target)
    {
        Debug.Log("Casting Projectile Ability");
        Vector2 minDirection = Quaternion.AngleAxis(-(angle/2), new Vector3(0, 0, 1)) * direction;
        float angleStep = angle / numberProjectiles;
        for (int i = 0; i < numberProjectiles; i++)
        {
            Vector2 projectileDirection = Quaternion.AngleAxis(angleStep * i, new Vector3(0, 0, 1)) * minDirection;
            GameObject go = Instantiate(projectile, position + projectileDirection.normalized * 1.0f, Quaternion.identity);
            go.transform.up = projectileDirection.normalized;
            go.GetComponent<Projectile>().target = target;
        }
    }
}
