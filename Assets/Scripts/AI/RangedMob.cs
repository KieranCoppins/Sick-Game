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

        if (moveStraight)
            return Vector2.Dot(targetDir, dir);

        // Move away from the target if too close
        if (dist < Ability.Range - 3.0f)
            return 1f - Mathf.Abs(Vector2.Dot(targetDir * -1f, dir) - 0.65f) + Vector2.Dot(RigidBody.velocity.normalized, dir);  // We add the dot product of our current velocity so that we try and favor where we are currently going - prevents random switches in direction

        // Circle the target if in range
        else if (dist < Ability.Range - 1.0f)
            return 1.0f - Mathf.Abs(Vector2.Dot(targetDir, dir)) + Vector2.Dot(RigidBody.velocity.normalized, dir);

        // Otherwise move towards the target
        return Vector2.Dot(targetDir, dir) + Vector2.Dot(RigidBody.velocity.normalized, dir);
    }
}
