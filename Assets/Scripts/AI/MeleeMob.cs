using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeMob : BaseMob
{

    protected override void Update()
    {
        base.Update();
    }

    protected override float MoveAround(Vector2 targetDir, Vector2 dir, Vector2 target, bool moveStraight)
    {
        throw new System.NotImplementedException();
    }

    protected override float AvoidObsticle(Vector2 targetDir, Vector2 dir)
    {
        throw new System.NotImplementedException();
    }
}
