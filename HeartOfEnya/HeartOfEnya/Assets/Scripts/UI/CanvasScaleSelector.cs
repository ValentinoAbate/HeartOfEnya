using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class CanvasScaleSelector : MonoBehaviour
{
    public int x = 16;
    public int y = 9;

    private CanvasScaler scaler;

    private void Awake()
    {
        scaler = GetComponent<CanvasScaler>();
        UpdateScalingMode();
    }
    
    public void UpdateScalingMode()
    {

    }
}
