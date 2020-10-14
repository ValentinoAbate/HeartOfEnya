using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEffectPushToRightSide : ActionEffect
{
    public const int squares = 10;

    public override IEnumerator ApplyEffect(Combatant user, Combatant target, ExtraData data)
    {
        Pos direction = Pos.Right;
        for(int i = 0; i < squares; ++i)
            if (!BattleGrid.main.MoveAndSetWorldPos(target, target.Pos + direction))
                break;
        target.moveSfxEvent.Play();
        yield return new WaitForSeconds(effectWaitTime);
    }
}
