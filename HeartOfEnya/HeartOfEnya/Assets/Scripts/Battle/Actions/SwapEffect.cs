using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapEffect : ActionEffect
{
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
        {
            if(user.Team != target.Team)
                target.Stunned = true;
        }
        BattleGrid.main.SwapAndSetWorldPos(user, target);
    }
}
