using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KieranCoppins.DecisionTrees;

public class A_SetCombatMode : CustomAction
{
    [SerializeField] private CombatState _state;

    public A_SetCombatMode() : base()
    {

    }

    public override IEnumerator Execute()
    {
        Mob.State = _state;
        if (_state == CombatState.Combat || _state == CombatState.Investigate)
        {
            Mob.Animator.SetBool("InCombat", true);
        }
        else
        {
            Mob.Animator.SetBool("InCombat", false);
        }
        yield return null;
    }

    public override string GetDescription(BaseNodeView nodeView)
    {
        return $"Set the mobs combat state to {_state}";
    }
}