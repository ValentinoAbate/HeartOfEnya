using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// An object that marks a tile as interactable with FieldObjects in some special way
/// The Allegience indicates what field objects can interact with this tile (Neutral == any allegience can interact)
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
    private Team allegience = Team.Neutral;
    public override Team Allegiance => allegience;
    [SerializeField]
    private Type tileType = Type.Primary;
    public Type TileType { get => tileType; }

    protected override void Initialize()
    {
        BattleGrid.main.AddEventTile(Pos, this);
    }

    public void OnLeaveTile(FieldObject obj)
    {
        if (Allegiance != Team.Neutral && Allegiance != obj.Allegiance)
            return;
        OnLeaveTileFn(obj);
    }

    public void OnSteppedOn(FieldObject obj)
    {
        if (Allegiance != Team.Neutral && Allegiance != obj.Allegiance)
            return;
        OnSteppedOnFn(obj);
    }

    protected virtual void OnSteppedOnFn(FieldObject steppedOn) { }

    protected virtual void OnLeaveTileFn(FieldObject obj)
    {

    }

}
