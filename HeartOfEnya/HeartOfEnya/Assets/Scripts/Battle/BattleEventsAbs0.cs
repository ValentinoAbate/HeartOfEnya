using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEventsAbs0 : MonoBehaviour
{
    public PauseManager pauseManager;
    public void Abs0PhaseChange()
    {
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        if (pData.gamePhase != PersistentData.gamePhaseAbsoluteZeroBattle)
            return;
        var abs0 = BattleGrid.main.Find<Combatant>((c) => c.GetComponent<EnemyAIAbs0Boss>() != null);
        if (abs0 == null || !abs0.Dead)
            return;
        PhaseManager.main.PauseHandle.Pause(PauseHandle.PauseSource.BattleInterrupt);
        pData.absoluteZeroDefeated = true;
        abs0.Hp = 9;
        abs0.invincible = false;
        var aiComponent = abs0.GetComponent<EnemyAIAbs0Boss>();
        aiComponent.secondPhase = true;
        StartCoroutine(Abs0PhaseChangeRoutine(abs0, aiComponent));
    }

    private IEnumerator Abs0PhaseChangeRoutine(Combatant abs0, EnemyAIAbs0Boss aiComponent)
    {
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        yield return abs0.UseAction(aiComponent.clearObstacles, Pos.Zero, Pos.Zero);
        yield return abs0.UseAction(aiComponent.moveAllToRight, Pos.Zero, Pos.Zero);
        yield return abs0.UseAction(aiComponent.spawnObstacles, Pos.Zero, Pos.Zero);
        foreach(var partyMember in PhaseManager.main.PartyPhase.Party)
        {
            partyMember.Hp = partyMember.maxHp;
            partyMember.Fp = partyMember.maxFp;
        }
        BattleUI.main.UpdateEnemiesRemaining(pData.numEnemiesLeft);
        // Start dialog scene here
        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);
    }
}
