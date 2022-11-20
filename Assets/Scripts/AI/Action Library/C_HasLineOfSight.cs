using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_HasLineOfSight : F_Condition
{
    public override bool Invoke() => mob.HasLineOfSight(mob.Target.position);
}
