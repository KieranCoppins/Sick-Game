using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAbilityBase : MonoBehaviour
{
    [SerializeField] public string Name { get; protected set; }
    [SerializeField] public float AbilityCalldown { get; protected set; }

    protected float LastCast { get; private set; }

    protected virtual void Update()
    {
        LastCast += Time.deltaTime;
    }

    /// <summary>
    /// Casts the ability
    /// </summary>
    public virtual void Cast()
    {
        LastCast = 0;
    }
}
