using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class AbilityBase : ScriptableObject
{
    [SerializeField] public float AbilityCooldown;

    /// <summary>
    /// Casts the ability
    /// </summary>
    public abstract void Cast(Vector2 position, Vector2 direction, Transform target);
}
