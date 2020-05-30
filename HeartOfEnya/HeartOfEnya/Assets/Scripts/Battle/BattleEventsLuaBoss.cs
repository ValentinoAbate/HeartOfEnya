using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEventsLuaBoss : MonoBehaviour
{
    public void LuaBossPhaseChange()
    {
        if (BattleEvents.main.luaBossPhaseChange.flag)
            return;
        BattleEvents.main.luaBossPhaseChange.flag = true;
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        if (!pData.InLuaBattle)
            return;
        var luaBoss = BattleGrid.main.Find<Combatant>((c) => c.GetComponent<EnemyAILuaBoss>() != null);
        if (luaBoss == null || !luaBoss.Dead)
            return;
        PhaseManager.main.PauseHandle.Pause(PauseHandle.PauseSource.BattleInterrupt);
        var aiComponent = luaBoss.GetComponent<EnemyAILuaBoss>();
        aiComponent.secondPhase = true;
        luaBoss.Hp = aiComponent.secondPhaseHp;

        StartCoroutine(LuaPhaseChangeRoutine(luaBoss, aiComponent));
    }

    private IEnumerator LuaPhaseChangeRoutine(Combatant luaBoss, EnemyAILuaBoss aiComponent)
    {
        luaBoss.CancelChargingAction();
        yield return luaBoss.UseAction(aiComponent.clearObstaclesAndEnemies, Pos.Zero, Pos.Zero);
        PhaseManager.main.SpawnPhase.ClearActiveSpawns();
        yield return luaBoss.UseAction(aiComponent.moveAllToRight, Pos.Zero, Pos.Zero);
        PhaseManager.main.SpawnPhase.SetEncounter(aiComponent.secondPhaseEnounter, true);
        PhaseManager.main.NextPhase();
        // Unpause
        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);
    }

    public void LuaBossDeathTrigger()
    {
        if (BattleEvents.main.luaBossPhase2Defeated.flag || !BattleEvents.main.luaBossPhaseChange.flag)
            return;
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        if (!pData.InLuaBattle)
            return;
        var luaBoss = BattleGrid.main.Find<Combatant>((c) => c.GetComponent<EnemyAILuaBoss>() != null);
        if (luaBoss == null || !luaBoss.Dead)
            return;
        luaBoss.invincible = false;
        luaBoss.Damage(10);
        pData.luaBossDefeated = true;
    }
}
