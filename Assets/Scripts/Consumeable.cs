using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Consumeable")]
public class Consumeable : InventoryItem
{
    [Tooltip("The status effects this consumeable will apply to the player")]
    public List<StatusEffect> StatusEffects;

    public override void Use(BaseCharacter character)
    {
        foreach (var statusEffect in StatusEffects)
        {
            character.StartCoroutine(statusEffect.Apply(character));
        }
    }
}
