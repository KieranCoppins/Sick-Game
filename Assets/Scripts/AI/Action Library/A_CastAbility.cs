using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

/// <summary>
/// An attack action that casts the mobs given ability
/// </summary>
public class A_CastAbility : A_Attack
{
    [SerializeField] AbilityBase ability;

    Vector2 totalTargetVelocity;
    int totalVelocityEntries = 0;

    public A_CastAbility() : base()
    {
    }

    public override void Initialise(BaseMob mob)
    {
        base.Initialise(mob);
        cooldown = ability.AbilityCooldown;
    }

    public override IEnumerator Execute()
    {
        totalTargetVelocity = mob.Target.GetComponent<Rigidbody2D>().velocity;
        totalVelocityEntries = 1;
        // Predict where the target will be
        Vector2 predictedLocation = PredictLocation();
        Vector2 predictedDirection = predictedLocation - (Vector2)mob.transform.position;

        // Do a ray cast to determine if we should start casting
        RaycastHit2D hit = Physics2D.Raycast((Vector2)mob.transform.position, predictedDirection.normalized, predictedDirection.magnitude);

        if ((mob.debugFlags & DebugFlags.Combat) == DebugFlags.Combat)
            Debug.DrawRay((Vector2)mob.transform.position, predictedDirection, Color.red, 0.5f);

        // We dont get a hit at all, or if we do hit something we hit our target
        if (!hit || (hit && hit.collider.CompareTag(mob.Target.tag)))
        {
            // Then we're good to start casting our ability

            // Stop our velocity
            mob.rb.velocity = Vector2.zero;
            mob.CanAttack = false;

            mob.animator.Play("Attack");
            EmitAlert.Emit(mob.transform.position, 10f);
            float timer = ability.CastTime;
            yield return null;

            // Whilst we are casting, we want to get the average of our player's movement vector
            yield return new DoTaskWhilstWaitingUntil(() => {
                if (mob.Target != null)
                {
                    totalTargetVelocity += mob.Target.GetComponent<Rigidbody2D>().velocity;
                    totalVelocityEntries += 1;
                }
            }, 
            () =>
            {
                timer -= Time.deltaTime;
                return !mob.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") || !mob.Target || timer <= 0f;
            });

            if (!mob.Target)
            {
                mob.CanAttack = true;
                yield return null;
            }
            else
            {
                // Recalculate the predicted movement at the last second since the player may have changed direction during our casting time
                predictedLocation = PredictLocation();
                predictedDirection = predictedLocation - (Vector2)mob.transform.position;

                ability.Cast(mob.transform.position, predictedDirection, mob.Target, mob);
                mob.StartCoroutine(Cooldown());
            }
        }

        yield return new WaitUntil(() => !mob.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"));
    }

    Vector2 PredictLocation()
    {
        // Check if our ability is a projectile ability
        ProjectileAbility projectileAbility = ability as ProjectileAbility;
        if (projectileAbility != null)
        {
            // Get the average velocity of our target
            Vector2 velocityVector = totalTargetVelocity / totalVelocityEntries;

            // Calculate how far away our target is
            float distanceFromAI = Vector2.Distance(mob.transform.position, mob.Target.position);

            // Using our targets position, the velocity they're moving and the velocity of our projectile, determine where they would be
            Vector2 predictedLocation = (Vector2)mob.Target.position + (velocityVector * (distanceFromAI / projectileAbility.GetProjectileVelocity()));

            return predictedLocation;
        }

        return mob.Target.position;
    }
}
