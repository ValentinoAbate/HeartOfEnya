using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEffectSplitUpDown : ActionEffect
{
    public int squares = 1;
    public bool centerIsDown = true;
    public int moveDamage = 0;

    public override IEnumerator ApplyEffect(Combatant user, Combatant target, ExtraData data)
    {
        Pos direction = Pos.Up;
        if (target.Pos.row > data.actionTargetPos.row)
            direction = Pos.Down;
        else if (centerIsDown && target.Pos.row == data.actionTargetPos.row)
            direction = Pos.Down;
        yield return StartCoroutine(DoMove(user, target, direction, moveDamage));
    }
}
