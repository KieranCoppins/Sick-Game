using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KieranCoppins.DecisionTrees;

/// <summary>
/// An attack action that melees the target
/// </summary>
public class A_Melee : A_Attack
{
    [SerializeField] float attackSpeed;

    public override void Initialise<T>(T metaData)
    {
        base.Initialise(metaData);
        Cooldown = attackSpeed;
    }


    public override IEnumerator Execute()
    {
        Mob.CanAttack = false;

        // We should play some kind of attack animation
        // We can do a sphere overlap cast to determine colliders where the melee weapon is.
        // We can then put an event in the animation to deal damage to all the colliders in the overlap check
        // This does require characters and animations to be included!
        Mob.Animator.Play("Attack");
        EmitAlert.Emit(Mob.transform.position, 10f);
        yield return null;

        yield return new WaitUntil(() => { return !Mob.Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"); });

        Mob.StartCoroutine(RunCooldown());
        yield return null;
    }

    public override string GetDescription(BaseNodeView nodeView)
    {
        return "Melee's the mob's target.";
    }
}
