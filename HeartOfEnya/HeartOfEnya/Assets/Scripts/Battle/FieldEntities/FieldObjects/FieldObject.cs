using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object with a position on the battlefield that other FieldObjects cannot share a square with.
/// These are the main interactable object on the field (party members, obstacles, enemies, etc.)
/// Can share a space with other FieldEntities, such as EventTiles.
/// </summary>
public class FieldObject : FieldEntity
{
    /// <summary>
    /// The object allegiences this object can move.
    /// </summary>
    public Teams canMoveThrough;
    /// <summary>
    /// Where the vfx should spawn
    /// </summary>
    public Vector3 VfxSpawnPoint { get => vfxSpawnPoint.transform.position; }
    [SerializeField]
    private GameObject vfxSpawnPoint;

    public Pos OriginalPos { get; protected set; }
    /// <summary>
    ///  A reference to the prefab this entity was spawned from. should be set in the spawnphase when spawned.
    ///  If the entity was not spawned, will be null
    /// </summary>
    public GameObject PrefabOrigin { get; set; }

    /// <summary>
    /// The default can move through predicate used for pathing and reachability
    /// </summary>
    public virtual bool CanMoveThrough(FieldObject other)
    {
        // Empty spaces (spaces which contain a null object) can always be moved through
        if (other == null)
            return true;
        return canMoveThrough.HasFlag(other.Team);
    }
    /// <summary>
    /// Set the object's position in the BattleGrid.
    /// </summary>
    protected override void Initialize()
    {
        if(BattleGrid.main == null)
        {
            Debug.LogError("FieldObject: " + DisplayName + " could not be added to the battlegrid because main is null.");
        }
        else
        {
            OriginalPos = Pos;
            BattleGrid.main.SetObject(Pos, this);
        }      
    }
}
