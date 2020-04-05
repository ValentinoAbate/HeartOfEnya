using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Entity with a position on the Battle Grid and an Allegiance (Team)
/// This is the base class for anything that will go on the grid.
/// For something that actually occupies a space, see FieldObject.
/// </summary>
[DisallowMultipleComponent]
public abstract class FieldEntity : MonoBehaviour
{
    [System.Flags]
    public enum Teams
    {
        None = 0,
        Party = 1,
        Enemy = 2,
        Neutral = 4,
    }
    /// <summary>
    /// The team(s) this object is affiliated with.
    /// Some objects (like units) should really only have one team.
    /// However, some objects, like traps, make have multiple values for what teams they affect
    /// </summary>
    public virtual Teams Team { get => Teams.Neutral; }

    /// <summary>
    /// The entity's name as it should be displayed to the player in the game.
    /// </summary>
    public string DisplayName => displayName;
    [SerializeField]
    private string displayName = string.Empty;
    public int Row { get => pos.row; set => pos.row = value; }
    public int Col { get => pos.col; set => pos.col = value; }
    [SerializeField]
    private Pos pos;
    public Pos Pos { get => pos; set => pos = value; }

    private void Start()
    {
        Initialize();
    }
    /// <summary>
    /// Should be called during the Start() function.
    /// The class should inherit Start() from FieldEntity, but if Start() is overriden add a call to this function.
    /// </summary>
    protected abstract void Initialize();

    /// <summary>
    /// Called the player presses the conform control with the square containing this unit highlighted
    /// </summary>
    /// <returns></returns>
    public virtual bool Select() { return false; }
    /// <summary>
    /// Called when a cursor highlighting the square containing this unit
    /// </summary>
    public virtual void Highlight() { }
    /// <summary>
    /// Called when a cursor that was highlighting the sqaure containing this unit moves to a different square
    /// </summary>
    public virtual void UnHighlight() { }
    /// <summary>
    /// Called at the begninning of the phase for each unit managed by that phase
    /// Used to update timed effects and initialize UI.
    /// </summary>
    public virtual void OnPhaseStart() { }
    /// <summary>
    /// Called at the end of the phase for each unit managed by that phase
    /// Used to update timed effects and cleanup UI.
    /// </summary>
    public virtual void OnPhaseEnd() { }
}
