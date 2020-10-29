using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class DisplaySettings : MonoBehaviour
{
    public FullScreenMode mode;
    private Resolution resolution;
    public TextMeshProUGUI selectorText;
    public TMP_Dropdown displayDropDown;
    public TMP_Dropdown resolutionDropDown;

    private void Awake()
    {
        resolution = Screen.currentResolution;
        resolutionDropDown.value = Array.IndexOf(Screen.resolutions.ToArray(), resolution);
        mode = Screen.fullScreenMode;
        displayDropDown.value = (int)mode;
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
        int ind = Array.IndexOf(Screen.resolutions.ToArray(), resolution);
        if(++ind >= Screen.resolutions.Length)
            ind = 0;
            resolution = Screen.resolutions[ind];
        selectorText.text = resolution.ToString();
    }

    public void SetResolution(int ind)
    {
        resolution = Screen.resolutions[ind];
        selectorText.text = resolution.ToString();
    }

    public void SetFullScreenMode(int mode)
    {
        this.mode = (FullScreenMode)mode;
    }

    public void Apply()
    {
        Screen.SetResolution(resolution.width, resolution.height, mode);
        StartCoroutine(UpdateScalingSettingsCr());
    }

    private void UpdateScalingSettings()
    {
        Camera.main.GetComponent<LetterBoxer>().PerformSizing();
        foreach (var canvasScaler in GameObject.FindObjectsOfType<CanvasScaleSelector>())
            canvasScaler.UpdateScalingMode();

    }

    private IEnumerator UpdateScalingSettingsCr()
    {
        yield return new WaitUntil(() => Screen.width == resolution.width && Screen.height == resolution.height);
        UpdateScalingSettings();
    }
}
