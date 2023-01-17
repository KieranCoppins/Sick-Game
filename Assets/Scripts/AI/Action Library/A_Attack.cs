using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KieranCoppins.DecisionTrees;

/// <summary>
/// An abstract attack action that all attack actions should inherit
/// </summary>
public abstract class A_Attack : CustomAction
{
    [SerializeField] protected float Cooldown;

    public A_Attack()
    {
        Flags |= ActionFlags.Interruptor;       // This action is an interruptor
        Flags &= ~ActionFlags.Interruptable;    // This action is not interruptable
    }

    protected virtual IEnumerator RunCooldown()
    {
        yield return new WaitForSeconds(Cooldown);
        Mob.CanAttack = true;
    }

    public override string GetDescription(BaseNodeView nodeView)
    {
        return $"Attack the mob's target. The attack has a {Cooldown} second cooldown";
    }
}
