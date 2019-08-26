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
    private Dictionary<Pos, List<EventTile>> eventTiles;

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
    }

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
        // Activate any event tiles if present
        if (eventTiles.ContainsKey(dest))
            eventTiles[dest].ForEach((et) => et.OnSteppedOn(obj));
        return true;
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
            if (e.TileType == EventTile.TType.Primary)
            {
                var primaryTile = tiles.Find((et) => et.TileType == EventTile.TType.Primary);
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


