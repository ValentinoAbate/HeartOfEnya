using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using System;

public class DisplaySettingsDropdown : MonoBehaviour
{
    public TMP_Dropdown menu;
    public DisplaySettings settings;
    // Start is called before the first frame update
    void Start()
    {
        menu.AddOptions(Screen.resolutions.Where((r) => r.width / 16 == r.height / 9).Select((r) => new TMP_Dropdown.OptionData(r.ToString())).ToList());
        menu.value = Array.IndexOf(Screen.resolutions, Screen.currentResolution);
    }
}
