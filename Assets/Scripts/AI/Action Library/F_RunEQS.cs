using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KieranCoppins.DecisionTrees;

// Replace function template type with the return type of your function
public class F_RunEQS : Function<Vector2>
{
    [SerializeField] private EnvironmentQuerySystem _eqs;
    // Change type of this function to what you need it to be
    public override Vector2 Invoke()
    {
        return _eqs.Run();
    }

    public override void Initialise<T>(T metaData)
    {
        base.Initialise(metaData);
        _eqs.Initialise(metaData as BaseMob);
    }

    public override DecisionTreeEditorNodeBase Clone()
    {
        F_RunEQS clone = Instantiate(this);
        clone._eqs = (EnvironmentQuerySystem)_eqs.Clone();
        return clone;
    }

    public override string GetDescription(BaseNodeView nodeView)
    {
        if (_eqs == null)
        {
            nodeView.Error = "Theres no Environment Query System in this function!";
            return "";
        }
        nodeView.Error = "";
        return _eqs.GetDescription(nodeView);
    }

    public override string GetSummary(BaseNodeView nodeView)
    {
        if (_eqs == null)
        {
            nodeView.Error = "Theres no Environment Query System in this function!";
            return "";
        }
        nodeView.Error = "";
        return $"runs the Environment Query System: {_eqs.name}";
    }
}