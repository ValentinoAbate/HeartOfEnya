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
        if (scaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
            return;
        float intendedScale = (float)x / y;
        float currScale = (float)Screen.width / Screen.height;
        if(currScale <= intendedScale)
        {
            // Match Width
            scaler.matchWidthOrHeight = 0;
        }
        else
        {
            // Match Height
            scaler.matchWidthOrHeight = 1;
        }
    }
}
