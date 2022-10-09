using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBoss : BaseBoss
{
    // In theory we should always call the attack function with a boss. Or set their range to be rather large
    // Since once we encounter a boss which would start off a trigger when entering the boss "arena" the boss
    // should be attacking the player always

    /*
    public override void Attack(GameObject target)
    {
        if (attackTimer < attackRate)
            return;

        if (HasLineOfSight(target.transform.position))
        {
            StopMoving();

            // We need to determine what ability we want to casts
            float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);

            stages[CurrentStage].abilities[2].Cast(transform, target.transform);

            // If the player is far away we shall cast comet
            if (distanceToTarget > 8.0f)
            {
                stages[CurrentStage].abilities[0].Cast(transform, target.transform);
            }
            // Otherwise we'll try cast comet wave
            else
            {
                stages[CurrentStage].abilities[1].Cast(transform, target.transform);
            }
        }
        else
        {
            ResumeMoving();
        }
    }*/

    protected override float MoveAround(Vector2 targetDir, Vector2 dir, Vector2 target, bool moveStraight)
    {
        throw new System.NotImplementedException();
    }
}
