using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_CombatMode : F_Condition
{
    [SerializeField] CombatState state;
    public override void Initialise(BaseMob mob)
    {
        base.Initialise(mob);
    }

    public override bool Invoke()
    {
        return mob.State == state;
    }
}