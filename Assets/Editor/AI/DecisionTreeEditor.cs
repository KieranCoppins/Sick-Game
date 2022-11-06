using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class DecisionTreeEditor : EditorWindow
{

    InspectorView inspectorView;
    DecisionTreeView treeView;

    [MenuItem("Window/UI Toolkit/DecisionTreeEditor")]
    public static void ShowExample()
    {
        DecisionTreeEditor wnd = GetWindow<DecisionTreeEditor>();
        wnd.titleContent = new GUIContent("DecisionTreeEditor");
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


        OnSelectionChange();
    }


    private void OnSelectionChange()
    {
        DecisionTree decisionTree = Selection.activeObject as DecisionTree;
        if (decisionTree && AssetDatabase.CanOpenAssetInEditor(decisionTree.GetInstanceID()))
        {
            treeView.PopulateView(decisionTree);
        }
    }
}