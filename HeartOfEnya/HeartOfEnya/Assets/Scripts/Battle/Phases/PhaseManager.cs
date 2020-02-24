using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public PartyPhase PartyPhase { get; set; }
    public EnemyPhase EnemyPhase { get; set; }

    [SerializeField]
    private string goToSceneOnEnd;

    private List<Phase> phases;
    private int currPhase;
    public bool Transitioning => transitioning;
    private bool transitioning = true;

    public PlaytestLogger logger;

    /// <summary>
    /// Singleton pattern implementation
    /// </summary>
    private void Awake()
    {
        logger = DoNotDestroyOnLoad.Instance.playtestLogger;
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
    }

    /// <summary>
    /// Start is called before the first frame update
    /// Initialize the turn count, run the battle start coroutine, and start the first phase
    /// </summary>
    IEnumerator Start()
    {
        Turn = 1;
        logger.testData.UpdateTurnCount(Turn);
        yield return StartCoroutine(StartBattle());
        yield return ActivePhase.OnPhaseStart();
        transitioning = false;        
    }

    /// <summary> 
    /// Update is called once per frame 
    /// Simply calls the current phase's update method
    /// </summary> 
    void Update()
    {
        if (!transitioning)
            ActivePhase.OnPhaseUpdate();
    }

    public void NextPhase()
    {
        if (transitioning)
            return;
        transitioning = true;
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
        // Log playtest data from previous wave
            logger.testData.NewDataLog(
                waveNum, DoNotDestroyOnLoad.Instance.persistentData.dayNum, CurrWave.numEnemies, "party retreated"
            );
            logger.LogData(logger.testData);


        SceneTransitionManager.main?.TransitionScenes(goToSceneOnEnd);
    }

    /// <summary>
    /// Go to the next phase, waiting for the phases to end and start
    /// If the current phase is the last phase, go to the next turn
    /// </summary>
    private IEnumerator NextPhaseCr()
    {
        yield return ActivePhase.OnPhaseEnd();
        if (++currPhase >= phases.Count)
        {
            currPhase = 0;
            ++Turn;
            logger.testData.UpdateTurnCount(Turn);
            Debug.Log("It is turn " + Turn);
        }          
        Debug.Log("Starting Phase: " + ActivePhase.displayName);
        yield return ActivePhase.OnPhaseStart();
        transitioning = false;
    }
}
