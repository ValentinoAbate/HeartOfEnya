using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialog;

public class SpawnPhase : Phase
{
    public DialogUI dialog;
    public const float delaySeconds = 0.25f;

    public override PauseHandle PauseHandle { get; set; } = new PauseHandle(null);
    public bool HasActiveSpawners => spawners.Count > 0;
    public Encounter CurrEncounter { get; private set; }

    [Header("Encounters")]
    [SerializeField]
    private Encounter tutDay1Encounter = null;
    [SerializeField]
    private Encounter tutDay2Encounter = null;
    [SerializeField]
    private Encounter tutDay3Encounter = null;
    [SerializeField]
    private Encounter luaEncounter = null;    
    [SerializeField]
    private Encounter luaEncounterPhase2Standalone = null;
    [SerializeField]
    private Encounter mainEncounter = null;
    [SerializeField]
    private Encounter absoluteZeroEncounter = null;
    [SerializeField]
    public Encounter levelEditorEncounter;

    [Header("Spawning Properties")]
    public GameObject spawnTileEnemyPrefab;
    public GameObject spawnTileObstaclePrefab;
    public int spawnDamage = 2;

    [Header("Level Editing Properties")]
    // Should we spawn enemies (used for the level editor)
    public bool spawnEnemies = true;
    public bool overrideSpawnLua = false;
    public int startAtWave = 1;

    public WaveData CurrWave => waveNum < CurrEncounter.Waves.Length ? CurrEncounter.Waves[waveNum] : null;
    public WaveData NextWave => waveNum < CurrEncounter.Waves.Length - 1 ? CurrEncounter.Waves[waveNum + 1] : null;
    private List<EventTileSpawn> spawners = new List<EventTileSpawn>();
    // Start at negative one to account for first turn
    private int turnsSinceLastWaveSpawn = 0;
    [HideInInspector] public int waveNum = 0;

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
    private PlaytestLogger logger;
    private List<GameObject> absolute0Bank = null;
    [Header("Abs0 encounter spawn parameters")]
    public List<Pos> abs0ReinforcementSpawnPositions = new List<Pos>();
    public int minAbs0Enemies = 5;
    public int abs0ReinforcementsSpawnNumber = 4;
    private bool endAfterFirstSpawnAbs0 = true;

    private void Start()
    {
        logger = DoNotDestroyOnLoad.Instance.playtestLogger;
        CurrEncounter = mainEncounter;
        if (DoNotDestroyOnLoad.Instance?.persistentData?.gamePhase == null)
            return;
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        // Go to next game phase, if applicable.
        string gamePhase = pData.gamePhase.ToUpper();
        if (pData.InTutorialFirstDay)
            CurrEncounter = tutDay1Encounter;
        else if (pData.InTutorialSecondDay)
            CurrEncounter = tutDay2Encounter;        
        else if (pData.InTutorialThirdDay)
            CurrEncounter = tutDay3Encounter;
        else if (pData.InLuaBattle)
            CurrEncounter = pData.luaBossPhase1Defeated ? luaEncounterPhase2Standalone : luaEncounter;
        else if (gamePhase == PersistentData.gamePhaseAbsoluteZeroBattle)
        {
            CurrEncounter = absoluteZeroEncounter;
            absolute0Bank = GetAbs0Bank();
        }
        else if (gamePhase == "LE")
        {
            CurrEncounter = levelEditorEncounter;
        }
    }

    private List<GameObject> GetAbs0Bank()
    {
        var enemies = new List<GameObject>();
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        // Add saved enemies first
        foreach(var savedEnemy in pData.listEnemiesLeft)
        {
            enemies.Add(savedEnemy.prefabAsset);
        }
        // Added saved spawns second
        foreach(var savedSpawn in pData.listActiveSpawners)
        {
            enemies.Add(savedSpawn.spawnObject);
        }
        // Add remaining unspawned waves
        for(int waveNum = pData.waveNum + 1; waveNum < mainEncounter.Waves.Length; ++waveNum)
        {
            foreach (var spawnData in mainEncounter.Waves[waveNum].enemies)
            {
                enemies.Add(spawnData.spawnObject);
            }
        }
        return enemies;
    }

