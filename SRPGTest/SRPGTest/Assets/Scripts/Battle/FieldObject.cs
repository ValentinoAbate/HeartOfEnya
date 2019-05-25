using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldObject : MonoBehaviour
{
    public enum ObjectType
    {
        Party,
        Enemy,
        Object,
    }
    [SerializeField]
    private ObjectType objType;
    public ObjectType ObjType { get => objType; set => objType = value; }
    public int Row { get; set; }
    public int Col { get; set; }

    public virtual bool Select() { return false; }

    [System.Serializable]
    public class Matrix : Serializable2DMatrix<FieldObject>
    {
        public Matrix(int rows, int columns) : base(rows,columns)
        {

        }
    };
}
