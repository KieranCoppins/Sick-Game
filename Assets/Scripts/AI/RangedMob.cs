using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedMob : BaseMob
{
    [Header("Ranged Mob Attributes")]
    [SerializeField] AbilityBase ability;

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

            ability.Cast(transform.position, direction, target.transform);

        }
        else
        {
            ResumeMoving();
        }
    }
}
