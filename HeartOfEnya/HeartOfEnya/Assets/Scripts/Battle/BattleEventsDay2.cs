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
            battleEvents.Pause();

            Debug.Log("Battle Triggers: Pushing");
            battleEvents.tutPushing.flag = true;
            BattleUI.main.HideEndTurnButton(true);
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
        BattleUI.main.ShowPrompt("Have Bapy push the box in front of them.");
        battleEvents.Unpause();
    }

    public void KnockOnTrigger()
    {
        if(battleEvents.tutorialDay2 && !battleEvents.tutKnockOn.flag)
        {
            battleEvents.Pause();

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
        BattleUI.main.ShowPrompt("Have Bapy move to the box and push it again.");
        battleEvents.Unpause();
    }

    public void MoveDamageTrigger()
    {
        // only run after the previous trigger has finished
        if(battleEvents.tutorialDay2 && battleEvents.tutKnockOn.flag && !battleEvents.tutMoveDamage.flag)
        {
            battleEvents.Pause();

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
        BattleUI.main.ShowPrompt("Try pushing the box into the pillar to damage it.");
        
        battleEvents.Unpause();
    }

    public void PullingTrigger()
    {
        // only run after the previous trigger has finished
        if(battleEvents.tutorialDay2 && battleEvents.tutMoveDamage.flag && !battleEvents.tutPulling.flag)
        {
            battleEvents.Pause();

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
        BattleUI.main.ShowPrompt("Pull the box with 4 Hp closer.");
        battleEvents.Unpause();
    }

    public void ChokePointsTrigger()
    {
        // only run after the previous trigger has finished
        if(battleEvents.tutorialDay2 && battleEvents.tutPulling.flag && !battleEvents.tutChokePoints.flag)
        {
            battleEvents.Pause();

            Debug.Log("Battle Triggers: Choke Points");
            battleEvents.tutChokePoints.flag = true;
            StartCoroutine(ChokePointsTriggerPost(DialogueManager.main.runner));
        }
    }

    private IEnumerator ChokePointsTriggerPost(DialogueRunner runner)
    {
        yield return StartCoroutine(PhaseManager.main.SpawnPhase.DeclareNextWave());
        DialogueManager.main.runner.StartDialogue("TutChokepoint");
        yield return new WaitWhile(() => runner.isDialogueRunning);

        // post-condition: re-enable all moves
        BattleUI.main.MoveableTiles.Clear();
        BattleUI.main.TargetableTiles.Clear();
        battleEvents.partyPhase.PartyWideClearSoloActions();

        // re-enable all characters and run tiles
        string[] units = {"Raina", "Soleil"};
        battleEvents.partyPhase.EnableUnits(new List<string>(units));
        
        // Declare next wave and end the phase to prevent waiting around

        PhaseManager.main.NextPhase();
        BattleUI.main.ShowEndTurnButton(true);
        BattleUI.main.EnableRunTiles();
        battleEvents.Unpause();
    }

    public void EnemyPushTrigger()
    {
        if(battleEvents.tutorialDay2 && !battleEvents.tutEnemyPush.flag)
        {
            battleEvents.Pause();

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

        battleEvents.Unpause();
    }
}
