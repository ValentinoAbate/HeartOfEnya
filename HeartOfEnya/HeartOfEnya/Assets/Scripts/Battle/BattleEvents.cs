using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

public class BattleEvents : MonoBehaviour
{
    // struct wrapper for unity event containing flag
    [System.Serializable]
    public struct BattleEvent
    {
        public UnityEvent _event;
        public bool flag;
    }

    public static BattleEvents main;
    [HideInInspector] public bool tutorialDay1;
    [HideInInspector] public bool tutorialDay2;
    [HideInInspector] public bool tutorialDay3;
    [HideInInspector] public bool luaBattle;
    [HideInInspector] public bool tutLuaJoin;
    [HideInInspector] public bool tutMainPhase;
    [HideInInspector] public bool abs0Battle;
    [HideInInspector] public bool inMainPhase;

    [Header("Tutorial Events")]
    public BattleEvent tutorialIntro;
    public BattleEvent tutMove;
    public BattleEvent tutRainaAttack;
    public BattleEvent tutBapySelect;
    public BattleEvent tutBapyCancel;
    public BattleEvent tutSoleilSelect;
    public BattleEvent tutSoleilAttack;
    public BattleEvent tutBapySelect2;
    public BattleEvent tutBapyWait;
    public BattleEvent tutSoleilChargeReminder;
    public BattleEvent tutSoleilChargeExplanation;
    public BattleEvent tutEndTurnButton;
    public BattleEvent tutEnemySpawnWarning;
    public BattleEvent tutEnemySpawn;
    public BattleEvent tutEnemyInfo;
    public BattleEvent tutEnemyRanged;
    public BattleEvent tutDD;

    [Header("Tutorial Day 2")]
    public BattleEvent tutPushing;
    public BattleEvent tutKnockOn;
    public BattleEvent tutMoveDamage;
    public BattleEvent tutPulling;
    public BattleEvent tutChokePoints;
    public BattleEvent tutSpawnBlock;
    public BattleEvent tutSpawnBlockEndTurn;
    public BattleEvent tutBapyCelebrateSpawnBlock;
    public BattleEvent tutEnemyPush;

    [Header("Tutorial Day 3")]
    public BattleEvent tutFlameMoves;
    public BattleEvent tutBurn;

    [Header("Lua Boss Events")]
    public BattleEvent luaBossIntro;
    public BattleEvent luaBossPhaseChange;
    public BattleEvent luaBossPhase2Defeated;

    [Header("Main Phase Intro Events")]
    public BattleEvent tutPassives;

    [Header("Lua Joins Events")]
    public BattleEvent tutMove3;
    public BattleEvent tutLua;

    [Header("Absolute Zero Events")]
    public BattleEvent abs0Intro;
    public BattleEvent abs0PhaseChange;
    public BattleEvent abs0Phase2Defeated;

    [Header("Generic Battle Events")]
    public BattleEvent graveInjury;
    public BattleEvent noEnemiesLeftMainPhase;

    // references to objects that will be needed to disable certain game functions
    [HideInInspector] public PartyPhase partyPhase;

    private Pos rainaMovePos = new Pos(1, 6);

    private void Awake()
    {
        if (main == null)
        {
            main = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        tutorialDay1 = pData.InTutorialFirstDay;
        tutorialDay2 = pData.InTutorialSecondDay;
        tutorialDay3 = pData.InTutorialThirdDay;
        luaBattle = pData.InLuaBattle;
        tutMainPhase = pData.InTutorialMainPhase;
        tutLuaJoin = pData.InTutorialLuaJoin;
        abs0Battle = pData.InAbs0Battle;
        inMainPhase = pData.InMainPhase;
        partyPhase = PhaseManager.main.PartyPhase;
    }

    public void Pause()
    {
        PhaseManager.main.PauseHandle.Pause(PauseHandle.PauseSource.BattleInterrupt);
        FMODBattle.main.TextPlaying = true;
        BattleUI.main.HidePrompt();
    }

    public void Unpause()
    {
        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);
        FMODBattle.main.TextPlaying = false;
    }

    public void IntroTrigger()
    {
        if(tutorialDay1 && !tutorialIntro.flag)
        {
            Debug.Log("Battle Triggers: start of battle");
            tutorialIntro.flag = true;
            BattleUI.main.HideEndTurnButton(true);

            // Start the dialog (connect to ambers code)
            Pause();
            DialogManager.main.runner.StartDialogue("TutIntro");
            
            // Wait for finish StartCoroutine(IntroTriggerPost(runner))
            StartCoroutine(IntroTriggerPost(DialogManager.main.runner));
        }
    }

