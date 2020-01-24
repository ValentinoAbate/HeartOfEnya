using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunEffect : ActionEffect
{
    public override void ApplyEffect(Combatant user, Combatant target, Reaction reaction)
    {
        if (reaction == Reaction.Immune)
            return;
        if (!target.Stunned)
            target.Stunned = true;
    }
}
