using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedMob : BaseMob
{
    [Header("Ranged Mob Attributes")]
    [SerializeField] public AbilityBase Ability;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override float MoveAround(Vector2 targetDir, Vector2 dir, Vector2 target, bool moveStraight)
    {
        float dist = Vector2.Distance(target, transform.position);

        if (!moveStraight)
        {
            // Move away from the target if too close
            if (dist < Ability.Range - 1.5f)
                return MovementCurve.Evaluate(Vector2.Dot(targetDir * -1f, dir)) + Vector2.Dot(RigidBody.velocity.normalized, dir);  // We add the dot product of our current velocity so that we try and favor where we are currently going - prevents random switches in direction

            // Move towards the target
            else if (dist < Ability.Range - 0.5f)
                return MovementCurve.Evaluate(Vector2.Dot(targetDir, dir)) + Vector2.Dot(RigidBody.velocity.normalized, dir);
        }

        // Otherwise move directly
        return Vector2.Dot(targetDir, dir);
    }
}
