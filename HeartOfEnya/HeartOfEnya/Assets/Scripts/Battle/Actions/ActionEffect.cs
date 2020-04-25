using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionEffect : MonoBehaviour
{
    public enum Target
    { 
        Self,
        Other,
        Tile,
    }
    public const float effectWaitTime = 0.4f;

    public Target target = Target.Other;

    public abstract IEnumerator ApplyEffect(Combatant user, Combatant target, Pos actionTargetPos);

    public virtual IEnumerator ApplyEffect(Combatant user, Pos target) { yield break; }

}
