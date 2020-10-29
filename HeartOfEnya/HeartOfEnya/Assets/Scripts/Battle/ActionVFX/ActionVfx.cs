using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionVfx : MonoBehaviour
{
    public bool allowFlipY = true;
    public bool allowFlipX = true;
    public abstract Coroutine Play();
}
