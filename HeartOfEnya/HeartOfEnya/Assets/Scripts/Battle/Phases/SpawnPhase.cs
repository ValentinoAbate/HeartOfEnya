using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPhase : Phase
{
    public override PauseHandle PauseHandle { get; set; } = new PauseHandle(null);

    public EncounterBank waves;

    public override Coroutine OnPhaseEnd()
    {
        return null;
    }

    public override Coroutine OnPhaseStart()
    {
        return null;
    }

    public override void OnPhaseUpdate() { }
}
