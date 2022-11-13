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
        System.Reflection.BindingFlags getFieldFlags = System.Reflection.BindingFlags.Default;
        getFieldFlags |= System.Reflection.BindingFlags.Public;
        getFieldFlags |= System.Reflection.BindingFlags.NonPublic;
        getFieldFlags |= System.Reflection.BindingFlags.Instance;
        var fields = typeof(BaseCharacter).GetFields(getFieldFlags);
        int currentChoice = 0;
        for (int i = 0; i < fields.Length; i++)
        {
            if (fields[i].FieldType == typeof(int) || fields[i].FieldType == typeof(float))
            {
                playerStatsLabels.Add(fields[i].Name);
                if (fields[i].Name == property.FindPropertyRelative("statName").stringValue)
                    currentChoice = i;

            }
        }

        EditorGUI.BeginChangeCheck();
        int choiceIndex = EditorGUI.Popup(position, label.text, currentChoice, playerStatsLabels.ToArray());
        if (EditorGUI.EndChangeCheck())
        {
            property.FindPropertyRelative("statName").stringValue = playerStatsLabels[choiceIndex];
        }
        EditorGUI.EndProperty();
    }
}
