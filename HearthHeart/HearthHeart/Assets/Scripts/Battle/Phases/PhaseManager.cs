using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseManager : MonoBehaviour, IPausable
{
    public static PhaseManager main;

    public PauseHandle PauseHandle { get => ActivePhase.PauseHandle; set => ActivePhase.PauseHandle = value; }
    public int Turn { get; private set; }
    public Phase ActivePhase { get => phases[currPhase]; }
    public PartyPhase PartyPhase { get; set; }
    public EnemyPhase EnemyPhase { get; set; }
    
    public EncounterManager encounterManager;
    private List<Phase> phases;
    private int currPhase;
    private bool transitioning = true;

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

    private void InitializePhases()
    {
        phases = new List<Phase>(); 
        phases.AddRange(GetComponentsInChildren<Phase>());
        PartyPhase = phases.Find((p) => p is PartyPhase) as PartyPhase;
        if (PartyPhase == null)
            Debug.LogError("Improper Phase Manager Setup: No Party Phase Found");
        EnemyPhase = phases.Find((p) => p is EnemyPhase) as EnemyPhase;
        if (EnemyPhase == null)
            Debug.LogError("Improper Phase Manager Setup: No Enemy Phase Found");
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        Turn = 1;
        yield return StartCoroutine(StartBattle());
        yield return ActivePhase.OnPhaseStart();
        transitioning = false;        
    }

    // Update is called once per frame
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

    private IEnumerator StartBattle()
    {
        yield break;
    }
    private IEnumerator NextPhaseCr()
    {
        yield return ActivePhase.OnPhaseEnd();
        if (++currPhase >= phases.Count)
        {
            currPhase = 0;
            ++Turn;
            encounterManager.ProcessTurn(Turn);
            Debug.Log("It is turn " + Turn);
        }          
        Debug.Log("Starting Phase: " + ActivePhase.displayName);
        yield return ActivePhase.OnPhaseStart();
        transitioning = false;
    }
}
