using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newScriptData", menuName = "Script Data")]
public class ScriptData : ScriptableObject
{
    public string nodeName;
    [Header("Monolog properties")]
    public bool isMonolog;
    public string musicEvent;
    public string campNode;
}
