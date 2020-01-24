using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Class responsible for calculating and displaying targeting patterns.
/// There are two main types of targeting patterns: spread, and directional.
/// Spread patterns emanate from a point, while directional patterns emanate from the user in a direction.
/// Custom editor drawing in: TargetPatternDrawer.cs
/// </summary>
[System.Serializable]
public class TargetPattern
{
    public enum Type
    {
        Spread,
        Directional,
    }

    public Type type; 
    /// <summary>
    /// The maximum dimensions of the target pattern. Used in editor GUI, and can be used in AI calculations
    /// </summary>
    public Vector2Int maxReach = new Vector2Int(1,1);
    /// <summary>
    /// The position being targeted. If directional, should be adjacent to UserPos. Set in Target()
    /// </summary>
    public Pos TargetPos { get; private set; }
    /// <summary>
    /// The position of the unit using this target pattern. Set in Target()
    /// </summary>
    private Pos UserPos { get; set; }
    /// <summary>
    /// The positions currently targeted. Takes into account the target position, but doesn't properly rotate. if directional
    /// </summary>
    public IEnumerable<Pos> Positions { get => offsets.Select((p) => p + TargetPos); }
    // the offsets from the target position that encode the target pattern
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

    /// <summary>
    /// Display a the targeting UI at the current target position
    /// </summary>
    public void Show(Material squareMat, Transform parent = null)
    {
        Hide();
        foreach(var pos in Positions)
        {
            var modPos = pos;
            // If the target pattern is directional, rotate the coordinates when applicable
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

    /// <summary>
    /// Hide the targeting UI
    /// </summary>
    public void Hide()
    {
        if (visualizationObjs == null)
            visualizationObjs = new List<GameObject>();
        foreach(var obj in visualizationObjs)
        {
            GameObject.Destroy(obj);
        }
        visualizationObjs.Clear();
    }

    /// <summary>
    /// Shift the target and UI position without destroying and recreating visualization objects
    /// </summary>
    public void Shift(Pos offset)
    {
        TargetPos += offset;
        Vector3 offsetWorldPos = offset.AsVector2 * BattleGrid.main.cellSize;
        foreach (var obj in visualizationObjs)
        {
            obj.transform.position += offsetWorldPos;
        }
    }

    /// <summary>
    /// Set the current user and target position
    /// </summary>
    public void Target(Pos userPos, Pos targetPos)
    {
        TargetPos = targetPos;
        UserPos = userPos;
    }
}
