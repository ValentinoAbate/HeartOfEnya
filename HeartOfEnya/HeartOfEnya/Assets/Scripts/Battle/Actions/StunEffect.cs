using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunEffect : ActionEffect
{
    public override void ApplyEffect(Combatant user, Combatant target)
    {
        if (!target.Stunned)
            target.Stunned = true;
    }
}
