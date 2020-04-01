using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIComponent<T> : MonoBehaviour where T : FieldObject
{
    public abstract IEnumerator DoTurn(T self);
}
