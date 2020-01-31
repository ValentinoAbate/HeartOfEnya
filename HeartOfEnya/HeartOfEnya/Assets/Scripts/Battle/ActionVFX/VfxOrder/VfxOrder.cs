using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class VfxOrder : MonoBehaviour
{
    public abstract bool UseBatches { get; }
    public abstract List<List<Pos>> GetPositions(List<Pos> targets);
}
