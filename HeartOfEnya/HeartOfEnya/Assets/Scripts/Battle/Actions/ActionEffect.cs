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

    public Target target = Target.Other;

    public abstract void ApplyEffect(Combatant user, Combatant target);

    public virtual void ApplyEffect(Combatant user, Pos target) { }

}
