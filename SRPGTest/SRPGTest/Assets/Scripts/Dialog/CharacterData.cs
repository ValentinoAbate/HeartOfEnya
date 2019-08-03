using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newCharaData", menuName = "Character Data")]
public class CharacterData : ScriptableObject
{
    public AudioClip textSrollSfx;
    public PortraitDict portraits;

    [System.Serializable]
    public class PortraitDict : SerializableCollections.SDictionary<string, Sprite> { }
}
