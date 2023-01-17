using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KieranCoppins.DecisionTreesEditor;
using KieranCoppins.DecisionTrees;
using UnityEditor;
using Unity.VisualScripting;

public class CustomDecisionTreeEditor : DecisionTreeEditor
{
    public override void OnSelectionChange()
    {
        DecisionTree dt = Selection.activeObject as DecisionTree;
        if (dt)
        {
            //base.OnSelectionChange();
            DecisionTree decisionTree = Selection.activeObject as DecisionTree;
            if (decisionTree && AssetDatabase.CanOpenAssetInEditor(decisionTree.GetInstanceID()))
                TreeView?.PopulateView(decisionTree);
        }
        else 
        {
            BaseMob mob = Selection.activeObject?.GetComponent<BaseMob>();
            if (Application.isPlaying && mob && mob?.DecisionTree)
            {
                TreeView?.PopulateView(mob.DecisionTree);
            }
        }
    }
}
