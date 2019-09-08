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
        public List<FieldObject> Objects => new List<FieldObject> { obj1, obj2, obj3 };
        public FieldObject obj1 = null;
        public FieldObject obj2 = null;
        public FieldObject obj3 = null;
    }
}
