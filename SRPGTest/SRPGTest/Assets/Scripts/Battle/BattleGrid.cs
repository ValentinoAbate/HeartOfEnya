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

    public GameObject SpawnDebugSquare(Pos p)
    {
        return Instantiate(debugSquarePrefab, GetSpace(p), Quaternion.identity);
    }

    public Vector2 GetSpace(Pos pos) => GetSpace(pos.row, pos.col);
    public Vector2 GetSpace(int row, int col)
    {
        float y = transform.position.y - row * (cellSize.y + skew.y + linewidth) + linewidth;
        float x = transform.position.x + col * (cellSize.x + skew.x + linewidth) + linewidth;
        return new Vector2(x, y);
    }
    public FieldObject GetObject(Pos pos) => GetObject(pos.row, pos.col);
    public FieldObject GetObject(int row, int col)
    {
        if (IsLegal(row, col))
            return field[row, col];
        return null;
    }

    public void RemoveObject(FieldObject obj)
    {
        field[obj.Row, obj.Col] = null;
    }

    public bool IsEmpty(Pos pos) => IsEmpty(pos.row, pos.col);
    public bool IsEmpty(int row, int col)
    {
        return field[row, col] == null;
    }

    public bool IsLegal(Pos pos) => IsLegal(pos.row, pos.col);
    public bool IsLegal(int row, int col)
    {
        return field.Contains(row, col);
    }

    

    public bool Move(Pos src, Pos dest) => Move(src.row, src.col, dest.row, dest.col);
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

    public HashSet<Pos> Reachable(Pos startPos, int range, params FieldObject.ObjType[] nonTraversible)
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
                var positions = new List<Pos>();
                var travPos = node.Offset(0, -1);
                bool Traversible()
                {
                    return IsLegal(travPos) && (!distances.ContainsKey(travPos) || currDepth + 1 < distances[travPos]) &&
                        (field[travPos.row, travPos.col] == null || nonTraversible.All((t) => field[travPos.row, travPos.col].ObjectType != t));
                }
                if (Traversible())
                    positions.Add(travPos);
                travPos.col += 2;
                if (Traversible())
                    positions.Add(travPos);
                travPos = node.Offset(-1, 0);
                if (Traversible())
                    positions.Add(travPos);
                travPos.row += 2;
                if (Traversible())
                    positions.Add(travPos);
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
            if (currDepth >= range)
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

    public List<Pos> Path(Pos start, Pos goal, params FieldObject.ObjType[] nonTraversible)
    {
        List<Pos> NodeAdj(Pos p) => Adj(p, (pos) => Traversible(pos, nonTraversible));
        return AStar.Pathfind(start, goal, NodeAdj, (p, pAdj) => 1, Dist);
    }

    List<Pos> Adj(Pos node, System.Predicate<Pos> traversible)
    {
        var positions = new List<Pos>();
        var travPos = node.Offset(0, -1);
        if (traversible(travPos))
            positions.Add(travPos);
        travPos.col += 2;
        if (traversible(travPos))
            positions.Add(travPos);
        travPos = node.Offset(-1, 0);
        if (traversible(travPos))
            positions.Add(travPos);
        travPos.row += 2;
        if (traversible(travPos))
            positions.Add(travPos);
        return positions;
    }

    bool Traversible(Pos travPos, params FieldObject.ObjType[] nonTraversible)
    {
        return IsLegal(travPos) 
            && (field[travPos.row, travPos.col] == null || nonTraversible.All((t) => field[travPos.row, travPos.col].ObjectType != t));
    }

    float Dist(Pos p, Pos p2)
    {
        return Math.Abs(p2.row - p.row) + Math.Abs(p2.col - p.col);
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
        public Pos Offset(int rowOff, int colOff)
        {
            return new Pos(row + rowOff, col + colOff);
        }

        public override bool Equals(object obj)
        {
            return obj is Pos && Equals((Pos)obj);
        }

        public bool Equals(Pos other)
        {
            return row == other.row && col == other.col;
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


