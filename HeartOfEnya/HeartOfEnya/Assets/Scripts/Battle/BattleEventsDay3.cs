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
            Debug.Log("Battle Triggers: Flame Moves");
            battleEvents.tutFlameMoves.flag = true;

            StartCoroutine(FlameMovesTriggerPost(DialogueManager.main.runner));
        }
    }

    private IEnumerator FlameMovesTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);

        // post-condition: disable everyone but raina
        string[] units = {"Soleil", "Bapy"};
        battleEvents.partyPhase.DisableUnits(new List<string>(units));

        // solo raina's flame move
        battleEvents.partyPhase.PartyWideSoloAction("RainaAction2_Flame");

        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);
    }

    public void BurnTrigger()
    {
        if(battleEvents.tutorialDay3 && !battleEvents.tutBurn.flag)
        {
            Debug.Log("Battle Triggers: Burn");
            battleEvents.tutFlameMoves.flag = true;

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
        BattleUI.main.EnableRunTiles();

        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);     
    }
}
