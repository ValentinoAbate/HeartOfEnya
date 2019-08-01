using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public enum TType
    {
        Primary,
        Secondary,
    }
    [SerializeField]
    private Team allegience = Team.Neutral;
    public override Team Allegiance => allegience;
    [SerializeField]
    private TType tileType = TType.Primary;
    public TType TileType { get => tileType; }

    protected override void Initialize()
    {
        BattleGrid.main.AddEventTile(Pos, this);
    }

    public void OnSteppedOn(FieldObject steppedOn)
    {
        if (Allegiance != Team.Neutral && Allegiance != steppedOn.Allegiance)
            return;
        OnSteppedOnFn(steppedOn);
    }

    protected virtual void OnSteppedOnFn(FieldObject steppedOn) { }

}
