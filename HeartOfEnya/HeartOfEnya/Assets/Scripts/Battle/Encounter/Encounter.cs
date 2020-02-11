using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Malee;

[CreateAssetMenu(fileName ="newEncounter", menuName ="Encounter")]
public class Encounter : ScriptableObject
{
    public WaveData[] Waves { get => waveList.ToArray(); }

    [Reorderable(singleLine = true, paginate = true)]
    public WaveList waveList;
    [System.Serializable]
    public class WaveList : ReorderableArray<WaveData> { }
}
