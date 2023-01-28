using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;

[CustomPropertyDrawer(typeof(CurveAttribute))]
public class CurveDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        CurveAttribute curve = attribute as CurveAttribute;
        if (property.propertyType == SerializedPropertyType.AnimationCurve)
        {
            EditorGUI.CurveField(position, property, curve.Colour, new Rect(curve.PosX, curve.PosY, curve.RangeX, curve.RangeY), label);
        }
    }
}
