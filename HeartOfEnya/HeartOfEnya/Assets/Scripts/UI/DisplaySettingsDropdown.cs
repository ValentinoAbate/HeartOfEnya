using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using System;
using FMODUnity;

public class DisplaySettingsDropdown : MonoBehaviour
{
    public TMP_Dropdown menu;
    public TextMeshProUGUI label;
    public StudioEventEmitter emitter;
    public ResolutionSettingsDropdown resolutionMenu; //reference to the resolution dropdown so we can refresh it on mode change

    //Used to convert the chosen display mode into a string form since we can't convert the choice index directly to and from the FullScreenMode enum anymore.
    //Entries should match the options as they appear in the options dropdown, including their order.
    public string[] optionDisplayText;

    // Start is called before the first frame update
    void Start()
    {
        menu.value = ConvertToIndex((int)Screen.fullScreenMode);
        label.text = optionDisplayText[menu.value];
        menu.onValueChanged.AddListener((val) => label.text = optionDisplayText[val]);
        menu.onValueChanged.AddListener((val) => emitter.Play());
        //Have the resolution dropdown refresh its option list when the display mode changes.
        //This should help with a bug on multiple monitor setups where moving the game to a different monitor causes the resolution menu
        //to use the resolution list from the old monitor, which results in weird behavior.
        menu.onValueChanged.AddListener((val) => resolutionMenu.Refresh());
    }

    //quick conversion between the FullScreenMode enum values and the option index values
    //WARNING: WILL BREAK IF WE CHANGE THE ORDER OF THE OPTIONS IN THE DROPDOWN!!
    public int ConvertToIndex(int enumVal)
    {
        if (enumVal == 1) //1 = FullScreen Window
        {
            return 0;
        }
        else //3 = Windowed
        {
            return 1;
        }
    }
}
