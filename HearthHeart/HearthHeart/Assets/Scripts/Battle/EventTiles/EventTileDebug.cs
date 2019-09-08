using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTileDebug : EventTile
{
    protected override void OnSteppedOnFn(FieldObject steppedOn)
    {
        Debug.Log("Tile: " + name + " has been stepped on by " + steppedOn.name);
    }
}
