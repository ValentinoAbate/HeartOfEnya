using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEffect : ActionEffect
{
    public enum Direction
    {
        Toward,
        Away,
    }
    public Direction moveType;
    public int squares = 1;

    public override void ApplyEffect(Combatant user, Combatant target)
    {
        Pos direction = Pos.DirectionBasic(user.Pos, target.Pos);
        if (moveType == Direction.Toward)
            direction *= -1;
        BattleGrid.main.MoveAndSetWorldPos(target, target.Pos + direction);
    }
}
