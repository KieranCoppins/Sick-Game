using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An attack action that melees the target
/// </summary>
public class A_Melee : A_Attack
{
    public A_Melee()
    {

    }
    public A_Melee(Transform target, float attackSpeed) : base(target, attackSpeed)
    {
    }
    public override IEnumerator Execute()
    {
        CanCast = false;

        // We should play some kind of attack animation
        // We can do a sphere overlap cast to determine colliders where the melee weapon is.
        // We can then put an event in the animation to deal damage to all the colliders in the overlap check
        // This does require characters and animations to be included!

        mob.StartCoroutine(Cooldown());
        yield return null;
    }
}
