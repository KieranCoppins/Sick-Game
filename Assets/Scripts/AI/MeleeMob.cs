using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeMob : BaseMob
{

    protected override void Update()
    {
        base.Update();
    }

    protected override float MoveTowards(Vector2 targetDir, Vector2 dir, Vector2 target)
    {
        throw new System.NotImplementedException();
    }

    protected override float AvoidTarget(Vector2 targetDir, Vector2 dir, Vector2 target)
    {
        throw new System.NotImplementedException();
    }
}
