using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : ActionEffect
{
    public int damage;
    public override void ApplyEffect(Combatant user, Combatant target)
    {
        Debug.Log(user.DisplayName + " dealt " + damage + " damage to " + target.DisplayName + " with " + name);
        target.Damage(damage);
    }
}
