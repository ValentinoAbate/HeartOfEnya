using System.Collections;
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
            battleEvents.Pause();
            
            Debug.Log("Battle Triggers: Flame Moves");
            battleEvents.tutFlameMoves.flag = true;
            DialogueManager.main.runner.StartDialogue("TutFlameMoves");
            StartCoroutine(FlameMovesTriggerPost(DialogueManager.main.runner));
        }
    }

    private IEnumerator FlameMovesTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);

        // post-condition: disable everyone but raina
        string[] units = {"Soleil", "Bapy"};
        battleEvents.partyPhase.DisableUnits(new List<string>(units));

        // Restrict movement
        var rainaPos = new Pos(2, 7);
        BattleUI.main.MoveableTiles.Add(rainaPos);
        BattleUI.main.TargetableTiles.Add(rainaPos + Pos.Left);

        // solo raina's flame move
        battleEvents.partyPhase.PartyWideSoloAction("RainaAction2_Flame");

        battleEvents.Unpause();
    }

    public void BurnTrigger()
    {
        if(battleEvents.tutorialDay3 && !battleEvents.tutBurn.flag)
        {
            battleEvents.Pause();
            
            Debug.Log("Battle Triggers: Burn");
            battleEvents.tutBurn.flag = true;
            DialogueManager.main.runner.StartDialogue("TutBurn");
            StartCoroutine(BurnTriggerPost(DialogueManager.main.runner));
        }
    }

    private IEnumerator BurnTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);

        // re-enable all characters
        string[] units = {"Soleil", "Bapy"};
        battleEvents.partyPhase.EnableUnits(new List<string>(units));
        battleEvents.partyPhase.PartyWideClearSoloActions();
        // lift movement restriction
        BattleUI.main.MoveableTiles.Clear();
        // lift targeting restrictions
        BattleUI.main.TargetableTiles.Clear();
        BattleUI.main.EnableRunTiles();
        battleEvents.Unpause();
    }
}
