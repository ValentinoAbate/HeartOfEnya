using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using System;
using FMODUnity;

public class ResolutionSettingsDropdown : MonoBehaviour
{
    public TMP_Dropdown menu;
    public TextMeshProUGUI label;
    public DisplaySettings settings;
    public StudioEventEmitter emitter;

    //stores the list of valid resolutions
    Resolution[] availableResolutions;

    // Start is called before the first frame update
    void Start()
    {
        // //generate a list of resolutions that are smaller or equal to the native resolution
        //availableResolutions = getLegalResolutions();

        // menu.AddOptions(availableResolutions.Select((r) => new TMP_Dropdown.OptionData(r.ToString())).ToList());
        // menu.value = Array.IndexOf(availableResolutions, Screen.currentResolution);
        // //In the unlikely event we somehow removed the current resolution from the list, scream bloody murder.
        // //Usually only happens in the editor window (since it can have a non-standard resolution), but let's just play it safe for the builds.
        // if (Array.Exists(availableResolutions, element => isResEqual(element, Screen.currentResolution))) //all is fine
        // {
        //     label.text = availableResolutions[menu.value].ToString();
        // }
        // else //current resolution somehow got removed
        // {
        //     Debug.LogError("Current resolution is not in the list of available resolutions!");
        //     //If the current resolution got removed, label.text will sometimes default to the lowest resolution.
        //     //It *should* throw an index-out-of-bounds error, but for some reason Array.IndexOf seems to be returning 0 instead of -1 if fed an item not in the array.
        //     //This is probably worrying, but at least it doesn't crash so :shrug:
        //     //Anyway, since builds won't have access to the console to see the error, instead of listing the wrong resolution we'll manually input the "select resolution" text instead.
        //     label.text = "Select Resolution";
        // }

        //use Refresh() to populate the options 
        Refresh();
        menu.onValueChanged.AddListener((val) => label.text = availableResolutions[val].ToString());
        menu.onValueChanged.AddListener((val) => emitter.Play());
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

    //re-generates the options list and currently selected option
    public void Refresh()
    {
        Debug.Log("Refreshing resolution options...");

        //remove previous options list
        menu.ClearOptions();

        //generate a list of resolutions that are smaller or equal to the native resolution
        availableResolutions = getLegalResolutions();

        //populate the options list and set the current resolution as the selection
        menu.AddOptions(availableResolutions.Select((r) => new TMP_Dropdown.OptionData(r.ToString())).ToList());
        menu.value = Array.IndexOf(availableResolutions, Screen.currentResolution);
        //In the unlikely event we somehow removed the current resolution from the list, scream bloody murder.
        //Usually only happens in the editor window (since it can have a non-standard resolution), but let's just play it safe for the builds.
        if (Array.Exists(availableResolutions, element => isResEqual(element, Screen.currentResolution))) //all is fine
        {
            label.text = availableResolutions[menu.value].ToString();
        }
        else //current resolution somehow got removed
        {
            Debug.LogError("Current resolution (" + Screen.currentResolution.ToString() + ") is not in the list of available resolutions!");
            //If the current resolution got removed, label.text will sometimes default to the lowest resolution.
            //It *should* throw an index-out-of-bounds error, but for some reason Array.IndexOf seems to be returning 0 instead of -1 if fed an item not in the array.
            //This is probably worrying, but at least it doesn't crash so :shrug:
            //Anyway, since builds won't have access to the console to see the error, instead of listing the wrong resolution we'll manually input the "select resolution" text instead.
            label.text = "Select Resolution";
        }
    }
}
