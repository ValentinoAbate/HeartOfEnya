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
            effectiveDamage *= 2;
        target.Damage(effectiveDamage);
    }
}
