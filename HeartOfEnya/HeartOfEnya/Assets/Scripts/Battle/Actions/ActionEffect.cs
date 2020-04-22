using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionEffect : MonoBehaviour
{
    public enum Target
    { 
        Target = 1,
        Tile,
    }
    public const float effectWaitTime = 0.4f;

    public Target target = Target.Target;

    public abstract IEnumerator ApplyEffect(Combatant user, Combatant target, ExtraData data);

    public virtual IEnumerator ApplyEffect(Combatant user, Pos target, ExtraData data) { yield break; }

    public struct ExtraData
    {
        public Pos actionTargetPos;
        public Pos primaryTargetPos;
    }

}
