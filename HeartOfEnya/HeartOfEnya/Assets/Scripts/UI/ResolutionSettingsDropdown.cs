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
        menu.AddOptions(Screen.resolutions.Select((r) => new TMP_Dropdown.OptionData(r.ToString())).ToList());
        menu.value = Array.IndexOf(Screen.resolutions, Screen.currentResolution);
        label.text = Screen.resolutions[menu.value].ToString();
        menu.onValueChanged.AddListener((val) => label.text = Screen.resolutions[val].ToString());
    }
}
