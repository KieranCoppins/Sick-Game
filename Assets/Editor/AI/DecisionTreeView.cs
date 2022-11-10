using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System.Linq;

public class DecisionTreeView : GraphView
{
    public System.Action<BaseNodeView> OnNodeSelected;
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

        // Create edges
        tree.inputs.ForEach(input =>
        {
            var inputNode = GetNodeByGuid(input.inputGUID) as BaseNodeView;
            var outputNode = GetNodeByGuid(input.outputGUID) as BaseNodeView;
            Edge edge = outputNode.outputPorts[input.outputPortName].ConnectTo(inputNode.inputPorts[input.inputPortName]);
            AddElement(edge);
        });

    }


    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        Vector2 clickPoint = viewTransform.matrix.inverse.MultiplyPoint(evt.localMousePosition);
        GridBackground grid = contentContainer[0] as GridBackground;
        var types = TypeCache.GetTypesDerivedFrom<DecisionTreeEditorNode>();
        foreach(var type in types)
        {
            // We want to ignore any abstract types, root node and environment query systems (these are handled later)
            if (!type.IsAbstract && type != typeof(RootNode) && type != typeof(EnvironmentQuerySystem))
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

        // Load all EQSes using the asset database, that way we can add existing EQSes to the tree
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(EnvironmentQuerySystem).Name);

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            EnvironmentQuerySystem eqs = AssetDatabase.LoadAssetAtPath<EnvironmentQuerySystem>(path);
            evt.menu.AppendAction($"Environment Query Systems/{eqs.name}", (a) =>
            {
                // We want to make a clone of the EQSes incase of any changes we make.
                EnvironmentQuerySystem eqsClone = Object.Instantiate(eqs);
                CreateNode(eqsClone, clickPoint);
            });
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

    void CreateNode(ScriptableObject scriptableObject, Vector2 creationPos)
    {
        DecisionTreeEditorNode node = tree.CreateNode(scriptableObject, creationPos);
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
        nodeView.OnNodeSelected = OnNodeSelected;
        AddElement(nodeView);
    }

    void CreateNodeView(EnvironmentQuerySystem eqs)
    {
        EQSView nodeView = new(eqs);
        nodeView.OnNodeSelected = OnNodeSelected;
        AddElement(nodeView);
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if (graphViewChange.elementsToRemove != null)
        {
            graphViewChange.elementsToRemove.ForEach(elem =>
            {
                // Delete our node from our tree
                BaseNodeView nodeView = elem as BaseNodeView;
                if (nodeView != null)
                    tree.DeleteNode(nodeView.node);

                // If our element is an edge, delete the edge
                Edge edge = elem as Edge;
                if (edge != null)
                {
                    BaseNodeView inputNode = edge.input.node as BaseNodeView;
                    tree.inputs = tree.inputs.Where((input) => input != edge).ToList();
                }

            });
        }
        if (graphViewChange.edgesToCreate != null)
        {
            graphViewChange.edgesToCreate.ForEach(elem =>
            {
                // Create the edge graphically
                BaseNodeView inputNode = elem.input.node as BaseNodeView;
                BaseNodeView outputNode = elem.output.node as BaseNodeView;

                InputOutputPorts input = new(inputNode.node.guid, elem.input.name, outputNode.node.guid, elem.output.name);

                // Apply the edge for real

                // If we're a decision node
                Decision decisionNode = outputNode.node as Decision;
                if (decisionNode != null)
                {
                    if (input.outputPortName == "TRUE")
                        decisionNode.trueNode = inputNode.node as DecisionTreeNode;
                    else if (input.outputPortName == "FALSE")
                        decisionNode.falseNode = inputNode.node as DecisionTreeNode;
                    else
                        Debug.LogError("Decision node was set from an invalid output?!");
                }

                // If we're a root node
                RootNode rootNode = outputNode.node as RootNode;
                if(rootNode != null)
                {
                    rootNode.child = inputNode.node as DecisionTreeNode;
                }

                // If we're an EQS node
                EnvironmentQuerySystem eqsNode = outputNode.node as EnvironmentQuerySystem;
                if(eqsNode != null)
                {
                    if (elem.input.portType == typeof(GetDestination))
                    {
                        A_PathTo pathToAction = inputNode.node as A_PathTo;
                        if (pathToAction != null)
                        {
                            pathToAction.destinationDelegate = eqsNode.Run;
                        }
                    }
                }

                tree.inputs.Add(input);
            });
        }
        return graphViewChange;
    }
}
