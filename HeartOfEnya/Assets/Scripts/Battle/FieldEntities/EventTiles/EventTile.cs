using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// An object that marks a tile as interactable with FieldObjects in some special way
/// The Allegience indicates what field objects can interact with this tile (Neutral == any allegience can interact).
/// There are two main types of EventTile: Primary and Secondary.
/// There can only be one Primary EventTile per grid square, while there can be any number of secondary event tiles.
/// Primary event tiles should have visual representation and should be used for player-facing effects (run tiles, etc.)
/// Secondary event tiles should not have visual representation and should be used for hidden effects (cutscene triggers, etc.)
/// The Type of a tile is stored in its TileType property
/// </summary>
public abstract class EventTile : FieldEntity
{
    /// <summary>
    /// The Type of event tile this is.
    /// Primary: there can only be one of these per Pos (usually displays graphics)
    /// Secondary: there can be as many of these per Pos as necessary (for cutscene triggers, etc)
    /// </summary>
    public enum Type
    {
        Primary,
        Secondary,
    }
    [SerializeField]
    private Teams allegience = Teams.Neutral;
    public override Teams Team => allegience;
    [SerializeField]
    private Type tileType = Type.Primary;
    /// <summary>
    /// There are two main types of EventTile: Primary and Secondary.
    /// There can only be one Primary EventTile per grid square, while there can be any number of secondary event tiles.
    /// Primary event tiles should have visual representation and should be used for player-facing effects (run tiles, etc.)
    /// Secondary event tiles should not have visual representation and should be used for hidden effects (cutscene triggers, etc.)
    /// </summary>
    public Type TileType { get => tileType; }

    protected override void Initialize()
    {
        BattleGrid.main.AddEventTile(Pos, this);
    }
    /// <summary>
    /// Function called when a unit steps on this tile (ends their move on the tile).
    /// To add functionailty when stepped on, override the OnSteppedOnFn() in a child class.
    /// </summary>
    public void OnSteppedOn(FieldObject obj)
    {
        if (Team != Teams.Neutral && Team != obj.Team)
            return;
        OnSteppedOnFn(obj);
    }
    /// <summary>
    /// Function called when a unit leaves this tile (starts their move on it and ends on a different tile)
    /// To add functionailty when stepped on, override the OnLeaveTileFn() in a child class.
    /// </summary>
    public void OnLeaveTile(FieldObject obj)
    {
        if (Team != Teams.Neutral && Team != obj.Team)
            return;
        OnLeaveTileFn(obj);
    }
    /// <summary>
    /// Function called when a unit of appropriate team steps on this tile
    /// </summary>
    protected virtual void OnSteppedOnFn(FieldObject steppedOn) { }
    /// <summary>
    /// Function called when a unit of appropriate team leaves this tile
    /// </summary>
    protected virtual void OnLeaveTileFn(FieldObject obj)
    {

    }

}
