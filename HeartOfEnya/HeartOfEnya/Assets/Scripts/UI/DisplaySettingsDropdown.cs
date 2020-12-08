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

    // Start is called before the first frame update
    void Start()
    {
        menu.value = (int)Screen.fullScreenMode;
        label.text = ((FullScreenMode)menu.value).ToString();
        menu.onValueChanged.AddListener((val) => label.text = ((FullScreenMode)val).ToString());
        menu.onValueChanged.AddListener((val) => emitter.Play());
    }
}
