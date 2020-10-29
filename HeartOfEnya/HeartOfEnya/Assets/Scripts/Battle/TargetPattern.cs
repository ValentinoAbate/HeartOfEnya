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
        Absolute,
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
    private List<TileUI.Entry> tileUIEntries = new List<TileUI.Entry>();

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

    public void Add(TargetPattern other)
    {
        offsets.AddRange(other.Positions);
    }

    public void RemoveDuplicates()
    {
        offsets = offsets.Distinct().ToList();
    }

    public void Remove(Pos p)
    {
        offsets.Remove(p);
    }

    /// <summary>
    /// Display the targeting UI at the current target position
    /// </summary>
    public void Show(TileUI.Type tileType, Transform parent = null)
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
            var tileEntry = BattleGrid.main.SpawnTileUI(modPos, tileType);
            // modPos wasn't in a legal square
            if (tileEntry.type == TileUI.Type.Empty)
                continue;
            if (parent != null)
                tileEntry.mesh.transform.SetParent(parent);
            tileUIEntries.Add(tileEntry);
        }
    }

    /// <summary>
    /// Hide the targeting UI
    /// </summary>
    public void Hide()
    {
        if (tileUIEntries == null)
            tileUIEntries = new List<TileUI.Entry>();
        foreach(var obj in tileUIEntries)
            BattleGrid.main.RemoveTileUI(obj);
        tileUIEntries.Clear();
    }

    /// <summary>
    /// Set the current user and target position
    /// </summary>
    public void Target(Pos userPos, Pos targetPos)
    {
        TargetPos = type == Type.Absolute ? Pos.Zero : targetPos;
        UserPos = userPos;
    }
}
