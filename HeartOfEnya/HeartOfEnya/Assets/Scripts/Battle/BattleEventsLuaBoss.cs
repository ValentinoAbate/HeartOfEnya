using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEventsLuaBoss : MonoBehaviour
{
    public BattleEvents battleEvents;

    public void LuaBossIntroTrigger()
    {
        if (!battleEvents.luaBattle)
            return;
        if (battleEvents.luaBossIntro.flag)
            return;
        battleEvents.luaBossIntro.flag = true;
        battleEvents.Pause();
        StartCoroutine(LuaBossIntroRoutine());
    }

    private IEnumerator LuaBossIntroRoutine()
    {
        var runner = DialogManager.main.runner;
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        if (pData.luaBossPhase1Defeated)
        {
            runner.StartDialogue("LuaBossIntroPhase2");
        }
        else if(pData.dayNum > PersistentData.dayNumStart + 1)
        {
            runner.StartDialogue("LuaBossIntroRepeat");
        }
        else
        {
            runner.StartDialogue("LuaBossIntro");
        }
        yield return new WaitWhile(() => runner.isDialogueRunning);
        battleEvents.Unpause();
    }

    public void LuaBossPhaseChange()
    {
        if (!battleEvents.luaBattle)
            return;
        if (battleEvents.luaBossPhaseChange.flag)
            return;
        var luaBoss = BattleGrid.main.Find<Enemy>((c) => c.GetComponent<EnemyAILuaBoss>() != null);
        if (luaBoss == null || !luaBoss.Dead)
            return;
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        if (pData.luaBossPhase1Defeated)
        {
            // Skip the transition
            battleEvents.luaBossPhaseChange.flag = true;
            return;
        }
        pData.luaBossPhase1Defeated = true;
        battleEvents.Pause();
        // cancel bonus moves
        foreach (var partyMember in PhaseManager.main.PartyPhase.Party)
        {
            var moveCursor = partyMember.GetComponent<MouseMoveCursor>();
            moveCursor.CancelBonusMode();
        }
        StartCoroutine(LuaPhaseChangeRoutine(luaBoss));
    }

    private IEnumerator LuaPhaseChangeRoutine(Enemy luaBoss)
    {
        var aiComponent = luaBoss.GetComponent<EnemyAILuaBoss>();
        luaBoss.sprite.sprite = aiComponent.secondPhaseSprite;
        aiComponent.secondPhase = true;
        luaBoss.CancelChargingAction();
        var runner = DialogManager.main.runner;
        var pManager = PhaseManager.main;
        // Pre-transition
        runner.StartDialogue("LuaBossPhase2-1");
        yield return new WaitWhile(() => runner.isDialogueRunning);
        // Transition
        yield return luaBoss.UseAction(aiComponent.clearObstaclesAndEnemies, Pos.Zero, Pos.Zero);
        pManager.SpawnPhase.ClearActiveSpawns();
        yield return luaBoss.UseAction(aiComponent.moveAllToRight, Pos.Zero, Pos.Zero);
        pManager.SpawnPhase.SetEncounter(aiComponent.secondPhaseEnounter, true);
        // Revive Luicicle
        luaBoss.Hp = aiComponent.secondPhaseHp;
        //if (!pManager.EnemyPhase.Enemies.Contains(luaBoss as Enemy))
            //pManager.EnemyPhase.Enemies.Add(luaBoss as Enemy);
        // Post-Transition
        runner.StartDialogue("LuaBossPhase2-2");
        yield return new WaitWhile(() => runner.isDialogueRunning);
        battleEvents.luaBossPhaseChange.flag = true;
        pManager.NextPhase();
        // Unpause
        battleEvents.Unpause();
    }

    public void LuaBossDeathTrigger()
    {
        if (!battleEvents.luaBattle)
            return;
        if (battleEvents.luaBossPhase2Defeated.flag || !battleEvents.luaBossPhaseChange.flag)
            return;
        battleEvents.luaBossPhase2Defeated.flag = true;
        var luaBoss = BattleGrid.main.Find<Combatant>((c) => c.GetComponent<EnemyAILuaBoss>() != null);
        if (luaBoss == null || !luaBoss.Dead)
            return;
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        pData.luaBossPhase2Defeated = true;
        battleEvents.Pause();
        FMODBattle.main.Music.Stop();
        StartCoroutine(LuaPhaseDeathRoutine());
    }

    private IEnumerator LuaPhaseDeathRoutine()
    {
        var runner = DialogManager.main.runner;
        runner.StartDialogue("LuaBossOutro");
        yield return new WaitWhile(() => runner.isDialogueRunning);
        PhaseManager.main.EndBattle();
    }
}
