using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : ActiveAbilityEffect
{
    public int damage;
    public override void ApplyEffect(Combatant user, Combatant target, Reaction reaction)
    {
        if (reaction == Reaction.Immune)
            return;
        int effectiveDamage = damage;
        if (reaction == Reaction.Vulnerable)
        {
            if (target.Stunned)
                ++effectiveDamage;
            else
                target.Stunned = true;
        }
        target.Damage(effectiveDamage);
        Debug.Log(user.name + " dealt " + effectiveDamage + " damage to " + target.name);
    }
}
