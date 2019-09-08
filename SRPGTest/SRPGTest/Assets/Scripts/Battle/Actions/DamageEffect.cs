using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : ActionEffect
{
    public int damage;
    public override void ApplyEffect(Combatant user, Combatant target, Reaction reaction)
    {
        if (reaction == Reaction.Immune)
        {
            Debug.Log(target.name + " blocked " + damage + " " + attribute.ToString() + " damage from " + user.name + "'s " + name);
            return;
        }

        int effectiveDamage = damage;
        if (reaction == Reaction.Vulnerable)
        {
            if (target.Stunned)
            {
                ++effectiveDamage;
                Debug.Log(user.name + " dealt " + effectiveDamage + " " + attribute.ToString() + " damage to " + target.name + " with " + name);
            }               
            else
            {
                Debug.Log(user.name + " dealt " + effectiveDamage + " " + attribute.ToString() + " damage to " + target.name + " with " + name);
                target.Stunned = true;
            }             
        }
        target.Damage(effectiveDamage);
    }
}
