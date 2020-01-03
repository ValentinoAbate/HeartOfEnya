using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Entity with a position on the Battle Grid and an Allegiance
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
    public virtual Teams Team { get => Teams.Neutral; }
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

    protected abstract void Initialize();

    public virtual bool Select() { return false; }
    public virtual void Highlight() { }
    public virtual void UnHighlight() { }
    public virtual Coroutine StartTurn() { return null; }
    public virtual void OnPhaseStart() { }
    public virtual void OnPhaseEnd() { }

}
