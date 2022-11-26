using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_SetCombatMode : Action
{
    [SerializeField] CombatState state;
    public A_SetCombatMode() : base()
    {
        
    }

    public override void Initialise(BaseMob mob)
    {
        base.Initialise(mob);
    }

    public override IEnumerator Execute()
    {
        mob.State = state;
        yield return null;
    }
}