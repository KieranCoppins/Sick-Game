using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AIAbility
{
    [HideInInspector] public float LastCast;
    public AbilityBase ability;

    public bool Cast(Transform castFrom, Transform castTo)
    {
        if (LastCast < ability.AbilityCooldown)
            return false;

        LastCast = 0;
        Vector2 direction = (castTo.position - castFrom.position).normalized;
        ability.Cast(castFrom.position, direction, castTo.transform);
        return true;
    }
}
