using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;

public class InspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { };
    Editor editor;
    public InspectorView()
    {

    }

    public void UpdateSelection(UnityEditor.Experimental.GraphView.Node nodeView)
    {
        Clear();

        UnityEngine.Object.DestroyImmediate(editor);
        if (nodeView.GetType() == typeof(DecisionTreeNodeView))
            editor = Editor.CreateEditor((nodeView as DecisionTreeNodeView).node);
        if (nodeView.GetType() == typeof(EQSView))
            editor = Editor.CreateEditor((nodeView as EQSView).eqs);

        IMGUIContainer container = new(() => { editor.OnInspectorGUI(); });
        Add(container);
    }
}
