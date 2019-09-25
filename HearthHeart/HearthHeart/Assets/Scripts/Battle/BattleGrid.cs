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

    [Header("Grid")]
    public Vector2Int dimensions;
    public Vector2 cellSize;
    public float skewAngle;
    private float skewXOffset;
    [Header("Rendering")]
    public GameObject gridLinePrefab;
    public GameObject gridSquarePrefab;
    public Material attackSquareMat;
    public Material moveSquareMat;
    public Material targetSquareMat;
    private Matrix field;
    private Dictionary<Pos, List<EventTile>> eventTiles;

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;
        Gizmos.color = Color.black;
        Vector2 offset = new Vector2((cellSize.x + skewXOffset) * 0.5f, -cellSize.y / 2);
        for(int row = 0; row <= Rows; ++row)
        {
            Vector2 point1 = GetSpace(new Pos(row, 0)) - offset;
            Vector2 point2 = GetSpace(new Pos(row, Cols)) - offset;
            Gizmos.DrawLine(point1, point2);
        }
        for (int col = 0; col <= Cols; ++col)
        {
            Vector2 point1 = GetSpace(new Pos(0, col)) - offset;
            Vector2 point2 = GetSpace(new Pos(Rows, col)) - offset;
            Gizmos.DrawLine(point1, point2);
        }
    }

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
        eventTiles = new Dictionary<Pos, List<EventTile>>();
        InitializeSkew();
        InitializeGridLines();
    }

    private void InitializeSkew()
    {
        float tan = Mathf.Tan(Mathf.Deg2Rad * skewAngle);
        skewXOffset = cellSize.y * tan;
    }

    #region Rendering

    private void InitializeGridLines()
    {
        if (!Application.isPlaying)
            return;
        Vector2 offset = new Vector2((cellSize.x + skewXOffset) * 0.5f, -cellSize.y / 2);
        for (int row = 0; row <= Rows; ++row)
        {
            var line = Instantiate(gridLinePrefab, transform).GetComponent<LineRenderer>();
            Vector2 point1 = GetSpace(new Pos(row, 0)) - offset;
            Vector2 point2 = GetSpace(new Pos(row, Cols)) - offset;
            line.SetPositions(new Vector3[] { point1, point2 });
        }
        for (int col = 0; col <= Cols; ++col)
        {
            var line = Instantiate(gridLinePrefab, transform).GetComponent<LineRenderer>();
            Vector2 point1 = GetSpace(new Pos(0, col)) - offset;
            Vector2 point2 = GetSpace(new Pos(Rows, col)) - offset;
            line.SetPositions(new Vector3[] { point1, point2 });
        }
    }

    public GameObject SpawnSquare(Pos p, Material mat = null)
    {
        var obj = Instantiate(gridSquarePrefab);
        var qMesh = obj.GetComponent<QuadMesh>();
        if(qMesh != null)
        {
            Vector2 offset = new Vector2((cellSize.x + skewXOffset) * 0.5f, -cellSize.y / 2);
            var v1 = GetSpace(p) - offset;
            var v2 = v1 + new Vector2(cellSize.x, 0);
            var v3 = GetSpace(p + Pos.Down) - offset;
            var v4 = v3 + new Vector2(cellSize.x, 0);
            qMesh.SetMesh(v1, v2, v3, v4);
            if (mat != null)
                qMesh.SetMaterial(mat);
        }
        return obj;
    }

    #endregion

    #region Field Methods

    public Vector2 GetSpace(Pos pos)
    {
        if (Application.isEditor && !Application.isPlaying)
            InitializeSkew();
        float y = transform.position.y - pos.row * (cellSize.y);
        float x = transform.position.x + pos.col * (cellSize.x);
        x += skewXOffset * pos.row;
        return new Vector2(x, y);
    }

    public Pos GetPos (Vector2 worldSpace)
    {
        float leniency = (cellSize.x / 2);
        int row = Mathf.FloorToInt(transform.position.y - worldSpace.y / (cellSize.y));
        int col = Mathf.FloorToInt((worldSpace.x + leniency - transform.position.x - (skewXOffset * row)) / (cellSize.x));
        return new Pos(row, col);
    }

    public bool IsLegal(Pos pos) => field.Contains(pos.row, pos.col);

    #endregion

    #region Field Object Methods

    public bool IsEmpty(Pos pos) => field.Get(pos) == null;

    public FieldObject GetObject(Pos pos)
    {
        if (IsLegal(pos))
        {
            // Avoid references to destroyed GameObjects not working with the ?. operator
            if (field.Get(pos) == null)
                return null;
            return field.Get(pos);
        }
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
        obj.Pos = Pos.OutOfBounds;
    }

    public bool MoveAndSetPosition(FieldObject obj, Pos dest)
    {
        bool moveSuccess = Move(obj, dest);
        if (moveSuccess)
            obj.transform.position = GetSpace(obj.Pos);
        return moveSuccess;
    }

    public bool Move(FieldObject obj, Pos dest)
    {
        if (!IsEmpty(dest))
            return false;
        Pos src = obj.Pos;
        // Clean up any event tiles on the square left from
        if (eventTiles.ContainsKey(src))
            eventTiles[src].ForEach((et) => et.OnLeaveTile(obj));
        field.Set(src, null);
        field.Set(dest, obj);
        obj.Pos = dest;
        // Activate any event tiles if present on the square moved to
        if (eventTiles.ContainsKey(dest))
            eventTiles[dest].ForEach((et) => et.OnSteppedOn(obj));
        return true;
    }

    public void Swap(FieldObject obj1, FieldObject obj2)
    {
        // Clean up any event tiles on the square left from
        if (eventTiles.ContainsKey(obj1.Pos))
            eventTiles[obj1.Pos].ForEach((et) => et.OnLeaveTile(obj1));
        if (eventTiles.ContainsKey(obj2.Pos))
            eventTiles[obj2.Pos].ForEach((et) => et.OnLeaveTile(obj2));
        // Swap positions
        Pos temp = obj1.Pos;
        obj1.Pos = obj2.Pos;
        obj2.Pos = temp;
        // Reset the grid data
        field.Set(obj1.Pos, obj1);       
        field.Set(obj2.Pos, obj2);       
        // Activate any event tiles if present on the square moved to
        if (eventTiles.ContainsKey(obj1.Pos))
            eventTiles[obj1.Pos].ForEach((et) => et.OnSteppedOn(obj1));
        if (eventTiles.ContainsKey(obj2.Pos))
            eventTiles[obj2.Pos].ForEach((et) => et.OnSteppedOn(obj2));
        // Set the transforms
        obj1.transform.position = GetSpace(obj1.Pos);
        obj2.transform.position = GetSpace(obj2.Pos);
    }

    #endregion

    #region Tile Event Methods
    public void AddEventTile(Pos pos, EventTile e)
    {
        if (!IsLegal(pos))
            return;
        if(!eventTiles.ContainsKey(pos))
        {
            eventTiles.Add(pos, new List<EventTile>() { e });
        }
        else
        {
            var tiles = eventTiles[pos];
            // If the tile we are trying to add is a primary type
            if (e.TileType == EventTile.Type.Primary)
            {
                var primaryTile = tiles.Find((et) => et.TileType == EventTile.Type.Primary);
                if (primaryTile != null)
                    return;
            }
            tiles.Add(e);
        }
    }
    public void RemoveEventTile(Pos pos, EventTile e)
    {
        if (!IsLegal(pos) || !eventTiles.ContainsKey(pos))
            return;
        var events = eventTiles[pos];
        if (events.Count == 1)
            eventTiles.Remove(pos);
        else
            events.Remove(e);
    }
    #endregion

    #region Pathing and Reachablilty

    public Dictionary<Pos,int> Reachable(Pos startPos, int range, Predicate<FieldObject> canMoveThrough)
    {
        // Initialize distances with the startPosition
        var distances = new Dictionary<Pos, int> { { startPos, 0 } };

        // Inner recursive method
        void ReachableRecursive(Pos p, int currDepth)
        {
            bool Traversable(Pos pos)
            {
                return (!distances.ContainsKey(pos) || currDepth + 1 < distances[pos]) && StdTraversable(pos, canMoveThrough);
            };

            // Log discovery and distance
            if (distances.ContainsKey(p))
            {
                if (distances[p] > currDepth)
                    distances[p] = currDepth;
            }
            else
            {
                distances.Add(p, currDepth);
            }
            // End Recursion
            if (currDepth >= range)
                return;
            // Get adjacent nodes
            var nodes = Adj(p, (travPos) => !Traversable(travPos));
            // Recur
            foreach (var node in nodes)
                ReachableRecursive(node, currDepth + 1);
        }
        // Start Recursion
        ReachableRecursive(startPos, 0);
        return distances;
    }

    public List<Pos> Path(Pos start, Pos goal, Predicate<FieldObject> canMoveThrough)
    {
        List<Pos> NodeAdj(Pos p) => Adj(p, (pos) => StdNonTraversable(pos, canMoveThrough));
        return AStar.Pathfind(start, goal, NodeAdj, (p, pAdj) => 1, (p1, p2) => Pos.Distance(p1,p2));
    }

    List<Pos> Adj(Pos pos, Predicate<Pos> nonTraversable)
    {
        var positions = new List<Pos>()
        {
            pos.Offset( 0, 1),
            pos.Offset( 0,-1),
            pos.Offset( 1, 0),
            pos.Offset(-1, 0),
        };
        positions.RemoveAll(nonTraversable);
        return positions;
    }

    bool StdTraversable(Pos pos, Predicate<FieldObject> canMoveThrough)
    {
        if (!IsLegal(pos))
            return false;
        return canMoveThrough(field.Get(pos));
    }

    bool StdNonTraversable(Pos pos, Predicate<FieldObject> canMoveThrough) => !StdTraversable(pos, canMoveThrough);

    public FieldObject SearchArea(Pos p, int range, Predicate<FieldObject> canMoveThrough, Predicate<FieldObject> pred)
    {
        foreach(var node in Reachable(p, range, canMoveThrough).Keys)
        {
            var obj = field.Get(node);
            if (pred(obj))
                return obj;
        }
        return null;
    }

    #endregion

    [System.Serializable]
    public class Matrix : SerializableCollections.SMatrix2D<FieldObject>
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


