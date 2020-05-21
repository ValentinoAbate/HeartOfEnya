using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODBattle : MonoBehaviour
{
    public static FMODBattle main;

    public bool InEnemyTurn
    {
        set
        {
            float val = value ? 1 : 0;
            enemyTurn.value = val;
            enemyTurn.TriggerParameters();
        }
    }

    public bool TextPlaying
    {
        set
        {
            float val = value ? 1 : 0;
            textPlaying.value = val;
            textPlaying.TriggerParameters();
        }
    }

    public StudioEventEmitter music;
    public StudioEventEmitter storm;
    [SerializeField]
    private StudioGlobalParameterTrigger textPlaying;
    [SerializeField]
    private StudioGlobalParameterTrigger enemyTurn;

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
