using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KieranCoppins.DecisionTrees;

public class C_CombatMode : CustomFunction<bool>
{
    [SerializeField] private CombatState _state;

    public override bool Invoke()
    {
        return Mob.State == _state;
    }

    public override string GetSummary(BaseNodeView nodeView)
    {
        return $"the mob's combat state is {_state}";
    }

    public override string GetDescription(BaseNodeView nodeView)
    {
        return $"Returns true if {GetSummary(nodeView)}.";
    }
}