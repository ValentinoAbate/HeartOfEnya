using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBattleInterrupt : MonoBehaviour
{
    public KeyCode testKey;
    public string startNode;
    public Yarn.Unity.DialogueRunner runner;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(testKey) && !runner.isDialogueRunning)
        {
            StartCoroutine(StartInterrupt());
        }
    }

    private IEnumerator StartInterrupt()
    {
        PhaseManager.main.PauseHandle.Pause(PauseHandle.PauseSource.BattleInterrupt);
        runner.StartDialogue(startNode);
        yield return new WaitWhile(() => runner.isDialogueRunning);
        PhaseManager.main.PauseHandle.UnPause(PauseHandle.PauseSource.BattleInterrupt);
    }
}
