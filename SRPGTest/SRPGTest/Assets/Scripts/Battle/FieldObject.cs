﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class FieldObject : MonoBehaviour
{
    public enum Team
    {
        Party,
        Enemy,
        Neutral,
    }
    public virtual Team Allegiance { get => Team.Neutral; }
    public int Row { get => pos.row; set => pos.row = value; }
    public int Col { get => pos.col; set => pos.col = value; }
    [SerializeField]
    private Pos pos;
    public Pos Pos { get => pos; set => pos = value; }

    private void Start()
    {
        Initialize();
    }

    public virtual bool CanMoveThrough(FieldObject other) => true;
    protected virtual void Initialize()
    {
        BattleGrid.main.SetObject(Pos, this);
    }

    public virtual bool Select() { return false; }
    public virtual void Highlight() { }
    public virtual void UnHighlight() { }
    public virtual Coroutine StartTurn() { return null; }    
    public virtual void OnPhaseStart() { }
    public virtual void OnPhaseEnd() { }
}