using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseManager : MonoBehaviour
{
    public static PhaseManager main;
    public Phase ActivePhase { get => phases[currPhase]; }
    public List<PartyMember> Party { get => (phases.Find((p) => p is PartyPhase) as PartyPhase)?.party; }
    public List<Phase> phases;
    private int currPhase;
    private bool transitioning = true;

    private void Awake()
    {
        if (main == null)
        {
            main = this;
        }
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
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
            currPhase = 0;
        yield return ActivePhase.OnPhaseStart();
        transitioning = false;
    }
}
