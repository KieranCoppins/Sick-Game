using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Replace function template type with the return type of your function
public class F_GetAreaOfInterest : Function<Vector2>
{
    // Change type of this function to what you need it to be
    public override Vector2 Invoke()
    {
        return mob.AreaOfInterest;
    }

    public override string GetDescription()
    {
        return "Returns the mob's area of interest.";
    }
}