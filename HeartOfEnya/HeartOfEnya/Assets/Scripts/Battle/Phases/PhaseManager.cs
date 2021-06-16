using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// The PhaseManager is the top-level class responsible for managing battle flow. It is essentially a basic state machine.
/// The PhaseManager manages a list of Phase classes which control individual phases of battle.
/// Every battle must have a PartyPhase and an EnemyPhase, but other phases may be added modularly.
/// This class implements the public-reference singleton pattern. The singleton is accessible through PhaseManager.main.
/// </summary>
public class PhaseManager : MonoBehaviour, IPausable
{
    /// <summary>
    /// Static singleton reference
    /// </summary>
    public static PhaseManager main;

    public PauseHandle PauseHandle { get => ActivePhase.PauseHandle; set => ActivePhase.PauseHandle = value; }
    public int Turn { get; private set; }
    public Phase ActivePhase { get => phases[currPhase]; }
    public PartyPhase PartyPhase { get; private set; }
    public EnemyPhase EnemyPhase { get; private set; }
    public SpawnPhase SpawnPhase { get; private set; }

    [SerializeField]
    private string goToSceneOnEnd = string.Empty;
    [SerializeField]
    private string goToSceneOnEndSpecial = string.Empty;

    private List<Phase> phases;
    private int currPhase;
    public bool Transitioning { get; private set; } = true;

    public BattleLighting lightingManager;
    private PlaytestLogger logger;

    /// <summary>
    /// Singleton pattern implementation
    /// </summary>
    private void Awake()
    {
        if (main == null)
        {
            main = this;
            InitializePhases();
        }
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Get the list of phases from the child objects
    /// Logs errors if valid party and enemy phases are not found
    /// </summary>
    private void InitializePhases()
    {
        phases = new List<Phase>();
        phases.AddRange(GetComponentsInChildren<Phase>());
        phases.RemoveAll((p) => !p.enabled);
        PartyPhase = phases.Find((p) => p is PartyPhase) as PartyPhase;
        if (PartyPhase == null)
            Debug.LogError("Improper Phase Manager Setup: No Party Phase Found");
        EnemyPhase = phases.Find((p) => p is EnemyPhase) as EnemyPhase;
        if (EnemyPhase == null)
            Debug.LogError("Improper Phase Manager Setup: No Enemy Phase Found");
        SpawnPhase = phases.Find((p) => p is SpawnPhase) as SpawnPhase;
        if (SpawnPhase == null)
            Debug.LogError("Improper Phase Manager Setup: No Spawn Phase Found");
    }

    /// <summary>
    /// Start is called before the first frame update
    /// Initialize the turn count, run the battle start coroutine, and start the first phase
    /// </summary>
    IEnumerator Start()
    {
        Turn = 1;
        logger = DoNotDestroyOnLoad.Instance.playtestLogger;
        logger.testData.UpdateTurnCount(Turn);

        yield return StartCoroutine(StartBattle());
        yield return ActivePhase.OnPhaseStart();
        Transitioning = false;
    }

    /// <summary>
    /// Update is called once per frame
    /// Simply calls the current phase's update method
    /// </summary>
    void Update()
    {
        if (!Transitioning)
            ActivePhase.OnPhaseUpdate();
    }

    public void NextPhase()
    {
        if (Transitioning)
            return;
        Transitioning = true;
        StartCoroutine(NextPhaseCr());
    }

    /// <summary>
    /// Do any logic and display any graphics needed to start the battle
    /// Currently placeholder
    /// </summary>
    private IEnumerator StartBattle()
    {
        yield break;
    }

    public void EndBattle()
    {
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        // If in the main phase, save the main encounter data
        if(pData.InMainPhase)
        {
            pData.listEnemiesLeft.Clear();
            foreach (var enemy in EnemyPhase.Enemies)
            {
                pData.listEnemiesLeft.Add(new PersistentData.SavedCombatant()
                {
                    prefabAsset = enemy.PrefabOrigin,
                    remainingHP = enemy.Hp,
                    spawnPos = enemy.OriginalPos
                });
            }
        }
        SpawnPhase.LogPersistentData();
        if (pData.absoluteZeroPhase1Defeated)
        {
            pData.returnScene = goToSceneOnEndSpecial; //mark what scene to return to on game load
            pData.SaveToFile(); //autosave
            SceneTransitionManager.main?.TransitionScenes(goToSceneOnEndSpecial);
        }
        else
        {
            pData.returnScene = goToSceneOnEnd; //mark what scene to return to on game load
            pData.SaveToFile(); //autosave
            SceneTransitionManager.main?.TransitionScenes(goToSceneOnEnd);
        }
    }

    /// <summary>
    /// Go to the next phase, waiting for the phases to end and start
    /// If the current phase is the last phase, go to the next turn
    /// </summary>
    private IEnumerator NextPhaseCr(int nextPhase = -1)
    {
        yield return new WaitWhile(() => PauseHandle.Paused);
        yield return ActivePhase.OnPhaseEnd();
        yield return new WaitWhile(() => PauseHandle.Paused);
        if(nextPhase >= 0)
        {
            currPhase = nextPhase;
        }
        else if (++currPhase >= phases.Count)
        {
            currPhase = 0;
            ++Turn;
            if (lightingManager != null && lightingManager.ReadyToProgress(Turn))
                lightingManager.ProgressLighting();
            logger.testData.UpdateTurnCount(Turn);
            Debug.Log("It is turn " + Turn);
        }
        yield return new WaitWhile(() => PauseHandle.Paused);
        Debug.Log("Starting Phase: " + ActivePhase.displayName);
        yield return ActivePhase.OnPhaseStart();
        yield return new WaitWhile(() => PauseHandle.Paused);
        Transitioning = false;
    }
}
