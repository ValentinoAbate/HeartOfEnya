using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEffectPushToRightSide : ActionEffect
{
    public const int squares = 10;

    public override IEnumerator ApplyEffect(Combatant user, Combatant target, Pos actionTargetPos)
    {
        Pos direction = Pos.Right;
        for(int i = 0; i < squares; ++i)
            if (!BattleGrid.main.MoveAndSetWorldPos(target, target.Pos + direction))
                break;
        var src = GetComponent<AudioSource>();
        src.PlayOneShot(target.moveSfx);
        yield return new WaitForSeconds(target.moveSfx.length);
    }
}
