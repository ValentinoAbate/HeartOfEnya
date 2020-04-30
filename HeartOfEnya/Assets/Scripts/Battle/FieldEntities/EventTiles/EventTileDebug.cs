using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A EventTile used solely for debugging purposes.
/// Prints a debug statement when stepped on.
/// </summary>
public class EventTileDebug : EventTile
{
    protected override void OnSteppedOnFn(FieldObject steppedOn)
    {
        Debug.Log("Tile: " + name + " has been stepped on by " + steppedOn.DisplayName);
    }
}
