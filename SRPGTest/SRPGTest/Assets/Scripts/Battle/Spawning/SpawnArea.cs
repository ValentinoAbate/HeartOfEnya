using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandomUtils.Shuffle;

public class SpawnArea : MonoBehaviour
{
    private EventTileSpawn[] spawnTiles;
    private void Awake()
    {
        FindSpawnTiles();
    }

    public void FindSpawnTiles()
    {
        spawnTiles = GetComponentsInChildren<EventTileSpawn>();
    }

    public bool SpawnFieldObject(FieldObject objectPrefab)
    {
        spawnTiles.Shuffle();
        Pos spawnPos = Pos.Zero;
        bool foundPosition = false;
        foreach(var tile in spawnTiles)
        {
            if(BattleGrid.main.IsEmpty(tile.Pos))
            {
                spawnPos = tile.Pos;
                foundPosition = true;
                break;
            }
        }
        if (!foundPosition)
            return false;
        var obj = Instantiate(objectPrefab.gameObject).GetComponent<FieldObject>();
        BattleGrid.main.SetObject(spawnPos, obj);
        return true;    
    }
}
