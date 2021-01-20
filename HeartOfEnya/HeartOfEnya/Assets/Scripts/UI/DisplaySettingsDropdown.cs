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
