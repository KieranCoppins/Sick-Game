using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;


public class DecisionTreeEditor : EditorWindow
{

    InspectorView inspectorView;
    DecisionTreeView treeView;

    [MenuItem("Window/AI/Decision Tree Editor")]
    public static void ShowExample()
    {
        DecisionTreeEditor wnd = GetWindow<DecisionTreeEditor>();
        wnd.titleContent = new GUIContent("Decision Tree Editor");
    }

    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceId, int line)
    {
        if (Selection.activeObject is DecisionTree)
        {
            ShowExample();
            return true;
        }
        return false;
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/AI/DecisionTreeEditor.uxml");
        visualTree.CloneTree(root);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/AI/DecisionTreeEditor.uss");
        root.styleSheets.Add(styleSheet);

        inspectorView = root.Q<InspectorView>();
        treeView = root.Q<DecisionTreeView>();
        treeView.OnNodeSelected = OnNodeSelectionChanged;

        OnSelectionChange();
    }


    private void OnSelectionChange()
    {
        DecisionTree decisionTree = Selection.activeObject as DecisionTree;
        if (decisionTree && AssetDatabase.CanOpenAssetInEditor(decisionTree.GetInstanceID()))
            treeView.PopulateView(decisionTree);
    }

    void OnNodeSelectionChanged(BaseNodeView nodeView)
    {
        inspectorView.UpdateSelection(nodeView);
    }
}