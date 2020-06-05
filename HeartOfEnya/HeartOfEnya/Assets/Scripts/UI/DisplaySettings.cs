using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplaySettings : MonoBehaviour
{
    private FullScreenMode mode;
    private Resolution resolution;
    public TextMeshProUGUI selectorText;

    private void Awake()
    {
        resolution = Screen.currentResolution;
        selectorText.text = resolution.ToString();
    }

    public void SetFullScreenModeWindowed()
    {
        mode = FullScreenMode.Windowed;
        Apply();
    }

    public void SetFullScreenModeWindowedMax()
    {
        mode = FullScreenMode.MaximizedWindow;
        Apply();
    }

    public void SetFullScreenModeFullScreen()
    {
        mode = FullScreenMode.FullScreenWindow;
        Apply();
    }

    public void NextResolution()
    {
        int ind = Array.IndexOf(Screen.resolutions, resolution);
        if(++ind >= Screen.resolutions.Length)
            ind = 0;
            resolution = Screen.resolutions[ind];
        selectorText.text = resolution.ToString();
    }

    public void Apply()
    {
        Screen.SetResolution(resolution.width, resolution.height, mode);
    }
}
