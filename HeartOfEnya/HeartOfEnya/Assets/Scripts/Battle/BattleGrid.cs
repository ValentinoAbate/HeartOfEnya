using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Top-Level class responsible for storing, organizing, and accessing all field entities involved with battle,
/// and displaying grid lines and grid UI (movement ranges, attack ranges, etc.).
/// Also containes utility methods for objects work with the graph, such as reachability and pathing algorithms.
/// Accessible as a singleton through the static reference BattleGrid.main.
/// Stores Field objects in a 2D matrix where each position may contain one and only one FieldObject (see FieldObject.cs),
/// and a dictionary where each position may contain one Primary EventTile and any number of secondary event tiles (see EventTile.cs).
/// Executes in edit mode so that editor positioning utilities can function properly.
/// </summary>
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
    public TileUI tileUIManager;

    private Matrix field;
    private Dictionary<Pos, List<EventTile>> eventTiles;

    /// <summary>
    /// Display grid lines in-editor when gizmos are drawn
    /// Doesn't run during runtime, even in-editor with gizmos enabled
    /// </summary>
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

    /// <summary>
    /// Implements the singleton pattern
    /// </summary>
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
        // Set the main reference during editor-time for editor utils
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

    /// <summary>
    /// Precalculate / bake grid skew math to optimize rendering and position calculation
    /// </summary>
    private void InitializeSkew()
    {
        float tan = Mathf.Tan(Mathf.Deg2Rad * skewAngle);
        skewXOffset = cellSize.y * tan;
    }

    #region Rendering

    /// <summary>
    /// Create and display the grid line objects
    /// Precondition: InitializeSkew has been called
    /// </summary>
    private void InitializeGridLines()
    {
        if (!Application.isPlaying)
            return;
        Vector2 offset = new Vector2((cellSize.x + skewXOffset) * 0.5f, -cellSize.y / 2);
        // Draw row lines
        for (int row = 0; row <= Rows; ++row)
        {
            var line = Instantiate(gridLinePrefab, transform).GetComponent<LineRenderer>();
            Vector2 point1 = GetSpace(new Pos(row, 0)) - offset;
            Vector2 point2 = GetSpace(new Pos(row, Cols)) - offset;
            line.SetPositions(new Vector3[] { point1, point2 });
        }
        // Drawn column lines
        for (int col = 0; col <= Cols; ++col)
        {
            var line = Instantiate(gridLinePrefab, transform).GetComponent<LineRenderer>();
            Vector2 point1 = GetSpace(new Pos(0, col)) - offset;
            Vector2 point2 = GetSpace(new Pos(Rows, col)) - offset;
            line.SetPositions(new Vector3[] { point1, point2 });
        }
    }

    /// <summary>
    /// Spawns a gridsquare prefab and initializes it's QuadMesh component with the proper vertices (if applicable)
    /// Optionally, applies a given mater to the mesh
    /// Used to display grid-related information such as movement and attack ranges
    /// </summary>
    /// <param name="p"> The grid position to spawn the square at </param>
    /// <param name="mat"> 
    /// The material to apply. The grid contains several references to materials (e.g BattleGrid.main.moveSquareMat) 
    /// </param>
    /// <returns> Returns the created object </returns>
    public TileUI.Entry SpawnTileUI(Pos p, TileUI.Type type)
    {
        if (!IsLegal(p))
            return new TileUI.Entry() { pos = Pos.OutOfBounds, type = TileUI.Type.Empty };
        // If there is already a tile, set the secondary type
        if (tileUIManager.HasActiveTileUI(p))
            return tileUIManager.AddType(p, type);

        // Calculate Vertices
        Vector2 offset = new Vector2((cellSize.x + skewXOffset) * 0.5f, -cellSize.y / 2);
        var v1 = GetSpace(p) - offset;
        var v2 = v1 + new Vector2(cellSize.x, 0);
        var v3 = GetSpace(p + Pos.Down) - offset;
        var v4 = v3 + new Vector2(cellSize.x, 0);
        // Else create a new tile
        return tileUIManager.SpawnTileUI(p, type, v1, v2, v3, v4);
    }

    public void RemoveTileUI(TileUI.Entry entry)
    {
        tileUIManager.RemoveType(entry);
    }

    #endregion

    #region Field Methods

    /// <summary>
    /// Get the world-space coordiante of the center of a square of the grid.
    /// Input does not need to be a legal position (see IsLegal(pos))
    /// </summary>
    public Vector2 GetSpace(Pos pos)
    {
        // If in editor mode and the application is not playing, make sure the skew is initialized
        if (Application.isEditor && !Application.isPlaying)
            InitializeSkew();
        float y = transform.position.y - pos.row * (cellSize.y);
        float x = transform.position.x + pos.col * (cellSize.x);
        x += skewXOffset * pos.row;
        return new Vector2(x, y);
    }

    /// <summary>
    /// Return the grid-space position of the sqaure that contains the given world space coordinate.
    /// May return a non-legal position (see IsLegal(pos))
    /// </summary>
    public Pos GetPos (Vector2 worldSpace)
    {
        float leniency = (cellSize.x / 2);
        float yleniency = cellSize.y / 2;
        int row = Mathf.FloorToInt(transform.position.y + yleniency - worldSpace.y / (cellSize.y));
        int col = Mathf.FloorToInt((worldSpace.x + leniency - transform.position.x - (skewXOffset * row)) / (cellSize.x));
        return new Pos(row, col);
    }

    public bool ContainsPoint(Vector2 worldSpace)
    {
        return IsLegal(GetPos(worldSpace));
    }

    /// <summary>
    /// Is this grid position legal (a.k.a can that grid position contain field objects and/or event tiles)?
    /// </summary>
    /// <param name="pos"> The position is grid space to test for legality </param>
    /// <returns> Is the space with in the grid's row and column dimensions </returns>
    public bool IsLegal(Pos pos) => field.Contains(pos.row, pos.col);

    #endregion

    #region Field Object Methods

    public bool IsEmpty(Pos pos) => field.Get(pos) == null;

    public bool IsEmptyEventTiles(Pos pos) => !eventTiles.ContainsKey(pos) || eventTiles[pos].Count <= 0;

    /// <summary>
    /// Return the object of type T at the given grid position, or null if the position is empty or filled with an object that is not of type T
    /// </summary>
    public T Get<T>(Pos pos) where T : FieldObject
    {
        if (IsLegal(pos))
        {
            // Avoid references to destroyed GameObjects not working with the ?. operator (return a ture null value)
            if (field.Get(pos) == null)
                return null;
            return field.Get(pos) as T;
        }
        return null;
    }

    public T Find<T>(Predicate<T> pred) where T : FieldObject
    {
        //brute force foreach of field; might optimize later
        foreach (var obj in field)
        {
            if (obj is T)
            {
                var objT = obj as T;
                //if object matches the predicate, add it to the list
                if (pred(objT))
                    return objT;
            }
        }
        return null;
    }

    public List<T> FindAll<T>(Predicate<T> pred = null) where T : FieldObject
    {
        var objects = new List<T>();
        //brute force foreach of field; might optimize later
        foreach (var obj in field)
        {
            if (obj is T)
            {
                var objT = obj as T;
                //if there is no predicate or object matches the predicate, add it to the list
                if (pred == null || pred(objT))
                    objects.Add(objT);
            }
        }
        return objects;
    }

    /// <summary>
    /// Set's the object at the given grid position to the given FieldObject reference (if pos is a legal Grid Position)
    /// </summary>
    public void SetObject(Pos pos, FieldObject value)
    {
        if(IsLegal(pos))
        {
            value.Pos = pos;
            field.Set(pos, value);
        }

    }
    
    /// <summary>
    /// Remove a field object from the grid. does not destroy the object or modify its world position.
    /// Set's the object's grid position to Pos.OutOfBounds, a non-legal position.
    /// </summary>
    public void RemoveObject(FieldObject obj)
    {
        field.Set(obj.Pos, null);
        obj.Pos = Pos.OutOfBounds;
    }

    /// <summary>
    /// Moves a field object to a new grid space and updates the objects world position if the object successfully moved.
    /// See Move(obj, dest) for success and failure conditions.
    /// Preconditdion: obj's Position is legal (see IsLegal())
    /// </summary>
    public bool MoveAndSetWorldPos(FieldObject obj, Pos dest)
    {
        bool moveSuccess = Move(obj, dest);
        if (moveSuccess)
            obj.transform.position = GetSpace(obj.Pos);
        return moveSuccess;
    }

    /// <summary>
    /// Move a FieldObject to a new grid space.
    /// Move fails if destination is illegal or occupied.
    /// Preconditdion: obj's Position is legal (see IsLegal())
    /// Does not update the object's world position. Use MoveAndSetWorldPos() to move and set world position.
    /// </summary>
    public bool Move(FieldObject obj, Pos dest)
    {
        // If the destination is not legal and empty, return
        if (!(IsLegal(dest) && IsEmpty(dest)))
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
        if (obj is Combatant)
        {
            var c = obj as Combatant;
            c.UpdateChargeTileUI();
        }
        return true;
    }

    /// <summary>
    /// Swap two Field Objects' grid positions and then update their world positions based on the new values.
    /// Preconditdion: both objs' Position are legal (see IsLegal()).
    /// </summary>
    public void SwapAndSetWorldPos(FieldObject obj1, FieldObject obj2)
    {
        Swap(obj1, obj2);
        // Set the transforms
        obj1.transform.position = GetSpace(obj1.Pos);
        obj2.transform.position = GetSpace(obj2.Pos);
    }

    /// <summary>
    /// Swap two Field Objects' grid positions.
    /// Preconditdion: both objs' Position are legal (see IsLegal()).
    /// Does not update the objects' world positions. Use SwapAndSetWorldPos() to swap and set world position.
    /// </summary>
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
        if (obj1 is Combatant)
        {
            var c = obj1 as Combatant;
            c.UpdateChargeTileUI();
        }
        if (obj2 is Combatant)
        {
            var c = obj2 as Combatant;
            c.UpdateChargeTileUI();
        }
    }

    #endregion

    #region Tile Event Methods

    /// <summary>
    /// Add an event tile to the event list for the given position (if position is legal).
    /// The event tile will not be added if it is a primary type and the position already has a primary event tile
    /// </summary>
    public void AddEventTile(Pos pos, EventTile e)
    {
        if (!IsLegal(pos))
            return;
        e.Pos = pos;
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
                // Look for primary event tiles that alread exist on this square
                var primaryTile = tiles.Find((et) => et.TileType == EventTile.Type.Primary);
                // Return if the square already contains a primary event tile (there may only be one)
                if (primaryTile != null)
                    return;
            }
            tiles.Add(e);
        }
    }

    /// <summary>
    /// Remove an event tile from the event list for the given position (if position is legal, and the list contains the given tile).
    /// </summary>
    public void RemoveEventTile(Pos pos, EventTile e)
    {
        if (!IsLegal(pos) || !eventTiles.ContainsKey(pos))
            return;
        var events = eventTiles[pos];
        events.Remove(e);
    }
    #endregion

    #region Pathing and Reachablilty

    /// <summary>
    /// Calculates the positions that can be reached from a given position, range, and predicate function defining what objects can be moved through.
    /// Takes into account obstacles, but spaces with objects considered to be traversable will still be returned in the dictionary,
    /// even if some mechanics that utilize this method (such as movement) should exclude those spaces later.
    /// </summary>
    /// <param name="startPos"> The grid position to start looking from </param>
    /// <param name="range"> The movement range to serach </param>
    /// <param name="canMoveThrough"> A predicate function determining if a given object is traversable </param>
    /// <returns> A dictionary of reachable grid positions to ints where the ints are the minimum distance to the position. </returns>
    public Dictionary<Pos,int> Reachable(Pos startPos, int range, Predicate<FieldObject> canMoveThrough)
    {
        // Initialize distances with the startPosition
        var distances = new Dictionary<Pos, int> { { startPos, 0 } };

        // Inner recursive method
        void ReachableRecursive(Pos p, int currDepth)
        {
            // Inner Traversability method
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
            // If depth is greater than range, end recursion
            if (currDepth >= range)
                return;
            // Get adjacent nodes (traversability function inverted as the Adj function takes a function for non-traversability)
            var nodes = Adj(p, (travPos) => !Traversable(travPos));
            // Recur
            foreach (var node in nodes)
                ReachableRecursive(node, currDepth + 1);
        }

        // Start Recursion
        ReachableRecursive(startPos, 0);
        return distances;
    }

    /// <summary>
    /// Returns a minimum distance path from the start to the goal, or null if a path doesn't exist.
    /// Based on the AStar pathfinding defined in AStar.Pathfinf
    /// </summary>
    /// <param name="start"> Start position in grid space </param>
    /// <param name="goal"> Goal position in grid space </param>
    /// <param name="canMoveThrough"> A pedicate function defining which field objects can be moved through </param>
    public List<Pos> Path(Pos start, Pos goal, Predicate<FieldObject> canMoveThrough)
    {
        List<Pos> NodeAdj(Pos p) => Adj(p, (pos) => StdNonTraversable(pos, canMoveThrough));
        return AStar.Pathfind(start, goal, NodeAdj, (p, pAdj) => 1, (p1, p2) => Pos.Distance(p1,p2));
    }

    /// <summary>
    /// Helper function for grid adjacency. Used in Reachability and pathfinding tests
    /// </summary>
    private List<Pos> Adj(Pos pos, Predicate<Pos> nonTraversable)
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

    /// <summary>
    /// Standard traversability function for use in pathing and reachability calls
    /// </summary>
    private bool StdTraversable(Pos pos, Predicate<FieldObject> canMoveThrough)
    {
        if (!IsLegal(pos))
            return false;
        return canMoveThrough(field.Get(pos));
    }

    /// <summary>
    /// Standard non-traversability function for use in pathing and reachability calls
    /// </summary>
    private bool StdNonTraversable(Pos pos, Predicate<FieldObject> canMoveThrough) => !StdTraversable(pos, canMoveThrough);

    /// <summary>
    /// Search an area defined by a reachability ranch for an object that matches a predicate.
    /// Returns the first object found, or null if none are found.
    /// </summary>
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

    /// <summary>
    /// Helper class made to store the underlying grid of FieldObject references.
    /// </summary>
    [System.Serializable]
    public class Matrix : SerializableCollections.SMatrix2D<FieldObject>
    {
        public Matrix(int rows, int columns) : base(rows, columns)
        {

        }

        public void Set(Pos p, FieldObject value)
        {
            if (!Contains(p.row, p.col))
                return;
            this[p.row, p.col] = value;
        }

        public FieldObject Get(Pos p)
        {
            if (!Contains(p.row, p.col))
                return null;
            return this[p.row, p.col];
        }
    };
}


