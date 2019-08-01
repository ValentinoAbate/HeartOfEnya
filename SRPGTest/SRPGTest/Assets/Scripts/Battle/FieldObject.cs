using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object with a position on the battlefield that other FieldObjects cannot end their movement on
/// Can share a space with other FieldEntities, such as EventTiles
/// </summary>
public class FieldObject : FieldEntity
{
    public virtual bool CanMoveThrough(FieldObject other) => true;
    protected override void Initialize()
    {
        BattleGrid.main.SetObject(Pos, this);
    }
}