    /// <summary>
    /// Initialization function called on Phase start if it is turn 1
    /// </summary>
    private void Initialize()
    {
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        int level = pData.PartyLevel;
        // Spawn party members
        Vector2 bapyVec = BattleGrid.main.GetSpace(bapyPos);
        var bapy = Instantiate(bapyLvl[level], bapyVec, Quaternion.identity).GetComponent<PartyMember>();
        dialog.characters.Add(bapy.GetComponent<Character>());
        bapy.Pos = bapyPos;

        Vector2 soleilVec = BattleGrid.main.GetSpace(soleilPos);
        var soleil = Instantiate(soleilLvl[level], soleilVec, Quaternion.identity).GetComponent<PartyMember>();
        dialog.characters.Add(soleil.GetComponent<Character>());
        soleil.Pos = soleilPos;

        Vector2 rainaVec = BattleGrid.main.GetSpace(rainaPos);
        var raina = Instantiate(rainaLvl[level], rainaVec, Quaternion.identity).GetComponent<PartyMember>();
        dialog.characters.Add(raina.GetComponent<Character>());
        raina.Pos = rainaPos;

        bool enableLua = pData.LuaUnfrozen || overrideSpawnLua;
        PartyMember lua = null;

        // Spawn Lua if enabled
        if (enableLua)
        {
            Vector2 luaVec = BattleGrid.main.GetSpace(luaPos);
            lua = Instantiate(luaLvl[level], luaVec, Quaternion.identity).GetComponent<PartyMember>();
            dialog.characters.Add(lua.GetComponent<Character>());
            lua.Pos = luaPos;
        }

        // Safety check for null buff list
        if (pData.buffStructures != null)
        {
            //Apply Soup Buffs
            foreach (var buff in pData.buffStructures)
            {
                // Default to bapy
                PartyMember chara;
                if (buff.targetCharacter == BuffStruct.Target.lua)
                {
                    if (!enableLua)
                        continue;
                    chara = lua;
                }
                else if (buff.targetCharacter == BuffStruct.Target.soleil)
                    chara = soleil;
                else if (buff.targetCharacter == BuffStruct.Target.raina)
                    chara = raina;
                else
                    chara = bapy;
                if (buff.effectType == BuffStruct.Effect.heal)
                {
                    chara.maxHp += 2;
                }
                else if (buff.effectType == BuffStruct.Effect.restore)
                {
                    chara.maxFp += 1;
                }
            }
        }

        // Don't spawn enemies
        if (!spawnEnemies || (pData.InMainPhase && pData.numEnemiesLeft <= 0 && pData.gamePhase != PersistentData.gamePhaseBeginMain))
            return;

        // This is a fresh encounter or a boss fight, just spawn everything
        if (CurrEncounter != pData.lastEncounter || pData.InLuaBattle || pData.InAbs0Battle)
        {
            waveNum = startAtWave - 1;
            SpawnAllEnemiesAndObstacles(CurrWave);
            int totalEnemies = 0;
            if (CurrEncounter == mainEncounter)
            {
                foreach (var wave in CurrEncounter.waveList)
                {
                    totalEnemies += wave.enemies.Count;
                }
                pData.numEnemiesLeft = totalEnemies;
            }
        }
        else // This is a continuing encounter, spawn the backed-up enemies
        {
            waveNum = pData.waveNum;
            // Backed up enemies or backed up spawners to to spawn
            if (pData.listEnemiesLeft.Count > 0 || pData.listActiveSpawners.Count > 0)
            {
                foreach (var spawnData in pData.listEnemiesLeft)
                {
                    if(!BattleGrid.main.IsEmpty(spawnData.spawnPos))
                    {
                        pData.numEnemiesLeft--;
                        continue;
                    }
                    var obj = Instantiate(spawnData.prefabAsset).GetComponent<Combatant>();
                    obj.PrefabOrigin = spawnData.prefabAsset;
                    obj.transform.position = BattleGrid.main.GetSpace(spawnData.spawnPos);
                    BattleGrid.main.SetObject(spawnData.spawnPos, obj);
                    obj.Hp = spawnData.remainingHP;
                }
                foreach (var spawnData in pData.listActiveSpawners)
                {
                    var spawnTile = Instantiate(spawnTileEnemyPrefab).GetComponent<EventTileSpawn>();
                    LogEventTile(spawnData, spawnTile);
                }
                foreach (var spawnData in CurrWave.obstacles)
                {
                    if (!BattleGrid.main.IsEmpty(spawnData.spawnPosition))
                    {
                        continue;
                    }
                    var obj = Instantiate(spawnData.spawnObject).GetComponent<FieldObject>();
                    obj.PrefabOrigin = spawnData.spawnObject;
                    obj.transform.position = BattleGrid.main.GetSpace(spawnData.spawnPosition);
                    BattleGrid.main.SetObject(spawnData.spawnPosition, obj);
                }

            }
            else // No backed up stuff, spawn first wave
            {
                SpawnAllEnemiesAndObstacles(CurrWave);
            }
        }
        BattleUI.main.UpdateEnemiesRemaining(pData.numEnemiesLeft);
    }

