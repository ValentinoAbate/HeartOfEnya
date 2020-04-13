using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : ActionEffect
{
    public int damage;
    public override IEnumerator ApplyEffect(Combatant user, Combatant target)
    {
        Debug.Log(user.DisplayName + " dealt " + damage + " damage to " + target.DisplayName + " with " + name);
        bool death = damage >= target.Hp;
        var src = GetComponent<AudioSource>();
        target.Damage(damage);
        src.PlayOneShot(target.damageSfx);
        yield return new WaitForSeconds(target.damageSfx.length);
        if (death)
        {
            src.PlayOneShot(target.deathSfx);
            yield return new WaitForSeconds(target.deathSfx.length);
        }
    }
}
