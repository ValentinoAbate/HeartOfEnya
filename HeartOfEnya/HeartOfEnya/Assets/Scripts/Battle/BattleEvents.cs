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

    [Header("Tutorial Events")]
    public BattleEvent tutorialIntro;
    public BattleEvent tutMove;
    public BattleEvent tutRainaAttack;
    public BattleEvent tutBapySelect;
    public BattleEvent tutBapyCancel;
    public BattleEvent tutSoleilSelect;
    public BattleEvent tutSoleilAttack;
    public BattleEvent tutSoleilChargeReminder;
    public BattleEvent tutSoleilChargeExplanation;
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
    public BattleEvent tutEnemyPush;

    [Header("Tutorial Day 3")]
    public BattleEvent tutFlameMoves;
    public BattleEvent tutBurn;

    [Header("Lua Boss Events")]
    public BattleEvent luaBossPhaseChange;
    public BattleEvent luaBossPhase2Defeated;

    [Header("Absolute Zero Events")]
    public BattleEvent abs0PhaseChange;
    public BattleEvent abs0Phase2Defeated;

    // references to objects that will be needed to disable certain game functions
    [HideInInspector] public PartyPhase partyPhase;

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
        tutorialDay1 = DoNotDestroyOnLoad.Instance.persistentData.InTutorialFirstDay;
        tutorialDay2 = DoNotDestroyOnLoad.Instance.persistentData.InTutorialSecondDay;
        tutorialDay3 = DoNotDestroyOnLoad.Instance.persistentData.InTutorialThirdDay;
        partyPhase = PhaseManager.main.PartyPhase;
    }

    public void IntroTrigger()
    {
        if(tutorialDay1 && !tutorialIntro.flag)
        {
            Debug.Log("Battle Triggers: start of battle");
            tutorialIntro.flag = true;

            // Start the dialog (connect to ambers code)
            PhaseManager.main.PauseHandle.Pause(PauseHandle.PauseSource.BattleInterrupt);
            DialogueManager.main.runner.StartDialogue("TutIntro");
            
            // Wait for finish StartCoroutine(IntroTriggerPost(runner))
            StartCoroutine(IntroTriggerPost(DialogueManager.main.runner));
        }
    }

    private IEnumerator IntroTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);

        // post-condition: disable everyone but raina
        string[] units = {"Bapy", "Soleil"};
        partyPhase.DisableUnits(new List<string>(units));

        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);
        
    }

    public void MoveTrigger()
    {
        if(tutorialDay1 && !tutMove.flag)
        {
            PhaseManager.main.PauseHandle.Pause(PauseHandle.PauseSource.BattleInterrupt);
            DialogueManager.main.runner.StartDialogue("TutMove");
            
            Debug.Log("Battle Triggers: raina move");
            tutMove.flag = true;
            
            StartCoroutine(MoveTriggerPost(DialogueManager.main.runner));
        }
    }

    private IEnumerator MoveTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);

        // restrict raina's movement to specific square
        BattleUI.main.MoveableTiles.Add(new Pos(1, 6));
        
        // disable cancelling, solo raina's cleave move
        BattleUI.main.CancelingEnabled = false;
        partyPhase.PartyWideSoloAction("RainaAction2");

        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);
    }

    public void RainaAttackTrigger()
    {
        if(tutorialDay1 && !tutRainaAttack.flag)
        {
            PhaseManager.main.PauseHandle.Pause(PauseHandle.PauseSource.BattleInterrupt);
            DialogueManager.main.runner.StartDialogue("TutRainaAttack");
            
            Debug.Log("Battle Triggers: raina attack");
            tutRainaAttack.flag = true;
            
            StartCoroutine(RainaAttackTriggerPost(DialogueManager.main.runner));
        }
    }

    private IEnumerator RainaAttackTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        
        // restrict raina to attacking to the left
        BattleUI.main.TargetableTiles.Add(new Pos (1, 5));

        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);
    }

    public void BapySelectTrigger()
    {
        if(tutorialDay1 && !tutBapySelect.flag)
        {
            PhaseManager.main.PauseHandle.Pause(PauseHandle.PauseSource.BattleInterrupt);
            DialogueManager.main.runner.StartDialogue("TutBapySelect");
            
            Debug.Log("Battle Triggers: select bapy");
            tutBapySelect.flag = true;
            
            StartCoroutine(BapySelectTriggerPost(DialogueManager.main.runner));
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
        
        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);
    }

    public void BapyCancelTrigger()
    {
        if(tutorialDay1 && !tutBapyCancel.flag)
        {
            PhaseManager.main.PauseHandle.Pause(PauseHandle.PauseSource.BattleInterrupt);
            DialogueManager.main.runner.StartDialogue("TutBapyCancel");
            
            Debug.Log("Battle Triggers: bapy cancel");
            tutBapyCancel.flag = true;
            
            StartCoroutine(BapyCancelTriggerPost(DialogueManager.main.runner));
        }
    }

    private IEnumerator BapyCancelTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        
        // restrict bapy's actions to only cancel
        BattleUI.main.CancelingEnabled = true;
        
        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);
    }

    public void SoleilSelectTrigger()
    {
        if(tutorialDay1 && !tutSoleilSelect.flag)
        {
            PhaseManager.main.PauseHandle.Pause(PauseHandle.PauseSource.BattleInterrupt);
            DialogueManager.main.runner.StartDialogue("TutSoleilSelect");
            
            Debug.Log("Battle Triggers: select soleil");
            tutSoleilSelect.flag = true;
            
            StartCoroutine(SoleilSelectTriggerPost(DialogueManager.main.runner));
        }
    }

    private IEnumerator SoleilSelectTriggerPost(DialogueRunner runner)
    {

        yield return new WaitWhile(() => runner.isDialogueRunning);
        
        // enable soleil
        partyPhase.EnableUnits("Soleil");

        // disable bapy
        partyPhase.DisableUnits("Bapy");

        // enable soleil's charge attack
        partyPhase.PartyWideSoloAction("SoleilAction2");
        BattleUI.main.CancelingEnabled = false;

        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);
    }

    public void SoleilAttackTrigger()
    {
        if(tutorialDay1 && !tutSoleilAttack.flag)
        {
            PhaseManager.main.PauseHandle.Pause(PauseHandle.PauseSource.BattleInterrupt);
            DialogueManager.main.runner.StartDialogue("TutSoleilAttack");
            
            Debug.Log("Battle Triggers: soleil attack");
            tutSoleilAttack.flag = true;
            
            StartCoroutine(SoleilAttackTriggerPost(DialogueManager.main.runner));
        }
    }

    private IEnumerator SoleilAttackTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        
        // re-enable bapy (and everyone else since their turns are over anyway)
        string[] units = {"Bapy", "Soleil", "Raina"};
        partyPhase.EnableUnits(new List<string>(units));

        // *****likely tentative? enable bapy's hoe
        partyPhase.PartyWideSoloAction("BapyAction1");
        BattleUI.main.CancelingEnabled = true;
        
        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);
    }

    public void SoleilChargeReminderTrigger()
    {
        if(tutorialDay1 && !tutSoleilChargeReminder.flag)
        {
            PhaseManager.main.PauseHandle.Pause(PauseHandle.PauseSource.BattleInterrupt);
            DialogueManager.main.runner.StartDialogue("TutSoleilChargeReminder");
            
            Debug.Log("Battle Triggers: SoleilChargeReminder");
            tutSoleilChargeReminder.flag = true;
            
            StartCoroutine(SoleilChargeReminderTriggerPost(DialogueManager.main.runner));
        }
    }

    private IEnumerator SoleilChargeReminderTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        
        // re-enable all actions
        partyPhase.PartyWideClearSoloActions();
        
        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);
    }

    public void SoleilChargeExplanationTrigger()
    {
        if(tutorialDay1 && !tutSoleilChargeExplanation.flag)
        {
            PhaseManager.main.PauseHandle.Pause(PauseHandle.PauseSource.BattleInterrupt);
            DialogueManager.main.runner.StartDialogue("TutSoleilChargeExplanation");
            
            Debug.Log("Battle Triggers: SoleilChargeExplanation");
            tutSoleilChargeExplanation.flag = true;
            
            StartCoroutine(SoleilChargeExplanationTriggerPost(DialogueManager.main.runner));
        }
    }

    private IEnumerator SoleilChargeExplanationTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        
        // no end functions
        
        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);
    }

    public void EnemySpawnWarningTrigger()
    {
        if(tutorialDay1 && !tutEnemySpawnWarning.flag)
        {
            PhaseManager.main.PauseHandle.Pause(PauseHandle.PauseSource.BattleInterrupt);
            DialogueManager.main.runner.StartDialogue("TutEnemySpawnWarning");
            
            Debug.Log("Battle Triggers: EnemySpawnWarning");
            tutEnemySpawnWarning.flag = true;
            
            StartCoroutine(EnemySpawnWarningTriggerPost(DialogueManager.main.runner));
        }
    }

    private IEnumerator EnemySpawnWarningTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        
        // no end functions
        
        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);
    }

    public void EnemySpawnTrigger()
    {
        if(tutorialDay1 && !tutEnemySpawn.flag)
        {
            PhaseManager.main.PauseHandle.Pause(PauseHandle.PauseSource.BattleInterrupt);
            DialogueManager.main.runner.StartDialogue("TutEnemySpawn");
            
            Debug.Log("Battle Triggers: EnemySpawn");
            tutEnemySpawn.flag = true;
            
            StartCoroutine(EnemySpawnTriggerPost(DialogueManager.main.runner));
        }
    }

    private IEnumerator EnemySpawnTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        
        // disable unit selecting
        string[] units = {"Bapy", "Soleil", "Raina"};
        partyPhase.DisableUnits(new List<string>(units));
        
        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);
    }

    public void EnemyInfoTrigger()
    {
        if(tutorialDay1 && !tutEnemyInfo.flag)
        {
            PhaseManager.main.PauseHandle.Pause(PauseHandle.PauseSource.BattleInterrupt);
            DialogueManager.main.runner.StartDialogue("TutEnemyInfo");
            
            Debug.Log("Battle Triggers: EnemyInfo");
            tutEnemyInfo.flag = true;
            
            StartCoroutine(EnemyInfoTriggerPost(DialogueManager.main.runner));
        }
    }

    private IEnumerator EnemyInfoTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        
        // re-enable unit selecting
        string[] units = {"Bapy", "Soleil", "Raina"};
        partyPhase.EnableUnits(new List<string>(units));
        
        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);
    }

    public void EnemyRangedTrigger()
    {
        if(tutorialDay1 && !tutEnemyRanged.flag)
        {
            PhaseManager.main.PauseHandle.Pause(PauseHandle.PauseSource.BattleInterrupt);
            DialogueManager.main.runner.StartDialogue("TutEnemyRanged");
            
            Debug.Log("Battle Triggers: EnemyRanged");
            tutEnemyRanged.flag = true;
            
            StartCoroutine(EnemyRangedTriggerPost(DialogueManager.main.runner));
        }
    }

    private IEnumerator EnemyRangedTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        
        // no end functions
        
        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);
    }

    public void DeathsDoorTrigger()
    {
        if(!tutDD.flag)
        {
            PhaseManager.main.PauseHandle.Pause(PauseHandle.PauseSource.BattleInterrupt);

            Debug.Log("Battle Triggers: DeathsDoor");
            tutDD.flag = true;

            StartCoroutine(DeathsDoorTriggerPost(DialogueManager.main.runner));
        }
    }

    private IEnumerator DeathsDoorTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        
        // willow stuns all enemies, run tiles appear
        yield return partyPhase.WillowStunAll();
        BattleUI.main.EnableRunTiles();

        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);
    }
}
