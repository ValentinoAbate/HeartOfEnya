using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEffect : ActionEffect
{
    public enum Direction
    {
        Toward,
        Away,
        Hybrid,
    }
    public Direction moveType;
    public int squares = 1;

    public override IEnumerator ApplyEffect(Combatant user, Combatant target, Pos actionTargetPos)
    {
        Pos direction = Pos.DirectionBasic(user.Pos, target.Pos);
        if (moveType == Direction.Toward || (moveType == Direction.Hybrid && Pos.Distance(user.Pos, target.Pos) > 1))
            direction *= -1;
        for (int i = 0; i < squares; ++i)
            if (!BattleGrid.main.MoveAndSetWorldPos(target, target.Pos + direction))
                break;
        var src = GetComponent<AudioSource>();
        src.PlayOneShot(target.moveSfx);
        yield return new WaitForSeconds(target.moveSfx.length);
    }
}
