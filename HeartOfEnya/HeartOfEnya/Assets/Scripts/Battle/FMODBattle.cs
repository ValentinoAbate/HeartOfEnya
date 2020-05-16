using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODBattle : MonoBehaviour
{
    public static FMODBattle main;

    public StudioEventEmitter music;
    public StudioEventEmitter storm;

    private void Awake()
    {
        if(main == null)
        {
            main = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
