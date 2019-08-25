using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ActiveAbility))]
public class ActiveAbilityInspector : Editor
{
    public override void OnInspectorGUI()
    {
        var ability = target as ActiveAbility;
        ability.accuracy = EditorGUILayout.IntSlider("Accuracy", Mathf.FloorToInt(ability.accuracy * 100), 0, 200) / 100f;
        EditorUtils.Separator();
        if(ability.targetPattern.type == TargetPattern.Type.Directional)
        {
            if(ability.range.minRange != 1)
            {
                ability.range.minRange = 1;
                ability.range.maxRange = 1;
                EditorUtils.SetSceneDirty(target);
            }
            else if (ability.range.maxRange != 1)
            {
                ability.range.minRange = 1;
                ability.range.maxRange = 1;
                EditorUtils.SetSceneDirty(target);
            }
            EditorGUILayout.LabelField(new GUIContent("Range: 1-1 (Directional)"), EditorUtils.Bold);
        }
        else
            EditorGUILayout.PropertyField(serializedObject.FindProperty("range"), new GUIContent("Range"), true);
        EditorUtils.Separator();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("targetPattern"), GUIContent.none);
        serializedObject.ApplyModifiedProperties();
        EditorUtils.SetSceneDirtyIfGUIChanged(target);
    }
}
