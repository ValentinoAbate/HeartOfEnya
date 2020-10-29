using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : ActionEffect
{
    public int damage;
    public override IEnumerator ApplyEffect(Combatant user, Combatant target, ExtraData data)
    {
        Debug.Log(user.DisplayName + " dealt " + damage + " damage to " + target.DisplayName + " with " + name);
        if(target is Enemy && (target as Enemy).isBoss && target.Team == user.Team)
            yield break;
        target.Damage(damage); // Where the death actually happens rn
        yield return new WaitForSeconds(effectWaitTime);
    }
}
