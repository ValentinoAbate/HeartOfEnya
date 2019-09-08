using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterManagerExplicit : EncounterManager
{
    public EncounterData data;
    public SpawnerDict spawners;

    public override void ProcessTurn(int turn)
    {
        if (!data.ContainsKey(turn))
            return;
        var waveData = data[turn];
        foreach(var kvp in waveData.spawnDict)
        {
            if (!spawners.ContainsKey(kvp.Key))
                continue;
            var spawn = spawners[kvp.Key];
            foreach (var obj in kvp.Value.Objects)
                if(obj != null)
                    spawn.SpawnFieldObject(obj);
        }
    }
}
