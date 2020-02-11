using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPhase : Phase
{
    public const float delaySeconds = 0.25f;
    public override PauseHandle PauseHandle { get; set; } = new PauseHandle(null);

    public Encounter encounter;
    public GameObject spawnTileEnemyPrefab;
    public GameObject spawnTileObstaclePrefab;
    public int spawnDamage = 2;

    public WaveData CurrWave => waveNum < encounter.waves.Count ? encounter.waves[waveNum] : null;
    public WaveData NextWave => waveNum < encounter.waves.Count - 1 ? encounter.waves[waveNum + 1] : null;
    private List<EventTileSpawn> spawners = new List<EventTileSpawn>();
    // Start at negative one to account for first turn
    private int turnsSinceLastSpawn = 0;
    private int waveNum = 0;

    public override Coroutine OnPhaseStart()
    {
        // If it is the first turn just spawn the enemies
        if (PhaseManager.main.Turn == 1)
        {
            foreach (var spawnData in CurrWave.AllSpawns)
            {
                var obj = Instantiate(spawnData.spawnObject).GetComponent<FieldObject>();
                obj.transform.position = BattleGrid.main.GetSpace(spawnData.spawnPosition);
                BattleGrid.main.SetObject(spawnData.spawnPosition, obj);
            }
            return null;
        }
        return StartCoroutine(OnPhaseStartCr());
    }
    
    private IEnumerator OnPhaseStartCr()
    {
        // If something is supposed to be spawned
        if(spawners.Count > 0)
        {
            turnsSinceLastSpawn = 0;
            foreach(var spawner in spawners)
            {
                var objInSquare = BattleGrid.main.GetObject(spawner.Pos);
                // The square is empty! Spawn the enemy.
                if(objInSquare == null)
                {
                    var fieldObject = Instantiate(spawner.SpawnData.spawnObject, spawner.transform.position, Quaternion.identity).GetComponent<FieldObject>();
                    BattleGrid.main.SetObject(spawner.Pos, fieldObject);
                    BattleGrid.main.RemoveEventTile(spawner.Pos, spawner);
                    Destroy(spawner.gameObject);
                }
                else // Else There is something occupying the square, so damage it.
                {
                    var combatant = objInSquare.GetComponent<Combatant>();
                    if(combatant != null)
                    {
                        combatant.Damage(spawnDamage);
                    }
                }
                yield return new WaitForSeconds(delaySeconds);
            }
            // Remove all spawners that spawned
            spawners.RemoveAll((s) => s == null);
        }
        else // Increase the turns since something was last spawned
        {
            ++turnsSinceLastSpawn;
        }
        // If there isn't a next wave, just yeet out of there
        if (NextWave == null)
            yield break;
        // Spawn the next wave if ready
        if (NextWaveReady())
        {
            // Declare next spawns
            foreach (var spawnData in NextWave.enemies)
            {
                var spawnTile = Instantiate(spawnTileEnemyPrefab).GetComponent<EventTileSpawn>();
                LogEventTile(spawnData, spawnTile);
                yield return new WaitForSeconds(delaySeconds);
            }
            foreach (var spawnData in NextWave.obstacles)
            {
                var spawnTile = Instantiate(spawnTileObstaclePrefab).GetComponent<EventTileSpawn>();
                LogEventTile(spawnData, spawnTile);
                yield return new WaitForSeconds(delaySeconds);
            }
            // Increment the wave counter
            ++waveNum;
        }
        yield break;
    }

    private void LogEventTile(WaveData.SpawnData spawnData, EventTileSpawn spawnTile)
    {
        spawnTile.transform.position = BattleGrid.main.GetSpace(spawnData.spawnPosition);
        spawnTile.SpawnData = spawnData;
        spawners.Add(spawnTile);
        BattleGrid.main.AddEventTile(spawnData.spawnPosition, spawnTile);
    }

    private bool NextWaveReady()
    {
        if(CurrWave.spawnWhenNumberOfEnemiesRemain &&
            PhaseManager.main.EnemyPhase.Enemies.Count <= CurrWave.numEnemies)
        {
            return true;
        }
        if(CurrWave.spawnAfterTurns && 
            turnsSinceLastSpawn >= CurrWave.numTurns )
        {
            return true;
        }
        return false;
    }

    public override Coroutine OnPhaseEnd()
    {
        return null;
    }

    public override void OnPhaseUpdate() => EndPhase();
}
