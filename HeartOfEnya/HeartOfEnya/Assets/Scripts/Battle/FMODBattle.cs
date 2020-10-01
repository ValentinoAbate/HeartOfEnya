using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODBattle : MonoBehaviour
{
    public static FMODBattle main;

    [SerializeField] private float newWaveLength;

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

    public bool NewWave
    {
        set
        {
            float val = value ? 1 : 0;
            newWave.value = val;
            newWave.TriggerParameters();
        }
    }

    public StudioEventEmitter Music
    {
        get
        {
            var pData = DoNotDestroyOnLoad.Instance.persistentData;
            return pData.gamePhase == PersistentData.gamePhaseAbsoluteZeroBattle ? abs0Music : music;
        }
    }

    [SerializeField]
    private StudioEventEmitter music;
    public StudioEventEmitter storm;
    [SerializeField]
    private StudioEventEmitter abs0Music;
    [SerializeField]
    private StudioGlobalParameterTrigger textPlaying;
    [SerializeField]
    private StudioGlobalParameterTrigger enemyTurn;
    [SerializeField]
    private StudioGlobalParameterTrigger newWave;

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

    private void Start()
    {
        Music.Play();
    }

    public void TriggerNewWave()
    {
        NewWave = true;
        Invoke("TurnOffNewWave", newWaveLength);
    }

    private void TurnOffNewWave() => NewWave = false;
}
