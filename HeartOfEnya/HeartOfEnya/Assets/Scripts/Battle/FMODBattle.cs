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

    [Header("Music")]
    [SerializeField]
    private StudioEventEmitter music;
    [SerializeField]
    private StudioEventEmitter abs0Music;

    [Header("Ambient")]
    public StudioEventEmitter storm;

    [Header("Effects")]
    [FMODUnity.EventRef]
    public string EnemyTurnTransition;

    [Header("Parameter Triggers")]
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

    public void StartEnemyTurn()
    {
        storm.SetParameter("Enemy Turn", 1);
        FMODUnity.RuntimeManager.PlayOneShot(EnemyTurnTransition);
        InEnemyTurn = true;
    }

    public void EndEnemyTurn()
    {
        storm.SetParameter("Enemy Turn", 0);
        InEnemyTurn = false;
    }

    private void TurnOffNewWave() => NewWave = false;

}