    private IEnumerator IntroTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);

        // post-condition: disable everyone but raina
        string[] units = {"Bapy", "Soleil"};
        partyPhase.DisableUnits(new List<string>(units));
        // restrict raina's movement to specific square
        BattleUI.main.MoveableTiles.Add(rainaMovePos);
        BattleUI.main.ShowPrompt("Click on Raina to select them.");
        Unpause();        
    }

    public void MoveTrigger()
    {
        if(tutorialDay1 && !tutMove.flag)
        {
            Pause();
            DialogManager.main.runner.StartDialogue("TutMove");
            
            Debug.Log("Battle Triggers: raina move");
            tutMove.flag = true;
            
            StartCoroutine(MoveTriggerPost(DialogManager.main.runner));
        }
    }

    private IEnumerator MoveTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        
        // disable cancelling, solo raina's cleave move
        BattleUI.main.CancelingEnabled = false;
        partyPhase.PartyWideSoloAction("RainaAction2");
        BattleUI.main.ShowPrompt("Click on the highlighted tile to move Raina.");
        Unpause();
    }

    public void RainaAttackTrigger()
    {
        if(tutorialDay1 && !tutRainaAttack.flag)
        {
            Pause();
            DialogManager.main.runner.StartDialogue("TutRainaAttack");
            
            Debug.Log("Battle Triggers: raina attack");
            tutRainaAttack.flag = true;
            
            StartCoroutine(RainaAttackTriggerPost(DialogManager.main.runner));
        }
    }

    private IEnumerator RainaAttackTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        
        // restrict raina to attacking to the left
        BattleUI.main.TargetableTiles.Add(rainaMovePos + Pos.Left);
        BattleUI.main.ShowPrompt("Click on Cleave, then choose a target tile to attack the boxes.");
        Unpause();
    }

    public void BapySelectTrigger()
    {
        if(tutorialDay1 && !tutBapySelect.flag)
        {
            Pause();
            DialogManager.main.runner.StartDialogue("TutBapySelect");
            
            Debug.Log("Battle Triggers: select bapy");
            tutBapySelect.flag = true;
            
            StartCoroutine(BapySelectTriggerPost(DialogManager.main.runner));
        }
    }

    private IEnumerator BapySelectTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        
        // re-enable bapy's turn
        partyPhase.EnableUnits("Bapy");

        // unrestrict movement/targeting
        BattleUI.main.MoveableTiles.Clear();
        BattleUI.main.TargetableTiles.Clear();
        BattleUI.main.ShowPrompt("Click on Bapy to select them, then click a tile to move.");
        Unpause();
    }

    public void BapyCancelTrigger()
    {
        if(tutorialDay1 && !tutBapyCancel.flag)
        {
            Pause();
            DialogManager.main.runner.StartDialogue("TutBapyCancel");
            
            Debug.Log("Battle Triggers: bapy cancel");
            tutBapyCancel.flag = true;
            
            StartCoroutine(BapyCancelTriggerPost(DialogManager.main.runner));
        }
    }

    private IEnumerator BapyCancelTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        
        // restrict bapy's actions to only cancel
        BattleUI.main.CancelingEnabled = true;
        // enable soleil's charge attack
        partyPhase.PartyWideClearSoloActions();
        partyPhase.PartyWideSoloAction("SoleilAction2");
        BattleUI.main.ShowPrompt("Right click to undo an action.\nRight click again to go back to unit selection.");
        Unpause();
    }

    public void SoleilSelectTrigger()
    {
        if(tutorialDay1 && !tutSoleilSelect.flag)
        {
            Pause();  
            Debug.Log("Battle Triggers: select soleil");
            tutSoleilSelect.flag = true;
            // Turn Cancelling back off
            BattleUI.main.CancelingEnabled = false;
            DialogManager.main.runner.StartDialogue("TutSoleilSelect");
            StartCoroutine(SoleilSelectTriggerPost(DialogManager.main.runner));
        }
    }

    private IEnumerator SoleilSelectTriggerPost(DialogueRunner runner)
    {

        yield return new WaitWhile(() => runner.isDialogueRunning);
        
        // enable soleil
        partyPhase.EnableUnits("Soleil");

        // disable bapy
        partyPhase.DisableUnits("Bapy");
        BattleUI.main.ShowPrompt("Click on Soleil to select her, then click a tile to move.");
        Unpause();
    }

    public void SoleilAttackTrigger()
    {
        if(tutorialDay1 && !tutSoleilAttack.flag)
        {
            Pause();
            DialogManager.main.runner.StartDialogue("TutSoleilAttack");
            
            Debug.Log("Battle Triggers: soleil attack");
            tutSoleilAttack.flag = true;
            
            StartCoroutine(SoleilAttackTriggerPost(DialogManager.main.runner));
        }
    }

    private IEnumerator SoleilAttackTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        
        // re-enable bapy (and everyone else since their turns are over anyway)
        string[] units = {"Bapy", "Soleil", "Raina"};
        partyPhase.EnableUnits(new List<string>(units));

        // make it so bapy can only move next to raina
        BattleUI.main.MoveableTiles.Add(rainaMovePos + Pos.Right);
        BattleUI.main.MoveableTiles.Add(rainaMovePos + Pos.Right + Pos.Right);
        BattleUI.main.ShowPrompt("Click on Blast, then pick an area to begin charging.");
        Unpause();
    }

    public void BapySelect2Trigger()
    {
        if (tutorialDay1 && tutSoleilAttack.flag && !tutBapySelect2.flag)
        {
            Pause();
            DialogManager.main.runner.StartDialogue("TutBapySelect2");

            // solo wait for bapy
            partyPhase.PartyWideSoloAction("Wait");

            Debug.Log("Battle Triggers: select bapy 2");
            tutBapySelect2.flag = true;
            BattleUI.main.CancelingEnabled = true;
            StartCoroutine(BapySelectTrigger2Post(DialogManager.main.runner));
        }
    }

    private IEnumerator BapySelectTrigger2Post(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        BattleUI.main.ShowPrompt("Click on Bapy to select them, then click a tile to move."); 
        Unpause();
    }

    public void BapyWaitTrigger()
    {
        if (tutorialDay1 && tutBapySelect2.flag && !tutBapyWait.flag)
        {
            Pause();
            DialogManager.main.runner.StartDialogue("TutBapyWait");

            Debug.Log("Battle Triggers: bapy wait");
            tutBapyWait.flag = true;

            StartCoroutine(BapyWaitTriggerPost(DialogManager.main.runner));
        }
    }

    private IEnumerator BapyWaitTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        BattleUI.main.ShowPrompt("Click on Wait to end Bapy’s turn without taking an action.");
        Unpause();
    }

    public void SoleilChargeReminderTrigger()
    {
        if(tutorialDay1 && !tutSoleilChargeReminder.flag)
        {
            Pause();
            DialogManager.main.runner.StartDialogue("TutSoleilChargeReminder");
            
            Debug.Log("Battle Triggers: SoleilChargeReminder");
            tutSoleilChargeReminder.flag = true;
            
            StartCoroutine(SoleilChargeReminderTriggerPost(DialogManager.main.runner));
        }
    }

    private IEnumerator SoleilChargeReminderTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        
        // re-enable all actions
        partyPhase.PartyWideClearSoloActions();
        // unrestrict movement/targeting
        BattleUI.main.MoveableTiles.Clear();

        Unpause();
    }

    public void SoleilChargeExplanationTrigger()
    {
        if(tutorialDay1 && !tutSoleilChargeExplanation.flag)
        {
            Pause();
            DialogManager.main.runner.StartDialogue("TutSoleilChargeExplanation");
            
            Debug.Log("Battle Triggers: SoleilChargeExplanation");
            tutSoleilChargeExplanation.flag = true;
            
            StartCoroutine(SoleilChargeExplanationTriggerPost(DialogManager.main.runner));
        }
    }

    private IEnumerator SoleilChargeExplanationTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        
        // no end functions
        
        Unpause();
    }

    public void EnemySpawnWarningTrigger()
    {
        if(tutorialDay1 && !tutEnemySpawnWarning.flag)
        {
            Pause();
            DialogManager.main.runner.StartDialogue("TutEnemySpawnWarning");
            
            Debug.Log("Battle Triggers: EnemySpawnWarning");
            tutEnemySpawnWarning.flag = true;
            
            StartCoroutine(EnemySpawnWarningTriggerPost(DialogManager.main.runner));
        }
    }

    private IEnumerator EnemySpawnWarningTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        
        // no end functions
        
        Unpause();
    }


    public void TutEndTurnButtonTrigger()
    {
        if (tutorialDay1 && !tutEndTurnButton.flag)
        {
            Pause();
            DialogManager.main.runner.StartDialogue("TutEndTurnButton");

            Debug.Log("Battle Triggers: TutEndTurnButton");
            tutEndTurnButton.flag = true;

            StartCoroutine(TutEndTurnButtonTriggerPost(DialogManager.main.runner));
        }
    }

    private IEnumerator TutEndTurnButtonTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);

        // no enable the end turn button
        BattleUI.main.ShowEndTurnButton(true);

        Unpause();
    }


    public void EnemySpawnTrigger()
    {
        if(tutorialDay1 && !tutEnemySpawn.flag)
        {
            Pause();
            DialogManager.main.runner.StartDialogue("TutEnemySpawn");
            
            Debug.Log("Battle Triggers: EnemySpawn");
            tutEnemySpawn.flag = true;
            
            StartCoroutine(EnemySpawnTriggerPost(DialogManager.main.runner));
        }
    }

    private IEnumerator EnemySpawnTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        
        // disable unit selecting
        string[] units = {"Bapy", "Soleil", "Raina"};
        partyPhase.DisableUnits(new List<string>(units));
        BattleUI.main.ShowPrompt("Mouse over an enemy to learn more about it.");
        Unpause();
    }

    public void EnemyInfoTrigger()
    {
        if(tutorialDay1 && !tutEnemyInfo.flag)
        {
            Pause();
            DialogManager.main.runner.StartDialogue("TutEnemyInfo");
            
            Debug.Log("Battle Triggers: EnemyInfo");
            tutEnemyInfo.flag = true;
            
            StartCoroutine(EnemyInfoTriggerPost(DialogManager.main.runner));
        }
    }

    private IEnumerator EnemyInfoTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        
        // re-enable unit selecting
        string[] units = {"Bapy", "Soleil", "Raina"};
        partyPhase.EnableUnits(new List<string>(units));
        Unpause();
    }

    public void EnemyRangedTrigger()
    {
        if(tutorialDay1 && !tutEnemyRanged.flag)
        {
            Pause();
            DialogManager.main.runner.StartDialogue("TutEnemyRanged");
            
            Debug.Log("Battle Triggers: EnemyRanged");
            tutEnemyRanged.flag = true;
            
            StartCoroutine(EnemyRangedTriggerPost(DialogManager.main.runner));
        }
    }

    private IEnumerator EnemyRangedTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        
        // no end functions
        
        Unpause();
    }

    public void DeathsDoorTrigger()
    {
        if(tutorialDay1 && !tutDD.flag)
        {
            tutDD.flag = true;
            Pause();
            var partyMemberAtDD = PhaseManager.main.PartyPhase.Party.Find((p) => p.DeathsDoor);
            //DialogueManager.main.runner.StartDialogue("TutDD" + partyMemberAtDD.DisplayName);
            if (partyMemberAtDD.DisplayName == "Bapy")
                DialogManager.main.runner.StartDialogue("TutDDBapy");
            else if(partyMemberAtDD.DisplayName == "Soleil")
                DialogManager.main.runner.StartDialogue("TutDDSoleil");
            else
                DialogManager.main.runner.StartDialogue("TutDDRaina");
            Debug.Log("Battle Triggers: DeathsDoor");


            StartCoroutine(DeathsDoorTriggerPost(DialogManager.main.runner));
        }
    }

    private IEnumerator DeathsDoorTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        
        // willow stuns all enemies, run tiles appear
        yield return partyPhase.WillowStunAll();
        DialogManager.main.runner.StartDialogue("TutStun");
        yield return new WaitWhile(() => runner.isDialogueRunning);
        DialogManager.main.runner.StartDialogue("TutRun");
        BattleUI.main.EnableRunTiles();
        yield return new WaitWhile(() => runner.isDialogueRunning);
        Unpause();
    }

    public void GraveInjuryTrigger()
    {
        Pause();
        var injuredPartyMember = PhaseManager.main.PartyPhase.Party.Find((p) => p.DeathsDoor && p.DeathsDoorCounter <= 0);
        DialogManager.main.runner.StartDialogue("GraveInjury" + injuredPartyMember.DisplayName);
        Debug.Log("Battle Triggers: GraveInjury");
        StartCoroutine(GraveInjuryTriggerPost(DialogManager.main.runner));
    }

    private IEnumerator GraveInjuryTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        PhaseManager.main.EndBattle();
    }

    public void NoEnemiesLeftInMainPhaseTrigger()
    {
        if (!inMainPhase || noEnemiesLeftMainPhase.flag || DoNotDestroyOnLoad.Instance.persistentData.numEnemiesLeft > 0)
            return;
        Pause();
        noEnemiesLeftMainPhase.flag = true;
        DialogManager.main.runner.StartDialogue("NoEnemiesRemain");
        Debug.Log("Battle Triggers: No Enemies Left In Main Encounter");
        StartCoroutine(NoEnemiesLeftInMainPhaseTriggerPost(DialogManager.main.runner));
    }

    private IEnumerator NoEnemiesLeftInMainPhaseTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        PhaseManager.main.EndBattle();
    }
}
