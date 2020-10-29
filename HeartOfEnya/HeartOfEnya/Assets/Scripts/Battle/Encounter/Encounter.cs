using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="newEncounter", menuName ="Encounter")]
public class Encounter : ScriptableObject
{
    [Tooltip("The number of waves to go back when the end of the encounter is reached")]
    public int repeatLastWaves = 1;
    public WaveData[] Waves { get => waveList.ToArray(); }

    public List<WaveData> waveList;
}
