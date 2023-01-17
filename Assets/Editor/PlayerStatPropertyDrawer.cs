using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using KieranCoppins.GenericHelpers;

[CustomPropertyDrawer(typeof(PlayerStat))]
public class PlayerStatPropertyDraw : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        List<string> playerStatsLabels = new List<string>();
        var properties = typeof(BaseCharacter).GetProperties(GenericHelpers.GetFieldFlags);
        int currentChoice = 0;
        for (int i = 0; i < properties.Length; i++)
        {
            if (properties[i].PropertyType == typeof(int) || properties[i].PropertyType == typeof(float))
            {
                playerStatsLabels.Add(properties[i].Name);
                if (properties[i].Name == property.FindPropertyRelative("_statName").stringValue)
                    currentChoice = i != 0 ? i - 1 : 0;

            }
        }

        EditorGUI.BeginChangeCheck();
        int choiceIndex = EditorGUI.Popup(position, label.text, currentChoice, playerStatsLabels.ToArray());

        if (EditorGUI.EndChangeCheck())
        {
            property.FindPropertyRelative("_statName").stringValue = playerStatsLabels[choiceIndex];
        }
        EditorGUI.EndProperty();
    }
}
