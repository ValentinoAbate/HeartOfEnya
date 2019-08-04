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
        data.textScrollSfx = EditorUtils.ObjectField(new GUIContent("Text Scroll SFX"), data.textScrollSfx, false);
        int numPortraits = data.portraits.Count;
        Sprite valGUI(Sprite spr) => EditorUtils.ObjectField(spr, false);
        data.portraits.DoGUILayout(valGUI, () => data.portraits.StringAddGUID(ref toAdd), "Portraits", true);
        if (GUI.changed) //|| data.portraits.Count != numPortraits)
            EditorUtility.SetDirty(data);
    }
}
