using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

public class DecisionTreeNodeView : BaseNodeView
{

    public DecisionTreeNodeView(DecisionTreeNode node) : base(node)
    {

        CreateInputPorts();
        CreateOutputPorts();
    }

    void CreateInputPorts()
    {
        if (node is RootNode)
            return;

        Port port = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
        port.portName = "";
        port.name = "main";
        inputPorts.Add("main", port);
        inputContainer.Add(port);
        var constructors = node.GetType().GetConstructors();
        foreach(var constructor in constructors)
        {
            if (constructor.GetParameters().Length > 0)
            {
                foreach(var parameter in constructor.GetParameters())
                {
                    port = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, parameter.ParameterType);
                    port.portName = parameter.Name;
                    port.name = parameter.Name;
                    inputPorts.Add(parameter.Name, port);
                    inputContainer.Add(port);
                }
            }
        }
    }

    void CreateOutputPorts()
    {
        if (node is Action)
        {

        }
        else if (node is Decision)
        {
            Port port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            port.portName = "TRUE";
            port.name = "TRUE";
            outputPorts.Add("TRUE", port);
            outputContainer.Add(port);


            port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            port.name = "FALSE";
            port.portName = "FALSE";
            outputPorts.Add("FALSE", port);
            outputContainer.Add(port);
        }
        else if (node is RootNode)
        {
            Port port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            port.name = "main";
            port.portName = "";
            outputPorts.Add("main", port);
            outputContainer.Add(port);
        }


    }

}
