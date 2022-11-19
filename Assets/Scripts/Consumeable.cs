using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
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

    public override string GetDescription()
    {
        string description = "";
        foreach (var statusEffect in StatusEffects)
        {
            string actionWord = statusEffect.value >= 0 ? "Increases" : "Decreases";
            string timePhrase = "";
            if (statusEffect.duration > 0)
            {
                if ((statusEffect.flags & StatusEffectFlags.shouldHappenOverTime) == StatusEffectFlags.shouldHappenOverTime)
                {
                    timePhrase += $" every second";
                }
                timePhrase += $" for {statusEffect.duration} seconds";
                if ((statusEffect.flags & StatusEffectFlags.shouldReset) == StatusEffectFlags.shouldReset)
                {
                    timePhrase += $" then resets";
                }
            }
            else
            {
                timePhrase = " instantly";
            }
            description += $"{actionWord} {Mathf.Abs(statusEffect.value)} points of {statusEffect.stat}{timePhrase}. ";
        }

        return description;
    }

    public override string ToString() => name;

    public static implicit operator string(Consumeable consumeable) => consumeable.name;
}
