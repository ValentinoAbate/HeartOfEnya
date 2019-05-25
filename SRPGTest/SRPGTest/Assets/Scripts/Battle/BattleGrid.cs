using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleGrid : MonoBehaviour
{
    public int Rows { get => field.Rows; }
    public int Cols { get => field.Columns; }

    public static BattleGrid main;
    public float linewidth;
    public Vector2Int dimensions;
    public Vector2 cellSize;
    public Vector2 skew;
    [SerializeField]
    private FieldObject.Matrix field;

    //Temp Debug Fields
    public List<Vector2Int> positions;
    public List<FieldObject> objects;
    private void Awake()
    {
        if (main == null)
        {
            main = this;
            Initialize();
        }
        else
            Destroy(gameObject);

    }

    private void Initialize()
    {
        field = new FieldObject.Matrix(dimensions.y, dimensions.x);
        for (int i = 0; i < Mathf.Min(positions.Count, objects.Count); ++i)
        {
            var pos = positions[i];
            field[pos.y, pos.x] = objects[i];
            objects[i].Col = pos.x;
            objects[i].Row = pos.y;
            objects[i].transform.position = GetSpace(pos.y, pos.x);
        }
    }

    public Vector2 GetSpace(int row, int col)
    {
        float y = transform.position.y - row * (cellSize.y + skew.y + linewidth) + linewidth;
        float x = transform.position.x + col * (cellSize.x + skew.x + linewidth) + linewidth;
        return new Vector2(x, y);
    }

    public FieldObject GetObject(int row, int col)
    {
        return field[row, col];
    }

    public bool IsEmpty(int row, int col)
    {
        return field[row, col] == null;
    }

    public bool Move(int srcRow, int srcCol, int dstRow, int dstCol)
    {
        if (!IsEmpty(dstRow, dstCol))
            return false;
        field[dstRow, dstCol] = field[srcRow, srcCol];
        field[srcRow, srcCol] = null;
        return true;
    }
}