    public override Coroutine OnPhaseStart()
    {
        // If it is the first turn just spawn the enemies
        if (PhaseManager.main.Turn == 1)
        {
            Initialize();
            return null;
        }
        if (!spawnEnemies)
            return null;
        return StartCoroutine(OnPhaseStartCr());
    }

    private IEnumerator OnPhaseStartCr()
    {
        // If something is supposed to be spawned
        if (HasActiveSpawners)
        {
            yield return StartCoroutine(Spawn());
            var pData = DoNotDestroyOnLoad.Instance.persistentData;
            // End after the first spawn in the abs0 phasechange
            if (pData.absoluteZeroPhase1Defeated && endAfterFirstSpawnAbs0)
            {
                endAfterFirstSpawnAbs0 = false;
                yield break;
            }
        }

        // Increase the turns since something was last spawned
        ++turnsSinceLastWaveSpawn;

        // If there isn't a next wave just yeet out of there
        if (NextWave == null)
        {
            yield break;
        }

        // Spawn the next wave if ready
        if (NextWaveReady())
        {
            yield return StartCoroutine(DeclareNextWave());
        }
    }

    /// <summary>
    /// Change the encounter mid-battle
    /// </summary>
    public void SetEncounter(Encounter encounter, bool spawnFirstWaveImmediately = true)
    {
        CurrEncounter = encounter;
        waveNum = 0;
        turnsSinceLastWaveSpawn = 0;
        if(spawnFirstWaveImmediately)
        {
            SpawnAllEnemiesAndObstacles(CurrWave);
        }
    }

    /// <summary>
    /// Clear any active spawners from the field without spawning anything
    /// </summary>
    public void ClearActiveSpawns()
    {
        foreach(var spawn in spawners)
        {
            BattleGrid.main.RemoveEventTile(spawn.Pos, spawn);
            Destroy(spawn.gameObject);
        }
        spawners.Clear();
    }

