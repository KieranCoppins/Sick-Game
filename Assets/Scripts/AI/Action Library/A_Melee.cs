using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An attack action that melees the target
/// </summary>
public class A_Melee : A_Attack
{
    [SerializeField] float attackSpeed;

    public A_Melee() : base()
    {

    }

    public override void Initialise(BaseMob mob)
    {
        base.Initialise(mob);

        cooldown = attackSpeed;

    }


    public override IEnumerator Execute()
    {
        mob.CanAttack = false;

        // We should play some kind of attack animation
        // We can do a sphere overlap cast to determine colliders where the melee weapon is.
        // We can then put an event in the animation to deal damage to all the colliders in the overlap check
        // This does require characters and animations to be included!
        mob.animator.Play("Attack");
        EmitAlert.Emit(mob.transform.position, 10f);
        yield return null;

        yield return new WaitUntil(() => { return !mob.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"); });

        mob.StartCoroutine(Cooldown());
        yield return null;
    }

    public override string GetDescription()
    {
        return "Melee's the mob's target.";
    }
}
