using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveData
{
    public SpawnDict spawnDict = new SpawnDict();
    [System.Serializable]
    public class SpawnDict : SerializableCollections.SDictionary<EncounterManager.SpawnerCode, SpawnSet> { }
    [System.Serializable]
    public class SpawnSet
    {
        public int numObjects = 0;
        public List<FieldObject> objects = new List<FieldObject>();
    }
}
