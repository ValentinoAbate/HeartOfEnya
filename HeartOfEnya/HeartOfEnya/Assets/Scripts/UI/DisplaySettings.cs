using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class DisplaySettings : MonoBehaviour
{
    public FullScreenMode mode;
    private Resolution resolution;
    public TextMeshProUGUI selectorText;
    public TMP_Dropdown displayDropDown;
    public TMP_Dropdown resolutionDropDown;
    public ResolutionSettingsDropdown resolutionMenu; //reference to the resolution dropdown so we can refresh it on mode change
    public Toggle vSyncToggle;


    private void Awake()
    {
        resolution = Screen.currentResolution;
        resolutionDropDown.value = Array.IndexOf(Screen.resolutions.ToArray(), resolution);
        mode = Screen.fullScreenMode;
        displayDropDown.value = (int)mode;
        selectorText.text = resolution.ToString();
        vSyncToggle.isOn = QualitySettings.vSyncCount > 0;
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
        resolution = resolutionMenu.availableResolutions[ind];
        selectorText.text = resolution.ToString();
    }

    public void SetFullScreenMode(int mode)
    {
        //convert the option's list index into an actual usable value & use it to set the display mode.
        //WARNING: WILL BREAK IF WE CHANGE THE ORDER OF THE OPTIONS IN THE DROPDOWN!!
        switch (mode)
        {
            case 0:
                this.mode = FullScreenMode.FullScreenWindow;
                break;
            case 1:
                this.mode = FullScreenMode.Windowed;
                break;
            default:
                Debug.LogError("Invalid display mode choice!");
                break;
        }
    }

    public void Apply()
    {
        Screen.SetResolution(resolution.width, resolution.height, mode);
        QualitySettings.vSyncCount = vSyncToggle.isOn ? 1 : 0;
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
        //Waiting until the screen hits the new resolution *SHOULD* work, but in reality this WaitUntil never executes.
        //yield return new WaitUntil(() => Screen.width == resolution.width && Screen.height == resolution.height);

        //Instead of waiting until we've confirmed the resolution change, let's just wait 4 frames and pray Unity sorts it out itself.
        yield return null; //waits until the end of the current frame
        yield return null; //waits until the end of the next frame (when the docs say the change SHOULD take effect)
        yield return null; //wait 2 more frames just in case Unity is slow and/or the docs are lying again
        yield return null;
        UpdateScalingSettings();
        //Have the resolution dropdown refresh its option list when the display mode changes.
        //This should help with a bug on multiple monitor setups where moving the game to a different monitor causes the resolution menu
        //to use the resolution list from the old monitor, which results in weird behavior.
        resolutionMenu.Refresh();
    }
}
