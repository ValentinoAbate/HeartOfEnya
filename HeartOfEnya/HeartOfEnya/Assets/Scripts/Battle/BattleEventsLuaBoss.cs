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
        StartCoroutine(LuaPhaseChangeRoutine());
    }

    private IEnumerator LuaPhaseChangeRoutine()
    {
        var runner = DialogueManager.main.runner;
        runner.StartDialogue("LuaBossIntro");
        yield return new WaitWhile(() => runner.isDialogueRunning);
        battleEvents.Unpause();
    }

    public void LuaBossPhaseChange()
    {
        if (!battleEvents.luaBattle)
            return;
        if (battleEvents.luaBossPhaseChange.flag)
            return;
        var luaBoss = BattleGrid.main.Find<Combatant>((c) => c.GetComponent<EnemyAILuaBoss>() != null);
        if (luaBoss == null || !luaBoss.Dead)
            return;
        battleEvents.Pause();
        StartCoroutine(LuaPhaseChangeRoutine(luaBoss));
    }

    private IEnumerator LuaPhaseChangeRoutine(Combatant luaBoss)
    {
        var aiComponent = luaBoss.GetComponent<EnemyAILuaBoss>();
        aiComponent.secondPhase = true;
        luaBoss.CancelChargingAction();
        var runner = DialogueManager.main.runner;
        // Pre-transition
        runner.StartDialogue("LuaBossPhase2-1");
        yield return new WaitWhile(() => runner.isDialogueRunning);
        // Transition
        yield return luaBoss.UseAction(aiComponent.clearObstaclesAndEnemies, Pos.Zero, Pos.Zero);
        PhaseManager.main.SpawnPhase.ClearActiveSpawns();
        yield return luaBoss.UseAction(aiComponent.moveAllToRight, Pos.Zero, Pos.Zero);
        PhaseManager.main.SpawnPhase.SetEncounter(aiComponent.secondPhaseEnounter, true);
        // Revive Luicicle
        luaBoss.Hp = aiComponent.secondPhaseHp;
        PhaseManager.main.EnemyPhase.Enemies.Add(luaBoss as Enemy);
        // Post-Transition
        runner.StartDialogue("LuaBossPhase2-2");
        yield return new WaitWhile(() => runner.isDialogueRunning);
        battleEvents.luaBossPhaseChange.flag = true;
        PhaseManager.main.NextPhase();
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
        pData.luaBossDefeated = true;
        battleEvents.Pause();
        StartCoroutine(LuaPhaseDeathRoutine());
    }

    private IEnumerator LuaPhaseDeathRoutine()
    {
        var runner = DialogueManager.main.runner;
        runner.StartDialogue("LuaBossOutro");
        yield return new WaitWhile(() => runner.isDialogueRunning);
        SceneTransitionManager.main.TransitionScenes("Camp");
    }
}
