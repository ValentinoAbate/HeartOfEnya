using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunEffect : ActionEffect
{
    public override IEnumerator ApplyEffect(Combatant user, Combatant target, ExtraData data)
    {
        yield break;
        // Stunning now happens all at the same time in the action script
        //if (user.Team == target.Team)
        //    yield break;
        //if (!target.Stunned)
        //    target.Stunned = true;
        //yield break;
    }
}
