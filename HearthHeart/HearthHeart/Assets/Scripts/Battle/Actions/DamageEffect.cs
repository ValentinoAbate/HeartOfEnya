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
            Debug.Log(target.DisplayName + " blocked " + damage + " " + attribute.ToString() + " damage from " + user.DisplayName + "'s " + name);
            return;
        }
        if (reaction == Reaction.Vulnerable && !target.Stunned)
        {              
            target.Stunned = true;         
        }
        Debug.Log(user.name + " dealt " + damage + " " + attribute.ToString() + " damage to " + target.name + " with " + name);
        target.Damage(damage);
    }
}
