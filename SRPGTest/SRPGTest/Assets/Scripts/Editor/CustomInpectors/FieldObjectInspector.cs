using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(FieldObject), true)]
public class FieldObjectInpector : Editor
{
    public override void OnInspectorGUI()
    {
        var obj = target as FieldObject;
        base.OnInspectorGUI();
        if(GUILayout.Button("Set Grid Position from World Pos"))
        {
            if (BattleGrid.main == null)
                Debug.LogWarning("No detected battle grid. Please reload scene or add one");
            serializedObject.Update();
            var newPos = BattleGrid.main.GetPos(obj.transform.position);
            var posProp = serializedObject.FindProperty("pos");
            posProp.FindPropertyRelative("row").intValue = newPos.row;
            posProp.FindPropertyRelative("col").intValue = newPos.col;
            var oldPos = obj.transform.position;
            obj.transform.position = BattleGrid.main.GetSpace(newPos);
            if(oldPos != obj.transform.position)
                EditorUtility.SetDirty(obj.transform);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
