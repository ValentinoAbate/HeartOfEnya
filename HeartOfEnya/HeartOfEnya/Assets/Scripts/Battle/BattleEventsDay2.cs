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
            DialogManager.main.runner.StartDialogue("TutPush");
            StartCoroutine(PushingTriggerPost(DialogManager.main.runner));
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
            DialogManager.main.runner.StartDialogue("TutChainedMovement");
            StartCoroutine(KnockOnTriggerPost(DialogManager.main.runner));
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
            DialogManager.main.runner.StartDialogue("TutMoveDamage");
            StartCoroutine(MoveDamageTriggerPost(DialogManager.main.runner));
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
            DialogManager.main.runner.StartDialogue("TutPull");
            StartCoroutine(PullingTriggerPost(DialogManager.main.runner));
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
        BattleUI.main.ShowPrompt("Pull a box closer.");
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
            StartCoroutine(ChokePointsTriggerPost(DialogManager.main.runner));
        }
    }

    private IEnumerator ChokePointsTriggerPost(DialogueRunner runner)
    {
        runner.StartDialogue("TutChokepoint");
        yield return new WaitWhile(() => runner.isDialogueRunning);

        // post-condition: reset actions NEED TO SET PROPER VALUES
        BattleUI.main.MoveableTiles.Clear();
        BattleUI.main.TargetableTiles.Clear();

        // make bapy pull the lower box to create a choke point NEED TO SET PROPER VALUES
        BattleUI.main.MoveableTiles.Add(new Pos(2, 3));
        BattleUI.main.TargetableTiles.Add(new Pos(2, 2));

        // Declare next wave and end the phase to prevent waiting around
        PhaseManager.main.NextPhase();
        battleEvents.Unpause();
    }

    public void SpawnBlockTrigger()
    {
        // only run after the previous trigger has finished
        if (battleEvents.tutorialDay2 && battleEvents.tutChokePoints.flag && !battleEvents.tutSpawnBlock.flag)
        {
            battleEvents.Pause();

            Debug.Log("Battle Triggers: Spawn Block");
            battleEvents.tutSpawnBlock.flag = true;
            StartCoroutine(SpawnBlockTriggerPost(DialogManager.main.runner));
        }
    }

    private IEnumerator SpawnBlockTriggerPost(DialogueRunner runner)
    {
        runner.StartDialogue("TutSpawnBlock");
        yield return new WaitWhile(() => runner.isDialogueRunning);


        PhaseManager.main.NextPhase();
        BattleUI.main.ShowPrompt("Try pushing a box to cover up the Frost’s spawn point.");
        battleEvents.Unpause();
    }

    public void SpawnBlockEndTurnTrigger()
    {
        // only run after the previous trigger has finished
        if (battleEvents.tutorialDay2 && battleEvents.tutSpawnBlock.flag && !battleEvents.tutSpawnBlockEndTurn.flag)
        {
            battleEvents.tutSpawnBlockEndTurn.flag = true;
            PhaseManager.main.NextPhase();
        }
    }

    public void CelebrateSpawnBlockTrigger()
    {
        // only run after the previous trigger has finished
        if (battleEvents.tutorialDay2 && battleEvents.tutSpawnBlockEndTurn.flag && !battleEvents.tutBapyCelebrateSpawnBlock.flag)
        {
            battleEvents.Pause();

            Debug.Log("Battle Triggers: Choke Points");
            battleEvents.tutBapyCelebrateSpawnBlock.flag = true;
            StartCoroutine(CelebrateSpawnBlockTriggerPost(DialogManager.main.runner));
        }
    }

    private IEnumerator CelebrateSpawnBlockTriggerPost(DialogueRunner runner)
    {
        runner.StartDialogue("TutBapyCelebrateSpawnBlock");
        yield return new WaitWhile(() => runner.isDialogueRunning);
        // post-condition: re-enable all moves
        BattleUI.main.MoveableTiles.Clear();
        BattleUI.main.TargetableTiles.Clear();
        battleEvents.partyPhase.PartyWideClearSoloActions();

        // re-enable all characters
        string[] units = { "Raina", "Soleil" };
        battleEvents.partyPhase.EnableUnits(new List<string>(units));

        // Enable end turn button and run tiles
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
            DialogManager.main.runner.StartDialogue("TutEnemyPush");
            StartCoroutine(EnemyPushTriggerPost(DialogManager.main.runner));
        }
    }

    private IEnumerator EnemyPushTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);

        // no post-condition

        battleEvents.Unpause();
    }
}
