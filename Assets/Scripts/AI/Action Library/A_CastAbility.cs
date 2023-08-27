using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using KieranCoppins.DecisionTrees;

/// <summary>
/// An attack action that casts the mobs given ability
/// </summary>
public class A_CastAbility : A_Attack
{
    [SerializeField] private AbilityBase _ability;

    private Vector2 _totalTargetVelocity;
    private int _totalVelocityEntries = 0;

    public A_CastAbility() : base()
    {
    }

    public override void Initialise<T>(T metaData)
    {
        base.Initialise(metaData);
        Cooldown = _ability.AbilityCooldown;
    }

    public override IEnumerator Execute()
    {
        _totalTargetVelocity = Mob.Target.GetComponent<Rigidbody2D>().velocity;
        _totalVelocityEntries = 1;
        // Predict where the target will be
        Vector2 predictedLocation = PredictLocation();
        Vector2 predictedDirection = predictedLocation - (Vector2)Mob.transform.position;

        // Do a ray cast to determine if we should start casting
        RaycastHit2D hit = Physics2D.Raycast((Vector2)Mob.transform.position, predictedDirection.normalized, predictedDirection.magnitude);

        if ((Mob.DebugFlags & DebugFlags.Combat) == DebugFlags.Combat)
            Debug.DrawRay((Vector2)Mob.transform.position, predictedDirection, Color.red, 0.5f);

        // We dont get a hit at all, or if we do hit something we hit our target
        if (!hit || (hit && hit.collider.CompareTag(Mob.Target.tag)))
        {
            // Then we're good to start casting our ability

            // Stop our velocity
            Mob.RigidBody.velocity = Vector2.zero;
            Mob.CanAttack = false;

            Mob.Animator.Play("Attack");
            EmitAlert.Emit(Mob.transform.position, 10f);
            float timer = _ability.CastTime;
            yield return null;

            // Whilst we are casting, we want to get the average of our player's movement vector
            yield return new DoTaskWhilstWaitingUntil(() =>
            {
                if (Mob.Target != null)
                {
                    _totalTargetVelocity += Mob.Target.GetComponent<Rigidbody2D>().velocity;
                    _totalVelocityEntries += 1;
                }
            },
            () =>
            {
                timer -= Time.deltaTime;
                return !Mob.Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") || !Mob.Target || timer <= 0f;
            });

            if (!Mob.Target)
            {
                Mob.CanAttack = true;
                yield return null;
            }
            else
            {
                // Recalculate the predicted movement at the last second since the player may have changed direction during our casting time
                predictedLocation = PredictLocation();
                predictedDirection = predictedLocation - (Vector2)Mob.transform.position;

                _ability.Cast(Mob.transform.position, predictedDirection, Mob.Target, Mob);
                Mob.StartCoroutine(RunCooldown());
            }
        }

        yield return new WaitUntil(() => !Mob.Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"));
    }

    Vector2 PredictLocation()
    {
        // Check if our ability is a projectile ability
        ProjectileAbility projectileAbility = _ability as ProjectileAbility;
        if (projectileAbility != null)
        {
            // Get the average velocity of our target
            Vector2 velocityVector = _totalTargetVelocity / _totalVelocityEntries;

            // Calculate how far away our target is
            float distanceFromAI = Vector2.Distance(Mob.transform.position, Mob.Target.position);

            // Using our targets position, the velocity they're moving and the velocity of our projectile, determine where they would be
            Vector2 predictedLocation = (Vector2)Mob.Target.position + (velocityVector * (distanceFromAI / projectileAbility.GetProjectileVelocity()));

            return predictedLocation;
        }

        return Mob.Target.position;
    }

    public override string GetDescription(BaseNodeView nodeView)
    {
        return $"Casts {_ability?.name} at the mob's target with a {Cooldown} second cooldown.";
    }

    public override string GetTitle()
    {
        if (_ability != null)
            return $"Cast {_ability.name}";

        return base.GetTitle();
    }
}
