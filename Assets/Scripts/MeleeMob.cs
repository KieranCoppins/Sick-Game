using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeMob : BaseMob
{

    protected override void Update()
    {
        base.Update();
    }
    public override void Attack(GameObject target)
    {
        if (attackTimer < attackRate)
            return;

        attackTimer = 0;
        
        // Deal damage to player here - also play any animations
    }
}
