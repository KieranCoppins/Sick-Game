using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_AND : F_LogicGate
{
    public F_AND(Function<bool> A, Function<bool> B) : base(A, B)
    {
    }

    public override bool Invoke()
    {
        return A.Invoke() && B.Invoke();
    }
}
