using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Replace function template type with the return type of your function
public class F_RunEQS : Function<Vector2>
{
    [SerializeField] EnvironmentQuerySystem eqs;
    // Change type of this function to what you need it to be
    public override Vector2 Invoke()
    {
        return eqs.Run();
    }

    public override void Initialise(BaseMob mob)
    {
        base.Initialise(mob);
        eqs.Initialise(mob);
    }

    public override DecisionTreeEditorNode Clone()
    {
        F_RunEQS clone = Instantiate(this);
        clone.eqs = (EnvironmentQuerySystem)eqs.Clone();
        return clone;
    }

    public override string GetDescription(BaseNodeView nodeView)
    {
        if (eqs == null)
        {
            nodeView.error = "Theres no Environment Query System in this function!";
            return "";
        }
        nodeView.error = "";
        return eqs.GetDescription(nodeView);
    }
}