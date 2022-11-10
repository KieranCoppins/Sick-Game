using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

public class EQSView : BaseNodeView
{
    public EQSView(EnvironmentQuerySystem eqs) : base(eqs)
    {
        Port port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(GetDestination));
        port.portName = "Position";
        port.name = "Position";
        outputPorts.Add("Position", port);
        outputContainer.Add(port);
    }
}
