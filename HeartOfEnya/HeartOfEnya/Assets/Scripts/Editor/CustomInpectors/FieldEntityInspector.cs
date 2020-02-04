using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(FieldEntity), true)]
[CanEditMultipleObjects]
public class FieldEntityInpector : Editor
{
    public override void OnInspectorGUI()
    {
        var obj = target as FieldEntity;
        base.OnInspectorGUI();
        Undo.RecordObject(obj, obj.name);
        if (GUILayout.Button("Set Grid Position from World Pos"))
        {
            if (BattleGrid.main == null)
            {
                Debug.LogWarning("No detected battle grid. Please reload scene or add one");
                return;
            }
            obj.Pos = BattleGrid.main.GetPos(obj.transform.position);
            obj.transform.position = BattleGrid.main.GetSpace(obj.Pos);
        }
        if (GUILayout.Button("Set World Position from Grid Pos"))
        {
            if (BattleGrid.main == null)
            {
                Debug.LogWarning("No detected battle grid. Please reload scene or add one");
                return;
            }
            obj.transform.position = BattleGrid.main.GetSpace(obj.Pos);
        }
    }
}
