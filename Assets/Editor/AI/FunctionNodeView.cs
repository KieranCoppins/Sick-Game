using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

public class FunctionNodeView : BaseNodeView
{
    public FunctionNodeView(object function) : base(function as DecisionTreeEditorNode)
    {
        Port port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, GenericHelpers.GetGenericType(function));
        port.portName = "Output";
        port.name = "Output";
        outputPorts.Add("Output", port);
        outputContainer.Add(port);
    }
}
