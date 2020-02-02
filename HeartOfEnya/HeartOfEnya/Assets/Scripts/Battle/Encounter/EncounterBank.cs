using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="newEncounterBank", menuName ="Encounter Bank")]
public class EncounterBank : ScriptableObject
{
    public List<WaveData> waves;
}
