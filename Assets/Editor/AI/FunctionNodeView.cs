using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

public class FunctionNodeView : BaseNodeView
{
    public FunctionNodeView(object function) : base(function as DecisionTreeEditorNode)
    {
        CreateInputPorts();

        Port port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, GenericHelpers.GetGenericType(function));
        port.portName = "Output";
        port.name = "Output";
        outputPorts.Add("Output", port);
        outputContainer.Add(port);
    }
    void CreateInputPorts()
    {
        if (node is RootNode)
            return;

        var constructors = node.GetType().GetConstructors();
        foreach (var constructor in constructors)
        {
            if (constructor.GetParameters().Length > 0)
            {
                foreach (var parameter in constructor.GetParameters())
                {
                    Port port = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, parameter.ParameterType);
                    port.portName = parameter.Name;
                    port.name = parameter.Name;
                    inputPorts.Add(parameter.Name, port);
                    inputContainer.Add(port);
                }
            }
        }
    }
}
