using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

public class DecisionTreeNodeView : UnityEditor.Experimental.GraphView.Node
{
    public DecisionTreeNode node;
    public Dictionary<string, Port> inputPorts;
    public Dictionary<string, Port> outputPorts;

    public DecisionTreeNodeView(DecisionTreeNode node)
    {
        this.node = node;
        this.title = node.name;

        style.left = node.positionalData.xMin;
        style.top = node.positionalData.yMin;
        this.viewDataKey = node.guid;

        inputPorts = new Dictionary<string, Port>();
        outputPorts = new Dictionary<string, Port>();

        CreateInputPorts();
        CreateOutputPorts();
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        node.positionalData = newPos;
    }

    void CreateInputPorts()
    {
        if (node is RootNode)
            return;

        Port port = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
        port.portName = "";
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
            outputPorts.Add("TRUE", port);
            outputContainer.Add(port);


            port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            port.portName = "FALSE";
            outputPorts.Add("FALSE", port);
            outputContainer.Add(port);
        }
        else if (node is RootNode)
        {
            Port port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            port.portName = "";
            outputPorts.Add("main", port);
            outputContainer.Add(port);
        }


    }

}
