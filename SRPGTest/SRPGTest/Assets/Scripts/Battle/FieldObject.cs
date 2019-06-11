using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldObject : MonoBehaviour
{
    public enum ObjType
    {
        Party,
        Enemy,
        Item,
        Obstacle,
    }
    public bool CanShareSquare { get => objType == ObjType.Item; }
    [SerializeField]
    private ObjType objType;
    public ObjType ObjectType { get => objType; set => objType = value; }
    public int Row { get => pos.row; set => pos.row = value; }
    public int Col { get => pos.col; set => pos.col = value; }
    [SerializeField]
    private Pos pos;
    public Pos Pos { get => pos; set => pos = value; }

    private void Start()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        BattleGrid.main.SetObject(Pos, this);
    }

    public virtual bool Select() { return false; }
    public virtual void Highlight() { }
    public virtual void UnHighlight() { }
    public virtual void OnPhaseStart() { }
    public virtual void OnPhaseEnd() { }
}
