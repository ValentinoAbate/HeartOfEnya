using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class GraphPosition
{
    [MenuItem("FieldObject/Set All World Positions")]
    static void PositionFieldObjectsWorldSpace()
    {
        var objs = Object.FindObjectsOfType<FieldEntity>();
        if (BattleGrid.main == null)
        {
            Debug.LogWarning("No detected battle grid. Please reload scene or add one");
            return;
        }
        foreach (var obj in objs)
        {
            var newPos = BattleGrid.main.GetSpace(obj.Pos);
            var oldPos = obj.transform.position;
            obj.transform.position = newPos;
            if (oldPos != obj.transform.position)
                EditorUtility.SetDirty(obj.transform);
        }
    }
    [MenuItem("FieldObject/Set All Graph Positions")]
    static void PositionFieldObjectsGraphSpace()
    {
        var objs = Object.FindObjectsOfType<FieldEntity>();
        if (BattleGrid.main == null)
        {
            Debug.LogWarning("No detected battle grid. Please reload scene or add one");
            return;
        }
        foreach (var obj in objs)
        {
            var oldGridPos = obj.Pos;
            var newGridPos = BattleGrid.main.GetPos(obj.transform.position);
            obj.Pos = newGridPos;
            var oldPos = obj.transform.position;
            obj.transform.position = BattleGrid.main.GetSpace(newGridPos);
            if (oldPos != obj.transform.position || oldGridPos != newGridPos)
            {
                EditorUtility.SetDirty(obj.transform);
            }
        }
    }
}
