using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;

public static partial class EditorUtils
{
    public static void Separator()
    {
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }
    /// <summary>Easy shortcut to the bold label text style</summary>
    public static GUIStyle Bold
    {
        get => new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold };
    }
    public static GUIStyle BoldCentered
    {
        get => new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
    }

    public static GUIStyle Centered
    {
        get => new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
    }

    public static T EnumPopup<T>(GUIContent label, T enumData, params GUILayoutOption[] options) where T : System.Enum
    {
        return (T)EditorGUILayout.EnumPopup(label, enumData, options);
    }
    public static T ObjectField<T>(T obj, bool allowSceneObjects, params GUILayoutOption[] options) where T : UnityEngine.Object
    {
        return EditorGUILayout.ObjectField(obj, typeof(T), allowSceneObjects, options) as T;
    }
    public static T ObjectField<T>(GUIContent label, T obj, bool allowSceneObjects, params GUILayoutOption[] options) where T : UnityEngine.Object
    {
        return EditorGUILayout.ObjectField(label, obj, typeof(T), allowSceneObjects, options) as T;
    }
    public static void SetSceneDirtyIfGUIChanged(Object target)
    {
        if (GUI.changed)
        {
            SetSceneDirty(target);
        }
    }

    public static void SetSceneDirty(Object target)
    {
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        if (prefabStage != null)
        {
            EditorSceneManager.MarkSceneDirty(prefabStage.scene);
        }
    }
}



