using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SerializableCollections.EditorGUIUtils;

[CustomEditor(typeof(TileUI))]
public class TileUIInspector : Editor
{
    private TileUI data;
    private const float labelWidth = 160;
    private void OnEnable()
    {
        data = target as TileUI;
    }
    public override void OnInspectorGUI()
    {
        Undo.RecordObject(data, data.name);
        data.tileUIPrefab = EditorUtils.ObjectField(new GUIContent("Tile UI Prefab"), data.tileUIPrefab, false);
        data.tileColors.DoGUILayout((key) => data.tileColors.KeyGUIFixedWidth(key, labelWidth), 
            (c) => c = EditorGUILayout.ColorField(c), data.tileColors.EnumAddGUI, "Colors", true);
        data.textures.DoGUILayout((key) => data.textures.KeyGUIFixedWidth(key, labelWidth),
            data.textures.ValueGUIObj, data.textures.EnumAddGUIVal, "Textures (Optional)", true);
    }
}
