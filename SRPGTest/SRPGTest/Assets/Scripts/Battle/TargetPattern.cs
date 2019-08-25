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
    public Pos Origin { get; private set; }
    public IEnumerable<Pos> Positions { get => offsets.Select((p) => p + Origin); }
    [SerializeField]
    private List<Pos> offsets = new List<Pos>();
    private List<GameObject> visualizationObjs = new List<GameObject>();

    public TargetPattern(params Pos[] positions)
    {
        Origin = Pos.Origin;
        offsets.AddRange(positions);
    }

    public TargetPattern(Pos origin, Pos[] offsets)
    {
        Origin = origin;
        this.offsets.AddRange(offsets);
    }

    public void Show(GameObject visualizationPrefab)
    {
        Hide();
        foreach(var pos in Positions)
        {
            visualizationObjs.Add(GameObject.Instantiate(visualizationPrefab, BattleGrid.main.GetSpace(pos), Quaternion.identity));
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
        Origin += offset;
        Vector3 offsetWorldPos = offset.AsVector2 * BattleGrid.main.cellSize;
        foreach (var obj in visualizationObjs)
        {
            obj.transform.position += offsetWorldPos;
        }
    }

    public void Target(Pos targetPos)
    {
        Origin = targetPos;
    }
}
