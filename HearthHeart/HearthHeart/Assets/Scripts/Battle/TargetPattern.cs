using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class TargetPattern
{
    public enum Type
    {
        Spread,
        Directional,
    }

    public Type type; 
    public Vector2Int maxReach = new Vector2Int(1,1);
    public Pos TargetPos { get; private set; }
    private Pos UserPos { get; set; }
    public IEnumerable<Pos> Positions { get => offsets.Select((p) => p + TargetPos); }
    [SerializeField]
    private List<Pos> offsets = new List<Pos>();
    private List<GameObject> visualizationObjs = new List<GameObject>();

    public TargetPattern(params Pos[] offsets)
    {
        TargetPos = Pos.Zero;
        this.offsets.AddRange(offsets);
    }

    public TargetPattern(Pos origin, Pos[] offsets)
    {
        TargetPos = origin;
        this.offsets.AddRange(offsets);
    }

    public TargetPattern Clone()
    {
        TargetPattern newT = new TargetPattern();
        newT.offsets.AddRange(offsets);
        newT.Target(UserPos, TargetPos);
        newT.type = type;
        newT.maxReach = maxReach;
        return newT;
    }

    public void Show(Material squareMat, Transform parent = null)
    {
        Hide();
        foreach(var pos in Positions)
        {
            var modPos = pos;
            if(type == Type.Directional)
            {
                Pos direction = TargetPos - UserPos;
                modPos = Pos.Rotated(UserPos, pos - direction, Pos.Right, direction);
            }
            if(parent == null)
                visualizationObjs.Add(BattleGrid.main.SpawnSquare(modPos, squareMat));
            else
            {
                var obj = BattleGrid.main.SpawnSquare(modPos, squareMat);
                obj.transform.SetParent(parent);
                visualizationObjs.Add(obj);
            }

        }
    }

    public void Hide()
    {
        foreach(var obj in visualizationObjs)
        {
            GameObject.Destroy(obj);
        }
        visualizationObjs.Clear();
    }

    public void Shift(Pos offset)
    {
        TargetPos += offset;
        Vector3 offsetWorldPos = offset.AsVector2 * BattleGrid.main.cellSize;
        foreach (var obj in visualizationObjs)
        {
            obj.transform.position += offsetWorldPos;
        }
    }

    public void Target(Pos userPos, Pos targetPos)
    {
        TargetPos = targetPos;
        UserPos = userPos;
    }
}
