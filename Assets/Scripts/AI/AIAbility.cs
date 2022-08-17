using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AIAbility
{
    [HideInInspector] public bool canCast { get; private set; }
    public AbilityBase ability;

    public AIAbility()
    {
        canCast = true;
    }

    public bool Cast(BaseMob castFrom, Transform castTo)
    {
        if (!canCast)
            return false;
        Vector2 direction = (castTo.position - castFrom.transform.position).normalized;
        ability.Cast(castFrom.transform.position, direction, castTo.transform);
        castFrom.StartCoroutine(Cooldown());
        return true;
    }

    public IEnumerator Cooldown()
    {
        canCast = false;
        yield return new WaitForSeconds(ability.AbilityCooldown);
        canCast = true;
    }
}
