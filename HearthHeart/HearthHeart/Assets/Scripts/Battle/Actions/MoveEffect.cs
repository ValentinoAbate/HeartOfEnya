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

    public override void ApplyEffect(Combatant user, Combatant target, Reaction reaction)
    {
        // Apply immunity (cancel effect)
        if (reaction == Reaction.Immune)
        {
            Debug.Log(target.DisplayName + " resisted the movement effect from " + user.DisplayName + "'s " + name);
            return;
        }
        // Apply vulnerability (stun)
        if (reaction == Reaction.Vulnerable && !target.Stunned)
            target.Stunned = true;
        Pos direction = Pos.DirectionBasic(user.Pos, target.Pos);
        if (moveType == Direction.Toward)
            direction *= -1;
        BattleGrid.main.MoveAndSetPosition(target, target.Pos + direction);
    }
}
