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
    [SerializeField]
    private Encounter mainEncounter;
    [SerializeField]
    private Encounter tutorialEncounter;
    [SerializeField]
    private Encounter luaEncounter;
    [SerializeField]
    private Encounter absoluteZeroEncounter;
    [SerializeField]
    public Encounter levelEditorEncounter;
    public Encounter CurrEncounter { get; private set;}
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
    private int turnsSinceLastSpawn = 0;
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
    private List<GameObject> absolute0Bank = new List<GameObject>();
    [Header("Abs0 encounter spawn parameters")]
    public List<Pos> abs0ReinforcementSpawnPositions = new List<Pos>();
    public int minAbs0Enemies = 4;
    public int abs0ReinforcementsSpawnNumber = 4;

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
        {
            CurrEncounter = absoluteZeroEncounter;
            absolute0Bank = GetAbs0Bank();
        }
        else if(gamePhase == "LE")
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
        for(int waveNum = pData.waveNum; waveNum < mainEncounter.Waves.Length; ++waveNum)
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
        // Spawn party members
        Vector2 bapyVec = BattleGrid.main.GetSpace(bapyPos);
        var bapy = Instantiate(bapyLvl[pData.partyLevel], bapyVec,
                                    Quaternion.identity).GetComponent<PartyMember>();
        dialog.characters.Clear();
        dialog.characters.Add(bapy.GetComponent<Character>());
        bapy.Pos = bapyPos;

        Vector2 soleilVec = BattleGrid.main.GetSpace(soleilPos);
        var soleil = Instantiate(soleilLvl[pData.partyLevel], soleilVec,
                                    Quaternion.identity).GetComponent<PartyMember>();
        dialog.characters.Add(soleil.GetComponent<Character>());
        soleil.Pos = soleilPos;

        Vector2 rainaVec = BattleGrid.main.GetSpace(rainaPos);
        var raina = Instantiate(rainaLvl[pData.partyLevel], rainaVec,
                                    Quaternion.identity).GetComponent<PartyMember>();
        dialog.characters.Add(raina.GetComponent<Character>());
        raina.Pos = rainaPos;

        bool enableLua = pData.LuaUnfrozen || overrideSpawnLua;
        PartyMember lua = null;

        // Spawn Lua if enabled
        if (enableLua)
        {
            Vector2 luaVec = BattleGrid.main.GetSpace(luaPos);
            lua = Instantiate(luaLvl[pData.partyLevel], luaVec,
                                Quaternion.identity).GetComponent<PartyMember>();
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
        if (!spawnEnemies)
            return;

        // This is a fresh encounter, just spawn everything
        if (CurrEncounter != pData.lastEncounter)
        {
            waveNum = startAtWave - 1;
            foreach (var spawnData in CurrWave.AllSpawns)
            {
                var obj = Instantiate(spawnData.spawnObject).GetComponent<FieldObject>();
                obj.PrefabOrigin = spawnData.spawnObject;
                obj.transform.position = BattleGrid.main.GetSpace(spawnData.spawnPosition);
                BattleGrid.main.SetObject(spawnData.spawnPosition, obj);
            }
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
                    var obj = Instantiate(spawnData.spawnObject).GetComponent<FieldObject>();
                    obj.PrefabOrigin = spawnData.spawnObject;
                    obj.transform.position = BattleGrid.main.GetSpace(spawnData.spawnPosition);
                    BattleGrid.main.SetObject(spawnData.spawnPosition, obj);
                }

            }
            else // No backed up stuff, spawn first wave
            {
                foreach (var spawnData in CurrWave.AllSpawns)
                {
                    var obj = Instantiate(spawnData.spawnObject).GetComponent<FieldObject>();
                    obj.PrefabOrigin = spawnData.spawnObject;
                    obj.transform.position = BattleGrid.main.GetSpace(spawnData.spawnPosition);
                    BattleGrid.main.SetObject(spawnData.spawnPosition, obj);
                }
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
        if(HasActiveSpawners)
            yield return StartCoroutine(Spawn());
        else // Increase the turns since something was last spawned
            ++turnsSinceLastSpawn;
        // If there isn't a next wave, just yeet out of there
        if (NextWave == null)
            yield break;
        // Spawn the next wave if ready
        if (NextWaveReady())
            yield return StartCoroutine(DeclareNextWave());
    }

    public IEnumerator DeclareNextWave()
    {
        if (!spawnEnemies)
            yield break;
        var pData = DoNotDestroyOnLoad.Instance.persistentData;

        #region ABS0 Phase 2 Spawn Code

        if (pData.absoluteZeroDefeated)
        {
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
        turnsSinceLastSpawn = 0;
        foreach (var spawner in spawners)
        {
            var objInSquare = BattleGrid.main.GetObject(spawner.Pos);
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
        if(pData.absoluteZeroDefeated)
            return PhaseManager.main.EnemyPhase.Enemies.Count <= minAbs0Enemies;
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