    public IEnumerator DeclareNextWave()
    {
        // Reset the turns since wave last spawn to -1 (to acount for the turn of the declared spawns spawning)
        turnsSinceLastWaveSpawn = -1;
        if (!spawnEnemies)
            yield break;
        var pData = DoNotDestroyOnLoad.Instance.persistentData;

        #region ABS0 Phase 2 Spawn Code

        if (pData.absoluteZeroPhase1Defeated)
        {
            FMODBattle.main.TriggerNewWave();

            for(int i = 0; i < abs0ReinforcementsSpawnNumber; ++i)
            {
                if (absolute0Bank.Count <= 0)
                    break;
                var spawnData = new WaveData.SpawnData()
                {
                    spawnObject = absolute0Bank[0],
                    spawnPosition = abs0ReinforcementSpawnPositions[i],
                };
                var spawnTile = Instantiate(spawnTileEnemyPrefab).GetComponent<EventTileSpawn>();
                LogEventTile(spawnData, spawnTile);
                absolute0Bank.RemoveAt(0);

                yield return new WaitForSeconds(delaySeconds);
            }
            yield break;
        }

        #endregion

        // Don't attempt to declare if there is no next wave
        if (NextWave == null)
            yield break;

        FMODBattle.main.TriggerNewWave();

        // Log playtest data from previous wave
        logger.testData.NewDataLog(
            waveNum, pData.dayNum, CurrWave.enemies.Count, "wave won"
        );
        logger.LogData(logger.testData);

        // run tutorial trigger telling player about enemy spawn
        BattleEvents.main.tutEnemySpawnWarning._event.Invoke();

        // Declare next spawns
        foreach (var spawnData in NextWave.enemies)
        {
            var spawnTile = Instantiate(spawnTileEnemyPrefab).GetComponent<EventTileSpawn>();
            LogEventTile(spawnData, spawnTile);
            yield return new WaitForSeconds(delaySeconds);
        }
        // Increment the wave counter
        ++waveNum;
        // Repeat the last x waves according to the number on the encounter
        if (NextWave == null)
            waveNum -= CurrEncounter.repeatLastWaves;

    }

    private IEnumerator Spawn()
    {
        foreach (var spawner in spawners)
        {
            var objInSquare = BattleGrid.main.Get<FieldObject>(spawner.Pos);
            // The square is empty! Spawn the enemy.
            if (objInSquare == null)
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
                if (combatant != null)
                {
                    combatant.Damage(spawnDamage);
                }
            }
            yield return new WaitForSeconds(delaySeconds);
        }
        // Remove all spawners that spawned
        spawners.RemoveAll((s) => s == null);

        // run tutorial trigger telling player about enemy spawn
        BattleEvents.main.tutEnemySpawn._event.Invoke();
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
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        var enemyCount = PhaseManager.main.EnemyPhase.Enemies.Count;
        // Special absolute 0 phase 2 logic
        if (pData.absoluteZeroPhase1Defeated)
            return enemyCount <= minAbs0Enemies;
        if(CurrWave.spawnWhenNumberOfEnemiesRemain && enemyCount <= CurrWave.numEnemies)
        {
            return true;
        }
        if(CurrWave.spawnAfterTurns && turnsSinceLastWaveSpawn >= CurrWave.numTurns )
        {
            return true;
        }
        return false;
    }

    public void SpawnAllEnemiesAndObstacles(WaveData wave)
    {
        foreach (var spawnData in wave.AllSpawns)
        {
            var obj = Instantiate(spawnData.spawnObject).GetComponent<FieldObject>();
            obj.PrefabOrigin = spawnData.spawnObject;
            obj.transform.position = BattleGrid.main.GetSpace(spawnData.spawnPosition);
            BattleGrid.main.SetObject(spawnData.spawnPosition, obj);
        }
    }

    public override Coroutine OnPhaseEnd()
    {
        return null;
    }

    public override void OnPhaseUpdate() => EndPhase();

    public void LogPersistentData()
    {
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        pData.waveNum = waveNum;
        pData.lastEncounter = CurrEncounter;
        pData.listActiveSpawners.Clear();
        foreach (var spawner in spawners)
            pData.listActiveSpawners.Add(spawner.SpawnData);

        // Log playtest data from previous wave
        logger.testData.NewDataLog(
            waveNum, pData.dayNum, CurrWave.enemies.Count, "party retreated"
            );
        logger.LogData(logger.testData);
    }
}
