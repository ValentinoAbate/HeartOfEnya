using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class BattleEventsMain : MonoBehaviour
{
    public BattleEvents battleEvents;

    public void PassivesTrigger()
    {
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        if(battleEvents.tutMainPhase && !battleEvents.tutLuaJoin && !battleEvents.tutPassives.flag)
        {
            battleEvents.Pause();

            Debug.Log("Battle Triggers: Passives");
            battleEvents.tutPassives.flag = true;

            StartCoroutine(PassivesTriggerPost(DialogueManager.main.runner));
        }
    }

    private IEnumerator PassivesTriggerPost(DialogueRunner runner)
    {
        runner.StartDialogue("TutPassivesAndMainPhase");
        yield return new WaitWhile(() => runner.isDialogueRunning);

        battleEvents.Unpause();
    }
}
