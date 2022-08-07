using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI Abilities/AOE Ability")]
public class AOEAbility : AbilityBase
{
    [SerializeField] float lifespan;
    [SerializeField] float damageRate;
    [SerializeField] int initialDamage;
    [SerializeField] int continousDamage;
    [SerializeField] bool friendlyFire;

    [SerializeField] GameObject AOEGameObject;

    // For AOE abilities we dont actually need a position or direction  - we just need a target
    public override void Cast(Vector2 position, Vector2 direction, Transform target)
    {
        Debug.Log("Casting AOE ability");
        // We could use position and direction for some particle effects to come from the mob casting
        GameObject go = Instantiate(AOEGameObject, target.position, Quaternion.identity);
        go.GetComponent<AOE>().Initialise(initialDamage, continousDamage, lifespan, friendlyFire, damageRate);
    }
}
