using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxPlayerDispatcher : MonoBehaviour
{
    public SfxPlayer sfxPlayerPrefab;
    public SfxPlayer Dispatch()
    {
        return Instantiate(sfxPlayerPrefab).GetComponent<SfxPlayer>();
    }
}
