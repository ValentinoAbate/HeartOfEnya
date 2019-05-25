using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Phase : MonoBehaviour
{
    public string ID;

    public abstract Coroutine OnPhaseStart();
    public abstract Coroutine OnPhaseEnd();
    public abstract void OnPhaseUpdate();
}
