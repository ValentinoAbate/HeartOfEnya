using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPrimaryTargetPositionEffect : ActionEffect
{
    public override IEnumerator ApplyEffect(Combatant user, Combatant target, ExtraData data)
    {
        yield break;
    }

    public override IEnumerator ApplyEffect(Combatant user, Pos target, ExtraData data)
    {
        var targetCombatant = BattleGrid.main.GetObject(data.primaryTargetPos).GetComponent<Combatant>();
        if (targetCombatant == null || !targetCombatant.isMovable)
            yield break;
        BattleGrid.main.MoveAndSetWorldPos(targetCombatant, target);
        yield return new WaitForSeconds(effectWaitTime);
    }
}
