using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class AbilityBase : ScriptableObject
{
    public Sprite icon;

    [SerializeField] public float AbilityCooldown;
    [SerializeField] public float CastTime;
    [SerializeField] public float Range;

    /// <summary>
    /// Casts the ability
    /// </summary>
    public abstract void Cast(Vector2 position, Vector2 direction, Transform target);

    public abstract string GetDescription();
}
