using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KieranCoppins.DecisionTrees;

/// <summary>
/// A custom wrapper for action nodes that automatically assign a Base Mob on initialisation.
/// </summary>
public abstract class CustomAction : Action
{
    protected BaseMob Mob { get; set; }

    public override void Initialise<T>(T metaData)
    {
        base.Initialise(metaData);
        if (metaData is BaseMob)
            Mob = metaData as BaseMob;
    }
}

/// <summary>
/// A custom wrapper for decision nodes that automatically assign a Base Mob on initialisation.
/// </summary>
public abstract class CustomDecision : Decision
{
    protected BaseMob Mob { get; set; }

    public override void Initialise<T>(T metaData)
    {
        base.Initialise(metaData);
        if (metaData is BaseMob)
            Mob = metaData as BaseMob;
    }
}

/// <summary>
/// A custom wrapper for function nodes that automatically assign a Base Mob on initialisation.
/// </summary>
public abstract class CustomFunction<T> : Function<T>
{
    protected BaseMob Mob { get; set; }

    public override void Initialise<TMetadata>(TMetadata metaData)
    {
        base.Initialise(metaData);
        if (metaData is BaseMob)
            Mob = metaData as BaseMob;
    }
}
