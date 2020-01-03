using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Pos))]
public class PosDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        const float labelWidth = 35;
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        var UIRect = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        float area = UIRect.width - 3;
        float fieldWidth = (area - labelWidth * 2) * 0.5f;

        UIRect.width = labelWidth;
        EditorGUI.LabelField(UIRect, new GUIContent("Row"));
        UIRect.x += UIRect.width;
        UIRect.width = fieldWidth;
        EditorGUI.PropertyField(UIRect, property.FindPropertyRelative("row"), GUIContent.none);
        UIRect.x += UIRect.width + 3;
        UIRect.width = labelWidth;
        EditorGUI.LabelField(UIRect, new GUIContent("Col"));
        UIRect.x += UIRect.width;
        UIRect.width = fieldWidth;
        EditorGUI.PropertyField(UIRect, property.FindPropertyRelative("col"), GUIContent.none);

        EditorGUI.EndProperty();
    }
}
