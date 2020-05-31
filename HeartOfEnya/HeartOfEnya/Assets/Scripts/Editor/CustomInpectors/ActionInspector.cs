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
        EditorGUILayout.LabelField(new GUIContent("Text"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("id"), new GUIContent("Action ID"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("displayName"), new GUIContent("Display Name"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("description"), new GUIContent("Description"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("detailTexts"), new GUIContent("Detail Texts"), true);
        EditorGUILayout.LabelField(new GUIContent("VFX / SFX Fields"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cutInPrefab"), new GUIContent("Cut-In Prefab"));
        if(action.targetPattern.type == TargetPattern.Type.Spread)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("actionFxPrefab"), new GUIContent("Per-Action Fx Prefab"));
        else
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("actionFxPrefabVertical"), new GUIContent("Per-Action Fx Prefab Vertical"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("actionFxPrefabHorizontal"), new GUIContent("Per-Action Fx Prefab Horizontal"));
        }
        EditorGUILayout.PropertyField(serializedObject.FindProperty("tileFxPrefab"), new GUIContent("Per-Tile Fx Prefab"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("userFxPrefab"), new GUIContent("User Fx Prefab"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("delayAtEnd"), new GUIContent("Delay At End"));
        EditorUtils.Separator();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("singleStartup"), new GUIContent("Single Startup SFX"));
        if(action.singleStartup)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("startupOverride"), new GUIContent("Startup SFX"));
        }
        else
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("startupEnemy"), new GUIContent("Startup SFX (Enemy)"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("startupWood"), new GUIContent("Startup SFX (Wood)"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("startupStone"), new GUIContent("Startup SFX (Stone)"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("startupParty"), new GUIContent("Startup SFX (Party)"));
        }
        EditorGUILayout.PropertyField(serializedObject.FindProperty("hasImpacts"), new GUIContent("Has impact SFX"));
        if(action.hasImpacts)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("impactEnemy"), new GUIContent("Impact SFX (Enemy)"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("impactWood"), new GUIContent("Impact SFX (Wood)"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("impactStone"), new GUIContent("Impact SFX (Stone)"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("impactParty"), new GUIContent("Impact SFX (Party)"));
        }
        EditorUtils.Separator();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("chargeTurns"), new GUIContent("Charge Turns"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("targetFilter"), new GUIContent("Target Filter"));
        EditorUtils.Separator();


        #region Target Pattern And Range
        EditorGUILayout.PropertyField(serializedObject.FindProperty("targetPatternGenerator"), new GUIContent("Target Pattern Generator"));
        if (action.targetPatternGenerator == null && action.targetPattern.type == TargetPattern.Type.Directional)
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
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("range"), new GUIContent("Range"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("useSecondaryRange"));
            if (action.useSecondaryRange)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("secondaryRange"), new GUIContent("Secondary Range"), true);
        }

        EditorUtils.Separator();
        if(action.targetPatternGenerator == null)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("targetPattern"), GUIContent.none);

        #endregion

        serializedObject.ApplyModifiedProperties();
        EditorUtils.SetSceneDirtyIfGUIChanged(target);
    }
}
