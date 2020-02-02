using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newWave", menuName = "Wave Data")]
public class WaveData : ScriptableObject
{
    [Header("Next Wave Spawn Conditions")]
    public bool spawnAfterTurns = true;
    public int numTurns = 3;
    public bool spawnWhenNumberOfEnemiesRemain = true;
    public int numEnemies = 3;
    [Header("Wave Data")]
    public List<SpawnData> data = new List<SpawnData>();

    [System.Serializable]
    public class SpawnData
    {
        public GameObject enemy;
        public Pos spawnPosition;
    }
}
