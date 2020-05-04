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
    private bool tutorial; // whether we are in the tutorial or not

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

    [Header("Tutorial Day 3")]

    [Header("Absolute Zero Events")]
    public BattleEvent abs0PhaseChange;

    // references to objects that will be needed to disable certain game functions
    private PartyPhase partyPhase;

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
        tutorial = (DoNotDestroyOnLoad.Instance.persistentData.gamePhase == PersistentData.gamePhaseTutorial);
        partyPhase = PhaseManager.main.PartyPhase;
    }

    public void IntroTrigger()
    {
        if(tutorial && !tutorialIntro.flag)
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
        if(tutorial && !tutMove.flag)
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
        if(tutorial && !tutRainaAttack.flag)
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
        if(tutorial && !tutBapySelect.flag)
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
        if(tutorial && !tutBapyCancel.flag)
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
        if(tutorial && !tutSoleilSelect.flag)
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
        if(tutorial && !tutSoleilAttack.flag)
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
        if(tutorial && !tutSoleilChargeReminder.flag)
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
        if(tutorial && !tutSoleilChargeExplanation.flag)
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
        
        // put the post-code here
        
        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);
    }

    public void EnemySpawnWarningTrigger()
    {
        if(tutorial && !tutEnemySpawnWarning.flag)
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
        
        // put the post-code here
        
        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);
    }

    public void EnemySpawnTrigger()
    {
        if(tutorial && !tutEnemySpawn.flag)
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
        
        // put the post-code here
        
        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);
    }

    public void EnemyInfoTrigger()
    {
        if(tutorial && !tutEnemyInfo.flag)
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
        
        // put the post-code here
        
        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);
    }

    public void EnemyRangedTrigger()
    {
        if(tutorial && !tutEnemyRanged.flag)
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
        
        // put the post-code here
        
        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);
    }

    public void DeathsDoorTrigger()
    {
        if(!tutDD.flag)
        {
            Debug.Log("Battle Triggers: DeathsDoor");
            tutDD.flag = true;
        }
    }

    private IEnumerator DeathsDoorTriggerPost(DialogueRunner runner)
    {
        yield return new WaitWhile(() => runner.isDialogueRunning);
        
        // put the post-code here
    }
}
