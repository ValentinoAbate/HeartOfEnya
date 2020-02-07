using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class GraphPosition
{
    [MenuItem("FieldObject/Set All World Positions")]
    public static void PositionFieldObjectsWorldSpace()
    {
        var objs = Object.FindObjectsOfType<FieldEntity>();
        if (BattleGrid.main == null)
        {
            Debug.LogWarning("No detected battle grid. Please reload scene or add one");
            return;
        }
        foreach (var obj in objs)
        {
            Undo.RecordObject(obj, obj.name);
            var newPos = BattleGrid.main.GetSpace(obj.Pos);
            obj.transform.position = newPos;
        }
    }
    [MenuItem("FieldObject/Set All Graph Positions")]
    public static void PositionFieldObjectsGraphSpace()
    {
        var objs = Object.FindObjectsOfType<FieldEntity>();
        if (BattleGrid.main == null)
        {
            Debug.LogWarning("No detected battle grid. Please reload scene or add one");
            return;
        }
        foreach (var obj in objs)
        {
            Undo.RecordObject(obj, obj.name);
            var newGridPos = BattleGrid.main.GetPos(obj.transform.position);
            obj.Pos = newGridPos;
            obj.transform.position = BattleGrid.main.GetSpace(newGridPos);
        }
    }
}
