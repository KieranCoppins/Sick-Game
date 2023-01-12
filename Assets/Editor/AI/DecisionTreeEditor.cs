using Codice.CM.Common;
using System;
using Unity.VisualScripting;
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

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange obj)
    {
        switch (obj)
        {
            case PlayModeStateChange.EnteredEditMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingEditMode:
                break;
            case PlayModeStateChange.EnteredPlayMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingPlayMode:
                break;
        }
    }


    private void OnSelectionChange()
    {
        DecisionTree decisionTree = Selection.activeObject as DecisionTree;
        if (!decisionTree)
        {
            BaseMob baseMob = Selection.activeObject?.GetComponent<BaseMob>();
            if (baseMob)
            {
                decisionTree = baseMob.decisionTree;
            }
        }

        if (decisionTree && Application.isPlaying)
            treeView?.PopulateView(decisionTree);
        else
            if (decisionTree && AssetDatabase.CanOpenAssetInEditor(decisionTree.GetInstanceID()))
                treeView.PopulateView(decisionTree);
    }

    void OnNodeSelectionChanged(BaseNodeView nodeView)
    {
        inspectorView.UpdateSelection(nodeView);
    }

    private void OnInspectorUpdate()
    {
        treeView?.UpdateNodeStates();
    }
}