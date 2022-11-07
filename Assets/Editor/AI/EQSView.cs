using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

public class EQSView : UnityEditor.Experimental.GraphView.Node
{
    public EnvironmentQuerySystem eqs;
    public Port output;
    public EQSView(EnvironmentQuerySystem eqs)
    {
        this.eqs = eqs;
        this.title = eqs.name;

        style.left = eqs.positionalData.xMin;
        style.top = eqs.positionalData.yMin;
        this.viewDataKey = eqs.guid;

        output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(GetDestination));
        output.portName = "Position";
        outputContainer.Add(output);
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        eqs.positionalData = newPos;
    }


}
