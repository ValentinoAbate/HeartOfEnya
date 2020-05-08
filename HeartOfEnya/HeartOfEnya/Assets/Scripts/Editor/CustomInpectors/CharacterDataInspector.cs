using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SerializableCollections.EditorGUIUtils;

[CustomEditor(typeof(CharacterData))]
public class CharacterDataInspector : Editor
{
    private CharacterData data;
    private string toAdd;
    private void OnEnable()
    {
        data = target as CharacterData;
    }
    public override void OnInspectorGUI()
    {
        data.voiceEvent = EditorGUILayout.TextField(new GUIContent("Voice Event"), data.voiceEvent);
        data.dialogBoxPrefab = EditorUtils.ObjectField(new GUIContent("Dialog Box Prefab"), data.dialogBoxPrefab, false);
        data.portraits.DoGUILayout(data.portraits.ValueGUIObj, () => data.portraits.StringAddGUID(ref toAdd), "Portraits", true);
        if (GUI.changed)
            EditorUtility.SetDirty(data);
    }
}
