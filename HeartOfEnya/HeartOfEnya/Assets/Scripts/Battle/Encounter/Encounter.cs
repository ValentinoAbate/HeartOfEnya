using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="newEncounter", menuName ="Encounter")]
public class Encounter : ScriptableObject
{
    public List<WaveData> waves;
}
