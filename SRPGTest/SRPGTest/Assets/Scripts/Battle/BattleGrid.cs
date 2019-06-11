using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[ExecuteInEditMode]
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
    private Matrix field;

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

    private void Update()
    {
        if(!Application.isPlaying && main == null)
        {
            main = this;
            Initialize();
        }
    }

    private void Initialize()
    {
        field = new Matrix(dimensions.y, dimensions.x);
    }

    #region Debugging

    public GameObject SpawnDebugSquare(Pos p)
    {
        return Instantiate(debugSquarePrefab, GetSpace(p), Quaternion.identity);
    }

    #endregion

    #region Field Methods

    public Vector2 GetSpace(Pos pos)
    {
        float y = transform.position.y - pos.row * (cellSize.y + skew.y + linewidth) + linewidth;
        float x = transform.position.x + pos.col * (cellSize.x + skew.x + linewidth) + linewidth;
        return new Vector2(x, y);
    }

    public Pos GetPos (Vector2 worldSpace)
    {
        int row = Mathf.FloorToInt(transform.position.y + 0.5f - worldSpace.y / (cellSize.y + skew.y + linewidth * 2));
        int col = Mathf.FloorToInt(worldSpace.x - transform.position.x / (cellSize.x + skew.x + linewidth * 2)) + 1;
        return new Pos(row, col);
    }

    public FieldObject GetObject(Pos pos)
    {
        if (IsLegal(pos))
            return field.Get(pos);
        return null;
    }

    public void SetObject(Pos pos, FieldObject value)
    {
        if(IsLegal(pos))
            field.Set(pos, value);
    }

    public void RemoveObject(FieldObject obj)
    {
        field.Set(obj.Pos, null);
    }

    public bool IsEmpty(Pos pos) => field.Get(pos) == null;

    public bool IsLegal(Pos pos) => field.Contains(pos.row, pos.col);

    public bool MoveAndSetPosition(FieldObject obj, Pos dest)
    {
        bool moveSuccess = Move(obj.Pos, dest);
        if (moveSuccess)
            obj.transform.position = GetSpace(obj.Pos);
        return moveSuccess;
    }

    public bool Move(Pos src, Pos dest)
    {
        if (!IsEmpty(dest) || IsEmpty(src))
            return false;
        var obj = field.Get(src);
        field.Set(dest, obj);
        field.Set(src, null);
        obj.Pos = dest;
        return true;
    }

    #endregion

    #region Pathing and Reachablilty

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
        return AStar.Pathfind(start, goal, NodeAdj, (p, pAdj) => 1, (p1, p2) => Pos.Distance(p1,p2));
    }

    List<Pos> Adj(Pos node, Predicate<Pos> traversible)
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

    public FieldObject SearchArea(Pos p, int range, Predicate<FieldObject> pred, params FieldObject.ObjType[] nonTraversible)
    {
        foreach(var node in Reachable(p, range, nonTraversible))
        {
            var obj = field.Get(node);
            if (pred(obj))
                return obj;
        }
        return null;
    }

    #endregion

    [System.Serializable]
    public class Matrix : Serializable2DMatrix<FieldObject>
    {
        public Matrix(int rows, int columns) : base(rows, columns)
        {

        }

        public void Set(Pos p, FieldObject value)
        {
            this[p.row, p.col] = value;
        }

        public FieldObject Get(Pos p)
        {
            return this[p.row, p.col];
        }
    };

}


