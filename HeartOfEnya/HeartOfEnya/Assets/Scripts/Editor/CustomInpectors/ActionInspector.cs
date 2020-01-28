using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Action))]
public class ActionInspector : Editor
{
    public override void OnInspectorGUI()
    {
        var action = target as Action;
        EditorGUILayout.LabelField(new GUIContent("VFX Fields"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cutInPrefab"), new GUIContent("Cut-In Prefab"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("fxPrefab"), new GUIContent("Vfx Prefab"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("delayAtEnd"), new GUIContent("Delay At End"));
        EditorUtils.Separator();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("chargeTurns"), new GUIContent("Charge Turns"));
        EditorUtils.Separator();
        if(action.targetPattern.type == TargetPattern.Type.Directional)
        {
            if(action.range.min != 1)
            {
                action.range.min = 1;
                action.range.max = 1;
                EditorUtils.SetSceneDirty(target);
            }
            else if (action.range.max != 1)
            {
                action.range.min = 1;
                action.range.max = 1;
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
