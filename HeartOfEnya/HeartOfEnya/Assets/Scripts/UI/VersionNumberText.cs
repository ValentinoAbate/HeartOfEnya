using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VersionNumberText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private void Awake()
    {
        text.text = "Version " + Application.version;
    }
}
