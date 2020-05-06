﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class BattleEventsDay3 : MonoBehaviour
{
    public BattleEvents battleEvents;

    public void FlameMovesTrigger()
    {
        if(battleEvents.tutorialDay3 && !battleEvents.tutFlameMoves.flag)
        {
            Debug.Log("Battle Triggers: Enemy Push");
            battleEvents.tutFlameMoves.flag = true;

            StartCoroutine(FlameMovesTriggerPost(DialogueManager.main.runner));
        }
    }

    private IEnumerator FlameMovesTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);

        // post-condition

        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);
    }

    public void BurnTrigger()
    {
        if(battleEvents.tutorialDay3 && !battleEvents.tutBurn.flag)
        {
            Debug.Log("Battle Triggers: Enemy Push");
            battleEvents.tutFlameMoves.flag = true;

            StartCoroutine(BurnTriggerPost(DialogueManager.main.runner));
        }
    }

    private IEnumerator BurnTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);

        // post-condition

        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);     
    }
}
