using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class AbilityBase : ScriptableObject
{
    public Sprite Icon;

    [SerializeField] public float AbilityCooldown;
    [SerializeField] public float CastTime;
    [SerializeField] public float Range;
    [SerializeField] public int ManaCost;

    /// <summary>
    /// Casts the ability
    /// </summary>
    public abstract void Cast(Vector2 position, Vector2 direction, Transform target, BaseCharacter caster, float directionalOffset = 1.0f);

    public abstract string GetDescription();
}
