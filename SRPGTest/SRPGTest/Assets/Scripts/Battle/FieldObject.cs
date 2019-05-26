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
    public int Row { get; set; }
    public int Col { get; set; }
    public BattleGrid.Pos Pos { get => new BattleGrid.Pos(Row, Col); }

    public virtual bool Select() { return false; }
    public virtual void Highlight() { }
    public virtual void UnHighlight() { }
    public virtual void OnPhaseStart() { }
    public virtual void OnPhaseEnd() { }

    [System.Serializable]
    public class Matrix : Serializable2DMatrix<FieldObject>
    {
        public Matrix(int rows, int columns) : base(rows,columns)
        {

        }
    };
}
