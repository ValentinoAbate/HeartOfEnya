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

    public virtual bool Select() { return false; }
    public virtual void Highlight() { }
    public virtual void UnHighlight() { }
    public virtual Coroutine StartTurn() { return null; }
    public virtual void OnPhaseStart() { }
    public virtual void OnPhaseEnd() { }
}
