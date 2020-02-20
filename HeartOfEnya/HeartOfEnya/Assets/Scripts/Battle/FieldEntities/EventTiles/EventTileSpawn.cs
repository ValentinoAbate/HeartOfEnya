using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An event tile that should be on tiles where enemies spawn.
/// </summary>
public class EventTileSpawn : EventTile
{
    public WaveData.SpawnData SpawnData { get; set; }  
}
