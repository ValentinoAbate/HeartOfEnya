using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEffectSplitUpDown : ActionEffect
{
    public int squares = 1;
    public bool centerIsDown = true;

    public override IEnumerator ApplyEffect(Combatant user, Combatant target, Pos actionTargetPos)
    {
        Pos direction = Pos.Up;
        if (target.Pos.row > actionTargetPos.row)
            direction = Pos.Down;
        else if (centerIsDown && target.Pos.row == actionTargetPos.row)
            direction = Pos.Down;
        for(int i = 0; i < squares; ++i)
            if (!BattleGrid.main.MoveAndSetWorldPos(target, target.Pos + direction))
                break;
        var src = GetComponent<AudioSource>();
        src.PlayOneShot(target.moveSfx);
        yield return new WaitForSeconds(target.moveSfx.length);
    }
}
