using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using System;

public class ResolutionSettingsDropdown : MonoBehaviour
{
    public TMP_Dropdown menu;
    public TextMeshProUGUI label;
    public DisplaySettings settings;
    // Start is called before the first frame update
    void Start()
    {
        //generate a list of resolutions that are smaller or equal to the native resolution
        Resolution[] availableResolutions = getLegalResolutions();

        menu.AddOptions(availableResolutions.Select((r) => new TMP_Dropdown.OptionData(r.ToString())).ToList());
        menu.value = Array.IndexOf(availableResolutions, Screen.currentResolution);
        //in the unlikely event we somehow removed the current resolution from the list, scream bloody murder
        if (Array.Exists(availableResolutions, element => isResEqual(element, Screen.currentResolution))) //all is fine
        {
            label.text = availableResolutions[menu.value].ToString();
        }
        else //current resolution somehow got removed
        {
            Debug.LogError("Current resolution is not in the list of available resolutions!");
            label.text = "!!UNKNOWN RESOLUTION!!"; //since I can only properly test this system in builds (where the console doesn't show), this is the easiest way to indicate this error in-game.
        }
        //label.text = availableResolutions[menu.value].ToString();
        menu.onValueChanged.AddListener((val) => label.text = availableResolutions[val].ToString());
    }

    //goes through the list of available resolutions and removes any larger than the native resolution
    Resolution[] getLegalResolutions()
    {
        //get list of all available resolutions
        Resolution[] availableRes = Screen.resolutions;

        //Get the native resolution so we can remove everything above it.
        //We'll use the systemWidth/systemHeight properties of the Display class, since unlike Screen.currentResolution they remain constant regardless of any changes to the display or resolution settings.
        Resolution native = new Resolution();       //This refuses to compile if fed arguments and I can't find any documentation of a constructor...
        native.width = Display.main.systemWidth;    //..so we'll just use the default 0x0 @ 0Hz and manually adjust the width and height.
        native.height = Display.main.systemHeight;  //We can leave the refresh rate at 0, since we're not filtering based on it (and the native refresh rate is harder to find).
        Debug.Log("NATIVE RES: " + native.ToString());

        //go through the available resolutions and toss out everything higher than the native resolution
        //Debug.Log("PRE-FILTERING:");
        //printResArray(availableRes);
        availableRes = Array.FindAll(availableRes, x => (x.width <= native.width && x.height <= native.height));
        //Debug.Log("POST-FILTERING:");
        //printResArray(availableRes);

        return availableRes;
    }

    //helper method for debugging, prints the contents of an array of Resolutions
    void printResArray(Resolution[] arr)
    {
        string output = "";
        foreach (Resolution r in arr)
        {
            output += r.ToString();
            output += "\n";
        }
        Debug.Log(output);
    }

    //helper method for testing Resolution equality
    bool isResEqual(Resolution a, Resolution b)
    {
        if ((a.width == b.width) && (a.height == b.height) && (a.refreshRate == b.refreshRate))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
