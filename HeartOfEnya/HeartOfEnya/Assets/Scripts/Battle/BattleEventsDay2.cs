using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class BattleEventsDay2 : MonoBehaviour
{
    public BattleEvents battleEvents;

    public void PushingTrigger()
    {
        if(battleEvents.tutorialDay2 && !battleEvents.tutPushing.flag)
        {
            PhaseManager.main.PauseHandle.Pause(PauseHandle.PauseSource.BattleInterrupt);

            Debug.Log("Battle Triggers: Pushing");
            battleEvents.tutPushing.flag = true;
            DialogueManager.main.runner.StartDialogue("TutPush");
            StartCoroutine(PushingTriggerPost(DialogueManager.main.runner));
        }
    }

    private IEnumerator PushingTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);

        // post-condition: disable everyone but bapy
        string[] units = {"Raina", "Soleil"};
        battleEvents.partyPhase.DisableUnits(new List<string>(units));

        // make bapy push the upper right box
        battleEvents.partyPhase.PartyWideSoloAction("BapyAction2");
        BattleUI.main.MoveableTiles.Add(new Pos(1, 8));
        BattleUI.main.TargetableTiles.Add(new Pos (1, 7));

        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);
        
    }

    public void KnockOnTrigger()
    {
        if(battleEvents.tutorialDay2 && !battleEvents.tutKnockOn.flag)
        {
            Debug.Log("Battle Triggers: Knock On");
            battleEvents.tutKnockOn.flag = true;
            DialogueManager.main.runner.StartDialogue("TutChainedMovement");
            StartCoroutine(KnockOnTriggerPost(DialogueManager.main.runner));
        }
    }

    private IEnumerator KnockOnTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);

        // post-condition: reset actions
        BattleUI.main.MoveableTiles.Clear();
        BattleUI.main.TargetableTiles.Clear();

        // make bapy push the boxes towards the pillar
        BattleUI.main.MoveableTiles.Add(new Pos(1, 7));
        BattleUI.main.TargetableTiles.Add(new Pos (1, 6));

        // end the turn manually
        PhaseManager.main.NextPhase();

        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);
    }

    public void MoveDamageTrigger()
    {
        // only run after the previous trigger has finished
        if(battleEvents.tutorialDay2 && battleEvents.tutKnockOn.flag && !battleEvents.tutMoveDamage.flag)
        {
            Debug.Log("Battle Triggers: Move Damage");
            battleEvents.tutMoveDamage.flag = true;
            DialogueManager.main.runner.StartDialogue("TutMoveDamage");
            StartCoroutine(MoveDamageTriggerPost(DialogueManager.main.runner));
        }
    }

    private IEnumerator MoveDamageTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);

        // post-condition: reset actions
        BattleUI.main.MoveableTiles.Clear();
        BattleUI.main.TargetableTiles.Clear();

        // make bapy push the boxes into the pillar
        BattleUI.main.MoveableTiles.Add(new Pos(1, 6));
        BattleUI.main.TargetableTiles.Add(new Pos (1, 5));

        // end the turn manually
        PhaseManager.main.NextPhase();

        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);
    }

    public void PullingTrigger()
    {
        // only run after the previous trigger has finished
        if(battleEvents.tutorialDay2 && battleEvents.tutMoveDamage.flag && !battleEvents.tutPulling.flag)
        {
            Debug.Log("Battle Triggers: Pulling");
            battleEvents.tutPulling.flag = true;
            DialogueManager.main.runner.StartDialogue("TutPull");
            StartCoroutine(PullingTriggerPost(DialogueManager.main.runner));
        }
    }

    private IEnumerator PullingTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);

        // post-condition: reset actions
        BattleUI.main.MoveableTiles.Clear();
        BattleUI.main.TargetableTiles.Clear();

        // make bapy pull the lower box to create a choke point
        BattleUI.main.MoveableTiles.Add(new Pos(3, 5));
        BattleUI.main.TargetableTiles.Add(new Pos (3, 3));

        // end the turn manually
        PhaseManager.main.NextPhase();

        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);
    }

    public void ChokePointsTrigger()
    {
        // only run after the previous trigger has finished
        if(battleEvents.tutorialDay2 && battleEvents.tutPulling.flag && !battleEvents.tutChokePoints.flag)
        {
            Debug.Log("Battle Triggers: Choke Points");
            battleEvents.tutChokePoints.flag = true;
            DialogueManager.main.runner.StartDialogue("TutChokepoint");
            StartCoroutine(ChokePointsTriggerPost(DialogueManager.main.runner));
        }
    }

    private IEnumerator ChokePointsTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);

        // post-condition: re-enable all moves
        BattleUI.main.MoveableTiles.Clear();
        BattleUI.main.TargetableTiles.Clear();
        battleEvents.partyPhase.PartyWideClearSoloActions();

        // re-enable all characters
        string[] units = {"Raina", "Soleil"};
        battleEvents.partyPhase.EnableUnits(new List<string>(units));
        // Declare next wave and end the phase to prevent waiting around
        yield return StartCoroutine(PhaseManager.main.SpawnPhase.DeclareNextWave());
        PhaseManager.main.NextPhase();
        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);
    }

    public void EnemyPushTrigger()
    {
        if(battleEvents.tutorialDay2 && !battleEvents.tutEnemyPush.flag)
        {
            Debug.Log("Battle Triggers: Enemy Push");
            battleEvents.tutEnemyPush.flag = true;
            DialogueManager.main.runner.StartDialogue("TutEnemyPush");
            StartCoroutine(EnemyPushTriggerPost(DialogueManager.main.runner));
        }
    }

    private IEnumerator EnemyPushTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);

        // no post-condition

        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);
    }
}
