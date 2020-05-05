using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class BattleEventsDay2 : MonoBehaviour
{
    public BattleEvents battleEvents;

    public void PushingTrigger()
    {
        if(battleEvents.tutorial && battleEvents.day == 2 && !battleEvents.tutPushing.flag)
        {
            PhaseManager.main.PauseHandle.Pause(PauseHandle.PauseSource.BattleInterrupt);

            Debug.Log("Battle Triggers: Pushing");
            battleEvents.tutPushing.flag = true;

            StartCoroutine(PushingTriggerPost(DialogueManager.main.runner));
        }
    }

    private IEnumerator PushingTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);

        // post-condition

        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);
        
    }

    public void KnockOnTrigger()
    {
        if(battleEvents.tutorial && battleEvents.day == 2 && !battleEvents.tutKnockOn.flag)
        {
            Debug.Log("Battle Triggers: Knock On");
            battleEvents.tutKnockOn.flag = true;
        }
    }

    public void MoveDamageTrigger()
    {
        if(battleEvents.tutorial && battleEvents.day == 2 && !battleEvents.tutMoveDamage.flag)
        {
            Debug.Log("Battle Triggers: Move Damage");
            battleEvents.tutMoveDamage.flag = true;
        }
    }

    public void PullingTrigger()
    {
        if(battleEvents.tutorial && battleEvents.day == 2 && !battleEvents.tutPulling.flag)
        {
            Debug.Log("Battle Triggers: Pulling");
            battleEvents.tutPulling.flag = true;
        }
    }

    public void ChokePointsTrigger()
    {
        if(battleEvents.tutorial && battleEvents.day == 2 && !battleEvents.tutChokePoints.flag)
        {
            Debug.Log("Battle Triggers: Choke Points");
            battleEvents.tutChokePoints.flag = true;
        }
    }

    public void EnemyPushTrigger()
    {
        if(battleEvents.tutorial && battleEvents.day == 2 && !battleEvents.tutEnemyPush.flag)
        {
            Debug.Log("Battle Triggers: Enemy Push");
            battleEvents.tutEnemyPush.flag = true;
        }
    }
}
