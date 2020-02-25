using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPhase : Phase
{
    public const float delaySeconds = 0.25f;
    public override PauseHandle PauseHandle { get; set; } = new PauseHandle(null);

    public Encounter mainEncounter;
    [SerializeField]
    private Encounter tutorialEncounter;
    [SerializeField]
    private Encounter luaEncounter;
    [SerializeField]
    private Encounter absoluteZeroEncounter;
    public Encounter CurrEncounter { get; private set;}
    public GameObject spawnTileEnemyPrefab;
    public GameObject spawnTileObstaclePrefab;
    public int spawnDamage = 2;
    public int startAtWave = 1;

    public WaveData CurrWave => waveNum < CurrEncounter.Waves.Length ? CurrEncounter.Waves[waveNum] : null;
    public WaveData NextWave => waveNum < CurrEncounter.Waves.Length - 1 ? CurrEncounter.Waves[waveNum + 1] : null;
    private List<EventTileSpawn> spawners = new List<EventTileSpawn>();
    // Start at negative one to account for first turn
    private int turnsSinceLastSpawn = 0;
    private int waveNum = 0;

    // Party Member prefab references for each level
    public List<GameObject> bapyLvl;
    public List<GameObject> soleilLvl;
    public List<GameObject> rainaLvl;
    public List<GameObject> luaLvl;

    // Party Member starting positions
    public Pos bapyPos;
    public Pos soleilPos;
    public Pos rainaPos;
    public Pos luaPos;

    // Playtest data logger reference
    public PlaytestLogger logger;

    private void Start()
    {
        logger = DoNotDestroyOnLoad.Instance.playtestLogger;

        CurrEncounter = mainEncounter;
        if (DoNotDestroyOnLoad.Instance?.persistentData?.gamePhase == null)
            return;
        // Go to next game phase, if applicable.
        string gamePhase = DoNotDestroyOnLoad.Instance.persistentData.gamePhase.ToUpper();
        if (gamePhase == PersistentData.gamePhaseTutorial)
            CurrEncounter = tutorialEncounter;
        else if (gamePhase == PersistentData.gamePhaseLuaBattle)
            CurrEncounter = luaEncounter;
        else if (gamePhase == PersistentData.gamePhaseAbsoluteZeroBattle)
            CurrEncounter = absoluteZeroEncounter;
    }

    public override Coroutine OnPhaseStart()
    {
        // If it is the first turn just spawn the enemies
        if (PhaseManager.main.Turn == 1)
        {
            // Spawn party members
            Vector2 bapyVec = BattleGrid.main.GetSpace(bapyPos);
            GameObject bapy = Instantiate(bapyLvl[DoNotDestroyOnLoad.Instance.persistentData.partyLevel], 
                                          new Vector3(bapyVec.x, bapyVec.y, 0), Quaternion.identity);
            bapy.GetComponent<PartyMember>().Pos = bapyPos;

            Vector2 soleilVec = BattleGrid.main.GetSpace(soleilPos);
            GameObject soleil = Instantiate(soleilLvl[DoNotDestroyOnLoad.Instance.persistentData.partyLevel], 
                                            new Vector3(soleilVec.x, soleilVec.y, 0), Quaternion.identity);
            soleil.GetComponent<PartyMember>().Pos = soleilPos;
            
            Vector2 rainaVec = BattleGrid.main.GetSpace(rainaPos);
            GameObject raina = Instantiate(rainaLvl[DoNotDestroyOnLoad.Instance.persistentData.partyLevel], 
                                           new Vector3(rainaVec.x, rainaVec.y, 0), Quaternion.identity);
            raina.GetComponent<PartyMember>().Pos = rainaPos;
            
            // Spawn Lua if the boss is defeated
            if(DoNotDestroyOnLoad.Instance.persistentData.luaBossDefeated)
            {
                Vector2 luaVec = BattleGrid.main.GetSpace(luaPos);
                GameObject lua = Instantiate(luaLvl[DoNotDestroyOnLoad.Instance.persistentData.partyLevel], 
                                             new Vector3(luaVec.x, luaVec.y, 0), Quaternion.identity);
                lua.GetComponent<PartyMember>().Pos = luaPos;
            }

            waveNum = startAtWave - 1;
            foreach (var spawnData in CurrWave.AllSpawns)
            {
                var obj = Instantiate(spawnData.spawnObject).GetComponent<FieldObject>();
                obj.PrefabOrigin = spawnData.spawnObject;
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
                    fieldObject.PrefabOrigin = spawner.SpawnData.spawnObject;
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
            // Log playtest data from previous wave
            logger.testData.NewDataLog(
                waveNum, DoNotDestroyOnLoad.Instance.persistentData.dayNum, CurrWave.numEnemies, "wave won"
            );
            logger.LogData(logger.testData);


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
