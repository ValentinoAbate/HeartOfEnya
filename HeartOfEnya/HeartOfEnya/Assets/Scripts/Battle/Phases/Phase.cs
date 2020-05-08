using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Phase : MonoBehaviour, IPausable
{
    public const bool playTransitionGrahics = true;
    public abstract PauseHandle PauseHandle { get; set; }

    public string displayName;
    public GameObject phaseTransitionPrefab;

    public abstract Coroutine OnPhaseStart();
    public abstract Coroutine OnPhaseEnd();
    public abstract void OnPhaseUpdate();

    protected void EndPhase()
    {
        PhaseManager.main.NextPhase();
    }

    protected void EndBattle()
    {
        PhaseManager.main.EndBattle();
    }
    protected IEnumerator PlayTransition()
    {
        if (playTransitionGrahics)
        {
            var cutIn = Instantiate(phaseTransitionPrefab);
            yield return new WaitForSeconds(1.5f);
            Destroy(cutIn);
        }
    }
}
