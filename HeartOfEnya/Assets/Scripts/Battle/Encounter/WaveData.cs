using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "newWave", menuName = "Wave Data")]
public class WaveData : ScriptableObject
{
    [Header("Next Wave Spawn Conditions")]
    public bool spawnAfterTurns = true;
    public int numTurns = 3;
    public bool spawnWhenNumberOfEnemiesRemain = true;
    public int numEnemies = 3;
    [Header("Wave Data")]
    public List<SpawnData> enemies = new List<SpawnData>();
    public List<SpawnData> obstacles = new List<SpawnData>();

    public List<SpawnData> AllSpawns { get => enemies.Concat(obstacles).ToList(); }

    [System.Serializable]
    public class SpawnData
    {
        public GameObject spawnObject;
        public Pos spawnPosition;
    }
}
