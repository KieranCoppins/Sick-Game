using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI Abilities/AOE Ability")]
public class AOEAbility : AbilityBase
{
    [SerializeField] private float _lifespan;
    [Tooltip("The amount of damage to inflict per second")]
    [SerializeField] private int _damageRate;
    [SerializeField] private int _initialDamage;
    [SerializeField] private bool _friendlyFire;

    [SerializeField] private GameObject _AOEGameObject;

    // For AOE abilities we dont actually need a position or direction  - we just need a target
    public override void Cast(Vector2 position, Vector2 direction, Transform target, BaseCharacter caster, float directionalOffset = 0.0f)
    {
        // We could use position and direction for some particle effects to come from the mob casting
        GameObject go = Instantiate(_AOEGameObject, target.position, Quaternion.identity);
        go.GetComponent<AOE>().Initialise(_initialDamage, _lifespan, _friendlyFire, _damageRate, caster);
    }

    public override string GetDescription()
    {
        string description = $"An area of effect ability that deals {_initialDamage} points of initial damage and then {_damageRate} points of damage over {_lifespan} seconds.";
        description += _friendlyFire ? "Does damage to allies" : "Doesn't damage allies";
        return description;
    }
}
