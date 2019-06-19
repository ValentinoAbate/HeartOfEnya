using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Phase : MonoBehaviour
{
    public string displayName;

    public abstract Coroutine OnPhaseStart();
    public abstract Coroutine OnPhaseEnd();
    public abstract void OnPhaseUpdate();

    protected void EndPhase()
    {
        PhaseManager.main.NextPhase();
    }
}
