using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedMob : BaseMob
{
    [Header("Ranged Mob Attributes")]
    [SerializeField] GameObject projectile;

    public override void Attack(GameObject target)
    {
        // Check if we have line of sight to our target
        if (HasLineOfSight(target.transform.position))
        {
            // If we do then we have to cancel movement and fire projectile
            StopMoving();

            if (attackTimer < attackRate)
                return;

            attackTimer = 0;

            Vector3 direction = (target.transform.position - transform.position).normalized;

            // Spawn our projectile
            GameObject go = Instantiate(projectile, transform.position + direction * 1f, Quaternion.identity);

            // Snap our projectile to face the right direction
            go.transform.up = direction;

            go.GetComponent<Projectile>().target = target.transform;

        }
        else
        {
            ResumeMoving();
        }
    }
}
