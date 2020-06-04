using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplaySettings : MonoBehaviour
{
    private FullScreenMode mode;
    private int resolutionW = 1920;
    private int resolutionH = 1080;
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

    public void SetResolutionWidth(int w) => resolutionW = w;
    public void SetResolutionHeight(int h) => resolutionH = h;

    public void Apply()
    {
        Screen.SetResolution(resolutionW, resolutionH, mode);
    }
}
