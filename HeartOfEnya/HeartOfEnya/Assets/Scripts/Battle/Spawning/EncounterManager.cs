using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EncounterManager : MonoBehaviour
{
    public enum SpawnerCode
    {
        Normal,
        Special,
        Special2,
        Special3,
    }

    public abstract void ProcessTurn(int Turn);


    [System.Serializable] public class EncounterData : SerializableCollections.SDictionary<int, WaveData> { }

    [System.Serializable] public class SpawnerDict : SerializableCollections.SDictionary<SpawnerCode, SpawnArea> { }
}
