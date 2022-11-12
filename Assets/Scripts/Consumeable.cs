using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Stat
{
    Health,
    Stamina,
    Mana,
    MaxHealth,
    MaxStamina,
    MaxMana,
}

[CreateAssetMenu(menuName = "Consumeable")]
public class Consumeable : ScriptableObject
{
    public List<StatusEffect> StatusEffects;

    public Consumeable()
    {

    }

    public void Consume(CharacterMovement character)
    {
        Debug.Log(character);
        foreach (var statusEffect in StatusEffects)
        {
            character.StartCoroutine(statusEffect.Apply(character));
        }
    }
}
