using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
[DisallowMultipleComponent]
public class SetRendererLayer : MonoBehaviour
{
    public string sortLayerName;
    public int sortLayerOrder;

    void Awake()
    {
        var r = GetComponent<Renderer>();
        r.sortingLayerID = SortingLayer.NameToID(sortLayerName);
        r.sortingOrder = sortLayerOrder;
    }

}
