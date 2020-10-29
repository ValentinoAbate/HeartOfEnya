using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ActionRange
{
    public enum Type
    { 
        Normal,
        Cardinal,
    }

    public Type type;
    public int min;
    public int max;

    public bool Contains(int val)
    {
        return val <= max && val >= min;
    }

    public override string ToString()
    {
        return min.ToString() + "-" + max.ToString();
    }
}
