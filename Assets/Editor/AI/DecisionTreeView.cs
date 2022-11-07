using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System.Linq;

public class DecisionTreeView : GraphView
{
    public new class UxmlFactory : UxmlFactory<DecisionTreeView, UxmlTraits> { };
    DecisionTree tree;
    public DecisionTreeView()
    {
        Insert(0, new GridBackground());

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/AI/DecisionTreeEditor.uss");
        styleSheets.Add(styleSheet);

        graphViewChanged += OnGraphViewChanged;
    }

    UnityEditor.Experimental.GraphView.Node FindNodeView(ScriptableObject node)
    {
        DecisionTreeNode dtNode = node as DecisionTreeNode;
        if (dtNode)
            return GetNodeByGuid(dtNode.guid) as DecisionTreeNodeView;

        EnvironmentQuerySystem eqs = node as EnvironmentQuerySystem;
        if (eqs)
            return GetNodeByGuid(eqs.guid) as EQSView;

        return null;
    }

    public void PopulateView(DecisionTree tree)
    {
        this.tree = tree;
        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        if (tree.root == null)
        {
            tree.root = tree.CreateNode(typeof(RootNode), Vector2.zero) as RootNode;
            EditorUtility.SetDirty(tree);
            AssetDatabase.SaveAssets();
        }

        // Create node views
        tree.nodes.ForEach(node =>
        {
            if (node is DecisionTreeNode)
                CreateNodeView(node as DecisionTreeNode);
            else if (node is EnvironmentQuerySystem)
                CreateNodeView(node as EnvironmentQuerySystem);
                
        });

        // Create node edges
        tree.nodes.ForEach(node =>
        {
            Decision decisionNode = node as Decision;

            if (decisionNode != null)
            {
                DecisionTreeNodeView parentView = FindNodeView(decisionNode) as DecisionTreeNodeView;
                if (decisionNode.trueNode != null)
                {
                    DecisionTreeNodeView trueView = FindNodeView(decisionNode.trueNode) as DecisionTreeNodeView;
                    Edge edge = parentView.outputPorts["TRUE"].ConnectTo(trueView.inputPorts["main"]);
                    AddElement(edge);
                }
                if (decisionNode.falseNode != null)
                {
                    DecisionTreeNodeView falseView = FindNodeView(decisionNode.falseNode) as DecisionTreeNodeView;
                    Edge edge = parentView.outputPorts["FALSE"].ConnectTo(falseView.inputPorts["main"]);
                    AddElement(edge);
                }
            }


            RootNode rootNode = node as RootNode;
            if (rootNode != null)
            {
                DecisionTreeNodeView parentView = FindNodeView(rootNode) as DecisionTreeNodeView;
                if (rootNode.child != null)
                {
                    DecisionTreeNodeView childNodeView = FindNodeView(rootNode.child) as DecisionTreeNodeView;
                    Edge edge = parentView.outputPorts["main"].ConnectTo(childNodeView.inputPorts["main"]);
                    AddElement(edge);
                }
            }

            EnvironmentQuerySystem eqs = node as EnvironmentQuerySystem;
            if (eqs != null)
            {
                EQSView parentView = FindNodeView(eqs) as EQSView;
                foreach (var connection in eqs.connections)
                {
                    DecisionTreeNodeView nodeView = FindNodeView(connection.Value) as DecisionTreeNodeView;
                    Edge edge = parentView.output.ConnectTo(nodeView.inputPorts[connection.Key]);
                    AddElement(edge);
                }
            }
        });

    }


    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        Vector2 clickPoint = viewTransform.matrix.inverse.MultiplyPoint(evt.localMousePosition);
        var types = TypeCache.GetTypesDerivedFrom<DecisionTreeEditorNode>();
        foreach(var type in types)
        {
            if (!type.IsAbstract && type != typeof(RootNode))
            {
                System.Type rootBaseType = type.BaseType;
                string pathString = "";
                while (rootBaseType != null && rootBaseType != typeof(DecisionTreeEditorNode))
                {
                    pathString = $"{rootBaseType.Name}/" + pathString;
                    rootBaseType = rootBaseType.BaseType;
                }
                evt.menu.AppendAction(pathString + $"{type.Name}", (a) => CreateNode(type, clickPoint));
            }
        }
    }

    void CreateNode(System.Type type, Vector2 creationPos)
    {
        DecisionTreeEditorNode node = tree.CreateNode(type, creationPos);
        if (node is DecisionTreeNode)
            CreateNodeView(node as DecisionTreeNode);
        else if (node is EnvironmentQuerySystem)
            CreateNodeView(node as EnvironmentQuerySystem);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
    }

    void CreateNodeView(DecisionTreeNode node)
    {
        DecisionTreeNodeView nodeView = new(node);
        AddElement(nodeView);
    }

    void CreateNodeView(EnvironmentQuerySystem eqs)
    {
        EQSView nodeView = new(eqs);
        AddElement(nodeView);
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if (graphViewChange.elementsToRemove != null)
        {
            graphViewChange.elementsToRemove.ForEach(elem =>
            {
                // Delete our node from our tree
                DecisionTreeNodeView nodeView = elem as DecisionTreeNodeView;
                if (nodeView != null)
                    tree.DeleteNode(nodeView.node);

                EQSView eqsView = elem as EQSView;
                if (eqsView != null)
                    tree.DeleteNode(eqsView.eqs);

                // If our element is an edge, delete the edge
                Edge edge = elem as Edge;
                if (edge != null)
                {
                    DecisionTreeNodeView decisionView = edge.output.node as DecisionTreeNodeView;
                    if (decisionView != null)
                    {
                        Decision decisionNode = decisionView.node as Decision;
                        if (decisionNode)
                        {
                            if (edge.output.portName == "TRUE")
                                decisionNode.trueNode = null;
                            else
                                decisionNode.falseNode = null;
                        }

                        RootNode rootNode = decisionView.node as RootNode;
                        if (rootNode != null)
                            rootNode.child = null;
                    }
                    eqsView = edge.output.node as EQSView;
                    if (eqsView != null)
                    {
                        DecisionTreeNodeView childView = edge.input.node as DecisionTreeNodeView;
                        childView.node.GetType().GetField(edge.input.portName).SetValue(childView.node, null);
                        eqsView.eqs.connections.Remove(edge.input.portName);
                    }
                }

            });
        }
        if (graphViewChange.edgesToCreate != null)
        {
            graphViewChange.edgesToCreate.ForEach(elem =>
            {
                // Assign the correct node to the correct child;
                DecisionTreeNodeView decisionView = elem.output.node as DecisionTreeNodeView;

                if (decisionView != null)
                {
                    DecisionTreeNodeView childView = elem.input.node as DecisionTreeNodeView;

                    Decision decisionNode = decisionView.node as Decision;
                    if (decisionNode)
                    {
                        if (elem.output.portName == "TRUE")
                            decisionNode.trueNode = childView.node;
                        else
                            decisionNode.falseNode = childView.node;
                    }

                    RootNode rootNode = decisionView.node as RootNode;
                    if (rootNode != null)
                        rootNode.child = childView.node;
                }

                EQSView eqsView = elem.output.node as EQSView;
                if (eqsView != null)
                {
                    DecisionTreeNodeView childView = elem.input.node as DecisionTreeNodeView;
                    EnvironmentQuerySystem eqs = eqsView.eqs;
                    if (eqs != null)
                    {
                        if (elem.input.portType == typeof(GetDestination))
                        {
                            GetDestination del = eqs.Run;
                            childView.node.GetType().GetField (elem.input.portName).SetValue(childView.node, del);
                            eqs.connections[elem.input.portName] = childView.node;
                        }
                    }
                }
            });
        }
        return graphViewChange;
    }
}
