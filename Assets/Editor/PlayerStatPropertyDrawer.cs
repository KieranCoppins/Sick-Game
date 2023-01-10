using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(PlayerStat))]
public class PlayerStatPropertyDraw : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        List<string> playerStatsLabels = new List<string>();
        var properties = typeof(BaseCharacter).GetProperties(GenericHelpers.getFieldFlags);
        int currentChoice = 0;
        for (int i = 0; i < properties.Length; i++)
        {
            if (properties[i].PropertyType == typeof(int) || properties[i].PropertyType == typeof(float))
            {
                playerStatsLabels.Add(properties[i].Name);
                if (properties[i].Name == property.FindPropertyRelative("statName").stringValue)
                    currentChoice = i != 0 ? i - 1 : 0;

            }
        }

        EditorGUI.BeginChangeCheck();
        int choiceIndex = EditorGUI.Popup(position, label.text, currentChoice, playerStatsLabels.ToArray());
        Debug.Log(choiceIndex);
        if (EditorGUI.EndChangeCheck())
        {
            property.FindPropertyRelative("statName").stringValue = playerStatsLabels[choiceIndex];
        }
        EditorGUI.EndProperty();
    }
}
