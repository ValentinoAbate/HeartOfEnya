using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BattleGrid : MonoBehaviour
{
    public int Rows { get => field.Rows; }
    public int Cols { get => field.Columns; }

    public static BattleGrid main;
    public GameObject debugSquarePrefab;
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
        if (IsLegal(row, col))
            return field[row, col];
        return null;
    }

    public bool IsEmpty(int row, int col)
    {
        return field[row, col] == null;
    }

    public bool IsLegal(int row, int col)
    {
        return field.Contains(row, col);
    }

    public bool Move(int srcRow, int srcCol, int dstRow, int dstCol)
    {
        if (!IsEmpty(dstRow, dstCol) || IsEmpty(srcRow, srcCol))
            return false;
        var obj = field[srcRow, srcCol];
        field[dstRow, dstCol] = obj;
        field[srcRow, srcCol] = null;
        obj.Row = dstRow;
        obj.Col = dstCol;
        return true;
    }

    public HashSet<Pos> Reachable(Pos startPos, int moveRange, params FieldObject.ObjType[] nonTraversible)
    {
        #region Initialize return set and distances
        var ret = new HashSet<Pos>();
        var distances = new Dictionary<Pos, int>();
        ret.Add(startPos);
        distances.Add(startPos, 0);
        #endregion

        // Inner recursive method
        void ReachableRecursive(Pos p, int currDepth)
        {
            // Inner adjacency method
            List<Pos> Adj(Pos node)
            {
                bool Traversible(int row, int col)
                {
                    Pos travPos = new Pos(row, col);
                    return IsLegal(row, col) && (!distances.ContainsKey(travPos) || currDepth + 1 < distances[travPos]) &&
                        (field[row, col] == null || nonTraversible.All((t) => field[row, col].ObjectType != t));
                }
                var positions = new List<Pos>();
                if (Traversible(node.row, node.col - 1))
                    positions.Add(new Pos(node.row, node.col - 1));
                if (Traversible(node.row, node.col + 1))
                    positions.Add(new Pos(node.row, node.col + 1));
                if (Traversible(node.row - 1, node.col))
                    positions.Add(new Pos(node.row - 1, node.col));
                if (Traversible(node.row + 1, node.col))
                    positions.Add(new Pos(node.row + 1, node.col));
                return positions;
            }
            // Log discovery and distance
            if (distances.ContainsKey(p))
            {
                if (distances[p] > currDepth)
                    distances[p] = currDepth;
            }
            else
            {
                distances.Add(p, currDepth);
                // Only Log in return if the square is empty or can be ended on
                if (field[p.row, p.col] == null || field[p.row, p.col].CanShareSquare)
                    ret.Add(p);
            }
            // End Recursion
            if (currDepth >= moveRange)
                return;
            // Get adjacent nodes
            var nodes = Adj(p);
            // Recur
            foreach (var node in nodes)
                ReachableRecursive(node, currDepth + 1);
        }
        // Start Recursion
        ReachableRecursive(startPos, 0);
        return ret;
    }

    public struct Pos : IEquatable<Pos>
    {
        public int row;
        public int col;
        public Pos(int row, int col)
        {
            this.row = row;
            this.col = col;
        }

        public override bool Equals(object obj)
        {
            return obj is Pos && Equals((Pos)obj);
        }

        public bool Equals(Pos other)
        {
            return row == other.row &&
                    col == other.col;
        }

        public override int GetHashCode()
        {
            var hashCode = -1720622044;
            hashCode = hashCode * -1521134295 + row.GetHashCode();
            hashCode = hashCode * -1521134295 + col.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return "row: " + row + " col: " + col;
        }

        public static bool operator ==(Pos pos1, Pos pos2)
        {
            return pos1.Equals(pos2);
        }

        public static bool operator !=(Pos pos1, Pos pos2)
        {
            return !(pos1 == pos2);
        }
    }

}


