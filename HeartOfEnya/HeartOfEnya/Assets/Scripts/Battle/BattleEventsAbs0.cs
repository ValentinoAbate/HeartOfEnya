using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEventsAbs0 : MonoBehaviour
{
    public void Abs0PhaseChange()
    {
        if (BattleEvents.main.abs0PhaseChange.flag)
            return;
        BattleEvents.main.abs0PhaseChange.flag = true;
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        if (pData.gamePhase != PersistentData.gamePhaseAbsoluteZeroBattle)
            return;
        var abs0 = BattleGrid.main.Find<Combatant>((c) => c.GetComponent<EnemyAIAbs0Boss>() != null);
        if (abs0 == null || !abs0.Dead)
            return;
        PhaseManager.main.PauseHandle.Pause(PauseHandle.PauseSource.BattleInterrupt);
        pData.absoluteZeroDefeated = true;
        abs0.Hp = 9;
        var aiComponent = abs0.GetComponent<EnemyAIAbs0Boss>();
        aiComponent.secondPhase = true;
        StartCoroutine(Abs0PhaseChangeRoutine(abs0, aiComponent));
    }

    private IEnumerator Abs0PhaseChangeRoutine(Combatant abs0, EnemyAIAbs0Boss aiComponent)
    {
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        yield return abs0.UseAction(aiComponent.clearObstaclesAndEnemies, Pos.Zero, Pos.Zero);
        yield return abs0.UseAction(aiComponent.moveAllToRight, Pos.Zero, Pos.Zero);
        yield return abs0.UseAction(aiComponent.spawnObstacles, Pos.Zero, Pos.Zero);
        yield return StartCoroutine(PhaseManager.main.SpawnPhase.DeclareNextWave());
        // Heal the party to full health
        foreach(var partyMember in PhaseManager.main.PartyPhase.Party)
        {
            partyMember.Hp = partyMember.maxHp;
            partyMember.Fp = partyMember.maxFp;
        }
        // Update the counter to actually show the number that remain +1 for abs0
        BattleUI.main.UpdateEnemiesRemaining(pData.numEnemiesLeft + 1);
        // Unpause here
        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);
    }

    public void Abs0Phase2Defeated()
    {
        if (BattleEvents.main.abs0Phase2Defeated.flag || !BattleEvents.main.abs0PhaseChange.flag)
            return;
        var pData = DoNotDestroyOnLoad.Instance.persistentData;
        if (pData.gamePhase != PersistentData.gamePhaseAbsoluteZeroBattle)
            return;
        var abs0 = BattleGrid.main.Find<Combatant>((c) => c.GetComponent<EnemyAIAbs0Boss>() != null);
        if (abs0 == null || !abs0.Dead)
            return;
        PhaseManager.main.PauseHandle.Pause(PauseHandle.PauseSource.BattleInterrupt);
        abs0.invincible = false;
        abs0.Damage(10);
        pData.absoluteZeroPhase2Defeated = true;
        StartCoroutine(Abs0Phase2DefeatedRoutine());
    }

    private IEnumerator Abs0Phase2DefeatedRoutine()
    {
        yield return new WaitForSeconds(1);
        PhaseManager.main.PauseHandle.Unpause(PauseHandle.PauseSource.BattleInterrupt);
    }
}
